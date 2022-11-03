using Controller;
using Model;
using RaceSim;

{
    Data.Initialize();
    ConsoleVisualization.Initialize(Data.CurrentRace);

    for (; ; )
    {
        Thread.Sleep(100);
    }
}


