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

      return Choice(leftParser, rightParser);
    }

    [NotNull, Pure]
    public static Parser<T> Choice<T>([NotNull] Parser<T> leftParser, [NotNull] Parser<T> rightParser)
    {
      if (leftParser == null) throw new ArgumentNullException("leftParser");
      if (rightParser == null) throw new ArgumentNullException("rightParser");

      List<Parser<T>> list = null;

      if (FlattenOrParsers(leftParser, ref list))
      {
        if (!FlattenOrParsers(rightParser, ref list))
          list.Add(rightParser);

        return new ManyChoicesParser<T>(list);
      }

      if (FlattenOrParsers(rightParser, ref list))
      {
        if (!FlattenOrParsers(leftParser, ref list))
          list.Insert(index: 0, item: leftParser);

        return new ManyChoicesParser<T>(list);
      }

      return new ChoiceParser<T>(leftParser, rightParser);
    }

    [NotNull, Pure]
    public static Parser<T> Choice<T>([NotNull] params Parser<T>[] parsers)
    {
      if (parsers == null) throw new ArgumentNullException("parsers");
      if (parsers.Length == 0) throw new ArgumentOutOfRangeException("parsers", "Length is 0");

      var list = new List<Parser<T>>();

      foreach (var parser in parsers)
      {
        if (!FlattenOrParsers(parser, ref list)) list.Add(parser);
      }

      if (list.Count == 2)
        return new ChoiceParser<T>(list[0], list[1]);

      return new ManyChoicesParser<T>(list);
    }

    [ContractAnnotation("=> true, accumulator: notnull; => false")]
    private static bool FlattenOrParsers<T>([NotNull] Parser<T> parser, ref List<Parser<T>> accumulator)
    {
      var choiceParser = parser as ChoiceParser<T>;
      if (choiceParser != null)
      {
        accumulator = accumulator ?? new List<Parser<T>>(2);
        accumulator.Add(choiceParser.LeftParser);
        accumulator.Add(choiceParser.RightParser);
        return true;
      }

      var manyChoicesParser = parser as ManyChoicesParser<T>;
      if (manyChoicesParser != null)
      {
        var parsers = manyChoicesParser.Parsers;
        accumulator = accumulator ?? new List<Parser<T>>(parsers.Length);
        accumulator.AddRange(parsers);
        return true;
      }

      return false;
    }
  }
}