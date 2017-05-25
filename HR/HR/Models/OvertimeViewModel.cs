using HR.Entity;
using HR.Entity.Dto;
using System.Web.Mvc;

namespace HR.Models
{
    public class OvertimeViewModel : BaseViewModel
    {
        public bool CanApprovedOvertime { get; set; }
        public Overtime Overtime { get; set; }
        public OvertimeSummary OvertimeSummary { get; set; }
        public SelectList OvertimePreferences { get; set; }
    }
}
