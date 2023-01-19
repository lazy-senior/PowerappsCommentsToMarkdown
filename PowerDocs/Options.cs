using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerDocs
{
    internal class Options
    {
        [Option('i', "input", Required = true)]
        public string? InputFolder { get; set; }

        [Option('o', "output", Required = true)]
        public string? OutputFile { get; set; }
    }
}
