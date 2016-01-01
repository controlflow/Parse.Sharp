using JetBrains.Annotations;

namespace Parse.Sharp.Parsers
{
  internal sealed class NotParser<T> : Parser<object>, Parser.IFailPoint
  {
    [NotNull] private readonly Parser<T> myParser;
    [CanBeNull] private readonly string myDescription;

    public NotParser([NotNull] Parser<T> parser, [CanBeNull] string description)
    {
      myParser = parser;
      myDescription = description;
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      var result = myParser.TryParseValue(input, offset);
      if (result.IsSuccessful)
      {
        return new ParseResult(failPoint: this, atOffset: offset);
      }

      return new ParseResult(value: null, nextOffset: offset);
    }

    public string GetExpectedMessage()
    {
      if (myDescription != null) return myDescription;

      var failPoint = myParser as IFailPoint;
      if (failPoint == null) return "not"; // eww

      return "not " + failPoint.GetExpectedMessage();
    }
  }
}