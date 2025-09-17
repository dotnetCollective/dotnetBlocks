using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RT.Comb;

namespace System
{

    /// <summary>
    /// Types of Comb Guid providers available.
    /// </summary>
    public enum GuidTypes
    {
        /// <summary>
        /// Legacy CombGuid 
        /// </summary>
        CombGuid,
        /// <summary>
        /// SQL server optimized (Legacy)
        /// </summary>
        SqlServer,
        /// <summary>
        /// SQLlite optimized (unix)
        /// </summary>
        SqlLite,
        /// <summary>
        /// Oracle optimized (unix)
        /// </summary>
        Oracle,
        /// <summary>
        /// PosGreSQL optimized
        /// </summary>
        PosGreSQL,
        /// <summary>
        /// Unix optimized
        /// </summary>
        Unix,
    }

    /// <summary>
    /// Extends Guid
    /// Adds semi-sequential CombGuid generation methods.
    /// Use Case: Generating new ComGuid <see cref="Guid"/> with time component for minimal database impact.
    /// </summary>
    /// <remarks>Uses the https://github.com/richardtallent/RT.Comb library to generate CombGuids for performance purposes.
    /// Implemented the most common types as extensions.
    /// User can look at the library for other use cases and additional functionality.
    /// </remarks>
    public static class GuidExtensions
    {


        /// <summary>
        /// Initializes the extended unique identifier.
        /// </summary>
        /// <param name="guid"><see cref="Guid> Value to extend and initialize."/></param>
        /// <returns>New <see cref="Guid"/> generated and stored.</returns>
        public static Guid Initialize(ref this Guid guid) => guid.Initialize(default);
        /// <summary>
        /// Initializes the extended unique identifier.
        /// </summary>
        /// <param name="guid">The extended unique identifier to initialize.</param>
        /// <param name="guidType"></param> <b></b>Optional</b> <see cref="GuidTypes"/> Type of the unique identifier and provider algorithm to use.</param>
        /// <returns></returns>
        public static Guid Initialize(ref this Guid guid, GuidTypes? guidType = default) => guid = GenerateId(guidType);


        public static Guid NewId(this Guid _, GuidTypes? guidType = default) => GenerateId(guidType);

        public static Guid NewId(this Guid _) => NewId();
        public static Guid NewId() => GenerateId(null);
        public static Guid GenerateId(this Guid _) => GenerateId(null);

        /// <summary>
        /// Generates a new <see cref=" Guid"/> using one of the CombGuid algorithms.
        /// </summary>
        /// <param name="_">Discarded extension parameter</param>
        /// <param name="guidType">optional <see cref="GuidTypes"/> type of guid to generate</param>
        /// <returns></returns>
        /// <see cref="GenerateId(GuidTypes?)"/>
        public static Guid GenerateId(this Guid _, GuidTypes? guidType = default) => GenerateId(guidType);


        /// <summary>
        /// Generates the CombGuid using the provider <see cref="GuidTypes"/> indicated.
        /// </summary>
        /// <param name="guidType"> guid provider <see cref="GuidTypes"/> to use. defaults to CombGuid. "/>  </param>
        /// <returns> New <see cref="Guid"/> optimized for the type of Guid requesting</returns>
        /// <remarks></remarks> Defaults to a CombGuid algorithm. </remarks> 
        /// 
        public static Guid GenerateId(GuidTypes? guidType = GuidTypes.CombGuid)
        {
            guidType ??= GuidTypes.CombGuid;
            return guidType switch
               {
                   GuidTypes.CombGuid => Provider.Legacy.Create(), // SQL Server
                   GuidTypes.SqlServer => Provider.Sql.Create(),
                   GuidTypes.SqlLite => Provider.SqliteBinary.Create(),
                   GuidTypes.Oracle => Provider.Oracle.Create(),
                   GuidTypes.PosGreSQL => Provider.PostgreSql.Create(),
                   GuidTypes.Unix => Provider.Sql.Create(),
                   _ => Guid.NewGuid(), // Default to guid.
               };
        }



    }
}
