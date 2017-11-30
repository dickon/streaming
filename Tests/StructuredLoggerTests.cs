using System;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using static Lib.StructuredLogger;

namespace Tests
{
    public class StructuredLoggerTests
    {
        [Fact]
        public async Task TestLogger() {
            var seq = new List<string>();
            SetupRecording(async x=>seq.Add(x));
            await StartSection("alpha");
            await Announce("hello");
            await EndSection();
            var text = string.Concat(seq);
            Assert.Equal("<div class=\"section\">\n <div class=\"heading\">alpha</div>\n    <div class=\"log\">hello</div>\n</div>\n", text);
        }

    }
}