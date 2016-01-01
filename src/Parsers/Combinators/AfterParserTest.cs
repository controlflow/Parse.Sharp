using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class AfterParserTest<T> : Parser<T>
  {
    [NotNull] private readonly Parser<T> myFirstParser;
    [NotNull] private readonly Parser myNextParser;

    public AfterParserTest([NotNull] Parser<T> firstParser, [NotNull] Parser nextParser)
    {
      myFirstParser = firstParser;
      myNextParser = nextParser;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var firstResult = myFirstParser.TryParseValue(input, offset);
      if (firstResult.IsSuccessful)
      {
        var nextResult = myNextParser.TryParseVoid(input, firstResult.Offset);
        if (nextResult.IsSuccessful)
        {
          return new ParseResult(firstResult.Value, nextResult.Offset);
        }

        return new ParseResult(nextResult.FailPoint, nextResult.Offset);
      }

      return firstResult;
    }
  }
}