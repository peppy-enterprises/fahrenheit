// SPDX-License-Identifier: MIT

namespace Fahrenheit.Tools.Atelier;

public enum ATEL_TOKEN_TYPE {
    LEFT_PAREN,  // (
    RIGHT_PAREN, // )
    LEFT_BRACE,  // [
    RIGHT_BRACE, // ]
    COMMA,       // ,
    DOT,         // .
    MINUS,       // -  subtraction operator
    PLUS,        // +  addition operator
    SEMICOLON,   // ;  statement terminator
    SLASH,       // /  division operator
    STAR,        // *  multiplication operator
    PERCENT,     // %  modulo operator

    NOT,  // ! negation
    NEQ,  // != inequality
    EQ,   // = declaration
    EQEQ, // == equality
    GT,   // >
    GTEQ, // >=
    LT,   // <
    LTEQ, // <=

    // Literals. Again, not sure what primitives can be declared in ATEL.
    IDENTIFIER,
    INTEGER,
    FLOAT,

    // Unclear whether ATEL supports all these operations.
    // They can be removed later.
    AND, ELSE, FUN, FOR, IF, OR,
    RETURN, VAR, WHILE,

    EOF
}

public record AtelToken(
    ATEL_TOKEN_TYPE Type,
    string          Lexeme,
    object?         Literal,
    int             Line);

public class AtelLexer {
    private readonly string          _source;
    private readonly List<AtelToken> _tokens;

    private int _start;
    private int _current;
    private int _line;

    public AtelLexer(string source) {
        _source = source;
        _tokens = [];
    }

    /// <summary>
    ///     Return the character at <paramref name="offset"/> relative to the lexer's current position.
    /// </summary>
    private char _peek(int offset = 0) {
        return _at_end(offset) ? '\0' : _source[_current + offset];
    }

    /// <summary>
    ///     Consume and return a character.
    /// </summary>
    private char _advance() {
        return _source[_current++];
    }

    /// <summary>
    ///     Checks whether the character at <paramref name="offset"/> relative to the lexer's current position is EOF.
    /// </summary>
    private bool _at_end(int offset = 0) {
        return _current + offset >= _source.Length;
    }

    /// <summary>
    ///     Consumes all characters until <paramref name="c"/> is encountered, without consuming <paramref name="c"/>.
    /// </summary>
    private bool _advance_to(char c) {
        while (_peek() != c && !_at_end()) {
            _advance();
        }
        return true;
    }

    /// <summary>
    ///     Determines whether the character at the lexer's current position is a valid identifier character.
    /// </summary>
    private bool _is_valid_identifier() {
        char c = _peek();
        return char.IsLetterOrDigit(c) || c == '_' || c == '（' || c == '）';
    }

    /// <summary>
    ///     Determines whether the character one ahead of the lexer's current position is <paramref name="c"/>.
    ///     Used to check whether the following morphs should occur:<para/>
    ///     - <see cref="ATEL_TOKEN_TYPE.NOT"/> to <see cref="ATEL_TOKEN_TYPE.NEQ"/> <br/>
    ///     - <see cref="ATEL_TOKEN_TYPE.EQ"/> to <see cref="ATEL_TOKEN_TYPE.EQEQ"/> <br/>
    ///     - <see cref="ATEL_TOKEN_TYPE.LT"/> to <see cref="ATEL_TOKEN_TYPE.LTEQ"/> <br/>
    ///     - <see cref="ATEL_TOKEN_TYPE.GT"/> to <see cref="ATEL_TOKEN_TYPE.GTEQ"/> <br/>
    /// </summary>
    private bool _check_composite_token(char c) {
        if (_at_end() || _peek() != c) return false;

        _current++;
        return true;
    }

    /// <summary>
    ///     Determines whether a <see cref="ATEL_TOKEN_TYPE.SLASH"/> token is the start of a
    ///     single- or multi-line comment block, and bypasses that comment block if appropriate.
    /// </summary>
    private AtelToken _handle_slash_token() {
        if (_check_composite_token('/')) {
            _advance_to('\n');
            return _scan();
        }
        if (_check_composite_token('*')) {
            _advance_to('/');
            _advance(); // Skip the terminating slash.
            return _scan();
        }
        return _create_token(ATEL_TOKEN_TYPE.SLASH);
    }

    /// <summary>
    ///     Bypasses a newline character and returns the next token.
    /// </summary>
    private AtelToken _handle_newline() {
        _line++;
        return _scan();
    }

    /// <summary>
    ///     Determines whether <paramref name="identifier"/> corresponds to a reserved keyword and,
    ///     if so, emits the keyword's <see cref="ATEL_TOKEN_TYPE"/>.
    /// </summary>
    private ATEL_TOKEN_TYPE? _check_keyword(ReadOnlySpan<char> identifier) {
        return identifier switch {
            "if"     => ATEL_TOKEN_TYPE.IF,
            "else"   => ATEL_TOKEN_TYPE.ELSE,
            "while"  => ATEL_TOKEN_TYPE.WHILE,
            "return" => ATEL_TOKEN_TYPE.RETURN,
            "var"    => ATEL_TOKEN_TYPE.VAR,
            "for"    => ATEL_TOKEN_TYPE.FOR,
            "fun"    => ATEL_TOKEN_TYPE.FUN,
            _        => null,
        };
    }

