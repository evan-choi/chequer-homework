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
    | QUATED_DOUBLE
    | (INTEGER | IDENTIFIER | '/' | '\\' | '.' | ':')+
    ;

identifier
    : IDENTIFIER
    | QUATED_DOUBLE
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

SELECT            : S E L E C T;
FROM              : F R O M;
WHERE             : W H E R E;
LIMIT             : L I M I T;
AS                : A S;
NOT               : N O T;
AND               : A N D;
OR                : O R;
NULL              : N U L L;
TRUE              : T R U E;
FALSE             : F A L S E;

// Functions
ROW               : R O W;
CURRENT_DATE      : C U R R E N T '_' D A T E;
CURRENT_TIME      : C U R R E N T '_' T I M E;
SUBSTRING         : S U B S T R I N G;
CAST              : C A S T;

// Predicates
LIKE              : L I K E;
ILIKE             : I L I K E;

// Types
TEXT              : T E X T;
NUMBER            : N U M B E R;
DATE              : D A T E;
TIME              : T I M E;
BOOLEAN           : B O O L E A N;

// Operators
PLUS              : '+';
MINUS             : '-';
ASTERISK          : '*';
SLASH             : '/';
PERCENT           : '%';
CONCAT            : '||';
EQ                : '=';
NEQ               : '<>' | '!=';
LT                : '<';
LTE               : '<=';
GT                : '>';
GTE               : '>=';

// == Literals ==

QUATED_DOUBLE
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

fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP];
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];

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
