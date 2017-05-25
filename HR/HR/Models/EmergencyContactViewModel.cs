using System.Collections.Generic;
using HR.Entity;
using System.Web.Mvc;

namespace HR.Models
{
    public class EmergencyContactViewModel : BaseViewModel
    {
        public SelectList Countries { get; set; }
        public EmergencyContact EmergencyContact { get; set; }
      
    }
}