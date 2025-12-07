using Newtonsoft.Json;
using OpenQA.Selenium;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Core.Locators;

/// <summary>
/// Reads and manages JSON-based locators
/// </summary>
public static class LocatorReader
{
    private static readonly Dictionary<string, PageLocators> _locatorCache = new();
    private static readonly object _lock = new();
    private static string _locatorsBasePath = string.Empty;

    public static string LocatorsBasePath
    {
        get
        {
            if (string.IsNullOrEmpty(_locatorsBasePath))
            {
                _locatorsBasePath = FindLocatorsPath();
            }
            return _locatorsBasePath;
        }
        set => _locatorsBasePath = value;
    }

    private static string FindLocatorsPath()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var locatorsPath = Path.Combine(basePath, "Locators");
        
        if (Directory.Exists(locatorsPath))
        {
            return locatorsPath;
        }

        // Try project root
        var projectRoot = Directory.GetCurrentDirectory();
        locatorsPath = Path.Combine(projectRoot, "Locators");
        
        if (Directory.Exists(locatorsPath))
        {
            return locatorsPath;
        }

        throw new DirectoryNotFoundException("Locators directory not found");
    }

    /// <summary>
    /// Gets page locators from JSON file
    /// </summary>
    public static PageLocators GetPageLocators(string pageName)
    {
        lock (_lock)
        {
            if (_locatorCache.TryGetValue(pageName, out var cached))
            {
                return cached;
            }

            var filePath = Path.Combine(LocatorsBasePath, $"{pageName}.json");
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Locator file not found: {filePath}");
            }

            var json = File.ReadAllText(filePath);
            var pageLocators = JsonConvert.DeserializeObject<PageLocators>(json) 
                ?? throw new InvalidOperationException($"Failed to parse locator file: {filePath}");

            _locatorCache[pageName] = pageLocators;
            Logger.Debug($"Loaded locators for page: {pageName}");

            return pageLocators;
        }
    }

    /// <summary>
    /// Gets a specific locator by page and element name
    /// </summary>
    public static LocatorDefinition GetLocator(string pageName, string elementName)
    {
        var pageLocators = GetPageLocators(pageName);
        
        if (!pageLocators.Locators.TryGetValue(elementName, out var locator))
        {
            throw new KeyNotFoundException($"Locator '{elementName}' not found in page '{pageName}'");
        }

        return locator;
    }

    /// <summary>
    /// Converts a LocatorDefinition to Selenium By
    /// </summary>
    public static By ToSeleniumBy(LocatorDefinition locator)
    {
        return locator.Type.ToLower() switch
        {
            "id" => By.Id(locator.Value),
            "name" => By.Name(locator.Value),
            "classname" or "class" => By.ClassName(locator.Value),
            "tagname" or "tag" => By.TagName(locator.Value),
            "linktext" or "link" => By.LinkText(locator.Value),
            "partiallinktext" or "partiallink" => By.PartialLinkText(locator.Value),
            "cssselector" or "css" => By.CssSelector(locator.Value),
            "xpath" => By.XPath(locator.Value),
            _ => throw new ArgumentException($"Unsupported locator type: {locator.Type}")
        };
    }

    /// <summary>
    /// Gets Selenium By directly from page and element name
    /// </summary>
    public static By GetBy(string pageName, string elementName)
    {
        var locator = GetLocator(pageName, elementName);
        return ToSeleniumBy(locator);
    }

    /// <summary>
    /// Clears the locator cache
    /// </summary>
    public static void ClearCache()
    {
        lock (_lock)
        {
            _locatorCache.Clear();
            Logger.Debug("Locator cache cleared");
        }
    }

    /// <summary>
    /// Reloads a specific page's locators
    /// </summary>
    public static void ReloadPageLocators(string pageName)
    {
        lock (_lock)
        {
            _locatorCache.Remove(pageName);
            GetPageLocators(pageName);
        }
    }
}
