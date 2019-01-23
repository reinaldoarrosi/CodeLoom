using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptProperty
{
    public class IndexerPropertyAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            var arg0 = (int)context.Arguments[0];
            context.Proceed();

            if (arg0 == 2)
            {
                context.SetReturnValue(new Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>(
                    3, 
                    "c", 
                    new SimpleStruct(3), 
                    new SimpleClass(3), 
                    new InheritsFromSimpleClass(3),
                    new Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>(
                        4, 
                        "d", 
                        new SimpleStruct(4), 
                        new SimpleClass(4), 
                        new InheritsFromSimpleClass(4), 
                        null
                    )
                ));
            }
        }

        public void OnSet(PropertyContext context)
        {
            var arg0 = (int)context.Arguments[0];

            if(arg0 == 2)
            {
                context.Arguments.SetArgument(0, 3);
                context.Arguments.SetArgument(1, "c");
                context.Arguments.SetArgument(2, new SimpleStruct(3));
                context.Arguments.SetArgument(3, new SimpleClass(3));
                context.Arguments.SetArgument(4, new InheritsFromSimpleClass(3));
                context.Arguments.SetArgument(5, new Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>(
                    4, 
                    "d", 
                    new SimpleStruct(4), 
                    new SimpleClass(4), 
                    new InheritsFromSimpleClass(4), 
                    null
                ));
            }
            
            context.Proceed();
        }
    }
}
