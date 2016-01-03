using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Combinators;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Return<T>(T value)
    {
      return new ReturnParser<T>(value);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Fail<T>([NotNull] string expectation)
    {
      return new FailureParser<T>(expectation);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Named<T>([NotNull] this Parser<T> parser, [NotNull] string expectation)
    {
      if (parser == null) throw new ArgumentNullException("parser");

      return new NamedRuleParser<T>(parser, expectation);
    }
  }
}