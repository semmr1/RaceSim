using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class RaceEndEventArgs
    {
        public Race Race { get; set; }

        public RaceEndEventArgs()
        {
            Race = Data.CurrentRace;
        }
    }
}
