using System;
using System.Collections.Generic;
using System.IO;
using Brainfuck.Net.Core.Tokens;

namespace Brainfuck.Net.Core
{
    public class BrainfuckParser
    {
        public BrainfuckTokenSequence Parse(TextReader streamReader, BrainfuckMemoryTape memoryTape = null)
        {
            memoryTape = memoryTape ?? new BrainfuckMemoryTape(); 
            
            var leftBracketStack = new Stack<LeftBracketToken>();
            
            BrainfuckToken first = null;
            BrainfuckToken prev = null;
            var index = -1;
                
            while (0 <= streamReader.Peek())
            {                
                ++index;

                var c = (char)streamReader.Read();

                BrainfuckToken current = null;
                switch (c)
                {
                    case '>':
                        current = new RightShiftToken();
                        break;
                    case '<':
                        current = new LeftShiftToken();
                        break;
                    case '+':
                        current = new PlusToken();
                        break;
                    case '-':
                        current = new MinusToken();
                        break;
                    case '.':
                        current = new DotToken();
                        break;
                    case ',':
                        current = new SemicolonToken();
                        break;
                    case '[':
                    {
                        var leftBracket = new LeftBracketToken();
                        leftBracketStack.Push(leftBracket);

                        current = leftBracket;
                    }
                        break;
                    case ']':
                    {
                        if (leftBracketStack.Count == 0)
                        {
                            throw new InvalidOperationException();
                        }

                        var leftBracket = leftBracketStack.Pop();
                        var rightBracket = new RightBracketToken();

                        leftBracket.RightBracketToken = rightBracket;
                        rightBracket.LeftBracketToken = leftBracket;

                        current = rightBracket;
                    }
                        break;
                    default:
                        break;
                }

                if(current == null)
                    continue;

                current.MemoryTape = memoryTape;
                current.Index = index;
                
                if (prev != null && prev.Next == null)
                {
                    prev.SetNextIndexToken(current);
                }
                
                prev = current;
                
                if(first != null)
                    continue;
                first = current;
            }
            
            if (leftBracketStack.Count != 0)
            {
                throw new InvalidOperationException();
            }

            return new BrainfuckTokenSequence(first);
        } 
    }

    
}