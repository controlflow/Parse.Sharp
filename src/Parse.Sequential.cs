using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Combinators;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> SurroundWith<T>([NotNull] this Parser<T> parser, [NotNull] Parser headAndTailParser)
    {
      if (parser == null) throw new ArgumentNullException("parser");
      if (headAndTailParser == null) throw new ArgumentNullException("headAndTailParser");

      return new SurroundParser<T>(headAndTailParser, parser, headAndTailParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> SurroundWith<T>(
      [NotNull] this Parser<T> parser, [NotNull] Parser headParser, [NotNull] Parser tailParser)
    {
      if (parser == null) throw new ArgumentNullException("parser");
      if (headParser == null) throw new ArgumentNullException("headParser");
      if (tailParser == null) throw new ArgumentNullException("tailParser");

      return new SurroundParser<T>(headParser, parser, tailParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Token<T>([NotNull] this Parser<T> parser)
    {
      if (parser == null) throw new ArgumentNullException("parser");

      // todo: whitespace.many()
      var whitespace = WhitespaceChar.Many(0, uint.MaxValue);
      return new SurroundParser<T>(whitespace, parser, whitespace);
    }




    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> WithTail<T>([NotNull] this Parser<T> parser, [NotNull] Parser tailParser)
    {
      return new AfterParserTest<T>(parser, tailParser);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> InFrontOf<T>([NotNull] this Parser<T> parser, [NotNull] Parser headParser)
    {
      return new BeforeParserTest<T>(headParser, parser);
    }

    

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> WhitespaceAfter<T>([NotNull] this Parser<T> parser)
    {
      // todo: whitespace.many()
      return new AfterParserTest<T>(parser, WhitespaceChar);
    }

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> WhitespaceBefore<T>([NotNull] this Parser<T> parser)
    {
      // todo: whitespace.many()
      return new AfterParserTest<T>(parser, WhitespaceChar);
    }
  }
}