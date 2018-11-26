﻿using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Aspects
{
    public interface IInterceptMethodAspect
    {
        void OnMethodInvoked(MethodContext context);
    }
}
