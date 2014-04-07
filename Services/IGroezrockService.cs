using Groezrock2014.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.Services
{
    public interface IGroezrockService
    {
        Band GetBand(string name);

        Band GetBand(int id);

        FestivalDay GetFestivalDay(int id);
    }
}
