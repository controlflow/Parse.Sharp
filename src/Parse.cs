using System;
using JetBrains.Annotations;
using Parse.Sharp.Parsers;

// ReSharper disable RedundantToStringCallForValueType

namespace Parse.Sharp
{
  [PublicAPI]
  public class Parse
  {
    public static readonly Parser<int> Digit = new DigitParser();

    

    public static readonly Parser<int> Int32 = new Integer32Parser();



    // characters:

    public static readonly Parser<char> AnyChar = new AnyCharacterParser(); 

    public static readonly Parser<char> Dot = new CharacterParser('.');
    public static readonly Parser<char> Comma = new CharacterParser(',');
    public static readonly Parser<char> LBrace = new CharacterParser('[');
    public static readonly Parser<char> RBrace = new CharacterParser(']');

    public static Parser<char> Char(char character)
    {
      return new CharacterParser(character);
    }

    // text:

    public static Parser<string> Text([NotNull] string text)
    {
      return new TextParser(text);
    }
    

    public static Parser<T> Return<T>(T value)
    {
      return new ReturnParser<T>(value);
    }

    private class ReturnParser<T> : Parser<T>
    {
      private readonly T myValue;

      public ReturnParser(T value)
      {
        myValue = value;
      }

      protected internal override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        return new ParseResult(myValue, offset);
      }
    }

    public static Parser<T> Choice<T>(Parser<T> left, Parser<T> right)
    {
      return left.Or(right);
    }

    //public static Parser<T?> OptionalOrNull<T>(this Parser<T> parser) where T : struct
    //{
    //  return null;
    //}

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