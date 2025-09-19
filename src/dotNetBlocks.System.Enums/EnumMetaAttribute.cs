using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System

{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple =false)]
    public class EnumMetaAttribute : Attribute
    {
        /// <summary>
        /// Display Name for the enum value..
        /// </summary>
        public string? Name = null;
        /// <summary>
        /// Description of the enum.
        /// </summary>
        public string? Description = null;
    }

}
