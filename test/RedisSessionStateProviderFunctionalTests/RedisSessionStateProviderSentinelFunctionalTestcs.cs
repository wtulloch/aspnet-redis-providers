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
                var serverToClose =sentinel.Servers.FirstOrDefault(su => su.ServerId == ServerId.Redis1);
                if (serverToClose != null)
                {
                    try
                    {
                        serverToClose.Server.Kill();
                    }
                    catch (Exception e)
                    {
                        var er = e.Message;
                       Debugger.Break();
                    }
                   
                }
                
                Debugger.Break();
            }
            
            Assert.True(true);
        }
    }
}
