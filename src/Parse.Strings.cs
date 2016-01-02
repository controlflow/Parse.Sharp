using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Strings;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<string> String([NotNull] string text)
    {
      return new StringParser(text);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<string> IgnoreCaseString([NotNull] string text)
    {
      return new IgnoreCaseStringParser(text);
    }
  }
}