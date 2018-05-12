using System;
using System.Diagnostics;
using AosComMeasurer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComMeasurerTest
{
    [TestClass]
    public class ComReaderTest
    {
        //[TestMethod]
        public void ComReaderTestMethod()
        {
            ComReader comReader = ComReader.Instance;
            if(comReader.Init())
                comReader.Start();
            else
                Debug.WriteLine($"Cant Init ComReader");
            comReader.DataReceived += ComReader_DataReceived;
            comReader.LostConnection += ComReader_LostConnection;

            while (true)
            {
                var key = Console.ReadKey();
            }
        }

        private void ComReader_LostConnection(object sender, DataEventArgs e)
        {
            Debug.WriteLine($"LostConnection");
        }

        private void ComReader_DataReceived(object sender, DataEventArgs e)
        {
            Debug.WriteLine($"Data recieved: {e.Data}");
        }
    }
}
