using System.Web.Mvc;
using HR.Entity;

namespace HR.Models
{
    public class SiteViewModel : BaseViewModel
    {
        public SelectList Countries { get; set; }
        public Site Site { get; set; }
    }
}