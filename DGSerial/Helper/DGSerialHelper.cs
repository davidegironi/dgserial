#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using System.IO.Ports;
using System.Text;

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
        /// List all serial ports
        /// </summary>
        /// <returns></returns>
        public static string[] ListPorts()
        {
            return SerialPort.GetPortNames();
        }

    }
}
