using System;
using System.Threading;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class DelayedParser<T> : Parser<T>
  {
    [NotNull] private readonly Lazy<Parser<T>> myUnderlyingParser;

    public DelayedParser([NotNull] Lazy<Parser<T>> parser)
    {
      myUnderlyingParser = parser;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      return myUnderlyingParser.Value.TryParseValue(input, offset);
    }

    protected override Parser<T> CreateIgnoreCaseParser()
    {
      var lazy = new Lazy<Parser<T>>(MapIgnoreCase, LazyThreadSafetyMode.PublicationOnly);
      return new DelayedParser<T>(lazy);
    }

    [NotNull] private Parser<T> MapIgnoreCase()
    {
      return myUnderlyingParser.Value.IgnoreCase();
    }
  }
}