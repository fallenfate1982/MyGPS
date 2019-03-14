using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    public class GeoFence
    {
        public GeoFence(int id, string name, string coordinates, bool? isPublic, string details, bool? status, int? zoom, string createdBy, DateTime? createdDate, string updatedBy, DateTime? updatedDate)
        {
            this.Id = id;
            this.Name = name;
            this.Coordinates = coordinates;
            this.IsPublic = isPublic;
            this.Details = details;
            this.Status = status;
            this.Zoom = zoom;
            this.CreatedBy = createdBy;
            this.CreatedDate = createdDate;
            this.UpdatedBy = updatedBy;
            this.UpdatedDate = updatedDate;
        }

        public int Id { get; set; }
        public string Name {get; set;}
        public string Coordinates {get; set;}
        public bool? IsPublic {get; set;}
        public string Details {get; set;}
        public bool? Status {get; set;}
        public int? Zoom {get; set;}
        public string CreatedBy{ get; set;}
        public DateTime? CreatedDate {get; set;}
        public string UpdatedBy{ get; set;}
        public DateTime? UpdatedDate {get; set;}


    }
}
