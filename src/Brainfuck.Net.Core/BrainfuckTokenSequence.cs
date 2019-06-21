using System.Collections;
using System.Collections.Generic;
using Brainfuck.Net.Core.Tokens;

namespace Brainfuck.Net.Core
{
    public class BrainfuckTokenSequence:IEnumerable<BrainfuckToken>
    {
        private readonly BrainfuckToken _first;

        internal BrainfuckTokenSequence(BrainfuckToken first)
        {
            _first = first;
        }
        
        public IEnumerator<BrainfuckToken> GetEnumerator()
        {
            var token = _first;

            while (token != null)
            {
                yield return token;
                token = token.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}