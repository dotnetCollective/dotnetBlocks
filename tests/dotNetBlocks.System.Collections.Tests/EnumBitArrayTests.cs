using Shouldly;
using System.Collections;

namespace dotNetBlocks.System.Collections.Tests
{
    [TestClass]
    public sealed class EnumBitArrayTests
    {
        private enum TestEnum
        {
            None = 0,
            Bit1 = 1,
            bit2 = 2,
            Bit3 = 4,
        }

        [TestMethod]
        public void bit_array_none_only_has_no_flags_set()
        {
            var enumArray = new EnumBitArray<TestEnum>();

            enumArray.HasAnySet().ShouldBeFalse();
        }

        [TestMethod]
        public void set_bit_and_validate_only_sets_one_bit()
        {
            var enumArray = new EnumBitArray<TestEnum>();

            // set flag for bit1
            enumArray.Set(TestEnum.Bit1);

            enumArray[TestEnum.Bit1].ShouldBeTrue();

            // Only bit 1 is set.
            enumArray.HasOtherFlags(TestEnum.Bit1).ShouldBeFalse();
        }

        [TestMethod]
        public void set_bit_using_indexer_and_verify()
        {
            var enumArray = new EnumBitArray<TestEnum>();
            // set flag for bit1
            enumArray[TestEnum.Bit1] = true;
            enumArray[TestEnum.Bit1].ShouldBeTrue();
            // Only bit 1 is set.
            enumArray.HasOtherFlags(TestEnum.Bit1).ShouldBeFalse();
        }

        [TestMethod]
        public void set_bit_using_method_and_verify()
        {
            var enumArray = new EnumBitArray<TestEnum>();
            // set flag for bit1
            enumArray.Set(TestEnum.Bit1);
            enumArray[TestEnum.Bit1].ShouldBeTrue();
            // Only bit 1 is set.
            enumArray.HasOtherFlags(TestEnum.Bit1).ShouldBeFalse();
        }


    }
}
