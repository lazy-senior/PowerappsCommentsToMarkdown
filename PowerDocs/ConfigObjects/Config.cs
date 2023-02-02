public class Config
{
    public string[]? PropertiesToScan { get; set; }
    public Dictionary<string, string[]>? ObjectsToScan{get;set;}
    public string? MermaidPrefix { get; set; }
    public string? MermaidSuffix { get; set; }
}
