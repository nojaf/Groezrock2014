using GalaSoft.MvvmLight;
using Groezrock2014.Models;
using Groezrock2014.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.ViewModel
{
    public class AllBandsViewModel:ViewModelBase
    {
        private IGroezrockService _groezrockService;

        /// <summary>
        /// The <see cref="Bands" /> property's name.
        /// </summary>
        public const string BandsPropertyName = "Bands";

        private List<AlphaKeyGroup<Band>> _bands = null;

        /// <summary>
        /// Sets and gets the Bands property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<AlphaKeyGroup<Band>> Bands
        {
            get
            {
                return _bands;
            }

            set
            {
                if (_bands == value)
                {
                    return;
                }

                RaisePropertyChanging(BandsPropertyName);
                _bands = value;
                RaisePropertyChanged(BandsPropertyName);
            }
        }

        public AllBandsViewModel(IGroezrockService groezrockService)
        {
            _groezrockService = groezrockService;

            InitData();
        }

        private async void InitData()
        {
            var allBands = await _groezrockService.GetAllBands();
            Bands = AlphaKeyGroup<Band>.CreateGroups(allBands, b => b.Name, true);
        }
    }
}
