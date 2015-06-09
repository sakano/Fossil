using System;
using System.Runtime.Serialization;

namespace Fossil
{
    [Serializable]
    public class RuntimeException : Exception
    {
        public RuntimeException(int lineNumber, string message)
            : base("[" + lineNumber + "] " + message)
        {
        }
    }
}
