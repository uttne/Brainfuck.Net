using System;

namespace Brainfuck.Net.Core.Tokens
{
    public class DotToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            stream.Write((char)memoryTape.Current);
        }
    }
}