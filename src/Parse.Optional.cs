using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Combinators;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Optional<T>([NotNull] this Parser<T> parser)
      where T : class
    {
      return new OptionalParser<T>(parser, default(T));
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Optional<T>([NotNull] this Parser<T> parser, T defaultValue)
    {
      return new OptionalParser<T>(parser, defaultValue);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T?> OptionalToNullable<T>([NotNull] this Parser<T> parser)
      where T : struct
    {
      return new NullableOptionalParser<T>(parser);
    }
  }
}