using Controller;
using Model;
using RaceSim;

{
    Competition F1Competition = new();
    Data.Initialize(F1Competition);

    Data.NextRace();


    ConsoleVisualization.DrawTrack(Data.CurrentRace.Track);

    var next = "";
    Console.SetCursorPosition(0,25);
    try
    {
        next = Console.ReadLine();
        if (next == "next") { Data.NextRace(); }
        Console.Clear();
        ConsoleVisualization.DrawTrack(Data.CurrentRace.Track);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
    for (; ; )
    {
        Thread.Sleep(100);
    }
}


