using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMind.Domain.Dto
{
    public class ExternalProjectDto
    {
        public string Name { get; set; } = string.Empty;        // GitHub name
        public string? Description { get; set; }               // GitHub description
        public DateTimeOffset? Updated_at { get; set; }        // GitHub updated_at
    }


}
