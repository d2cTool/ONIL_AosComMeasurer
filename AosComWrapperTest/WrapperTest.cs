using System;
using System.Diagnostics;
using System.Threading;
using AosComWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComWrapperTest
{
    [TestClass]
    public class WrapperTest
    {
        //[TestMethod]
        public void WrapperTestMethod1()
        {
            Wrapper wrapper = new Wrapper();
            Wrapper.Init();

            Wrapper.controller.ChangedProbePoints += Controller_ChangedProbePoints;
            Wrapper.controller.ConnectMeasurer += Controller_ConnectMeasurer;
            Wrapper.controller.DisconnectMeasurer += Controller_DisconnectMeasurer;

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        private void Controller_DisconnectMeasurer(object sender, EventArgs e)
        {
            Debug.WriteLine("Disconnected");
            //Wrapper.Init();
        }

        private void Controller_ConnectMeasurer(object sender, AosComMeasurer.GetSerialsEventArgs e)
        {
            Debug.WriteLine("Connected");
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = $"{appDir}\\..\\..\\..\\MDeviceTest\\base";
            Wrapper.controller.InitializeBase(path);
        }

        private void Controller_ChangedProbePoints(object sender, AosComMeasurer.ProbeEventArgs e)
        {
            Debug.WriteLine(Wrapper.controller.SetMeasure(10, 200, 350.3, 400, 10));
        }
    }
}
