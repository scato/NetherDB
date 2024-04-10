using System.Diagnostics;
using System.Formats.Cbor;
using System.Text;

namespace NetherDB.Core.Storage;

public class Sequence
{
    public static Sequence CreateNew(Type type, Stream stream, long offset)
    {
        var sequence = new Sequence(type, stream, offset);

        sequence.Initialize();
        sequence.Flush();

        return sequence;
    }

    public static Sequence Create(Type type, Stream stream, long offset)
    {
        var sequence = new Sequence(type, stream, offset);

        sequence.Load();

        return sequence;
    }

    private Sequence(Type type, Stream stream, long offset)
    {
        if (type != typeof(int) && type != typeof(long))
        {
            throw new ApplicationException("Sequence must be of type int or long");
        }

        Type = type;
        Current = 0;
        _stream = stream;
        _offset = offset;
    }

    public Type Type { get; }

    public object Current { get; private set; }

    private Stream _stream;
    private long _offset;

    private void Initialize()
    {
        if (Type == typeof(int))
        {
            Current = 1;
        }
        else if (Type == typeof(long))
        {
            Current = 1L;
        }
    }

    public object Next()
    {
        object current = 0;

        if (Type == typeof(int))
        {
            current = Current;
            Current = (int) current + 1;
        }
        else if (Type == typeof(long))
        {
            current = Current;
            Current = (long) current + 1L;
        }

        Flush();

        return current;
    }

    private void Load()
    {
        _stream.Position = _offset;

        using (var binaryReader = new BinaryReader(_stream, Encoding.UTF8, true))
        {
            var pageType = binaryReader.ReadByte();
            Debug.Assert(pageType == 0x01); // PageType.Sequence

            var valueLength = binaryReader.ReadUInt16();
            var value = binaryReader.ReadBytes(valueLength);

            var cborReader = new CborReader(value);

            if (Type == typeof(int))
            {
                Current = cborReader.ReadInt32();
            }
            else if (Type == typeof(long))
            {
                Current = cborReader.ReadInt64();
            }
        }
    }

    private void Flush()
    {
        _stream.Position = _offset;

        using(var binaryWriter = new BinaryWriter(_stream, Encoding.UTF8, true))
        {
            var cborWriter = new CborWriter();

            if (Type == typeof(int))
            {
                cborWriter.WriteInt32((int) Current);
            }
            else if (Type == typeof(long))
            {
                cborWriter.WriteInt64((long) Current);
            }

            var value = cborWriter.Encode();

            binaryWriter.Write((byte) 0x01); // PageType.Sequence
            binaryWriter.Write((ushort) value.Length);
            binaryWriter.Write(value);
        }
    }
}
