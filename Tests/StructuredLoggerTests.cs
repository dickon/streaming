using System;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lib;

namespace Tests
{
    public class StructuredLoggerTests
    {
        [Fact]
        public async Task TestLogger() {
            var seq = new List<string>();
            var sl = new StructuredLogger(x=> {
                seq.Add(x);
                return Task.FromResult<object>(null);
            });
            await sl.StartSection("alpha");
            await sl.Announce("hello");
            await sl.EndSection();
            var text = string.Concat(seq);
            Assert.Equal("<div class=\"section\">\n <div class=\"heading\">alpha</div>\n    <div class=\"log\">hello</div>\n</div>\n", text);
        }

    }
}