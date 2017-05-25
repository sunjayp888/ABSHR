namespace HR.Entity
{
    using Dto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ApprovalStateMetadata))]
    public partial class ApprovalState
    {       
        //public enum State
        //{
        //    Unapproved = 1,
        //    Approved = 2,
        //    Declined = 3
        //}

        public ApprovalStates AsState => (ApprovalStates)ApprovalStateId;

        public string ApprovalStateIcon
        {
            get
            {
                switch (AsState)
                {
                    //case ApprovalStates.Requested:
                    //    return "fa-question-circle";
                    //case ApprovalStates.InApproval:
                    //    return "fa-question-circle";
                    case ApprovalStates.Approved:
                        return "fa-check-circle";
                    case ApprovalStates.Declined:
                        return "fa-times-circle";
                    
                }

                return string.Empty;
            }
        }

        private class ApprovalStateMetadata
        {
            [JsonProperty("name")]
            public string Name { get; set; }

        }
    }
}