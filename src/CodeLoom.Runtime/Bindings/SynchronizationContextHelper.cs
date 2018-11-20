using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeLoom.Bindings
{
    public class SynchronizationContextHelper
    {
        public static TaskScheduler GetSynchronizationContext()
        {
            if (SynchronizationContext.Current != null)
            {
                return TaskScheduler.FromCurrentSynchronizationContext();
            }
            else
            {
                return TaskScheduler.Current;
            }
        }
    }
}
