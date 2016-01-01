using System;
using JetBrains.Annotations;
using Parse.Sharp.Parsers;
using Parse.Sharp.Parsers.Characters;
using Parse.Sharp.Parsers.Combinators;
using Parse.Sharp.Parsers.Strings;

namespace Parse.Sharp
{
  // todo: Parse.SeparatedList(item, separator)

  [PublicAPI]
  public class Parse
  {
    // digits and numbers:

    [NotNull] public static readonly Parser<int> Digit = new DigitParser();

    [NotNull] public static readonly Parser<int> Int32 = new Integer32Parser();

    // characters:

    [NotNull] public static readonly Parser<char> AnyChar = new AnyCharacterParser();

    [NotNull] public static readonly Parser<char> Dot = new CharacterParser('.');
    [NotNull] public static readonly Parser<char> Comma = new CharacterParser(',');
    [NotNull] public static readonly Parser<char> LBrace = new CharacterParser('[');
    [NotNull] public static readonly Parser<char> RBrace = new CharacterParser(']');

    [NotNull, Pure]
    public static Parser<char> Char(char character)
    {
      return new CharacterParser(character);
    }

    [NotNull] public static readonly Parser<char> DigitChar = Char(char.IsDigit, "digit character");
    [NotNull] public static readonly Parser<char> LetterChar = Char(char.IsLetter, "letter character");
    [NotNull] public static readonly Parser<char> LetterOrDigitChar = Char(char.IsLetterOrDigit, "letter or digit character");
    [NotNull] public static readonly Parser<char> LowerCaseChar = Char(char.IsLower, "lower case character");
    [NotNull] public static readonly Parser<char> UpperCaseChar = Char(char.IsUpper, "upper case character");
    [NotNull] public static readonly Parser<char> WhitespaceChar = Char(char.IsWhiteSpace, "whitespace");

    [NotNull, Pure]
    public static Parser<char> Char([NotNull] Predicate<char> predicate, [NotNull] string description)
    {
      return new PredicateCharacterParser(predicate, description);
    }

    // strings:

    [NotNull, Pure] public static Parser<string> String([NotNull] string text)
    {
      return new StringParser(text);
    }

    [NotNull, Pure] public static Parser<string> IgnoreCaseString([NotNull] string text)
    {
      return new IgnoreCaseStringParser(text);
    }

    // combinators:

    [NotNull, Pure] public static Parser<T> Return<T>(T value)
    {
      return new ReturnParser<T>(value);
    }

    [NotNull, Pure]
    public static Parser<T> Choice<T>([NotNull] Parser<T> left, [NotNull] Parser<T> right)
    {
      return left.Or(right);
    }

    public static Parser Sequence<T1, T2>(Parser<T1> firstParser, Parser<T2> secondParser)
    {
      throw new NotImplementedException();
    }

    public static Parser<TResult> Sequence<T1, T2, T3, TResult>(
      Parser<T1> firstParser,
      Parser<T2> secondParser,
      Parser<T3> thirdParser,
      Func<T1, T2, T3, TResult> f)
    {
      throw new NotImplementedException();
    }
  }
}