using HR.Entity;
using System.Collections.Generic;

namespace HR.Models
{
    public class CompanyViewModel : BaseViewModel
    {
        public Company Company { get; set; }
        public List<Colour> ColoursList { get; set; }
    }
}