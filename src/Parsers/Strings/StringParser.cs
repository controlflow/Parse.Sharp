using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Strings
{
  internal sealed class StringParser : Parser<string>, Parser.IFailPoint
  {
    [NotNull] private readonly string myString;

    public StringParser([NotNull] string @string)
    {
      myString = @string;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var expectedLength = myString.Length;
      if (expectedLength + offset <= input.Length)
      {
        var comparison = string.Compare(input, offset, myString, 0, expectedLength, StringComparison.Ordinal);
        if (comparison == 0)
        {
          return new ParseResult(value: myString, nextOffset: offset + expectedLength);
        }
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public string GetExpectedMessage()
    {
      return "'" + myString + "'";
    }
  }
}