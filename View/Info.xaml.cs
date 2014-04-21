using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Groezrock2014.Messages;

namespace Groezrock2014.View
{
    public partial class Info : PhoneApplicationPage
    {
        public Info()
        {
            InitializeComponent();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<HtmlInfoMessage>(this, ReceiveMessage);
        }

        private void ReceiveMessage(HtmlInfoMessage message)
        {
            this.webView.NavigateToString(message.Html);
        }
    }
}