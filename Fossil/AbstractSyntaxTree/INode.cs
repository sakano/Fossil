using System;
using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    [ContractClass(typeof(ContractForINode))]
    internal interface INode
    {
        Variant Eval(Environment env);
    }

    [ContractClassFor(typeof(INode))]
    internal abstract class ContractForINode : INode
    {
        public Variant Eval(Environment env)
        {
            Contract.Requires<ArgumentNullException>(env != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return default(Variant);
        }
    }
}
