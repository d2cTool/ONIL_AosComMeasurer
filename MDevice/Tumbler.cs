using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace MDevice
{
    public class TType
    {
        private List<string> Angles;
        private List<string> Values;
        private List<string> Msgs;
        private int CurrentIndex;
        public TType(XElement type)
        {
            Angles = new List<string>();
            Values = new List<string>();
            Msgs = new List<string>();

           var points = type.Descendants("point");

            foreach (XElement p in points)
            {
                Angles.Add(p.Attribute("angle").Value);
                Values.Add(p.Attribute("value").Value);
                Msgs.Add(p.Attribute("msg").Value);
            }
        }

        public string GetValue(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index >= 0)
                CurrentIndex = index;
            return Values[CurrentIndex];
        }

        public string GetAngle(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index >= 0)
                CurrentIndex = index;
            return Angles[CurrentIndex];
        }

        public string GetNextAngle()
        {
            if (CurrentIndex < Values.Count - 1)
                ++CurrentIndex;
            else
                CurrentIndex = 0;

            return Angles[CurrentIndex];
        }

        public string GetPrevAngle()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
            else
                CurrentIndex = Values.Count - 1;

            return Angles[CurrentIndex];
        }
    }

    public class TLimit
    {
        public List<string> Angles { get; private set; }
        public List<double> Values { get; private set; }
        public List<string> Msgs { get; private set; }
        public List<string> Groups { get; private set; }
        private int CurrentIndex;
        public TLimit(XElement limit)
        {
            Angles = new List<string>();
            Values = new List<double>();
            Msgs = new List<string>();
            Groups = new List<string>();


            var groupXElmnt = limit.Descendants("group");

            foreach (XElement el in groupXElmnt)
            {
                string gname = el.Attribute("name").Value;
                var points = el.Descendants("point");
                foreach (XElement item in points)
                {
                    Angles.Add(item.Attribute("angle").Value);
                    Values.Add(double.Parse(item.Attribute("value").Value, CultureInfo.InvariantCulture));
                    Msgs.Add(item.Attribute("msg").Value);
                    Groups.Add(gname);
                }
            }
        }

        public double GetValue(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index >= 0)
                CurrentIndex = index;
            return Values[CurrentIndex];
        }

        public string GetAngle(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if (index >= 0)
                CurrentIndex = index;
            return Angles[CurrentIndex];
        }

        public string GetGroup(string msg)
        {
            int index = Msgs.FindIndex(el => el == msg);
            if(index >= 0)
                CurrentIndex = index;
            return Groups[CurrentIndex];
        }

        public string GetNextAngle()
        {
            if (CurrentIndex < Values.Count - 1)
                ++CurrentIndex;
            else
                CurrentIndex = 0;

            return Angles[CurrentIndex];
        }

        public string GetPrevAngle()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
            else
                CurrentIndex = Values.Count - 1;

            return Angles[CurrentIndex];
        }
    }
}
