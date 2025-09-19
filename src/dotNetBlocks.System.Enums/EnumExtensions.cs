namespace System
{
    public static class EnumExtensions
    {


        public static TEnum Min<TEnum>(this TEnum _)
            where TEnum : struct, Enum
        => EnumMetaManager<TEnum>.Min;

        public static TEnum Max<TEnum>(this TEnum _)
            where TEnum : struct, Enum
        => EnumMetaManager<TEnum>.Max;

        public static Int32 Length<TEnum>(this TEnum _)
            where TEnum : struct, Enum
        => EnumMetaManager<TEnum>.Length;


        public static string? Name<TEnum>(this TEnum enumItem)
            where TEnum : struct, Enum
            => EnumMetaManager<TEnum>.GetName(enumItem);

        public static string? Description<TEnum>(this TEnum enumItem)
            where TEnum : struct, Enum
            => EnumMetaManager<TEnum>.GetDescription(enumItem);

        public static EnumMeta<TEnum> GetMeta<TEnum>(this TEnum enumItem)
            where TEnum : struct, Enum
        => EnumMetaManager<TEnum>.GetMeta(enumItem);
    }
}
