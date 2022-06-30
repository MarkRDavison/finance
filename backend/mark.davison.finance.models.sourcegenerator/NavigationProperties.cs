using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace mark.davison.finance.models.sourcegenerator
{
    [Generator]
    public class NavigationProperties : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;

            var generator = new NavigationPropertyDocumentGenerator("mark.davison.finance.models.Entities");

            var entityNames = new List<string> { "User" };
            foreach (var syntaxTree in syntaxTrees)
            {
                var nodes = syntaxTree
                    .GetRoot()
                    .DescendantNodesAndSelf();
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
#pragma warning disable RS1024
                var workpaperHashSet = nodes
                    .OfType<ClassDeclarationSyntax>()
                    .Select(_ => semanticModel.GetDeclaredSymbol(_))
                    .OfType<ITypeSymbol>()
                    .Where(_ => _.BaseType.Name == "FinanceEntity")
                    .ToImmutableHashSet();
#pragma warning restore RS1024
                entityNames.AddRange(workpaperHashSet.Select(x => x.Name).ToList());
            }
            foreach (var syntaxTree in syntaxTrees)
            {
                var nodes = syntaxTree
                    .GetRoot()
                    .DescendantNodesAndSelf();
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

#pragma warning disable RS1024
                var workpaperHashSet = nodes
                    .OfType<ClassDeclarationSyntax>()
                    .Select(_ => semanticModel.GetDeclaredSymbol(_))
                    .OfType<ITypeSymbol>()
                    .Where(_ => _.BaseType.Name == "FinanceEntity")
                    .ToImmutableHashSet();
#pragma warning restore RS1024

                foreach (var typeSymbol in workpaperHashSet)
                {
                    context.AddSource($"{typeSymbol.Name}.g.cs", generator.GenerateNavigationProperties(typeSymbol, entityNames));
                    //context.AddSource($"{typeSymbol.Name}EntityConfiguration.g.cs", generator.GenerateEntityConfiguration(typeSymbol, entityNames));
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {

        }
    }

}