using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers;
using Parse.Sharp.Parsers.Combinators;
using Parse.Sharp.Parsers.Strings;

namespace Parse.Sharp
{
  public abstract class Parser<T> : Parser
  {
    [Pure] public T Parse([NotNull] string input)
    {
      var result = TryParseValue(input, offset: 0);
      if (result.IsSuccessful)
      {
        var endOffset = result.Offset;
        if (endOffset == input.Length) return result.Value;

        var message = Unexpected("end of string", input, endOffset);
        throw new ParseException(message, endOffset);
      }
      else
      {
        var expected = result.FailPoint.GetExpectedMessage();
        var message = Unexpected(expected, input, result.Offset);
        throw new ParseException(message, result.Offset);
      }
    }

    [Pure] protected internal abstract ParseResult TryParseValue([NotNull] string input, int offset);

    // todo: check out inheritors for more efficient implementations
    protected internal sealed override ParseAttempt TryParseVoid(string input, int offset)
    {
      var parseResult = TryParseValue(input, offset);
      if (parseResult.IsSuccessful)
      {
        return new ParseAttempt(parseResult.Offset);
      }

      return new ParseAttempt(parseResult.FailPoint, parseResult.Offset);
    }

    [DebuggerDisplay("{ToString()}")]
    protected internal struct ParseResult
    {
      [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly T myValue;
      [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int myOffset;
      [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly IFailPoint myFailPoint;

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

      public override string ToString()
      {
        return IsSuccessful
          ? string.Format("Success[offset={0}]: {1}", Offset, Value)
          : string.Format("Failure[offset={0}]: {1}", Offset, FailPoint.GetExpectedMessage());
      }
    }

    [Pure, NotNull, DebuggerStepThrough]
    public new Parser<T> IgnoreCase() { return CreateIgnoreCaseParser(); }

    [Pure, DebuggerStepThrough]
    protected sealed override Parser IgnoreCaseVoid() { return CreateIgnoreCaseParser(); }

    // todo: to abstract!
    [Pure, NotNull, DebuggerStepThrough]
    protected virtual Parser<T> CreateIgnoreCaseParser() { return this; }

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

    protected static char InvertCharCase(char ch)
    {
      var lower = char.ToLowerInvariant(ch);
      if (lower != ch) return lower;

      return char.ToUpperInvariant(ch);
    }


    [NotNull, Pure, DebuggerStepThrough]
    public Parser<string> ManyToString([CanBeNull] string description = null)
    {
      return new ManyToStringParser(this, description);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> SurroundWith([NotNull] Parser headAndTailParser)
    {
      return new SurroundParser<T>(headAndTailParser, this, headAndTailParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> SurroundWith([NotNull] Parser headParser, [NotNull] Parser tailParser)
    {
      return new SurroundParser<T>(headParser, this, tailParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> WithTail([NotNull] Parser tailParser)
    {
      return new AfterParserTest<T>(this, tailParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> Token()
    {
      // todo: whitespace.many()
      var whitespace = Sharp.Parse.WhitespaceChar;
      return new SurroundParser<T>(whitespace, this, whitespace);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> WhitespaceAfter()
    {
      // todo: whitespace.many()
      return new AfterParserTest<T>(this, Sharp.Parse.WhitespaceChar);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> WhitespaceBefore()
    {
      // todo: whitespace.many()
      return new AfterParserTest<T>(this, Sharp.Parse.WhitespaceChar);
    }



    // query syntax support:

    [Pure, NotNull, DebuggerStepThrough]
    public Parser<TResult> Select<TResult>([NotNull] Func<T, TResult> selector)
    {
      return new SelectParser<T, TResult>(this, selector);
    }

    [Pure, NotNull, DebuggerStepThrough]
    public Parser<TResult> SelectMany<TResult>([NotNull] Func<T, Parser<TResult>> nextParser)
    {
      return new SequentialParser<T, TResult>(this, nextParser);
    }

    [Pure, NotNull, DebuggerStepThrough]
    public Parser<TResult> SelectMany<TNext, TResult>(
      [NotNull] Func<T, Parser<TNext>> nextParser, [NotNull] Func<T, TNext, TResult> selector)
    {
      return new SequentialParser<T, TNext, TResult>(this, nextParser, selector);
    }

    [Pure, NotNull, DebuggerStepThrough]
    public Parser<TAccumulate> Aggregate<TAccumulate>(
      TAccumulate seed, [NotNull] Func<TAccumulate, T, TAccumulate> fold)
    {
      return new AggregateParser<T, TAccumulate>(this, () => seed, fold);
    }

    [Pure, NotNull, DebuggerStepThrough]
    public Parser<TAccumulate> Aggregate<TAccumulate>(
      [NotNull] Func<TAccumulate> seedFactory, [NotNull] Func<TAccumulate, T, TAccumulate> fold)
    {
      return new AggregateParser<T, TAccumulate>(this, seedFactory, fold);
    }

    // combinators:

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> Or(Parser<T> other)
    {
      return new ChoiceParser<T>(this, other);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<object> Not([CanBeNull] string description = null)
    {
      return new NotParser<T>(this, description);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> NonEmpty([CanBeNull] string description = null)
    {
      return new NonEmptyParser<T>(this, description);
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

        AssertParserAllocation();
      }

      protected internal override ParseResult TryParseValue(string input, int offset)
      {
        if (myMin > 0 && myMin == myMax)
        {
          // fixed size array
        }

        for (uint count = 0; count < myMax; count++)
        {
          // todo: compute if conditional!

          var result = myUnderlyingParser.TryParseValue(input, offset);
          if (result.IsSuccessful)
          {

          }

        }


        // todo:
        return new ParseResult();
      }
    }

    [NotNull, Pure]
    public Parser<T> OptionalOrDefault()
    {
      return new OptionalParser<T>(this, default(T));
    }

    [NotNull, Pure]
    public Parser<T> OptionalOrDefault(T defaultValue)
    {
      return new OptionalParser<T>(this, defaultValue);
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
    protected internal abstract ParseAttempt TryParseVoid([NotNull] string input, int offset);

    [DebuggerDisplay("{ToString()}")]
    protected internal struct ParseAttempt
    {
      [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly int myOffset;
      [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly IFailPoint myFailPoint;

      public ParseAttempt(int nextOffset)
      {
        myOffset = nextOffset;
        myFailPoint = null;
      }

      public ParseAttempt([NotNull] IFailPoint failPoint, int atOffset)
      {
        myOffset = atOffset;
        myFailPoint = failPoint;
      }

      public bool IsSuccessful { get { return myFailPoint == null; } }

      public int Offset { get { return myOffset; } }
      public IFailPoint FailPoint { get { return myFailPoint; } }

      public override string ToString()
      {
        return IsSuccessful
          ? string.Format("Success[offset={0}]", Offset)
          : string.Format("Failure[offset={0}]: {1}", Offset, FailPoint.GetExpectedMessage());
      }
    }


    [NotNull, Pure, DebuggerStepThrough]
    public Parser IgnoreCase() { return IgnoreCaseVoid(); }

    [Pure, NotNull]
    protected abstract Parser IgnoreCaseVoid();


    [NotNull, Pure, DebuggerStepThrough]
    public Parser<T> Select<T>(T value)
    {
      return new SelectParser<T>(this, value);
    }


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

    [ThreadStatic] internal static bool AssertAvoidParserAllocations;

    protected static void AssertParserAllocation()
    {
      if (AssertAvoidParserAllocations)
        throw new ArgumentException("Parser allocation");
    }

    //public abstract Parser IgnoreCase();
  }
}