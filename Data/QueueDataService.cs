using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data
{
    public class QueueDataService : DataServiceBase<QueueContext>
    {
        public QueueDataService(IDbContextFactory<QueueContext> contextFactory) : base(contextFactory)
        {
        }
    }
}
