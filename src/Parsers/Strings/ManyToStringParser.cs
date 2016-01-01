using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Strings
{
  internal sealed class ManyToStringParser : Parser<string>, Parser.IFailPoint
  {
    [NotNull] private readonly Parser myContentsParser;
    [CanBeNull] private readonly string myDescription;

    public ManyToStringParser([NotNull] Parser contentsParser, [CanBeNull] string description)
    {
      myContentsParser = contentsParser;
      myDescription = description;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var innerOffset = offset;
      for (; innerOffset < input.Length; )
      {
        var nextOffset = myContentsParser.TryParseAny(input, innerOffset);
        if (nextOffset < 0) break;

        innerOffset = (nextOffset > innerOffset) ? nextOffset : innerOffset + 1;
      }

      var value = input.Substring(startIndex: offset, length: innerOffset - offset);
      return new ParseResult(value, nextOffset: innerOffset);
    }

    public string GetExpectedMessage()
    {
      if (myDescription != null) return myDescription;

      var failPoint = myContentsParser as IFailPoint;
      if (failPoint == null) return "string";

      return "string of " + failPoint.GetExpectedMessage();
    }
  }
}