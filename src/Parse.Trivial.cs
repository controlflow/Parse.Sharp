using System;
using System.Diagnostics;
using System.Threading;
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
      if (expectation == null) throw new ArgumentNullException("expectation");

      return new FailureParser<T>(expectation);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Named<T>([NotNull] this Parser<T> parser, [NotNull] string expectation)
    {
      if (parser == null) throw new ArgumentNullException("parser");
      if (expectation == null) throw new ArgumentNullException("expectation");

      return new NamedRuleParser<T>(parser, expectation);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Ref<T>([NotNull] Func<Parser<T>> parser)
    {
      if (parser == null) throw new ArgumentNullException("parser");

      var lazy = new Lazy<Parser<T>>(parser, LazyThreadSafetyMode.PublicationOnly);
      return new DelayedParser<T>(lazy);
    }
  }
}