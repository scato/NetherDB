using FluentAssertions;
using NetherDB.Core;

namespace NetherDB.Test.Unit.Core;

public class DatabaseTest
{
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    public void ShouldCreateSequences(Type type)
    {
        var database = Database.CreateNew();
        var sequence = database.CreateSequence(type);

        sequence.Type.Should().Be(type);
    }

    [Fact]
    public void ShouldProvideExistingSequences()
    {
        var database = Database.CreateNew();
        database.CreateSequence(typeof(int));

        var sequence = database.GetSequence(typeof(int), 0L);

        sequence.Type.Should().Be(typeof(int));
    }
}
