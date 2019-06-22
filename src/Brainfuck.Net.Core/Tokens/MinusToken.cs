namespace Brainfuck.Net.Core.Tokens
{
    public class MinusToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            memoryTape.Decrement();
        }
    }
}