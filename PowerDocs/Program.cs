// See https://aka.ms/new-console-template for more information
using CommandLine;
using PowerDocs;

Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed<CommandLineOptions>(o => {
    if (Directory.Exists(o.InputFolder) && File.Exists(o.ConfigFile))
    {
        var pUtils = new App(o.ConfigFile);
        pUtils.LoadApp(o.InputFolder);
        pUtils.GenerateMarkdown(o.OutputFolder);

        Console.WriteLine(String.Join(",",pUtils.getScreenNames().ToArray()));
    } else {
        Console.WriteLine($"Wrong configuration:\r\n\t-i:'{o.InputFolder}'\r\n\t-o:'{o.OutputFolder}'\r\n\t-c:'{o.ConfigFile}'");
    }
});