from unittest import result
import pyparsing as pp
import inspect
import os
#import UnityEngine
from itertools import groupby


pp.enable_all_warnings()
pp.ParserElement.enable_packrat()

#
# Parse Actions (Clean up parse results by hooking into parse logic return values)
# 
def commandParseAction(string, location, tokens):
    tokens.append('\n')   
    return tokens.asList()  # Not returning it as 'asList()' gives you literally parseResults(...) instead of pure list
def listParseAction(string, location, tokens):
    tokens.insert(0, "list")
    return tokens.asList()

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

comment = pp.Regex(r"#.*").setName("Comment").suppress() # don't include comments
float_ = pp.pyparsing_common.real
number = pp.Combine(pp.Opt(pp.oneOf("− -")) + (pp.pyparsing_common.number ^ float_)).setName("Number") # - is different to − ...
identifier = pp.Word(pp.alphas+"_", pp.alphanums+"_").setName("Identifier")

plusorminus = pp.Literal('+') ^ pp.Literal('-')
integer = pp.Combine(pp.Optional(plusorminus) + number)
digit = pp.Char(pp.nums)
boolean = pp.Literal("0") ^ pp.Literal('1')
string = pp.QuotedString(quoteChar="\"").setName("String")
lparen = pp.Literal("(").suppress().setName("left parenthesis")
rparen = pp.Literal(")").suppress().setName("right parenthesis")
lbracket = pp.Literal("[").suppress().setName("left bracket")
rbracket = pp.Literal("]").suppress().setName("right bracket")
semicolon = pp.Literal(";").setName("Semicolon")

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
argList = pp.Opt(pp.delimitedList(expression)).setName("ArgList")
funcCall = (identifier + pp.Group(lparen + argList + rparen))


arithExpr = pp.Forward().setName("Arith Expression")
arithOperand = pp.Group(lparen + arithExpr + rparen) ^ funcCall ^ identifier ^ number ^ string
multiplyExpr = arithOperand + pp.Optional((mult ^ div) + arithOperand)
arithExpr <<= (multiplyExpr + pp.Optional((plus ^ minus) + multiplyExpr))
list_ = pp.Group(lparen + expression + pp.ZeroOrMore(pp.Literal(",").suppress() + expression) + rparen)
#list_.setDebug().setName("List")


cmpOp = (eq ^ ne ^ ge ^ le ^ gt ^ lt).setName("Comparison Operator")
cmpExpr = arithExpr + pp.Optional(cmpOp + arithExpr)

setExpr = pp.Group(list_ + contains_ + identifier) ^ pp.Group(identifier + in_ + (list_ | funcCall))
boolOperand = pp.Group(lparen + setExpr + rparen) ^ cmpExpr ^ setExpr
notExpr = pp.Group(not_ + boolOperand) ^ boolOperand
andExpr = notExpr + pp.Optional(and_ + notExpr)
boolExpr = andExpr + pp.Optional(or_ + andExpr).setName("Boolean Expression")

expression <<= (number ^ string ^ funcCall ^ identifier ^ boolExpr ^ list_)

# selection expressions

topoSelector = funcCall
attrSelector = lbracket + boolExpr + rbracket
groupSelector = lbracket + pp.Literal("::") + funcCall + rbracket
selectorSeq = pp.Optional(topoSelector) + pp.ZeroOrMore(attrSelector ^ groupSelector)

# variable and function
assignment = identifier + pp.Literal("=") + expression + semicolon.suppress()

selectionExpression = pp.Group(pp.Literal("<") + pp.Optional(selectorSeq + pp.ZeroOrMore(pp.Keyword("/") + selectorSeq)) + pp.Literal(">"))

# 
# EXECUTION MODEL
#

actions = pp.OneOrMore(funcCall + semicolon)
rule = pp.Literal("{") + selectionExpression + pp.Keyword("−>") + actions + pp.Literal("}")
exit_ = pp.Keyword("exit;")
command = comment ^ rule ^ assignment ^ exit_
program = pp.ZeroOrMore(command)

#
# Set special parse actions
#
command.setParseAction(commandParseAction)
list_.setParseAction(listParseAction)


#
#   Test & Debug
#
def enableDebug(enabled, testEnabled):
    if (enabled):
        cmpOp
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
        assignment.setName("Assignment").setDebug()
        selectionExpression.setName("Selection Expression").setDebug()
        #lparen.setDebug()
        #rparen.setDebug()
        topoSelector.setName("Topology Selector").setDebug()
        attrSelector.setName("Attribute Selector").setDebug()
        groupSelector.setName("Group Selector").setDebug()
        selectorSeq.setName("Selector Sequence").setDebug()
        string.setDebug()
        rule.setName("Rule")
        actions.setName("Actions")
    if (testEnabled):
        # assignment
        print(assignment.parseString("facW = 17.6;"))
        #C4: add a door touching the ground;
        print(command.parseString("""{ <descendant() [ label == "facade" ] / [ label == "main" ] / \
        cell() [ colLabel == "mid_col" ][ rowLabel == "gnd_floor" ] > \
        −> addShape("door", toGlobalX(0.5), toGlobalY(0.45), 1.76, 2.64, \
        constrain(dist2region( dist2bottom(0.0, 0.0) ))); }"""))

        # function call with list
        print(funcCall.parseString("""rows( lineElem((3.5, 3.0, 4.0), (1, 1), label("gnd_floor")))"""))

        # nested functions & lists
        print(command.parseString("""{ <descendant() [ label == "facade" ] > −>   \
        createGrid("main",                                                   \
        rows( lineElem((3.5, 3.0, 4.0), (1, 1), label("gnd_floor")),         \
        lineElem((3.0, 3.0, 3.5), (1, 10), label("top_floor"))               \
        ),                                                                   \
        cols( lineElem((2.8, 2.5, 3.0), (1, 10), label("left_col")),         \
        lineElem((3.6, 3.0, 4.0), (1, 1), label("mid_col")),                 \
        lineElem((2.8, 2.5, 3.0), (2, 2), label("right_col"))               \
        )); }"""))       
        # Idk
        print(command.parseString(""" { <descendant() [ label == "facade" ] / [ label == "main" ] /   \
        cell() [ colLabel == "left_col" ][ rowIdx == rowLast(−1) ] >                                 \
        −> addShape("win3", toGlobalX(0.5), toGlobalY(0.5), 1.44, 1.28); }"""))
        # Idk 2
        print(command.parseString(""" { <descendant() [ label == "facade" ] / [ label == "main" ] /
        cell() [ colLabel == "mid_col" ][ rowLabel == "gnd_floor" ] > −>
        addShape("door", toGlobalX(0.5), toGlobalY(0.45), 1.76, 2.64,
        constrain(dist2region( dist2bottom(0.0, 0.0) ))); } """))
enableDebug(False,False)


#src_file_path = inspect.getfile(lambda: None)
# This really needs to be relative to the Unity project somehow
fileSource = r"C:\Users\love\Documents\GitHub\exjobb2\Assets\Parsing\selex_file.txt"
fileResult = r"C:\Users\love\Documents\Github\exjobb2\Assets\Parsing\ParseResult.txt"
res = program.parse_file(fileSource, parseAll=True)

res1 = [list(group) for k, group in groupby(res, lambda x: x == '\n') if not k]

# Write to file
#with open("", "w") as text_file:
#    text_file.write(resultString)
with open(fileResult, 'w', encoding="utf-8") as f:
    for r in res1:
        print(r, file=f)
print("Completed printing parse results to file ParseResult.txt")