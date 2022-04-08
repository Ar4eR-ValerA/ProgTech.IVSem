using Microsoft.CodeAnalysis.CSharp;

namespace ClientGen;

public static class TypeMapper
{
    private static string FindKeywords(string type)
    {
        return type switch
        {
            "ArrayList" => "List",
            _ => type
        };
    }

    public static string MapType(string type)
    {
        var complicatedType = type.Split('[', ']', '<', '>').Where(s => s != "").ToArray();

        if (complicatedType.Length == 1 && complicatedType.Last() == "]")
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
}