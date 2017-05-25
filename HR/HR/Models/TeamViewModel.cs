using HR.Entity;
using System.Collections.Generic;

namespace HR.Models
{
    public class TeamViewModel : BaseViewModel
    {
        public Team Team { get; set; }

        public List<Colour> ColoursList { get; set; }

    }
}
