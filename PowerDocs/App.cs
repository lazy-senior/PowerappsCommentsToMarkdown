using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            Screens = new Dictionary<string, JsonElement>();
        }

        public App(string configFile) : this()
        {
            Config = YamlUtils.SerializeFile<Config>(configFile);
            Console.Write(Config.MermaidPrefix);
        }

        private bool isValidPowerappsFolder(string path)
        {
            return Directory.Exists(path + "/Src");
        }

        private IList<string> getYAMLFilesInDirectory(string path)
        {
            return Directory.GetFiles(path).Where(f => YamlUtils.IsYamlFile(f)).ToList();
        }

        public void LoadApp(string filePath) 
        {
            if(!isValidPowerappsFolder(filePath)) return;

            string srcPath = Path.GetFullPath(filePath) + "/Src";
            var yamlFiles = getYAMLFilesInDirectory(srcPath);

            foreach(var yamlFile in yamlFiles)
            {
                var jsonDocument = YamlUtils.SerializeYamlFileAsJsonDocument(yamlFile);

                foreach(var jsonElement in jsonDocument.RootElement.EnumerateObject())
                {
                    var nameParts = jsonElement.Name.Split(" ", 2);
                    if (nameParts.Length != 2) continue;
                    switch (nameParts[1])
                    {
                        case "As appinfo":
                            this.AppSettings = jsonElement.Value;
                            break;
                        case "As screen":
                            this.Screens.Add(nameParts[0], jsonElement.Value);
                            break;
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
    