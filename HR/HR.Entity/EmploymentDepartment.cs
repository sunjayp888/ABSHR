namespace HR.Entity
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EmploymentDepartment")]
    public partial class EmploymentDepartment
    {
        public int EmploymentDepartmentId { get; set; }

        public int OrganisationId { get; set; }

        public int EmploymentId { get; set; }

        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Employment Employment { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
