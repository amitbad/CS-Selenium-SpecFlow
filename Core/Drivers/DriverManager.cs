using OpenQA.Selenium;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Core.Drivers;

/// <summary>
/// Thread-safe WebDriver manager for parallel test execution
/// </summary>
public class DriverManager : IDisposable
{
    private static readonly ThreadLocal<IWebDriver?> _driver = new();
    private bool _disposed;

    public static IWebDriver Driver
    {
        get
        {
            if (_driver.Value == null)
            {
                throw new InvalidOperationException("WebDriver has not been initialized. Call InitializeDriver() first.");
            }
            return _driver.Value;
        }
    }

    public static bool HasDriver => _driver.Value != null;

    public static void InitializeDriver(string? browserType = null)
    {
        try
        {
            if (_driver.Value != null)
            {
                Logger.Warning("Driver already exists. Quitting existing driver before creating new one.");
                QuitDriver();
            }

            _driver.Value = DriverFactory.CreateDriver(browserType);
            Logger.Info($"Driver initialized: {_driver.Value.GetType().Name}");
        }
        catch (WebDriverException ex)
        {
            Logger.Error(ex, $"Failed to initialize WebDriver: {ex.Message}");
            throw new InvalidOperationException($"Failed to initialize {browserType ?? "default"} browser. Ensure the browser is installed.", ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Unexpected error initializing driver: {ex.Message}");
            throw;
        }
    }

    public static void QuitDriver()
    {
        if (_driver.Value != null)
        {
            try
            {
                _driver.Value.Quit();
                _driver.Value.Dispose();
                Logger.Info("Driver quit successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error quitting driver: {ex.Message}");
            }
            finally
            {
                _driver.Value = null;
            }
        }
    }

    public static void NavigateTo(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be null or empty", nameof(url));
        }

        try
        {
            Logger.Info($"Navigating to: {url}");
            Driver.Navigate().GoToUrl(url);
        }
        catch (WebDriverException ex)
        {
            Logger.Error(ex, $"Failed to navigate to URL: {url}");
            throw new InvalidOperationException($"Navigation to '{url}' failed. Check if the URL is valid and accessible.", ex);
        }
    }

    public static void NavigateToBaseUrl()
    {
        var baseUrl = Configuration.ConfigurationManager.BaseUrl;
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Base URL is not configured. Set APP_BASE_URL environment variable or configure in appsettings.json");
        }
        NavigateTo(baseUrl);
    }

    public static string TakeScreenshot(string fileName)
    {
        try
        {
            var screenshotPath = Configuration.ConfigurationManager.ScreenshotPath;
            Directory.CreateDirectory(screenshotPath);

            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            var filePath = Path.Combine(screenshotPath, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            
            screenshot.SaveAsFile(filePath);
            Logger.Info($"Screenshot saved: {filePath}");
            
            return filePath;
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to take screenshot: {ex.Message}");
            return string.Empty;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                QuitDriver();
            }
            _disposed = true;
        }
    }
}
