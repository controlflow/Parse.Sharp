using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public sealed class JsonParserTest : ParserTest
  {
    [Test] public void M()
    {
      //"{\"aaaaa\":\"booo\",\"xx\"  : [\"aaa\", \"123\"] }";

      //var stringLiteral =
      //  from lquote in Parse.Char('"')
      //  from content in Parse.AnyChar.Expect(Parse.Char('"'))
      //  from rquote in Parse.Char('"')
      //  select content;
    }
  }
}