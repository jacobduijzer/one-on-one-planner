using OneOnOnePlanner.Core;

if (args.Length < 1)
{
    Console.WriteLine("Usage: dotnet run -- <inputfile.csv>");
    return;
}

var inputFile = args[0];
var people = AvailabilityParser.Parse(inputFile);
var result = BiWeeklyTuesdayPlanner.Plan(people, "Tuesday 1", "Tuesday 2");

Console.WriteLine("=== SCHEDULE ===");
foreach (var a in result.Assignments)
    Console.WriteLine($"{a.Day,-9} {a.Slot}  {a.Name} <{a.Email}>");

Console.WriteLine();
Console.WriteLine("=== UNPLANNED ===");
foreach (var (p, why) in result.Unplanned)
    Console.WriteLine($"{p.Name} <{p.Email}> — {why}");

File.WriteAllLines("schedule.csv",
    new[] { "Day,Slot,Name,Email" }
        .Concat(result.Assignments.Select(a => $"{a.Day},{a.Slot},{a.Name},{a.Email}")));

// File.WriteAllLines("unplanned.csv",
//     new[] { "Name,Email,Reason" }
//         .Concat(result.Unplanned.Select(u => $"{u.Person.Name},{u.Person.Email},{u.Reason}")));

Console.WriteLine();
Console.WriteLine("Wrote schedule.csv and unplanned.csv");