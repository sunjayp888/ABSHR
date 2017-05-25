using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.Entity.Interfaces;

namespace HR.Entity
{
    [MetadataType(typeof(PersonnelSearchFieldMetaData))]
    public partial class PersonnelSearchField : IOrganisationFilterable
    {
        private class PersonnelSearchFieldMetaData
        {

        }
    }
}
