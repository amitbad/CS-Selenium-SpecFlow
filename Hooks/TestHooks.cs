using Reqnroll;
using CS_Selenium_SpecFlow.Core.Drivers;
using CS_Selenium_SpecFlow.Core.Logging;
using CS_Selenium_SpecFlow.Core.Configuration;
using CS_Selenium_SpecFlow.Core.Reporting;

namespace CS_Selenium_SpecFlow.Hooks;

/// <summary>
/// SpecFlow hooks for test lifecycle management
/// </summary>
[Binding]
public class TestHooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly FeatureContext _featureContext;

    public TestHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
    {
        _scenarioContext = scenarioContext;
        _featureContext = featureContext;
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Logger.Info("=== Test Run Started ===");
        Logger.Info($"Environment: {Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Development"}");
        Logger.Info($"Browser: {ConfigurationManager.BrowserType}");
        Logger.Info($"Base URL: {ConfigurationManager.BaseUrl}");
        
        // Initialize Extent Reports
        ExtentReportManager.InitializeReport();
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Logger.Info("=== Test Run Completed ===");
        
        // Flush Extent Reports
        ExtentReportManager.FlushReport();
    }

    [BeforeFeature]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        Logger.Info($"[FEATURE START] {featureContext.FeatureInfo.Title}");
        ExtentReportManager.CreateFeature(featureContext.FeatureInfo.Title);
    }

    [AfterFeature]
    public static void AfterFeature(FeatureContext featureContext)
    {
        Logger.Info($"[FEATURE END] {featureContext.FeatureInfo.Title}");
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
        var tags = _scenarioContext.ScenarioInfo.Tags;

        Logger.TestStart(scenarioTitle);
        ExtentReportManager.CreateScenario(scenarioTitle);

        // Initialize browser for UI tests (skip for API-only tests)
        if (!tags.Contains("api"))
        {
            DriverManager.InitializeDriver();
        }
    }

    [AfterScenario]
    public void AfterScenario()
    {
        var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
        var testError = _scenarioContext.TestError;

        if (testError != null)
        {
            Logger.Error($"Scenario failed: {testError.Message}");
            
            // Take screenshot on failure
            if (DriverManager.HasDriver && ConfigurationManager.ScreenshotOnFailure)
            {
                var screenshotPath = DriverManager.TakeScreenshot($"FAILED_{scenarioTitle.Replace(" ", "_")}");
                ExtentReportManager.LogFail($"Scenario failed: {testError.Message}", screenshotPath);
            }
            else
            {
                ExtentReportManager.LogFail($"Scenario failed: {testError.Message}");
            }

            Logger.TestEnd(scenarioTitle, false);
        }
        else
        {
            ExtentReportManager.LogPass("Scenario passed");
            Logger.TestEnd(scenarioTitle, true);
        }

        // Quit browser
        if (DriverManager.HasDriver)
        {
            DriverManager.QuitDriver();
        }
    }

    [BeforeStep]
    public void BeforeStep()
    {
        var stepInfo = _scenarioContext.StepContext.StepInfo;
        Logger.StepInfo($"{stepInfo.StepDefinitionType} {stepInfo.Text}");
    }

    [AfterStep]
    public void AfterStep()
    {
        var stepInfo = _scenarioContext.StepContext.StepInfo;
        var stepText = $"{stepInfo.StepDefinitionType} {stepInfo.Text}";

        if (_scenarioContext.TestError == null)
        {
            ExtentReportManager.LogStep(stepText);
        }
    }
}
