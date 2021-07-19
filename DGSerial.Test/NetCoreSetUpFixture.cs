#region License
// Copyright (c) 2021 Davide Gironi
//
// Please refer to LICENSE file for licensing information.
#endregion

#if NETFRAMEWORK
#else
using NUnit.Framework;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace DG.Serial.Test
{

    [SetUpFixture]
    public class NetCoreSetUpFixture
    {
        /// <summary>
        /// Initialized app config for .NET core
        /// </summary>
        [OneTimeSetUp]
        public void InitializeTestRunnerAppConfig()
        {
            string appConfigPath = Assembly.GetExecutingAssembly().Location + ".config";
            if (!File.Exists(appConfigPath))
                return;
            Configuration configurationApp = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = appConfigPath }, ConfigurationUserLevel.None);
            Configuration configurationActive = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configurationApp == configurationActive)
                return;
            configurationActive.AppSettings.Settings.Clear();
            foreach (string key in configurationApp.AppSettings.Settings.AllKeys)
                configurationActive.AppSettings.Settings.Add(configurationApp.AppSettings.Settings[key]);
            configurationActive.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
#endif
