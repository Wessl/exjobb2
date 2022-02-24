from typing import Literal
import pyparsing as pp
from pp import (
    Literal,
    Word,
    Group,
    Forward,
    alphas,
    alphanums,
    Regex,
    ParseException,
    CaselessKeyword,
    Suppress,
    delimitedList,
)
import math
import operator

comment = "#" (char) *
char = ?any ASCII character?
letter = ?alphabetic characters a−z and A−Z?

identifier = letter (letter | digit | "_") *
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"

# Keywords
keywords = {
    k: CaselessKeyword(k)
    for k in """\
    child descendant parent root self neighbor label type rowIdx colIdx rowLabel 
    colLabel last rowLast colLast groupRows groupCols groupRegions if randomSelect eval
    """.split()
}

# literals
float = ["−" | "+"] digit+ "." digit+
integer = Word(digit)
number = float | integer

Boolean = "0" | "1"

string = "\"" (char) * "\""



# Operators
"+" "−" " * " "/" "in" "contains" "!" "&&" "||"
">" "<" "==" ">=" "<=" "!="
plus, minus, mult, div = map(Literal, "+-*/")
in = Literal("in")
contains = Literal("contains")
# etc

# Delimiters
"{" "}" "[" "]" "(" ")" "<" ">"
"=" "," ":" ";" "−>"

# Whitespace
whitespace = "\n" | " " | "\t"

# list
list = "(" expression ("," expression)* ")"

# selection expressions
selectionExpression = "<" [selectorSeq ("/" selectorSeq) * ] ">"
selectorSeq = [topoSelector] [attrSelector | groupSelector] *
topoSelector = funcCall
attrSelector = "[" boolExpr "]"
groupSelector = "[" "::" funcCall "]"

for greeting_str in [
    "123",
    "abracadabra"
    ]:
    greeting = greet.parse_string(greeting_str)