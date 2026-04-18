// SPDX-License-Identifier: MIT

namespace Fahrenheit.Tools.Atelier;

public class AtelParseException : Exception {
    public AtelParseException()                                                       { }
    public AtelParseException(string message)                  : base(message)        { }
    public AtelParseException(string message, Exception inner) : base(message, inner) { }
}

public class AtelParser {
    private readonly AtelToken[] _tokens;
    private          int         _current;

    public AtelParser(List<AtelToken> tokens) {
        _tokens = [ .. tokens ];
    }

    public List<AtelStmt> parse() {
        List<AtelStmt> statements = [];

        while (!_at_end()) {
            AtelStmt? decl_stmt = _stmt_decl();
            if (decl_stmt != null) {
                statements.Add(decl_stmt);
            }
        }

        return statements;
    }

    /// <summary>
    ///     Checks whether the <see cref="AtelToken"/> at the parser's current position is EOF.
    /// </summary>
    private bool _at_end() {
        return _tokens[_current].Type == ATEL_TOKEN_TYPE.EOF;
    }

    /// <summary>
    ///     Return the <see cref="AtelToken"/> at <paramref name="offset"/> relative to the parser's current position.
    /// </summary>
    private AtelToken _peek(int offset = 0) {
        return _tokens[_current + offset];
    }

    /// <summary>
    ///     Consume and return an <see cref="AtelToken"/>.
    /// </summary>
    private AtelToken _advance() {
        if (!_at_end()) _current++;
        return _peek(-1);
    }

    /// <summary>
    ///     Consumes an <see cref="AtelToken"/> if it is of type <paramref name="target_type"/>.
    /// </summary>
    private bool _advance_if(ATEL_TOKEN_TYPE target_type) {
        if (_peek().Type != target_type) return false;

        _advance();
        return true;
    }

    /// <summary>
    ///     Returns the current <see cref="AtelToken"/> if it is of type
    ///     <paramref name="target_type"/>, or throws an <see cref="AtelParseException"/> if not.
    /// </summary>
    private AtelToken _advance_if_or_abort(ATEL_TOKEN_TYPE target_type, string msg) {
        AtelToken current_token = _peek();

        if (current_token.Type == target_type) return _advance();
        throw new AtelParseException($"{msg} (at line {current_token.Line})");
    }

    /* [fkelava 30/8/25 00:13]
     * I bluntly have no clue what this is for. Isn't throwing enough? Apparently not.
     * https://craftinginterpreters.com/parsing-expressions.html#synchronizing-a-recursive-descent-parser
     *
     * I'm also fairly sure this is incomplete.
     */
    private void _synchronize() {
        AtelToken previous_token = _advance();

        while (!_at_end()) {
            AtelToken current_token = _peek();
            if (previous_token.Type == ATEL_TOKEN_TYPE.SEMICOLON) return;

            switch (current_token.Type) {
                case ATEL_TOKEN_TYPE.IF:
                case ATEL_TOKEN_TYPE.RETURN:
                case ATEL_TOKEN_TYPE.WHILE:
                case ATEL_TOKEN_TYPE.FOR:
                case ATEL_TOKEN_TYPE.VAR:
                case ATEL_TOKEN_TYPE.FUN:
                    return;
            }

            _advance();
        }
    }

    private AtelStmt? _stmt_decl() {
        try {
            if (_advance_if(ATEL_TOKEN_TYPE.FUN)) return _stmt_func();
            if (_advance_if(ATEL_TOKEN_TYPE.VAR)) return _stmt_var_decl();

            return _stmt();
        }
        catch (AtelParseException ex) {
            Console.WriteLine(ex.Message);
            _synchronize();
            return null;
        }
    }

