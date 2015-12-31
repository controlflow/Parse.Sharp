namespace Parse.Sharp.Combinators
{
  internal sealed class ReturnParser<T> : Parser<T>
  {
    private readonly T myValue;

    public ReturnParser(T value)
    {
      myValue = value;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      return new ParseResult(value: myValue, nextOffset: offset);
    }
  }
}