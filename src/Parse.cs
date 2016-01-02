using System;
using System.Diagnostics;
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

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> Char([NotNull] Predicate<char> predicate, [NotNull] string description)
    {
      return new PredicateCharacterParser(predicate, description, isNegative: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> CharExcept([NotNull] Predicate<char> predicate, [NotNull] string description)
    {
      return new PredicateCharacterParser(predicate, description, isNegative: true);
    }

    // character set:

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> Chars([NotNull] string characters)
    {
      return new CharacterSetParser(characters.ToCharArray(), description: null, isNegative: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> CharsExcept([NotNull] string characters)
    {
      return new CharacterSetParser(characters.ToCharArray(), description: null, isNegative: true);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> Chars([NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description: null, isNegative: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> CharsExcept([NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description: null, isNegative: true);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> Chars([NotNull] string description, [NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description, isNegative: false);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<char> CharsExcept([NotNull] string description, [NotNull] params char[] characters)
    {
      return new CharacterSetParser(characters, description, isNegative: true);
    }

    // todo: .CharsIgnoreCase?

    // + ManyCharacters()

    // strings:

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<string> String([NotNull] string text)
    {
      return new StringParser(text);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<string> IgnoreCaseString([NotNull] string text)
    {
      return new IgnoreCaseStringParser(text);
    }

    

    // combinators:

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Return<T>(T value)
    {
      return new ReturnParser<T>(value);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Choice<T>([NotNull] Parser<T> left, [NotNull] Parser<T> right)
    {
      return left.Or(right);
    }

    // .And()
    // .Or()
    // .XOr()

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

    // allocations assert:

    [NotNull, Pure, DebuggerStepThrough]
    public static IDisposable AssertNoAllocations()
    {
      Parser.AssertAvoidParserAllocations = true;
      return AllocationsAssertDisposable.Instance;
    }

    private sealed class AllocationsAssertDisposable : IDisposable
    {
      [NotNull] public static readonly IDisposable Instance = new AllocationsAssertDisposable();

      public void Dispose() { Parser.AssertAvoidParserAllocations = false; }
    }
  }

  // todo: .Ref(() => parser)
}