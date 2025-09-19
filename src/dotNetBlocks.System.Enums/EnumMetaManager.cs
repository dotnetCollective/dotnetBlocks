using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace System
{



    /// <summary>
    /// Provides meta data information about Enums classes.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <remarks> Relies on generic closure to maintain internal information on each enum built on demand.</remarks>
    public static class EnumMetaManager<TEnum>
            where TEnum : struct, Enum
    {

        private static readonly Lazy<IDictionary<TEnum, EnumMeta<TEnum>>> _metaData = new Lazy<IDictionary<TEnum, EnumMeta<TEnum>>>(DiscoverMetaData);
        public static IDictionary<TEnum, EnumMeta<TEnum>> MetaData => _metaData.Value;


        /// <summary>
        /// Discovers the meta data.
        /// </summary>
        /// <remarks> called at first access of the meta data information.</remarks>
        private static IDictionary<TEnum, EnumMeta<TEnum>> DiscoverMetaData()
        {
            Type enumType = typeof(TEnum);

            return
                (from e in Enum.GetValues<TEnum>() // Iterate through the enum value which are actually members.
                 let name = Enum.GetName(e)        // We need the name for further actions
                 let fi = enumType.GetField(name!) // Get the field information to lookup the member.
                 let meta = fi?.GetCustomAttribute<EnumMetaAttribute>()
                 select new EnumMeta<TEnum>()
                 {
                     Value = e,
                     Name = meta?.Name ?? name,
                     Description = meta?.Description ?? name,
                     FieldInfo = fi,
                 }).ToDictionary(k => k.Value);

        }

        public static int Length => MetaData.Count();


        private static TEnum? _max = null;
        private static TEnum? _min = null;
        public static TEnum Max
            => _max ??= MetaData.Keys.Max();

        public static TEnum Min
            => _min  ??= MetaData.Keys.Min();

        public static EnumMeta<TEnum> GetMeta(TEnum value) => MetaData[value];

        public static IEnumerable<EnumMeta<TEnum>> GetMeta() => MetaData.Values;

        public static string? GetName(TEnum value) => MetaData[value].Name;
        public static string? GetDescription(TEnum value) => MetaData[value].Description;

    }
}
    