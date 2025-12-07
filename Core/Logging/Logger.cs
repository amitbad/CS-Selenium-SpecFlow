using Serilog;
using Serilog.Events;
using CS_Selenium_SpecFlow.Core.Configuration;

namespace CS_Selenium_SpecFlow.Core.Logging;

/// <summary>
/// Centralized logging utility using Serilog
/// </summary>
public static class Logger
{
    private static ILogger? _logger;
    private static readonly object _lock = new();

    public static ILogger Instance
    {
        get
        {
            if (_logger == null)
            {
                lock (_lock)
                {
                    _logger ??= CreateLogger();
                }
            }
            return _logger;
        }
    }

    private static ILogger CreateLogger()
    {
        var logPath = ConfigurationManager.LogPath;
        Directory.CreateDirectory(logPath);

        var logLevel = ConfigurationManager.LogLevel.ToLower() switch
        {
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };

        return new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                Path.Combine(logPath, "test-log-.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();
    }

    public static void Verbose(string message) => Instance.Verbose(message);
    public static void Debug(string message) => Instance.Debug(message);
    public static void Info(string message) => Instance.Information(message);
    public static void Warning(string message) => Instance.Warning(message);
    public static void Error(string message) => Instance.Error(message);
    public static void Error(Exception ex, string message) => Instance.Error(ex, message);
    public static void Fatal(string message) => Instance.Fatal(message);
    public static void Fatal(Exception ex, string message) => Instance.Fatal(ex, message);

    public static void StepInfo(string stepName) => Info($"[STEP] {stepName}");
    public static void TestStart(string testName) => Info($"[TEST START] {testName}");
    public static void TestEnd(string testName, bool passed) => 
        Info($"[TEST END] {testName} - {(passed ? "PASSED" : "FAILED")}");
}