    /// <summary>
    ///     Determines whether a span of characters is a valid numeric literal,
    ///     whether it is floating-point or an integer, and emits the corresponding <see cref="AtelToken"/>.
    /// </summary>
    private AtelToken? _create_numeric_literal(char c) {
        // Bail if the current character is not actually a digit.
        if (!char.IsDigit(c)) return null;

        while (char.IsDigit(_peek())) {
            _advance();
        }

        // Look for a decimal point.
        char possible_decimal_point            = _peek();
        char char_after_possible_decimal_point = _peek(1);
        bool has_decimal_point                 = possible_decimal_point == '.';

        if (has_decimal_point && char.IsDigit(char_after_possible_decimal_point)) {
            _advance(); // Consume the decimal point.
            while (char.IsDigit(_peek())) {
                _advance();
            }
        }

        string literal = _source[_start .. _current];
        return has_decimal_point
            ? new AtelToken(ATEL_TOKEN_TYPE.FLOAT,   literal, float.Parse(literal, CultureInfo.InvariantCulture), _line)
            : new AtelToken(ATEL_TOKEN_TYPE.INTEGER, literal, int  .Parse(literal, CultureInfo.InvariantCulture), _line);
    }

    /// <summary>
    ///     Determines whether a span of characters is a valid identifier,
    ///     whether it is a reserved keyword, and emits the corresponding <see cref="AtelToken"/>.
    /// </summary>
    private AtelToken? _create_identifier(char c) {
        // Bail if the opening character of the possible identifier is not a letter.
        if (!char.IsLetter(c)) return null;

        while (_is_valid_identifier()) {
            _advance();
        }

        ReadOnlySpan<char> identifier_text       = _source.AsSpan()[_start .. _current];
        ATEL_TOKEN_TYPE    identifier_token_type = _check_keyword(identifier_text) ?? ATEL_TOKEN_TYPE.IDENTIFIER;

        return _create_token(identifier_token_type);
    }

    private AtelToken _scan() {
        _start = _current;
        char c = _advance();

        return c switch {
            '('  => _create_token(ATEL_TOKEN_TYPE.LEFT_PAREN),
            ')'  => _create_token(ATEL_TOKEN_TYPE.RIGHT_PAREN),
            '{'  => _create_token(ATEL_TOKEN_TYPE.LEFT_BRACE),
            '}'  => _create_token(ATEL_TOKEN_TYPE.RIGHT_BRACE),
            ','  => _create_token(ATEL_TOKEN_TYPE.COMMA),
            '.'  => _create_token(ATEL_TOKEN_TYPE.DOT),
            '-'  => _create_token(ATEL_TOKEN_TYPE.MINUS),
            '+'  => _create_token(ATEL_TOKEN_TYPE.PLUS),
            ';'  => _create_token(ATEL_TOKEN_TYPE.SEMICOLON),
            '*'  => _create_token(ATEL_TOKEN_TYPE.STAR),
            '%'  => _create_token(ATEL_TOKEN_TYPE.PERCENT),
            '='  => _check_composite_token('=') ? _create_token(ATEL_TOKEN_TYPE.EQEQ) : _create_token(ATEL_TOKEN_TYPE.EQ),
            '!'  => _check_composite_token('=') ? _create_token(ATEL_TOKEN_TYPE.NEQ)  : _create_token(ATEL_TOKEN_TYPE.NOT),
            '<'  => _check_composite_token('=') ? _create_token(ATEL_TOKEN_TYPE.LTEQ) : _create_token(ATEL_TOKEN_TYPE.LT),
            '>'  => _check_composite_token('=') ? _create_token(ATEL_TOKEN_TYPE.GTEQ) : _create_token(ATEL_TOKEN_TYPE.GT),
            '/'  => _handle_slash_token(),
            '\t' => _scan(),
            '\r' => _scan(),
            ' '  => _scan(),
            '\n' => _handle_newline(),
            // It would be tiresome to write cases for every possible digit or alphanumeric character,
            // so we run the number and identifier handlers first.
            _    => _create_numeric_literal(c) ?? _create_identifier(c) ?? throw new NotImplementedException($"Unknown token {c} at line {_line}"),
        };
    }

    /// <summary>
    ///     Creates a <see cref="AtelToken"/> of the specified <paramref name="type"/>
    ///     with an optional <paramref name="value"/> if it is a literal.
    /// </summary>
    private AtelToken _create_token(ATEL_TOKEN_TYPE type, object? value = null) {
        string lexeme = _source[_start .. _current];
        return new AtelToken(type, lexeme, value, _line);
    }

    public List<AtelToken> get_tokens() {
        while (!_at_end()) {
            _tokens.Add(_scan());
        }

        _tokens.Add(_create_token(ATEL_TOKEN_TYPE.EOF));
        return _tokens;
    }
}
