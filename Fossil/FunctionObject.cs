using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    class FunctionObject
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
            Contract.Requires<ArgumentNullException>(env != null);
            Contract.Requires<ArgumentNullException>(parameters != null);
            Contract.Ensures(Contract.Result<Variant>() != null);
            Environment newEnv = new Environment(env);
            for (var i = 0; i < argNames.Count; ++i) {
                Contract.Assume(argNames[i] != null);
                if (parameters.Count <= i) {
                    if (!newEnv.AssignNew(argNames[i].Name, new Variant())) {
                        throw new RuntimeException(funcName.LineNumber, "variable already defined");
                    }
                } else {
                    Contract.Assume(parameters[i] != null);
                    if (!newEnv.AssignNew(argNames[i].Name, parameters[i].Eval(env))) {
                        throw new RuntimeException(funcName.LineNumber, "variable already defined");
                    }
                }
            }
            return body.EvalWithoutNewEnvironment(newEnv);
        }

        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
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
