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
        Task<Schedule[]> GetSchedules();

        Task SetActiveBand(string bandName);

        Band SelectedBand {  get; }

        string GetStageFromBand(string bandName);

        Task<Band[]> GetMySchedule();

        Task<Band[]> GetAllBands();

        void Persist();

        Task LoadAll(IProgress<int> progress);

        Task<Info[]> GetAllInfo();

        Task<Info> GetInfo(string title);
    }
}
