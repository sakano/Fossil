using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    internal class VoidNode : INode
    {
        public VoidNode()
        {
        }

        public Variant Eval(Environment env)
        {
            return new Variant();
        }
    }
}
