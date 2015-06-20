using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fossil.AbstractSyntaxTree;

namespace Fossil
{
    class NativeFunctionObject : ICallableObject
    {
        
        public NativeFunctionObject(string funcName, NativeFunction nativeFunction)
        {
            Contract.Requires<ArgumentNullException>(funcName != null);
            Contract.Requires<ArgumentNullException>(nativeFunction != null);
            this.funcName = funcName;
            this.nativeFunction = nativeFunction;
        }
        
        public Variant Call(Environment env, List<INode> parameters)
        {
            List<Variant> evaledParameters = parameters.Select(node => node.Eval(env)).ToList();
            Variant result = nativeFunction.Invoke(evaledParameters);
            if (result == null) { return new Variant(); }
            return result;
        }

        public string Name { get { return funcName; } }
        
        public delegate Variant NativeFunction(List<Variant> args);
        private readonly NativeFunction nativeFunction;
        private readonly string funcName;
    }
}
