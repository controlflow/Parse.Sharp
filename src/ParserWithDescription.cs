using JetBrains.Annotations;

namespace Parse.Sharp
{
  internal abstract class ParserWithDescription<T> : Parser<T>, Parser.IFailPoint
  {
    [NotNull] protected readonly string Description;

    protected ParserWithDescription([NotNull] string description)
    {
      Description = description;
    }

    public string GetExpectedMessage()
    {
      return Description;
    }
  }
}