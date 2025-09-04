namespace OneOnOnePlanner.Core;


public static class BiWeeklyTuesdayPlanner
{
    
    private static readonly string[] FixedSlots =
    [
        "08:30 - 09:00",
        "10:00 - 10:30",
        "10:45 - 11:15",
        "11:30 - 12:00",
        "13:30 - 14:00",
        "15:00 - 15:30",
        "15:45 - 16:15"
    ];

    private static string Canon(string s) => s.Replace(" ", "");

    public record Assignment(string Name, string Email, string Day, string Slot);
    public record Result(List<Assignment> Assignments, List<(PersonAvail Person, string Reason)> Unplanned);

    /// <summary>
    /// Plans everyone at most once across two Tuesdays using maximum matching.
    /// If a person selected Tuesday but provided no times, we treat it as "any of the fixed slots".
    /// </summary>
    public static Result Plan(List<PersonAvail> people, string day1, string day2)
    {
        // Build the slot universe: Tue#1 + Tue#2
        var fixedKeys = FixedSlots.Select(Canon).ToArray();
        var slots = new List<(string, string Label, string Key)>(capacity: 16);
        slots.AddRange(FixedSlots.Select(s => (day1, s, Canon(s))));
        slots.AddRange(FixedSlots.Select(s => (day2, s, Canon(s))));

        // Index slots by "date|key"
        var slotIndex = slots
            .Select((x, i) => (x, i))
            .ToDictionary(t => $"{t.x.Item1}|{t.x.Key}", t => t.i, StringComparer.OrdinalIgnoreCase);

        // Build adjacency: person -> list of slot indices they can take
        var adj = new List<int>[people.Count];
        var compatCounts = new int[people.Count]; // for diagnostics

        for (int u = 0; u < people.Count; u++)
        {
            var p = people[u];
            var edges = new HashSet<int>();

            var hasTuesday = p.Days.Contains("Tuesday", StringComparer.OrdinalIgnoreCase);

            if (hasTuesday)
            {
                // If they gave no times for Tuesday, assume "any slot"
                var prefs = (p.TuesdaySlots.Count == 0
                            ? fixedKeys
                            : p.TuesdaySlots.Select(Canon).Where(k => fixedKeys.Contains(k, StringComparer.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase))
                           .ToList();

                foreach (var k in prefs)
                {
                    var k1 = $"{day1}|{k}";
                    var k2 = $"{day2}|{k}";
                    if (slotIndex.TryGetValue(k1, out var v1)) edges.Add(v1);
                    if (slotIndex.TryGetValue(k2, out var v2)) edges.Add(v2);
                }
            }

            adj[u] = edges.ToList();
            compatCounts[u] = adj[u].Count;
        }

        // Run maximum bipartite matching
        var hk = new HopcroftKarp(people.Count, slots.Count, adj);
        hk.MaxMatching();

        // Build results
        var assignments = new List<Assignment>();
        var unplanned   = new List<(PersonAvail, string)>();

        for (int u = 0; u < people.Count; u++)
        {
            int v = hk.PairU[u];
            if (v != -1)
            {
                var (date, label, _) = slots[v];
                assignments.Add(new Assignment(people[u].Name, people[u].Email, date, label));
            }
            else
            {
                var p = people[u];
                string reason =
                    !p.Days.Contains("Tuesday", StringComparer.OrdinalIgnoreCase) ? "Tuesday not selected" :
                    (compatCounts[u] == 0 ? "No overlap with fixed Tuesday slots" :
                     "All compatible Tuesday slots were taken");
                unplanned.Add((p, reason));
            }
        }

        assignments = assignments
            .OrderBy(a => a.Day)
            .ThenBy(a => a.Slot[..5]) // start time
            .ThenBy(a => a.Name)
            .ToList();

        return new Result(assignments, unplanned);
    }
}