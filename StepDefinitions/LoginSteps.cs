using Reqnroll;
using NUnit.Framework;
using CS_Selenium_SpecFlow.Pages;
using CS_Selenium_SpecFlow.Core.Configuration;

namespace CS_Selenium_SpecFlow.StepDefinitions;

/// <summary>
/// Step definitions for login functionality
/// </summary>
[Binding]
public class LoginSteps
{
    private readonly LoginPage _loginPage;
    private readonly HomePage _homePage;
    private readonly ScenarioContext _scenarioContext;

    public LoginSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _loginPage = new LoginPage();
        _homePage = new HomePage();
    }

    [Given(@"I am on the login page")]
    public void GivenIAmOnTheLoginPage()
    {
        _loginPage.NavigateToPage();
    }

    [When(@"I enter username ""(.*)""")]
    public void WhenIEnterUsername(string username)
    {
        // Support environment variable substitution
        if (username == "{env:TEST_USERNAME}")
        {
            username = ConfigurationManager.TestUsername;
        }
        _loginPage.EnterUsername(username);
    }

    [When(@"I enter password ""(.*)""")]
    public void WhenIEnterPassword(string password)
    {
        // Support environment variable substitution
        if (password == "{env:TEST_PASSWORD}")
        {
            password = ConfigurationManager.TestPassword;
        }
        _loginPage.EnterPassword(password);
    }

    [When(@"I click the login button")]
    public void WhenIClickTheLoginButton()
    {
        _loginPage.ClickLoginButton();
    }

    [When(@"I login with username ""(.*)"" and password ""(.*)""")]
    public void WhenILoginWithUsernameAndPassword(string username, string password)
    {
        if (username == "{env:TEST_USERNAME}")
        {
            username = ConfigurationManager.TestUsername;
        }
        if (password == "{env:TEST_PASSWORD}")
        {
            password = ConfigurationManager.TestPassword;
        }
        _loginPage.Login(username, password);
    }

    [When(@"I login with valid credentials")]
    public void WhenILoginWithValidCredentials()
    {
        _loginPage.Login(ConfigurationManager.TestUsername, ConfigurationManager.TestPassword);
    }

    [When(@"I check the remember me checkbox")]
    public void WhenICheckTheRememberMeCheckbox()
    {
        _loginPage.ClickRememberMe();
    }

    [When(@"I click forgot password")]
    public void WhenIClickForgotPassword()
    {
        _loginPage.ClickForgotPassword();
    }

    [Then(@"I should see the login form")]
    public void ThenIShouldSeeTheLoginForm()
    {
        Assert.That(_loginPage.IsLoginFormDisplayed(), Is.True, "Login form should be displayed");
    }

    [Then(@"I should see an error message")]
    public void ThenIShouldSeeAnErrorMessage()
    {
        Assert.That(_loginPage.IsErrorMessageDisplayed(), Is.True, "Error message should be displayed");
    }

    [Then(@"I should see error message ""(.*)""")]
    public void ThenIShouldSeeErrorMessage(string expectedMessage)
    {
        var actualMessage = _loginPage.GetErrorMessage();
        Assert.That(actualMessage, Does.Contain(expectedMessage), 
            $"Expected error message to contain '{expectedMessage}' but got '{actualMessage}'");
    }

    [Then(@"I should be logged in successfully")]
    public void ThenIShouldBeLoggedInSuccessfully()
    {
        Assert.That(_homePage.IsUserLoggedIn(), Is.True, "User should be logged in");
    }

    [Then(@"I should see the home page")]
    public void ThenIShouldSeeTheHomePage()
    {
        Assert.That(_homePage.IsLogoDisplayed(), Is.True, "Home page should be displayed");
    }

    [Then(@"the login button should be enabled")]
    public void ThenTheLoginButtonShouldBeEnabled()
    {
        Assert.That(_loginPage.IsLoginButtonEnabled(), Is.True, "Login button should be enabled");
    }

    [Then(@"the login button should be disabled")]
    public void ThenTheLoginButtonShouldBeDisabled()
    {
        Assert.That(_loginPage.IsLoginButtonEnabled(), Is.False, "Login button should be disabled");
    }
}
