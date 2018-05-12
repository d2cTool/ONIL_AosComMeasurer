using System;
using System.Threading;
using AosComMeasurer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComMeasurerTest
{
    [TestClass]
    public class ConnectionTest
    {
        //[TestMethod]
        public void TestMethod1()
        {
            ComWorker comWorker = ComWorker.Instance;

            while (true)
            {
                Thread.Sleep(1);
            }
        }

        //[TestMethod]
        public void ControllerInitTest()
        {
            Controller controller = Controller.Instance;
            controller.InitializeBase("D:\\Dropbox\\Onil\\Projects\\Measurer Driver\\AosComMeasurer\\MDeviceTest\\base");

            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}
