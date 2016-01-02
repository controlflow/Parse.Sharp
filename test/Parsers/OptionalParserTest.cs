using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture] public sealed class OptionalParserTest : ParserTest
  {
    [Test] public void Optional()
    {
      var maybeDot = Parse.Dot.OptionalToNullable();

      AssertParse(maybeDot, "", expectedValue: null);
      AssertParse(maybeDot, ".", expectedValue: '.');

      var parser = Parse.AnyCharOf("ab").OptionalToNullable()
        .WithTail(Parse.AnyChar.OptionalToNullable())
        .IgnoreCase();

      AssertParse(parser, "a", expectedValue: 'a');
      AssertParse(parser, "A", expectedValue: 'A');
      AssertParse(parser, "b", expectedValue: 'b');
      AssertParse(parser, "c", expectedValue: null);
    }

    [Test] public void OptionalOrDefault()
    {
      var parser = Parse.String("abc").Optional();

      AssertParse(parser, "abc", "abc");
      AssertParse(parser, "", null);
    }

    [Test] public void OptionalOrDefaultValue()
    {
      var obj = new object();
      var parser = Parse.AnyChar.Cast<object>().Optional(obj);

      AssertParse(parser, "a", 'a');
      AssertParse(parser, "", obj);
    }
  }
}