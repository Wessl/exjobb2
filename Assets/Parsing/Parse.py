import pyparsing as pp

comment = pp.Regex(r"#.*")#.suppress() # don't include comments
char = pp.Char(pp.pyparsing_unicode.printables)
letter = pp.Char(pp.alphas)
number = pp.pyparsing_common.number
identifier = pp.Word(pp.alphanums + "_")

plusorminus = pp.Literal('+') | pp.Literal('-')
integer = pp.Combine(pp.Optional(plusorminus) + number)
digit = pp.Char(pp.nums)
boolean = pp.Literal("0") | pp.Literal('1')
string = pp.QuotedString(quoteChar="'")

# operators
operators = ('+','-','*','/','in','contains',\
    '!','&&','||','>','<','==','>=','<=','!')
plus, minus, mult, div, in_, contains_, not_, and_, or_, gt, lt, eq,\
    ge, le, ne = map(pp.Literal, operators)

delimiters = pp.oneOf("{ } [ ] ( ) < > = , : ; −>")
whitespace = pp.oneOf(["\n", " " "\t"])


# expression
expression = pp.Forward()
funcCall = pp.Forward()
list_ = pp.Forward()

arithExpr = pp.Forward()
multiplyExpr = pp.Forward()
arithOperand = expression | pp.Group(pp.Literal("(") + arithExpr + pp.Literal(")"))
arithExpr << multiplyExpr + pp.Optional( pp.Group(plus | minus) + multiplyExpr)
multiplyExpr << arithOperand + pp.Optional( pp.Group(mult | div) + arithOperand)

cmpOp = eq | ne | ge | le | gt | lt
cmpExpr = arithExpr + pp.Optional(cmpOp + arithExpr)

notExpr = (not_ + boolean) | boolean
andExpr = notExpr + pp.Optional(and_ + notExpr)
boolExpr = andExpr + pp.Optional(and_ + andExpr)
setExpr = pp.Group(expression + in_ + list_) | pp.Group(list_ + contains_ + expression)
boolOperand = setExpr | cmpExpr | pp.Group("(" + setExpr + ")")
expression <<= identifier | funcCall | boolExpr

list_ << pp.Literal("(") + expression + pp.OneOrMore(pp.Literal(",") + expression) + ")"

# selection expressions

topoSelector = funcCall
attrSelector = pp.Literal("[") + boolExpr + pp.Literal("]")
groupSelector = pp.Literal("[") + pp.Literal("::") + funcCall + pp.Literal("]")
selectorSeq = pp.Optional(topoSelector) + pp.Optional(attrSelector | groupSelector)

# variable and function
assignment = identifier + eq + expression
argList = pp.Optional(expression + pp.Group(pp.ZeroOrMore(pp.Literal(",") + expression)))
funcCall << identifier + pp.Literal("(") + argList + pp.Literal(")")

selectionExpression = pp.Literal("<") + pp.Optional(selectorSeq)

# 
# EXECUTION MODEL
#

actions = pp.OneOrMore(funcCall + pp.Literal(";"))
rule = pp.Literal("{") + selectionExpression + pp.Literal("->") + actions + pp.Literal("}")
exit_ = pp.Literal("exit;")
command = rule | assignment | exit_
program = pp.ZeroOrMore(command)



# Keywords
#keywords = {
#    k: pp.CaselessKeyword(k)
#    for k in """\
#    child descendant parent root self neighbor label type rowIdx colIdx rowLabel 
#    colLabel last rowLast colLast groupRows groupCols groupRegions if randomSelect eval
#    """.split()
#}
###
# literals

# test area
print(comment.parseString("# this is a comment"))
print(letter.parseString("hey"))
print(identifier.parseString("a123_john"))
print(integer.parseString("-99"))
print(string.parseString("'heyo this is a string, I think. 123 123'"))
print(plus.parseString('+'))
print(delimiters.parseString("};.,"))