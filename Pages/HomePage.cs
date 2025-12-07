using CS_Selenium_SpecFlow.Core.Pages;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Pages;

/// <summary>
/// Page Object for Home Page
/// </summary>
public class HomePage : BasePage
{
    protected override string PageName => "HomePage";

    public bool IsLogoDisplayed()
    {
        return IsDisplayed("logo");
    }

    public bool IsNavigationMenuDisplayed()
    {
        return IsDisplayed("navigationMenu");
    }

    public void Search(string searchTerm)
    {
        Logger.Info($"Searching for: {searchTerm}");
        Type("searchInput", searchTerm);
        Click("searchButton");
    }

    public void ClickUserProfile()
    {
        Click("userProfileDropdown");
    }

    public void Logout()
    {
        Logger.Info("Performing logout");
        ClickUserProfile();
        Click("logoutButton");
    }

    public string GetWelcomeMessage()
    {
        return GetText("welcomeMessage");
    }

    public bool IsUserLoggedIn()
    {
        return IsDisplayed("userProfileDropdown");
    }
}
