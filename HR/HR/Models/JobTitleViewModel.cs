using HR.Entity;
using System.Collections.Generic;

namespace HR.Models
{
    public class JobTitleViewModel : BaseViewModel
    {
        public JobTitle JobTitle { get; set; }
        public List<JobGrade> JobGrades { get; set; }
        //public JobTitleDocument JobTitleDocument { get; set; }
    }
}