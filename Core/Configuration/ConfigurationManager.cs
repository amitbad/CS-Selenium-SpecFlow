using Microsoft.Extensions.Configuration;

namespace CS_Selenium_SpecFlow.Core.Configuration;

/// <summary>
/// Manages application configuration from multiple sources
/// Priority: Environment Variables > appsettings.{Environment}.json > appsettings.json
/// </summary>
public static class ConfigurationManager
{
    private static IConfiguration? _configuration;
    private static readonly object _lock = new();

    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                lock (_lock)
                {
                    _configuration ??= BuildConfiguration();
                }
            }
            return _configuration;
        }
    }

    private static IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Development";
        var basePath = GetConfigBasePath();

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Config/appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    private static string GetConfigBasePath()
    {
        // Try to find the config directory
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        
        // Check if running from bin folder
        if (currentDir.Contains("bin"))
        {
            return currentDir;
        }
        
        return Directory.GetCurrentDirectory();
    }

    public static void ReloadConfiguration()
    {
        lock (_lock)
        {
            _configuration = BuildConfiguration();
        }
    }

    // Browser Settings
    public static string BrowserType => 
        Environment.GetEnvironmentVariable("BROWSER_TYPE") ?? 
        Configuration["Browser:Type"] ?? "Chrome";

    public static bool IsHeadless => 
        bool.TryParse(Environment.GetEnvironmentVariable("BROWSER_HEADLESS"), out var headless) 
            ? headless 
            : Configuration.GetValue<bool>("Browser:Headless");

    public static int ImplicitWait => 
        Configuration.GetValue<int>("Browser:ImplicitWait", 10);

    public static int ExplicitWait => 
        Configuration.GetValue<int>("Browser:ExplicitWait", 30);

    public static int PageLoadTimeout => 
        Configuration.GetValue<int>("Browser:PageLoadTimeout", 60);

    public static int WindowWidth => 
        Configuration.GetValue<int>("Browser:WindowSize:Width", 1920);

    public static int WindowHeight => 
        Configuration.GetValue<int>("Browser:WindowSize:Height", 1080);

    // Application Settings
    public static string BaseUrl => 
        Environment.GetEnvironmentVariable("APP_BASE_URL") ?? 
        Configuration["Application:BaseUrl"] ?? "https://example.com";

    public static string ApiBaseUrl => 
        Environment.GetEnvironmentVariable("API_BASE_URL") ?? 
        Configuration["Application:ApiBaseUrl"] ?? "https://api.example.com";

    // Credentials
    public static string TestUsername => 
        Environment.GetEnvironmentVariable("TEST_USERNAME") ?? "";

    public static string TestPassword => 
        Environment.GetEnvironmentVariable("TEST_PASSWORD") ?? "";

    public static string ApiKey => 
        Environment.GetEnvironmentVariable("API_KEY") ?? "";

    // Reporting Settings
    public static bool ExtentReportsEnabled => 
        Configuration.GetValue<bool>("Reporting:ExtentReports:Enabled", true);

    public static string ExtentReportsPath => 
        Environment.GetEnvironmentVariable("REPORT_PATH") ?? 
        Configuration["Reporting:ExtentReports:ReportPath"] ?? "Reports/ExtentReports";

    public static string AllureResultsDirectory => 
        Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY") ?? 
        Configuration["Reporting:Allure:ResultsDirectory"] ?? "allure-results";

    public static bool ScreenshotOnFailure => 
        Configuration.GetValue<bool>("Reporting:Screenshots:OnFailure", true);

    public static string ScreenshotPath => 
        Configuration["Reporting:Screenshots:Path"] ?? "Reports/Screenshots";

    // Retry Settings
    public static int MaxRetryAttempts => 
        Configuration.GetValue<int>("Retry:MaxAttempts", 2);

    public static int RetryDelayMs => 
        Configuration.GetValue<int>("Retry:DelayMs", 1000);

    // Logging
    public static string LogLevel => 
        Configuration["Logging:Level"] ?? "Information";

    public static string LogPath => 
        Configuration["Logging:LogPath"] ?? "Logs";
}
