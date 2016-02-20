#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using System.Text;
using System.Linq;
using System.Management;

namespace DG.Serial.Helper
{
    public class DGSerialHelper
    {
        public DGSerialHelper()
        { }

        /// <summary>
        /// Convert a string to a byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string str)
        {
            byte[] ret = new byte[str.Length];
            //do not convert the last EOL byte
            for (int i = 0; i < str.Length; i++)
                ret[i] = (byte)(System.Text.Encoding.ASCII.GetBytes(str.Substring(i, 1))[0]);
            return ret;
        }

        /// <summary>
        /// Convert a byte array to string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            return ASCIIEncoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Find all comports with the give VID and PID
        /// </summary>
        /// <param name="VID"></param>
        /// <param name="PID"></param>
        /// <returns></returns>
        public static string[] FindPortNameByVIDPID(string VID, string PID)
        {
            string[] comports = new string[] { };

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(string.Format(@"Select * From WIN32_SerialPort WHERE PNPDeviceID Like '%VID_{0}&PID_{1}%'", VID, PID)))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                string description = device.GetPropertyValue("Description").ToString();
                string deviceID = device.GetPropertyValue("DeviceID").ToString();
                string name = device.GetPropertyValue("Name").ToString();

                comports = comports.Concat(new string[] { deviceID }).ToArray();
            }

            collection.Dispose();

            return comports;
        }

        /// <summary>
        /// List all serial ports
        /// </summary>
        /// <returns></returns>
        public static string[] ListPorts()
        {
            string[] comports = new string[] { };

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_SerialPort"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                string description = device.GetPropertyValue("Description").ToString();
                string deviceID = device.GetPropertyValue("DeviceID").ToString();
                string name = device.GetPropertyValue("Name").ToString();

                comports = comports.Concat(new string[] { deviceID }).ToArray();
            }

            collection.Dispose();

            return comports;
        }

    }
}
