namespace OneOnOnePlanner.Core;

public record RawRow(
    string Id,
    string SubmittedAt,
    string UpdatedAt,
    string Email,
    string Name,
    string Days,
    string TuesdaySlots,
    string WednesdaySlots
);