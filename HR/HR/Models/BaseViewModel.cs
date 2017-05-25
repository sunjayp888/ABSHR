using HR.Entity.Dto;

namespace HR.Models
{
    public class BaseViewModel
    {
        public string OrganisationName { get; set; }
        public int PersonnelId { get; set; }
        public Permissions Permissions { get; set; }
    }
}