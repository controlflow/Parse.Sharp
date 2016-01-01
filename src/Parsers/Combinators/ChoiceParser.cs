using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  internal sealed class ChoiceParser<T> : Parser<T>
  {
    [NotNull] private readonly Parser<T> myLeft;
    [NotNull] private readonly Parser<T> myRight;

    public ChoiceParser([NotNull] Parser<T> left, [NotNull] Parser<T> right)
    {
      myLeft = left;
      myRight = right;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var leftResult = myLeft.TryParseValue(input, offset);
      if (leftResult.IsSuccessful) return leftResult;

      var rightResult = myRight.TryParseValue(input, offset);
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
  }
}