    private AtelStmt _stmt_func() {
        AtelToken name          = _advance_if_or_abort(ATEL_TOKEN_TYPE.IDENTIFIER, "Expected function name.");
        AtelToken opening_paren = _advance_if_or_abort(ATEL_TOKEN_TYPE.LEFT_PAREN, "Expected '(' after function declaration.");

        List<AtelToken> parameters = [];

        if (_peek().Type is not ATEL_TOKEN_TYPE.RIGHT_PAREN) {
            do {
                //if (parameters.Count >= 255) {
                //    throw new AtelParseException();
                //}
                parameters.Add(_advance_if_or_abort(ATEL_TOKEN_TYPE.IDENTIFIER, "Expected parameter name."));
            } while (_advance_if(ATEL_TOKEN_TYPE.COMMA));
        }

        AtelToken closing_paren      = _advance_if_or_abort(ATEL_TOKEN_TYPE.RIGHT_PAREN, "Expected ')' after parameters.");
        AtelToken body_opening_token = _advance_if_or_abort(ATEL_TOKEN_TYPE.LEFT_BRACE,  "Expected '{' before function body.");

        List<AtelStmt> body = _stmt_block();

        return new AtelStmtFunc(name, parameters, body);
    }

    private AtelStmtVar _stmt_var_decl() {
        AtelToken name_token  = _advance_if_or_abort(ATEL_TOKEN_TYPE.IDENTIFIER, "Expected variable name.");
        AtelExpr? initializer = null;

        if (_advance_if(ATEL_TOKEN_TYPE.EQ)) {
            initializer = _expr();
        }

        _advance_if_or_abort(ATEL_TOKEN_TYPE.SEMICOLON, "Expected ';' after var declaration.");
        return new AtelStmtVar(name_token, initializer);
    }

    private AtelStmt _stmt() {
        if (_advance_if(ATEL_TOKEN_TYPE.FOR)) {
            return _stmt_for();
        }

        if (_advance_if(ATEL_TOKEN_TYPE.IF)) {
            return _stmt_if();
        }

        if (_advance_if(ATEL_TOKEN_TYPE.RETURN)) {
            return _stmt_return();
        }

        if (_advance_if(ATEL_TOKEN_TYPE.WHILE)) {
            return _stmt_while();
        }

        if (_advance_if(ATEL_TOKEN_TYPE.LEFT_BRACE)) {
            return new AtelStmtBlock(_stmt_block());
        }

        return _stmt_expr();
    }

    private AtelStmt _stmt_return() {
        AtelToken keyword = _peek();
        AtelExpr? retval  = null;

        if (_peek().Type is not ATEL_TOKEN_TYPE.SEMICOLON) {
            retval = _expr();
        }

        _advance_if_or_abort(ATEL_TOKEN_TYPE.SEMICOLON, "Expected ';' after return value.");
        return new AtelStmtReturn(keyword, retval);
    }

    private AtelStmt _stmt_for() {
        _advance_if_or_abort(ATEL_TOKEN_TYPE.LEFT_PAREN, "Expected '(' after 'for'.");

        AtelStmt? initializer = null;

        AtelToken current_token = _peek();
        if (current_token.Type is ATEL_TOKEN_TYPE.SEMICOLON) {
            _advance();
        }
        else if (current_token.Type is ATEL_TOKEN_TYPE.VAR) {
            _advance();
            initializer = _stmt_var_decl();
        }
        else {
            initializer = _stmt_expr();
        }

        AtelExpr? cond = null;
        current_token = _peek();
        if (current_token.Type is not ATEL_TOKEN_TYPE.SEMICOLON) {
            cond = _expr();
        }
        _advance_if_or_abort(ATEL_TOKEN_TYPE.SEMICOLON, "Expected ';' after 'for' condition.");

        AtelExpr? increment = null;
        current_token = _peek();
        if (current_token.Type is not ATEL_TOKEN_TYPE.RIGHT_PAREN) {
            increment = _expr();
        }
        _advance_if_or_abort(ATEL_TOKEN_TYPE.RIGHT_PAREN, "Expected ')' after 'for' clause.");

        AtelStmt body = _stmt();

        if (increment != null) {
            body = new AtelStmtBlock([ body, new AtelStmtExpr(increment) ]);
        }

        cond ??= new AtelExprLiteral(true);
        body   = new AtelStmtWhile(cond, body);

        if (initializer != null) {
            body = new AtelStmtBlock([ initializer, body ]);
        }

        return body;
    }

