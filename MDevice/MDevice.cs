using NLog;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace MDevice
{
    public class Device
    {
        private double dcI = 0;
        private double acI = 0;
        private double dcU = 0;
        private double acU = 0;
        private double ohm = 0;

        private string Type = string.Empty;
        private double Limit = 0;
        private string Btn = string.Empty;
        private string Group = string.Empty;

        public TLimit TLimit { get; private set; }
        public TType TType { get; private set; }
        public List<Scale> Scales { get; private set; }
        public bool IsBtnPushed { get; private set; }
        public string DeviceType { get; private set; }
        public string MeasuringType { get; private set; }
        private double CurrentValue { get; set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Device(XElement device, XElement scales)
        {
            try
            {
                XElement limitXElmnt = device.Descendants("tumbler").FirstOrDefault(el => el.Attribute("name")?.Value == "limit");
                XElement typeXElmnt = device.Descendants("tumbler").FirstOrDefault(el => el.Attribute("name")?.Value == "type");

                DeviceType = device.Attribute("type").Value;

                TLimit = new TLimit(limitXElmnt);
                TType = new TType(typeXElmnt);

                Scales = new List<Scale>();
                var ss = scales.Descendants("scale");
                foreach (XElement item in ss)
                {
                    XElement sc = device.Descendants("scale").FirstOrDefault(el => el.Attribute("type")?.Value == item.Attribute("type")?.Value);
                    Scales.Add(new Scale(sc, item));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Device type: {0}.", DeviceType);
            }
        }

        public void SetValues(double dcI, double acI, double dcU, double acU, double ohm)
        {
            this.dcI = dcI;
            this.acI = acI;
            this.dcU = dcU;
            this.acU = acU;
            this.ohm = ohm;
        }

        public string SetType(string msg)
        {
            try
            {
                Type = TType.GetValue(msg);
                return TType.GetAngle(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Set type: {0}.", msg);
                return string.Empty;
            }
        }

        public string SetLimit(string msg)
        {
            try
            {
                Group = TLimit.GetGroup(msg);
                Limit = TLimit.GetValue(msg);
                return TLimit.GetAngle(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Set limit: {0}.", msg);
                return string.Empty;
            }
        }

        public string SetBtn(string msg)
        {
            Btn = msg;
            return Btn;
        }

        public int GetArrowAngle(out int hwAngle, out double angle, out bool isBurn)
        {
            try
            {
                if (Type == "off" || Type == string.Empty || Btn != "1")
                {
                    Scales[0].GetZeroAngle(out hwAngle, out angle);
                    isBurn = false;
                    return 0x50;
                }

                var currentScale = Scales.Find(sc => sc.Type == Type);
                byte rez = currentScale.GetArrowAngle(GetMesuringValue(), Limit, out hwAngle, out angle, out isBurn); //TODO: replace isBurn
                return GetMesuringType() << 8 | rez; // TODO replace
            }
            catch (Exception ex)
            {
                hwAngle = 0;
                angle = 0;
                isBurn = false;
                return 0x50; // TODO check
            }
        }

        private double GetMesuringValue()
        {
            if (Group == "ohm" && Type == "ohm")
                return ohm;

            if (Group == "voltage" && Type == "ac")
                return acU;

            if (Group == "voltage" && Type == "dc")
                return dcU;

            if (Group == "current" && Type == "ac")
                return acI;

            if (Group == "current" && Type == "dc")
                return dcI;

            return 0;
        }

        private byte GetMesuringType()
        {
            if (Group == "ohm" && Type == "ohm")
                return 4;

            if (Group == "voltage" && Type == "ac")
                return 3;

            if (Group == "voltage" && Type == "dc")
                return 2;

            if (Group == "current" && Type == "ac")
                return 1;

            if (Group == "current" && Type == "dc")
                return 0;

            return 5;
        }
    }
}
