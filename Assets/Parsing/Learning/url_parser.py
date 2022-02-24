from tokenize import group
from pyparsing import *

url_chars = alphanums + '-_.~%+'
fragment = Combine((Suppress('#') + Word(url_chars)))('fragment')
scheme = oneOf('http https ftp file')('scheme')
host = Combine(delimitedList(Word(url_chars), '.'))('host')
port = Suppress(':') + Word(nums)('port')
user_info = (
    Word(url_chars)('username')
    + Suppress(':')
    + Word(url_chars)('password')
    + Suppress('@')
)

query_pair = Group(Word(url_chars) + Suppress('=') + Word(url_chars))
query = Group(Suppress('?') + delimitedList(query_pair, '&'))('query')

path = Combine(
    Suppress('/')
    + OneOrMore(~query + Word(url_chars + '/'))
)('path')

url_parser = (
    scheme
    + Suppress('://')
    + Optional(user_info)
    + host
    + Optional(port)
    + Optional(path)
    + Optional(query)
    + Optional(fragment)
)