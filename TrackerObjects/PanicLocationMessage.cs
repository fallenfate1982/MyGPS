using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    public class PanicLocationMessage : CellLocationMessage
    {
        //private int _vibe;

        //public int Vibe
        //{
        //    get { return _vibe; }
        //    set { _vibe = value; }
        //}
        //private string _message;

        //public string Message
        //{
        //    get { return _message; }
        //    set { _message = value; }
        //}
        //private int _status;

        //public int StatusLR
        //{
        //    get { return _status; }
        //    set { _status = value; }
        //}


        public PanicLocationMessage(NameValueCollection data) : base(data)
        {
            try
            {
                //_vibe = int.Parse(data["vibe"]);
                //_message = data["message"];
                //_status = int.Parse(data["status"]);
            }
            catch (Exception e)
            {
                isValid = false;
                ExceptionHandler.HandleGeneralException(e);
            }
        }

        public PanicLocationMessage(string phnum, string lat, string lon, string heading, string speed, string clientTime)
            : base(phnum, lat, lon, heading, speed, clientTime)
        {
            try
            {
                //_vibe = int.Parse(data["vibe"]);
                //_message = data["message"];
                //_status = int.Parse(data["status"]);
            }
            catch (Exception e)
            {
                isValid = false;
                ExceptionHandler.HandleGeneralException(e);
            }
        }
    }
}
