// See https://aka.ms/new-console-template for more information
using CommandLine;
using PowerDocs;

Console.WriteLine("Hello, World!");

Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(o => {
    if (Directory.Exists(o.InputFolder))
    {
        var pUtils = new PowerappUtils();
        pUtils.LoadApp(o.InputFolder);

        Console.WriteLine(String.Join(",",pUtils.getScreenNames().ToArray()));
    } 
});