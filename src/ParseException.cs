using System;
using JetBrains.Annotations;

namespace Parse.Sharp
{
  [PublicAPI]
  public class ParseException : Exception
  {
    private readonly int myOffset;

    public ParseException(int offset)
    {
      myOffset = offset;
    }

    public ParseException(string message, int offset)
      : base(message)
    {
      myOffset = offset;
    }

    public int Offset { get { return myOffset; } }
  }
}