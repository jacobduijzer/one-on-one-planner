using OneOnOnePlanner.Core;

namespace OneOnOnePlanner.Tests;

public class TuesdayPlannerShould
{
   [Fact]
   public void Test()
   {
      var path = Path.Combine("TestData", "test-data-01.csv");
      var people = AvailabilityParser.Parse(path);

      var res = BiWeeklyTuesdayPlanner.Plan(people,"Tuesday 1", "Tuesday 2");

      // Console.WriteLine("=== SCHEDULE ===");
      // foreach (var a in res.Assignments)
      //    Console.WriteLine($"{a.Date:yyyy-MM-dd} {a.Slot}  {a.Name} <{a.Email}>");
      //
      // Console.WriteLine("\n=== UNPLANNED ===");
      // foreach (var (p, why) in res.Unplanned)
      //    Console.WriteLine($"{p.Name} <{p.Email}>  — {why}");
   }
}