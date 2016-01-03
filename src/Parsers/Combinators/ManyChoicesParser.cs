using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class ManyChoicesParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser<T>[] myParsers;

    public ManyChoicesParser([NotNull] Parser<T>[] parsers) { myParsers = parsers; }
    public ManyChoicesParser([NotNull] List<Parser<T>> parsers) : this(parsers.ToArray()) { }

    [NotNull] public Parser<T>[] Parsers { get { return myParsers; } }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var bestFailure = new ParseResult(); // dangerous :(
      var otherBestFailures = default(List<ParseResult>);

      foreach (var parser in myParsers)
      {
        var result = parser.TryParseValue(input, offset);
        if (result.IsSuccessful) return result;

        if (bestFailure.FailPoint == null)
        {
          bestFailure = result;
        }
        else if (result.Offset > bestFailure.Offset)
        {
          bestFailure = result;
          if (otherBestFailures != null) otherBestFailures.Clear();
        }
        else if (result.Offset == bestFailure.Offset)
        {
          otherBestFailures = otherBestFailures ?? new List<ParseResult>();
          otherBestFailures.Add(result);
        }
      }

      if (otherBestFailures == null || otherBestFailures.Count == 0)
        return bestFailure;

      var failPoint = new ChoiceFailPoint(bestFailure, otherBestFailures);
      return new ParseResult(failPoint: failPoint, atOffset: bestFailure.Offset);
    }

    private sealed class ChoiceFailPoint : IFailPoint
    {
      private readonly ParseResult myBestFailure;
      [NotNull] private readonly List<ParseResult> myOtherFailures;

      public ChoiceFailPoint(ParseResult bestFailure, [NotNull] List<ParseResult> otherFailures)
      {
        myBestFailure = bestFailure;
        myOtherFailures = otherFailures;
      }

      public string GetExpectedMessage()
      {
        var builder = new StringBuilder(myBestFailure.FailPoint.GetExpectedMessage());
        var separator = myOtherFailures.Count > 2 ? "|" : " or ";

        foreach (var failure in myOtherFailures)
        {
          builder.Append(separator).Append(failure.FailPoint.GetExpectedMessage());
        }

        return builder.ToString();
      }
    }

  }
}