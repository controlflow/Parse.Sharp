using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  [TestFixture]
  public sealed class JsonParserTest : ParserTest
  {
    private static readonly Parser<char> DoubleQuote = Parse.Char('"');

    private static readonly Parser<string> StringLiteral =
      from lQuote   in DoubleQuote
      from contents in DoubleQuote.Not().ManyToString()
      from rQuote   in DoubleQuote
      select contents;

    private static readonly Parser<ObjLiteral> ObjectLiteral =
      from lBrace in Parse.Char('{').WithWhitespaceAfter()
      from properties in (
           from propertyName in StringLiteral
           from colon in Parse.Char(':').Token()
           from propertyValue in PropertyValue
           select new KeyValuePair<string, object>(propertyName, propertyValue)
         )
         // + ws
         // todo: .Many()/.CommaSeparated()
         .Select(x => new[] { x }.AsEnumerable())
         //.Select(xs => xs.ToDictionary(x => x.Key, x => x.Value))
         // .Many()
      from rBrace in Parse.Char('}')
      select new ObjLiteral(properties);

    private static readonly Parser<object[]> ArrayLiteral =
      from lBrace in Parse.Char('[').WithWhitespaceAfter()
      from items in (
        from value in PropertyValue // +ws
        select value
        )
      from rBrace in Parse.Char(']')
      select new object[0];

    private static readonly Parser<object> PropertyValue =
      StringLiteral.Cast<object>()
      .Or(ObjectLiteral.Cast<object>())
      .Or(ArrayLiteral.Cast<object>())
      // todo: numbers
      ;

    class ObjLiteral : List<KeyValuePair<string, object>>
    {
      public ObjLiteral() { }
      public ObjLiteral([NotNull] IEnumerable<KeyValuePair<string, object>> collection)
        : base(collection) { }
    }


    [Test] public void M()
    {
      AssertParse(StringLiteral, "\"abc\"", "abc");



      //"{\"aaaaa\":\"booo\",\"xx\"  : [\"aaa\", \"123\"] }";

      

      //var stringLiteral =
      //  from lquote in Parse.Char('"')
      //  from content in Parse.AnyChar.Expect(Parse.Char('"'))
      //  from rquote in Parse.Char('"')
      //  select content;
    }
  }
}