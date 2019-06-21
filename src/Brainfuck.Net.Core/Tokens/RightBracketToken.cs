namespace Brainfuck.Net.Core.Tokens
{
    public class RightBracketToken : BrainfuckToken
    {
        public override void Do()
        {
            
        }
        
        public LeftBracketToken LeftBracketToken { get; internal set; }

        public override BrainfuckToken Next
        {
            get
            {
                if (MemoryTape.Current == 0)
                {
                    return NextIndexToken;
                }

                return LeftBracketToken;
            }
        }
    }
}