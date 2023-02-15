using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PowerDocs
{

    internal class App
    {
        private JsonElement AppSettings { get; set; }
        private Dictionary<string, JsonElement> Screens { get; set; }
        private YamlUtils YamlUtils { get; set; } = new();
        private Config Config { get; set; }
        private MarkdownGenerator markdownGenerator { get; set; }

        public App()
        {
            Config = new();
            Screens = new Dictionary<string, JsonElement>();
            markdownGenerator = new();
        }

        public App(string configFile) : this()
        {
            Config = YamlUtils.SerializeFile<Config>(configFile);
            Console.WriteLine(String.Join(",",Config.PropertiesToScan));
        }

        private bool isValidPowerappsFolder(string path)
        {
            return Directory.Exists(path + "/Src");
        }

        private IList<string> getYAMLFilesInDirectory(string path)
        {
            return Directory.GetFiles(path).Where(f => YamlUtils.IsYamlFile(f)).ToList();
        }
        private bool TryGetPowerappsNameAndTypeAsString(string key, out (string Name,string Type) nameAndType)
        {
            nameAndType = (string.Empty,string.Empty);
            var keySegments = key.Split(" As ", 2);
            if(keySegments.Length != 2)
                return false;
            nameAndType = (keySegments[0], keySegments[1]);
            return true;
        }

        public void LoadApp(string filePath) 
        {
            if(!isValidPowerappsFolder(filePath)) return;

            string srcPath = Path.GetFullPath(filePath) + "/Src";
            var yamlFiles = getYAMLFilesInDirectory(srcPath);
            (string Name ,string Type) nameAndType;

            foreach(var yamlFile in yamlFiles)
            {
                var jsonDocument = YamlUtils.SerializeYamlFileAsJsonDocument(yamlFile);

                foreach(var jsonElement in jsonDocument.RootElement.EnumerateObject())
                {
                    if(!TryGetPowerappsNameAndTypeAsString(jsonElement.Name, out nameAndType)){
                        continue;
                    }
                    if (nameAndType.Type.StartsWith("appinfo"))
                    {
                        this.AppSettings = jsonElement.Value;
                    }
                    else if(nameAndType.Type.StartsWith("screen"))
                    {
                        this.Screens.Add(nameAndType.Name, jsonElement.Value);
                    } 
                }
            }
        }
        public void GenerateMarkdown(string outputPath)
        {
            Console.WriteLine("Generating .md-files...");
            Console.WriteLine($"\tScreens:{this.Screens.Count}");
            Console.WriteLine($"\tSaving to:{outputPath}");
            foreach(var screen in this.Screens)
            {
                Console.Write($"\t->{screen.Key}:");
                LoadProperties(screen.Key, screen.Value, 0);
                markdownGenerator.OutputToFile($"{outputPath}\\{screen.Key}.md");
                Console.WriteLine($"\tdone.");
            }
        }
        public void LoadProperties(string name, JsonElement jsonElement, int depth)
        {
            (string Name ,string Type) nameAndType;
            markdownGenerator.AddLine(name, "", true, depth+1);
            foreach(JsonProperty jsonProperty in jsonElement.EnumerateObject()){
                //Normales Attribute am Objekt
                if(!TryGetPowerappsNameAndTypeAsString(jsonProperty.Name, out nameAndType))
                {
                    if(Config.PropertiesToScan.Any(prop => jsonProperty.Name.StartsWith(prop)))
                    {
                        markdownGenerator.AddLine(jsonProperty.Name, jsonProperty.Value.ToString(), false, depth+1);
                    }
                }
                //Weiteres Object mit eigenen Attributen
                else {
                    if(Config.ObjectsToScan.Keys.Any(key => nameAndType.Type.StartsWith(key)))
                    {
                        LoadProperties(jsonProperty.Name, jsonProperty.Value, depth+1);
                    }
                }
            }
        }
       
        public IList<string> getScreenNames()
        {
            return Screens.Keys.ToList();
        }
    }
}
    