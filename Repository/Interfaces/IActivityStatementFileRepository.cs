using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repository.Interfaces
{
    public interface IActivityStatementFileRepository : ICrudBaseRepository<ActivityStatementFile, Guid>
    {
    }
}
