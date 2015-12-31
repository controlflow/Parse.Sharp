using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public sealed class TextParserTest : ParserTest
  {
    [Test] public void Foo()
    {
      AssertParse(
        parser: Parse.Text("class"),
        input: "class",
        expectedValue: "class");

      AssertParse(
        parser: from kw in Parse.Text("class")
                from lb in Parse.Char('{')
                from rb in Parse.Char('}')
                select true,
        input: "class{}");
    }
  }
}