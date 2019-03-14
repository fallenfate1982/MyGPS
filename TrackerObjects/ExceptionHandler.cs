using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GTSBizObjects
{
    public static class ExceptionHandler
    {

        public static void HandleGeneralException(Exception e)
        {

            //FileStream ostrm;
            //StreamWriter writer;
            //TextWriter oldOut = Console.Out;
            //try
            //{
            //    ostrm = new FileStream("Exceptions123.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //    writer = new StreamWriter(ostrm);
            //}
            //catch (Exception b)
            //{
                string sSource;
                string sLog;
                string sEvent;

                sSource = "PhoneGap GPS Info Handler";
                sLog = "Application";
                sEvent = "Problems";

                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);

                //EventLog.WriteEntry(sSource, "Cannot open Exceptions123.txt for writing");
                //EventLog.WriteEntry(sSource, b.Message,
                //    EventLogEntryType.Error, 234);
                EventLog.WriteEntry(sSource, e.Message + "\n " + e.Source + "\n " + e.StackTrace,
                    EventLogEntryType.Error, 234);
                
            //    return;
            //}
            //Console.SetOut(writer);
            //Console.Write(e.Message + "\n " + e.Source + "\n " + e.StackTrace);
        }
    }
}
