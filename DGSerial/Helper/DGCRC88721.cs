#region License
// Copyright (c) 2015 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

using System;

namespace DG.Serial.Helper
{
    public class DGCRC8721
    {
        /// <summary>
        /// CRC poly X^8 + X^7 + X^2 + 1 on a single byte
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte CRC8721(Byte b)
        {
	        int i = 0;
	        for(i=0; i<8; i++) {
                if ((byte)(b & 0x80) == 0x80)
                    b = (byte)((b << 1) ^ 0x85);
                else
                    b <<= 1;
	        }
	        return b;
        }

        /// <summary>
        /// CRC poly X^8 + X^7 + X^2 + 1 on a byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte CRC8721(byte[] bytes)
        {
            byte crc = 0x00;
            foreach (byte b in bytes)
            {
                crc = CRC8721((byte)(crc ^ b));
            }
            return crc;
        }

        //
        // ANSI C IMPLEMENTATION
        //
        //unsigned char crc8721byte(unsigned char b) {
        //    unsigned char i = 0;
        //    for(i=0; i<8; i++) {
        //        if(b & 0x80) {
        //            b = (b << 1) ^ 0x85;
        //        } else
        //            b <<= 1;
        //    }
        //    return b;
        //}
        //unsigned char crc8721bytes(unsigned char b[], unsigned int size) {
        //    unsigned int i = 0;
        //    unsigned char crc = 0;
        //    for(i=0; i<size; i++) {
        //        crc = crc8721byte(crc ^ b[i]);
        //    }
        //    return crc;
        //}
    }
}
