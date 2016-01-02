using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class OptionalParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser<T> myParser;
    private readonly T myDefaultValue;

    public OptionalParser([NotNull] Parser<T> parser, T defaultValue)
    {
      myParser = parser;
      myDefaultValue = defaultValue;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var result = myParser.TryParseValue(input, offset);
      if (result.IsSuccessful) return result;

      return new ParseResult(myDefaultValue, offset);
    }
  }
}