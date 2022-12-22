grammar CqlBase;

root
    : statement EOF
    ;

statement
    : query #selectStatement
    /* TOOD: | INSERT INTO <file-name> queryOrInlineValues #insertStatement */
    ;

query
    : term  = queryTerm
      limit = limitClause?
    ;

queryTerm
    : querySpecification  #default
    | '(' query ')'       #parenthesizedQuery
    ;

querySpecification
    : SELECT items+=selectItem (',' items+=selectItem)*
      (FROM from=dataSource)?
      (WHERE where=booleanExpression)?
    ;

selectItem
    : expression (AS alias=identifier)? #selectExpression
    | identifier '.' '*'                #selectAll
    | '*'                               #selectAll
    ;

dataSource
    : dataSourcePrimary (AS alias=identifier)?
    ;

dataSourcePrimary
    : fileName                  #fileDataSource
    | '(' query ')'             #subquery
    | '(' dataSourcePrimary ')' #parenthesizedDataSource
    ;

limitClause
    : LIMIT (
        offset=INTEGER ',' limit=INTEGER
        | limit=INTEGER
    )
    ;

fileName
    : INTEGER
    | IDENTIFIER
    | QUOTED_DOUBLE
    | (INTEGER | IDENTIFIER | '/' | '\\' | '.' | ':')+
    ;

identifier
    : IDENTIFIER
    | QUOTED_DOUBLE
    ;

qualifiedName
    : identifier ('.' identifier)?
    ;

expression
    : valueExpression
    | booleanExpression
    ;

valueExpression
    : primaryExpression                                         #primary
    | op=('+' | '-') valueExpression                            #arithmeticUnary
    | l=valueExpression op=('*' | '/' | '%') r=valueExpression  #arithmeticBinary
    | l=valueExpression op=('+' | '-') r=valueExpression        #arithmeticBinary
    | l=valueExpression CONCAT r=valueExpression                #concatenation
    ;

primaryExpression
    : NULL               #nullLiteral
    | INTEGER            #numberLiteral
    | STRING             #stringLiteral
    | booleanValue       #booleanLiteral
    | qualifiedName      #columnReference
    | function           #functionCall
    | '(' expression ')' #parenthesizedExpression
    ;

booleanExpression
    : l=valueExpression op=(LIKE | ILIKE) r=valueExpression                    #predicateLike
    | l=valueExpression op=(EQ | NEQ | LT | LTE | GT | GTE) r=valueExpression  #predicateComparison
    | NOT booleanExpression                                                    #logicalNot
    | l=booleanExpression op=(AND | OR) r=booleanExpression                    #logicalBinary
    ;

booleanValue
    : TRUE
    | FALSE
    ;

function
    : ROW                             #row
    | CURRENT_DATE                    #currentDate
    | CURRENT_TIME                    #currentTime
    | SUBSTRING '(' 
        src=valueExpression 
        ',' offset=valueExpression
        (',' length=valueExpression)?
      ')'                             #substring
    | CAST '(' expression AS type ')' #cast
    ;

type
    : TEXT
    | NUMBER
    | DATE
    | TIME
    | BOOLEAN
    ;

// == Remove Comment ==

SINGLELINE_COMMENT
    : '-- ' ~[\r\n]* '\r'? '\n'? -> channel(HIDDEN)
    ;

MULTILINE_COMMENT
    : '/*' .*? '*/' -> channel(HIDDEN)
    ;

// == Keywords ==

SELECT       : 'SELECT';
FROM         : 'FROM';
WHERE        : 'WHERE';
LIMIT        : 'LIMIT';
AS           : 'AS';
NOT          : 'NOT';
AND          : 'AND';
OR           : 'OR';
NULL         : 'NULL';
TRUE         : 'TRUE';
FALSE        : 'FALSE';

// Functions
ROW          : 'ROW';
CURRENT_DATE : 'CURRENT_DATE';
CURRENT_TIME : 'CURRENT_TIME';
SUBSTRING    : 'SUBSTRING';
CAST         : 'CAST';

// Predicates
LIKE         : 'LIKE';
ILIKE        : 'ILIKE';

// Types
TEXT         : 'TEXT';
NUMBER       : 'NUMBER';
DATE         : 'DATE';
TIME         : 'TIME';
BOOLEAN      : 'BOOLEAN';

// Operators
PLUS         : '+';
MINUS        : '-';
ASTERISK     : '*';
SLASH        : '/';
PERCENT      : '%';
CONCAT       : '||';
EQ           : '=';
NEQ          : '<>' | '!=';
LT           : '<';
LTE          : '<=';
GT           : '>';
GTE          : '>=';

// == Literals ==

QUOTED_DOUBLE
    : '"' ( ~'"' | '""' )* '"'
    ;

STRING
    : '\'' ( ~'\'' | '\'\'' )* '\''
    ;

INTEGER
    : DIGIT+
    ;

IDENTIFIER
    : LETTER+ (LETTER | DIGIT)*
    ;

// == Fragments ==

fragment DIGIT: [0-9];
fragment LETTER: [a-zA-Z\u0080-\uFFFF_];

// == Delimiter ==

DM
    : ';' -> type(EOF)
    ;

// == Remove Whitespace ==

WS
    : [ \r\n\t]+ -> channel(HIDDEN)
    ;

// == Unrecognized character ==

UNKNOWN
    : .
    ;
