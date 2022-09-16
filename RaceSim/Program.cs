using Controller;
using Model;

Competition F1Race = new();

Data.Initialize(F1Race);
Data.NextRace();

Console.WriteLine($"Racing on Track: {Data.CurrentRace.Track.Name}");
//Console.WriteLine($"Section count: {Data.CurrentRace.Track.Sections.Count}");
//foreach (IParticipant participant in F1Race.Participants)
//{
//    Console.WriteLine($"* {participant.Name}: Points={participant.Points}/200, Team {participant.TeamColor} (Car: Quality={participant.Equipment.Quality}/100; Performance={participant.Equipment.Performance}/100; {participant.Equipment.Speed}km/h)");
//}

for (; ; )
{
    Thread.Sleep(100);
}
