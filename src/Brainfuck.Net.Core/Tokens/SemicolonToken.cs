using System;

namespace Brainfuck.Net.Core.Tokens
{
    public class SemicolonToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            var c = Console.Read();
            memoryTape.Current = (int) c;
        }
    }
}