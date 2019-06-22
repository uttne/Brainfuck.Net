namespace Brainfuck.Net.Core.Tokens
{
    public abstract class BrainfuckToken
    {
        public BrainfuckToken NextIndexToken { get; private set; }
        public int Index { get; internal set; } = -1;

        public abstract void Operate(BrainfuckMemoryTape memoryTape, IBrainfuckStream stream);

        public void SetNextIndexToken(BrainfuckToken nextIndexToken) => NextIndexToken = nextIndexToken;

        public virtual BrainfuckToken GetNext(BrainfuckMemoryTape memoryTape) => NextIndexToken;
    }
}