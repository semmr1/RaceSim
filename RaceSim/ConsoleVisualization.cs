using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.XPath;
using Controller;
using Microsoft.VisualBasic.CompilerServices;
using Model;

namespace RaceSim
{

    enum Direction
    {
        North,
        East,
        South,
        West,  
    }
    public static class ConsoleVisualization
    {
        private static Race currentRace;
        private static Section currentSection;
        private static int raceNumber;
        private static int origRow, origCol;
        private static int x, y;
        private static Direction direction;
        private static Direction lastDirection;

        #region graphics

        /*
        Console track legend:
        >   == || walls
        >   # start/finish line
        >   - grid
        >   A (char) participant
        >   // corner

        > Every section is 7x4 ASCII Characters
                                
                                       //====\\
                                      // 1  1 \\
                                     // 2    2 \\
                                    //    /\    \\
                  //================/    //\\    \==\\
                 // 1    ] 1  ##1     1 //  \\ 1   1 \\
                // 2    ] 2   ##2    2 //    \\ 2   2 \\
               //    /================//      \\==\    \\
               \\    \=========\\      //====\\   ||   ||
                \\ 1     1    1 \\    // 1  1 \\  ||1  ||
                 \\ 2    2     2 \\  // 2    2 \\ ||  2||
                  \\=========\    \\//    /\    \\||   ||
                             \\    \/    //\\    \/    //
                              \\ 1    1 //  \\ 1    1 //
                               \\ 2  2 //    \\ 2  2 //
                                \\====//      \\====//
        */

        private static string[] _straightHorizontal = { "=======", "   1   ", "   2   ", "=======" };
        private static string[] _straightVertical = { "||   ||", "||1  ||", "||  2||", "||   ||" };

        private static string[] _gridHorizontal = { "=======", "   ] 1 ", "  ] 2  ", "=======" };
        private static string[] _gridVertical = { "||-  ||", "||  1||", "||2  ||", "||  -||" };

        private static string[] _finishHorizontal = { "=======", " ##1   ", " ##2   ", "=======" };
        private static string[] _finishVertical = { "||   ||", "||###||", "||1 2||", "||   ||" };

        private static string[] _rightCornerHorizontal = { "==\\\\   ", " 1 \\\\  ", "  2 \\\\ ", "\\    \\\\" };
        private static string[] _rightCornerVertical = { "   //==", "  // 1 ", " // 2  ", "//    /" };

        private static string[] _leftCornerHorizontal = { "/    //", "  1 // ", " 2 //  ", "==//   " };
        private static string[] _leftCornerVertical = { "\\\\    \\", " \\\\ 1  ", "  \\\\ 2 ", "   \\\\==" };

        #endregion

        public static void Initialize(Race race)
        {
            Console.Clear();
            currentRace = race;
            currentSection = null;
            direction = Direction.East;
            lastDirection = Direction.North;
            x = 0;
            y = 4;
            raceNumber += 1;
            currentRace.RaceEnded += OnRaceEnding;
            currentRace.DriversChanged += OnDriversChanged;
            origCol = Console.CursorTop;
            origRow = Console.CursorLeft;
            DrawSpecifications();
            Console.SetCursorPosition(origCol + x, origRow + y);
            //Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Red;
        }

        public static void DrawTrack(Track track)
        {
            //var log = 0;
            foreach (var section in track.Sections)
            {
                Console.SetCursorPosition(x, y);
                //currentSection = track.Sections.ElementAt(i);
                currentSection = section;
                SetDirection(currentSection.SectionType);
                var isHorizontal = IsHorizontal();

                switch (currentSection.SectionType)
                {
                    case SectionTypes.Finish:
                        DrawSection(isHorizontal ? _finishHorizontal : _finishVertical);
                        break;
                    case SectionTypes.StartGrid:
                        x += 22;
                        y += 4;
                        DrawSection(isHorizontal ? _gridHorizontal : _gridVertical);
                        break;
                    case SectionTypes.Straight:
                        DrawSection(isHorizontal ? _straightHorizontal : _straightVertical);
                        break;
                    case SectionTypes.RightCorner:
                        DrawSection(DetermineCorner());
                        break;
                    case SectionTypes.LeftCorner:
                        DrawSection(DetermineCorner());
                        break;
                }

                switch (direction)
                {
                    case Direction.North:
                        y -= 4;
                        break;
                    case Direction.East:
                        x += 7;
                        break;
                    case Direction.South:
                        y += 4;
                        break;
                    case Direction.West:
                        x -= 7;
                        break;
                }

                //Console.SetCursorPosition(0, 15 + log);
                //Console.ForegroundColor = ConsoleColor.DarkGray;
                //Console.WriteLine($"Section: {Enum.GetName(typeof(Model.SectionTypes), currentSection.SectionType)} \t=> Heading {Enum.GetName(typeof(Direction), direction)}");
                //Console.ForegroundColor = ConsoleColor.Red;
                //log++;
                lastDirection = direction;

            }
            x = 0;
            y = 4;
        }

