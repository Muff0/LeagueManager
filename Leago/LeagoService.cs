using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagoService
{
    internal class LeagoService
    {

        public Task<string> GetMessageAsync()
        {
            return Task.FromResult("Hello from the backend service!");
        }
    }
}
