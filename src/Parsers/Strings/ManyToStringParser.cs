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

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var innerOffset = offset;
      for (; innerOffset < input.Length; )
      {
        var nextResult = myContentsParser.TryParseVoid(input, innerOffset);
        if (!nextResult.IsSuccessful) break;

        innerOffset = (nextResult.Offset > innerOffset) ? nextResult.Offset : innerOffset + 1;
      }

      var value = input.Substring(startIndex: offset, length: innerOffset - offset);
      return new ParseResult(value, nextOffset: innerOffset);
    }

    // todo: throw in position when parse value is ignored?

    protected override Parser<string> CreateIgnoreCaseParser()
    {
      var ignoreCaseContentParser = myContentsParser.IgnoreCase();
      if (ReferenceEquals(myContentsParser, ignoreCaseContentParser)) return this;

      return new ManyToStringParser(ignoreCaseContentParser, myDescription);
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