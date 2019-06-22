namespace Brainfuck.Net.Core.Tokens
{
    public class RightBracketToken : BrainfuckToken
    {
        public override void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream)
        {
            
        }
        
        public LeftBracketToken LeftBracketToken { get; internal set; }

        public override BrainfuckToken GetNext(BrainfuckMemoryTape memoryTape)
        {
            if (memoryTape.Current == 0)
            {
                return NextIndexToken;
            }

            return LeftBracketToken;
        }
    }
}