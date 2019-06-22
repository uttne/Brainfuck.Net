namespace Brainfuck.Net.Core
{
    public interface IBrainfuckStream
    {
        void Write(char c);
        char Read();
    }
}