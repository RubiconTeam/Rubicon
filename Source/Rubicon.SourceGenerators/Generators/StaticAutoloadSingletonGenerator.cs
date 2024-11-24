using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using IFieldSymbol = Microsoft.CodeAnalysis.IFieldSymbol;
using IPropertySymbol = Microsoft.CodeAnalysis.IPropertySymbol;

namespace Rubicon.SourceGenerators;

[Generator]
public class StaticAutoloadSingletonGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
            
    }

    public void Execute(GeneratorExecutionContext context)
    {
        INamedTypeSymbol[] godotClassesWithAttr = context
            .Compilation.SyntaxTrees
            .SelectMany(tree =>
                tree.GetRoot().DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .SelectClassesOfType("GodotSharp", GenerationConstants.NodeClass, context.Compilation)
                    .Where(x => x.symbol.GetAttributes().Any(a => a.AttributeClass?.IsStaticAutoloadAttribute() ?? false))
                    .Select(x => x.symbol))
            .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
            .ToArray();

        foreach (INamedTypeSymbol autoLoadClass in godotClassesWithAttr)
            MakeStaticAutoloadClass(context, autoLoadClass);
    }

    private static void MakeStaticAutoloadClass(GeneratorExecutionContext context, INamedTypeSymbol symbol)
    {
        AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.IsStaticAutoloadAttribute() ?? false);
        if (attribute == null)
            return;

        string nameSpace = attribute.ConstructorArguments[0].Value?.ToString()!;
        string className = attribute.ConstructorArguments[1].Value?.ToString()!;

        List<string> allUsings = [ symbol.ContainingNamespace.FullQualifiedNameOmitGlobal(), "Godot" ];
        StringBuilder finalClass = new();
        
        if (!string.IsNullOrEmpty(nameSpace))
            finalClass.Append($"namespace {nameSpace};\n\n");

        // Make singleton field here
        finalClass.Append($"/// <summary>\n/// A static class that allows for accessing the <see cref=\"{symbol.Name}\"/> singleton's functions easier.\n/// </summary>\n" +
                          $"public static partial class {className}\n" +
                          "{\n" +
                          $"\t/// <summary>\n\t/// The current global instance of <see cref=\"{symbol.Name}\"/>.\n\t/// </summary>\n" +
                          $"\tpublic static {symbol.Name} Singleton => Engine.GetMainLoop() is SceneTree tree ? tree.Root.GetNode<{symbol.Name}>(\"{className}\") : null;\n\n");

        ImmutableArray<ISymbol> members = symbol.GetMembers();

        IPropertySymbol[] properties = members
            .Where(s => s.Kind == SymbolKind.Property)
            .Cast<IPropertySymbol>()
            .ToArray();
        
        IPropertySymbol[] publicProperties = properties
            .Where(s => s.DeclaredAccessibility == Accessibility.Public && !s.IsStatic)
            .ToArray();

        IFieldSymbol[] publicFields = members
            .Where(s => s.Kind == SymbolKind.Field && !s.IsImplicitlyDeclared)
            .Cast<IFieldSymbol>()
            .Where(s => s.DeclaredAccessibility == Accessibility.Public && !s.IsStatic)
            .ToArray();

        IMethodSymbol[] publicMethods = members
            .Where(s => s.Kind == SymbolKind.Method)
            .Cast<IMethodSymbol>()
            .Where(s => s.DeclaredAccessibility == Accessibility.Public && !s.IsStatic)
            .ToArray();

        INamedTypeSymbol[] signals = members
            .Where(s => s.Kind == SymbolKind.NamedType &&
                        s.GetAttributes().Any(a => a.AttributeClass?.IsGodotSignalAttribute() ?? false))
            .Cast<INamedTypeSymbol>()
            .ToArray();
        
        foreach (IPropertySymbol property in publicProperties)
        {
            string propertyNameSpace = property.Type.GetNamespaceName();
            if (!string.IsNullOrEmpty(propertyNameSpace) && propertyNameSpace != nameSpace && !allUsings.Contains(propertyNameSpace))
                allUsings.Add(propertyNameSpace);

            // Make documentation comment
            finalClass.Append($"\t/// <inheritdoc cref=\"{symbol.Name}.{property.Name}\"/>\n");

            if (property.IsReadOnly)
            {
                finalClass.Append($"\tpublic static {property.Type.ToDisplayString()} {property.Name} => Singleton.{property.Name};\n\n");
                continue;
            }

            if (property.IsWriteOnly)
            {
                finalClass.Append($"\tpublic static {property.Type.ToDisplayString()} {property.Name}\n" +
                                  "\t{\n" +
                                  $"\t\tset => Singleton.{property.Name} = value;\n" +
                                  "\t}\n\n");
                continue;
            }

            finalClass.Append($"\tpublic static {property.Type.ToDisplayString()} {property.Name}\n" +
                              "\t{\n");
            
            if (property.Type.TypeKind != TypeKind.Delegate)
            {
                finalClass.Append($"\t\tget => Singleton.{property.Name};\n" +
                                  $"\t\tset => Singleton.{property.Name} = value;\n");
            }
            else
            {
                finalClass.Append($"\t\tadd => Singleton.{property.Name} += value;\n" +
                                  $"\t\tremove => Singleton.{property.Name} -= value;\n");
            }
                              
            finalClass.Append("\t}\n\n");
        }

        foreach (IFieldSymbol field in publicFields)
        {
            string fieldNameSpace = field.Type.GetNamespaceName();
            if (!string.IsNullOrEmpty(fieldNameSpace) && fieldNameSpace != nameSpace && !allUsings.Contains(fieldNameSpace))
                allUsings.Add(fieldNameSpace);   
            
            // Make documentation comment
            finalClass.Append($"\t/// <inheritdoc cref=\"{symbol.Name}.{field.Name}\"/>\n" +
                              $"\tpublic static {field.Type.ToDisplayString()} {field.Name}\n" +
                              "\t{\n");
            
            if (field.Type.TypeKind != TypeKind.Delegate)
            {
                finalClass.Append($"\t\tget => Singleton.{field.Name};\n" +
                                  $"\t\tset => Singleton.{field.Name} = value;\n");
            }
            else
            {
                finalClass.Append($"\t\tadd => Singleton.{field.Name} += value;\n" +
                                  $"\t\tremove => Singleton.{field.Name} -= value;\n");
            }
                              
            finalClass.Append("\t}\n\n");
        }

        foreach (INamedTypeSymbol signal in signals)
        {
            string signalName = signal.Name.Remove(signal.Name.IndexOf("EventHandler", StringComparison.Ordinal));
            finalClass.Append($"\t/// <inheritdoc cref=\"{symbol.Name}.{signalName}\"/>\n" +
                              $"\tpublic static event {signal.ToDisplayString()} {signalName}\n" +
                              "\t{\n" +
                              $"\t\tadd => Singleton.{signalName} += value;\n" +
                              $"\t\tremove => Singleton.{signalName} -= value;\n" +
                              "\t}\n\n");
        }

        foreach (IMethodSymbol method in publicMethods)
        {
            if (method.Name == ".ctor" || ((method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) && properties.Any(x => x.Name == method.Name.Substring(4))))
                continue;

            string methodNameSpace = method.ReturnType.GetNamespaceName();
            if (!string.IsNullOrEmpty(methodNameSpace) && methodNameSpace != nameSpace && !allUsings.Contains(methodNameSpace))
                allUsings.Add(methodNameSpace);   
            
            // Make documentation comment
            finalClass.Append($"\t/// <inheritdoc cref=\"{symbol.Name}.{method.Name}\"/>\n" +
                              $"\tpublic static {method.ReturnType.ToDisplayString()} {method.Name}(");

            IParameterSymbol[] parameters = method.Parameters.ToArray();
            Array.Sort(parameters, (a, b) =>
            {
                if (a.Ordinal < b.Ordinal)
                    return -1;
                if (a.Ordinal > b.Ordinal)
                    return 1;

                return 0;
            });
            
            for (int i = 0; i < parameters.Length; i++)
            {
                IParameterSymbol parameter = parameters[i];

                string paramNameSpace = parameter.Type.GetNamespaceName();
                if (!string.IsNullOrEmpty(paramNameSpace) && paramNameSpace != nameSpace && !allUsings.Contains(paramNameSpace))
                    allUsings.Add(paramNameSpace);   
                
                finalClass.Append($"{parameter.Type.ToDisplayString()} {parameter.Name}");

                if (parameter.HasExplicitDefaultValue)
                    finalClass.Append($" = {parameter.ExplicitDefaultValue ?? "null"}");
                
                if (i < parameters.Length - 1)
                    finalClass.Append(", ");
            }

            finalClass.Append($") => Singleton.{method.Name}(");
            
            for (int i = 0; i < parameters.Length; i++)
            {
                IParameterSymbol parameter = parameters[i];
                finalClass.Append($"{parameter.Name}");

                if (i < parameters.Length - 1)
                    finalClass.Append(", ");
            }

            finalClass.Append(");\n\n");
        }

        finalClass.Remove(finalClass.Length - 1, 1);
        finalClass.Append("}");

        
        StringBuilder usingsText = new();
        foreach (string usingDirective in allUsings)
            usingsText.Append($"using {usingDirective};\n");
        usingsText.Append("\n");

        //throw new Exception((usingsText.ToString() + finalClass.ToString()).Replace("\n", "").Replace("\t", ""));

        context.AddSource($"{className}.RubiconGenerated.cs", usingsText + finalClass.ToString());
    }
}