using Antlr4.Runtime.Tree;

namespace AntlrExample.ParsingTools;

public class GraphVisualizer
{
    private T FindNewContext<T>(IParseTree parseTree) where T : IParseTree
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

    private void Visualize(StreamWriter writer, int indent, IParseTree parseTree)
    {
        string pads = string.Empty.PadRight(indent);
        writer.Write($"{pads}{parseTree.GetType().FullName.Split("+").Last().Split(".").Last()}");

        if (parseTree is TerminalNodeImpl)
        {
            writer.Write($"  ({parseTree.GetText()})");
        }
        
        writer.WriteLine();
        
        if (parseTree is not TerminalNodeImpl)
        {
            for (var i = 0; i < parseTree.ChildCount; i++)
            {
                var child = parseTree.GetChild(i);

                Visualize(writer, indent + 4, child);
            }

        }
    }

    public void Visualize(IParseTree parseTree, string filePath)
    {
        var writer = new StreamWriter(filePath);
        
        Visualize(writer, 0, parseTree);
        
        writer.Close();
    }
}