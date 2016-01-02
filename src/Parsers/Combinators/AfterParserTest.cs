using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class AfterParserTest<T> : Parser<T>
  {
    [NotNull] private readonly Parser<T> myHeadParser;
    [NotNull] private readonly Parser myTailParser;

    public AfterParserTest([NotNull] Parser<T> headParser, [NotNull] Parser tailParser)
    {
      myHeadParser = headParser;
      myTailParser = tailParser;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var headResult = myHeadParser.TryParseValue(input, offset);
      if (headResult.IsSuccessful)
      {
        var tailResult = myTailParser.TryParseVoid(input, headResult.Offset);
        if (tailResult.IsSuccessful)
        {
          return new ParseResult(headResult.Value, tailResult.Offset);
        }

        return new ParseResult(tailResult.FailPoint, tailResult.Offset);
      }

      return headResult;
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

      return new AfterParserTest<T>(ignoreCaseHeadParser, ignoreCaseTailParser);
    }
  }
}