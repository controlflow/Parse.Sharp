using System;

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

      throw new ParseException(result.ErrorMessage, result.Offset);
    }

    protected abstract ParseResult TryParse(string input, int offset, bool isConditional);

    protected struct ParseResult
    {
      private readonly T myValue;
      private readonly int myOffset;
      private readonly string myError;

      public ParseResult(T value, int offset)
      {
        myValue = value;
        myOffset = offset;
        myError = null;
      }

      public ParseResult(string message, int offset)
      {
        myValue = default(T);
        myOffset = offset;
        myError = message;
      }

      public bool IsSuccessful { get { return myError == null; } }

      public T Value { get { return myValue; } }
      public int Offset { get { return myOffset; } }
      public string ErrorMessage { get { return myError; } }
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

    // linq:

    public Parser<TResult> Select<TResult>(Func<T, TResult> selector)
    {
      return new SelectorParser<TResult>(this, selector);
    }

    public Parser<TResult> SelectMany<TResult>(Func<T, Parser<TResult>> nextParser)
    {
      return new SequentialParser<TResult>(this, nextParser);
    }

    public Parser<TResult> SelectMany<TNext, TResult>(Func<T, Parser<TNext>> nextParser, Func<T, TNext, TResult> selector)
    {
      return new SequentialParser<TNext, TResult>(this, nextParser, selector);
    }

    private class SelectorParser<TResult> : Parser<TResult>
    {
      private readonly Parser<T> myUnderlyingParser;
      private readonly Func<T, TResult> mySelector;

      public SelectorParser(Parser<T> underlyingParser, Func<T, TResult> selector)
      {
        myUnderlyingParser = underlyingParser;
        mySelector = selector;
      }

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var result = myUnderlyingParser.TryParse(input, offset, isConditional);
        if (result.IsSuccessful)
        {
          return new ParseResult(mySelector(result.Value), result.Offset);
        }

        return new ParseResult(result.ErrorMessage, result.Offset);
      }
    }

    private class SequentialParser<TResult> : Parser<TResult>
    {
      private readonly Parser<T> myFirstParser;
      private readonly Func<T, Parser<TResult>> myNextParser;

      public SequentialParser(Parser<T> firstParser, Func<T, Parser<TResult>> nextParser)
      {
        myFirstParser = firstParser;
        myNextParser = nextParser;
      }

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var result = myFirstParser.TryParse(input, offset, isConditional);
        if (!result.IsSuccessful)
        {
          return new ParseResult(result.ErrorMessage, result.Offset);
        }

        var nextParser = myNextParser(result.Value);
        return nextParser.TryParse(input, result.Offset, isConditional);
      }
    }

    private class SequentialParser<TNext, TResult> : Parser<TResult>
    {
      private readonly Parser<T> myFirstParser;
      private readonly Func<T, Parser<TNext>> myNextParser;
      private readonly Func<T, TNext, TResult> mySelector;

      public SequentialParser(Parser<T> firstParser, Func<T, Parser<TNext>> nextParser, Func<T, TNext, TResult> selector)
      {
        myFirstParser = firstParser;
        myNextParser = nextParser;
        mySelector = selector;
      }

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var result = myFirstParser.TryParse(input, offset, isConditional);
        if (!result.IsSuccessful)
        {
          return new ParseResult(result.ErrorMessage, result.Offset);
        }

        var nextParser = myNextParser(result.Value);
        var nextResult = nextParser.TryParse(input, result.Offset, isConditional);
        if (!nextResult.IsSuccessful)
        {
          return new ParseResult(nextResult.ErrorMessage, nextResult.Offset);
        }

        var value = mySelector(result.Value, nextResult.Value);
        return new ParseResult(value, nextResult.Offset);
      }
    }

    // conditional:

    public Parser<T> Or(Parser<T> other)
    {
      return new ChoiceParser(this, other);
    }

    private class ChoiceParser : Parser<T>
    {
      private readonly Parser<T> myLeft;
      private readonly Parser<T> myRight;

      public ChoiceParser(Parser<T> left, Parser<T> right)
      {
        myLeft = left;
        myRight = right;
      }

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var leftResult = myLeft.TryParse(input, offset, isConditional: true);
        if (leftResult.IsSuccessful)
        {
          return new ParseResult(leftResult.Value, leftResult.Offset);
        }

        var rightResult = myRight.TryParse(input, offset, isConditional: true);
        if (rightResult.IsSuccessful)
        {
          return rightResult;
        }

        return new ParseResult(leftResult.ErrorMessage + " or " + rightResult.ErrorMessage, offset);
      }
    }

    public Parser<TResult> Or<T2, TResult>(Parser<T2> other, Func<T, TResult> leftSelector, Func<T2, TResult> rightSelector)
    {
      return new ChoiceParser<T2, TResult>(this, other, leftSelector, rightSelector);
    }

    private class ChoiceParser<T2, TResult> : Parser<TResult>
    {
      private readonly Parser<T> myLeft;
      private readonly Parser<T2> myRight;
      private readonly Func<T, TResult> myLeftSelector;
      private readonly Func<T2, TResult> myRightSelector;

      public ChoiceParser(Parser<T> left, Parser<T2> right, Func<T, TResult> leftSelector, Func<T2, TResult> rightSelector)
      {
        myLeft = left;
        myRight = right;
        myLeftSelector = leftSelector;
        myRightSelector = rightSelector;
      }

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var leftResult = myLeft.TryParse(input, offset, isConditional);
        if (leftResult.IsSuccessful)
        {
          return new ParseResult(myLeftSelector(leftResult.Value), leftResult.Offset);
        }

        var rightResult = myRight.TryParse(input, offset, isConditional);
        if (rightResult.IsSuccessful)
        {
          return new ParseResult(myRightSelector(rightResult.Value), rightResult.Offset);
        }

        return new ParseResult();
      }
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

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
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

      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        var result = myParser.TryParse(input, offset, isConditional: true);
        if (result.IsSuccessful) return result;

        return new ParseResult(myDefaultValue, offset);
      }
    }

    private class ToNullable<TResult> : Parser<TResult?>
      where TResult : struct
    {
      protected override ParseResult TryParse(string input, int offset, bool isConditional)
      {
        return new ParseResult();
      }
    }
  }

  public abstract class Parser
  {
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
  }
}