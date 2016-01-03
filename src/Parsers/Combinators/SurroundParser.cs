using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class SurroundParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser myHeadParser;
    [NotNull] private readonly Parser<T> myNextParser;
    [NotNull] private readonly Parser myTailParser;

    public SurroundParser([NotNull] Parser headParser, [NotNull] Parser<T> nextParser, [NotNull] Parser tailParser)
    {
      myHeadParser = headParser;
      myNextParser = nextParser;
      myTailParser = tailParser;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var headResult = myHeadParser.TryParseVoid(input, offset);
      if (!headResult.IsSuccessful) return new ParseResult(headResult.FailPoint, headResult.Offset);

      var nextResult = myNextParser.TryParseValue(input, headResult.Offset);
      if (!nextResult.IsSuccessful) return new ParseResult(nextResult.FailPoint, nextResult.Offset);

      var tailResult = myTailParser.TryParseVoid(input, nextResult.Offset);
      if (!tailResult.IsSuccessful) return new ParseResult(tailResult.FailPoint, tailResult.Offset);

      return new ParseResult(nextResult.Value, tailResult.Offset);
    }

    protected override Parser<T> CreateIgnoreCaseParser()
    {
      var ignoreCaseHeadParser = myHeadParser.IgnoreCase();
      var ignoreCaseNextParser = myNextParser.IgnoreCase();
      var ignoreCaseTailParser = myTailParser.IgnoreCase();

      if (ReferenceEquals(myHeadParser, ignoreCaseHeadParser) &&
          ReferenceEquals(myNextParser, ignoreCaseNextParser) &&
          ReferenceEquals(myTailParser, ignoreCaseTailParser)) return this;

      return new SurroundParser<T>(ignoreCaseHeadParser, ignoreCaseNextParser, ignoreCaseTailParser);
    }
  }
}