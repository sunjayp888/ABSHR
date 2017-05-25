using HR.Entity;

namespace HR.Models
{
    public class PublicHolidayPolicyViewModel : BaseViewModel
    {
        public PublicHolidayPolicy PublicHolidayPolicy { get; set; }
        public PublicHoliday PublicHoliday { get; set; }
    }
}