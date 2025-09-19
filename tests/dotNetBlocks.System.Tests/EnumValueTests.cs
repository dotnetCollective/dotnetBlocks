using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.System
{

    [TestClass]
    public class EnumValueTests
    {

        enum ValueTesting
        { 
            None=0,
            One=1,
            Two=2,
            Three=3
        }


        [TestMethod]
        public void min_max_tests()
        {

            EnumMetaManager<ValueTesting>.Min.ShouldBe(ValueTesting.None);
            EnumMetaManager<ValueTesting>.Max.ShouldBe(ValueTesting.Three);

            EnumMetaManager<ValueTesting>.Length.ShouldBe(4);

        }

        [TestMethod]
        public void min_max_extension_tests()
        {
            var value = ValueTesting.None;

            value.Min().ShouldBe(ValueTesting.None);
            value.Max().ShouldBe(ValueTesting.Three);

            value.Length().ShouldBe(4);

        }

        [TestMethod]
        public void test_default_meta()
        {
            foreach (var value in Enum.GetValues<ValueTesting>())
            {
                value.Name().ShouldBe(value.GetMeta().Name);
                value.Name().ShouldBe(value.GetMeta().Description);
                value.GetMeta().Value.ShouldBe(value);
            }
        }

    }
}