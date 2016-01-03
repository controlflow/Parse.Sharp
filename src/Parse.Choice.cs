using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Combinators;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Or<T>([NotNull] this Parser<T> leftParser, [NotNull] Parser<T> rightParser)
    {
      if (leftParser == null) throw new ArgumentNullException("leftParser");
      if (rightParser == null) throw new ArgumentNullException("rightParser");

      var orParsers = rightParser.FlattenOrParsers(leftParser.FlattenOrParsers());
      if (orParsers != null) return new ManyChoicesParser<T>(orParsers);

      return Choice(leftParser, rightParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Choice<T>([NotNull] Parser<T> leftParser, [NotNull] Parser<T> rightParser)
    {
      if (leftParser == null) throw new ArgumentNullException("leftParser");
      if (rightParser == null) throw new ArgumentNullException("rightParser");

      var orParsers = rightParser.FlattenOrParsers(leftParser.FlattenOrParsers());
      if (orParsers != null) return new ManyChoicesParser<T>(orParsers);

      return new ChoiceParser<T>(leftParser, rightParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Choice<T>([NotNull] params Parser<T>[] parsers)
    {
      if (parsers == null) throw new ArgumentNullException("parsers");
      if (parsers.Length == 0) throw new ArgumentOutOfRangeException("parsers", "Length is 0");

      List<Parser<T>> orParsers = null;

      foreach (var parser in parsers)
        orParsers = parser.FlattenOrParsers(orParsers);

      if (orParsers != null)
        return new ManyChoicesParser<T>(orParsers);

      return new ManyChoicesParser<T>(parsers);
    }

    [CanBeNull, Pure]
    private static List<Parser<T>> FlattenOrParsers<T>(
      [NotNull] this Parser<T> parser, [CanBeNull] List<Parser<T>> accumulator = null)
    {
      var choiceParser = parser as ChoiceParser<T>;
      if (choiceParser != null)
      {
        accumulator = accumulator ?? new List<Parser<T>>(2);
        accumulator.Add(choiceParser.LeftParser);
        accumulator.Add(choiceParser.RightParser);
      }

      var manyChoicesParser = parser as ManyChoicesParser<T>;
      if (manyChoicesParser != null)
      {
        var parsers = manyChoicesParser.Parsers;
        accumulator = accumulator ?? new List<Parser<T>>(parsers.Length);
        accumulator.AddRange(parsers);
      }

      return accumulator;
    }
  }
}