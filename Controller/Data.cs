using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Controller
{
    public static class Data
    {
        public static Competition Comp { get; set; }

        public static void Initialize()
        {
            Comp = new Competition();
            AddParticipants();
            GenerateTracks();
            NextRace();
        }

        private static void AddParticipants()
        {
            Comp.Participants = new List<IParticipant>
            {
                new Driver("Sem", 0, new Car(2, 4, 200, false), TeamColors.Red),
                new Driver("Daan", 0, new Car(2, 2, 200, false), TeamColors.Green),
                new Driver("Jort", 0, new Car(2, 8, 225, false), TeamColors.Blue),
                new Driver("Adri", 0, new Car(2, 9, 225, false), TeamColors.Yellow)
            };
        }

        private static void GenerateTracks()
        {
            Comp.Tracks = new Queue<Track>();
            Comp.Tracks.Enqueue(new Track("Silverstone", new[]
           {
                SectionTypes.StartGrid, SectionTypes.LeftCorner,
                SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.RightCorner,
                SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner,
                SectionTypes.LeftCorner, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner,
                SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Finish
            }));
            
            Comp.Tracks.Enqueue(new Track("Monza", new[]
            {
                SectionTypes.StartGrid, SectionTypes.RightCorner,
                SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner,
                SectionTypes.RightCorner, SectionTypes.Finish
            }));
            Comp.Tracks.Enqueue(new Track("Spa", new[]
            {
                SectionTypes.StartGrid, SectionTypes.LeftCorner,
                SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.LeftCorner, SectionTypes.RightCorner,
                SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.Straight,
                SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.RightCorner, SectionTypes.Finish
            }));
            Comp.Tracks.Enqueue(new Track("Red Bull Ring", new[]
            {
                SectionTypes.StartGrid, SectionTypes.RightCorner, SectionTypes.Straight,
                SectionTypes.RightCorner, SectionTypes.Straight, SectionTypes.Straight, SectionTypes.RightCorner,
                SectionTypes.Straight, SectionTypes.RightCorner, SectionTypes.Finish
            }));
        }

        public static Race CurrentRace { get; set; }

        public static void NextRace()
        {
            if(Comp.Tracks != null)
            {
                CurrentRace = new Race(Comp.NextTrack(), Comp.Participants);
                CurrentRace.Start();
            }
        }

    }
}
