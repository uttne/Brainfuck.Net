using System;

namespace Brainfuck.Net.Core.Tokens
{
    public class LeftBracketToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            
        }
        
        public RightBracketToken RightBracketToken { get; internal set; }
        
        public override BrainfuckToken GetNext(BrainfuckMemoryTape memoryTape)
        {
            if (memoryTape.Current == 0)
            {
                return RightBracketToken;
            }

            return NextIndexToken;
        }
    }
}