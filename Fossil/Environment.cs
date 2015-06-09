using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace Fossil
{
    class Environment
    {
        public Environment()
        {
        }

        public void Assign(string name, Variant variant)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentException>(name.Length != 0);
            Contract.Requires<ArgumentNullException>(variant != null);
            if (variables.ContainsKey(name)) {
                variables[name] = variant;
            } else {
                variables.Add(name, variant);
            }
        }

        public Variant Get(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentException>(name.Length != 0);
            if (variables.ContainsKey(name)) {
                return variables[name];
            } else {
                return null;
            }
        }

        private readonly Dictionary<string, Variant> variables = new Dictionary<string, Variant>();
    }
}
