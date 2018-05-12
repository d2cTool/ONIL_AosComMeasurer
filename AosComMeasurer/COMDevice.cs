using System.Collections.Generic;
using System.Management;
using System.Text.RegularExpressions;

namespace AosComMeasurer
{
    public struct COMDeviceInfo
    {
        public COMDeviceInfo(string deviceID, string name, string port, string pnpDeviceID, string description)
        {
            DeviceID = deviceID;
            PnpDeviceID = pnpDeviceID;
            Description = description;
            Name = name;
            Port = port;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
        public string Port { get; private set; }
    }

    public static class COMDeviceManager
    {
        public static List<COMDeviceInfo> GetComDevices()
        {
            List<COMDeviceInfo> COMDeviceInfoList = new List<COMDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""))
                collection = searcher.Get();

            Regex comRegexp = new Regex(@"COM\d+");

            foreach (var device in collection)
            {
                string name = (string)device.GetPropertyValue("Name");

                Match match = comRegexp.Match(name);
                if (match.Success)
                {
                    COMDeviceInfoList.Add(new COMDeviceInfo(
                        (string)device.GetPropertyValue("DeviceID"),
                        name,
                        match.Value,
                        (string)device.GetPropertyValue("PNPDeviceID"),
                        (string)device.GetPropertyValue("Description")
                        ));
                }
            }

            collection.Dispose();
            return COMDeviceInfoList;
        }
    }


}
