using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Safari;
using CS_Selenium_SpecFlow.Core.Configuration;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Core.Drivers;

/// <summary>
/// Factory class for creating WebDriver instances
/// Selenium 4.6+ includes Selenium Manager which automatically handles driver management
/// </summary>
public static class DriverFactory
{
    public static IWebDriver CreateDriver(string? browserType = null)
    {
        var browser = browserType ?? ConfigurationManager.BrowserType;
        var isHeadless = ConfigurationManager.IsHeadless;

        Logger.Info($"Creating {browser} driver (Headless: {isHeadless})");

        IWebDriver driver = browser.ToLower() switch
        {
            "chrome" => CreateChromeDriver(isHeadless),
            "firefox" => CreateFirefoxDriver(isHeadless),
            "edge" => CreateEdgeDriver(isHeadless),
            "safari" => CreateSafariDriver(),
            _ => throw new ArgumentException($"Browser '{browser}' is not supported")
        };

        ConfigureDriver(driver);
        return driver;
    }

    private static IWebDriver CreateChromeDriver(bool headless)
    {
        // Selenium Manager (built into Selenium 4.6+) automatically handles driver management
        var options = new ChromeOptions();
        
        if (headless)
        {
            options.AddArgument("--headless=new");
        }

        // Common Chrome options for stability
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-infobars");
        options.AddArgument("--start-maximized");
        options.AddArgument($"--window-size={ConfigurationManager.WindowWidth},{ConfigurationManager.WindowHeight}");
        
        // For CI/CD environments
        options.AddArgument("--disable-software-rasterizer");
        options.AddArgument("--remote-allow-origins=*");

        return new ChromeDriver(options);
    }

    private static IWebDriver CreateFirefoxDriver(bool headless)
    {
        var options = new FirefoxOptions();
        
        if (headless)
        {
            options.AddArgument("--headless");
        }

        options.AddArgument($"--width={ConfigurationManager.WindowWidth}");
        options.AddArgument($"--height={ConfigurationManager.WindowHeight}");

        return new FirefoxDriver(options);
    }

    private static IWebDriver CreateEdgeDriver(bool headless)
    {
        var options = new EdgeOptions();
        
        if (headless)
        {
            options.AddArgument("--headless=new");
        }

        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument($"--window-size={ConfigurationManager.WindowWidth},{ConfigurationManager.WindowHeight}");

        return new EdgeDriver(options);
    }

    private static IWebDriver CreateSafariDriver()
    {
        // Safari doesn't support headless mode
        var options = new SafariOptions();
        return new SafariDriver(options);
    }

    private static void ConfigureDriver(IWebDriver driver)
    {
        var timeouts = driver.Manage().Timeouts();
        
        timeouts.ImplicitWait = TimeSpan.FromSeconds(ConfigurationManager.ImplicitWait);
        timeouts.PageLoad = TimeSpan.FromSeconds(ConfigurationManager.PageLoadTimeout);
        
        driver.Manage().Window.Maximize();
        
        Logger.Info("Driver configured successfully");
    }
}
