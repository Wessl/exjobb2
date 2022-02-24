from pyparsing import *

dash = '-'
ssn_parser = Combine(
 Word(nums, exact=3) + dash + Word(nums, exact=2) + dash + Word(nums, exact=4)
)

input_string = """
    xxx 225-92-8416 yyy
    103-33-3929 zzz 028-91-0122
"""
for match, start, stop in ssn_parser.scanString(input_string):
    print(match, start, stop)