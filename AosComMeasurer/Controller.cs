using MDevice;
using NLog;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using WpfMeasurerView;

namespace AosComMeasurer
{
    public sealed class Controller
    {
        private static volatile Controller instance;

        private ComWorker comWorker;
        private Window measurerWindow = null;
        private MDevicesBase mDevicesBase;
        private Device device;
        private byte showWindow = 0;
        private string measurerType;
        private string measurerSerial;
        private MeasurerState measurerState;
        private bool uiActive;

        private int plus;
        private int minus;
        public int FirmwareVersion { get; private set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly CultureInfo cultureInfo = new CultureInfo("en");

        private Thread uiThread;

        public event EventHandler<ProbeEventArgs> ChangedProbePoints;
        public event EventHandler DisconnectMeasurer;
        public event EventHandler<GetSerialsEventArgs> ConnectMeasurer;

        private Controller()
        {
            comWorker = ComWorker.Instance;
            comWorker.ConnectMeasurer += ComWorker_ConnectMeasurer;
            comWorker.DisconnectMeasurer += ComWorker_DisconnectMeasurer;
        }

        public void Init()
        {
            comWorker?.InitComReader();
        }

        public static Controller Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Controller();
                }
                return instance;
            }
        }

        private void ComWorker_DisconnectMeasurer(object sender, EventArgs e)
        {
            comWorker.ChangedProbePoints -= ComWorker_ChangedProbePoints;
            comWorker.ChangeMeasurerState -= ComWorker_ChangeMeasurerState;

            ActivateUiWindow();
            //TODO measurerWindow: show and set active
            DisconnectMeasurer?.Invoke(this, e);
        }

        private void ComWorker_ChangedProbePoints(object sender, ProbeEventArgs e)
        {
            ChangedProbePoints?.Invoke(this, e);
        }

        private void ComWorker_ConnectMeasurer(object sender, MeasurerConnectEventArgs e)
        {
            measurerSerial = e.MesurerSerial;
            measurerState = e.State;

            if (uiThread != null)
                DeactivateUiWindow();

            ConnectMeasurer?.Invoke(this, new GetSerialsEventArgs(e.MesurerSerial, e.BoardSerial));
        }

        public void InitializeBase(string pathToBase)
        {
            logger.Info("");
            mDevicesBase = new MDevicesBase(pathToBase);

            string type = mDevicesBase.GetType(measurerSerial);
            device = new Device(mDevicesBase.GetDevice(measurerSerial), mDevicesBase.GetScalesBySerial(measurerSerial));

            switch (type)
            {
                case "c4352m1":

                    uiThread = new Thread(new ThreadStart(() =>
                    {
                        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                        c4352m1_window window = new c4352m1_window();
                        measurerWindow = window;

                        window.Closed += (s, e) => Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                        window.Show();
                        //window.Visibility = Visibility.Collapsed;
                        Dispatcher.Run();
                    }));

                    uiThread.SetApartmentState(ApartmentState.STA);
                    uiThread.IsBackground = true;

                    uiThread.Start();
                    break;
                default:
                    break;
            }

            //device.SetMesurerState(measurerState.Limit, measurerState.Type, measurerState.Btn);

            comWorker.ChangedProbePoints += ComWorker_ChangedProbePoints;
            comWorker.ChangeMeasurerState += ComWorker_ChangeMeasurerState;
        }

        private void ComWorker_ChangeMeasurerState(object sender, MeasurerChangeStateEventArgs e)
        {
            var type = device?.SetType(e.Type);
            var limit = double.Parse(device?.SetLimit(e.Limit), cultureInfo);
            var btn = device?.SetBtn(e.Btn);

            UpdateUi(type, limit, btn);

            ChangedProbePoints?.Invoke(this, new ProbeEventArgs(plus, minus));
        }

        public int SetMeasure(double dcI, double acI, double dcU, double acU, double ohm)
        {
            int rez = 0x50;
            if (device != null)
            {
                device.SetValues(dcI, acI, dcU, acU, ohm);
                rez = device.GetArrowAngle(out int hwAngle, out double angle, out bool isBurn);

                if (isBurn)
                    comWorker.Send("h14");

                MoveArrow(angle);
                comWorker.Send($"v{hwAngle}");
            }

            return rez;
        }

        public byte ShowMeasurer(byte state)
        {
            logger.Info("");
            if (state == 1)
            {
                ShowWindow();
            }
            else
            {
                HideWindow();
            }
            return state;
        }

        public byte SetLamp(int index, byte state)
        {
            string command = (state == 1) ? "h" : "l";
            comWorker?.Send($"{command}{index}");
            return state;
        }

        public void PlaySound(int fileId)
        {
            comWorker?.Send($"p{fileId}");
        }

        private void HideWindow()
        {
            logger.Info("");
            Dispatcher.FromThread(uiThread).Invoke(new Action(() =>
            {
                if (measurerWindow != null)
                    measurerWindow.Visibility = Visibility.Collapsed;
            }));
        }

        private void ShowWindow()
        {
            logger.Info("");
            Dispatcher.FromThread(uiThread).Invoke(new Action(() =>
            {
                if (measurerWindow != null)
                {
                    measurerWindow.Visibility = Visibility.Visible;

                }
            }));
        }

        private void ActivateUiWindow()
        {
            logger.Info("");
            Dispatcher.FromThread(uiThread).Invoke(new Action(() =>
            {
                if (measurerWindow != null)
                    (measurerWindow as c4352m1_window).isActive = true;
            }));
        }

        private void DeactivateUiWindow()
        {
            Dispatcher.FromThread(uiThread).Invoke(new Action(() =>
            {
                if (measurerWindow != null)
                    (measurerWindow as c4352m1_window).isActive = false;
            }));
        }

        private void UpdateUi(string Type, double limit, string btn)
        {
            Dispatcher.FromThread(uiThread).Invoke(new Action(() =>
            {
                c4352m1_window w = measurerWindow as c4352m1_window;
                w?.UpdateView(Type, limit, btn);
            }));
        }

        private void MoveArrow(double angle)
        {
            Dispatcher.FromThread(uiThread).Invoke(new Action(() =>
            {
                (measurerWindow as c4352m1_window)?.MoveArrow(angle);
            }));
        }

    }
}
