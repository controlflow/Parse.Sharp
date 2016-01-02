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
    public static Parser<T> Fail<T>([NotNull] string description)
    {
      return new FailureParser<T>(description);
    }
  }
}