using System;

namespace Brainfuck.Net.Core.Tokens
{
    public class DotToken : BrainfuckToken
    {
        public override void Do()
        {
            Console.Write((char)MemoryTape.Current);
        }
    }
}