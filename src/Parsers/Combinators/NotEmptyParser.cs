using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  public sealed class NotEmptyParser<T> : Parser<T>, Parser.IFailPoint
  {
    [NotNull] private readonly Parser<T> myUnderlyingParser;
    [CanBeNull] private readonly string myDescription;

    public NotEmptyParser([NotNull] Parser<T> underlyingParser, [CanBeNull] string description)
    {
      myUnderlyingParser = underlyingParser;
      myDescription = description;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var result = myUnderlyingParser.TryParse(input, offset);
      if (result.IsSuccessful)
      {
        if (result.Offset > offset) return result;

        return new ParseResult(failPoint: this, atOffset: offset);
      }

      return result;
    }

    public string GetExpectedMessage()
    {
      if (myDescription != null) return myDescription;

      var failPoint = myUnderlyingParser as IFailPoint;
      if (failPoint == null) return "non-empty string";

      return "non-empty " + failPoint.GetExpectedMessage();
    }
  }
}