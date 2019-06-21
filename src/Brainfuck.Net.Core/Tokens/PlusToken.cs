namespace Brainfuck.Net.Core.Tokens
{
    public class PlusToken : BrainfuckToken
    {
        public override void Do()
        {
            MemoryTape.Increment();
        }
    }
}