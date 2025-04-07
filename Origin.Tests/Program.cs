using System;
using Xunit;
using Xunit.Runner.InProc.SystemConsole;

[assembly: TestFramework("Xunit.Runner.InProc.SystemConsole.TestFramework", "Xunit.Runner.InProc.SystemConsole")]

namespace Origin.Tests
{
    class Program
    {
        static int Main(string[] args)
        {
            return new ConsoleRunner().Run(args);
        }
    }
}
