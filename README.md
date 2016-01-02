# Parse.Sharp
Yet another C# parser combinator library

### Character parsers

Parser expression | Valid input(s)
-------------------|-------------
`Parse.Dot` | `.`
`Parse.Char('a')` | `a`
`Parse.Char('a').IgnoreCase()` | `a`, `A`
`Parse.CharExcept('"')` | `a`, `'`, ...
-|-

`Parse.Chars("ab")` | `a`, `b`
`Parse.Chars('a', 'b')` | `a`, `b`
`Parse.Chars("ab").IgnoreCase()` | `a`, `b`, `A`, `B`
`Parse.CharsExcept("abc")` | `d`, `E`, ...
