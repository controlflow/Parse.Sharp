using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Combinators;
using Parse.Sharp.Parsers.Strings;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<List<T>> Many<T>([NotNull] this Parser<T> parser, uint count)
    {
      return new QuantifiedParser<T>(parser, count, count);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<List<T>> Many<T>([NotNull] this Parser<T> parser, uint min, uint max)
    {
      return new QuantifiedParser<T>(parser, min, max);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<List<T>> AtLeastOnce<T>([NotNull] this Parser<T> parser)
    {
      return new QuantifiedParser<T>(parser, min: 1, max: uint.MaxValue);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<string> ManyToString<T>([NotNull] this Parser<T> parser)
    {
      return new ManyToStringParser(parser);
    }



    // .Many(min: )
    // .AtLeastOnce()
    // .Times(n)
  }
}