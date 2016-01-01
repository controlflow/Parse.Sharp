using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class BeforeParserTest<T> : Parser<T>
  {
    [NotNull] private readonly Parser myFirstParser;
    [NotNull] private readonly Parser<T> myNextParser;

    public BeforeParserTest([NotNull] Parser firstParser, [NotNull] Parser<T> nextParser)
    {
      myFirstParser = firstParser;
      myNextParser = nextParser;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var firstResult = myFirstParser.TryParseVoid(input, offset);
      if (firstResult.IsSuccessful)
      {
        var nextResult = myNextParser.TryParseValue(input, firstResult.Offset);
        if (nextResult.IsSuccessful)
        {
          return nextResult;
        }

        return new ParseResult(nextResult.FailPoint, nextResult.Offset);
      }

      return new ParseResult(firstResult.FailPoint, firstResult.Offset);
    }
  }
}