using System;
using Brainfuck.Net.Core;

namespace Brainfuck.Net
{
    public class BrainfuckStream : IBrainfuckStream
    {
        public void Write(char c)
        {
            Console.Write(c);
        }

        public char Read()
        {
            return (char)Console.Read();
        }
    }
}