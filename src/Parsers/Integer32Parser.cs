namespace Parse.Sharp.Parsers
{
  internal sealed class Integer32Parser : Parser<int>, Parser.IFailPoint
  {
    // todo: handle int32 overflow

    protected internal override ParseResult TryParse(string input, int offset, bool isConditional)
    {
      var value = default(int);
      var parsed = false;

      for (; offset < input.Length; offset++)
      {
        var ch = input[offset];
        if (ch < '0' || ch > '9') break;

        var digit = ch - '0';
        value = value * 10 + digit;
        parsed = true;
      }

      if (parsed)
      {
        return new ParseResult(value: value, nextOffset: offset);
      }

      return new ParseResult(failPoint: this, atOffset: offset);
    }

    public string GetExpectedMessage()
    {
      return "integer";
    }
  }
}