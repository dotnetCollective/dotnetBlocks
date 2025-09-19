using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace System
{
    public class EnumMeta<TEnum>
        where TEnum : struct, Enum
    {
        public TEnum Value;
        public string? Name;
        public string? Description;
        public FieldInfo? FieldInfo;
    }

}