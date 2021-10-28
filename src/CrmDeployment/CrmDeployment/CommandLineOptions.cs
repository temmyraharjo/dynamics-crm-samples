using CommandLine;

namespace CrmDeployment
{
    public class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "CRM Connection string.")]
        public string ConnectionString { get; set; }
        [Value(index: 1, Required = true, HelpText = "Type of the execution console.")]
        public string Type { get; set; }
        [Option(shortName: 'f', longName: "filepath", Required = false, HelpText = "FilePath of plugin.")]
        public string FilePath { get; set; }
        [Option(shortName: 'd', longName: "directory", Required = false, HelpText = "Directory")]
        public string Directory { get; set; }
        [Option(shortName: 'm', longName: "managed", Required = false, HelpText = "Managed")]
        public bool? IsManaged { get; set; }
        [Option(shortName: 't', longName: "today", Required = false, HelpText = "Import Today Solution")]
        public bool? IsToday { get; set; }
    }
}
