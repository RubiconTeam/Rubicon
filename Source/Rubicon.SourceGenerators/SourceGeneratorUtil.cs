using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rubicon.SourceGenerators;

public static class SourceGeneratorUtil
{
    public static string GetNamespaceName(this ITypeSymbol symbol)
    {
        if (symbol.IsNamespace)
            return symbol.Name;

        if (symbol.ContainingNamespace != null)
            return symbol.ContainingNamespace.ToDisplayString();

        return "";
    }
    
    /// <summary>
    /// Whether the symbol inherits from the type provided.
    /// </summary>
    /// <param name="symbol">The current symbol</param>
    /// <param name="assemblyName">The assembly name (typically the csproj name, like "GodotSharp")</param>
    /// <param name="typeFullName">The type's full name (Example: Godot.Node)</param>
    /// <returns></returns>
    public static bool InheritsFrom(this ITypeSymbol? symbol, string assemblyName, string typeFullName)
    {
        while (symbol != null)
        {
            if (symbol.ContainingAssembly?.Name == assemblyName && symbol.FullQualifiedNameOmitGlobal() == typeFullName)
                return true;

            symbol = symbol.BaseType;
        }

        return false;
    }
    
    /// <summary>
    /// Tests if the script is of the type specified.
    /// </summary>
    /// <param name="cds">The <see cref="ClassDeclarationSyntax"/></param>
    /// <param name="assemblyName">The assembly name (example: GodotSharp)</param>
    /// <param name="typeFullName">The full name of the type (example: Godot.GodotObject)</param>
    /// <param name="compilation">The compilation</param>
    /// <param name="symbol">The symbol retrieved</param>
    /// <returns>True if it is a valid Godot class, false if not.</returns>
    private static bool TryGetClassOfType(this ClassDeclarationSyntax cds, string assemblyName, string typeFullName, Compilation compilation, out INamedTypeSymbol? symbol)
    {
        SemanticModel semanticModel = compilation.GetSemanticModel(cds.SyntaxTree);
        INamedTypeSymbol? classTypeSymbol = semanticModel.GetDeclaredSymbol(cds);

        if (classTypeSymbol?.BaseType == null || !classTypeSymbol.BaseType.InheritsFrom(assemblyName, typeFullName))
        {
            symbol = null;
            return false;
        }

        symbol = classTypeSymbol;
        return true;
    }

    /// <summary>
    /// Only selects classes inheriting from the type specified.
    /// </summary>
    /// <param name="source">The source</param>
    /// <param name="assemblyName">The assembly name (example: GodotSharp)</param>
    /// <param name="typeFullName">The full name of the type (example: Godot.GodotObject)</param>
    /// <param name="compilation">The compilation</param>
    /// <returns>Both the ClassDeclarationSymbol and the symbol associated.</returns>
    public static IEnumerable<(ClassDeclarationSyntax cds, INamedTypeSymbol symbol)> SelectClassesOfType(this IEnumerable<ClassDeclarationSyntax> source, string assemblyName, string typeFullName, Compilation compilation)
    {
        foreach (var cds in source)
        {
            if (cds.TryGetClassOfType(assemblyName, typeFullName, compilation, out var symbol))
                yield return (cds, symbol!);
        }
    }
        
    /// <summary>
    /// A <see cref="SymbolDisplayFormat.FullyQualifiedFormat"/> that omits the global namespace.
    /// </summary>
    private static SymbolDisplayFormat FullyQualifiedFormatOmitGlobal { get; } =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    /// <summary>
    /// Get the fully qualified name of a symbol. Omits the global namespace.
    /// </summary>
    /// <param name="symbol">The symbol</param>
    /// <returns>The fully qualified name w/o the global namespace (Example: Rubicon.Data.Generation.StaticAutoloadSingletonAttribute)</returns>
    public static string FullQualifiedNameOmitGlobal(this ITypeSymbol symbol) // SymbolDisplayTypeQualificationStyle
        => symbol.ToDisplayString(NullableFlowState.NotNull, FullyQualifiedFormatOmitGlobal);

    /// <summary>
    /// Get the fully qualified name of a namespace symbol. Omits the global namespace.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol</param>
    /// <returns>The fully qualified name w/o the global namespace (Example: Rubicon.Data.Generation)</returns>
    public static string FullQualifiedNameOmitGlobal(this INamespaceSymbol namespaceSymbol)
        => namespaceSymbol.ToDisplayString(FullyQualifiedFormatOmitGlobal);
        
    /// <summary>
    /// Checks if the provided symbol is of the type specified at <see cref="GenerationConstants.StaticAutoloadAttr"/>.
    /// </summary>
    /// <param name="symbol">The symbol</param>
    /// <returns>Whether the type is matching or not</returns>
    public static bool IsStaticAutoloadAttribute(this INamedTypeSymbol symbol) => symbol.FullQualifiedNameOmitGlobal() == GenerationConstants.StaticAutoloadAttr;

    public static bool IsGodotSignalAttribute(this INamedTypeSymbol symbol) => symbol.FullQualifiedNameOmitGlobal() == GenerationConstants.SignalAttr;
}