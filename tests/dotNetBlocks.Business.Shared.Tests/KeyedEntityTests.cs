using dotNetBlocks.Business.Shared;

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
            testAuditProperties.CreatedAt.ShouldBe(_auditProperties.CreatedAt);
            testAuditProperties.CreatedBy.ShouldBe(_auditProperties.CreatedBy);

            testAuditProperties.UpdatedAt.ShouldBe(_auditProperties.UpdatedAt);
            testAuditProperties.UpdatedBy.ShouldBe(_auditProperties.UpdatedBy);
        }
    };
}