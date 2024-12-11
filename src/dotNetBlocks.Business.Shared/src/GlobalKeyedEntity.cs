using RT.Comb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared
{
    /// <summary>
    /// Keyed entity with GlobalId <see cref=" Guid"/> for a dual key strategy.
    /// </summary>
    /// <seealso cref="dotNetBlocks.Business.Shared.KeyedEntity{TId}" />
    public class GlobalKeyedEntity<TId> : KeyedEntity<TId>
                where TId : struct, IEquatable<TId> // TODO: Convertible, IComparable

    {


        public GlobalKeyedEntity(Guid GlobalId = default, TId id = default) : base(id)
        {
            GlobalID = GlobalId;
        }

        /// <summary>
        /// GlobalId Guid Identifier for this business object.
        /// </summary>
        /// <value>
        /// The global identifier.
        /// </value>
        public virtual Guid GlobalID { get; set; }

        public virtual Guid NewGlobalId() => RT.Comb.Provider.Legacy.Create(); // Default implementation in Guid is the Comb Guid which creates sequential unique guids.


    }
}
