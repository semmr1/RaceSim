using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Competition
    {
        public List<IParticipant> Participants { get; set; }

        public Queue<Track>? Tracks { get; set; }

        public Track? NextTrack()
        {
            if (Tracks == null)
            {
                Tracks = new();
            }
            return Tracks.Count > 0 ? Tracks.Dequeue() : null;
        }
    }

}
