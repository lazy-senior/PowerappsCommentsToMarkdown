﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerDocs
{
    internal class CommandLineOptions
    {
        [Option('i', "input", Required = true)]
        public string? InputFolder { get; set; }

        [Option('o', "output", Required = false)]
        public string? OutputFolder { get; set; }

        [Option('c', "config", Required = true)]
        public string? ConfigFile { get; set; }
    }
}
