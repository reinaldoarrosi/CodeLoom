﻿using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptYieldMethod
{
    public class ReplaceEmptyEnumerableWithANonEmptyEnumerableAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();
            context.SetReturnValue(new[] { DateTime.Now, DateTime.Today });
        }
    }
}
