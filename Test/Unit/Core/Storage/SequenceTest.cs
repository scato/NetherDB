using FluentAssertions;
using NetherDB.Core;
using NetherDB.Core.Storage;

namespace NetherDB.Test.Unit.Core.Storage;

public class SequenceTest
{
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    public void ShouldProduceValues(Type type)
    {
        var sequence = Sequence.CreateNew(type, new MemoryStream(new byte[Database.PAGE_SIZE]), 0L);

        sequence.Type.Should().Be(type);
        sequence.Current.Should().BeOfType(type);
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    public void ShouldStartAtOne(Type type)
    {
        var sequence = Sequence.CreateNew(type, new MemoryStream(new byte[Database.PAGE_SIZE]), 0L);

        sequence.Current.Should().Be(1);
    }

    [Fact]
    public void ShouldIncrementValues()
    {
        var sequence = Sequence.CreateNew(typeof(int), new MemoryStream(new byte[Database.PAGE_SIZE]), 0L);

        sequence.Next().Should().Be(1);
        sequence.Next().Should().Be(2);
        sequence.Next().Should().Be(3);
    }

    [Fact]
    public void ShouldBePersisted()
    {
        var stream = new MemoryStream(new byte[Database.PAGE_SIZE]);
        var sequence1 = Sequence.CreateNew(typeof(int), stream, 0L);

        sequence1.Next();

        var sequence2 = Sequence.Create(typeof(int), stream, 0L);

        sequence2.Next().Should().Be(2);
    }
}
