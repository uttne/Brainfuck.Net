namespace Brainfuck.Net.Core.Tokens
{
    public class LeftBracketToken : BrainfuckToken
    {
        public override void Do()
        {
            
        }
        
        public RightBracketToken RightBracketToken { get; internal set; }
        
        public override BrainfuckToken Next
        {
            get
            {
                if (MemoryTape.Current == 0)
                {
                    return RightBracketToken;
                }

                return NextIndexToken;
            }
        }
    }
}