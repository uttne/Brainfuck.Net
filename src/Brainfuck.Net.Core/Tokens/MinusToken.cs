namespace Brainfuck.Net.Core.Tokens
{
    public class MinusToken : BrainfuckToken
    {
        public override void Do()
        {
            MemoryTape.Decrement();
        }
    }
}