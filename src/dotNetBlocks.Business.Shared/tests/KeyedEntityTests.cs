using dotNetBlocks.Business.Shared;
using FluentAssertions;

namespace dotNetBlocks.Business.Shared.Tests
{


    [TestClass]
    public class KeyedEntityTests
    {

        private readonly AuditProperties _auditProperties = new AuditProperties().populateAuditProperties();


        [TestMethod]
        public void TestAuditProperties()
        {
            // Create a new audit properties and compare the values.
            AuditProperties testAuditProperties = new AuditProperties().populateAuditProperties();

            // Validate the properties match.
            testAuditProperties.CreatedAt.Should().Be(_auditProperties.CreatedAt);
            testAuditProperties.CreatedBy.Should().Be(_auditProperties.CreatedBy);

            testAuditProperties.UpdatedAt.Should().Be(_auditProperties.UpdatedAt);
            testAuditProperties.UpdatedBy.Should().Be(_auditProperties.UpdatedBy);
        }
    };
}