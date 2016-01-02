using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Parse.Sharp.Parsers;

namespace Parse.Sharp
{
  // todo: Parse.SeparatedList(item, separator)

  [PublicAPI]
  public static partial class Parse
  {
    // digits and numbers:

    [NotNull] public static readonly Parser<int> Digit = new DigitParser();

    [NotNull] public static readonly Parser<int> Int32 = new Integer32Parser();

    // characters:

    

    // character set:

    

    // todo: .CharsIgnoreCase?

    // + ManyCharacters()

    // strings:

    

    

    // combinators:

    

    [NotNull, Pure, DebuggerStepThrough]
    public static Parser<T> Choice<T>([NotNull] Parser<T> left, [NotNull] Parser<T> right)
    {
      return left.Or(right);
    }

    // .And()
    // .Or()
    // .XOr()

    public static Parser Sequence<T1, T2>(Parser<T1> firstParser, Parser<T2> secondParser)
    {
      throw new NotImplementedException();
    }

    public static Parser<TResult> Sequence<T1, T2, T3, TResult>(
      Parser<T1> firstParser,
      Parser<T2> secondParser,
      Parser<T3> thirdParser,
      Func<T1, T2, T3, TResult> f)
    {
      throw new NotImplementedException();
    }

    // allocations assert:

    [NotNull, Pure, DebuggerStepThrough]
    public static IDisposable AssertNoAllocations()
    {
      Parser.AssertAvoidParserAllocations = true;
      return AllocationsAssertDisposable.Instance;
    }

    private sealed class AllocationsAssertDisposable : IDisposable
    {
      [NotNull] public static readonly IDisposable Instance = new AllocationsAssertDisposable();

      public void Dispose() { Parser.AssertAvoidParserAllocations = false; }
    }
  }

  // todo: .Ref(() => parser)
}