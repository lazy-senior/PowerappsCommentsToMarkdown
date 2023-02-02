using System.Text;

namespace PowerDocs
{
    internal class MarkdownGenerator
    {
        private Dictionary<string,string> LineBuffer { get; set; }

        private StringBuilder Output { get; set; }

        public MarkdownGenerator(){
            Output = new StringBuilder();
            LineBuffer = new();
        }
        public void AddLine(string key, string content, bool isHeader, int depth)
        {
            if(isHeader){
                AddLineBuffer();
                AddHeading(key, depth);
            } else {
                if(content.TrimEnd().TrimStart().Replace("=", "").Length > 0)
                    LineBuffer.Add(key,content);
            }
        }

        private void AddLineBuffer(){
            if(LineBuffer.Where(line => !line.Key.StartsWith("On")).Count() > 0) {
                AddTableHeading();
                foreach(var line in LineBuffer.Where(line => !line.Key.StartsWith("On"))){
                    AddTableLine(line.Key, line.Value);
                }
            }
            foreach(var line in LineBuffer.Where(line => line.Key.StartsWith("On"))){
                AddCodeBlock(line.Key, line.Value);
            }
            LineBuffer = new();
        }

        private void AddHeading(string heading, int depth){
            Output.AppendLine($"{new String('#',depth)} {heading}");
        }
        private void AddTableHeading(string keyHeading = "Attribute", string valueHeading = "Value"){
            Output.AppendLine($"|{keyHeading}|{valueHeading}|");
            Output.AppendLine("|---|---|");
        }
        private void AddTableLine(string key, string value){
            Output.AppendLine($"|{key}|{value}");
        }
        private void AddCodeBlock(string key, string value){
            Output.AppendLine($"{key}");
            Output.AppendLine($"```yaml\n{value}\n```");
        }

        public void OutputToFile(string outputFile)
        {
            AddLineBuffer();
            System.IO.File.WriteAllText(outputFile, Output.ToString());
            Output.Clear();
        }
    }
}
    