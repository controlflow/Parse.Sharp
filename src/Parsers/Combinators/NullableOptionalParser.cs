using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class NullableOptionalParser<T> : Parser<T?> where T : struct
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;

    public NullableOptionalParser([NotNull] Parser<T> underlyingParser)
    {
      myUnderlyingParser = underlyingParser;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var result = myUnderlyingParser.TryParseValue(input, offset);
      if (result.IsSuccessful) return new ParseResult(result.Value, result.Offset);

      return new ParseResult(value: null, nextOffset: offset);
    }

    protected override Parser<T?> CreateIgnoreCaseParser()
    {
      var ignoreCaseParser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCaseParser)) return this;

      return new NullableOptionalParser<T>(ignoreCaseParser);
    }
  }
}