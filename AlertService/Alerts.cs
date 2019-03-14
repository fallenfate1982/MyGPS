using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Timers;
//using System.Web.Script.Serialization;
using System.Collections.Generic;
using GTSDataStorage.Manager;
using GTSDataStorage;
using System.Net.Mail;
using System.Net.Configuration;


namespace AlertService
{
  public partial class Alerts : ServiceBase
  {
    private Timer timer = new Timer();
    private double servicePollInterval;
    private string alertName = "";
    private string message = "";  
    private string latExp = @"lat:[+-]*(.)\d(.)\d*";
    private string lgExp = @"lg:[+-]*(.)\d(.)\d*";

    public Alerts()
    {
      InitializeComponent();
      int migrationInterval = Convert.ToInt32(ConfigurationManager.AppSettings["MigrationInterval"]);
      dataMigrationTimer.Interval = migrationInterval;
    }

    protected override void OnStart(string[] args)
    {
      timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
      //providing the time in miliseconds 
      servicePollInterval = Convert.ToDouble(ConfigurationManager.AppSettings["AlertServiceInterval"]);
      timer.Interval = servicePollInterval;
      timer.AutoReset = true;      
      timer.Enabled = true;
      timer.Start();
    }

    private void timer_Elapsed(object sender, EventArgs e)
    {
      timer.Stop();
     
      var logs = LocationMessageAlertsManager.GetLogs();
      foreach (var log in logs) //Lopp through then each location log
      {
        var fences = FencesManager.GetAll();
        foreach (var fence in fences)
        {
          var a = fence.FencesId;

          //var jsons = "{lat:28.589611097714087, lg:77.11919783963822},{lat:28.600161714673284, lg:77.1233177126851},{lat:28.604683083437216, lg:77.13155745877884},{lat:28.596695200206437, lg:77.14614867581986},{lat:28.59368074686057, lg:77.1562766970601},{lat:28.59111839354537, lg:77.16297149076127},{lat:28.58508908056065, lg:77.14992522611283},{lat:28.584184653792438, lg:77.13636397733353},{lat:28.584033915241474, lg:77.12675094022416}";
          var jsons = fence.FencesCoordinate;

          var latMatchCol = Regex.Matches(jsons, latExp);
          var lgMatchCol = Regex.Matches(jsons, lgExp);

          var locs = new List<Loc>();
          for (int i = 0; i < latMatchCol.Count; i++)
          {
            var l = latMatchCol[i].Value.Split(':')[1];
            var g = lgMatchCol[i].Value.Split(':')[1];
            locs.Add(new Loc { Lt = Convert.ToDouble(l), Lg = Convert.ToDouble(g) });
          }

          for (int i = 0; i < lgMatchCol.Count; i++)
          {
            var l = lgMatchCol[i].Value.Split(':')[1];
            var g = lgMatchCol[i].Value.Split(':')[1];
            locs.Add(new Loc { Lt = Convert.ToDouble(l), Lg = Convert.ToDouble(g) });
          }

          var IsPointInPolygon = ServiceHelper.IsPointInPolygon(locs, new Loc { Lt = (double)log.Lat, Lg = (double)log.Lang });

          if (IsPointInPolygon == true)
          {
            //If already entered then go for next fences
            if (AlertFiredManager.IsAlreadyEnterdInFences(log.TrackerId))
            {
              continue;
            }
            var groupAlert = GroupAlertManager.GetAllByFencesId(fence.FencesId);
            foreach (var item in groupAlert)
            { 
              var alert = AlertManager.GetById(item.AlertId);

              if (alert.AlertOnGeofenceEnter ?? false)
              {
                //Get all user details and send mail
                alertName = alert.AlertName;
                message = alert.Message;
                SendAlertAndSaveInAlertLogTbl(fence.FencesId, log.MessageAlertId, item.AlertId, "onEnter");           
              }
            }
            //now inserting value in fired update table 
            var alrtFired = new AlertFired
            {
              TrackerId = log.TrackerId,
              MessageAlertId=log.MessageAlertId,
              LocationMessageId=log.LocationMessageId,
              IsEnter = true,
              IsExit = false,
              Active = true, //??????,
              FenceId = fence.FencesId
            };
            AlertFiredManager.Save(alrtFired);
          }
          
          else                 //Ispointinpolygon==false
          {
            //var fired = AlertFiredManager.GetActiveEnterByTrackerId(log.TrackerId,Convert.ToInt32(log.LocationMessageId));
            var fired = AlertFiredManager.GetActiveEnterByTrackerId(log.TrackerId);
            if (fired == null) continue;

            var groupAlert = GroupAlertManager.GetAllByFencesId(fence.FencesId);
            foreach (var item in groupAlert)
            {
              var alert = AlertManager.GetById(item.AlertId);
             
               if (alert.AlertOnGeofenceLeave == true)
               {
                //get user details and send msg
                alertName = alert.AlertName;
                message = alert.Message;
                SendAlertAndSaveInAlertLogTbl(fence.FencesId,log.MessageAlertId,item.AlertId,"onExit");    
               }
            }
            //update values Active to false and is isexit and isenter to true in alertFired table;      
            AlertFiredManager.ProcessedAlertExitFired(fired.Id);
          }          
        }
        //update IsProccessed to true in LocationMessageAlert table.
        LocationMessageAlertsManager.UpdateByTrackerId(log.TrackerId); 
      }
      timer.Start();
    }
    
