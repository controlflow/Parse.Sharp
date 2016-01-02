using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class PredicateCharacterParser : Parser<char>, Parser.IFailPoint
  {
    [NotNull] private readonly Predicate<char> myPredicate;
    [NotNull] private readonly string myDescription;
    private readonly bool myIsNegative;

    public PredicateCharacterParser([NotNull] Predicate<char> predicate, [NotNull] string description, bool isNegative)
    {
      myPredicate = predicate;
      myDescription = description;
      myIsNegative = isNegative;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length)
      {
        var ch = input[offset];
        if (myPredicate(ch) != myIsNegative)
        {
          return new ParseResult(value: ch, nextOffset: offset + 1);
        }
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public string GetExpectedMessage()
    {
      return myDescription;
    }
  }
}