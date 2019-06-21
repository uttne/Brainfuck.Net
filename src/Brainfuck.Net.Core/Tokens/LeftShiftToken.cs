namespace Brainfuck.Net.Core.Tokens
{
    public class LeftShiftToken : BrainfuckToken
    {
        public override void Do()
        {
            MemoryTape.LeftShift();
        }
    }
}