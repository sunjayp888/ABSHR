namespace HR.Entity
{
    public partial class Frequency
    {

        public enum FrequencyType
        {
            Yearly = 1,
            Quarterly = 2
        }

        public FrequencyType AsFrequencyType => (FrequencyType)FrequencyId;

    }
}
