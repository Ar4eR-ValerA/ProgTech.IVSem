using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeModels;

public static class Tools
{
    public static T SteppedDownContext<T>(IParseTree parseTree) where T : IParseTree
    {
        if (parseTree is null)
        {
            return default;
        }

        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is T childT)
            {
                return childT;
            }
        }

        return default;
    }

    public static IReadOnlyList<T> SteppedDownContexts<T>(IParseTree parseTree) where T : IParseTree
    {
        var contexts = new List<T>();

        if (parseTree is null)
        {
            return contexts;
        }

        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is T childT)
            {
                contexts.Add(childT);
            }
        }

        return contexts;
    }
}