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
        public Environment(Environment parentEnvironment)
        {
            this.parentEnvironment = parentEnvironment;
        }

        public void Assign(string name, Variant variant)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentException>(name.Length != 0);
            Contract.Requires<ArgumentNullException>(variant != null);
            if (!AssignIfExist(name, variant)) {
                variables.Add(name, variant);
            }
        }

        public bool AssignIfExist(string name, Variant variant)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentException>(name.Length != 0);
            Contract.Requires<ArgumentNullException>(variant != null);
            if (variables.ContainsKey(name)) {
                variables[name] = variant;
                return true;
            } else if (parentEnvironment != null) {
                return parentEnvironment.AssignIfExist(name, variant);
            }
            return false;
        }

        public Variant Get(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentException>(name.Length != 0);
            if (variables.ContainsKey(name)) {
                return variables[name];
            } else {
                if (parentEnvironment != null) {
                    var value = parentEnvironment.Get(name);
                    if (value != null) { return value; }
                }
                return null;
            }
        }

        private readonly Dictionary<string, Variant> variables = new Dictionary<string, Variant>();
        private readonly Environment parentEnvironment;
    }
}
