using System;
using System.Diagnostics;
using System.Threading;
using AosComMeasurer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComMeasurerTest
{
    [TestClass]
    public class ComWorkerTest
    {
        //[TestMethod]
        public void ComWorkerTestMethod()
        {
            ComWorker comWorker = ComWorker.Instance;
            comWorker.InitComReader();
            comWorker.DisconnectMeasurer += ComWorker_DisconnectMeasurer;
            comWorker.ChangedProbePoints += ComWorker_ChangedProbePoints;
            comWorker.ChangeMeasurerState += ComWorker_ChangeMeasurerState;
            comWorker.ConnectMeasurer += ComWorker_ConnectMeasurer;

            int i = 0;
            while(i < 100000)
            {
                Thread.Sleep(1);
                i++;
            }
        }

        private void ComWorker_ConnectMeasurer(object sender, MeasurerConnectEventArgs e)
        {
            Debug.WriteLine($"connect");
        }

        private void ComWorker_ChangeMeasurerState(object sender, MeasurerChangeStateEventArgs e)
        {
            Debug.WriteLine($"type = {e.Type}, limit = {e.Limit}, btn = {e.Btn}");
        }


        private void ComWorker_ChangedProbePoints(object sender, ProbeEventArgs e)
        {
            Debug.WriteLine($"plus = {e.Plus}, minus = {e.Minus}");
        }

        private void ComWorker_DisconnectMeasurer(object sender, EventArgs e)
        {
            Debug.WriteLine($"disconnect");
        }
    }
}
