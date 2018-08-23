using CodeLoom.Aspects;
using CodeLoom.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Fody.Tests
{
    public class TestAspect : IAspect
    {
        private Action<Invocation> _executor;

        public TestAspect(Action<Invocation> executor)
        {
            _executor = executor;
        }

        public void Execute(Invocation invocation)
        {
            _executor?.Invoke(invocation);
        }
    }
}
