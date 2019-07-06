namespace Brainfuck.Net.Core.Tokens
{
    public class PlusToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            memoryTape.Increment();
        }
    }
}