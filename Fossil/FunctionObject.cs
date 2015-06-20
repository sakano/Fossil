using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    class FunctionObject : ICallableObject
    {
        public FunctionObject(IdentifierToken funcName, List<IdentifierNode> argNames, BlockNode body)
        {
            Contract.Requires<ArgumentNullException>(funcName != null);
            Contract.Requires<ArgumentNullException>(argNames != null);
            Contract.Requires<ArgumentNullException>(body != null);
            this.funcName = funcName;
            this.argNames = argNames;
            this.body = body;
        }

        public Variant Call(Environment env, List<INode> parameters)
        {
            Environment newEnv = new Environment(env);
            for (var i = 0; i < argNames.Count; ++i) {
                Contract.Assume(argNames[i] != null);
                if (parameters.Count <= i) {
                    var assigned = newEnv.AssignNew(argNames[i].Name, new Variant());
                    Contract.Assume(assigned);
                } else {
                    Contract.Assume(parameters[i] != null);
                    var assigned = newEnv.AssignNew(argNames[i].Name, parameters[i].Eval(env));
                    Contract.Assume(assigned);
                }
            }
            return body.EvalWithoutNewEnvironment(newEnv);
        }

        public string Name
        {
            get
            {
                return funcName.Value;
            }
        }

        private readonly IdentifierToken funcName;
        private readonly List<IdentifierNode> argNames;
        private readonly BlockNode body;

        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return string.Format("[function]{0}({1})", funcName.Value, string.Join(", ", argNames.Select((arg) => arg.Name)));
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.funcName != null);
            Contract.Invariant(this.argNames != null);
            Contract.Invariant(this.body != null);
        }
    }
}
