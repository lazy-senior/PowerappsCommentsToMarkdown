using System;
using System.Collections.Generic;
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
    internal class PowerappUtils
    {
        private JsonElement AppSettings { get; set; }
        private Dictionary<string, JsonElement> Screens { get; set; }
        private IDeserializer yamlDeserializer { get; set; }
        private ISerializer yamlSerializer { get; set; }

        public PowerappUtils() { 
            yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            yamlSerializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            Screens = new Dictionary<string, JsonElement>();
        }
        private bool isYamlFile(string path)
        {
            return path.EndsWith("yaml");
        }

        private IList<string> getYAMLFilesInDirectory(string path)
        {
            return Directory.GetFiles(path).Where(f => isYamlFile(f)).ToList();
        }

        public void LoadApp(string filePath) 
        { 
            string srcPath = Path.GetFullPath(filePath) + "/Src";
            if (!Directory.Exists(srcPath))
            {
                Console.WriteLine($"Directory {srcPath} does not exists!");
            }

            var yamlFiles = getYAMLFilesInDirectory(srcPath);

            Console.WriteLine($"{yamlFiles.Count()} yamlFiles found.");

            foreach(var yamlFile in yamlFiles)
            {
                //Convert yaml to json
                var yamlFileContent = File.ReadAllText(yamlFile);
                using var stringReader = new StringReader(yamlFileContent);
                var yamlObject = yamlDeserializer.Deserialize(stringReader);
                var jsonString = yamlSerializer.Serialize(yamlObject);

                var jsonDocument = JsonDocument.Parse(jsonString);
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
    