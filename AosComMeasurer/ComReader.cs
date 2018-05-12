using NLog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace AosComMeasurer
{
    public sealed class ComReader
    {
        private static volatile ComReader instance;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Thread thrd;
        private SerialPort com;
        private volatile bool _shouldStop;

        public string Port { get; private set; }

        private ComReader() { }
        public static ComReader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComReader();
                }
                return instance;
            }
        }

        public bool Init()
        {
            List<COMDeviceInfo> deviceList = null;
            try
            {
                if (com != null && com.IsOpen)
                    com.Close();

                deviceList = COMDeviceManager.GetComDevices();

                if (deviceList.Count < 1)
                {
                    Port = string.Empty;
                    return false;
                }

                Port = deviceList[0].Port;

                if (Port == string.Empty)
                {
                    return false;
                }

                com = new SerialPort(Port, 115200, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    RtsEnable = false,
                    DtrEnable = false,
                    NewLine = "\r\n"
                };
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, string.Format("port: {0}, deviceListCount: {1}", Port, deviceList?.Count));
                return false;
            }
        }

        public void Start()
        {
            try
            {
                if (com != null)
                {
                    _shouldStop = false;

                    if (!com.IsOpen)
                    {
                        com.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                        com.Open();
                    }

                    thrd = new Thread(Loop);
                    thrd.Start();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "port: {0}", com.PortName);
                LostConnection?.Invoke(this, new StrDataEventArgs("Can't open com port."));
            }
        }

        public event EventHandler<StrDataEventArgs> DataReceived;
        public event EventHandler<StrDataEventArgs> LostConnection;

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs ev)
        {
            try
            {
                SerialPort serial = (SerialPort)sender;
                string indata = serial.ReadLine();
                DataReceived?.Invoke(this, new StrDataEventArgs(indata));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DataReceivedHandler.");
                LostConnection?.Invoke(this, new StrDataEventArgs("Can't read data."));
                Stop();
            }
        }

        public void Stop()
        {
            _shouldStop = true;

            try
            {
                com?.Close();
                thrd?.Join();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Can't stop listening");
            }
        }

        private void Loop()
        {
            while (!_shouldStop)
            {
                try
                {
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    LostConnection?.Invoke(this, new StrDataEventArgs("Error in main loop."));
                    Stop();
                }
            }
        }

        public void Send(string data)
        {
            try
            {
                if (com.IsOpen)
                {
                    com.Write(data);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Can't send: {0}", data);
                LostConnection?.Invoke(this, new StrDataEventArgs("Can't write data."));
                Stop();
            }
        }

    }
}
