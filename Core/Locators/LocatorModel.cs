using Newtonsoft.Json;

namespace CS_Selenium_SpecFlow.Core.Locators;

/// <summary>
/// Represents a single locator definition
/// </summary>
public class LocatorDefinition
{
    [JsonProperty("type")]
    public string Type { get; set; } = "id";

    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("timeout")]
    public int? Timeout { get; set; }
}

/// <summary>
/// Represents a page's locator collection
/// </summary>
public class PageLocators
{
    [JsonProperty("pageName")]
    public string PageName { get; set; } = string.Empty;

    [JsonProperty("pageUrl")]
    public string? PageUrl { get; set; }

    [JsonProperty("locators")]
    public Dictionary<string, LocatorDefinition> Locators { get; set; } = new();
}

/// <summary>
/// Supported locator types
/// </summary>
public enum LocatorType
{
    Id,
    Name,
    ClassName,
    TagName,
    LinkText,
    PartialLinkText,
    CssSelector,
    XPath
}
