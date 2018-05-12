using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;
using System;

namespace MDevice
{
    public class Scale
    {
        public string Type { get; private set; }
        public List<double> Values { get; private set; }
        public double maxValue { get; private set; }
        public bool canBurn { get; private set; }
        public List<string> StrValues { get; private set; }
        public List<double> Angles { get; private set; }
        public List<int> HwAngles { get; private set; }
        public Scale(XElement scale, XElement hw_scale)
        {
            StrValues = new List<string>();
            Values = new List<double>();
            Angles = new List<double>();
            HwAngles = new List<int>();

            CultureInfo cultureInfo = new CultureInfo("en");

            maxValue = double.Parse(scale.Attribute("max_value").Value, cultureInfo);
            canBurn = bool.Parse(scale.Attribute("can_burn").Value);

            Type = scale.Attribute("type").Value;
            var scaleElement = scale.Descendants("point");

            foreach (XElement el in scaleElement)
            {
                Angles.Add(double.Parse(el.Attribute("angle").Value, cultureInfo));
                string strVal = el.Attribute("value").Value;
                StrValues.Add(strVal);
                switch (strVal)
                {
                    case "+inf":
                        Values.Add(double.PositiveInfinity);
                        break;
                    case "-inf":
                        Values.Add(double.NegativeInfinity);
                        break;
                    default:
                        Values.Add(double.Parse(strVal, cultureInfo));
                        break;
                }

                var rez = hw_scale.Descendants("point").FirstOrDefault(elmt => elmt.Attribute("value")?.Value == strVal);
                HwAngles.Add(int.Parse(rez.Attribute("hw_angle").Value, cultureInfo));
            }
        }

        public byte GetArrowAngle(double value, double limit, out int hwAngle, out double angle, out bool isBurn)
        {
            double relativeValue = value * maxValue / limit;
            isBurn = false;

            GetNearest(relativeValue, out int prevIndex, out int nextIndex);

            double prevValue = Values[prevIndex];
            double nextValue = Values[nextIndex];

            double prevAngle = Angles[prevIndex];
            double nextAngle = Angles[nextIndex];

            int prevHwAngle = HwAngles[prevIndex];
            int nextHwAngle = HwAngles[nextIndex];

            if (double.IsInfinity(prevValue))
            {
                double rel = Math.Abs(nextValue - relativeValue);

                if (rel < maxValue / 10)
                {
                    angle = prevAngle + (nextAngle - prevAngle) * 0.5;
                    hwAngle = Convert.ToInt32(Math.Round(prevHwAngle + (nextHwAngle - prevHwAngle) * 0.5));
                    return 4;
                }

                if (rel < maxValue / 2 || canBurn == false)
                {
                    angle = prevAngle;
                    hwAngle = prevHwAngle;
                    return 5;
                }

                var index = Values.FindIndex(el => el == 0);
                angle = Angles[index];
                hwAngle = HwAngles[index];
                isBurn = true;
                return 8;
            }

            if (double.IsInfinity(nextValue))
            {
                double rel = Math.Abs(prevValue - relativeValue);

                if (rel < maxValue / 10)
                {
                    angle = nextAngle + (nextAngle - prevAngle) * 0.5;
                    hwAngle = Convert.ToInt32(Math.Round(nextHwAngle + (nextHwAngle - prevHwAngle) * 0.5));
                    return 6;
                }

                if (rel < maxValue / 2 || canBurn == false)
                {
                    angle = nextAngle;
                    hwAngle = nextHwAngle;
                    return 7;
                }

                var index = Values.FindIndex(el => el == 0);
                angle = Angles[index];
                hwAngle = HwAngles[index];
                isBurn = true;
                return 8;
            }

            hwAngle = HwAngleInter(relativeValue, prevIndex, nextIndex);
            angle = AngleInter(relativeValue, prevIndex, nextIndex);
            return 1; //TODO fix
        }

        public void GetZeroAngle(out int hwAngle, out double angle)
        {
            var zeroIndex = Values.FindIndex(el => el == 0);
            hwAngle = HwAngles[zeroIndex];
            angle = Angles[zeroIndex];
        }

        private void GetNearest(double value, out int prevIndex, out int nextIndex)
        {
            double closest = Values.OrderBy(item => Math.Abs(value - item)).First();

            if (value == closest)
            {
                nextIndex = prevIndex = Values.FindIndex(el => el == closest);
                return;
            }

            if (value - closest < 0)
            {
                prevIndex = Values.FindIndex(el => el == closest) - 1;
            }
            else
            {
                prevIndex = Values.FindIndex(el => el == closest);
            }
            nextIndex = prevIndex + 1;
        }

        private int HwAngleInter(double value, int prevIndex, int nextIndex)
        {
            double prevValue = Values[prevIndex];
            double nextValue = Values[nextIndex];

            int prevAngle = HwAngles[prevIndex];
            int nextAngle = HwAngles[nextIndex];

            if (prevIndex == nextIndex)
                return prevAngle;

            double dif = (value - prevValue) / (nextValue - prevValue);
            return Convert.ToInt32(Math.Round(prevAngle + (nextAngle - prevAngle) * dif));
        }

        private double AngleInter(double value, int prevIndex, int nextIndex)
        {
            double prevValue = Values[prevIndex];
            double nextValue = Values[nextIndex];

            double prevAngle = Angles[prevIndex];
            double nextAngle = Angles[nextIndex];

            if (prevIndex == nextIndex)
                return prevAngle;

            double dif = (value - prevValue) / (nextValue - prevValue);
            return prevAngle + (nextAngle - prevAngle) * dif;
        }
    }
}
