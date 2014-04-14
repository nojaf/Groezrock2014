using Groezrock2014.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.Services
{
    public class DesignService:IGroezrockService
    {
        private Schedule[] _schedules;

        public DesignService()
        {
            InitData();
        }

        private void InitData()
        {
            Schedule dayOne = new Schedule()
            {
                Date = GroezrockConstants.DayOne,
                Stages = new List<Stage>
                {
                    new Stage(){
                        Id = 1,
                        Name = "Monster Main Stage",
                        Bands = new List<Band>{
                            new Band(){
                                Id = 1,
                                Name = "NoFx",
                                Starts = new DateTime(2014,5,3,0,5,0),
                                Ends = new DateTime(2014,5,3,1,20,0),
                                Stage = "Monster Main Stage"
                            },
                            new Band(){
                                Id = 2,
                                Name = "Brand New",
                                Starts = new DateTime(2014,5,2,22,35,0),
                                Ends = new DateTime(2014,5,2,23,35,0),
                                Stage = "Monster Main Stage"
                            }
                        }
                    },
                    new Stage(){
                        Id = 1,
                        Name = "Impericon Stage",
                        Bands = new List<Band>{
                            new Band(){
                                Id = 1,
                                Name = "Quicksand",
                                Starts = new DateTime(2014,5,2,23,5,0),
                                Ends = new DateTime(2014,5,3,0,05,0),
                                Stage = "Impericon Stage"
                            },
                            new Band(){
                                Id = 2,
                                Name = "Taking Back Sunday",
                                Starts = new DateTime(2014,5,2,21,35,0),
                                Ends = new DateTime(2014,5,2,22,35,0),
                                Stage = "Impericon Stage"
                            },
                            new Band(){
                                Id = 2,
                                Name = "Ignite",
                                Starts = new DateTime(2014,5,2,20,20,0),
                                Ends = new DateTime(2014,5,2,21,10,0),
                                Stage ="Impericon Stage"
                            }
                        }
                    }
                }
            };

            Schedule dayTwo = new Schedule()
            {
                Date = GroezrockConstants.DayOne,
                Stages = new List<Stage>
                {
                    new Stage(){
                        Id = 1,
                        Name = "Monster Main Stage",
                        Bands = new List<Band>{
                            new Band(){
                                Id = 1,
                                Name = "The Offspring",
                                Starts = new DateTime(2014,5,3,0,5,0),
                                Ends = new DateTime(2014,5,3,1,20,0)
                            },
                            new Band(){
                                Id = 2,
                                Name = "The Hives",
                                Starts = new DateTime(2014,5,2,22,35,0),
                                Ends = new DateTime(2014,5,2,23,35,0)
                            }
                        }
                    },
                    new Stage(){
                        Id = 1,
                        Name = "Impericon Stage",
                        Bands = new List<Band>{
                            new Band(){
                                Id = 1,
                                Name = "Falling In Reverse",
                                Starts = new DateTime(2014,5,2,23,5,0),
                                Ends = new DateTime(2014,5,3,0,05,0)
                            },
                            new Band(){
                                Id = 2,
                                Name = "Caliban",
                                Starts = new DateTime(2014,5,2,21,35,0),
                                Ends = new DateTime(2014,5,2,22,35,0)
                            },
                            new Band(){
                                Id = 2,
                                Name = "Ignite",
                                Starts = new DateTime(2014,5,2,20,20,0),
                                Ends = new DateTime(2014,5,2,21,10,0)
                            }
                        }
                    }
                }
            };

            _schedules = new[] { dayOne, dayTwo };
        }

        public Task<Models.Band> GetBand(string name)
        {
            return Task.Run<Band>(() =>
            {
                var bands = _schedules.SelectMany(x => x.Stages.SelectMany(y => y.Bands)).ToArray();
                return bands.FirstOrDefault(x => x.Name == name);
            });
        }


        public Task<Schedule[]> GetSchedules()
        {
            return Task.Run<Schedule[]>(() =>
            {
                return _schedules;
            });
        }


        public Task SetActiveBand(string bandName)
        {
            throw new NotImplementedException();
        }

        public Band SelectedBand
        {
            get { 
                return new Band(){
                    Bio = 
@"At the end of the nineties, around the turn of the millenium the emo hype reached its climax. 
Every band that played a kind of melodic guitar rock seemed to have some heartache. 
The originality was on a very low level and the genre got less convincing. 
But every now and then some bands turned up who released a record that gave reason to the existence of 'emo'. 
'Your New Favourite Weapon' was such a record, Brand New was such a band. 
Strong, ultra catchy, recognizable, quirky, deep and deliciously shallow, they had it all. 
Since then Brand New proved with every new album (they've already released 4 of them)
that they are a very extraordinary and talented band. 
This year we welcome them for the first time at Groezrock and it's no surprise that we're looking forward to this! 
 
[nl]
We schrijven het hoogtepunt van de emo-hype, het einde van de nineties, begin van de jaren 2000, 
elke band die ietwat melodieuze gitaarrock maakte bleek over te lopen van hartzeer. 
De originaliteit was ver zoek en het genre werd steeds minder overtuigend. 
Maar af en toe doken er ineens bands op en kwamen er platen uit die kort, 
krachtig en met verve het bestaansrecht van ‘emo’ teruggaven. 
‘Your New Favourite Weapon’ was zo’n plaat, Brand New was zo’n band. 
Krachtig, ultracatchy, herkenbaar, quirky, diep en toch lekker oppervlakkig, 
ze hadden en brachten het allemaal. 
Ondertussen bewees Brand New met elke nieuwe plaat (ze zitten nu aan hun vierde) dat ze een uitzonderlijk getalenteerde band is. 
Dit jaar kunnen wij van Groezrock ze voor de eerste keer verwelkomen op ons festival, en dat we ernaar uitkijken mag geen verrassing zijn!",
                    Name = "The Lawrence army long name",
                    Starts = new DateTime(2014,5,2,22,35,0),
                    Ends = new DateTime(2014,5,2,23,35,0),
                    AddToMySchedule=true
                };
            }
        }


        public string GetStageFromBand(string bandName)
        {
            return "Monster Main Stage";
        }


        public Task<Band[]> GetMySchedule()
        {
            return Task.Factory.StartNew<Band[]>(() =>
            {
                return new Band[]{
                    new Band(){
                        Id = 1,
                        Name = "NoFx",
                        Starts = new DateTime(2014,5,3,0,5,0),
                        Ends = new DateTime(2014,5,3,1,20,0),
                        Stage = "Monster"
                    },
                    new Band(){
                        Id = 2,
                        Name = "Brand New",
                        Starts = new DateTime(2014,5,2,22,35,0),
                        Ends = new DateTime(2014,5,2,23,35,0),
                        Stage = "Monster"
                    },
                    new Band(){
                        Id = 2,
                        Name = "Ignite",
                        Starts = new DateTime(2014,5,2,20,20,0),
                        Ends = new DateTime(2014,5,2,21,10,0),
                        Stage = "Impericon"
                    }
                };
            });
        }


        public void Persist()
        {
            //whatever
        }


        public Task<Band[]> GetAllBands()
        {
            return Task.Factory.StartNew(() =>
            {
                return new Band[]{
                        new Band(){
                            Id = 1,
                            Name = "NoFx",
                            Starts = new DateTime(2014,5,3,0,5,0),
                            Ends = new DateTime(2014,5,3,1,20,0),
                            Stage = "Monster Main Stage"
                        },
                        new Band(){
                            Id = 2,
                            Name = "NoFx 2",
                            Starts = new DateTime(2014,5,2,22,35,0),
                            Ends = new DateTime(2014,5,2,23,35,0),
                            Stage = "Monster Main Stage"
                        },
                        new Band(){
                            Id = 2,
                            Name = "Alkaline Trio",
                            Starts = new DateTime(2014,5,2,22,35,0),
                            Ends = new DateTime(2014,5,2,23,35,0),
                            Stage = "Monster Main Stage"
                        }
                    };
            });
        }
    }
}
//22:35-23:35