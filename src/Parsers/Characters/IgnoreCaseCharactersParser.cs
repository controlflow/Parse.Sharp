namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class IgnoreCaseCharactersParser : Parser<char>, Parser.IFailPoint
  {
    private readonly char myCharacter, myAltCharacter;

    public IgnoreCaseCharactersParser(char character, char altCharacter)
    {
      myCharacter = character;
      myAltCharacter = altCharacter;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length)
      {
        var ch = input[offset];
        if (ch == myCharacter | ch == myAltCharacter)
        {
          return new ParseResult(value: ch, nextOffset: offset + 1);
        }
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public string GetExpectedMessage()
    {
      // ReSharper disable once RedundantToStringCallForValueType
      return "'" + myCharacter.ToString() + "'";
    }
  }
}