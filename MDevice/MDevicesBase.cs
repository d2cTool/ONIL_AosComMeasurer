using NLog;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MDevice
{
    public class MDevicesBase
    {
        private XElement baseFile;
        private string pathToBase;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MDevicesBase(string pathToBase)
        {
            try
            {
                this.pathToBase = pathToBase;
                baseFile = XElement.Load(pathToBase + @"\base.xml");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Can't find main base file: {0}.", baseFile);
            }
        }

        public bool HasDevice(string serial)
        {
            XElement measr = baseFile?.
                Descendants("device")?.
                FirstOrDefault(el => el.Attribute("serial")?.Value == serial);

            return (measr != null) ? true : false;
        }

        public string GetType(string serial)
        {
            XElement measr = baseFile?.
                Descendants("device")?.
                FirstOrDefault(el => el.Attribute("serial")?.Value == serial);

            return measr?.Attribute("type").Value;
        }

        public XElement GetDevice(string serial)
        {
            string fileName = string.Empty;
            try
            {
                fileName = $"{pathToBase}\\{GetType(serial)}.xml";
                return XElement.Load(fileName);
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Can't load device file: {0}, serial: {1}.", fileName, serial);
                return null;
            }
        }

        public XElement GetDeviceBySerial(string serial)
        {
            string filePath = "";
            try
            {
                filePath = pathToBase + "/" + GetType(serial) + ".xml";
                return XElement.Load(filePath);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "MDevicesBase: GetXElementBySerial: file {0}, serial {1}", filePath, serial);
                return null;
            }
        }

        public XElement GetScalesBySerial(string serial)
        {
            try
            {
                XElement device = baseFile?.Descendants("device")?.FirstOrDefault(el => el.Attribute("serial")?.Value == serial);
                //return device.Descendants("scales")?.FirstOrDefault();
                return device;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "MDevicesBase: GetScalesBySerial: serial {0}", serial);
                return null;
            }
        }

        public void AddDevice(string serial) //TODO: fix
        {
            if (!HasDevice(serial))
            {
                XElement deflt = baseFile.
                Descendants("device").
                FirstOrDefault(el => el.Attribute("serial")?.Value == "default");

                XElement xElmnt = new XElement(deflt);
                xElmnt.SetAttributeValue("serial", serial);
                baseFile.LastNode.AddAfterSelf(xElmnt);
                //baseFile.Save(baseFileName);
            }
        }
        public void RemoveDevice(string serial) //TODO: check
        {
            if (HasDevice(serial))
            {
                XElement measr = baseFile.
                Descendants("device").
                FirstOrDefault(el => el.Attribute("serial")?.Value == serial);

                measr.Remove();
                //baseFile.Save(baseFileName);
            }
        }
        public void SaveBase() //TODO: do it
        {
            //var xmlFromLINQ = new XElement("device",
            //    new XAttribute("serial", serial),
            //    new XElement("scales",
            //        from sc in arrow.scales
            //        select new XElement("scale",
            //            new XAttribute("type", sc.type),
            //            from pt in sc.points
            //            select new XElement("point",
            //                new XAttribute("angle", pt.angle),
            //                new XAttribute("value", pt.value)
            //                )
            //        )
            //     )
            //);

            //removeDevice(serial);
            //var t = baseFile.LastNode;
            //baseFile.LastNode.AddAfterSelf(xmlFromLINQ);
            //baseFile.Save(baseFileName);
        }
    }
}
