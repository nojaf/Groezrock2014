using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.Services
{
    public interface INavigationService
    {
        void Navigate(string location);

        PhoneApplicationFrame RootFrame { get; }
    }
}
