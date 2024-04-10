using FluentAssertions;
using NetherDB.Core;
using System.Diagnostics;
using Xunit.Gherkin.Quick;

namespace NetherDB.Test.Acceptance;

[FeatureFile("./Features/Sequence.feature")]
public class SequenceFeature : Feature, IDisposable
{
    private string? _path;
    private Database? _database;
    private Core.Storage.Sequence? _sequence;

    [Given(@"an empty database was created")]
    public void CreateDatabase()
    {
        Debug.Assert(_database == null);

        _path = Path.GetTempFileName();
        _database = Database.CreateFromFile(_path);
    }

    [And(@"I open the database later")]
    public void CloseAndOpenDatabase()
    {
        Debug.Assert(_database != null);
        Debug.Assert(_path != null);

        _database.Dispose();

        _database = Database.CreateFromFile(_path);
        _sequence = _database.GetSequence(typeof(int), 0L);
    }

    [And(@"a sequence was created")]
    [When(@"I create a sequence")]
    public void CreateSequence()
    {
        _sequence = _database!.CreateSequence(typeof(int));
    }

    [When(@"I get one value from that sequence")]
    public void GetNextSequenceValue()
    {
        _sequence!.Next();
    }

    [Then(@"the next value in that sequence should be (\d+)")]
    public void CurrentSequenceValueShouldBe(int value)
    {
        _sequence!.Current.Should().Be(value);
    }

    public void Dispose()
    {
        if (_database != null)
        {
            _database.Dispose();
        }

        if (_path != null)
        {
            File.Delete(_path);
        }
    }
}
