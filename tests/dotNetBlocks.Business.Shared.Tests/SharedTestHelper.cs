using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Business.Shared.Tests
{
    public static class SharedTestHelper
    {
        static  DateTimeOffset createdAt = DateTimeOffset.Now;
            static string createdBy = "cTheron";
            static DateTimeOffset updatedAt = DateTimeOffset.Now;
            static string updatedBy = "bHill";


        public static AuditProperties populateAuditProperties(this AuditProperties auditProperties)
        {
            auditProperties.CreatedAt = createdAt;
            auditProperties.CreatedBy = createdBy;
            auditProperties.UpdatedAt = updatedAt;
            auditProperties.UpdatedBy = updatedBy;
            return auditProperties;
        }
    }
}