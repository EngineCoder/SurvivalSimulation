// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationLoader.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigurationLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using ExitGames.Logging;

namespace Photon.NameServer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    public class ConfigurationLoader 
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public static bool TryLoadFromFile(string fileName, out List<Node> config, out string message)
        {
            config = null;
            message = string.Empty;

            try
            {
                config = LoadFromFile(fileName);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public static List<Node> LoadFromFile(string fileName)
        {
            NodeList result; 
            using (var reader = new StreamReader(fileName))
            {
                string json = reader.ReadToEnd();
                var expandedJson = Environment.ExpandEnvironmentVariables(json);
                if (json != expandedJson)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("NameServer JSON config after env variables expansion\n{0}", expandedJson);
                    }
                }
                else
                {
                    log.Info("NameSever JSON was not changed during env variables expansion");
                }
                result = JsonConvert.DeserializeObject<NodeList>(expandedJson); 
            }

            return result.Nodes; 
        }
    }
}
