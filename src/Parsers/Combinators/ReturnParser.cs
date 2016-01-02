using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class ReturnParser<T> : Parser<T>
  {
    private readonly T myValue;

    public ReturnParser(T value)
    {
      myValue = value;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      return new ParseResult(value: myValue, nextOffset: offset);
    }
  }
}