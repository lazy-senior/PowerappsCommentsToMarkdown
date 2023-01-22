// See https://aka.ms/new-console-template for more information
using CommandLine;
using PowerDocs;

Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(o => {
    if (Directory.Exists(o.InputFolder) && File.Exists(o.ConfigFile))
    {
        var pUtils = new App(o.ConfigFile);
        pUtils.LoadApp(o.InputFolder);

        Console.WriteLine(String.Join(",",pUtils.getScreenNames().ToArray()));
    } 
});