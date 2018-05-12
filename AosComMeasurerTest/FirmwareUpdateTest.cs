using System;
using AosComMeasurer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AosComMeasurerTest
{
    [TestClass]
    public class FirmwareUpdateTest
    {
        [TestMethod]
        public void FirmwareUpdateTestMethod()
        {
            FirmwareUpdater firmwareUpdater = new FirmwareUpdater("COM3", "D:\\3.hex");
        }
    }
}
