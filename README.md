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
`Parse.AnyCharOf("ab")` | `a`, `b`
`Parse.AnyCharOf('a', 'b')` | `a`, `b`
`Parse.AnyCharOf("ab").IgnoreCase()` | `a`, `b`, `A`, `B`
`Parse.AnyCharExcept("abc")` | `d`, `E`, ...

### String parsers

Parser expression | Valid input(s)
------------------|---------------
`Parse.String("if")` | `if`
`Parse.IgnoreCaseString("if")` | `if`, `If`, `iF`, `IF`

### Combinators

Parser expression | Purpose
------------------|---------------
`parser.Select(x => f(x))` | Applies arbitrary transformation to `parser`'s output
`parser.Not()` | Succeeds when `parser` fails to parse input, do not advances parsing offset
`parser.SurroundWith(p1, p2)` | Applies `p1`, `parser`, `p2` parsers sequentially and returns the result of `parser`
`parser.Return(t)` | Always succeeds and produces value `t` as output
`from x in p1 from y in p2 select f(x, y)` | Runs `p1` and `p2` parsers sequentially transforming the result