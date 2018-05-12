using NLog;
using System;
using System.Timers;

namespace AosComMeasurer
{
    public sealed class ComWorker
    {
        private static volatile ComWorker instance;
        private Timer poolingTimer;
        private Timer reconnectTimer;

        private BoardState BoardState { get; set; }
        private MeasurerState MeasurerState { get; set; }

        private ComReader comReader;
        private ComMsgParser msgParser;

        public event EventHandler<MeasurerConnectEventArgs> ConnectMeasurer;
        public event EventHandler DisconnectMeasurer;

        public event EventHandler<GetSerialsEventArgs> ConnectBoard;
        public event EventHandler DisconnectBoard;

        public event EventHandler<ProbeEventArgs> ChangedProbePoints;
        public event EventHandler<MeasurerChangeStateEventArgs> ChangeMeasurerState;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ComWorker()
        {
            poolingTimer = new Timer(5000);
            poolingTimer.Elapsed += PoolingTimer_Elapsed;
            poolingTimer.Enabled = false;

            reconnectTimer = new Timer(50000);
            reconnectTimer.Elapsed += ReconnectTimer_Elapsed;
            reconnectTimer.Enabled = false;

            BoardState = new BoardState();
            MeasurerState = new MeasurerState();

            msgParser = new ComMsgParser();
            msgParser.ConnectMeasurer += MsgParser_ConnectMeasurer;

            comReader = ComReader.Instance;
            comReader.DataReceived += ComReader_DataReceived;
            comReader.LostConnection += ComReader_LostConnection;
        }

        private void ReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (InitComReader())
                reconnectTimer.Enabled = false;
        }

        public static ComWorker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComWorker();
                }
                return instance;
            }
        }

        public bool InitComReader()
        {
            if (comReader.Init())
            {
                comReader.Start();
                InitializeCmds();
                return true;
            }
            return false;
        }

        public void StopComReader()
        {
            comReader.Stop();
        }

        #region events
        private void PoolingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Send("n");
        }

        private void MsgParser_ConnectMeasurer(object sender, GetSerialsEventArgs e)
        {
            poolingTimer.Enabled = true;
            ConnectMeasurer?.Invoke(this, new MeasurerConnectEventArgs(e.Measurer, e.Board, MeasurerState));

            msgParser.ChangedProbePoints += MsgParser_ChangedProbePoints;
            msgParser.ChangeBtn += MsgParser_ChangeBtn;
            msgParser.ChangeFirmwareVersion += MsgParser_ChangeFirmwareVersion;
            msgParser.ChangeLimit += MsgParser_ChangeLimit;
            msgParser.ChangeType += MsgParser_ChangeType;

        }

        private void MsgParser_ChangeType(object sender, StrDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MsgParser_ChangeLimit(object sender, StrDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MsgParser_ChangeFirmwareVersion(object sender, IntDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MsgParser_ChangeBtn(object sender, StrDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MsgParser_ChangedProbePoints(object sender, ProbeEventArgs e)
        {
            ChangedProbePoints?.Invoke(this, e);
        }

        private void ComReader_LostConnection(object sender, EventArgs e)
        {
            MeasurerState = new MeasurerState();
            BoardState = new BoardState();

            DisconnectMeasurer?.Invoke(this, e);

            poolingTimer.Enabled = false;
            reconnectTimer.Enabled = true;

            msgParser.ChangedProbePoints -= MsgParser_ChangedProbePoints;
            msgParser.ChangeBtn -= MsgParser_ChangeBtn;
            msgParser.ChangeFirmwareVersion -= MsgParser_ChangeFirmwareVersion;
            msgParser.ChangeLimit -= MsgParser_ChangeLimit;
            msgParser.ChangeType -= MsgParser_ChangeType;
        }

        private void ComReader_DataReceived(object sender, StrDataEventArgs e)
        {
            try
            {
                if (!msgParser.Parse(e.Data, MeasurerState, BoardState))
                {
                    logger.Error("Cant parse: {0}", e.Data);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Cant parse: {0}", e.Data);
            }
        }
        #endregion events

        private void InitializeCmds()
        {
            comReader?.Send("h13");
            comReader?.Send("v0");
            comReader?.Send("e");
            comReader?.Send("n");
        }

        public void Send(string msg)
        {
            comReader?.Send(msg);
        }
    }
}
