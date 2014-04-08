using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.Services
{
    public class GroezrockNavigationService:INavigationService
    {
        public PhoneApplicationFrame RootFrame { get; set; }

        public GroezrockNavigationService(PhoneApplicationFrame rootframe)
        {
            RootFrame = rootframe;
        }

        public void Navigate(string location)
        {
            RootFrame.Navigate(new Uri(location, UriKind.Relative));
        }
    }
}
