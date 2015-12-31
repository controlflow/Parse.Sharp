namespace Parse.Sharp.Combinators
{
  internal sealed class ChoiceParser<T> : Parser<T>
  {
    private readonly Parser<T> myLeft;
    private readonly Parser<T> myRight;

    public ChoiceParser(Parser<T> left, Parser<T> right)
    {
      myLeft = left;
      myRight = right;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var leftResult = myLeft.TryParse(input, offset);
      if (leftResult.IsSuccessful) return leftResult;

      var rightResult = myRight.TryParse(input, offset);
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