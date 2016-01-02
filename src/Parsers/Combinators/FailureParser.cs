using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Combinators
{
  // note: should be choosen by choice combinator to show meaningful message?

  internal sealed class FailureParser<T> : ParserWithDescription<T>
  {
    public FailureParser([NotNull] string description) : base(description) { }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      return new ParseResult(failPoint: this, atOffset: offset);
    }
  }
}