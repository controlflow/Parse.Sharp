using System;
using System.Text;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class CharacterSetParser : Parser<char>, Parser.IFailPoint
  {
    [NotNull] private readonly char[] myCharacters;
    [CanBeNull] private string myDescription;
    private readonly bool myIsExcept;

    public CharacterSetParser([NotNull] char[] characters, [CanBeNull] string description, bool isExcept)
    {
      if (characters.Length == 0)
        throw new ArgumentException("Non-empty character set expected", "characters");

      myCharacters = characters;
      myDescription = description;
      myIsExcept = isExcept;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length)
      {
        var ch = input[offset];
        foreach (var expectedCharacter in myCharacters)
        {
          if (ch == expectedCharacter)
          {
            return myIsExcept
              ? new ParseResult(failPoint: this, atOffset: offset)
              : new ParseResult(value: ch, nextOffset: offset + 1);
          }
        }

        if (myIsExcept) return new ParseResult(value: ch, nextOffset: offset + 1);
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    // todo: test!
    protected override Parser<char> CreateIgnoreCaseParser()
    {
      var count = 0;

      foreach (var character in myCharacters)
      {
        var alternative = InvertCharCase(character);
        if (alternative != character) count ++;
      }



      if (count == 0) return this;

      var index = 0;
      var newCharacters = new char[myCharacters.Length + count];

      foreach (var character in myCharacters)
      {
        newCharacters[index++] = character;

        var alternative = InvertCharCase(character);
        if (alternative != character)
          newCharacters[index++] = alternative;
      }

      var forcedDescription = GetExpectedMessage();
      return new CharacterSetParser(newCharacters, forcedDescription, myIsExcept);
    }

    public string GetExpectedMessage()
    {
      if (myDescription == null)
      {
        var builder = new StringBuilder();
        foreach (var character in myCharacters)
        {
          if (builder.Length > 0) builder.Append('|');
          builder.Append('\'').Append(character).Append('\'');
        }

        if (myIsExcept) builder.Insert(0, "not ");

        myDescription = builder.ToString();
      }

      return myDescription;
    }
  }
}