namespace Brainfuck.Net.Core.Tokens
{
    public class RightShiftToken : BrainfuckToken
    {
        public override void Do()
        {
            MemoryTape.RightShift();
        }
    }
}