# Parse.Sharp
Yet another C# parser combinator library

| Parser expression | Valid input(s) |
|-------------------|-------------|
| `Parse.Dot` | `.` |
| `Parse.Char('a')` | `a` |
| `Parse.Chars('a').IgnoreCase()` | `a`, `A`
