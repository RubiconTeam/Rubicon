using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Rubicon.SourceGenerators;

[Generator]
public class UserSettingsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {

    }

    public void Execute(GeneratorExecutionContext context)
    {
        #region User Settings Data
        INamedTypeSymbol? settingsData = context.Compilation.GetTypeByMetadataName(GenerationConstants.UserSettingsData);
        if (settingsData is null)
            throw new Exception("Could not find UserSettingsData found in \"GenerationConstants.UserSettingsData\".");

        var results = RecursiveSearchForValidOptions(settingsData.GetMembers().ToArray());
        string settingsNameSpace = settingsData.GetNamespaceName();

        StringBuilder dataClass = new StringBuilder();
        dataClass.Append("using Godot;\n" +
                         "using Godot.Collections;\n" +
                         "\n" +
                         $"namespace {settingsNameSpace};\n" +
                         "\n" +
                         $"public partial class {settingsData.Name}\n" +
                         "{\n");

        // Make load method
        dataClass.Append("\tpublic partial void Load(ConfigFile config)\n" +
                         "\t{\n");

        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;

            string section = result.pathTo.Substring(0, result.pathTo.IndexOf('.'));
            string key = result.pathTo.Substring(result.pathTo.IndexOf('.') + 1).Replace('.', '/');

            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\t{result.pathTo} = ({result.type.ToDisplayString()})config.GetValue(\"{section}\", \"{key}\", (long){result.pathTo}).AsInt64();\n");
            else
                dataClass.Append($"\t\t{result.pathTo} = ({result.type.ToDisplayString()})config.GetValue(\"{section}\", \"{key}\", {result.pathTo});\n");
        }

        // Make CreateConfigFileInstance()
        dataClass.Append("\t}\n" +
                         "\n" +
                         "\tpublic partial ConfigFile CreateConfigFileInstance()\n" +
                         "\t{\n" +
                         "\t\tConfigFile file = new ConfigFile();\n\n");

        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;

            string section = result.pathTo.Substring(0, result.pathTo.IndexOf('.'));
            string key = result.pathTo.Substring(result.pathTo.IndexOf('.') + 1).Replace('.', '/');

            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\tfile.SetValue(\"{section}\", \"{key}\", (long){result.pathTo});\n");
            else
                dataClass.Append($"\t\tfile.SetValue(\"{section}\", \"{key}\", {result.pathTo});\n");
        }

        // Make GetSetting
        dataClass.Append("\t\treturn file;\n" +
                         "\t}\n" +
                         "\n" +
                         "\tpublic partial Variant GetSetting(string key)\n" +
                         "\t{\n" +
                         "\t\tswitch (key)\n" +
                         "\t\t{\n");

        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;

            dataClass.Append($"\t\t\tcase \"{result.pathTo.Replace('.', '/')}\":\n");
            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\t\t\treturn Variant.CreateFrom((long){result.pathTo});\n");
            else
                dataClass.Append($"\t\t\t\treturn Variant.CreateFrom({result.pathTo});\n");
        }

        dataClass.Append("\t\t\tdefault:\n" +
                         "\t\t\t\treturn default;\n" +
                         "\t\t}\n" +
                         "\t}\n" +
                         "\n" +
                         "\tpublic partial void SetSetting(string key, Variant val)\n" +
                         "\t{\n" +
                         "\t\tswitch (key)\n" +
                         "\t\t{\n");

        foreach (var result in results)
        {
            if (!result.pathTo.Contains('.'))
                continue;

            dataClass.Append($"\t\t\tcase \"{result.pathTo.Replace('.', '/')}\":\n");
            if (result.type.TypeKind == TypeKind.Enum)
                dataClass.Append($"\t\t\t\t{result.pathTo} = ({result.type.ToDisplayString()})val.AsInt64();");
            else
                dataClass.Append($"\t\t\t\t{result.pathTo} = ({result.type.ToDisplayString()})val;");

            dataClass.Append("\n\t\t\t\tbreak;\n");
        }

        dataClass.Append("\t\t}\n" +
                         "\t}\n" +
                         "}");

        context.AddSource($"{settingsData.Name}.RubiconGenerated.cs", dataClass.ToString());
        #endregion

        #region User Settings Instance
        INamedTypeSymbol? settingsInstance = context.Compilation.GetTypeByMetadataName(GenerationConstants.UserSettingsInstance);
        if (settingsInstance is null)
            throw new Exception("Could not find UserSettingsInstance found in \"GenerationConstants.UserSettingsInstance\".");

        AttributeData? staticAutoload = settingsInstance.GetAttributes().FirstOrDefault(x => x.AttributeClass?.IsStaticAutoloadAttribute() ?? false);
        if (staticAutoload is null)
            throw new Exception("Could not find static autoload attribute (GenerationConstants.StaticAutoloadAttr) in class \"GenerationConstants.UserSettingsInstance\")");

        string staticNameSpace = staticAutoload.ConstructorArguments[0].Value?.ToString()!;
        string staticClassName = staticAutoload.ConstructorArguments[1].Value?.ToString()!;

        IPropertySymbol[] dataProperties = settingsData.GetMembers()
            .Where(x => x.Kind is SymbolKind.Property && x is { IsStatic: false, DeclaredAccessibility: Accessibility.Public })
            .Cast<IPropertySymbol>()
            .Where(x => !x.IsWriteOnly && !x.IsImplicitlyDeclared)
            .ToArray();

        IFieldSymbol[] dataFields = settingsData.GetMembers()
            .Where(x => x.Kind is SymbolKind.Field && x is { IsStatic: false, DeclaredAccessibility: Accessibility.Public })
            .Cast<IFieldSymbol>()
            .Where(x => !x.IsImplicitlyDeclared)
            .ToArray();

        string instanceNameSpace = settingsInstance.GetNamespaceName();

        List<string> allInstanceUsings = new List<string>();
        StringBuilder instanceClass = new StringBuilder();
        instanceClass.Append($"namespace {instanceNameSpace};\n" +
                             "\n" +
                             $"public partial class {settingsInstance.Name}" +
                             "{\n");

        StringBuilder staticClass = new StringBuilder();
        if (!string.IsNullOrEmpty(staticNameSpace))
            staticClass.Append($"namespace {staticNameSpace};\n\n");

        staticClass.Append($"public static partial class {staticClassName}\n" +
                           "{\n");

        foreach (IPropertySymbol property in dataProperties)
        {
            string propertyNameSpace = property.Type.GetNamespaceName();
            if (!string.IsNullOrEmpty(propertyNameSpace) && propertyNameSpace != instanceNameSpace && !allInstanceUsings.Contains(propertyNameSpace))
                allInstanceUsings.Add(propertyNameSpace);

            // Make documentation comment
            instanceClass.Append($"\t/// <inheritdoc cref=\"{settingsData.Name}.{property.Name}\"/>\n");
            staticClass.Append($"\t/// <inheritdoc cref=\"{settingsData.Name}.{property.Name}\"/>\n");

            if (property.IsReadOnly)
            {
                instanceClass.Append($"\tpublic {property.Type.ToDisplayString()} {property.Name} => _data.{property.Name};\n\n");
                staticClass.Append($"\tpublic static {property.Type.ToDisplayString()} {property.Name} => Singleton.{property.Name};");
                continue;
            }

            instanceClass.Append($"\tpublic {property.Type.ToDisplayString()} {property.Name}\n" +
                                 "\t{\n");
            staticClass.Append($"\tpublic static {property.Type.ToDisplayString()} {property.Name}\n" +
                               "\t{\n");

            if (property.Type.TypeKind != TypeKind.Delegate)
            {
                instanceClass.Append($"\t\tget => _data.{property.Name};\n" +
                                     $"\t\tset\n" +
                                     "\t\t{\n" +
                                     $"\t\t\t_data.{property.Name} = value;\n" +
                                     "\t\t\tUpdateSettings();\n" +
                                     "\t\t}\n");

                staticClass.Append($"\t\tget => Singleton.{property.Name};\n" +
                                   $"\t\tset => Singleton.{property.Name} = value;\n");
            }
            else
            {
                instanceClass.Append($"\t\tadd => _data.{property.Name} += value;\n" +
                                     $"\t\tremove => _data.{property.Name} -= value;\n");

                staticClass.Append($"\t\tadd => Singleton.{property.Name} += value;\n" +
                                   $"\t\tremove => Singleton.{property.Name} -= value;\n");
            }

            instanceClass.Append("\t}\n\n");
            staticClass.Append("\t}\n\n");
        }

        foreach (IFieldSymbol field in dataFields)
        {
            string fieldNameSpace = field.Type.GetNamespaceName();
            if (!string.IsNullOrEmpty(fieldNameSpace) && fieldNameSpace != instanceNameSpace && !allInstanceUsings.Contains(fieldNameSpace))
                allInstanceUsings.Add(fieldNameSpace);

            // Make documentation comment
            instanceClass.Append($"\t/// <inheritdoc cref=\"{settingsData.Name}.{field.Name}\"/>\n" +
                                $"\tpublic {field.Type.ToDisplayString()} {field.Name}\n" +
                                "\t{\n");

            staticClass.Append($"\t/// <inheritdoc cref=\"{settingsData.Name}.{field.Name}\"/>\n" +
                               $"\tpublic static {field.Type.ToDisplayString()} {field.Name}\n" +
                               "\t{\n");

            if (field.Type.TypeKind != TypeKind.Delegate)
            {
                instanceClass.Append($"\t\tget => _data.{field.Name};\n" +
                                     $"\t\tset\n" +
                                     "\t\t{\n" +
                                     $"\t\t\t_data.{field.Name} = value;\n" +
                                     "\t\t\tUpdateSettings();\n" +
                                     "\t\t}\n");

                staticClass.Append($"\t\tget => Singleton.{field.Name};\n" +
                                   $"\t\tset => Singleton.{field.Name} = value;\n");
            }
            else
            {
                instanceClass.Append($"\t\tadd => _data.{field.Name} += value;\n" +
                                     $"\t\tremove => _data.{field.Name} -= value;\n");

                staticClass.Append($"\t\tadd => Singleton.{field.Name} += value;\n" +
                                   $"\t\tremove => Singleton.{field.Name} -= value;\n");
            }

            instanceClass.Append("\t}\n\n");
            staticClass.Append("\t}\n\n");
        }

        instanceClass.Remove(instanceClass.Length - 1, 1);
        instanceClass.Append("}");

        staticClass.Remove(staticClass.Length - 1, 1);
        staticClass.Append("}");

        StringBuilder usingsText = new();
        foreach (string usingDirective in allInstanceUsings)
            usingsText.Append($"using {usingDirective};\n");
        usingsText.Append("\n");

        context.AddSource($"{settingsInstance.Name}.RubiconGenerated.cs", usingsText.ToString() + instanceClass.ToString());

        usingsText.Remove(usingsText.Length - 1, 1);
        usingsText.Append($"using {instanceNameSpace};");
        usingsText.Append("\n");

        context.AddSource($"{staticClassName}Ex.RubiconGenerated.cs", usingsText.ToString() + staticClass.ToString());

        #endregion
    }

    private (string pathTo, ITypeSymbol type)[] RecursiveSearchForValidOptions(ISymbol[] symbols)
    {
        IPropertySymbol[] properties = symbols
            .Where(x => x.Kind is SymbolKind.Property && x is { IsStatic: false, DeclaredAccessibility: Accessibility.Public })
            .Cast<IPropertySymbol>()
            .Where(x => !x.IsReadOnly && !x.IsWriteOnly && !x.IsImplicitlyDeclared)
            .ToArray();

        IFieldSymbol[] fields = symbols
            .Where(x => x.Kind is SymbolKind.Field && x is { IsStatic: false, DeclaredAccessibility: Accessibility.Public })
            .Cast<IFieldSymbol>()
            .Where(x => !x.IsImplicitlyDeclared)
            .ToArray();

        List<(string pathTo, ITypeSymbol type)> results = new();
        foreach (IPropertySymbol property in properties)
        {
            string typeFullName = property.Type.ToDisplayString();
            if (property.Type.TypeKind is TypeKind.Class
                && !typeFullName.Contains("Godot.Collections.Array")
                && !typeFullName.Contains("Godot.Collections.Dictionary"))
            {
                var propertyResults =
                    RecursiveSearchForValidOptions(property.Type.GetMembers().ToArray());

                foreach (var result in propertyResults)
                    results.Add((property.Name + "." + result.pathTo, result.type));

                continue;
            }

            results.Add((property.Name, property.Type));
        }

        foreach (IFieldSymbol field in fields)
        {
            string typeFullName = field.Type.ToDisplayString();
            if (field.Type.TypeKind is TypeKind.Class && !typeFullName.Contains("Godot.Collections.Array") && !typeFullName.Contains("Godot.Collections.Dictionary"))
            {
                var fieldResults =
                    RecursiveSearchForValidOptions(field.Type.GetMembers().ToArray());

                foreach (var result in fieldResults)
                    results.Add((field.Name + "." + result.pathTo, result.type));

                continue;
            }

            results.Add((field.Name, field.Type));
        }

        return results.ToArray();
    }
}