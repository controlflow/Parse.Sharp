using System;
using JetBrains.Annotations;

// ReSharper disable RedundantToStringCallForValueType

namespace Parse.Sharp
{
  [PublicAPI]
  public class Parse
  {
    public static readonly Parser<int> Digit = new DigitParser();

    private class DigitParser : Parser<int>
    {
      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        if (offset < input.Length)
        {
          var ch = input[offset];
          if (ch >= '0' && ch <= '9')
          {
            return new ParseResult(ch - '0', offset + 1);
          }
        }

        return new ParseResult(Unexpected("Digit", input, offset), offset);
      }
    }

    public static readonly Parser<int> Int32 = new IntParser();

    private class IntParser : Parser<int>
    {
      // todo: handle int32 overflow

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var value = default(int);
        var parsed = false;

        for (; offset < input.Length; offset++)
        {
          var ch = input[offset];
          if (ch < '0' || ch > '9') break;

          var digit = ch - '0';
          value = value * 10 + digit;
          parsed = true;
        }

        if (parsed)
        {
          return new ParseResult(value, offset);
        }

        return new ParseResult(Unexpected("Integer", input, offset), offset);
      }
    }

    public static readonly Parser<char> Dot = new CharParser('.');

    public static Parser<char> Char(char character)
    {
      return new CharParser(character);
    }

    private class CharParser : Parser<char>
    {
      private readonly char myExpectedCharacter;

      public CharParser(char expectedCharacter)
      {
        myExpectedCharacter = expectedCharacter;
      }

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        if (offset < input.Length && input[offset] == myExpectedCharacter)
        {
          return new ParseResult(myExpectedCharacter, offset + 1);
        }

        var message = "Char '" + myExpectedCharacter.ToString() + "'";
        return new ParseResult(Unexpected(message, input, offset), offset);
      }
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

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
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