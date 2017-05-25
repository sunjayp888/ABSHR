using System.Web.Mvc;
using HR.Entity;

namespace HR.Models
{
    public class BuildingViewModel : BaseViewModel
    {
        public SelectList Sites { get; set; }
        public Building Building { get; set; }
    }
}