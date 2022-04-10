using Microsoft.CodeAnalysis.CSharp;

namespace ClientGen;

public static class TypeMapper
{
    private static string FindKeywords(string type)
    {
        return type switch
        {
            "ArrayList" => "List",
            "boolean" => "bool",
            _ => type
        };
    }

    public static string MapType(string type)
    {
        var complicatedType = type.Split('[', ']', '<', '>').Where(s => s != "").ToArray();

        if (type.Last() == ']')
        {
            return $"{FindKeywords(complicatedType.First())}[]";
        }
        
        if (complicatedType.Length == 1)
        {
            return FindKeywords(type);
        }

        var otherType = type.Remove(0, complicatedType.First().Length);
        otherType = otherType.Remove(otherType.Length - 1, 1).Remove(0, 1);

        return $"{FindKeywords(complicatedType.First())}<{MapType(otherType)}>";
    }

    public static bool IsQuery(string type)
    {
        return type is
            "string" or
            "char" or
            "int" or
            "byte" or
            "short" or
            "decimal" or
            "double" or
            "float" or
            "long" or
            "String" or
            "Char" or
            "Int" or
            "Byte" or
            "Short" or
            "Decimal" or
            "Double" or
            "Float" or
            "Long" or
            "bool";
    }
}