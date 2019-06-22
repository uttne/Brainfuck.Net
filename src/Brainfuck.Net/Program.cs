using System;
using System.IO;
using Brainfuck.Net.Core;

namespace Brainfuck.Net
{
    static class Program
    {
        static void Main(string[] args)
        {
            
            //Todo Analyse args.
            
            var src = "+++++++++[>++++++++>+++++++++++>+++++<<<-]>.>++.+++++++..+++.>-." +
                      "------------.<++++++++.--------.+++.------.--------.>+.#";
            var parser = new BrainfuckParser();
            using (var sr = new StringReader(src))
            {
                var tokens = parser.Parse(sr);

                var memoryTape = new BrainfuckMemoryTape();

                var stream = new BrainfuckStream();
                
                var steps = BrainfuckSteps.Create(tokens, memoryTape, stream);
                
                steps.Run();
            }
            
        }
    }
}