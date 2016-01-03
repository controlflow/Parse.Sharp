using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Parse.Sharp.Tests.Parsers
{
  // todo: identifiers, null
  // todo: comma-separated list combinator without tail comma

  [TestFixture]
  public sealed class JsonParserTest : ParserTest
  {
    private static readonly Parser<char> DoubleQuote = Parse.Char('"');

    private static readonly Parser<string> StringLiteral =
      Parse.CharExcept('"').ManyToString().SurroundWith(DoubleQuote);

    private static readonly Parser<char> ColonToken = Parse.Char(':').Token();

    private static readonly Parser<KeyValuePair<string, object>> ObjectProperty =
      from propertyName  in StringLiteral
      from colon         in ColonToken
      from propertyValue in JsonValue
      select new KeyValuePair<string, object>(propertyName, propertyValue);

    private static readonly Parser<JsonObject> ObjectLiteral =
      ObjectProperty // todo: comma-separated list
        .WithTail(Parse.Char(',').Token().Or(Parse.Whitespace.Select(' ') /* todo: solve this */))
        .Many().Select(properties => new JsonObject(properties))
        .SurroundWith(
          headParser: Parse.Char('{').WithWhitespaceAfter(),
          tailParser: Parse.Char('}'));

    private static readonly Parser<object> NullLiteral =
      Parse.String("null").Select<object>(value: null);

    private static readonly Parser<object[]> ArrayLiteral =
      Parse.Ref(() => JsonValue)
        .WithTail(Parse.Char(',').Token().Or(Parse.Whitespace.Select(' ') /* todo: solve this */))
        .Many().Select(values => values.ToArray())
        .SurroundWith(
          headParser: Parse.Char('[').WithWhitespaceAfter(),
          tailParser: Parse.Char(']'));

    private static readonly Parser<object> JsonValue =
      StringLiteral.Cast<object>()
      .Or(ObjectLiteral.Cast<object>())
      .Or(ArrayLiteral.Cast<object>())
      .Or(NullLiteral);

    private sealed class JsonObject : List<KeyValuePair<string, object>>
    {
      public JsonObject() { }
      public JsonObject([NotNull] IEnumerable<KeyValuePair<string, object>> collection)
        : base(collection) { }

      public void Add([NotNull] string key, object value)
      {
        Add(new KeyValuePair<string, object>(key, value));
      }
    }

    [Test] public void ParseJson()
    {
      AssertParse(StringLiteral, "\"abc\"", "abc");
      AssertParse(ColonToken, "  :  ", ':');
      AssertParse(NullLiteral, "null", null);
      AssertParse(ObjectProperty.Select(x => x.Key + x.Value), "\"abc\":null", "abc");
      AssertParse(ObjectLiteral, "{  }", new JsonObject());
      AssertParse(ObjectLiteral,
        equalityComparer: JsonEqualityComparer.Instance,
        input: "{\"aa\":null}",
        expectedValue: new JsonObject { { "aa", null } });
      AssertParse(ObjectLiteral,
        equalityComparer: JsonEqualityComparer.Instance,
        input: "{ \"foo\" : \"bar\" }",
        expectedValue: new JsonObject { { "foo", "bar" } });
      AssertParse(ObjectLiteral,
        equalityComparer: JsonEqualityComparer.Instance,
        input: "{\"foo\" :\"bar\", \"def\": \"\"}",
        expectedValue: new JsonObject { { "foo", "bar" }, { "def", "" } });

      AssertParse(ArrayLiteral, "[]", new object[0]);
      AssertParse(ArrayLiteral,
        equalityComparer: JsonEqualityComparer.Instance,
        input: "[\"foo\", null, [ ],  \"def\", {}]",
        expectedValue: new object[] { "foo", null, new object[0], "def", new JsonObject() });

      AssertParse(JsonValue,
        equalityComparer: JsonEqualityComparer.Instance,
        input: "{\"a\":{\"b\":{\"abc\":[{\"def\":null}]}}}",
        expectedValue: new JsonObject {
          {"a", new JsonObject {
            {"b", new JsonObject {
              {"abc", new object[] {
                new JsonObject {{"def", null}}
              }}
            }}
          }}
        });
    }

    private sealed class JsonEqualityComparer : IEqualityComparer<object>
    {
      [NotNull] public static readonly IEqualityComparer<object> Instance = new JsonEqualityComparer();
      private JsonEqualityComparer() { }

      public new bool Equals(object x, object y)
      {
        var xLiteral = x as JsonObject;
        var yLiteral = y as JsonObject;
        if (xLiteral != null && yLiteral != null)
        {
          if (xLiteral.Count != yLiteral.Count) return false;

          for (var index = 0; index < xLiteral.Count; index++)
          {
            var xProperty = xLiteral[index];
            var yProperty = yLiteral[index];
            if (xProperty.Key != yProperty.Key) return false;
            if (!Instance.Equals(xProperty.Value, yProperty.Value)) return false;
          }

          return true;
        }

        var xArray = x as object[];
        var yArray = y as object[];
        if (xArray != null && yArray != null)
        {
          if (xArray.Length != yArray.Length) return false;

          for (var index = 0; index < xArray.Length; index++)
          {
            if (!Instance.Equals(xArray[index], yArray[index])) return false;
          }

          return true;
        }

        return object.Equals(x, y);
      }

      public int GetHashCode(object obj)
      {
        throw new InvalidOperationException();
      }
    }
  }
}