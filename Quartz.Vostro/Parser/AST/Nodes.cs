using System.Collections.Generic;

namespace Origin.Vostro.Parser.AST
{
    public abstract class Node
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Source { get; set; }
    }

    public class ProgramNode : Node
    {
        public List<Statement> Body { get; } = new();
    }

    public abstract class Statement : Node { }
    public abstract class Expression : Node { }

    public class VariableDeclaration : Statement
    {
        public List<VariableDeclarator> Declarations { get; } = new();
        public string Kind { get; set; } // "var", "let", "const"
    }

    public class VariableDeclarator : Node
    {
        public Identifier Id { get; set; }
        public Expression Init { get; set; }
    }

    public class Identifier : Expression
    {
        public string Name { get; set; }
    }

    public class Literal : Expression
    {
        public object Value { get; set; }
    }

    public class FunctionDeclaration : Statement
    {
        public Identifier Id { get; set; }
        public List<Identifier> Parameters { get; } = new();
        public BlockStatement Body { get; set; }
    }

    public class BlockStatement : Statement
    {
        public List<Statement> Body { get; } = new();
    }

    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; set; }
    }

    // Additional node types would be added here:
    // - Control flow (if, for, while, etc.)
    // - Operators (binary, unary, etc.)
    // - Member expressions
    // - Call expressions
    // - And many more...
}
