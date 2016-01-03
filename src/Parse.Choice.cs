using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Combinators;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Or<T>([NotNull] this Parser<T> parser, [NotNull] Parser<T> other)
    {
      if (parser == null) throw new ArgumentNullException("parser");
      if (other == null) throw new ArgumentNullException("other");

      return Choice(parser, other);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Choice<T>([NotNull] Parser<T> left, [NotNull] Parser<T> right)
    {
      if (left == null) throw new ArgumentNullException("left");
      if (right == null) throw new ArgumentNullException("right");

      return new ChoiceParser<T>(left, right);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Choice<T>([NotNull] params Parser<T>[] parsers)
    {
      // todo: more efficient impl
      return parsers.Aggregate((a, b) => a.Or(b));
    }
  }
}