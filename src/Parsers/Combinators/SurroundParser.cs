using System.Linq;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class SurroundParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser myFirstParser;
    [NotNull] private readonly Parser<T> myNextParser;
    [NotNull] private readonly Parser myLastParser;

    public SurroundParser([NotNull] Parser firstParser, [NotNull] Parser<T> nextParser, [NotNull] Parser lastParser)
    {
      myFirstParser = firstParser;
      myNextParser = nextParser;
      myLastParser = lastParser;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var firstResult = myFirstParser.TryParseVoid(input, offset);
      if (firstResult.IsSuccessful)
      {
        var nextResult = myNextParser.TryParseValue(input, firstResult.Offset);
        if (nextResult.IsSuccessful)
        {
          var lastResult = myLastParser.TryParseVoid(input, nextResult.Offset);
          if (lastResult.IsSuccessful)
          {
            return new ParseResult(nextResult.Value, lastResult.Offset);
          }

          return new ParseResult(lastResult.FailPoint, lastResult.Offset);
        }

        return new ParseResult(nextResult.FailPoint, nextResult.Offset);
      }

      return new ParseResult(firstResult.FailPoint, firstResult.Offset);
    }
  }

  //internal sealed class SequenceParser<T>
  //{
  //  
  //}
}