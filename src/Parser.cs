using System;
using JetBrains.Annotations;
using Parse.Sharp.Combinators;

namespace Parse.Sharp
{
  public abstract class Parser<T> : Parser
  {
    public T Parse(string input)
    {
      var result = TryParse(input, offset: 0, isConditional: false);
      if (result.IsSuccessful)
      {
        var endOffset = result.Offset;
        if (endOffset == input.Length) return result.Value;

        var message = Unexpected("End of string", input, endOffset);
        throw new ParseException(message, endOffset);
      }

      throw new ParseException(result.FailPoint.GetExpectedMessage(), result.Offset);
    }

    protected internal abstract ParseResult TryParse(string input, int offset, bool isConditional);

    protected internal struct ParseResult
    {
      private readonly T myValue;
      private readonly int myOffset;
      private readonly IFailPoint myFailPoint;

      public ParseResult(T value, int nextOffset)
      {
        myValue = value;
        myOffset = nextOffset;
        myFailPoint = null;
      }

      public ParseResult([NotNull] IFailPoint failPoint, int atOffset)
      {
        myValue = default(T);
        myOffset = atOffset;
        myFailPoint = failPoint;
      }

      public bool IsSuccessful { get { return myFailPoint == null; } }

      public T Value { get { return myValue; } }
      public int Offset { get { return myOffset; } }
      public IFailPoint FailPoint { get { return myFailPoint; } }
    }

    

    // utils:

    protected static string Unexpected(string expected, string input, int offset)
    {
      if (offset >= input.Length)
      {
        return expected + " expected, got end of string";
      }

      var tail = input.Substring(offset);
      if (tail.Length > 20)
      {
        tail = tail.Substring(0, 20) + "...";
      }

      return expected + " expected, got '" + ReplaceNewLines(tail) + "'";
    }

    // query syntax support:

    [Pure, NotNull]
    public Parser<TResult> Select<TResult>([NotNull] Func<T, TResult> selector)
    {
      return new SelectParser<T, TResult>(this, selector);
    }

    [Pure, NotNull]
    public Parser<TResult> SelectMany<TResult>([NotNull] Func<T, Parser<TResult>> nextParser)
    {
      return new SequentialParser<T, TResult>(this, nextParser);
    }

    [Pure, NotNull]
    public Parser<TResult> SelectMany<TNext, TResult>(
      [NotNull] Func<T, Parser<TNext>> nextParser, [NotNull] Func<T, TNext, TResult> selector)
    {
      return new SequentialParser<T, TNext, TResult>(this, nextParser, selector);
    }

    // conditional:

    public Parser<T> Or(Parser<T> other)
    {
      return new ChoiceParser<T>(this, other);
    }

    // qualifiers:

    public Parser<T[]> ZeroOrMany()
    {
      return new QuantifiedParser(this, min: 0, max: uint.MaxValue);
    }

    private class QuantifiedParser : Parser<T[]>
    {
      // todo: do not allocate for empty parsers

      private readonly Parser<T> myUnderlyingParser;
      private readonly uint myMin, myMax;

      public QuantifiedParser(Parser<T> parser, uint min, uint max)
      {
        myUnderlyingParser = parser;
        myMin = min;
        myMax = max;
      }

      protected internal override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        if (myMin > 0 && myMin == myMax)
        {
          // fixed size array
        }

        for (uint count = 0; count < myMax; count++)
        {
          // todo: compute if conditional!

          var result = myUnderlyingParser.TryParse(input, offset, isConditional);
          if (result.IsSuccessful)
          {

          }

        }


        // todo:
        return new ParseResult();
      }
    }

    public Parser<T> OptionalOrDefault()
    {
      return new OptionalParser(this, default(T));
    }

    public Parser<T> OptionalOrDefault(T defaultValue)
    {
      return new OptionalParser(this, defaultValue);
    }

    private class OptionalParser : Parser<T>
    {
      private readonly Parser<T> myParser;
      private readonly T myDefaultValue;

      public OptionalParser(Parser<T> parser, T defaultValue)
      {
        myParser = parser;
        myDefaultValue = defaultValue;
      }

      protected internal override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var result = myParser.TryParse(input, offset, isConditional: true);
        if (result.IsSuccessful) return result;

        return new ParseResult(myDefaultValue, offset);
      }
    }

    //private class ToNullable<TResult> : Parser<TResult?>
    //  where TResult : struct
    //{
    //  protected override ParseResult TryParse(string input, int offset, bool isConditional)
    //  {
    //    return new ParseResult();
    //  }
    //}
  }

  

  public abstract class Parser
  {
    //[NotNull, Pure]
    //protected internal virtual string GetExpectedMessage()
    //{
    //  throw new InvalidOperationException("Should not be invoked");
    //}

    

    private static readonly string[] NewLineStrings = {
      "\r\n", "\u0085", "\u2028", "\u2029", "\r", "\n"
    };

    protected static string ReplaceNewLines(string text)
    {
      foreach (var newLine in NewLineStrings)
      {
        text = text.Replace(newLine, " ");
      }

      return text;
    }

    [CannotApplyEqualityOperator]
    protected internal interface IFailPoint
    {
      [NotNull] string GetExpectedMessage();
    }
  }
}