using CodeLoom.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Aspects
{
    public interface IAspect
    {
        void Execute(Invocation invocation);
    }
}
