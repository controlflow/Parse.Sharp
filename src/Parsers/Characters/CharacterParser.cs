namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class CharacterParser : Parser<char>, Parser.IFailPoint
  {
    private readonly char myCharacter;

    public CharacterParser(char character)
    {
      myCharacter = character;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length && input[offset] == myCharacter)
      {
        return new ParseResult(value: myCharacter, nextOffset: offset + 1);
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