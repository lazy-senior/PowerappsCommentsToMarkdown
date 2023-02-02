using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        public App()
        {
            Config = new();
            Screens = new Dictionary<string, JsonElement>();
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
            var keySegments = key.Split(" ", 3);
            if(keySegments.Length != 3)
                return false;
            nameAndType = (keySegments[0], keySegments[2]);
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
                    switch (nameAndType.Type)
                    {
                        case "appinfo":
                            this.AppSettings = jsonElement.Value;
                            break;
                        case "screen":
                            this.Screens.Add(nameAndType.Name, jsonElement.Value);
                            break;
                    } 
                }
            }

            foreach(var screen in this.Screens)
            {
                LoadProperties(screen.Key, screen.Value, 0);
                break;
            }

        }
        public void LoadProperties(string name, JsonElement jsonElement, int depth)
        {
            (string Name ,string Type) nameAndType;
            Console.WriteLine($"{new string('\t',depth)}{name}:");
            foreach(JsonProperty jsonProperty in jsonElement.EnumerateObject()){
                //Normales Attribute am Objekt
                if(!TryGetPowerappsNameAndTypeAsString(jsonProperty.Name, out nameAndType))
                {
                    if(Config.PropertiesToScan.Any(prop => jsonProperty.Name.StartsWith(prop)))
                    {
                        Console.WriteLine($"{new string('\t',depth+1)}{jsonProperty.Name}: {jsonProperty.Value}");
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
    