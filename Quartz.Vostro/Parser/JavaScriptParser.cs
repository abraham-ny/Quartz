using System;
using Quartz.Vostro.Parser.AST;

namespace Quartz.Vostro.Parser
{
    public sealed class JavaScriptParser
    {
        private readonly Lexer _lexer = new();
        private readonly SyntaxAnalyzer _syntaxAnalyzer = new();

        public ProgramNode Parse(string source, string sourceUrl)
        {
            // 1. Lexical analysis (tokenization)
            var tokens = _lexer.Tokenize(source, sourceUrl);
            
            // 2. Syntax analysis (parsing)
            var ast = _syntaxAnalyzer.Parse(tokens);
            
            // 3. Return complete AST
            return ast;
        }
    }

    internal class Lexer
    {
        public Token[] Tokenize(string source, string sourceUrl)
        {
            // TODO: Implement lexical analysis
            throw new NotImplementedException();
        }
    }

    internal class SyntaxAnalyzer
    {
        public ProgramNode Parse(Token[] tokens)
        {
            // TODO: Implement syntax analysis
            throw new NotImplementedException();
        }
    }
}
