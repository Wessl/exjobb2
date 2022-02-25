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
#string = pp.QuotedString(pp.alphanums)


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
print(string.parseString("heyo this is a string, I think. 123 123"))