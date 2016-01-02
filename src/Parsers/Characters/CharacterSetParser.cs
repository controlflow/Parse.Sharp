using System;
using System.Collections.Generic;
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

    protected override Parser<char> CreateIgnoreCaseParser()
    {
      HashSet<char> ignoreCaseSet = null;

      foreach (var character in myCharacters)
      {
        var alternative = InvertCharCase(character);
        if (alternative != character)
        {
          ignoreCaseSet = ignoreCaseSet ?? new HashSet<char>(myCharacters);
          ignoreCaseSet.Add(alternative);
        }
      }

      if (ignoreCaseSet == null) return this;

      var ignoreCaseCharsCount = ignoreCaseSet.Count;
      if (ignoreCaseCharsCount == myCharacters.Length) return this;

      var newCharacters = new char[ignoreCaseCharsCount];
      ignoreCaseSet.CopyTo(newCharacters);

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