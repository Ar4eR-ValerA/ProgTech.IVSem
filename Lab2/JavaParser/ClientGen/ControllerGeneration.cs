using AntlrExample.UsableTreeModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ClientGen;

public class ControllerGeneration
{
    public ControllerGeneration(CsController csController)
    {
        CsController = csController;
        DtosGeneration = new DtosGeneration(csController.Dtos.ToList());
    }

    public CsController CsController { get; private set; }
    public DtosGeneration DtosGeneration { get; private set; }

    public void GenerateController(string path)
    {
        DtosGeneration.GenerateDtos(path);
    }
}