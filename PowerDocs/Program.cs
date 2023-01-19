// See https://aka.ms/new-console-template for more information
using CommandLine;
using PowerDocs;

Console.WriteLine("Hello, World!");

Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
    if (Directory.Exists(o.InputFolder))
    {
        var pUtils = new PowerappsUtils();
        if (pUtils.TryParseApp(o.InputFolder, out var parsedApp))
        {
            pUtils.GenerateDoc(o.InputFolder, o.OutputFile);
        }
    } 
});