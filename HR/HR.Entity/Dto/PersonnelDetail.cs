using System.Collections.Generic;

namespace HR.Entity.Dto
{
    public class PersonnelDetail
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public List<Colour> Colours { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public int JobTitleId { get; set; }
        public string Name
        {
            get
            {
                return Forename + ' ' + Surname;
            }
        }
        string _JobTitle;
        public string JobTitle
        {
            get
            {
                return "Job Title: " + _JobTitle;
            }
            set
            {
                _JobTitle = value;
            }
        }
        public string Photo { get; set; }
        public bool showLink { get; set; }
    }
}
