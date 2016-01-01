using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Strings
{
  internal sealed class NonEmptyStringParser : Parser<string>, Parser.IFailPoint
  {
    [NotNull] private readonly Parser<string> myUnderlyingParser;
    [CanBeNull] private readonly string myDescription;

    public NonEmptyStringParser([NotNull] Parser<string> underlyingParser, [CanBeNull] string description)
    {
      myUnderlyingParser = underlyingParser;
      myDescription = description;
    }

    protected internal override ParseResult TryParse(string input, int offset)
    {
      var result = myUnderlyingParser.TryParse(input, offset);
      if (result.IsSuccessful && string.IsNullOrEmpty(result.Value))
      {
        return new ParseResult(failPoint: this, atOffset: offset);
      }

      return result;
    }

    public string GetExpectedMessage()
    {
      if (myDescription != null) return myDescription;

      var failPoint = myUnderlyingParser as IFailPoint;
      if (failPoint == null) return "non empty";

      return "non empty " + failPoint.GetExpectedMessage();
    }
  }
}