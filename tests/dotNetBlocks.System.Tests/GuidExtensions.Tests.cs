
using Shouldly;

namespace dotNetBlocks.System.Tests

{
    [TestClass]
    public sealed class GuidExtensionTests
    {

        [TestMethod]
        public void initialize_with_comb_guid()
        {

            Guid guid1 = new Guid();
            guid1.ShouldBeEquivalentTo(Guid.Empty);

            var guid2 = guid1.Initialize();

            // Validate guid initialized propertly and returned the value correctly.

            guid1.ShouldNotBe(Guid.Empty);
            guid2.ShouldNotBe(Guid.Empty);
            guid2.ShouldBe(guid1);

            guid2.Initialize(GuidTypes.Unix);
            guid2.ShouldNotBe(guid1);
            guid2.ShouldNotBe(Guid.Empty);


            // Check that we initialize with different values each call.
            guid1 = guid2;
            guid2.Initialize(GuidTypes.Unix);
            guid2.ShouldNotBe(Guid.Empty);
            guid2.ShouldNotBe(guid1);


            // Test methods that don't change the value.
            guid1 = guid2.NewId();
            guid1.ShouldNotBe(guid2);
            guid1.ShouldNotBe(Guid.Empty);
            guid2.ShouldNotBe(Guid.Empty);

            guid2 = guid1;

            _ = guid2.NewId(GuidTypes.Unix);

            guid1.ShouldBe(guid2);
            guid2.ShouldBe(guid1);


        }


    }
}