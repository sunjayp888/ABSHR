using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HR.Entity;

namespace HR.Models
{
    public class CountryViewModel : BaseViewModel
    {
        public Country Country { get; set; }
        //public PublicHoliday PublicHoliday { get; set; }
    }
}