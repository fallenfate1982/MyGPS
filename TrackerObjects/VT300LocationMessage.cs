using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace GTSBizObjects
{
    public class VT300LocationMessage:GTSLocationMessage
    {
        internal VT300LocationMessage(string tid, byte[] message, int bytesRead)
            : base(tid, message, bytesRead)
         {
         }

        protected override void objectify(byte[] message, int bytesRead)
        {
            //need to implement better breaking up of message now that we have specifics!!!
            ASCIIEncoding encoder = new ASCIIEncoding();
            rawTextData = encoder.GetString(message, 0, bytesRead);

            string[] _barSeparatedData = rawTextData.Split('|');
            string[] _data = _barSeparatedData[0].Split(',');

            Debug.WriteLine("Objectification Report for VT300");
            Debug.WriteLine("Raw Text: "+rawTextData);

            try
            {
                Debug.WriteLine("Validity: " + _data[1]);
                if (_data[1] == "V") throw new Exception("Reading is not valid, halting saving process");
                isValid = true;
                // need to update so that if not valid the thread will stop processing and wait for next message
                
                // need to update code to pull and process date time
                clientDateTime = DateTime.Now;

                double.TryParse(_data[7],out direction);
                Debug.WriteLine("Direction: " + direction);

                // need to put in better exception handling for this section
                speed = double.Parse(_data[6]) * 1.852;
                Debug.WriteLine("Speed: " + speed);


                //_barSeparatedData[3] gives Input/Output status
                status = _barSeparatedData[3];
                Debug.WriteLine("Raw Input/Output Status: " + _barSeparatedData[3]);

                //Temporary code to show input voltage level for VT310e
                if (_barSeparatedData.Length > 4)
                {
                    string[] _voltageData = _barSeparatedData[4].Split(',');
                    try
                    {
                        input1 = (uint.Parse(_voltageData[0], System.Globalization.NumberStyles.AllowHexSpecifier) * 6.0) / 1024;
                    }
                    catch
                    {
                        if (_voltageData.Length >= 1)
                        {
                            Debug.WriteLine("Parse of Input Voltage 1 failed: " + _voltageData[0]);
                        }
                        input1 = 0.0;
                    }

                    try
                    {
                        input2 = (uint.Parse(_voltageData[1], System.Globalization.NumberStyles.AllowHexSpecifier) * 6.0) / 1024;
                    }
                    catch
                    {
                        if (_voltageData.Length > 1)
                        {
                            Debug.WriteLine("Parse of Input Voltage 2 failed: " + _voltageData[1]);
                        }
                        input2 = 0.0;
                    }
                }
                else
                {
                    Debug.WriteLine("No Input Voltage data available : ArrayLength - " + _barSeparatedData.Length);
                    input1 = 0.0;
                    input2 = 0.0;
                }

                Debug.WriteLine("Voltage on Analog 1: " + input1);
                Debug.WriteLine("Voltage on Analog 2: " + input2);

                // Digital Sensors
                // this code trips up the older trackers (namely the VT300) Need to clean it to work properly for all. Placed a temporaty check

                int theNum;
                int digiIn;
                string thenew = "";

                if (int.TryParse(_barSeparatedData[3], out theNum))
                {
                    digiIn = unchecked((int)uint.Parse(_barSeparatedData[3], System.Globalization.NumberStyles.AllowHexSpecifier));
                }
                else
                {
                    // for vt300
                    thenew = _barSeparatedData[3].Substring(0, 4);
                    status = thenew;
                    digiIn = unchecked((int)uint.Parse(thenew, System.Globalization.NumberStyles.AllowHexSpecifier));
                    
                }
                

                BitArray b = new BitArray(new int[] { digiIn });
                int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();

                dInput1 = b[8];
                dInput2 = b[9];
                dInput3 = b[10];
                dInput4 = b[11];
                dInput5 = b[12];


                //Debug.WriteLine("Voltage on Analog 1: " + ((BitConverter.ToSingle(BitConverter.GetBytes(uint.Parse(_voltageData[0], System.Globalization.NumberStyles.AllowHexSpecifier)),0) * 6) / 1024));
                //Debug.WriteLine("Voltage on Analog 2: " + ((BitConverter.ToSingle(BitConverter.GetBytes(uint.Parse(_voltageData[1], System.Globalization.NumberStyles.AllowHexSpecifier)), 0) * 6) / 1024));

                //status = ""; // need to process _statusData[3&4] for this and save in xml

                // Data Conversion
                latitude.Degrees = int.Parse(_data[2].Substring(0, 2));
                Debug.WriteLine("latitude.Degrees: " + latitude.Degrees);

                latitude.Minutes = int.Parse(_data[2].Substring(2, 2));
                Debug.WriteLine("latitude.Minutes: " + latitude.Minutes);

                latitude.Seconds = float.Parse(_data[2].Substring(5, 4)) / 10000 * 60;
                Debug.WriteLine("latitude.Seconds: " + latitude.Seconds);

                latitude.Heading = _data[3][0];
                Debug.WriteLine("latitude.Heading: " + latitude.Heading);

                string[] lngSplit = _data[4].Split('.');

                if (lngSplit[0].Length == 4)
                {
                    longitude.Degrees = int.Parse(_data[4].Substring(0, 2));
                    Debug.WriteLine("longitude.Degrees: " + longitude.Degrees);

                    longitude.Minutes = int.Parse(_data[4].Substring(2, 2));
                    Debug.WriteLine("longitude.Minutes: " + longitude.Minutes);

                    longitude.Seconds = float.Parse(_data[4].Substring(5, 4)) / 10000 * 60;
                    Debug.WriteLine("longitude.Seconds: " + longitude.Seconds);

                    longitude.Heading = _data[5][0];
                    Debug.WriteLine("longitude.Heading: " + longitude.Heading);
                }
                else
                {
                    longitude.Degrees = int.Parse(_data[4].Substring(0, 3));
                    Debug.WriteLine("longitude.Degrees: " + longitude.Degrees);

                    longitude.Minutes = int.Parse(_data[4].Substring(3, 2));
                    Debug.WriteLine("longitude.Minutes: " + longitude.Minutes);

                    longitude.Seconds = float.Parse(_data[4].Substring(6, 4)) / 10000 * 60;
                    Debug.WriteLine("longitude.Seconds: " + longitude.Seconds);

                    longitude.Heading = _data[5][0];
                    Debug.WriteLine("longitude.Heading: " + longitude.Heading);
                }

                //VS: Removed - don't think this is in the right place anyway. Status is now a few lines down, as barSeparated[3]
                //// need to update to process full status data in xml
                //string[] _statusData = _data[11].Split('|');
                //System.Diagnostics.Debug.WriteLineIf(_statusData.Length < 3, "Error in _data for status info: " + _data[11]);

                double.TryParse(_barSeparatedData[1], out hdop);
                Debug.WriteLine("HDOP: " + hdop);

                int.TryParse(_barSeparatedData[2], out altitude);
                Debug.WriteLine("Altitude: " + altitude);

                
               
            }
            catch (Exception e)
            {
                //Need to log exceptions properly
                System.Diagnostics.Debug.WriteLine(e.Message+">>>>\n"+e.StackTrace);
            }

        }
    }
}
