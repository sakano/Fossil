using System.Diagnostics.Contracts;

namespace Fossil.AbstractSyntaxTree
{
    [ContractClass(typeof(ContractForINode))]
    internal interface INode
    {
        Variant eval();
    }

    [ContractClassFor(typeof(INode))]
    internal abstract class ContractForINode : INode
    {
        public Variant eval()
        {
            Contract.Ensures(Contract.Result<Variant>() != null);
            return default(Variant);
        }
    }
}
