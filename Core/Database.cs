using System.IO.MemoryMappedFiles;
using NetherDB.Core.Storage;

namespace NetherDB.Core;

public class Database : IDisposable
{
    public static readonly long PAGE_SIZE = 8192;
    private Stream _stream;

    private Database(Stream stream)
    {
        _stream = stream;
    }

    public static Database CreateFromFile(string path)
    {
        return new Database(File.Open(path, FileMode.OpenOrCreate));
    }

    public static Database CreateNew()
    {
        return new Database(new MemoryStream());
    }

    public Sequence CreateSequence(Type type)
    {
        var offset = AllocatePage();
        var sequence = Sequence.CreateNew(type, _stream, offset);

        return sequence;
    }

    public Sequence GetSequence(Type type, long offset)
    {
        return Sequence.Create(type, _stream, offset);
    }

    private long AllocatePage()
    {
        var offset = _stream.Length;

        _stream.SetLength(_stream.Length + PAGE_SIZE);
        _stream.Flush();

        return offset;
    }

    public void Dispose()
    {
        _stream.Dispose();
    }
}
