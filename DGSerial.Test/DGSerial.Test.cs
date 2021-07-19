#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using DG.Serial.Helper;
using NUnit.Framework;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Threading;

namespace DG.Serial.Test
{
    [TestFixture]
    public class DGSerialTest
    {
        private static string _portName1 = "";
        private static int _baudRate1 = 0;
        private static string _portName2 = "";
        private static int _baudRate2 = 0;

        /// <summary>
        /// Initialized app config for .NET core
        /// </summary>
        [OneTimeSetUp]
        public void InitializeTestRunnerAppConfig()
        {
            string appConfigPath = Assembly.GetExecutingAssembly().Location + ".config";
            if (!File.Exists(appConfigPath))
                return;
            Configuration configurationApp = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = appConfigPath }, ConfigurationUserLevel.None);
            Configuration configurationActive = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configurationApp == configurationActive)
                return;
            configurationActive.AppSettings.Settings.Clear();
            foreach (string key in configurationApp.AppSettings.Settings.AllKeys)
                configurationActive.AppSettings.Settings.Add(configurationApp.AppSettings.Settings[key]);
            configurationActive.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        public DGSerialTest()
        {
            _portName1 = ConfigurationManager.AppSettings["virtualSerial1PortName"];
            _baudRate1 = Convert.ToInt32(ConfigurationManager.AppSettings["virtualSerial1BaudRate"]);
            _portName2 = ConfigurationManager.AppSettings["virtualSerial2PortName"];
            _baudRate2 = Convert.ToInt32(ConfigurationManager.AppSettings["virtualSerial2BaudRate"]);
        }

        [Test]
        public void Get()
        {
            DGSerial serial = new DGSerial(_portName1, _baudRate1);

            Assert.That(serial.Get(), Is.Not.EqualTo(null));
        }

        [Test]
        public void Open()
        {
            DGSerial serial = null;

            serial = new DGSerial(_portName1, _baudRate1);

            Assert.IsTrue(serial.Open());

            serial.Close();
        }

        [Test]
        public void Close()
        {
            DGSerial serial = new DGSerial(_portName1, _baudRate1);

            Assert.IsFalse(serial.Close());

            serial.Open();

            Assert.IsTrue(serial.Close());
        }

        void WriteBytes_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            Assert.That(sp.ReadExisting(), Is.EqualTo("Test1"));
        }

        [Test]
        public void WriteBytes()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            serial2.AttachDataReceived(WriteBytes_DataReceived);

            byte[] buffertx = DGSerialHelper.StringToBytes("Test1");
            Assert.IsTrue(serial1.WriteBytes(buffertx));

            serial1.Close();
            Thread.Sleep(1000);

            serial2.Close();
        }

        [Test]
        public void ReadBytes()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            byte[] buffertx = DGSerialHelper.StringToBytes("Test1");
            serial1.WriteBytes(buffertx);

            byte[] bufferrx = serial2.ReadBytes();
            Assert.That(buffertx, Is.EqualTo(bufferrx));

            serial1.Close();
            serial2.Close();
        }

        [Test]
        public void ReadBytes2()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            byte[] buffertx = DGSerialHelper.StringToBytes("Test1");
            serial1.WriteBytes(buffertx);

            byte[] bufferrx = serial2.ReadBytes(5);
            serial2.ReadBytes();
            Assert.That(serial2.BytesToRead(), Is.EqualTo(0));

            serial1.Close();
            serial2.Close();
        }

        private void WriteBytesBlocking_ReadThread()
        {
            Thread.Sleep(2000);

            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial2.Open();

            byte[] bufferrx = serial2.ReadBytes();

            serial2.Close();
        }

        [Test]
        public void WriteBytesBlocking()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);

            Thread thread = new Thread(new ThreadStart(WriteBytesBlocking_ReadThread));

            serial1.Open();
            byte[] buffertx = new byte[serial1.Get().WriteBufferSize];
            Stopwatch watch = Stopwatch.StartNew();
            thread.Start();
            serial1.WriteBytesBlocking(buffertx);
            watch.Stop();
            Assert.That(watch.ElapsedMilliseconds, Is.AtLeast(1999));

            serial1.Close();
        }

        private void ReadBytesBlocking_ReadThread()
        {
            Thread.Sleep(2000);

            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);

            serial1.Open();

            byte[] buffertx = new byte[serial1.Get().ReadBufferSize];
            serial1.WriteBytes(buffertx);

            serial1.Close();
        }

        [Test]
        public void ReadBytesBlocking()
        {
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            Thread thread = new Thread(new ThreadStart(ReadBytesBlocking_ReadThread));

            serial2.Open();
            Stopwatch watch = Stopwatch.StartNew();
            thread.Start();
            byte[] bufferrx = serial2.ReadBytesBlocking();
            watch.Stop();
            var wd = watch.ElapsedMilliseconds;
            Assert.That(watch.ElapsedMilliseconds, Is.AtLeast(1999));

            serial2.Close();
        }

        private void ReadBytesTo_ReadThread()
        {
            Thread.Sleep(2000);

            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);

            serial1.Open();

            byte[] buffertx = DGSerialHelper.StringToBytes("T44es41t1");
            serial1.WriteBytes(buffertx);

            serial1.Close();
        }

        [Test]
        public void ReadBytesTo()
        {
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial2.Open();

            Thread thread = new Thread(new ThreadStart(ReadBytesTo_ReadThread));
            thread.Start();
            byte[] bufferrx = serial2.ReadBytesTo(Convert.ToByte('1'), Convert.ToByte('4'), SerialPort.InfiniteTimeout);
            var a = DGSerialHelper.BytesToString(bufferrx);
            Assert.That(DGSerialHelper.BytesToString(bufferrx), Is.EqualTo("T4es1t"));

            Thread thread2 = new Thread(new ThreadStart(ReadBytesTo_ReadThread));
            thread2.Start();
            byte[] bufferrx2 = serial2.ReadBytesTo(Convert.ToByte('2'), Convert.ToByte('4'), 3000);
            Assert.That(DGSerialHelper.BytesToString(bufferrx2), Is.EqualTo("T4es1t1"));

            serial2.Close();
        }

        void AttachDataReceived_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            Assert.That(sp.ReadExisting(), Is.EqualTo("Test1"));
        }

        [Test]
        public void AttachDataReceived()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            serial2.AttachDataReceived(WriteBytes_DataReceived);

            byte[] buffertx = DGSerialHelper.StringToBytes("Test1");
            serial1.WriteBytes(buffertx);

            serial1.Close();
            Thread.Sleep(1000);

            serial2.Close();
        }

        [Test]
        public void BytesToRead()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            byte[] buffertx = DGSerialHelper.StringToBytes("Test1");
            serial1.WriteBytes(buffertx);

            Assert.That(serial2.BytesToRead(), Is.EqualTo(buffertx.Length));
            byte[] bufferrx = serial2.ReadBytes();

            Assert.That(serial2.BytesToRead(), Is.EqualTo(0));

            serial1.Close();
            serial2.Close();
        }

        [Test]
        public void BytesToWrite()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            byte[] buffertx = DGSerialHelper.StringToBytes("Test1");
            serial1.WriteBytes(buffertx);

            byte[] bufferrx = serial2.ReadBytes();

            Assert.That(serial2.BytesToWrite(), Is.EqualTo(0));

            serial1.Close();
            serial2.Close();
        }

        [Test]
        public void TimeOutWriteFail()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);

            serial1.Open();

            byte[] buffertx1 = DGSerialHelper.StringToBytes("Test1");
            Assert.IsTrue(serial1.WriteBytes(buffertx1));

            byte[] buffertx2 = DGSerialHelper.StringToBytes("Test1");
            Assert.IsFalse(serial1.WriteBytes(buffertx2));

            serial1.Close();
        }

        [Test]
        public void TimeOutWriteSuccess()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            byte[] buffertx1 = new byte[serial1.Get().ReadBufferSize];
            Assert.IsTrue(serial1.WriteBytes(buffertx1));
            byte[] bufferrx1 = serial2.ReadBytes();

            byte[] buffertx2 = new byte[serial1.Get().ReadBufferSize - 5];
            Assert.IsTrue(serial1.WriteBytes(buffertx2));
            byte[] bufferrx2 = serial2.ReadBytes();

            serial1.Close();
            serial2.Close();
        }

        [Test]
        public void TimeOutReadFail()
        {
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial2.Open();

            byte[] bufferrx1 = serial2.ReadBytes();
            Assert.That(bufferrx1.Length, Is.EqualTo(0));

            serial2.Close();
        }

        [Test]
        public void TimeOutReadSuccess()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            byte[] buffertx1 = new byte[serial1.Get().ReadBufferSize];
            serial1.WriteBytes(buffertx1);
            byte[] bufferrx1 = serial2.ReadBytes();
            Assert.That(bufferrx1.Length, Is.EqualTo(serial1.Get().ReadBufferSize));

            byte[] buffertx2 = new byte[serial1.Get().ReadBufferSize - 5];
            serial1.WriteBytes(buffertx2);
            byte[] bufferrx2 = serial2.ReadBytes();
            Assert.That(bufferrx2.Length, Is.EqualTo(serial1.Get().ReadBufferSize - 5));

            serial1.Close();
            serial2.Close();
        }

        [Test]
        public void DiscardInBuffer()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);
            DGSerial serial2 = new DGSerial(_portName2, _baudRate2);

            serial1.Open();
            serial2.Open();

            byte[] buffertx = new byte[serial2.Get().ReadBufferSize];
            serial1.WriteBytes(buffertx);
            serial1.WriteBytes(buffertx);
            serial1.WriteBytes(buffertx);

            Assert.That(serial2.BytesToRead(), Is.EqualTo(serial2.Get().ReadBufferSize));

            serial2.DiscardInBuffer();

            Assert.That(serial2.BytesToRead(), Is.EqualTo(0));

            serial1.Close();
            serial2.Close();
        }


        [Test]
        public void DiscardOutBuffer()
        {
            DGSerial serial1 = new DGSerial(_portName1, _baudRate1);

            serial1.Open();

            byte[] buffertx = new byte[serial1.Get().WriteBufferSize];
            serial1.WriteBytes(buffertx);

            serial1.DiscardOutBuffer();

            Assert.That(serial1.BytesToRead(), Is.EqualTo(0));

            serial1.Close();
        }

    }
}
