# Parse.Sharp
Yet another C# parser combinator library

### Character parsers

Parser expression | Valid input(s)
------------------|---------------
`Parse.AnyChar` | `a`, `_`, ...
`Parse.Dot` | `.`
`Parse.Char('a')` | `a`
`Parse.Char('a').IgnoreCase()` | `a`, `A`
`Parse.CharExcept('"')` | `a`, `'`, ...
`Parse.Char(char.IsDigit)` | `1`, `7`, ...
`Parse.Char(c => c >= '1' && c <= '0')` | `1`, `2`, ... `0`
`Parse.LowerCaseChar` | `a`, `b`, ...

------------------|---------------
`Parse.Chars("ab")` | `a`, `b`
`Parse.Chars('a', 'b')` | `a`, `b`
`Parse.Chars("ab").IgnoreCase()` | `a`, `b`, `A`, `B`
`Parse.CharsExcept("abc")` | `d`, `E`, ...

### String parsers

Parser expression | Valid input(s)
------------------|---------------
`Parse.String("if")` | `if`
`Parse.IgnoreCaseString("if")` | `if`, `If`, `iF`, `IF`

### Combinators

Parser expression | Purpose
------------------|---------------
`parser.Select(x => ...)` | Applies arbitrary transformation to `parser` output