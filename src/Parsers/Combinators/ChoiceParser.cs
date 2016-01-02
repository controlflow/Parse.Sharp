using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class ChoiceParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser<T> myLeftParser;
    [NotNull] private readonly Parser<T> myRightParser;

    public ChoiceParser([NotNull] Parser<T> leftParser, [NotNull] Parser<T> rightParser)
    {
      myLeftParser = leftParser;
      myRightParser = rightParser;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var leftResult = myLeftParser.TryParseValue(input, offset);
      if (leftResult.IsSuccessful) return leftResult;

      var rightResult = myRightParser.TryParseValue(input, offset);
      if (rightResult.IsSuccessful) return rightResult;

      if (leftResult.Offset > rightResult.Offset)
      {
        return new ParseResult(leftResult.FailPoint, leftResult.Offset);
      }

      if (rightResult.Offset > leftResult.Offset)
      {
        return new ParseResult(rightResult.FailPoint, rightResult.Offset);
      }

      var failPoint = new ChoiseFailPoint(leftResult, rightResult);
      return new ParseResult(failPoint, offset);
    }

    private sealed class ChoiseFailPoint : IFailPoint
    {
      private readonly ParseResult myLeftResult;
      private readonly ParseResult myRightResult;

      public ChoiseFailPoint(ParseResult leftResult, ParseResult rightResult)
      {
        myLeftResult = leftResult;
        myRightResult = rightResult;
      }

      public string GetExpectedMessage()
      {
        return myLeftResult.FailPoint.GetExpectedMessage()
             + " or "
             + myRightResult.FailPoint.GetExpectedMessage();
      }
    }

    protected override Parser<T> CreateIgnoreCaseParser()
    {
      var ignoreCaseLeftParser = myLeftParser.IgnoreCase();
      var ignoreCaseRightParser = myRightParser.IgnoreCase();

      if (ReferenceEquals(myLeftParser, ignoreCaseLeftParser) &&
          ReferenceEquals(myRightParser, ignoreCaseRightParser))
      {
        return this;
      }

      return new ChoiceParser<T>(myLeftParser, myRightParser);
    }
  }

  // todo: choice of array
  // todo: normalization of same-level choices
}