using System.Collections.Generic;
using Brainfuck.Net.ArgsAnalysis;

namespace Brainfuck.Net
{
    public class Options
    {
        [Parameters]
        public IList<string> Parameters { get; set; }
        
        [Option("h|help","show help")]
        public bool Help { get; set; }
        
        [Option("s|source","")]
        public string Source { get; set; }
        
        [Option("c|code","",true)]
        public string Code { get; set; }
    }
}