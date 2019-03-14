using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace GTSBizObjects
{// not sure if this class should be split into two, one for functionality needed for console server and the other state info
    // and funcitonality for the UI....seems so far to work like that with no transferable items on either side
    // starting to think that this should belong to an inheritance hierarchy
    public class VT340 : IGPSTracker, IGPSTrackerWithOutputs
    {
        #region Stuff for UI side

        protected string trackerId;
        protected string name;
        protected string description;
        protected string status;// definitly needs to be updated as an enum
        protected string serial;
        protected string password;
        protected string authorizedNumbers; // xml string
        protected List<GPSTrackerOutput> outputs;
        protected bool outputChangePending;

        protected TcpClient tcpClient;

        public VT340(object client)
        {
            tcpClient = (TcpClient)client;
            outputs = new List<GPSTrackerOutput>();
        }

        public VT340(string _trackerId, string _name, string _desc, string _status,
            string _serial, string _password, string _authNumbers)
        {
            trackerId = _trackerId;
            name = _name;
            description = _desc;
            status = _status;
            serial = _serial;
            password = _password;
            authorizedNumbers = _authNumbers;
        }

        public string TrackerId
        {
            get
            {
                return trackerId;
            }
        }


        #endregion


        #region Stuff for Console Server side

        /// <summary>
        /// Checks the DB for pending messages to send and send them
        /// </summary>
        public void SendMessages(TcpClient client)
        {
            Utilities.writeLine("Debug SEND DATA 1: Getting Pending Messages for "+ this.trackerId);
            IQueryable<GTSDataStorage.TrackerCommandLog> messages =  Management.GetTrackerCommands(this.TrackerId);

            Utilities.writeLine("Debug SEND DATA 2: There are " + messages.Count() +" Pending Messeags");

            foreach ( GTSDataStorage.TrackerCommandLog m in messages)
            {
                Utilities.writeLine("Debug SEND DATA 3: Sending Message '" +m.Message+"' to tracker: " + this.trackerId);
                Utilities.SendCommand(client, m.Message);
            }
            Utilities.writeLine("Debug SEND DATA 4: Done sending Messages for " + this.trackerId);
        }


        public void RecievedMessage(byte[] message, int length)
        {
            Utilities.writeLine("Debug 11: Recieving Message...");

            Utilities.writeLine("Debug 12: Hex: " + Utilities.ByteArrayToString(message, length));

            //Not sure if this is failsafe, but need to get the trackerId regardless of message Type
            trackerId= getTrackerId(message, length);

            // see if we should even be processing this tracker
           if (!GTSBizObjects.Management.TrackerExist(trackerId))
                throw new Exception("These are not the trackers you're looking for! Tracker does not exist in the DB");


            MessageType mType = getMessageType(message, length);

            // Save Location messages
            if (mType == MessageType.Location)
            {
                SaveLocationMessage(trackerId, message, length);

                //this below should be outside of the condition. Just here for now to make sure we have TrackerId.
                // TODO - need to get this working SetOutputs();
            }
            //else if (mType == MessageType.Login)
            //{
            //    //for now, just okay the request
            //    SendLoginConfirmation();
            //}
            //else if (mType == MessageType.Alert)
            //{
            //    // Call to BOManagement so that event hooks only live in there
            //    SaveAlertLocationMessage(getTrackerId(message, length), message, length);
            //}
        }

        private void SaveLocationMessage(string tid, byte[] message, int length)
        {
            // need to update this to also record messages if there are multiple in the report.
            GTSLocationMessage locationMessage = new VT340LocationMessage(tid, message, length);
            if (locationMessage.IsValid)// only store valid readings
                GTSBizObjects.Management.SaveTrackerInformation(locationMessage);
        }

        private void SaveAlertLocationMessage(string tid, byte[] message, int length)
        {
            // need to update this to also record messages if there are multiple in the report.
            GTSLocationMessage locationMessage = new VT310eAlarmLocationMessage(tid, message, length);
            if (locationMessage.IsValid)// only store valid readings
                GTSBizObjects.Management.SaveTrackerInformation(locationMessage);
        }

        private string getTrackerId(byte[] message, int length)
        {
            Utilities.writeLine("Debug 13: getting Tracker ID");
            String msg = Utilities.ByteArrayToStringDecimal(message,length);
            String[] parts = msg.Split(',');

            if (!string.IsNullOrEmpty(trackerId)) return trackerId;
            string _trackerId = parts[1];

            Utilities.writeLine("Debug 14: Tracker ID:"+ _trackerId);
            return trackerId = _trackerId;
        }

        private MessageType getMessageType(byte[] message, int length)
        {
            Utilities.writeLine("Debug 15: Getting the Message Type");

            String msg = Utilities.ByteArrayToStringDecimal(message, length);
            String[] parts = msg.Split(',');

            string _messageTypeCode = parts[2];

            MessageType ret;
            if (_messageTypeCode == "AAA")
            {
                ret = MessageType.Location;
                Utilities.writeLine("Debug 16: Command Type: automatic event report");
            }
            else if (_messageTypeCode == "F01") {
                ret = MessageType.RestartOK;
                Utilities.writeLine("Debug 16: Command Type: restart OK");
            }
            else
            {
                ret = MessageType.Unknown;
                Utilities.writeLine("Debug 17: Command Type: "+_messageTypeCode);
            }

            return ret;
        }



        #region Build and Send OutputControl commands
        public void SendLoginConfirmation()
        {
            //Prepare the command.
            //Command format for VT300/310e:
            //@@<L><ID><Command:4000><Flag><Checksum>0D0A

            //Let's kick things off with the @@ and length = 12
            StringBuilder sb = new StringBuilder("40400012");
            sb.Append(getPaddedTrackerId());
            sb.Append("400001");
            sb.Append(Utilities.CalcCRC16(sb.ToString()));
            sb.Append("0D0A");

            Utilities.SendCommand(tcpClient, sb.ToString());

        }

        public void SetOutputs()
        {
            //make sure we know current state according to the DB
            refreshOutputs();

            //if there's a change pending
            if (outputChangePending)
            {
                //Okay, so something needs to be sent, but we have three sets of commands that may need to be sent independently
                //1. Immediate
                //2. <10KM
                //3. <20KM

                //So let's build and (conditionally) send our three commands if they apply

                SendOutputChange(OutputTriggerType.Immediate, "4115");
                SendOutputChange(OutputTriggerType.TenK, "4114");
                SendOutputChange(OutputTriggerType.TwentyK, "5114");


            }
        }

        private void SendOutputChange(OutputTriggerType TriggerType, string command)
        {
            //Command format for VT300/310e:
            //@@<L><ID><Command:4115/4114/5114><AABBCCDDEE><Checksum>0D0A

            //Thankfully, everything else about the commands are identical

            StringBuilder data = new StringBuilder();
            data.Append(getOutputCommand(1, TriggerType));
            data.Append(getOutputCommand(2, TriggerType));
            data.Append(getOutputCommand(3, TriggerType));
            data.Append(getOutputCommand(4, TriggerType));
            data.Append(getOutputCommand(5, TriggerType));

            //if there's nothing to do, don't bother sending the command;
            if (data.ToString().Equals("0202020202")) return;

            //If we're still here, there's a change so let's kick things off with the @@ and length = 16
            StringBuilder sb = new StringBuilder("40400016");
            sb.Append(getPaddedTrackerId());
            sb.Append(command);
            sb.Append(data.ToString());
            sb.Append(Utilities.CalcCRC16(sb.ToString()));
            sb.Append("0D0A");

            //Utilities.SendCommand(tcpClient, sb.ToString());
        }

        private string getPaddedTrackerId()
        {
            StringBuilder tId = new StringBuilder(TrackerId);
            while (tId.Length < 14)
            {
                tId.Append("f");
            }

            return tId.ToString();
        }

        private string getOutputCommand(int outputNumber, OutputTriggerType TriggerType)
        {
            GPSTrackerOutput output = outputs.Find(m => m.Number == outputNumber);

            //for the 300/310, 02 means "leave the output at it's previous value"
            if ((output == null) || (output.TriggerType != TriggerType) || (output.ProposedState == output.LastVerifiedState) || (output.ProposedState == OutputState.Previous))
                return "02";

            if (output.ProposedState == OutputState.On)
                return "00";
            if (output.ProposedState == OutputState.Off)
                return "01";

            //Uhh.. why're we here?
            return "02";
        }

        public void refreshOutputs()
        {
            //probably a neater way to do this than clearing the list and reloading
            outputs.Clear();

            //5 outputs on 300/310. These will always return an object, even if there's no DB record
            outputs.Add(Management.GetTrackerOutput(TrackerId, 1));
            outputs.Add(Management.GetTrackerOutput(TrackerId, 2));
            outputs.Add(Management.GetTrackerOutput(TrackerId, 3));
            outputs.Add(Management.GetTrackerOutput(TrackerId, 4));
            outputs.Add(Management.GetTrackerOutput(TrackerId, 5));

            //After we load all the data, we should do a quick check to see if any "Current" state doesn't match the "Proposed" state.
            // If so, we'll know that a command needs to be sent, if not then we'll leave things alone.

            outputChangePending = false;
            foreach (GPSTrackerOutput output in outputs)
            {
                if (output.ProposedState != output.LastVerifiedState)
                    outputChangePending = true;
            }
        }

        #endregion


        #endregion





    }
}
