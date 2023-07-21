using Microsoft.Extensions.Configuration;
using System;

public class AppSettings
{
    private static IConfigurationRoot configuration;

    static AppSettings()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true);

        configuration = configBuilder.Build();
    }

    public static string GetBaseUrl()
    {
        return configuration["BaseUrl"];
    }

    public static int GetTimeoutInSeconds()
    {
        return int.Parse(configuration["TimeoutInSeconds"]);
    }

    public static int GetMaxRetries()
    {
        return int.Parse(configuration["MaxRetries"]);
    }
}





