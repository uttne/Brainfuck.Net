using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Brainfuck.Net.Core.Tokens;

namespace Brainfuck.Net.Core
{
    public class BrainfuckSteps:IEnumerable<BrainfuckToken>
    {
        private readonly BrainfuckToken _first;
        private readonly BrainfuckMemoryTape _memoryTape;
        private readonly IBrainfuckStream _stream;

        public BrainfuckSteps(BrainfuckToken first,BrainfuckMemoryTape memoryTape,IBrainfuckStream stream)
        {
            _first = first ?? throw new ArgumentNullException(nameof(first));
            _memoryTape = memoryTape ?? throw new ArgumentNullException(nameof(memoryTape));
            _stream = stream;
        }
        
        public IEnumerator<BrainfuckToken> GetEnumerator()
        {
            var token = _first;

            while (token != null)
            {
                yield return token;
                token = token.GetNext(_memoryTape);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Run()
        {
            foreach (var token in this)
            {
                token.Operate(_memoryTape,_stream);
            }
        }

        public static BrainfuckSteps Create(IEnumerable<BrainfuckToken> tokens, BrainfuckMemoryTape memoryTape,
            IBrainfuckStream stream)
        {
            return new BrainfuckSteps(tokens.FirstOrDefault(),memoryTape,stream);
        }
    }
}