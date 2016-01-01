using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class SequentialParser<T, TResult> : Parser<TResult>
  {
    [NotNull] private readonly Parser<T> myFirstParser;
    [NotNull] private readonly Func<T, Parser<TResult>> myNextParser;

    public SequentialParser([NotNull] Parser<T> firstParser, [NotNull] Func<T, Parser<TResult>> nextParser)
    {
      myFirstParser = firstParser;
      myNextParser = nextParser;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var result = myFirstParser.TryParse(input, offset);
      if (result.IsSuccessful)
      {
        var nextParser = myNextParser(result.Value);
        return nextParser.TryParse(input, result.Offset);
      }

      return new ParseResult(result.FailPoint, result.Offset);
    }
  }

  internal sealed class SequentialParser<T, TNext, TResult> : Parser<TResult>
  {
    [NotNull] private readonly Parser<T> myFirstParser;
    [NotNull] private readonly Func<T, Parser<TNext>> myNextParser;
    [NotNull] private readonly Func<T, TNext, TResult> mySelector;

    public SequentialParser([NotNull] Parser<T> firstParser,
                            [NotNull] Func<T, Parser<TNext>> nextParser,
                            [NotNull] Func<T, TNext, TResult> selector)
    {
      myFirstParser = firstParser;
      myNextParser = nextParser;
      mySelector = selector;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var firstResult = myFirstParser.TryParse(input, offset);
      if (!firstResult.IsSuccessful)
      {
        return new ParseResult(firstResult.FailPoint, firstResult.Offset);
      }

      var nextParser = myNextParser(firstResult.Value);
      var nextResult = nextParser.TryParse(input, firstResult.Offset);
      if (!nextResult.IsSuccessful)
      {
        return new ParseResult(nextResult.FailPoint, nextResult.Offset);
      }

      var value = mySelector(firstResult.Value, nextResult.Value);
      return new ParseResult(value, nextResult.Offset);
    }
  }
}