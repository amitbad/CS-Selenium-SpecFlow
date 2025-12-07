using Reqnroll;
using NUnit.Framework;
using CS_Selenium_SpecFlow.Core.Drivers;
using CS_Selenium_SpecFlow.Core.Configuration;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.StepDefinitions;

/// <summary>
/// Common step definitions for navigation and general actions
/// </summary>
[Binding]
public class CommonSteps
{
    private readonly ScenarioContext _scenarioContext;

    public CommonSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"I navigate to the application")]
    public void GivenINavigateToTheApplication()
    {
        DriverManager.NavigateToBaseUrl();
    }

    [Given(@"I navigate to ""(.*)""")]
    public void GivenINavigateTo(string url)
    {
        if (url.StartsWith("/"))
        {
            url = ConfigurationManager.BaseUrl.TrimEnd('/') + url;
        }
        DriverManager.NavigateTo(url);
    }

    [Given(@"I am on the (.*) page")]
    public void GivenIAmOnThePage(string pageName)
    {
        var url = ConfigurationManager.BaseUrl.TrimEnd('/') + "/" + pageName.ToLower().Replace(" ", "-");
        DriverManager.NavigateTo(url);
    }

    [Then(@"the page title should be ""(.*)""")]
    public void ThenThePageTitleShouldBe(string expectedTitle)
    {
        var actualTitle = DriverManager.Driver.Title;
        Assert.That(actualTitle, Is.EqualTo(expectedTitle), $"Expected title '{expectedTitle}' but got '{actualTitle}'");
    }

    [Then(@"the page title should contain ""(.*)""")]
    public void ThenThePageTitleShouldContain(string expectedText)
    {
        var actualTitle = DriverManager.Driver.Title;
        Assert.That(actualTitle, Does.Contain(expectedText), $"Expected title to contain '{expectedText}' but got '{actualTitle}'");
    }

    [Then(@"the URL should contain ""(.*)""")]
    public void ThenTheUrlShouldContain(string expectedText)
    {
        var currentUrl = DriverManager.Driver.Url;
        Assert.That(currentUrl, Does.Contain(expectedText), $"Expected URL to contain '{expectedText}' but got '{currentUrl}'");
    }

    [Then(@"the URL should be ""(.*)""")]
    public void ThenTheUrlShouldBe(string expectedUrl)
    {
        var currentUrl = DriverManager.Driver.Url;
        Assert.That(currentUrl, Is.EqualTo(expectedUrl), $"Expected URL '{expectedUrl}' but got '{currentUrl}'");
    }

    [When(@"I wait for (.*) seconds")]
    public void WhenIWaitForSeconds(int seconds)
    {
        Logger.Info($"Waiting for {seconds} seconds");
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }

    [When(@"I refresh the page")]
    public void WhenIRefreshThePage()
    {
        DriverManager.Driver.Navigate().Refresh();
    }

    [When(@"I go back")]
    public void WhenIGoBack()
    {
        DriverManager.Driver.Navigate().Back();
    }

    [When(@"I go forward")]
    public void WhenIGoForward()
    {
        DriverManager.Driver.Navigate().Forward();
    }

    [Then(@"I take a screenshot")]
    public void ThenITakeAScreenshot()
    {
        var scenarioName = _scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
        DriverManager.TakeScreenshot(scenarioName);
    }
}
