using System;
using System.Text;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class CharacterSetParser : Parser<char>, Parser.IFailPoint
  {
    [NotNull] private readonly char[] myCharacters;
    [CanBeNull] private string myDescription;
    private readonly bool myIsNegative;

    public CharacterSetParser([NotNull] char[] characters, [CanBeNull] string description, bool isNegative)
    {
      if (characters.Length == 0)
        throw new ArgumentException("Non-empty character set expected", "characters");

      myCharacters = characters;
      myDescription = description;
      myIsNegative = isNegative;

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
            return myIsNegative
              ? new ParseResult(failPoint: this, atOffset: offset)
              : new ParseResult(value: ch, nextOffset: offset + 1);
          }
        }

        if (myIsNegative) return new ParseResult(value: ch, nextOffset: offset + 1);
      }

      return new ParseResult(failPoint: this, atOffset: offset);
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

        if (myIsNegative) builder.Insert(0, "not ");

        myDescription = builder.ToString();
      }

      return myDescription;
    }
  }
}