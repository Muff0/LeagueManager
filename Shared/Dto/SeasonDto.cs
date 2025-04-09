using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class SeasonDto
    {
        public string LeagoL1Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public int Id { get; set; }
        public string LeagoL2Key { get; set; } = string.Empty;
    }
}
