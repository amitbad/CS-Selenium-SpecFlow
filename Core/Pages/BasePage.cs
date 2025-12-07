using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using CS_Selenium_SpecFlow.Core.Drivers;
using CS_Selenium_SpecFlow.Core.Locators;
using CS_Selenium_SpecFlow.Core.Logging;
using CS_Selenium_SpecFlow.Core.Configuration;

namespace CS_Selenium_SpecFlow.Core.Pages;

/// <summary>
/// Base class for all Page Objects
/// </summary>
public abstract class BasePage
{
    protected IWebDriver Driver => DriverManager.Driver;
    protected WebDriverWait Wait { get; }
    protected abstract string PageName { get; }

    protected BasePage()
    {
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(ConfigurationManager.ExplicitWait));
    }

    #region Element Locator Methods

    /// <summary>
    /// Gets element using JSON locator
    /// </summary>
    protected IWebElement GetElement(string elementName)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Element name cannot be null or empty", nameof(elementName));
        }

        try
        {
            var by = LocatorReader.GetBy(PageName, elementName);
            return WaitForElement(by);
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.Error(ex, $"Element '{elementName}' not found on page '{PageName}' within timeout");
            throw new NoSuchElementException($"Element '{elementName}' not found on page '{PageName}'. Check if the locator is correct in {PageName}.json", ex);
        }
        catch (KeyNotFoundException ex)
        {
            Logger.Error(ex, $"Locator '{elementName}' not defined for page '{PageName}'");
            throw;
        }
    }

    /// <summary>
    /// Gets elements using JSON locator
    /// </summary>
    protected IReadOnlyCollection<IWebElement> GetElements(string elementName)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Element name cannot be null or empty", nameof(elementName));
        }

        try
        {
            var by = LocatorReader.GetBy(PageName, elementName);
            return Driver.FindElements(by);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to get elements '{elementName}' on page '{PageName}'");
            throw;
        }
    }

    /// <summary>
    /// Checks if element exists
    /// </summary>
    protected bool ElementExists(string elementName)
    {
        try
        {
            var by = LocatorReader.GetBy(PageName, elementName);
            return Driver.FindElements(by).Count > 0;
        }
        catch (Exception ex)
        {
            Logger.Debug($"ElementExists check failed for '{elementName}': {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Wait Methods

    protected IWebElement WaitForElement(By by, int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.ElementIsVisible(by));
    }

    protected IWebElement WaitForElementClickable(string elementName, int? timeoutSeconds = null)
    {
        var by = LocatorReader.GetBy(PageName, elementName);
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.ElementToBeClickable(by));
    }

    protected bool WaitForElementInvisible(string elementName, int? timeoutSeconds = null)
    {
        var by = LocatorReader.GetBy(PageName, elementName);
        var timeout = timeoutSeconds ?? ConfigurationManager.ExplicitWait;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        return wait.Until(ExpectedConditions.InvisibilityOfElementLocated(by));
    }

    protected void WaitForPageLoad(int? timeoutSeconds = null)
    {
        var timeout = timeoutSeconds ?? ConfigurationManager.PageLoadTimeout;
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
        wait.Until(driver => ((IJavaScriptExecutor)driver)
            .ExecuteScript("return document.readyState").Equals("complete"));
    }

    #endregion

    #region Action Methods

    protected void Click(string elementName)
    {
        Logger.Debug($"Clicking element: {elementName}");
        var element = WaitForElementClickable(elementName);
        element.Click();
    }

    protected void Type(string elementName, string text)
    {
        Logger.Debug($"Typing '{text}' into element: {elementName}");
        var element = GetElement(elementName);
        element.Clear();
        element.SendKeys(text);
    }

    protected void ClearAndType(string elementName, string text)
    {
        var element = GetElement(elementName);
        element.Clear();
        element.SendKeys(text);
    }

    protected string GetText(string elementName)
    {
        return GetElement(elementName).Text;
    }

    protected string GetAttribute(string elementName, string attributeName)
    {
        return GetElement(elementName).GetAttribute(attributeName) ?? string.Empty;
    }

    protected bool IsDisplayed(string elementName)
    {
        try
        {
            return GetElement(elementName).Displayed;
        }
        catch
        {
            return false;
        }
    }

    protected bool IsEnabled(string elementName)
    {
        return GetElement(elementName).Enabled;
    }

    protected bool IsSelected(string elementName)
    {
        return GetElement(elementName).Selected;
    }

    protected void SelectByText(string elementName, string text)
    {
        var element = GetElement(elementName);
        var select = new SelectElement(element);
        select.SelectByText(text);
    }

    protected void SelectByValue(string elementName, string value)
    {
        var element = GetElement(elementName);
        var select = new SelectElement(element);
        select.SelectByValue(value);
    }

    protected void SelectByIndex(string elementName, int index)
    {
        var element = GetElement(elementName);
        var select = new SelectElement(element);
        select.SelectByIndex(index);
    }

    #endregion

    #region JavaScript Methods

    protected void ScrollToElement(string elementName)
    {
        var element = GetElement(elementName);
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
    }

    protected void JavaScriptClick(string elementName)
    {
        var element = GetElement(elementName);
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
    }

    protected object ExecuteScript(string script, params object[] args)
    {
        return ((IJavaScriptExecutor)Driver).ExecuteScript(script, args);
    }

    #endregion

    #region Navigation Methods

    public virtual void NavigateToPage()
    {
        var pageLocators = LocatorReader.GetPageLocators(PageName);
        if (!string.IsNullOrEmpty(pageLocators.PageUrl))
        {
            var fullUrl = ConfigurationManager.BaseUrl.TrimEnd('/') + pageLocators.PageUrl;
            Driver.Navigate().GoToUrl(fullUrl);
            WaitForPageLoad();
            Logger.Info($"Navigated to page: {PageName}");
        }
    }

    public string GetCurrentUrl() => Driver.Url;

    public string GetPageTitle() => Driver.Title;

    #endregion

    #region Screenshot

    public string TakeScreenshot(string? fileName = null)
    {
        return DriverManager.TakeScreenshot(fileName ?? PageName);
    }

    #endregion
}
