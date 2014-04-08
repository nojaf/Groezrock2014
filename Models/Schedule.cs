using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.Models
{
    public class Schedule
    {
        public DateTime Date { get; set; }
        public IEnumerable<Stage> Stages { get; set; }
    }
}
