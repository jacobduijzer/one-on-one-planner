namespace OneOnOnePlanner.Core;

public class PersonAvail
{

    public string Email { get; set; } = string.Empty;
    public string Name  { get; set; } = string.Empty;
    public HashSet<string> Days { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> TuesdaySlots { get; set; } = new();
}