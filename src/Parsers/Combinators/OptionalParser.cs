using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  // todo: tests

  internal sealed class OptionalParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    private readonly T myDefaultValue;

    public OptionalParser([NotNull] Parser<T> underlyingParser, T defaultValue)
    {
      myUnderlyingParser = underlyingParser;
      myDefaultValue = defaultValue;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var result = myUnderlyingParser.TryParseValue(input, offset);
      if (result.IsSuccessful) return result;

      return new ParseResult(myDefaultValue, offset);
    }

    protected override Parser<T> CreateIgnoreCaseParser()
    {
      var ignoreCaseParser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCaseParser)) return this;

      return new OptionalParser<T>(ignoreCaseParser, myDefaultValue);
    }
  }
}