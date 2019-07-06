using System;
using System.Collections.Generic;
using System.IO;
using Brainfuck.Net.ArgsAnalysis;
using Brainfuck.Net.Core;
using Brainfuck.Net.Core.Tokens;

namespace Brainfuck.Net
{
    static class Program
    {
        static void Main(string[] args)
        {
            var argsParser = new ArgsParser<Options>();

            IArgsData<Options> options; 
            try
            {
                options = argsParser.Parse(args);
            }
            catch (AnalysisException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (options.Option.Help)
            {
                throw new NotImplementedException();
            }

            if (options.Has(x => x.Code) && options.Has(x => x.Source))
            {
                throw new NotImplementedException();
            }

            string src;
            
            
            var parser = new BrainfuckParser();
            IEnumerable<BrainfuckToken> tokens;
            if (options.Has(x => x.Code))
            {
                src = options.Option.Code;
                using (var sr = new StringReader(src))
                {
                    tokens = parser.Parse(sr);
                }
            }
            else if (options.Has(x=>x.Source))
            {
                using (var fs = new FileStream(options.Option.Source, FileMode.Open, FileAccess.Read, FileShare.Read))
                using(var sr = new StreamReader(fs))
                {
                    tokens = parser.Parse(sr);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            var memoryTape = new BrainfuckMemoryTape();

            var stream = new BrainfuckStream();

            var steps = BrainfuckSteps.Create(tokens, memoryTape, stream);

            steps.Run();

        }
    }
}