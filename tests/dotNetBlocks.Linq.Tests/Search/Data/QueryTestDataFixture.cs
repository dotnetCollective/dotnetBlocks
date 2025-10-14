using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace dotNetBlocks.Linq.Tests.Search.Data
{
    public class QueryTestDataFixture : IDisposable
    {
        public QueryTestDataFixture()
        {
            BuildDatabase();
        }

        /// <summary>
        /// Used to ensure that only one test acceses the database at a time.
        /// </summary>
        /// <value>
        /// <see cref="SemaphoreSlim"/> instance."/>  initialized for one thread only..
        /// </value>
        public SemaphoreSlim ThreadLock { get; } = new SemaphoreSlim(1, 1);

        public SearchItemContext Db { get; set; }

        [MemberNotNull(nameof(Db))]
        public void BuildDatabase()
        {
            var options = new DbContextOptionsBuilder<SearchItemContext>()
                . UseInMemoryDatabase(databaseName: "QueryTestData")
                .Options;

            Db = new SearchItemContext(options);

            Db.AddRange(
                new SearchItem[]
                {
                    new SearchItem{ Field1 = null, Field2 = null
                        , Child = new SearchChildItem{ ChildField1 = null, ChildField2 = null } 
                    },
                    new SearchItem{ Field1 = null, Field2 = "xyz"
                        , Child = new SearchChildItem{ ChildField1 = null, ChildField2 = "xyz" }
                    },
                    new SearchItem{ Field1 = "abc", Field2 = null
                        , Child = new SearchChildItem{ ChildField1 = "abc", ChildField2 = null }
                    },
                    new SearchItem{ Field1 = "abc", Field2 = "def"
                        , Child = new SearchChildItem{ ChildField1 = "abc", ChildField2 = "def" }
                    },
                    new SearchItem{ Field1 = "habc", Field2 = "def"
                        , Child = new SearchChildItem{ ChildField1 = "habc", ChildField2 = "def" }
                    },
                    new SearchItem{ Field1 = "ahbc", Field2 = "def"
                        , Child = new SearchChildItem{ ChildField1 = "ahbc", ChildField2 = "def" }
                    },
                    new SearchItem{ Field1 = "abch", Field2 = "def"
                        , Child = new SearchChildItem{ ChildField1 = "abch", ChildField2 = "def" }
                    },
                    new SearchItem{ Field1 = "abc", Field2 = "hdef"
                        , Child = new SearchChildItem{ ChildField1 = "abc", ChildField2 = "hdef" }
                    },
                    new SearchItem{ Field1 = "abc", Field2 = "dhef"
                        , Child = new SearchChildItem{ ChildField1 = "abc", ChildField2 = "dhef" }
                    },
                    new SearchItem{ Field1 = "abc", Field2 = "defh"
                        , Child = new SearchChildItem{ ChildField1 = "abc", ChildField2 = "defh" }
                    },
                    new SearchItem{ Field1 = "uvw", Field2 = "xyz"
                        , Child = new SearchChildItem{ ChildField1 = "uvw", ChildField2 = "xyz" }
                    },
                    new SearchItem{ Field1 = "huvw", Field2 = "xyz"
                        , Child = new SearchChildItem{ ChildField1 = "huvw", ChildField2 = "xyz" }
                    },
                    new SearchItem{ Field1 = "uhvw", Field2 = "xyz"
                        , Child = new SearchChildItem{ ChildField1 = "uhvw", ChildField2 = "xyz" }
                    },
                    new SearchItem{ Field1 = "uvwh", Field2 = "xyz"
                        , Child = new SearchChildItem{ ChildField1 = "uvwh", ChildField2 = "xyz" }
                    },
                    new SearchItem{ Field1 = "uvw", Field2 = "hxyz"
                        , Child = new SearchChildItem{ ChildField1 = "uvw", ChildField2 = "hxyz" }
                    },
                    new SearchItem{ Field1 = "uvw", Field2 = "xhyz"
                        , Child = new SearchChildItem{ ChildField1 = "uvw", ChildField2 = "xhyz" }
                    },
                    new SearchItem{ Field1 = "uvw", Field2 = "xyzh"
                        , Child = new SearchChildItem{ ChildField1 = "uvw", ChildField2 = "xyzh" }
                    }
                }
            );

            Db.SaveChanges();

        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
