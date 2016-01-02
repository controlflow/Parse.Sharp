using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using Parse.Sharp.Parsers.Combinators;

namespace Parse.Sharp
{
  public static partial class Parse
  {
    [Pure, NotNull, DebuggerStepThrough]
    public static Parser<TAccumulate> Aggregate<T, TAccumulate>(
      [NotNull] this Parser<T> parser, TAccumulate seed,
      [NotNull] Func<TAccumulate, T, TAccumulate> fold)
    {
      return new AggregateParser<T, TAccumulate, TAccumulate>(
        parser, () => seed, fold, AggregateHelper<TAccumulate>.Id);
    }

    [Pure, NotNull, DebuggerStepThrough]
    public static Parser<TAccumulate> Aggregate<T, TAccumulate>(
      [NotNull] this Parser<T> parser,
      [NotNull] Func<TAccumulate> seedFactory,
      [NotNull] Func<TAccumulate, T, TAccumulate> fold)
    {
      return new AggregateParser<T, TAccumulate, TAccumulate>(
        parser, seedFactory, fold, AggregateHelper<TAccumulate>.Id);
    }

    [Pure, NotNull, DebuggerStepThrough]
    public static Parser<TResult> Aggregate<T, TAccumulate, TResult>(
      [NotNull] this Parser<T> parser,
      [NotNull] Func<TAccumulate> seedFactory,
      [NotNull] Func<TAccumulate, T, TAccumulate> fold,
      [NotNull] Func<TAccumulate, TResult> resultSelector)
    {
      return new AggregateParser<T, TAccumulate, TResult>(parser, seedFactory, fold, resultSelector);
    }

    [Pure, NotNull, DebuggerStepThrough]
    public static Parser<string> AggregateToString([NotNull] this Parser<char> parser)
    {
      return new AggregateParser<char, StringBuilder, string>(parser,
        seedFactory: () => new StringBuilder(),
        fold: (sb, value) => sb.Append(value),
        resultSelector: sb => sb.ToString());
    }

    private sealed class AggregateHelper<T>
    {
      [NotNull] public static readonly Func<T, T> Id = x => x;
    }
  }
}