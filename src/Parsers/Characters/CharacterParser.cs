namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class CharacterParser : Parser<char>, Parser.IFailPoint
  {
    private readonly char myCharacter;

    public CharacterParser(char character)
    {
      myCharacter = character;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length && input[offset] == myCharacter)
      {
        return new ParseResult(value: myCharacter, nextOffset: offset + 1);
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public override Parser<char> IgnoreCase()
    {
      var alternative = InvertCharCase(myCharacter);
      if (alternative != myCharacter)
        return new IgnoreCaseCharacterParser(myCharacter, alternative);

      return this;
    }

    public string GetExpectedMessage()
    {
      // ReSharper disable once RedundantToStringCallForValueType
      return "'" + myCharacter.ToString() + "'";
    }

    private sealed class IgnoreCaseCharacterParser : Parser<char>, IFailPoint
    {
      private readonly char myCharacter, myAltCharacter;

      public IgnoreCaseCharacterParser(char character, char altCharacter)
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
}