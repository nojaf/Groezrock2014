using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace Groezrock2014.UserControls
{
    public partial class UCBackground : UserControl
    {
        public UCBackground()
        {
            InitializeComponent();
            if (false)
            {
                //Fade.Begin();
            }
        }

        public void ForwardTo(TimeSpan offset)
        {
            Fade.Seek(offset);
        }
    }
}
