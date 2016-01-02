namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class CharacterExceptParser : Parser<char>, Parser.IFailPoint
  {
    private readonly char myCharacter;

    public CharacterExceptParser(char character)
    {
      myCharacter = character;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length)
      {
        var ch = input[offset];
        if (ch != myCharacter)
        {
          return new ParseResult(value: ch, nextOffset: offset + 1);
        }
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    protected override Parser<char> CreateIgnoreCaseParser()
    {
      var alternative = InvertCharCase(myCharacter);
      if (alternative != myCharacter)
        return new IgnoreCaseCharacterExceptParser(myCharacter, alternative);

      return this;
    }

    public string GetExpectedMessage()
    {
      // ReSharper disable once RedundantToStringCallForValueType
      return "not '" + myCharacter.ToString() + "'";
    }

    private sealed class IgnoreCaseCharacterExceptParser : Parser<char>, IFailPoint
    {
      private readonly char myCharacter;
      private readonly char myAltCharacter;

      public IgnoreCaseCharacterExceptParser(char character, char altCharacter)
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
          if (ch != myCharacter & ch != myAltCharacter)
          {
            return new ParseResult(value: ch, nextOffset: offset + 1);
          }
        }

        return new ParseResult(failPoint: this, atOffset: offset);
      }

      public string GetExpectedMessage()
      {
        // ReSharper disable once RedundantToStringCallForValueType
        return "not '" + myCharacter.ToString() + "'";
      }
    }
  }
}