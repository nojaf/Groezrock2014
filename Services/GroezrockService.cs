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
using Windows.System.Threading;
using System.IO.IsolatedStorage;
using System.Windows;



namespace Groezrock2014.Services
{
    public class GroezrockService:IGroezrockService
    {
        private Schedule[] _schedules = null;
        private CachedGroezrockService Cache { get; set; }
        private Band _selectedBand = null;
        private Info[] _infoItems = null;


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
                    await Cache.PersistSchedules(_schedules);
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
            name = name.Replace("-", "");
            name = name.Replace(' ','-');
            name = name.Replace(",","");
            name = name.Replace("!","");
            name = name.Replace("é", "");
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
                FoundFile foundSchedules = await Cache.HasFile(GroezrockConstants.CacheFile);
                if (foundSchedules.Found)
                {
                    _schedules = await Cache.ReadFromStorage<Schedule>(foundSchedules.File);
                }
                else
                {
                    Schedule dayOne = await GetSchedule(GroezrockConstants.DayOne);
                    Schedule dayTwo = await GetSchedule(GroezrockConstants.DayTwo);
                    _schedules = new[] { dayOne, dayTwo };
                    await Cache.PersistSchedules(_schedules);
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
            await Cache.PersistSchedules(_schedules);
        }

        public async Task<Band[]> GetAllBands()
        {
            if (_schedules == null) _schedules = await GetSchedules();

            return _schedules.SelectMany(x => x.Stages.SelectMany(y => y.Bands)).ToArray();
        }

        public async Task LoadAll(IProgress<int> progress)
        {
            progress.Report(1);
            Band[] allBands = await GetAllBands();
            int length = allBands.Length;
            Info[] allInfo = await GetAllInfo();
            int infoLength = allInfo.Length;

            for (int i = 0; i < length; i++)
            {
                try
                {
                    allBands[i] = await GetBand(allBands[i].Name);              
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to load " + allBands[i].Name + " " + ex.Message);
                }

                double procent = (double)i / ((double)length + (double)infoLength);
                progress.Report((int)Math.Round(procent * 100, 0));
            }

            for (int j = 0; j < infoLength; j++)
            {
                try
                {
                    allInfo[j] = await GetInfo(allInfo[j].Title);              
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to load inf " + allInfo[j].Title + " " + ex.Message);
                }

                double procent = ((double)length + (double)j) / ((double)length + (double)infoLength);
                progress.Report((int)Math.Round(procent * 100, 0));
            }

            await Cache.PersistSchedules(_schedules);
            progress.Report(100);
        }

        #region info
        public async Task<Info[]> GetAllInfo()
        {
            if (_infoItems == null)
            {
                FoundFile foundFile = await Cache.HasFile(GroezrockConstants.InfoFile);
                if(foundFile.Found)
                {
                    _infoItems = await Cache.ReadFromStorage<Info>(foundFile.File);
                    await Cache.PersistInfo(_infoItems);
                }
                else
                {
                    await LoadInfo();
                }
            }
                
            return _infoItems;
        }

        private async Task LoadInfo()
        {
            string infoHtml = null;
            using(HttpClient client = new HttpClient())
            {
                infoHtml = await client.GetStringAsync(GroezrockConstants.InfoUrl);
            }

            if(infoHtml != null)
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(infoHtml);

                var links = htmlDoc.DocumentNode.SelectNodes("//nav[@id = 'nav-info']/ul/li/a");
                int length = links.Count;
                _infoItems = new Info[length];
                for (int i = 0; i < length; i++)
                {
                    _infoItems[i] = new Info()
                    {
                        Title = links[i].InnerText,
                        Url = links[i].Attributes["href"].Value
                    };
                }
            }
        }

        public async Task<Info> GetInfo(string title)
        {
            if (_infoItems == null) await LoadInfo();
            Info info = _infoItems.FirstOrDefault(x => x.Title == title);

            if(string.IsNullOrEmpty(info.HtmlContent))
            {
                info = await LoadInfoFull(info);
                await Cache.PersistInfo(_infoItems);
            }

            return info;
        }

        private async Task<Info> LoadInfoFull(Info info)
        {
            string html;
            using(HttpClient client = new HttpClient())
            {
                html = await client.GetStringAsync(GroezrockConstants.GroezrockUrl + info.Url.Substring(1,info.Url.Length - 1));
            }

            if(!string.IsNullOrEmpty(html))
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var article = htmlDoc.DocumentNode.SelectSingleNode("//article");
                HtmlDocument resultDoc = new HtmlDocument();
                resultDoc.LoadHtml(@"<html><head><link href=\""/html/style.css\"" rel=\""stylesheet\"" type=\""text/css\"" /></head><body></body></html>");
              //  styleNode.InnerText = "padding:15%;background-color:transparent;color:white;";
                resultDoc.DocumentNode.LastChild.LastChild.AppendChild(article);
                info.HtmlContent = resultDoc.DocumentNode.OuterHtml;
            }

            return info;
        }
        #endregion
    }

    public class CachedGroezrockService
    {
        public async Task<FoundFile> HasFile(string file)
        {
            FoundFile foundSchedules = new FoundFile();
            try
            {
                foundSchedules.Found = true;
                foundSchedules.File = await ApplicationData.Current.LocalFolder.GetFileAsync(file);
                //no exception means file exists
            }
            catch (FileNotFoundException)
            {
                //find out through exception 
                foundSchedules.Found = false;
            }
            return foundSchedules;
        }


        public async Task<T[]> ReadFromStorage<T>(StorageFile textFile)
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

                    T[] cachedItems = JsonConvert.DeserializeObject<T[]>(json);
                    return cachedItems;
                }
            }
        }



        async internal Task PersistSchedules(Schedule[] schedules)
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
        async internal Task PersistInfo(Info[] info)
        {
            string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(info));
            StorageFile dataFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(GroezrockConstants.InfoFile, CreationCollisionOption.ReplaceExisting);
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