    private AtelStmtIf _stmt_if() {
        _advance_if_or_abort(ATEL_TOKEN_TYPE.LEFT_PAREN, "Expected '(' after 'if'.");

        AtelExpr cond_expr = _expr();

        _advance_if_or_abort(ATEL_TOKEN_TYPE.RIGHT_PAREN, "Expected ')' after if condition.");

        AtelStmt  then_stmt = _stmt();
        AtelStmt? else_stmt = null;

        if (_advance_if(ATEL_TOKEN_TYPE.ELSE)) {
            else_stmt = _stmt();
        }

        return new AtelStmtIf(cond_expr, then_stmt, else_stmt);
    }

    private AtelStmtWhile _stmt_while() {
        _advance_if_or_abort(ATEL_TOKEN_TYPE.LEFT_PAREN, "Expected '(' after 'while'.");

        AtelExpr cond_expr = _expr();

        _advance_if_or_abort(ATEL_TOKEN_TYPE.RIGHT_PAREN, "Expected ')' after while condition.");

        AtelStmt body_stmt = _stmt();

        return new AtelStmtWhile(cond_expr, body_stmt);
    }

    private List<AtelStmt> _stmt_block() {
        List<AtelStmt> statements = [];

        while (_peek().Type is not ATEL_TOKEN_TYPE.RIGHT_BRACE && !_at_end()) {
            AtelStmt? decl_stmt = _stmt_decl();
            if (decl_stmt != null) {
                statements.Add(decl_stmt);
            }
        }

        _advance_if_or_abort(ATEL_TOKEN_TYPE.RIGHT_BRACE, "Expected '}' after block.");

        return statements;
    }

    private AtelStmt _stmt_expr() {
        AtelExpr  expr          = _expr();
        AtelToken current_token = _peek();

        _advance_if_or_abort(ATEL_TOKEN_TYPE.SEMICOLON, "Expected ';' after value.");

        return new AtelStmtExpr(expr);
    }

    private AtelExpr _expr() {
        return _expr_assignment();
    }

    private AtelExpr _expr_assignment() {
        AtelExpr  expr          = _expr_or();
        AtelToken current_token = _peek();

        if (current_token.Type is ATEL_TOKEN_TYPE.EQ) {
            AtelToken assigned_to_token = _advance();
            AtelExpr  value             = _expr_assignment();

            if (expr is AtelExprVar var_expr) {
                AtelToken name = var_expr.name;
                return new AtelExprAssign(name, value);
            }

            Console.WriteLine($"Invalid assignment target ({assigned_to_token.Lexeme} at {assigned_to_token.Line})");
        }

        return expr;
    }

    private AtelExpr _expr_or() {
        AtelExpr expr = _expr_and();

        while (_peek().Type is ATEL_TOKEN_TYPE.OR) {
            AtelToken op  = _advance();
            AtelExpr  rhs = _expr_and();
            expr = new AtelExprLogical(expr, op, rhs);
        }

        return expr;
    }

    private AtelExpr _expr_and() {
        AtelExpr expr = _expr_equality();

        while (_peek().Type is ATEL_TOKEN_TYPE.AND) {
            AtelToken op  = _advance();
            AtelExpr  rhs = _expr_equality();
            expr = new AtelExprLogical(expr, op, rhs);
        }

        return expr;
    }

