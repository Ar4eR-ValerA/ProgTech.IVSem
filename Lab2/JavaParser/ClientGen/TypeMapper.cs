using Microsoft.CodeAnalysis.CSharp;

namespace ClientGen;

public static class TypeMapper
{
    public static string MapType(string type)
    {
        return type switch
        {
            "String" => "string",
            _ => MapComplicatedType(type)
        };
    }

    private static string MapComplicatedType(string type)
    {
        return "int";
    }
}