        private static bool IsHorizontal()
        {
            return direction == Direction.East || direction == Direction.West;
        }

        private static string[] DetermineCorner()
        {
            if (currentSection.SectionType == SectionTypes.RightCorner)
            {
                switch (direction)
                {
                    case (Direction.North):
                        return _leftCornerVertical;
                    case (Direction.East):
                        return _rightCornerVertical;
                    case (Direction.South):
                        return _rightCornerHorizontal;
                    case (Direction.West):
                        return _leftCornerHorizontal;
                }
            }

            if (currentSection.SectionType == SectionTypes.LeftCorner)
            {
                switch (direction)
                {
                    case (Direction.North):
                        return _leftCornerHorizontal;
                    case (Direction.East):
                        return _leftCornerVertical;
                    case (Direction.South):
                        return _rightCornerVertical;
                    case (Direction.West):
                        return _rightCornerHorizontal;
                }
            }
            return null;
        }

        private static string PlaceParticipants(string sectionLine, IParticipant left, IParticipant right)
        {
            string newSectionLine = sectionLine;
            if (left != null)
            {
                if (direction == Direction.West)
                {
                    if (left.Equipment.IsBroken)
                    {
                        newSectionLine = newSectionLine.Replace("2", "X");
                    } else
                    {
                        newSectionLine = newSectionLine.Replace("2", left.Name.Substring(0, 1));
                    }
                }
                else
                {
                    if (left.Equipment.IsBroken)
                    {
                        newSectionLine = newSectionLine.Replace("1", "X");
                    }
                    else
                    {
                        newSectionLine = newSectionLine.Replace("1", left.Name.Substring(0, 1));
                    }

                }
            }
            else
            {
                if (direction == Direction.West)
                {
                    newSectionLine = newSectionLine.Replace("2", " ");
                }
                else
                {
                    newSectionLine = newSectionLine.Replace("1", " ");
                }
            }
            if (right != null)
            {
                if (direction == Direction.West)
                {
                    if(right.Equipment.IsBroken)
                    {
                        newSectionLine = newSectionLine.Replace("1", "X");
                    } else {
                        newSectionLine = newSectionLine.Replace("1", right.Name.Substring(0, 1));
                    }
                }
                else
                {
                    if (right.Equipment.IsBroken)
                    {
                        newSectionLine = newSectionLine.Replace("2", "X");
                    } else {
                        newSectionLine = newSectionLine.Replace("2", right.Name.Substring(0, 1));
                    }
                }
            }
            else
            {
                if (direction == Direction.West)
                {
                    newSectionLine = newSectionLine.Replace("1", " ");
                }
                else
                {
                    newSectionLine = newSectionLine.Replace("2", " ");
                }
            }
            return newSectionLine;
        }

        private static void OnDriversChanged(object source, DriversChangedEventArgs e)
        {
            DrawTrack(e.Track);
            //Debug.WriteLine("updated track");
        }

        private static void OnRaceEnding(object source, RaceEndEventArgs e)
        {
            Initialize(e.Race);
        }

        private static void SetDirection(SectionTypes sectionType)
        {
            if (sectionType == SectionTypes.LeftCorner)
            {
                if (lastDirection == Direction.North)
                {
                    direction = lastDirection + 3;
                }
                else
                {
                    direction = lastDirection - 1;
                }
            }

            if (sectionType == SectionTypes.RightCorner)
            {
                if (lastDirection == Direction.West)
                {
                    direction = lastDirection - 3;
                }
                else
                {
                    direction = lastDirection + 1;
                }
            }
        }

        private static void DrawSection(string[] trackLines)
        {
            var xCopy = x;
            var yCopy = y;
            string newLine;

            foreach (var line in trackLines)
            {
                var pos1 = currentRace.GetSectionData(currentSection).Left;
                var pos2 = currentRace.GetSectionData(currentSection).Right;
                newLine = PlaceParticipants(line, pos1, pos2);
                Console.SetCursorPosition(xCopy, yCopy);
                Console.WriteLine(newLine);
                yCopy++;
            }
        }

        //private static string[] ReverseStringArray(string[] array)
        //{
        //    var s2 = "";
        //    var newArray = array;
        //    for (var i = 0; i < newArray.Length; i++)
        //    {
        //        var charArray = array[i].ToCharArray();
        //        Array.Reverse(charArray);
        //        foreach (var ch in charArray)
        //        {
        //            s2 += ch;
        //        }
        //        newArray[i] = s2;
        //    }
        //    return newArray;
        //}
        

        // TODO: get total Track count
        private static void DrawSpecifications()
        {
            Console.WriteLine($"Track {raceNumber}/{Data.Comp.Tracks.Count+raceNumber}: {currentRace.Track.Name}");
            Console.Write($"Section count: {currentRace.Track.Sections.Count}");
        }
    }
}
