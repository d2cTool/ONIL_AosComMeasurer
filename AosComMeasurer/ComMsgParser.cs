using NLog;
using System;
using System.Text.RegularExpressions;

namespace AosComMeasurer
{
    public class ComMsgParser
    {
        public event EventHandler<StrDataEventArgs> ConnectBoard;
        public event EventHandler DisconnectBoard;

        public event EventHandler<GetSerialsEventArgs> ConnectMeasurer;

        public event EventHandler<ProbeEventArgs> ChangedProbePoints;

        public event EventHandler<StrDataEventArgs> ChangeType;
        public event EventHandler<StrDataEventArgs> ChangeLimit;
        public event EventHandler<StrDataEventArgs> ChangeBtn;

        public event EventHandler<IntDataEventArgs> ChangeFirmwareVersion;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public bool Parse(string msg, MeasurerState measurerState, BoardState boardState)
        {
            //MiniMaket ver180313. Speed 115200. Input n[umber],p[lay]xxx,s[top],v[al]xxx,l[ow]x,h[igh]x and integer. Output m[inus],p[lus],s[elector],d[istance],b[utton],i[ndicator],v[cc]=4.70
            Regex welcome = new Regex(@"^MiniMaket ver(\d+). Speed 115200. Input n\[umber\],p\[lay\]xxx,s\[top\],v\[al\]xxx,l\[ow\]x,h\[igh\]x and integer. Output m\[inus\],p\[lus\],s\[elector\],d\[istance\],b\[utton\],i\[ndicator\],v\[cc\]=(\S+)$");

            //m0 p0 s0 d48 b0 v0 i0000000000000000 ok
            Regex state = new Regex(@"^m(\d+) p(\d+) s(\d+) d(\d+) b(\d+) v(\d+) i(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d)(\d) ok$");

            //n28FFBC6BA1160506 n28FFCEFAB21603F5
            Regex serials = new Regex(@"n([0-9A-F]{16})");

            //e180428
            Regex version = new Regex(@"^e(\d+)");

            Match match = welcome.Match(msg);
            if (match.Success)
            {
                measurerState.FirmwareVersion = int.Parse(match.Groups[1].Value);
                measurerState.Vcc = match.Groups[2].Value;
                return true;
            }

            match = state.Match(msg);
            if (match.Success)
            {
                int Minus = int.Parse(match.Groups[1].Value);
                int Plus = int.Parse(match.Groups[2].Value);

                string Type = match.Groups[3].Value;
                string Limit = match.Groups[4].Value;
                string Btn = match.Groups[5].Value;
                string Value = match.Groups[6].Value;

                byte I0 = byte.Parse(match.Groups[7].Value);
                byte I1 = byte.Parse(match.Groups[8].Value);
                byte I2 = byte.Parse(match.Groups[9].Value);
                byte I3 = byte.Parse(match.Groups[10].Value);
                byte I4 = byte.Parse(match.Groups[11].Value);
                byte I5 = byte.Parse(match.Groups[12].Value);
                byte I6 = byte.Parse(match.Groups[13].Value);
                byte I7 = byte.Parse(match.Groups[14].Value);
                byte I8 = byte.Parse(match.Groups[15].Value);
                byte I9 = byte.Parse(match.Groups[16].Value);
                byte I10 = byte.Parse(match.Groups[17].Value);
                byte I11 = byte.Parse(match.Groups[18].Value);
                byte I12 = byte.Parse(match.Groups[19].Value);
                byte I13 = byte.Parse(match.Groups[20].Value);
                byte I14 = byte.Parse(match.Groups[21].Value);
                byte I15 = byte.Parse(match.Groups[22].Value);

                if (Minus != boardState.Minus || Plus != boardState.Plus)
                {
                    ChangedProbePoints?.Invoke(this, new ProbeEventArgs(Plus, Minus));
                }

                boardState.Minus = Minus;
                boardState.Plus = Plus;

                if(Type != measurerState.Type)
                {
                    ChangeType?.Invoke(this, new StrDataEventArgs(Type));
                }

                if (Limit != measurerState.Limit)
                {
                    ChangeLimit?.Invoke(this, new StrDataEventArgs(Limit));
                }

                if (Btn != measurerState.Btn)
                {
                    ChangeBtn?.Invoke(this, new StrDataEventArgs(Btn));
                }

                if (measurerState.Serial != null && measurerState.FirmwareVersion != 0 && Limit != null)
                {
                    ConnectMeasurer?.Invoke(this, new GetSerialsEventArgs(measurerState.Serial, boardState.Serial));
                }

                measurerState.Type = Type;
                measurerState.Limit = Limit;
                measurerState.Btn = Btn;
                measurerState.Value = Value;

                measurerState.I0 = I0;
                measurerState.I1 = I1;
                measurerState.I2 = I2;
                measurerState.I3 = I3;
                measurerState.I4 = I4;
                measurerState.I5 = I5;
                measurerState.I6 = I6;
                measurerState.I7 = I7;
                measurerState.I8 = I8;
                measurerState.I9 = I9;
                measurerState.I10 = I10;
                measurerState.I11 = I11;
                measurerState.I12 = I12;
                measurerState.I13 = I13;
                measurerState.I14 = I14;
                measurerState.I15 = I15;

                return true;
            }

            MatchCollection matches = serials.Matches(msg);
            if (matches.Count > 0)
            {
                string measurerSerial = matches[0].Groups[1].Value;
                string boardSerial = (matches.Count > 1) ? matches[1].Groups[1].Value : string.Empty;

                if (measurerState.FirmwareVersion != 0 && measurerState.Limit != null)
                {

                    if (boardState.Serial != null && boardState.Serial != string.Empty && boardSerial == string.Empty)
                        DisconnectBoard?.Invoke(this, new EventArgs());

                    if (boardSerial != string.Empty && boardState.Serial != boardSerial)
                        ConnectBoard?.Invoke(this, new StrDataEventArgs(boardSerial));

                    if (measurerState.Serial != measurerSerial)
                        ConnectMeasurer?.Invoke(this, new GetSerialsEventArgs(measurerSerial, boardSerial));
                }

                measurerState.Serial = measurerSerial;
                boardState.Serial = boardSerial;

                return true;
            }

            match = version.Match(msg);
            if (match.Success)
            {
                int firmwareVersion = int.Parse(match.Groups[1].Value);

                if (firmwareVersion != measurerState.FirmwareVersion)
                    ChangeFirmwareVersion?.Invoke(this, new IntDataEventArgs(firmwareVersion));

                if (firmwareVersion != measurerState.FirmwareVersion && measurerState.Limit != null && measurerState.Serial != string.Empty && measurerState.Serial != null)
                    ConnectMeasurer?.Invoke(this, new GetSerialsEventArgs(measurerState.Serial, boardState.Serial));

                measurerState.FirmwareVersion = firmwareVersion;

                return true;
            }

            return false;
        }
    }
}
