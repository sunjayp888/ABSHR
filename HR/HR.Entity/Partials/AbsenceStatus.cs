namespace HR.Entity
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(AbsenceStatusMetadata))]
    public partial class AbsenceStatus
    {       
        public enum Status
        {
            Unapproved = 1,
            Approved = 2,
            Declined = 3
        }

        public Status AsStatus => (Status)AbsenceStatusId;

        public string AbsenceStatusIcon
        {
            get
            {
                switch (AsStatus)
                {
                    case Status.Unapproved:
                        return "fa-question-circle";
                    case Status.Approved:
                        return "fa-check-circle";
                    case Status.Declined:
                        return "fa-times-circle";
                    
                }

                return "fa-question-circle";
            }
        }

        private class AbsenceStatusMetadata
        {
            [JsonProperty("name")]
            public string Name { get; set; }

        }
    }
}