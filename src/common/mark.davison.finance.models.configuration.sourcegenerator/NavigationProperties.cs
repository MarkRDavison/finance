using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mark.davison.finance.models.configuration.sourcegenerator
{
    [Generator]
    public class NavigationProperties : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;

            var generator = new NavigationPropertyDocumentGenerator("mark.davison.finance.models.Entities");
            var assemblySymbol = context.Compilation.SourceModule.ReferencedAssemblySymbols.FirstOrDefault(q => q.Name == "mark.davison.finance.models");
            if (assemblySymbol == null)
            {
                return;
            }


            HashSet<string> ignore = new HashSet<string>() { "AccountSummary" };

            foreach (var t in assemblySymbol.TypeNames)
            {
                if (t == null || t.Contains('<') || t.Contains('>') || t.Contains("Attribute") || ignore.Contains(t))
                {
                    //throw new InvalidOperationException($"{t} is a bad type name");
                    continue;
                }

                var type = assemblySymbol.GetTypeByMetadataName($"mark.davison.finance.models.Entities.{t}");
                if (type == null)
                {
                    context.AddSource($"{t}EntityConfiguration.g.cs", "");
                    throw new InvalidOperationException($"mark.davison.finance.models.Entities.{t} is a good type name:  {type.Kind}");
                }
                context.AddSource($"{t}EntityConfiguration.g.cs", "");
                throw new InvalidOperationException($"mark.davison.finance.models.Entities.{t} is a bad type name");
            }


            //context.AddSource($"UserEntityConfiguration.g.cs", "");
            //

            //            var entityNames = new List<string> { "User" };
            //            foreach (var syntaxTree in syntaxTrees)
            //            {
            //                var nodes = syntaxTree
            //                    .GetRoot()
            //                    .DescendantNodesAndSelf();
            //                var semanticModel = compilation.GetSemanticModel(syntaxTree);
            //#pragma warning disable RS1024
            //                var workpaperHashSet = nodes
            //                    .OfType<ClassDeclarationSyntax>()
            //                    .Select(_ => semanticModel.GetDeclaredSymbol(_))
            //                    .OfType<ITypeSymbol>()
            //                    .Where(_ => _.BaseType.Name == "FinanceEntity")
            //                    .ToImmutableHashSet();
            //#pragma warning restore RS1024
            //                entityNames.AddRange(workpaperHashSet.Select(x => x.Name).ToList());
            //            }
            //            foreach (var syntaxTree in syntaxTrees)
            //            {
            //                var nodes = syntaxTree
            //                    .GetRoot()
            //                    .DescendantNodesAndSelf();
            //                var semanticModel = compilation.GetSemanticModel(syntaxTree);

            //#pragma warning disable RS1024
            //                var workpaperHashSet = nodes
            //                    .OfType<ClassDeclarationSyntax>()
            //                    .Select(_ => semanticModel.GetDeclaredSymbol(_))
            //                    .OfType<ITypeSymbol>()
            //                    .Where(_ => _.BaseType.Name == "FinanceEntity")
            //                    .ToImmutableHashSet();
            //#pragma warning restore RS1024

            //                foreach (var typeSymbol in workpaperHashSet)
            //                {
            //                    //context.AddSource($"{typeSymbol.Name}.g.cs", generator.GenerateNavigationProperties(typeSymbol, entityNames));
            //                    context.AddSource($"{typeSymbol.Name}EntityConfiguration.g.cs", generator.GenerateEntityConfiguration(typeSymbol, entityNames));
            //                }
            //            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {

        }
    }

}