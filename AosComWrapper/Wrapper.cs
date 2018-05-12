using AosComMeasurer;
using NLog;
using System;
using System.Runtime.InteropServices;

namespace AosComWrapper
{
    public class Wrapper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private delegate void OnConnectDelegate(string measurerSerial, string boardSerial);
        private delegate void OnDisconnectDelegate();
        private delegate void OnStateChangedDelegate(int plus, int minus);

        private static OnConnectDelegate onConnectDelegate;
        private static OnDisconnectDelegate onDisconnectDelegate;
        private static OnStateChangedDelegate onStateChangedDelegate;

        public static Controller controller; 

        #region events
        private static void Controller_DisconnectMeasurer(object sender, EventArgs e)
        {
            onDisconnectDelegate?.Invoke();
        }

        private static void Controller_ConnectMeasurer(object sender, GetSerialsEventArgs e)
        {
            onConnectDelegate?.Invoke(e.Measurer, e.Board);
        }

        private static void Controller_ChangedProbePoints(object sender, ProbeEventArgs e)
        {
            onStateChangedDelegate?.Invoke(e.Plus, e.Minus);
        }
        #endregion events

        [DllExport(CallingConvention.Cdecl)]
        public static void Init()
        {
            controller = Controller.Instance;
            controller.Init();
            controller.ChangedProbePoints += Controller_ChangedProbePoints;
            controller.ConnectMeasurer += Controller_ConnectMeasurer;
            controller.DisconnectMeasurer += Controller_DisconnectMeasurer;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void InitMeasurer(string pathToBase)
        {
            logger.Info("Init measurer with path = {0}", pathToBase);
            controller?.InitializeBase(pathToBase);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static byte ShowMeasurer(byte bShow)
        {
            return controller.ShowMeasurer(bShow);
        }


        [DllExport(CallingConvention.Cdecl)]
        public static byte SetLamp(int lamp, byte state)
        {
            return controller.SetLamp(lamp, state);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void PlayMSound(int track)
        {
            controller.PlaySound(track);
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void SetAngle(double angle)
        {
            //TODO implement
        }

        [DllExport(CallingConvention.Cdecl)]
        public static void SetValue(double value)
        {
            //TODO implement
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int GetFirmware()
        {
            //TODO implement
            return 0;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int UpdateFirmware(string hexFullName)
        {
            //TODO implement
            return 0;
        }

        [DllExport(CallingConvention.Cdecl)]
        public static int SetMeasure(double acI, double dcI, double acU, double dcU, double ohm)
        {
            return controller.SetMeasure(acI, dcI, acU, dcU, ohm);
        }

        [DllExport("OnConnectCallbackFunction", CallingConvention.StdCall)]
        public static bool OnConnectCallbackFunction(IntPtr callback)
        {
            onConnectDelegate = (OnConnectDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnConnectDelegate));
            return true;
        }

        [DllExport("OnDisconnectCallbackFunction", CallingConvention.StdCall)]
        public static bool OnDisconnectCallbackFunction(IntPtr callback)
        {
            onDisconnectDelegate = (OnDisconnectDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnDisconnectDelegate));
            return true;
        }

        [DllExport("OnStateChangedCallbackFunction", CallingConvention.StdCall)]
        public static bool OnStateChangedCallbackFunction(IntPtr callback)
        {
            onStateChangedDelegate = (OnStateChangedDelegate)Marshal.GetDelegateForFunctionPointer(callback, typeof(OnStateChangedDelegate));
            return true;
        }
    }
}