    protected override void OnStop()
    {
      timer.Stop(); 
    }

    protected override void OnContinue()
    {
      base.OnContinue();
      timer.Start();
    }

    protected override void OnPause()
    {
      base.OnPause();
      timer.Stop();
    }

    protected override void OnShutdown()
    {
      base.OnShutdown();
      timer.Stop();
    }
    
   public void DataMigration()
    {
      var maxId = LocationMessageAlertsManager.GetMaxLocationMessageId();
      var locationMessage = LocationMessageManager.GetByMaxLocMessageId(Convert.ToInt32(maxId));
     
     foreach(var msg in locationMessage)
      {
        var alert = new LocationMessageAlert();

        alert.LocationMessageId = msg.Id;
        alert.TrackerId = msg.TrackerId;
        alert.Lat = msg.LatSeconds;
        alert.Lang = msg.LngSeconds;
        alert.Alt = msg.Altitude;
        alert.LocationMessageId = msg.Id;
        alert.IsProcessed = false;

        LocationMessageAlertsManager.Save(alert);      
       }
    }
   
    public void SendAlertAndSaveInAlertLogTbl(int fenceId,long messageAlertId,int alertId,string action)
    {
      var alertUsers = UserFenceMapManager.GetByFenceId(fenceId);
      foreach (var user in alertUsers)
      {
        var userDetail = Aspnet_UserManager.GetByUserName(user.UserName);
        var emailId = userDetail.EmailId;
        SendMail(emailId, alertName, message);
        
        //now inserting value in alertlog table
        var alertLog = new AlertLog
        {
          MessageAlertId = messageAlertId,
          UserId = userDetail.UserId,
          AlertId= alertId,
          FenceId=fenceId,
          Action=action,
          Success=true
        };
        AlertLogManager.Save(alertLog);
      }
    }

    public void SendMail(string emailId, string alertName, string message)
    {
      try
      {
        MailMessage mail = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient();
        mail.To.Add(emailId);
        mail.Subject = alertName;
        mail.Body = message;
        SmtpServer.Send(mail);
       // System.Windows.Forms.MessageBox.Show("Mail Sent");
      }
      catch (System.Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ex.ToString());
      }
    }


    private void dataMigrationTimer_Tick(object sender, EventArgs e)
    {
     DataMigration();
    }


  }
}
