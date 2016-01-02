using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Strings
{
  internal sealed class StringParser : Parser<string>, Parser.IFailPoint
  {
    [NotNull] private readonly string myText;

    public StringParser([NotNull] string text)
    {
      myText = text;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
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

    protected override Parser<string> CreateIgnoreCaseParser()
    {
      return new IgnoreCaseStringParser(myText);
    }

    public string GetExpectedMessage()
    {
      return "'" + myText + "'";
    }
  }
}