// SPDX-License-Identifier: MIT

namespace Fahrenheit.Tools.Atelier;

/* [fkelava 29/8/25 18:01]
 * I do not see the particular use of the visitor pattern
 * but I'm following the book verbatim. This can be replaced later.
 */

// The interpreter used to use this before I dumped it. The compiler may do the same.

internal interface IAtelStmtVisitor {
    internal void handle_stmt_expr  (AtelStmtExpr   stmt);
    internal void handle_stmt_func  (AtelStmtFunc   stmt);
    internal void handle_stmt_var   (AtelStmtVar    stmt);
    internal void handle_stmt_block (AtelStmtBlock  stmt);
    internal void handle_stmt_if    (AtelStmtIf     stmt);
    internal void handle_stmt_while (AtelStmtWhile  stmt);
    internal void handle_stmt_return(AtelStmtReturn stmt);
}

public abstract record AtelStmt() {
    internal abstract void accept(IAtelStmtVisitor visitor);
}

public sealed record AtelStmtExpr(AtelExpr expr) : AtelStmt {
    internal override void accept(IAtelStmtVisitor visitor) {
        visitor.handle_stmt_expr(this);
    }
}

public sealed record AtelStmtFunc(AtelToken name, List<AtelToken> param_list, List<AtelStmt> body) : AtelStmt {
    internal override void accept(IAtelStmtVisitor visitor) {
        visitor.handle_stmt_func(this);
    }
}

public sealed record AtelStmtVar(AtelToken name, AtelExpr? initializer) : AtelStmt {
    internal override void accept(IAtelStmtVisitor visitor) {
        visitor.handle_stmt_var(this);
    }
}

public sealed record AtelStmtReturn(AtelToken keyword, AtelExpr? value) : AtelStmt {
    internal override void accept(IAtelStmtVisitor visitor) {
        visitor.handle_stmt_return(this);
    }
}

public sealed record AtelStmtBlock(List<AtelStmt> statements) : AtelStmt {
    internal override void accept(IAtelStmtVisitor visitor) {
        visitor.handle_stmt_block(this);
    }
}

public sealed record AtelStmtIf(AtelExpr cond, AtelStmt then_branch, AtelStmt? else_branch) : AtelStmt {
    internal override void accept(IAtelStmtVisitor visitor) {
        visitor.handle_stmt_if(this);
    }
}

public sealed record AtelStmtWhile(AtelExpr cond, AtelStmt body) : AtelStmt {
    internal override void accept(IAtelStmtVisitor visitor) {
        visitor.handle_stmt_while(this);
    }
}

internal interface IAtelExprVisitor<T> {
    internal T handle_expr_binary  (AtelExprBinary   expr);
    internal T handle_expr_grouping(AtelExprGrouping expr);
    internal T handle_expr_literal (AtelExprLiteral  expr);
    internal T handle_expr_unary   (AtelExprUnary    expr);
    internal T handle_expr_var     (AtelExprVar      expr);
    internal T handle_expr_assign  (AtelExprAssign   expr);
    internal T handle_expr_logical (AtelExprLogical  expr);
    internal T handle_expr_call    (AtelExprCall     expr);
}

public abstract record AtelExpr() {
    internal abstract T accept<T>(IAtelExprVisitor<T> visitor);
}

public sealed record AtelExprBinary(AtelExpr lhs, AtelToken op, AtelExpr rhs) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_binary(this);
    }
}

public sealed record AtelExprCall(AtelExpr callee, AtelToken paren, List<AtelExpr> args) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_call(this);
    }
}

public sealed record AtelExprGrouping(AtelExpr expr) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_grouping(this);
    }
}

public sealed record AtelExprLiteral(object? value) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_literal(this);
    }
}

public sealed record AtelExprUnary(AtelToken op, AtelExpr rhs) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_unary(this);
    }
}

public sealed record AtelExprVar(AtelToken name) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_var(this);
    }
}

public sealed record AtelExprAssign(AtelToken name, AtelExpr value) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_assign(this);
    }
}

public sealed record AtelExprLogical(AtelExpr lhs, AtelToken op, AtelExpr rhs) : AtelExpr {
    internal override T accept<T>(IAtelExprVisitor<T> visitor) {
        return visitor.handle_expr_logical(this);
    }
}
