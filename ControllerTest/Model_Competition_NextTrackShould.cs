using Model;
using Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ControllerTest
{
    [TestFixture]
    public class Model_Competition_NextTrackShould
    {
        private Competition _competition;

        [SetUp]
        public void Setup()
        {
            _competition = new Competition();
            _competition.Tracks = new Queue<Track>();
        }

        [Test]
        public void NextTrack_EmptyQueue_ReturnNull()
        {
            var result = _competition.NextTrack();
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_OneInQueue_ReturnTrack()
        {
            Track track = new("mugello", new[] { SectionTypes.StartGrid, SectionTypes.Finish });
            _competition.Tracks.Enqueue(track);
            var result = _competition.NextTrack();
            Assert.AreEqual(result, track);
        }

        [Test]
        public void NextTrack_OneInQueue_RemoveTrackFromQueue()
        {
            Track track = new("zandvoort", new[]{SectionTypes.StartGrid, SectionTypes.Finish});
            _competition.Tracks.Enqueue(track);
            var result = _competition.NextTrack();
            result = _competition.NextTrack();
            Assert.IsNull(result);
        }

        [Test]
        public void NextTrack_TwoInQueue_ReturnNextTrack()
        {
            Track track1 = new("red bull ring", new[] { SectionTypes.StartGrid, SectionTypes.Finish });
            Track track2 = new("silverstone", new[] { SectionTypes.StartGrid, SectionTypes.Finish });
            _competition.Tracks.Enqueue(track1);
            _competition.Tracks.Enqueue(track2);
            var resultTrack1 = _competition.NextTrack();
            var resultTrack2 = _competition.NextTrack();
            Assert.AreEqual(resultTrack1, track1);
            Assert.AreEqual(resultTrack2, track2);
        }
    }
}
