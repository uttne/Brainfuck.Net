namespace Brainfuck.Net.Core.Tokens
{
    public abstract class BrainfuckToken
    {
        public BrainfuckMemoryTape MemoryTape { get; internal set; }
        public virtual BrainfuckToken Next => NextIndexToken;
        
        public BrainfuckToken NextIndexToken { get; private set; }
        public int Index { get; internal set; } = -1;

        public abstract void Do();

        public void SetNextIndexToken(BrainfuckToken nextIndexToken)
        {
            NextIndexToken = nextIndexToken;
        }
    }
}