using System;
using JetBrains.Annotations;

namespace Parse.Sharp.Parsers.Characters
{
  internal sealed class PredicateCharacterParser : ParserWithDescription<char>
  {
    [NotNull] private readonly Predicate<char> myPredicate;
    private readonly bool myIsExcept;

    public PredicateCharacterParser([NotNull] Predicate<char> predicate, [NotNull] string description, bool isExcept)
      : base(description)
    {
      myPredicate = predicate;
      myIsExcept = isExcept;

      AssertParserAllocation();
    }

    protected internal override ParseResult TryParseValue(string input, int offset)
    {
      if (offset < input.Length)
      {
        var ch = input[offset];
        var match = myPredicate(ch);
        if (match != myIsExcept)
        {
          return new ParseResult(value: ch, nextOffset: offset + 1);
        }
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public override Parser<char> IgnoreCase()
    {
      if (myPredicate.Target is IgnoreCasePredicate) return this;

      var predicate = new IgnoreCasePredicate(myPredicate);
      return new PredicateCharacterParser(predicate.IgnoreCase, Description, myIsExcept);
    }

    private sealed class IgnoreCasePredicate
    {
      [NotNull] private readonly Predicate<char> myPredicate;

      public IgnoreCasePredicate([NotNull] Predicate<char> predicate)
      {
        myPredicate = predicate;
      }

      public bool IgnoreCase(char ch)
      {
        if (myPredicate(ch)) return true;

        var alternative = InvertCharCase(ch);
        if (alternative == ch) return false;

        return myPredicate(alternative);
      }
    }
  }
}