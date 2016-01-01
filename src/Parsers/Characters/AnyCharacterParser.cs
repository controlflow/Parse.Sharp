namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class AnyCharacterParser : Parser<char>, Parser.IFailPoint
  {
    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length)
      {
        return new ParseResult(value: input[offset], nextOffset: offset + 1);
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public string GetExpectedMessage()
    {
      return "any character";
    }
  }
}