namespace Parse.Sharp.Parsers
{
  internal sealed class DigitParser : Parser<int>, Parser.IFailPoint
  {
    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length)
      {
        var ch = input[offset];
        if (ch >= '0' && ch <= '9')
        {
          return new ParseResult(value: ch - '0', nextOffset: offset + 1);
        }
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public string GetExpectedMessage()
    {
      return "digit";
    }
  }
}