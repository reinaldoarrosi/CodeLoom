using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Core
{
    public class Invocation
    {
        private Action<Invocation> _proceedExecutor;

        public Invocation(InvocationSource source, Argument[] arguments, Action<Invocation> proceedExecutor)
        {
            _proceedExecutor = proceedExecutor;

            Source = source;
            Arguments = new Arguments(arguments);
        }

        public InvocationSource Source { get; private set; }
        public Arguments Arguments { get; private set; }
        public object ReturnValue { get; set; }

        public virtual void Proceed()
        {
            _proceedExecutor(this);
        }
    }
}
