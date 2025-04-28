using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyInEditorAttribute : Attribute { }
}
