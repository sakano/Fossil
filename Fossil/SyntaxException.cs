using System;
using System.Runtime.Serialization;

namespace Fossil
{
    [Serializable]
    public class SyntaxException : Exception
    {
        public SyntaxException(int lineNumber)
            : base("[" + lineNumber + "] Syntax error")
        {
        }
    }
}
