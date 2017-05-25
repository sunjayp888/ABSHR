using System.Collections.Generic;
using HR.Entity;

namespace HR.Models
{
    public class DepartmentViewModel : BaseViewModel
    {
        public Department Department { get; set; }
        public List<Colour> ColoursList { get; set; }
    }
}