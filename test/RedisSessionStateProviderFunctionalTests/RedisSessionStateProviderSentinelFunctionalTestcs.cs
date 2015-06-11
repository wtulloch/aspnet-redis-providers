using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xunit;

namespace Microsoft.Web.Redis.FunctionalTests
{
    public class RedisSessionStateProviderSentinelFunctionalTestcs
    {
        [Fact]
        public void DummyTest()
        {
            using (RedisSentinel sentinel = new RedisSentinel())
            {
                Debugger.Break();
            }
            
            Assert.True(true);
        }
    }
}
