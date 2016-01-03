using System;
using System.Text;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class QuantifiedParserToString<T> : Parser<string>
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    private readonly uint myMin, myMax;

    public QuantifiedParserToString([NotNull] Parser<T> parser, uint min, uint max)
    {
      myUnderlyingParser = parser;
      myMin = min;
      myMax = max;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var charParser = myUnderlyingParser as Parser<char>;
      if (charParser != null)
        return TryParseValueCharOptimized(charParser, input, offset);

      var builder = new StringBuilder(capacity: (int)myMin);

      uint index = 0;
      while (index < myMax)
      {
        var result = myUnderlyingParser.TryParseValue(input, offset);
        if (result.IsSuccessful)
        {
          if (offset == result.Offset)
            throw new ArgumentException("Infinite iteration detected");

          builder.Append(result.Value); // boxes each char if T is 'char'

          offset = result.Offset;
          index++;

          if (index == myMax) break;
        }
        else
        {
          if (result.Offset == offset && index >= myMin) break;

          return new ParseResult(failPoint: result.FailPoint, atOffset: result.Offset);
        }
      }

      return new ParseResult(value: builder.ToString(), nextOffset: offset);
    }

    [Pure] private ParseResult TryParseValueCharOptimized([NotNull] Parser<char> parser, string input, int offset)
    {
      StringBuilder builder = null;

      var count = 0;
      while (count < myMax)
      {
        var result = parser.TryParseValue(input, offset);
        if (result.IsSuccessful)
        {
          var delta = result.Offset - offset;
          if (delta == 0)
            throw new ArgumentException("Infinite iteration detected");

          if (delta == 1 && result.Value == input[offset])
          {
            if (builder != null) builder.Append(result.Value);
            // skip character otherwise
          }
          else // character projection or multiple characters offset (like escaping decoding)
          {
            builder = builder ?? new StringBuilder().Append(input, offset - count, count);
            builder.Append(result.Value);
          }

          offset = result.Offset;
          count++;

          if (count == myMax) break;
        }
        else
        {
          if (result.Offset == offset && count >= myMin) break;

          return new ParseResult(failPoint: result.FailPoint, atOffset: result.Offset);
        }
      }

      var resultText = (builder == null) ? input.Substring(offset - count, count) : builder.ToString();
      return new ParseResult(resultText, offset);
    }

    protected internal override ParseAttempt TryParseVoid(string input, int offset)
    {
      throw new ArgumentException(".ManyToString() should not be used in positions where it's value is ignored");
    }

    protected override Parser<string> CreateIgnoreCaseParser()
    {
      var ignoreCaseParser = myUnderlyingParser.IgnoreCase();
      if (ReferenceEquals(myUnderlyingParser, ignoreCaseParser)) return this;

      return new QuantifiedParserToString<T>(ignoreCaseParser, myMin, myMax);
    }
  }
}