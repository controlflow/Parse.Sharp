using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class SequentialParser<T, TResult> : Parser<TResult>
  {
    [NotNull] private readonly Parser<T> myHeadParser;
    [NotNull] private readonly Func<T, Parser<TResult>> myTailParserFactory;

    public SequentialParser([NotNull] Parser<T> headParser, [NotNull] Func<T, Parser<TResult>> tailParserFactory)
    {
      myHeadParser = headParser;
      myTailParserFactory = tailParserFactory;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var headResult = myHeadParser.TryParseValue(input, offset);
      if (headResult.IsSuccessful)
      {
        var tailParser = myTailParserFactory(headResult.Value);
        return tailParser.TryParseValue(input, headResult.Offset);
      }

      return new ParseResult(headResult.FailPoint, headResult.Offset);
    }

    protected override Parser<TResult> CreateIgnoreCaseParser()
    {
      return new SequentialParser<T, TResult>(
        headParser: myHeadParser.IgnoreCase(),
        tailParserFactory: arg => myTailParserFactory(arg).IgnoreCase());
    }
  }

  internal sealed class SequentialParser<T, TNext, TResult> : Parser<TResult>
  {
    [NotNull] private readonly Parser<T> myHeadParser;
    [NotNull] private readonly Func<T, Parser<TNext>> myTailParserFactory;
    [NotNull] private readonly Func<T, TNext, TResult> myResultSelector;

    public SequentialParser([NotNull] Parser<T> headParser,
                            [NotNull] Func<T, Parser<TNext>> tailParserFactory,
                            [NotNull] Func<T, TNext, TResult> resultSelector)
    {
      myHeadParser = headParser;
      myTailParserFactory = tailParserFactory;
      myResultSelector = resultSelector;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var firstResult = myHeadParser.TryParseValue(input, offset);
      if (!firstResult.IsSuccessful)
      {
        return new ParseResult(firstResult.FailPoint, firstResult.Offset);
      }

      var nextParser = myTailParserFactory(firstResult.Value);
      var nextResult = nextParser.TryParseValue(input, firstResult.Offset);
      if (!nextResult.IsSuccessful)
      {
        return new ParseResult(nextResult.FailPoint, nextResult.Offset);
      }

      var value = myResultSelector(firstResult.Value, nextResult.Value);
      return new ParseResult(value, nextResult.Offset);
    }

    protected override Parser<TResult> CreateIgnoreCaseParser()
    {
      return new SequentialParser<T, TNext, TResult>(
        headParser: myHeadParser.IgnoreCase(),
        tailParserFactory: arg => myTailParserFactory(arg).IgnoreCase(),
        resultSelector: myResultSelector);
    }
  }
}