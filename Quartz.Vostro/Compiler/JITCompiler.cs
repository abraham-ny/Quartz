using Origin.Vostro.Parser;

namespace Origin.Vostro.Compiler
{
    public sealed class JITCompiler
    {
        private readonly JavaScriptParser _parser = new();
        private readonly BytecodeGenerator _generator = new();
        private readonly Optimizer _optimizer = new();

        public CompiledScript Compile(string source, string sourceUrl)
        {
            // Parse to AST
            var ast = _parser.Parse(source, sourceUrl);
            
            // Optimize AST
            ast = _optimizer.Optimize(ast);
            
            // Generate bytecode
            var bytecode = _generator.Generate(ast);
            
            return new CompiledScript(bytecode, sourceUrl);
        }
    }

    public class CompiledScript
    {
        public byte[] Bytecode { get; }
        public string SourceUrl { get; }

        public CompiledScript(byte[] bytecode, string sourceUrl)
        {
            Bytecode = bytecode;
            SourceUrl = sourceUrl;
        }
    }
}
