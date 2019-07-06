namespace Brainfuck.Net.Core.Tokens
{
    public class RightShiftToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            memoryTape.RightShift();
        }
    }
}