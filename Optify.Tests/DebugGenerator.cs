using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Optify.Tests;

public class DebugGenerator
{
    [Test]
    public Task Should_allow_the_debugger_in_the_generator()
    {
        var generator = new OptifyGenerator();
        var compilation = CSharpCompilation.Create("CSharpCodeGen.GenerateAssembly")
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(DebugGeneratorSource.Source))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        driver.GetRunResult();
        return Task.CompletedTask;
    }
}

file static class DebugGeneratorSource
{
    // This will need to be expanded for every type to generate for while debugging.
    public const string Source =
        """
        using Optify;
        namespace CSharpCodeGen;
        
        [Optify] public class Config;
        
        internal partial class Program
        {
            private static void Main()
            {
            }
        }
        """;
}