using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Seedwork.Configuration;
using StructuringNotifications.WebApp.Configuration;

namespace StructuringNotifications.Tests.Tools
{
    [SetUpFixture]
    public class SetUpTest
    {
        private const string ConfigName = "appsettings.autotests.json";
        protected const string ControlTable = "TestCommits";

        protected StructuringNotificationsConfiguration Configuration { get; private set; }

        public SetUpTest()
        {
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTestsOnes()
        {
            Configuration = LoadConfiguration();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTestsOnes()
        {
        }

        private static StructuringNotificationsConfiguration LoadConfiguration()
        {
            var appConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: ConfigName, optional: false, reloadOnChange: true)
                .Build();
            var settingsConnectionString = appConfig.GetValue<string>("SettingsStorageConnectionString");
            var defaultSettings = appConfig.GetValue<string>("SettingsName");
            var loadConfigLocal = appConfig.GetValue<bool>("LoadConfigLocal");
            var buildAgent = appConfig.GetValue<string>("BuildAgent");
            var reader = new ConfigurationReader(settingsConnectionString, defaultSettings, loadConfigLocal);
            var configuration = reader.ReadConfiguration<StructuringNotificationsConfiguration>();
            if (!string.IsNullOrWhiteSpace(buildAgent))
            {
                var dbNameMatch = Regex.Match(configuration.StructuringNotificationsDbContext, @"(?<=Initial Catalog=)\S+Test");
                if (dbNameMatch.Success)
                    configuration.StructuringNotificationsDbContext =
                        configuration.StructuringNotificationsDbContext.Replace(dbNameMatch.Value,
                            $"{dbNameMatch.Value}-{buildAgent}");
            }

            return configuration;
        }

    }
}