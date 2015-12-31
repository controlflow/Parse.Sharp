namespace Parse.Sharp.Parsers
{
  internal class AnyCharacterParser : Parser<char>, Parser.IFailPoint
  {
    protected internal override ParseResult TryParse(string input, int offset, bool isConditional)
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