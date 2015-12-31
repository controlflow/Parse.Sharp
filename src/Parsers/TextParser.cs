using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers
{
  internal sealed class TextParser : Parser<string>, Parser.IFailPoint
  {
    [NotNull] private readonly string myText;

    public TextParser([NotNull] string text)
    {
      myText = text;
    }

    protected internal override ParseResult TryParse(string input, int offset, bool isConditional)
    {
      var expectedLength = myText.Length;
      if (expectedLength + offset <= input.Length)
      {
        var comparison = string.Compare(input, offset, myText, 0, expectedLength, StringComparison.Ordinal);
        if (comparison == 0)
        {
          return new ParseResult(value: myText, nextOffset: offset + expectedLength);
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