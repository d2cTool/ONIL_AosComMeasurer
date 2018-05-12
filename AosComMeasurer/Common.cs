using System;

namespace AosComMeasurer
{
    public class MeasurerConnectEventArgs : EventArgs
    {
        public string MesurerSerial { get; private set; }
        public string BoardSerial { get; private set; }
        public MeasurerState State { get; private set; }
        public MeasurerConnectEventArgs(string m_serial, string b_serial, MeasurerState state) { MesurerSerial = m_serial; BoardSerial = b_serial; State = state; }
    }

    public class MeasurerChangeStateEventArgs : EventArgs
    {
        public string Type { get; private set; }
        public string Limit { get; private set; }
        public string Btn { get; private set; }
        public MeasurerChangeStateEventArgs(string type, string limit, string btn) { Type = type; Limit = limit; Btn = btn; }
    }

    public class StrDataEventArgs : EventArgs
    {
        public string Data { get; private set; }
        public StrDataEventArgs(string data) { Data = data; }
    }

    public class IntDataEventArgs : EventArgs
    {
        public int Data { get; private set; }
        public IntDataEventArgs(int data) { Data = data; }
    }

    public class ProbeEventArgs : EventArgs
    {
        public int Plus { get; private set; }
        public int Minus { get; private set; }
        public ProbeEventArgs(int plus, int minus) { Plus = plus; Minus = minus; }
    }
    public class GetSerialsEventArgs : EventArgs
    {
        public string Measurer { get; private set; }
        public string Board { get; private set; }
        public GetSerialsEventArgs(string measurer, string board) { Measurer = measurer; Board = board; }
    }

    public static class Common
    {
    }
}
