using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using CS_Selenium_SpecFlow.Core.Drivers;
using CS_Selenium_SpecFlow.Core.Configuration;

namespace CS_Selenium_SpecFlow.Core.Utilities;

/// <summary>
/// Helper class for various wait operations
/// </summary>
public static class WaitHelper
{
    private static IWebDriver Driver => DriverManager.Driver;

    public static IWebElement WaitForElementVisible(By by, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.ElementIsVisible(by));
    }

    public static IWebElement WaitForElementClickable(By by, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.ElementToBeClickable(by));
    }

    public static bool WaitForElementInvisible(By by, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.InvisibilityOfElementLocated(by));
    }

    public static IWebElement WaitForElementExists(By by, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.ElementExists(by));
    }

    public static bool WaitForTextInElement(By by, string text, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.TextToBePresentInElementLocated(by, text));
    }

    public static bool WaitForUrlContains(string urlPart, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.UrlContains(urlPart));
    }

    public static bool WaitForTitleContains(string titlePart, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.TitleContains(titlePart));
    }

    public static void WaitForPageLoad(int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.PageLoadTimeout;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        wait.Until(driver => ((IJavaScriptExecutor)driver)
            .ExecuteScript("return document.readyState").Equals("complete"));
    }

    public static void WaitForAjaxComplete(int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        wait.Until(driver =>
        {
            var jsExecutor = (IJavaScriptExecutor)driver;
            return (bool)jsExecutor.ExecuteScript(
                "return (typeof jQuery === 'undefined') || (jQuery.active === 0)");
        });
    }

    public static IAlert WaitForAlert(int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.AlertIsPresent());
    }

    public static IWebDriver WaitForFrameAndSwitch(By by, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(by));
    }

    public static T WaitFor<T>(Func<IWebDriver, T> condition, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(condition);
    }

    public static bool WaitForCondition(Func<bool> condition, int? timeoutSeconds = null, int pollingIntervalMs = 500)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var endTime = DateTime.Now.AddSeconds(timeout);

        while (DateTime.Now < endTime)
        {
            if (condition())
            {
                return true;
            }
            Thread.Sleep(pollingIntervalMs);
        }

        return false;
    }
}
