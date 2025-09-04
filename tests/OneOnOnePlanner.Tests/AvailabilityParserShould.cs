using OneOnOnePlanner.Core;

namespace OneOnOnePlanner.Tests;

public class AvailabilityParserShould
{
    [Fact]
    public void Parse_Data_File_To_List_Of_PersonAvailable()
    {
        // Arrange
        var path = Path.Combine("TestData", "test-data-01.csv");

        // Act
        var people = AvailabilityParser.Parse(path);

        // Assert
        var slots = people.SelectMany(x => x.TuesdaySlots).Distinct().Order();
        Assert.NotNull(people);
        Assert.Equal(12, people.Count);
        
        var first = people.First();
        Assert.Equal("Tuesday", first.Days.Single());
        
        var second = people.Skip(1).Take(1).First();
        Assert.Equal(2, second.Days.Count);
    }
}