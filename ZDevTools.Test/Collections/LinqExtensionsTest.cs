using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using ZDevTools.Collections;

namespace ZDevTools.Test.Collections
{
    public class LinqExtensionsTest
    {
        [Fact]
        public void WhereSelectTest()
        {
            string[] strings = ["a", "b", "c"];

            Assert.Equal(["b娃", "c娃"], strings.WhereSelect(s => (s != "a", s + "娃")));

            Assert.Equal(["c娃"], strings.WhereSelect((s, i) => (s != "a" && i > 1, s + "娃")));
        }

    }
}
