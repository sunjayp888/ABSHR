using HR.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HR.Models
{
    public class AbsenceTypeViewModel : BaseViewModel
    {
        [Remote("AbsenceTypeAlreadyExists", "AbsenceType")]
        [Editable(true)]
        public string Name { get; set; }
        public AbsenceType AbsenceType { get; set; }
        public List<Colour> ColoursList { get; set; }
    }
}