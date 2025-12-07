using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using CS_Selenium_SpecFlow.Core.Configuration;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Core.Reporting;

/// <summary>
/// Manages Extent Reports for test reporting
/// </summary>
public static class ExtentReportManager
{
    private static ExtentReports? _extent;
    private static readonly ThreadLocal<ExtentTest?> _feature = new();
    private static readonly ThreadLocal<ExtentTest?> _scenario = new();
    private static readonly object _lock = new();

    public static ExtentReports Extent
    {
        get
        {
            if (_extent == null)
            {
                lock (_lock)
                {
                    _extent ??= CreateExtentReports();
                }
            }
            return _extent;
        }
    }

    private static ExtentReports CreateExtentReports()
    {
        var reportPath = ConfigurationManager.ExtentReportsPath;
        Directory.CreateDirectory(reportPath);

        var reportFile = Path.Combine(reportPath, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        
        var sparkReporter = new ExtentSparkReporter(reportFile);
        sparkReporter.Config.Theme = Theme.Standard;
        sparkReporter.Config.DocumentTitle = "Test Automation Report";
        sparkReporter.Config.ReportName = "CS-Selenium-SpecFlow Test Report";
        sparkReporter.Config.Encoding = "UTF-8";

        var extent = new ExtentReports();
        extent.AttachReporter(sparkReporter);
        
        // Add system info
        extent.AddSystemInfo("Environment", Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Development");
        extent.AddSystemInfo("Browser", ConfigurationManager.BrowserType);
        extent.AddSystemInfo("Base URL", ConfigurationManager.BaseUrl);
        extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
        extent.AddSystemInfo("Machine", Environment.MachineName);
        extent.AddSystemInfo(".NET Version", Environment.Version.ToString());

        Logger.Info($"Extent Report initialized: {reportFile}");

        return extent;
    }

    public static void InitializeReport()
    {
        _ = Extent; // Force initialization
    }

    public static void CreateFeature(string featureName)
    {
        _feature.Value = Extent.CreateTest<AventStack.ExtentReports.Gherkin.Model.Feature>(featureName);
    }

    public static void CreateScenario(string scenarioName)
    {
        if (_feature.Value != null)
        {
            _scenario.Value = _feature.Value.CreateNode<AventStack.ExtentReports.Gherkin.Model.Scenario>(scenarioName);
        }
        else
        {
            _scenario.Value = Extent.CreateTest(scenarioName);
        }
    }

    public static void LogStep(string stepText)
    {
        _scenario.Value?.Info(stepText);
    }

    public static void LogPass(string message)
    {
        _scenario.Value?.Pass(message);
    }

    public static void LogFail(string message, string? screenshotPath = null)
    {
        if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
        {
            _scenario.Value?.Fail(message, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
        }
        else
        {
            _scenario.Value?.Fail(message);
        }
    }

    public static void LogWarning(string message)
    {
        _scenario.Value?.Warning(message);
    }

    public static void LogInfo(string message)
    {
        _scenario.Value?.Info(message);
    }

    public static void LogSkip(string message)
    {
        _scenario.Value?.Skip(message);
    }

    public static void AddScreenshot(string screenshotPath, string title = "Screenshot")
    {
        if (File.Exists(screenshotPath))
        {
            _scenario.Value?.Info(title, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
        }
    }

    public static void FlushReport()
    {
        lock (_lock)
        {
            _extent?.Flush();
            Logger.Info("Extent Report flushed");
        }
    }
}
