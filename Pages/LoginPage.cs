using CS_Selenium_SpecFlow.Core.Pages;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Pages;

/// <summary>
/// Page Object for Login Page
/// </summary>
public class LoginPage : BasePage
{
    protected override string PageName => "LoginPage";

    public void EnterUsername(string username)
    {
        Logger.Info($"Entering username: {username}");
        Type("usernameInput", username);
    }

    public void EnterPassword(string password)
    {
        Logger.Info("Entering password");
        Type("passwordInput", password);
    }

    public void ClickLoginButton()
    {
        Logger.Info("Clicking login button");
        Click("loginButton");
    }

    public void Login(string username, string password)
    {
        Logger.Info($"Performing login for user: {username}");
        EnterUsername(username);
        EnterPassword(password);
        ClickLoginButton();
    }

    public void ClickRememberMe()
    {
        Click("rememberMeCheckbox");
    }

    public void ClickForgotPassword()
    {
        Click("forgotPasswordLink");
    }

    public string GetErrorMessage()
    {
        return GetText("errorMessage");
    }

    public bool IsErrorMessageDisplayed()
    {
        return IsDisplayed("errorMessage");
    }

    public bool IsLoginFormDisplayed()
    {
        return IsDisplayed("loginForm");
    }

    public bool IsLoginButtonEnabled()
    {
        return IsEnabled("loginButton");
    }
}
