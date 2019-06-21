using System;

namespace Brainfuck.Net.Core.Tokens
{
    public class SemicolonToken : BrainfuckToken
    {
        public override void Do()
        {
            var c = Console.Read();
            MemoryTape.Current = (int) c;
        }
    }
}