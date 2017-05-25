using HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Interfaces
{
    public interface IHRDatabaseFactory
    {
        HRDatabase Create();
        HRDatabase Create(int organisationId);
    }
}
