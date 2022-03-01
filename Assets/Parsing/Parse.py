import pyparsing as pp


pp.enable_all_warnings()
pp.ParserElement.enable_packrat()

keywords = ("child", "descedant", "parent", "root", "self", "neighbor",\
"label", "type", "rowIdx", "colIdx", "rowLabel", "colLabel",\
"last", "rowLast", "colLast", "groupRows", "groupCols",\
"groupRegions", "if", "randomSelect", "eval")
child, descendant, parent, root, self_, neightbour, label, \
type_, rowIdx, colIdx, rowLabel, colLabel, last, rowLast,   \
colLast, groupRows, groupCols, groupRegions, if_, randomSelect, eval_ = map(pp.Keyword, keywords)
keywords2 = (child | descendant | parent | root | self_ | neightbour | label | \
type_ | rowIdx | colIdx | rowLabel | colLabel | last | rowLast |   \
colLast | groupRows | groupCols | groupRegions | if_ | randomSelect | eval_)

comment = pp.Regex(r"#.*")#.suppress() # don't include comments
float_ = pp.pyparsing_common.real
number = (pp.pyparsing_common.number ^ float_).setName("Number")
identifier = pp.Word(pp.alphas+"_"+"'", pp.alphanums+"_"+"'").setName("Identifier")

plusorminus = pp.Literal('+') ^ pp.Literal('-')
integer = pp.Combine(pp.Optional(plusorminus) + number)
digit = pp.Char(pp.nums)
boolean = pp.Literal("0") ^ pp.Literal('1')
string = pp.QuotedString(quoteChar="'")
lparen = pp.Literal("(").suppress().setName("left parenthesis")
rparen = pp.Literal(")").suppress().setName("right parenthesis")

# operators
operators = ('+','-','*','/',\
    '!','&&','||','>','<','==','>=','<=','!=')
plus, minus, mult, div, not_, and_, or_, gt, lt, eq,\
    ge, le, ne = map(pp.Keyword, operators)
eq.setName("Equals")
ne.setName("Not Equals")
in_ = pp.Keyword("in")
contains_ = pp.Keyword("contains")


# expression
expression = pp.Forward()

# function and argument list
argList = pp.Opt(pp.delimitedList(expression))

funcCall = (identifier + pp.Group(lparen + argList + rparen))

arithExpr = pp.Forward().setName("Arith Expression")
arithOperand = pp.Group(lparen + arithExpr + rparen) | identifier | number
multiplyExpr = arithOperand + pp.Optional((mult ^ div) + arithOperand)
arithExpr <<= (multiplyExpr + pp.Optional((plus ^ minus) + multiplyExpr))
list_ = pp.Literal("(") + expression + pp.ZeroOrMore(pp.Literal(",") + expression) + ")"


cmpOp = (eq ^ ne ^ ge ^ le ^ gt ^ lt)
cmpExpr = arithExpr + pp.Optional(cmpOp + arithExpr)

setExpr = pp.Group(list_ + contains_ + identifier) | pp.Group(identifier + in_ + list_)
boolOperand = pp.Group(lparen + setExpr + rparen) | cmpExpr | setExpr
notExpr = pp.Group(not_ + boolOperand) | boolOperand
andExpr = notExpr + pp.Optional(and_ + notExpr)
boolExpr = andExpr + pp.Optional(or_ + andExpr).setName("Boolean Expression")

expression <<= (number | funcCall | identifier | boolExpr)

# selection expressions

topoSelector = funcCall
attrSelector = pp.Literal("[") + boolExpr + pp.Literal("]")
groupSelector = pp.Literal("[") + pp.Keyword("::") + funcCall + pp.Literal("]")
selectorSeq = pp.Optional(topoSelector) + pp.ZeroOrMore(attrSelector ^ groupSelector)

# variable and function
assignment = identifier + pp.Literal("=") + expression

selectionExpression = pp.Group(pp.Literal("<") + pp.Optional(selectorSeq + pp.ZeroOrMore(pp.Keyword("/") + selectorSeq)) + pp.Literal(">"))

# 
# EXECUTION MODEL
#

actions = pp.OneOrMore(funcCall + pp.Literal(";"))
rule = pp.Literal("{") + selectionExpression + pp.Keyword("−>") + actions + pp.Literal("}")
exit_ = pp.Keyword("exit;")
command = rule ^ assignment ^ exit_
program = pp.ZeroOrMore(command)

##print(program.parse_file("selex_file.txt", parseAll=True))


def enableDebug(enabled):
    if (enabled):
        cmpOp.setName("Comparison Operator")
        number.setDebug()
        identifier.setDebug()
        funcCall.setDebug().setName("Function Call")
        boolExpr.setDebug().setName("Boolean Expression")
        arithExpr.setName("Arithmetic expression").setDebug()
        arithOperand.setName("Arithmetic Operand").setDebug()
        multiplyExpr.setName("Multiply Expression").setDebug()
        notExpr.setName("Not Expression").setDebug()
        andExpr.setName("And Expression").setDebug()
        cmpExpr.setName("Compare Expression").setDebug()
        boolOperand.setName("Bool Operand").setDebug()
        expression.setName("Expression").setDebug()
        setExpr.setName("Set Expression").setDebug()
        argList.setName("argument list").setDebug()
        assignment.setName("Assignment").setDebug()
        lparen.setDebug()
        rparen.setDebug()
        topoSelector.setDebug()
        attrSelector.setDebug()
        groupSelector.setDebug()
        selectorSeq.setDebug()
enableDebug(True)
# Keywords

###
# literals

# test area
#print(comment.parseString("# this is a comment"))
#print(identifier.parseString("'a123_john'"))
#print(integer.parseString("-99"))
#print(string.parseString("'heyo this is a string, I think. 123 123'"))
#print(plus.parseString('+'))
#print(cmpOp.parseString(">"))
#print(assignment.parseString("a=5"))
#print(comment.parseString("#C1: setup the facade size"))
#funcCall.parseString("descendant()").pprint()
#print(identifier.parseString("123"))
#print(funcCall.parseString("descendant(123)"))
#print(number.parseString("17.6"))
#print(number.parseString("15"))
#print(assignment.parseString("facW = 17.6;"))
#print(assignment.parseString("facH = 12.8;"))

#C4: add a door touching the ground;
print(command.parseString("{ <descendant() [ label == 'facade' ] / [ label == 'main' ] / \
cell() [ colLabel == 'mid_col' ][ rowLabel == 'gnd_floor' ] > \
−> addShape('door', toGlobalX(0.5), toGlobalY(0.45), 1.76, 2.64, \
constrain(dist2region( dist2bottom(0.0, 0.0) ))); }"))

#print(command.parseString("{ <> −> addShape('facade', 0, 0, facW, facH); }"))
#print(pp.alphanums)
#print(command.parseString("{ <descendant() [ label == 'facade' ] > −> coverShape(); }"))