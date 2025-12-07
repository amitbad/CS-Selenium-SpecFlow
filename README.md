# CS-Selenium-SpecFlow(Reqnroll) Test Automation Framework

A comprehensive test automation framework built with C#, Selenium WebDriver, Reqnroll (BDD - SpecFlow successor), and NUnit for both UI and REST API testing.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Writing Tests](#writing-tests)
- [Running Tests](#running-tests)
- [Reporting](#reporting)
- [CI/CD Integration](#cicd-integration)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Features

- **BDD with Reqnroll**: Write tests in Gherkin syntax for better collaboration (Reqnroll is the community-maintained successor to SpecFlow, supporting .NET 8+)
- **Selenium WebDriver**: Cross-browser UI testing (Chrome, Firefox, Edge, Safari)
- **REST API Testing**: Built-in support for API testing with RestSharp
- **JSON-based Locators**: Simplified, maintainable element locators
- **Page Object Model**: Clean separation of test logic and page interactions
- **Multiple Environments**: Easy switching between Dev, Staging, Production
- **Parallel Execution**: Thread-safe design for parallel test execution
- **Comprehensive Reporting**: Extent Reports and Allure integration
- **CI/CD Ready**: GitHub Actions and Jenkins configurations included
- **Logging**: Structured logging with Serilog

## Prerequisites

- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider
- **Browser**: Chrome, Firefox, Edge, or Safari

### Verify Installation

```bash
dotnet --version
# Should output: 10.x.x
```

## Project Structure

```
CS-Selenium-SpecFlow/
├── .github/
│   └── workflows/
│       └── test.yml              # GitHub Actions workflow
├── Config/
│   ├── appsettings.json          # Base configuration
│   ├── appsettings.Development.json
│   ├── appsettings.Staging.json
│   └── appsettings.Production.json
├── Core/
│   ├── Api/
│   │   ├── ApiClient.cs          # REST API client
│   │   └── ApiAssertions.cs      # Fluent API assertions
│   ├── Configuration/
│   │   └── ConfigurationManager.cs
│   ├── Drivers/
│   │   ├── DriverFactory.cs      # WebDriver factory
│   │   └── DriverManager.cs      # Thread-safe driver management
│   ├── Locators/
│   │   ├── LocatorModel.cs       # Locator data models
│   │   └── LocatorReader.cs      # JSON locator reader
│   ├── Logging/
│   │   └── Logger.cs             # Serilog wrapper
│   ├── Pages/
│   │   └── BasePage.cs           # Base page object
│   ├── Reporting/
│   │   └── ExtentReportManager.cs
│   └── Utilities/
│       ├── TestDataReader.cs     # Test data utilities
│       └── WaitHelper.cs         # Wait utilities
├── Features/
│   ├── Login.feature             # UI test scenarios
│   └── ApiTests.feature          # API test scenarios
├── Hooks/
│   └── TestHooks.cs              # SpecFlow lifecycle hooks
├── Locators/
│   ├── LoginPage.json            # Login page locators
│   └── HomePage.json             # Home page locators
├── Pages/
│   ├── LoginPage.cs              # Login page object
│   └── HomePage.cs               # Home page object
├── StepDefinitions/
│   ├── CommonSteps.cs            # Common step definitions
│   ├── LoginSteps.cs             # Login step definitions
│   └── ApiSteps.cs               # API step definitions
├── TestData/
│   └── users.json                # Test data files
├── .env.example                  # Environment variables template
├── .gitignore
├── CS-Selenium-SpecFlow.csproj
├── CS-Selenium-SpecFlow.sln
├── Jenkinsfile                   # Jenkins pipeline
├── README.md
└── reqnroll.json                 # Reqnroll configuration
```

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/amitbad/CS-Selenium-SpecFlow.git
cd CS-Selenium-SpecFlow
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Configure Environment

Copy the environment template and update values:

```bash
cp .env.example .env
```

Edit `.env` with your configuration:

```env
TEST_ENVIRONMENT=Development
BROWSER_TYPE=Chrome
BROWSER_HEADLESS=false
APP_BASE_URL=https://your-app.com
API_BASE_URL=https://api.your-app.com
TEST_USERNAME=your-test-user
TEST_PASSWORD=your-test-password
```

### 5. Run Tests

```bash
dotnet test
```

## Configuration

### Configuration Priority

Configuration is loaded in the following order (later sources override earlier):

1. `Config/appsettings.json` (base settings)
2. `Config/appsettings.{Environment}.json` (environment-specific)
3. Environment variables (highest priority)

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `TEST_ENVIRONMENT` | Environment name (Development/Staging/Production) | Development |
| `BROWSER_TYPE` | Browser to use (Chrome/Firefox/Edge/Safari) | Chrome |
| `BROWSER_HEADLESS` | Run browser in headless mode | false |
| `APP_BASE_URL` | Application base URL | https://example.com |
| `API_BASE_URL` | API base URL | https://api.example.com |
| `TEST_USERNAME` | Test user username | - |
| `TEST_PASSWORD` | Test user password | - |
| `API_KEY` | API key for authentication | - |

### Browser Configuration

Edit `Config/appsettings.json`:

```json
{
  "Browser": {
    "Type": "Chrome",
    "Headless": false,
    "ImplicitWait": 10,
    "ExplicitWait": 30,
    "PageLoadTimeout": 60,
    "WindowSize": {
      "Width": 1920,
      "Height": 1080
    }
  }
}
```

## Writing Tests

### Feature Files (Gherkin)

Create feature files in the `Features/` directory:

```gherkin
@ui @login
Feature: Login Functionality

  @smoke @positive
  Scenario: Successful login with valid credentials
    Given I am on the login page
    When I enter username "testuser"
    And I enter password "testpassword"
    And I click the login button
    Then I should be logged in successfully
```

### JSON Locators

Create locator files in the `Locators/` directory:

```json
{
  "pageName": "LoginPage",
  "pageUrl": "/login",
  "locators": {
    "usernameInput": {
      "type": "id",
      "value": "username",
      "description": "Username input field"
    },
    "passwordInput": {
      "type": "css",
      "value": "input[type='password']",
      "description": "Password input field"
    },
    "loginButton": {
      "type": "xpath",
      "value": "//button[@type='submit']",
      "description": "Login submit button"
    }
  }
}
```

**Supported Locator Types:**
- `id` - By ID
- `name` - By name attribute
- `className` or `class` - By class name
- `tagName` or `tag` - By tag name
- `linkText` or `link` - By link text
- `partialLinkText` or `partialLink` - By partial link text
- `cssSelector` or `css` - By CSS selector
- `xpath` - By XPath

### Page Objects

Create page objects in the `Pages/` directory:

```csharp
using CS_Selenium_SpecFlow.Core.Pages;

public class LoginPage : BasePage
{
    protected override string PageName => "LoginPage";

    public void EnterUsername(string username)
    {
        Type("usernameInput", username);
    }

    public void EnterPassword(string password)
    {
        Type("passwordInput", password);
    }

    public void ClickLoginButton()
    {
        Click("loginButton");
    }

    public void Login(string username, string password)
    {
        EnterUsername(username);
        EnterPassword(password);
        ClickLoginButton();
    }
}
```

### Step Definitions

Create step definitions in the `StepDefinitions/` directory:

```csharp
using TechTalk.SpecFlow;
using NUnit.Framework;

[Binding]
public class LoginSteps
{
    private readonly LoginPage _loginPage = new();

    [Given(@"I am on the login page")]
    public void GivenIAmOnTheLoginPage()
    {
        _loginPage.NavigateToPage();
    }

    [When(@"I enter username ""(.*)""")]
    public void WhenIEnterUsername(string username)
    {
        _loginPage.EnterUsername(username);
    }

    [Then(@"I should be logged in successfully")]
    public void ThenIShouldBeLoggedInSuccessfully()
    {
        Assert.That(_homePage.IsUserLoggedIn(), Is.True);
    }
}
```

### API Testing

```gherkin
@api
Feature: REST API Testing

  Scenario: Get user by ID
    Given I have an API client
    When I send a GET request to "/users/1"
    Then the response status code should be 200
    And the response JSON property "id" should be "1"
```

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run by Category/Tag

```bash
# Run smoke tests
dotnet test --filter "Category=smoke"

# Run UI tests only
dotnet test --filter "Category=ui"

# Run API tests only
dotnet test --filter "Category=api"

# Run multiple categories
dotnet test --filter "Category=smoke|Category=regression"
```

### Run Specific Feature

```bash
dotnet test --filter "FullyQualifiedName~Login"
```

### Run with Specific Browser

```bash
BROWSER_TYPE=Firefox dotnet test
```

### Run in Headless Mode

```bash
BROWSER_HEADLESS=true dotnet test
```

### Run with Specific Environment

```bash
TEST_ENVIRONMENT=Staging dotnet test
```

### Parallel Execution

```bash
dotnet test --parallel
```

## Reporting

### Extent Reports

Reports are automatically generated in `Reports/ExtentReports/` after test execution.

Open the HTML report in a browser to view:
- Test execution summary
- Pass/Fail statistics
- Screenshots on failure
- Step-by-step execution details

### Allure Reports

1. Install Allure CLI:
   ```bash
   # macOS
   brew install allure

   # Windows (Scoop)
   scoop install allure
   ```

2. Generate and open report:
   ```bash
   allure serve allure-results
   ```

### Screenshots

Screenshots are automatically captured on test failure and saved to `Reports/Screenshots/`.

## CI/CD Integration

### GitHub Actions (Step-by-Step Guide)

GitHub Actions is a CI/CD service built into GitHub. If you're new to it, follow these steps:

#### Prerequisites
- Your code must be hosted on GitHub (public or private repository)
- The workflow file `.github/workflows/test.yml` is already included in this framework

#### Step 1: Push Code to GitHub

```bash
# Initialize git (if not already done)
git init

# Add all files
git add .

# Commit
git commit -m "Initial commit - Test automation framework"

# Add your GitHub repository as remote
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git

# Push to GitHub
git push -u origin main
```

#### Step 2: Configure GitHub Secrets

Secrets store sensitive data like passwords and API keys securely.

1. Go to your repository on GitHub
2. Click **Settings** (tab at the top)
3. In the left sidebar, click **Secrets and variables** → **Actions**
4. Click **New repository secret**
5. Add each secret one by one:

| Secret Name | Description | Example Value |
|-------------|-------------|---------------|
| `APP_BASE_URL` | Your application URL | `https://myapp.com` |
| `API_BASE_URL` | Your API URL | `https://api.myapp.com` |
| `TEST_USERNAME` | Test user login | `testuser@example.com` |
| `TEST_PASSWORD` | Test user password | `SecurePassword123` |
| `API_KEY` | API key (if needed) | `sk-abc123...` |

> **Note:** Secrets are encrypted and never exposed in logs.

#### Step 3: Verify Workflow File Exists

Ensure `.github/workflows/test.yml` exists in your repository. It's already created by this framework.

#### Step 4: Trigger the Workflow

The workflow runs automatically on:
- **Push** to `main` or `develop` branches
- **Pull requests** to `main` or `develop`

To run manually:
1. Go to your repository on GitHub
2. Click **Actions** tab
3. Select **Test Automation** workflow from the left
4. Click **Run workflow** button (dropdown on right)
5. Select options:
   - Environment: Development/Staging/Production
   - Browser: Chrome/Firefox/Edge
   - Tags: (optional) e.g., `@smoke`
6. Click **Run workflow**

#### Step 5: View Results

1. Go to **Actions** tab
2. Click on the running/completed workflow
3. View logs, download artifacts (test results, screenshots)

#### Workflow Features

The included workflow supports:
- **Multi-browser testing**: Chrome, Firefox, Edge
- **Environment selection**: Dev, Staging, Production
- **Tag filtering**: Run specific test categories
- **Artifacts**: Test results, screenshots, Allure reports
- **Allure Reports**: Auto-published to GitHub Pages

#### Example: Running Smoke Tests Only

1. Go to Actions → Test Automation → Run workflow
2. Set Tags to `@smoke`
3. Run workflow

#### Troubleshooting GitHub Actions

**Workflow not running?**
- Check if `.github/workflows/test.yml` exists
- Verify branch name matches trigger conditions

**Secrets not working?**
- Secret names are case-sensitive
- Secrets are only available in the repository where they're defined

**Tests failing in CI but passing locally?**
- CI runs in headless mode by default
- Check if all required secrets are configured
- Review the workflow logs for detailed errors

### Running Without GitHub Actions

If you can't use GitHub Actions, you can still run tests:

#### Option 1: Run Locally
```bash
# Set environment variables
export APP_BASE_URL="https://myapp.com"
export TEST_USERNAME="testuser"
export TEST_PASSWORD="password"

# Run tests
dotnet test
```

#### Option 2: Use Jenkins
The `Jenkinsfile` is already included. See Jenkins section below.

#### Option 3: Any Other CI Tool
Just run these commands in your CI pipeline:
```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --logger "trx;LogFileName=results.trx"
```

### Jenkins

1. Install required plugins:
   - .NET SDK Support
   - Allure Jenkins Plugin
   - MSTest Plugin

2. Configure credentials in Jenkins:
   - `app-base-url`
   - `api-base-url`
   - `test-username`
   - `test-password`
   - `api-key`

3. Create a Pipeline job pointing to the `Jenkinsfile`

### Azure DevOps

Create `azure-pipelines.yml`:
```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '10.x'

- script: dotnet restore
  displayName: 'Restore'

- script: dotnet build --configuration Release
  displayName: 'Build'

- script: dotnet test --configuration Release --logger trx
  displayName: 'Test'
  env:
    APP_BASE_URL: $(APP_BASE_URL)
    TEST_USERNAME: $(TEST_USERNAME)
    TEST_PASSWORD: $(TEST_PASSWORD)

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/*.trx'
```

## Best Practices

### Locator Strategy

1. **Prefer stable locators**: ID > Name > CSS > XPath
2. **Avoid dynamic attributes**: Don't use auto-generated IDs
3. **Use data attributes**: `data-testid="login-button"`
4. **Keep locators in JSON files**: Easy maintenance and reuse

### Page Object Model

1. **One page = One class**: Keep page objects focused
2. **Encapsulate actions**: Methods should represent user actions
3. **Return page objects**: For fluent chaining
4. **No assertions in page objects**: Keep assertions in step definitions

### Test Data

1. **Externalize test data**: Use JSON files in `TestData/`
2. **Use environment variables**: For sensitive data
3. **Data-driven tests**: Use Scenario Outlines with Examples

### Parallel Execution

1. **Thread-safe design**: Use ThreadLocal for driver instances
2. **Independent tests**: Tests should not depend on each other
3. **Unique test data**: Avoid data conflicts between parallel tests

## Error Handling

The framework includes comprehensive error handling throughout:

### Driver Errors
- **Driver not initialized**: Clear message if you try to use driver before calling `InitializeDriver()`
- **Browser not found**: Helpful error if browser isn't installed
- **Navigation failures**: Detailed errors with URL that failed

### Locator Errors
- **Missing locator file**: Error specifies which JSON file is missing
- **Missing element in JSON**: Error tells you which element isn't defined
- **Element not found on page**: Timeout error with element name and page

### API Errors
- **Missing base URL**: Clear message to configure `API_BASE_URL`
- **Request failures**: Logs full request details and error
- **Deserialization errors**: Specifies the type that failed to deserialize

### Example Error Messages

```
❌ Element 'loginButton' not found on page 'LoginPage'. Check if the locator is correct in LoginPage.json

❌ Base URL is not configured. Set APP_BASE_URL environment variable or configure in appsettings.json

❌ Failed to initialize Chrome browser. Ensure the browser is installed.

❌ Locator file not found: /path/to/Locators/LoginPage.json
```

### Logging

All errors are logged with full stack traces to `Logs/` directory. Enable debug logging for more details:

```json
{
  "Logging": {
    "Level": "Debug"
  }
}
```

## Troubleshooting

### Common Issues

**1. WebDriver not found**
```bash
# WebDriverManager handles this automatically
# If issues persist, ensure Chrome/Firefox is installed
```

**2. Element not found**
```bash
# Increase explicit wait time in appsettings.json
# Check if locator is correct in JSON file
# Verify element is visible and not in iframe
```

**3. Tests fail in headless mode**
```bash
# Some elements may behave differently in headless
# Try increasing window size
# Check for JavaScript errors in console
```

**4. Configuration not loading**
```bash
# Ensure appsettings.json is copied to output
# Check TEST_ENVIRONMENT variable is set correctly
```

### Debug Mode

Enable verbose logging:

```json
{
  "Logging": {
    "Level": "Debug"
  }
}
```

### Getting Help

1. Check the logs in `Logs/` directory
2. Review screenshots in `Reports/Screenshots/`
3. Enable debug logging for more details

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests locally
5. Submit a pull request

## License

This project is licensed under the MIT License.

---

**Happy Testing!**
