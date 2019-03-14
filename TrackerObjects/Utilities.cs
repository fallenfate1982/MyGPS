using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Diagnostics;

namespace GTSBizObjects
{
    public static class Utilities
    {
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Standard method to be used to write to the console. Will not affect efficiency when compiled
        /// as release
        /// </summary>
        /// <param name="message"></param>
        public static void writeLine(string message)
        {
            Debug.WriteLine(message);
        }

        // TODO: need to add methods for error handling and tracing and stuff.

        public static string ByteArrayToString(byte[] ba, int length)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int i = 0; i < length; i++)
                hex.AppendFormat("{0:x2}", ba[i]);
            return hex.ToString();
        }

        public static string ByteArrayToStringDecimal(byte[] ba, int length)
        {
            string returnVal = System.Text.Encoding.UTF8.GetString(ba, 0, length);
            return returnVal;
        }

        public static string CalcCRC16(string strInput)
        {
            ushort crc = 0xFFFF;
            byte[] data = GetBytesFromHexString(strInput);
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc.ToString("X4");
        }

        public static Byte[] GetBytesFromHexString(string strInput)
        {
            Byte[] bytArOutput = new Byte[] { };
            if (!string.IsNullOrEmpty(strInput) && strInput.Length % 2 == 0)
            {
                SoapHexBinary hexBinary = null;
                try
                {
                    hexBinary = SoapHexBinary.Parse(strInput);
                    if (hexBinary != null)
                    {
                        bytArOutput = hexBinary.Value;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return bytArOutput;
        }
       
        public static void SendCommand(TcpClient tcpClient, string message)
        {
            byte[] buffer = StringToByteArray(message);
            NetworkStream ns = tcpClient.GetStream();

            ns.Write(buffer, 0, buffer.Length);

            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debug.WriteLine("Converted Back: " + Utilities.ByteArrayToString(buffer, buffer.Length));
        }
        public static void SendCommandASCII(TcpClient tcpClient, string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            NetworkStream ns = tcpClient.GetStream();

            ns.Write(buffer, 0, buffer.Length);

            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debug.WriteLine("Converted Back: " + Utilities.ByteArrayToString(buffer, buffer.Length));
        }

        public static double CalculateDistanceBetween(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {
            var sCoord = new GeoCoordinate(sLatitude, sLongitude);
            var eCoord = new GeoCoordinate(eLatitude, eLongitude);

            return sCoord.GetDistanceTo(eCoord)/1000.00; // COnversion from Meters to KM
        }

    }
}
