using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace OneOnOnePlanner.Core;

public static class AvailabilityParser
{
    public static List<PersonAvail> Parse(string path)
    {
        using var reader = new StreamReader(path);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = false,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim
        };

        reader.BaseStream.Position = 0;
        reader.DiscardBufferedData();

        using var csv = new CsvReader(reader, config);

        var people = new List<PersonAvail>();
        foreach (var r in csv.GetRecords<RawRow>().Skip(1))
        {
            var days = SplitInnerSemis(r.Days);
            var tue  = SplitSlots(r.TuesdaySlots);
            var wed  = SplitSlots(r.WednesdaySlots);

            people.Add(new PersonAvail
            {
                Email = r.Email.Trim(),
                Name  = r.Name.Trim(),
                Days  = new HashSet<string>(days, StringComparer.OrdinalIgnoreCase),
                TuesdaySlots = tue,
            });
        }
        return people;
    }

    private static string? ReadNextNonEmpty(StreamReader sr)
    {
        string? line;
        while ((line = sr.ReadLine()) != null)
            if (!string.IsNullOrWhiteSpace(line)) return line;
        return null;
    }

    private static List<string> SplitInnerSemis(string s) =>
        s.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    private static List<string> SplitSlots(string s) =>
        SplitInnerSemis(s).Where(v => !v.Equals("None of the above", StringComparison.OrdinalIgnoreCase)).ToList();
}