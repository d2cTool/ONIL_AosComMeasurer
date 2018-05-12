using System;
using System.Xml.Linq;
using MDevice;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MDeviceTest
{
    [TestClass]
    public class BaseTest
    {
        [TestMethod]
        public void BaseTestMethod()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = $"{appDir}\\..\\..\\base";
            MDevicesBase mDevicesBase = new MDevicesBase($"{appDir}\\..\\..\\base");
            XElement dev = mDevicesBase.GetDevice("28FFBC6BA1160506");
        }

        [TestMethod]
        public void CreateDeviceTestMethod()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            MDevicesBase mDevicesBase = new MDevicesBase($"{appDir}\\..\\..\\base");
            XElement dev = mDevicesBase.GetDevice("28FFBC6BA1160506");
            XElement scale = mDevicesBase.GetScalesBySerial("28FFBC6BA1160506");

            Device mDevice = new Device(dev, scale);
        }

        [TestMethod]
        public void ArrowTestMethod()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            MDevicesBase mDevicesBase = new MDevicesBase($"{appDir}\\..\\..\\base");
            XElement dev = mDevicesBase.GetDevice("28FFBC6BA1160506");
            XElement scale = mDevicesBase.GetScalesBySerial("28FFBC6BA1160506");

            Device mDevice = new Device(dev, scale);

            mDevice.Scales[0].GetArrowAngle(0, 30, out int hwAngle, out double angle, out bool isBurn);
            Assert.AreEqual(hwAngle, mDevice.Scales[0].HwAngles[1]);
            Assert.AreEqual(angle, mDevice.Scales[0].Angles[1]);
            Assert.IsFalse(isBurn);

            mDevice.Scales[0].GetArrowAngle(-10, 30, out hwAngle, out angle, out isBurn);
            Assert.AreEqual(hwAngle, mDevice.Scales[0].HwAngles[0]);
            Assert.AreEqual(angle, mDevice.Scales[0].Angles[0]);
            Assert.IsFalse(isBurn);

            mDevice.Scales[0].GetArrowAngle(-30, 30, out hwAngle, out angle, out isBurn);
            Assert.AreEqual(hwAngle, mDevice.Scales[0].HwAngles[1]);
            Assert.AreEqual(angle, mDevice.Scales[0].Angles[1]);
            Assert.IsTrue(isBurn);

            mDevice.Scales[0].GetArrowAngle(38, 30, out hwAngle, out angle, out isBurn);
            Assert.AreEqual(hwAngle, mDevice.Scales[0].HwAngles[32]);
            Assert.AreEqual(angle, mDevice.Scales[0].Angles[32]);
            Assert.IsFalse(isBurn);

            mDevice.Scales[0].GetArrowAngle(-46, 30, out hwAngle, out angle, out isBurn);
            Assert.AreEqual(hwAngle, mDevice.Scales[0].HwAngles[1]);
            Assert.AreEqual(angle, mDevice.Scales[0].Angles[1]);
            Assert.IsTrue(isBurn);

            mDevice.Scales[0].GetArrowAngle(0.5, 30, out hwAngle, out angle, out isBurn);
            Assert.AreEqual(hwAngle, 2);
            Assert.AreEqual(angle, 1.6);
            Assert.IsFalse(isBurn);

            mDevice.Scales[2].GetArrowAngle(100, 10, out hwAngle, out angle, out isBurn);
            Assert.AreEqual(hwAngle, 0);
            Assert.AreEqual(angle, 0);
            Assert.IsFalse(isBurn);
        }
    }
}
