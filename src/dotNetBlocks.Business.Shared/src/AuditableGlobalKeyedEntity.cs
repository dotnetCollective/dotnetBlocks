using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared
{
    public class AuditableGlobalKeyedEntity<TId> : GlobalKeyedEntity<TId>, IAuditable
                where TId : struct, IEquatable<TId> // TODO: Convertible, IComparable

    {
        private readonly IAuditable _auditProperties = new AuditProperties(); // Default audit properties.

        public AuditableGlobalKeyedEntity( Guid id = default, IAuditable? auditProperties = default) : base(id)
         => _auditProperties = auditProperties ?? _auditProperties; // keep the default if nothing is provided.


        #region IAuditable
        public DateTimeOffset? CreatedAt { get => _auditProperties.CreatedAt; set => _auditProperties.CreatedAt = value; }
        public string? CreatedBy { get => _auditProperties.CreatedBy; set => _auditProperties.CreatedBy = value; }
        public DateTimeOffset? UpdatedAt { get => _auditProperties.UpdatedAt; set => _auditProperties.UpdatedAt = value; }
        public string? UpdatedBy { get => _auditProperties.UpdatedBy; set => _auditProperties.UpdatedBy = value; }

        #endregion IAuditable
    }
}
