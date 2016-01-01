using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Strings
{
  internal sealed class IgnoreCaseStringParser : Parser<string>, Parser.IFailPoint
  {
    [NotNull] private readonly string myText;

    public IgnoreCaseStringParser([NotNull] string text)
    {
      myText = text;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var expectedLength = myText.Length;
      if (expectedLength + offset <= input.Length)
      {
        var ignoreCaseComparison = string.Compare(input, offset, myText, 0, expectedLength, StringComparison.OrdinalIgnoreCase);
        if (ignoreCaseComparison == 0)
        {
          var ordinalComparison = string.Compare(input, offset, myText, 0, expectedLength, StringComparison.Ordinal);
          if (ordinalComparison == 0)
          {
            return new ParseResult(value: myText, nextOffset: offset + expectedLength);
          }

          var substring = input.Substring(offset, expectedLength);
          return new ParseResult(value: substring, nextOffset: offset + expectedLength);
        }
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public string GetExpectedMessage()
    {
      return "'" + myText + "'";
    }
  }
}