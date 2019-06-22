using System;
using System.IO;
using Brainfuck.Net.ArgParsers;
using Brainfuck.Net.Core;

namespace Brainfuck.Net
{
    enum MyEnum
    {
        Test
    }

    class TestClass
    {
        public TestClass(string text)
        {
            Text = text;
        }
        public string Text { get; }
    }
    
    class Test2Class
    {
        public string Text { get; private set; }

        public static Test2Class Parse(string text)
        {
            return new Test2Class()
            {
                Text = text
            };
        }
    }
    
    static class Program
    {
        static void Main(string[] args)
        {
            var value = ArgsParser<int>.Option.Convert(typeof(Test2Class), "Test");
            
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