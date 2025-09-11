using Shouldly;
using System.Collections;

namespace dotNetBlocks.System.Collections.Tests
{

    public enum TestEnum
    {
        None = 0,
        Bit1 = 1,
        Bit2 = 2,
        Bit3 = 4,
    }

    [TestClass] [TestCategory("System.Collections.EnumBitArray")]
    public sealed class EnumBitArrayTests
    {

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

        [TestMethod]
        public void convert_from_enum_sets_flag()
        {
            EnumBitArray<TestEnum> enumArray;

            enumArray = TestEnum.None; // zero'th bit
            enumArray.HasAnySet().ShouldBeTrue();
            enumArray[TestEnum.None].ShouldBeTrue();
            enumArray.ShouldBe(TestEnum.None);


            enumArray = TestEnum.Bit1;
            enumArray.HasAnySet().ShouldBeTrue();
            enumArray[TestEnum.Bit1].ShouldBeTrue();
            enumArray[TestEnum.Bit2].ShouldBeFalse();
            enumArray = TestEnum.Bit2;
            enumArray[TestEnum.Bit2].ShouldBeTrue();
            enumArray[TestEnum.Bit1].ShouldBeFalse();
        }


    }
}
