using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class NamedRuleParser<T> : ParserWithDescription<T>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;

    public NamedRuleParser([NotNull] Parser<T> underlyingParser, [NotNull] string description)
      : base(description)
    {
      myUnderlyingParser = underlyingParser;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var result = myUnderlyingParser.TryParseValue(input, offset);
      if (result.IsSuccessful) return result;

      return new ParseResult(failPoint: this, atOffset: offset);
    }
  }
}