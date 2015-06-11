//
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
//

using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Microsoft.Web.Redis.FunctionalTests
{
    internal class RedisSentinel : IDisposable
    {
        private List<ServerSetup> _servers;


        private readonly string  redisServerLocation = @"..\..\..\..\..\..\packages\Redis-64.2.8.19\redis-server.exe";

        private readonly string redisConfigLocation = @"..\..\..\..\..\..\Test\Configs\";

        public List<ServerSetup> Servers
        {
            get
            {
                return _servers;
            }
        }

        private static void WaitForServerToStart(int port)
        {
            for (var i = 0; i < 200; i++)
            {
                Thread.Sleep(10);
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect("localhost", port);
                    
                    socket.Close();
                    LogUtility.LogInfo("Successful started redis server after Time: {0} ms", (i + 1) * 10);
                    break;
                }
                catch
                {
                }
            }
        }

        public RedisSentinel()
        {
            KillRedisServers();
            SetUpServers();

            StartServers();
        }

        private void StartServers()
        {
            foreach (var serverSetup in _servers)
            {
                serverSetup.Server.Start();
                WaitForServerToStart(serverSetup.Port);
            }
        }

        // Make sure that no redis-server instance is running
        private void KillRedisServers()
        {
            foreach (var proc in Process.GetProcessesByName("redis-server"))
            {
                try
                {
                    proc.Kill();
                }
                catch
                {
                }
            }
        }

        public void Dispose()
        {
            foreach (var serverSetup in _servers)
            {
                try
                {
                    if (serverSetup.Server != null)
                    {
                        serverSetup.Server.Kill();
                    }
                }
                catch
                { }
            }
           
        }

        private void SetUpServers()
        {
            _servers = new List<ServerSetup>();
            var redis1Args = String.Format("{0}redis1.conf", redisConfigLocation);
            var redis2Args = String.Format("{0}redis2.conf", redisConfigLocation);
            var sentinel1Args = string.Format("{0}sentinel1.conf --sentinel", redisConfigLocation);
            var sentinel2Args = string.Format("{0}sentinel2.conf --sentinel", redisConfigLocation);
            var sentinel3Args = string.Format("{0}sentinel3.conf --sentinel", redisConfigLocation);
            _servers.Add(CreateServerSetup(ServerId.Redis1,  6379, redis1Args));
            _servers.Add(CreateServerSetup(ServerId.Redis2, 6380, redis2Args));
            _servers.Add(CreateServerSetup(ServerId.Sentinel1, 5000, sentinel1Args));
            _servers.Add(CreateServerSetup(ServerId.Sentinel2, 5001, sentinel2Args));
            _servers.Add(CreateServerSetup(ServerId.Sentinel3, 5002, sentinel3Args));

        }

        private ServerSetup CreateServerSetup(ServerId serverId,int port, string arguments)
        {
            var serverSetup = new ServerSetup();
            serverSetup.ServerId = serverId;
            serverSetup.Port = port;
            var server = new Process
                             {
                                 StartInfo =
                                     {
                                         FileName = redisServerLocation,
                                         Arguments = arguments,
                                         WindowStyle = ProcessWindowStyle.Normal
                         
                                     },
                                     
                             };
            serverSetup.Server = server;

            return serverSetup;
        }
    }

    public class ServerSetup
    {
        public int Port { get; set; }
        public ServerId ServerId { get; set; }
        public Process Server { get; set; }
    }

    public enum ServerId
    {
        Redis1,
        Redis2,
        Sentinel1,
        Sentinel2,
        Sentinel3
    }
   
}
