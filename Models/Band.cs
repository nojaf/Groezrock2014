using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Groezrock2014.Models
{
    public class Band
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public string Link { get; set; }

        public string PlaysAtFull
        {
            get
            {
                return Starts.ToString("MMMM d ,") + " " + Starts.ToString("HH:mm") + "-" + Ends.ToString("HH:mm") + " @ " + Stage;
            }
        }

        public string PlaysAt
        {
            get
            {
                return Starts.ToString("HH:mm") + "-" + Ends.ToString("HH:mm");
            }
        }

        public string ImageFileName { get; set; }
        public string Stage { get; set; }

        public bool AddToMySchedule
        {
            get;
            set;
        }



        [JsonIgnore]
        public BitmapImage Image { get; set; }

        public override string ToString()
        {
            return Name + " [" + Starts.ToString("HH:mm") + "-" + Ends.ToString("HH:mm") + "]";
        }

        public int Day { get; set; }
    }
}

/*
- Id
- Name
- Image
- Bio
- Starts
- Ends
- Link
- Cached?
 */