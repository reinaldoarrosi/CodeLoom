using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom
{
    [System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class CodeLoomSetupAttribute : Attribute
    {
        public CodeLoomSetupAttribute(Type setupType)
        {
            if (setupType == null)
                throw new ArgumentNullException(nameof(setupType));

            if (!typeof(CodeLoomSetup).IsAssignableFrom(setupType))
                throw new ArgumentException($"{nameof(setupType)} must inherit from {nameof(CodeLoomSetup)}");

            SetupType = setupType;
        }

        public Type SetupType { get; private set; }
    }
}
