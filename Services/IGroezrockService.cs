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
        Task<Band> GetBand(string name);

        Task<Band> GetBand(int id);

        Task<Schedule[]> GetSchedules();

        void SetActiveBand(string bandName);

        Band SelectedBand {  get; }
    }
}
