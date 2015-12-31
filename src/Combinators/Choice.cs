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

    protected internal override ParseResult TryParse(string input, int offset, bool isConditional)
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

      // todo: reference equality is a bad thing, I guess

      // first parsed a bit
      if (leftResult.FailPoint != myLeft && rightResult.FailPoint == myRight)
      {
        return new ParseResult(leftResult.FailPoint, leftResult.Offset);
      }

      // second parsed a bit
      if (rightResult.FailPoint != myRight && leftResult.FailPoint == myLeft)
      {
        return new ParseResult(rightResult.FailPoint, rightResult.Offset);
      }

      return new ParseResult(failPoint: new ChoiseFailPoint(leftResult, rightResult), atOffset: offset);
    }

    class ChoiseFailPoint : IFailPoint
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