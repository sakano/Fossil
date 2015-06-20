using System;
using Fossil.AbstractSyntaxTree;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Fossil
{
    [ContractClass(typeof(ContractForICallable))]
    internal interface ICallableObject
    {
        Variant Call(Environment env, List<INode> parameters);
        string Name { get; }
    }

    [ContractClassFor(typeof(ICallableObject))]
    internal abstract class ContractForICallable : ICallableObject
    {
        public Variant Call(Environment env, List<INode> parameters)
        {
            Contract.Requires<ArgumentNullException>(env != null);
            Contract.Requires<ArgumentNullException>(parameters != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            return default(Variant);
        }

        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return default(string);
            }
        }
    }
}
