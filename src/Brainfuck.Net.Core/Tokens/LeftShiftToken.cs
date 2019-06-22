namespace Brainfuck.Net.Core.Tokens
{
    public class LeftShiftToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            memoryTape.LeftShift();
        }
    }
}