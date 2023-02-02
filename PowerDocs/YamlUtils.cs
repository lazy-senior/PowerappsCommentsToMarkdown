using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PowerDocs
{
    internal class YamlUtils
    {
        private IDeserializer yamlDeserializer { get; set; }
        private ISerializer yamlSerializer { get; set; }

        public YamlUtils()
        {
            yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            yamlSerializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();
        }

        public T SerializeFile<T>(string path)
        {
            var yamlFileContent = File.ReadAllText(path);
            using var stringReader = new StringReader(yamlFileContent);
            return yamlDeserializer.Deserialize<T>(stringReader);
        }

        public JsonDocument SerializeYamlFileAsJsonDocument(string path)
        {
            var yamlFileContent = File.ReadAllText(path);
            using var stringReader = new StringReader(yamlFileContent);
            var yamlObject = yamlDeserializer.Deserialize(stringReader);
            if(yamlObject != null){
                var jsonString = yamlSerializer.Serialize(yamlObject);
                return JsonDocument.Parse(jsonString);
            }
            return JsonDocument.Parse("");
        }

        public bool IsYamlFile(string path)
        {
            return path.EndsWith("yaml");
        }

    }
}
