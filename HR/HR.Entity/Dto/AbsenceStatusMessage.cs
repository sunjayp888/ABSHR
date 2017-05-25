namespace HR.Entity.Dto
{
    public class AbsenceStatusMessage
    {
        public decimal Duration { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string AbsenceType { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string LinkUrl { get; set; }
        public string LinkText { get; set; }
    }
}
