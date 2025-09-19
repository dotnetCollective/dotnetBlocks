using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.System
{

    [TestClass]
    public class EnumMetaTests
    {

        enum MetaEnum
        { 
            [EnumMeta(Name = "Zero", Description = "Notany")]
            None=0,
            [EnumMeta(Name = "Single", Description = "alone")]
            One = 1,
            [EnumMeta(Name = "Duex", Description = "Notany")]
            Two = 2,
            [EnumMeta(Name = "Trio", Description = "many")]
            Three = 3,
            [EnumMeta()]
            Default=4
        }


        [TestMethod]
        public void meta_manager_meta_description_tests()
        {
            EnumMetaManager<MetaEnum>.GetName(MetaEnum.None).ShouldBe("Zero");
            EnumMetaManager<MetaEnum>.GetDescription(MetaEnum.None).ShouldBe("Notany");
            EnumMetaManager<MetaEnum>.GetDescription(MetaEnum.One).ShouldBe("alone");
            EnumMetaManager<MetaEnum>.GetDescription(MetaEnum.Two).ShouldBe("Notany");
            EnumMetaManager<MetaEnum>.GetDescription(MetaEnum.Three).ShouldBe("many");
            EnumMetaManager<MetaEnum>.GetDescription(MetaEnum.Default).ShouldBe(nameof(MetaEnum.Default));

        }

        [TestMethod]
        public void extension_meta_description_tests()
        {
            var value = MetaEnum.None;

            value.Name().ShouldBe("Zero");
            value = MetaEnum.None;
            value.Description().ShouldBe("Notany");
            value = MetaEnum.One;
            value.Description().ShouldBe("alone");

            value = MetaEnum.Two;
            value.Description().ShouldBe("Notany");

            value = MetaEnum.Three;

            value.Description().ShouldBe("many");
            value = MetaEnum.Default;
            value.Description().ShouldBe(nameof(MetaEnum.Default));
        }

        [TestMethod]
        public void attribute_indexer_tests()
        {
            foreach (var value in Enum.GetValues<MetaEnum>())
            {
                Should.NotThrow(() => EnumMetaManager<MetaEnum>.MetaData[value]);
                Should.NotThrow(() => EnumMetaManager<MetaEnum>.GetMeta(value));
                value.GetMeta().Value.ShouldBe(value);
            }

        }


    }
}