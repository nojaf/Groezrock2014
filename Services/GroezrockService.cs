using Groezrock2014.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Groezrock2014.Extensions;



namespace Groezrock2014.Services
{
    public class GroezrockService:IGroezrockService
    {
        private Schedule[] _schedules = null;
        private CachedGroezrockService Cache { get; set; }
        private Band _selectedBand = null;

        public GroezrockService()
        {
            Cache = new CachedGroezrockService();
        }

        #region Band
        private async Task<Band> GetBand(string name)
        {
            Band band = _schedules.SelectMany(x => x.Stages.SelectMany(y => y.Bands)).FirstOrDefault(x => x.Name == name);
            if(band != null)
            {
                if(string.IsNullOrEmpty(band.ImageFileName) || string.IsNullOrEmpty(band.Bio))
                {
                    //load band
                    band = await FetchAdditionalBandData(band);
                    await Cache.Persist(_schedules);
                }else if(!string.IsNullOrEmpty(band.ImageFileName))
                {
                    band.Image = await Cache.LoadImage(band.ImageFileName);
                }
            }

            return band;
          
        }

        /// <summary>
        /// Fetch the image and the bio from the groezrock/bands/<band> url
        /// </summary>
        /// <param name="band"></param>
        /// <returns></returns>
        private async Task<Band> FetchAdditionalBandData(Band band)
        {
            string filteredName = FilterName(band.Name);
            string url = GroezrockConstants.BandUrl + filteredName;
            string html;
            using(HttpClient client = new HttpClient())
            {
                html = await client.GetStringAsync(url);
            }
            if(!string.IsNullOrEmpty(html))
            {
                html = html.Replace("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">", "");
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                //image
                var img = htmlDoc.DocumentNode.SelectSingleNode("//div[@class ='band-img']/img");
                if(img != null)
                {
                    string src = GroezrockConstants.GroezrockUrl + img.Attributes["src"].Value;
                    if(!string.IsNullOrEmpty(src))
                    {
                        await GetBandImage(src, band, filteredName);
                    }
                }

                //bio
                var bioParagraphs = htmlDoc.DocumentNode.SelectNodes("//div[@class ='mod-bottom']/p");
                band.Bio = string.Join("\n", bioParagraphs.Select(x => x.InnerText));
                band.Bio = band.Bio.Replace("&nbsp;", "\n");
            }

            return band;
        }

        /// <summary>
        /// Image request and save to local storage
        /// </summary>
        /// <param name="src"></param>
        /// <param name="band"></param>
        /// <param name="filteredName"></param>
        /// <returns></returns>
        private async Task GetBandImage(string src, Band band, string filteredName)
        {
            using(HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(src);
                Stream imageStream = await response.Content.ReadAsStreamAsync();
                BitmapImage image = new BitmapImage();
                image.SetSource(imageStream);

                string fileName = filteredName +"."+ src.Split('.').Last();
                byte[] imageBytes = imageStream.ToByteArray();


                //save the image
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile storedImage = await localFolder.CreateFileAsync(fileName,CreationCollisionOption.ReplaceExisting);
                var writeStream = await storedImage.OpenStreamForWriteAsync();
                await writeStream.WriteAsync(imageBytes, 0, imageBytes.Length);

                band.Image = image;
                band.ImageFileName = fileName;
            }
        }

        private string FilterName(string name)
        {
            name = name.ToLower();
            name = name.Replace(' ','-');
            name = name.Replace(",","");
            name = name.Replace("!","");
            return name;
        }

        private Band GetBandFromHtmlNode(HtmlNode htmlNode, int id, DateTime date)
        {
            try
            {
                Band band = new Band()
                {
                    Id = id,
                    Name = htmlNode.FirstChild.FirstChild.InnerText
                };

                //17:20-18:05 (0:45)
                int indexBr = htmlNode.InnerHtml.IndexOf("<br>");
                string playTime = htmlNode.InnerHtml.Substring(indexBr, htmlNode.InnerHtml.Length - indexBr);
                playTime = Regex.Replace(playTime, "[^0-9.]", "");

                int startHour = int.Parse(playTime.Substring(0, 2));
                int startMinute = int.Parse(playTime.Substring(2, 2));

                int day = date.Day;
                if (startHour < 12)
                {
                    day++;
                }

                band.Starts = new DateTime(date.Year, date.Month, day, startHour, startMinute, 0);

                int endHour = int.Parse(playTime.Substring(4, 2));
                int endMinute = int.Parse(playTime.Substring(6, 2));
                band.Ends = new DateTime(date.Year, date.Month, day, endHour, endMinute, 0);

                

                return band;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;

        }
        #endregion

        #region Schedule
        private async Task<Schedule> GetSchedule(DateTime dateTime)
        {
            Schedule schedule = new Schedule()
            {
                Date = dateTime
            };
            //2014-05-03
            string html;
            using (HttpClient client = new HttpClient())
            {
                html = await client.GetStringAsync(GroezrockConstants.ScheduleUrl + dateTime.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">", "");
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var divs = htmlDoc.DocumentNode.SelectNodes("//div[@class = 'sched-stage' or @class = 'sched-band-box']");
                List<Stage> stages = new List<Stage>();
                Stage currentStage = null;

                int length = divs.Count;
                for (int i = 0; i < length; i++)
                {
                    if (divs[i] == null) continue;
                    if (divs[i].Attributes["class"] == null) continue;


                    string className = divs[i].Attributes["class"].Value;
                    if (className == "sched-stage")
                    {
                        Stage stage = GetStage(divs[i], i);
                        stages.Add(stage);
                        currentStage = stage;
                    }
                    else if (className == "sched-band-box")
                    {
                        Band band = GetBandFromHtmlNode(divs[i], i, dateTime);
                        band.Stage = currentStage.Name;
                        band.Day = schedule.Date.Day;
                        currentStage.Bands.Add(band);
                    }
                }

                schedule.Stages = stages;
            }
            return schedule;
        }

        private static Stage GetStage(HtmlNode div, int id)
        {
            Stage stage = new Stage()
            {
                Bands = new List<Band>(),
                Id = id,
                Name = div.InnerText
            };
            return stage;
        }

        public async Task<Schedule[]> GetSchedules()
        {
            if (_schedules == null)
            {
                FoundSchedules foundSchedules = await Cache.HasSchedules();
                if (foundSchedules.Found)
                {
                    _schedules = await Cache.GetSchedules(foundSchedules.File);
                }
                else
                {
                    Schedule dayOne = await GetSchedule(GroezrockConstants.DayOne);
                    Schedule dayTwo = await GetSchedule(GroezrockConstants.DayTwo);
                    _schedules = new[] { dayOne, dayTwo };
                    await Cache.Persist(_schedules);
                }
            }

            return _schedules;
        }
        #endregion

        public async Task SetActiveBand(string bandName)
        {
            _selectedBand = await GetBand(bandName);
        }

        public Band SelectedBand
        {
            get { return _selectedBand; }
        }

        public string GetStageFromBand(string bandName)
        {
            return _schedules.SelectMany(x => x.Stages).FirstOrDefault(x => x.Bands.Any(y => y.Name == bandName)).Name; 
        }

        public async Task<Band[]> GetMySchedule()
        {
            if (_schedules == null) _schedules = await GetSchedules();
            var bandIWantToSee = _schedules.SelectMany(x => x.Stages.SelectMany(y => y.Bands)).Where(b => b.AddToMySchedule).ToArray();
            return bandIWantToSee;
        }

        public async void Persist()
        {
            if (_schedules == null) return;
            await Cache.Persist(_schedules);
        }


        public async Task<Band[]> GetAllBands()
        {
            if (_schedules == null) _schedules = await GetSchedules();

            return _schedules.SelectMany(x => x.Stages.SelectMany(y => y.Bands)).ToArray();
        }
    }

    public class CachedGroezrockService
    {
        public async Task<FoundSchedules> HasSchedules()
        {
            FoundSchedules foundSchedules = new FoundSchedules();
            try
            {
                foundSchedules.Found = true;
                foundSchedules.File = await ApplicationData.Current.LocalFolder.GetFileAsync(GroezrockConstants.CacheFile);
                //no exception means file exists
            }
            catch (FileNotFoundException)
            {
                //find out through exception 
                foundSchedules.Found = false;
            }
            return foundSchedules;
        }

        public async Task<Schedule[]> GetSchedules(StorageFile textFile)
        {
            // Getting JSON from file
            using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
            {
                // Read text stream     
                using (DataReader textReader = new DataReader(textStream))
                {
                    //get size                       
                    uint textLength = (uint)textStream.Size;
                    await textReader.LoadAsync(textLength);
                    // read it                    
                    string json = textReader.ReadString(textLength);

                    Schedule[] cachedSchedules = JsonConvert.DeserializeObject<Schedule[]>(json);
                    return cachedSchedules;
                }
            }
        }

        async internal Task Persist(Schedule[] schedules)
        {
            string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(schedules));
            StorageFile dataFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(GroezrockConstants.CacheFile, CreationCollisionOption.ReplaceExisting);
            // Open the file...      
            using (IRandomAccessStream textStream = await dataFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // write the JSON string!
                using (DataWriter textWriter = new DataWriter(textStream))
                {
                    textWriter.WriteString(json);
                    await textWriter.StoreAsync();
                }
            }
        }

        internal async Task<BitmapImage> LoadImage(string path)
        {
            BitmapImage image = new BitmapImage();
            Stream imageStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(path);
            image.SetSource(imageStream);
            return image;
        }
    }
}
