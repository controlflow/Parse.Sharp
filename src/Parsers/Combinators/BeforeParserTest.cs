using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class BeforeParserTest<T> : Parser<T>
  {
    [NotNull] private readonly Parser myHeadParser;
    [NotNull] private readonly Parser<T> myTailParser;

    public BeforeParserTest([NotNull] Parser headParser, [NotNull] Parser<T> tailParser)
    {
      myHeadParser = headParser;
      myTailParser = tailParser;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var headResult = myHeadParser.TryParseVoid(input, offset);
      if (headResult.IsSuccessful)
      {
        var tailResult = myTailParser.TryParseValue(input, headResult.Offset);
        if (tailResult.IsSuccessful)
        {
          return tailResult;
        }

        return new ParseResult(tailResult.FailPoint, tailResult.Offset);
      }

      return new ParseResult(headResult.FailPoint, headResult.Offset);
    }

    protected override Parser<T> CreateIgnoreCaseParser()
    {
      var ignoreCaseHeadParser = myHeadParser.IgnoreCase();
      var ignoreCaseTailParser = myTailParser.IgnoreCase();

      if (ReferenceEquals(myHeadParser, ignoreCaseHeadParser) &&
          ReferenceEquals(myTailParser, ignoreCaseTailParser))
      {
        return this;
      }

      return new BeforeParserTest<T>(ignoreCaseHeadParser, ignoreCaseTailParser);
    }
  }
}