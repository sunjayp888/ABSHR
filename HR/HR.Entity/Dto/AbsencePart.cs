using System.ComponentModel.DataAnnotations;

namespace HR.Entity.Dto
{
    public enum AbsencePart
    {
        [Display(Name = "Full Day")]
        FullDay = 1,
        [Display(Name = "Half Day AM")]
        HalfDayAM = 2,
        [Display(Name = "Half Day PM")]
        HalfDayPM = 3        
    }
}