using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dissimilis.Configuration
{
    public class ConfigurationInfo
    {
        public const string NORWEGIAN_CUTURE = "nb-no";
        internal static ILogger configurationLogger;

        public enum RequiredValues
        {
            ASPNETCORE_ENVIRONMENT,
            API_BASEURL,
            FRONTEND_BASE_URL,

            APPINSIGHTS_INSTRUMENTATIONKEY,

            SQL_CONNECTION_STRING,
            AZURE_CLIENT_SECRET
        }

        /// <summary>
        /// Values that should always have a system fallback set
        /// </summary>
        private enum OptionalValues
        {
            AZURE_DIRECTORY_ID,
            AZURE_APPLICATION_ID,
            IS_AUTOMATED_TESTING_MODE
        }

        internal static IConfiguration StaticConfig;

        public ConfigurationInfo(IConfiguration staticConfig, ILogger configurationLogger = null)
        {
            ConfigurationInfo.configurationLogger = configurationLogger;
            ConfigurationInfo.StaticConfig = staticConfig;
        }

        public static bool IsConfigurationHealthOk(bool throwException = true)
        {
            try
            {
                var configurationValues = Enum.GetValues(typeof(RequiredValues))
                    .Cast<RequiredValues>()
                    .ToList();

                foreach (var configurationValue in configurationValues)
                {
                    var value = ConfigurationInfo.StaticConfig.GetSection(configurationValue.ToString());
                    if (value == null)
                    {
                        throw new Exception($"Required configuration missing: {configurationValue.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                configurationLogger?.LogError("EnviromentService - HealthCheck failing");
                if (throwException)
                {
                    throw e;
                }

                return false;
            }

            return true;
        }

        #region Support functions

        private static string GetValue(RequiredValues enumKey)
        {
            var value = ConfigurationInfo.StaticConfig.GetSection(enumKey.ToString())?.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                var mesasge = $"Required configuration value not found: {enumKey.ToString()}";
                configurationLogger?.LogError(mesasge);
                throw new Exception(mesasge);
            }

            return value.Trim();
        }

        private static bool GetValueAsBool(RequiredValues enumKey) => GetValue(enumKey) == "1";

        private static string GetValue(OptionalValues enumKey, string fallbackValue)
        {
            var value = ConfigurationInfo.StaticConfig.GetSection(enumKey.ToString())?.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                value = fallbackValue;
            }

            return value.Trim();
        }
        private static bool GetValueAsBool(OptionalValues enumKey, string fallback) => GetValue(enumKey, fallback) == "1";

        #endregion


        public static string GetFrontendBaseUrl() => GetValue(RequiredValues.FRONTEND_BASE_URL).TrimEnd('/');
        public static string GetApiBaseUrl() => GetValue(RequiredValues.API_BASEURL).TrimEnd('/');

        public static string GetEnviromentVariable() => GetValue(RequiredValues.ASPNETCORE_ENVIRONMENT);
        public static string GetSqlConnectionString() => GetValue(RequiredValues.SQL_CONNECTION_STRING);



        public static string GetAPPINSIGHTS_INSTRUMENTATIONKEY() =>
            GetValue(RequiredValues.APPINSIGHTS_INSTRUMENTATIONKEY);

        public static bool IsLocalDebugBuild()
        {
#if DEBUG
            return true;
#else
            return false;
#endif

        }

        public static string GetAzureClientSecret() => GetValue(RequiredValues.AZURE_CLIENT_SECRET);

        public static Guid GetAzureDirectoryId() => new Guid(GetValue(OptionalValues.AZURE_DIRECTORY_ID, "774897da - 0c2c - 4c71 - 9897 - 873c4d659aee"));

        public static Guid GetAzureApplicationId() => new Guid(GetValue(OptionalValues.AZURE_APPLICATION_ID, "5a41da85-fa69-4aa0-93f2-1ce65104a1b2"));

        public static bool IsAutomatedTestingMode() => GetValueAsBool(OptionalValues.IS_AUTOMATED_TESTING_MODE, "0");

    }

}
