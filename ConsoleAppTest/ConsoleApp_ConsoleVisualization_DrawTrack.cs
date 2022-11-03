using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;
using RaceSim;

namespace ControllerTest
{
    [TestFixture]
    public class ConsoleApp_ConsoleVisualization_DrawTrack
    {
        private Competition _competition;
        private StringBuilder _ConsoleOutput;

        [SetUp]
        public void Setup()
        {
            _competition = new Competition();
            _competition.Tracks = new Queue<Track>();
            Data.Initialize(_competition);
            _ConsoleOutput = new StringBuilder();
            var _consoleOutputWriter = new StringWriter(_ConsoleOutput);
            Console.SetOut(_consoleOutputWriter);
        }

        private string[] RunDrawTrackAndGetConsoleOutput()
        {
            _competition.Tracks.Clear();
            //ConsoleVisualization.DrawTrack(Data.CurrentRace.Track);
            return _ConsoleOutput.ToString().Split("\r\n");
        }

        //[Test]
        public void DrawCompetition_EmptyQueue_ReturnNull()
        {
            var expectedResult = "";
            var outputLines = RunDrawTrackAndGetConsoleOutput();
            Assert.AreEqual(outputLines, expectedResult);
        }
    }
}
