﻿using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrleansSiloHost
{
    public class OrleansHostWrapper
    {
        private readonly SiloHost siloHost;

        public OrleansHostWrapper(ClusterConfiguration config, string[] args)
        {
            var siloArgs = SiloArgs.ParseArguments(args);
            if (siloArgs == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(siloArgs.DeploymentId))
            {
                config.Globals.DeploymentId = siloArgs.DeploymentId;
            }

            siloHost = new SiloHost(siloArgs.SiloName, config);
            siloHost.LoadOrleansConfig();
        }

        public int Run()
        {
            if (siloHost == null)
            {
                SiloArgs.PrintUsage();
                return 1;
            }
            try
            {
                siloHost.InitializeOrleansSilo();
                if (siloHost.StartOrleansSilo())
                {
                    Console.WriteLine($"Orleans solo 启动成功 '{siloHost.Name}' 作为一个 {siloHost.Type} 节点.");
                    return 0;
                }
                else
                {
                    throw new OrleansException($"Orleans solo 启动失败 '{siloHost.Name}' 作为一个 {siloHost.Type} 节点.");
                }
            }
            catch (Exception ex)
            {
                siloHost.ReportStartupError(ex);
                Console.Error.WriteLine(ex);
                return -1;
            }

        }

        public int Stop()
        {
            if (siloHost != null)
            {
                try
                {
                    siloHost.StopOrleansSilo();
                    siloHost.Dispose();
                    Console.WriteLine($"Orleans silo '{siloHost.Name}' 已成功关闭.");
                }
                catch (Exception exc)
                {
                    siloHost.ReportStartupError(exc);
                    Console.Error.WriteLine(exc);
                    return 1;
                }
            }
            return 0;
        }

        public class SiloArgs
        {
            public string SiloName { get; set; }
            public string DeploymentId { get; set; }

            public SiloArgs(string siloName, string deploymentId)
            {
                this.DeploymentId = deploymentId;
                this.SiloName = siloName;
            }

            public static SiloArgs ParseArguments(string[] args)
            {
                string deploymentId = null;
                string siloName = null;

                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];
                    if (arg.StartsWith("-") || arg.StartsWith("/"))
                    {
                        switch (arg.ToLowerInvariant())
                        {
                            case "/?":
                            case "/help":
                            case "-?":
                            case "-help":
                                // Query usage help. Return null so that usage is printed
                                return null;
                            default:
                                Console.WriteLine($"Bad command line arguments supplied: {arg}");
                                return null;
                        }
                    }
                    else if (arg.Contains("="))
                    {
                        string[] parameters = arg.Split('=');
                        if (String.IsNullOrEmpty(parameters[1]))
                        {
                            Console.WriteLine($"Bad command line arguments supplied: {arg}");
                            return null;
                        }
                        switch (parameters[0].ToLowerInvariant())
                        {
                            case "deploymentid":
                                deploymentId = parameters[1];
                                break;
                            case "name":
                                siloName = parameters[1];
                                break;
                            default:
                                Console.WriteLine($"Bad command line arguments supplied: {arg}");
                                return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Bad command line arguments supplied: {arg}");
                        return null;
                    }
                }
                // Default to machine name
                siloName = siloName ?? Dns.GetHostName();
                return new SiloArgs(siloName, deploymentId);
            }

            public static void PrintUsage()
            {
                string consoleAppName = Assembly.GetExecutingAssembly().GetName().Name;
                Console.WriteLine(
                    $@"USAGE: {consoleAppName} [name=<siloName>] [deploymentId=<idString>] [/debug]
                Where:
                name=<siloName> - Name of this silo (optional)
                deploymentId=<idString> - Optionally override the deployment group this host instance should run in 
                (otherwise will use the one in the configuration");
            }

        }
    }
}
