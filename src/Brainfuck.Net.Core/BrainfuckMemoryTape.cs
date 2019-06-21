using System;
using System.Collections.Generic;
using System.Linq;

namespace Brainfuck.Net.Core
{
    public class BrainfuckMemoryTape
    {
        private readonly List<int> _memory = Enumerable.Repeat(0, 100).ToList();
        
        private int _index;

        public int Index => _index;

        public void RightShift()
        {
            ++_index;

            while (_memory.Count <= _index)
            {
                _memory.Add(0);
            }
        }
        
        public void LeftShift()
        {
            --_index;
            if(_index < 0)
                throw new InvalidOperationException();
        }

        public int Decrement()
        {
            return --_memory[_index];
        }
        
        public int Increment()
        {
            return ++_memory[_index];
        }

        public int Current
        {
            get => _memory [_index];
            set => _memory[_index] = value;
        }
    }
}