using HR.Business.Models;
using HR.Entity;

namespace HR.Models
{
    public class ApprovalLevelUserViewModel : BaseViewModel
    {
        public ApprovalUser ApprovalUser { get; set; }
        public ApprovalLevelUser ApprovalLevelUser { get; set; }
    }
}
