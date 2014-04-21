using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.Messages
{
    public class HtmlInfoMessage
    {
        public HtmlInfoMessage()
        {

        }

        public HtmlInfoMessage(string html)
        {
            Html = html;
        }

        public string Html { get; set; }
    }
}
