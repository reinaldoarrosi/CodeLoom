using CodeLoom.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Aspects
{
    public class BoundariesAspectContext
    {
        public enum Flows
        {
            Continue = 0,
            Return = 1,
            Throw = 2
        }

        private Invocation _invocation;

        public BoundariesAspectContext(Invocation invocation)
        {
            _invocation = invocation;
            Flow = Flows.Continue;
        }

        public InvocationSource Source
        {
            get { return _invocation.Source; }
        }

        public Arguments Arguments
        {
            get { return _invocation.Arguments; }
        }

        public object ReturnValue
        {
            get { return _invocation.ReturnValue; }
            set { _invocation.ReturnValue = value; }
        }

        public Exception Exception
        {
            get;
            set;
        }

        public Flows Flow
        {
            get;
            set;
        }
    }
}