    private AtelExpr _expr_equality() {
        AtelExpr expr = _expr_comparison();

        while (_peek().Type is ATEL_TOKEN_TYPE.EQ or ATEL_TOKEN_TYPE.EQEQ) {
            AtelToken op  = _advance();
            AtelExpr  rhs = _expr_comparison();
            expr = new AtelExprBinary(expr, op, rhs);
        }

        return expr;
    }

    private AtelExpr _expr_comparison() {
        AtelExpr expr = _expr_term();

        while (_peek().Type is ATEL_TOKEN_TYPE.GT or ATEL_TOKEN_TYPE.GTEQ or ATEL_TOKEN_TYPE.LT or ATEL_TOKEN_TYPE.LTEQ) {
            AtelToken op  = _advance();
            AtelExpr  rhs = _expr_term();
            expr = new AtelExprBinary(expr, op, rhs);
        }

        return expr;
    }

    private AtelExpr _expr_term() {
        AtelExpr expr = _expr_factor();

        while (_peek().Type is ATEL_TOKEN_TYPE.MINUS or ATEL_TOKEN_TYPE.PLUS) {
            AtelToken op  = _advance();
            AtelExpr  rhs = _expr_factor();
            expr = new AtelExprBinary(expr, op, rhs);
        }

        return expr;
    }

    private AtelExpr _expr_factor() {
        AtelExpr expr = _expr_unary();

        while (_peek().Type is ATEL_TOKEN_TYPE.SLASH or ATEL_TOKEN_TYPE.STAR or ATEL_TOKEN_TYPE.PERCENT) {
            AtelToken op  = _advance();
            AtelExpr  rhs = _expr_unary();
            expr          = new AtelExprBinary(expr, op, rhs);
        }

        return expr;
    }

    private AtelExpr _expr_unary() {
        if (_peek().Type is ATEL_TOKEN_TYPE.NOT or ATEL_TOKEN_TYPE.MINUS) {
            AtelToken op  = _advance();
            AtelExpr  rhs = _expr_unary();
            return new AtelExprUnary(op, rhs);
        }

        return _expr_call();
    }

    private AtelExpr _expr_call_args(AtelExpr callee) {
        List<AtelExpr> args = [];

        if (_peek().Type is not ATEL_TOKEN_TYPE.RIGHT_PAREN) {
            do {
                args.Add(_expr());
            } while (_advance_if(ATEL_TOKEN_TYPE.COMMA));
        }

        AtelToken closing_token = _advance_if_or_abort(ATEL_TOKEN_TYPE.RIGHT_PAREN, "Expected ')' after arguments.");

        return new AtelExprCall(callee, closing_token, args);
    }

    private AtelExpr _expr_call() {
        AtelExpr expr = _expr_primary();

        while (true) {
            if (_peek().Type is ATEL_TOKEN_TYPE.LEFT_PAREN) {
                _advance();
                expr = _expr_call_args(expr);
            }
            else {
                break;
            }
        }

        return expr;
    }

    private AtelExpr _expr_primary_parenthesized() {
        AtelExpr expr = _expr();

        _advance_if_or_abort(ATEL_TOKEN_TYPE.RIGHT_PAREN, "Expected ')' after expression.");

        return new AtelExprGrouping(expr);
    }

    private AtelExpr _expr_primary() {
        AtelToken current_token = _peek();
        AtelExpr  expr          = current_token.Type switch {
            ATEL_TOKEN_TYPE.IDENTIFIER => new AtelExprVar    (current_token),
            ATEL_TOKEN_TYPE.INTEGER    => new AtelExprLiteral(current_token.Literal),
            ATEL_TOKEN_TYPE.FLOAT      => new AtelExprLiteral(current_token.Literal),
            ATEL_TOKEN_TYPE.LEFT_PAREN => _expr_primary_parenthesized(),
            _                          => throw new AtelParseException($"Non-primary token {current_token.Type} ({current_token.Lexeme}) at {current_token.Line} fell through."),
        };

        _advance();
        return expr;
    }
}
