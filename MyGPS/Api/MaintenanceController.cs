using GTSDataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyGPS.Api
{
    public class MaintenanceController : ApiController
    {
        [HttpGet]
        public bool Add(string trackerId, string mileageCount, string maintenanceNote)
        {

            try
            {
                //dirty
                GPSTrackerEntities1 context = new GPSTrackerEntities1();

                Maintenance m = context.Maintenance.CreateObject();

                m.trackerId = trackerId;
                m.mileage = Double.Parse(mileageCount);
                m.notes = maintenanceNote;
                m.dateTimeRecorded = DateTime.Now;


                // Need to update table and last set location message?
                Trackers _tracker = context.Trackers.Where(t => t.Id == trackerId).FirstOrDefault();
                _tracker.Mileage = double.Parse(mileageCount);


                context.Maintenance.AddObject(m);
                context.SaveChanges();
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}