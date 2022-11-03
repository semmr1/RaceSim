using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Controller
{
    public class Race
    {
        public Race(Track track, List<IParticipant> participants)
        {
            Track = track;
            Participants = participants;
            driversOnTrack = Participants.Count;
            _positions = new Dictionary<Section, SectionData>();
            _random = new Random(DateTime.Now.Millisecond);
            RandomizeEquipment();
            DetermineStartPosition(Track, Participants);
            _timer = new Timer();
            _timer.Interval = 250;
            _timer.Elapsed += OnTimedEvent;
        }

        private Random _random;
        private Timer _timer;
        private const int sectionDistance = 100;
        private const int lapCount = 1;
        private static int propability = 0;
        private static int driversOnTrack;

        public event EventHandler<DriversChangedEventArgs> DriversChanged;
        public event EventHandler<RaceEndEventArgs> RaceEnded;
        public Track Track { get; set; }

        public List<IParticipant> Participants { get; set; }

        public DateTime StartTime { get; set; }

        private Dictionary<Section, SectionData> _positions;

        private void OnTimedEvent(object source, EventArgs args)
        {
            if (driversOnTrack == 0)
            {
                CleanUpRace();
                Data.NextRace();
                RaceEnded?.Invoke(null, new RaceEndEventArgs());
            } else
            {
                CheckForDamage();
                MoveAroundDrivers();
                DriversChanged?.Invoke(this, new DriversChangedEventArgs(Track));
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public SectionData GetSectionData(Section section)
        {
            if (_positions.ContainsKey(section))
            {
                return _positions[section];
            }

            var posData = new SectionData();
            _positions.Add(section, posData);
            return posData;
        }

        private void RandomizeEquipment()
        {
            foreach (var participant in Participants)
            {
                var equipment = participant.Equipment;
                equipment.Performance = _random.Next(10, 40);
                equipment.Speed = _random.Next(70, 100);
                var speed = (equipment.Performance + equipment.Speed) / equipment.Quality;
                Debug.WriteLine($">> [{participant.Name}] driving {speed}km/h | {equipment.Performance}p | {equipment.Quality}q");
            }
        }

        private void MoveAroundDrivers()
        {
            foreach (IParticipant participant in Participants)
            {
                Driver driver = (Driver)participant;
                if(!driver.Equipment.IsBroken)
                {
                    MoveIndividualDriver(driver);
                }
            }
        }

        private void MoveIndividualDriver(Driver driver)
        {
            var speed = (driver.Equipment.Performance + driver.Equipment.Speed) / driver.Equipment.Quality;
            foreach (Section section in Track.Sections)
            {
                var sectionData = GetSectionData(section);
                var nextSection = Track.NextSection(section);
                var nextSectionData = GetSectionData(nextSection);
                if (sectionData.Left is not null || sectionData.Right is not null)
                {
                    if (sectionData.Left == driver)
                    {
                        if ((sectionData.DistanceLeft - speed) > 0)
                        {
                            sectionData.DistanceLeft -= speed;
                        }
                        else
                        {
                            if (nextSectionData.Left == null)
                            {
                                nextSectionData.Left = driver;
                                sectionData.Left = null;
                                sectionData.DistanceLeft = sectionDistance;
                                PassedFinishCheck(section, nextSectionData, driver);
                                break;
                            }
                            else if (nextSectionData.Right == null)
                            {
                                nextSectionData.Right = driver;
                                sectionData.Left = null;
                                sectionData.DistanceLeft = sectionDistance;
                                PassedFinishCheck(section, nextSectionData, driver);
                                break;
                            }
                        }
                    }
                    else if (sectionData.Right == driver)
                    {

                        if ((sectionData.DistanceRight - speed) > 0)
                        {
                            sectionData.DistanceRight -= speed;
                        }
                        else
                        {
                            if (nextSectionData.Right == null)
                            {
                                nextSectionData.Right = driver;
                                sectionData.Right = null;
                                sectionData.DistanceRight = sectionDistance;
                                PassedFinishCheck(section, nextSectionData, driver);
                                break;
                            }
                            else if (nextSectionData.Left == null)
                            {
                                nextSectionData.Left = driver;
                                sectionData.Right = null;
                                sectionData.DistanceRight = sectionDistance;
                                PassedFinishCheck(section, nextSectionData, driver);
                                break;
                            }   
                        }
                    }
                }
            }
        }

        //Debug.WriteLine($"{driver.Name} moved from {section.SectionType} (right) to {nextSection.SectionType} (right)");

        private static void PassedFinishCheck(Section section, SectionData sectionData, Driver driver)
        {
            if(section.SectionType == SectionTypes.Finish)
            {
                if (driver.Points < lapCount)
                {
                    driver.Points += 1;
                }
                else
                {
                    if (sectionData.Left == driver)
                    {
                        sectionData.Left = null;
                        driversOnTrack--;
                        Debug.WriteLine($"Driver {driver.Name} finished!");
                        driver.HasFinished = true;
                    }
                    else if (sectionData.Right == driver)
                    {
                        sectionData.Right = null;
                        driversOnTrack--;
                        Debug.WriteLine($"Driver {driver.Name} finished!");
                        driver.HasFinished = true;
                    }
                }
            }
        }

        private void CleanUpRace()
        {
            foreach(IParticipant participant in Participants)
            {
                Driver driver = (Driver)participant;
                driver.HasFinished = false;
                driver.Points = 0;
            }
            _timer.Stop();
        }

        private void CheckForDamage()
        {
            foreach(IParticipant p in Participants) {
                Driver d = (Driver)p;
                propability = d.Equipment.Performance / d.Equipment.Speed * d.Equipment.Quality;

                if(!d.HasFinished)
                {
                    if (!d.Equipment.IsBroken && _random.Next(0, 100) <= propability)
                    {
                        d.Equipment.IsBroken = true;
                        Debug.WriteLine($"Driver {d.Name} broke down!");
                    }
                    if (d.Equipment.IsBroken && _random.Next(0, 100) < 25)
                    {
                        d.Equipment.Performance -= 3;
                        d.Equipment.Speed -= 2;
                        d.Equipment.IsBroken = false;
                    }
                }
            }
        }

        private void DetermineStartPosition(Track track, List<IParticipant> participants)
        {
            var i = 0;
            foreach (var section in track.Sections)
            {
                if (section.SectionType == SectionTypes.StartGrid)
                {
                    var previousSection = Track.PreviousSection(section);
                    var previousSectionData = GetSectionData(previousSection);
                    var startData = GetSectionData(section);
                    if (i < participants.Count && startData.Left == null)
                    {
                        startData.Left = participants[i++];
                    }
                    if (i < participants.Count && startData.Right == null)
                    {
                        startData.Right = participants[i++];
                    }
                    if (i < participants.Count && previousSectionData.Left == null)
                    {
                        previousSectionData.Left = participants[i++];
                        previousSectionData.Left.Points--;
                    }
                    if (i < participants.Count && previousSectionData.Right == null)
                    {
                        previousSectionData.Right = participants[i++];
                        previousSectionData.Right.Points--;
                    }
                    startData.DistanceRight = sectionDistance;
                    startData.DistanceLeft = sectionDistance;
                }
                else
                {
                    GetSectionData(section).DistanceRight = sectionDistance;
                    GetSectionData(section).DistanceLeft = sectionDistance;
                }
            }
        }
    }
}
