using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Characters;

namespace Parse.Sharp
{
  public partial class Parse
  {
    [NotNull] public static readonly Parser<char> AnyChar = AnyCharacterParser.Instance;

    [NotNull] public static readonly Parser<char> Dot = new CharacterParser('.');
    [NotNull] public static readonly Parser<char> Comma = new CharacterParser(',');
    [NotNull] public static readonly Parser<char> LBrace = new CharacterParser('[');
    [NotNull] public static readonly Parser<char> RBrace = new CharacterParser(']');

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> Char(char character)
    {
      return new CharacterParser(character);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> CharExcept(char character)
    {
      return new CharacterExceptParser(character);
    }

    [NotNull] public static readonly Parser<char> DigitChar = Char(char.IsDigit, "digit character");
    [NotNull] public static readonly Parser<char> LetterChar = Char(char.IsLetter, "letter character");
    [NotNull] public static readonly Parser<char> LetterOrDigitChar = Char(char.IsLetterOrDigit, "letter or digit character");
    [NotNull] public static readonly Parser<char> LowerCaseChar = Char(char.IsLower, "lower case character");
    [NotNull] public static readonly Parser<char> UpperCaseChar = Char(char.IsUpper, "upper case character");
    [NotNull] public static readonly Parser<char> WhitespaceChar = Char(char.IsWhiteSpace, "whitespace");

    [NotNull] public static readonly Parser Whitespace = WhitespaceChar.Many();

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> Char([NotNull] Predicate<char> predicate, [NotNull] string description)
    {
      return new PredicateCharacterParser(predicate, description, isExcept: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> CharExcept([NotNull] Predicate<char> predicate, [NotNull] string description)
    {
      return new PredicateCharacterParser(predicate, description, isExcept: true);
    }

    // character sets:

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> AnyCharOf([NotNull] string characters)
    {
      return new CharacterSetParser(characters.ToCharArray(), description: null, isExcept: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> AnyCharExcept([NotNull] string characters)
    {
      return new CharacterSetParser(characters.ToCharArray(), description: null, isExcept: true);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> AnyCharOf([NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description: null, isExcept: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> AnyCharExcept([NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description: null, isExcept: true);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> AnyCharOf([NotNull] string description, [NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description, isExcept: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> AnyCharExcept([NotNull] string description, [NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description, isExcept: true);
    }
  }
}