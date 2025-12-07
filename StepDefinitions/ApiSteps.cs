using Reqnroll;
using NUnit.Framework;
using System.Net;
using CS_Selenium_SpecFlow.Core.Api;
using CS_Selenium_SpecFlow.Core.Configuration;
using Newtonsoft.Json.Linq;

namespace CS_Selenium_SpecFlow.StepDefinitions;

/// <summary>
/// Step definitions for REST API testing
/// </summary>
[Binding]
public class ApiSteps
{
    private readonly ScenarioContext _scenarioContext;
    private ApiClient? _apiClient;
    private RestSharp.RestResponse? _response;

    public ApiSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private ApiClient ApiClient => _apiClient ??= new ApiClient();

    [Given(@"I have an API client")]
    public void GivenIHaveAnApiClient()
    {
        _apiClient = new ApiClient();
    }

    [Given(@"I have an API client with base URL ""(.*)""")]
    public void GivenIHaveAnApiClientWithBaseUrl(string baseUrl)
    {
        _apiClient = new ApiClient(baseUrl);
    }

    [Given(@"I set the API key")]
    public void GivenISetTheApiKey()
    {
        ApiClient.SetApiKey(ConfigurationManager.ApiKey);
    }

    [Given(@"I set the API key ""(.*)""")]
    public void GivenISetTheApiKey(string apiKey)
    {
        ApiClient.SetApiKey(apiKey);
    }

    [Given(@"I set the bearer token ""(.*)""")]
    public void GivenISetTheBearerToken(string token)
    {
        ApiClient.SetBearerToken(token);
    }

    [Given(@"I set header ""(.*)"" to ""(.*)""")]
    public void GivenISetHeaderTo(string headerName, string headerValue)
    {
        ApiClient.AddDefaultHeader(headerName, headerValue);
    }

    [When(@"I send a GET request to ""(.*)""")]
    public void WhenISendAGetRequestTo(string endpoint)
    {
        _response = ApiClient.Get(endpoint);
    }

    [When(@"I send a GET request to ""(.*)"" with query parameters")]
    public void WhenISendAGetRequestToWithQueryParameters(string endpoint, Table table)
    {
        var queryParams = table.Rows.ToDictionary(row => row["Key"], row => row["Value"]);
        _response = ApiClient.Get(endpoint, queryParams);
    }

    [When(@"I send a POST request to ""(.*)""")]
    public void WhenISendAPostRequestTo(string endpoint)
    {
        _response = ApiClient.Post(endpoint);
    }

    [When(@"I send a POST request to ""(.*)"" with body")]
    public void WhenISendAPostRequestToWithBody(string endpoint, string body)
    {
        var jsonBody = JObject.Parse(body);
        _response = ApiClient.Post(endpoint, jsonBody);
    }

    [When(@"I send a POST request to ""(.*)"" with JSON")]
    public void WhenISendAPostRequestToWithJson(string endpoint, Table table)
    {
        var body = new JObject();
        foreach (var row in table.Rows)
        {
            body[row["Key"]] = row["Value"];
        }
        _response = ApiClient.Post(endpoint, body);
    }

    [When(@"I send a PUT request to ""(.*)"" with body")]
    public void WhenISendAPutRequestToWithBody(string endpoint, string body)
    {
        var jsonBody = JObject.Parse(body);
        _response = ApiClient.Put(endpoint, jsonBody);
    }

    [When(@"I send a PATCH request to ""(.*)"" with body")]
    public void WhenISendAPatchRequestToWithBody(string endpoint, string body)
    {
        var jsonBody = JObject.Parse(body);
        _response = ApiClient.Patch(endpoint, jsonBody);
    }

    [When(@"I send a DELETE request to ""(.*)""")]
    public void WhenISendADeleteRequestTo(string endpoint)
    {
        _response = ApiClient.Delete(endpoint);
    }

    [Then(@"the response status code should be (.*)")]
    public void ThenTheResponseStatusCodeShouldBe(int statusCode)
    {
        Assert.That(_response, Is.Not.Null, "Response should not be null");
        Assert.That((int)_response!.StatusCode, Is.EqualTo(statusCode), 
            $"Expected status code {statusCode} but got {(int)_response.StatusCode}");
    }

    [Then(@"the response should be successful")]
    public void ThenTheResponseShouldBeSuccessful()
    {
        Assert.That(_response, Is.Not.Null, "Response should not be null");
        Assert.That(_response!.IsSuccessful, Is.True, 
            $"Expected successful response but got {_response.StatusCode}: {_response.Content}");
    }

    [Then(@"the response should contain ""(.*)""")]
    public void ThenTheResponseShouldContain(string expectedContent)
    {
        Assert.That(_response?.Content, Does.Contain(expectedContent), 
            $"Response should contain '{expectedContent}'");
    }

    [Then(@"the response JSON should have property ""(.*)""")]
    public void ThenTheResponseJsonShouldHaveProperty(string propertyPath)
    {
        Assert.That(_response?.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");
        var json = JObject.Parse(_response!.Content!);
        var token = json.SelectToken(propertyPath);
        Assert.That(token, Is.Not.Null, $"JSON should have property at path '{propertyPath}'");
    }

    [Then(@"the response JSON property ""(.*)"" should be ""(.*)""")]
    public void ThenTheResponseJsonPropertyShouldBe(string propertyPath, string expectedValue)
    {
        Assert.That(_response?.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");
        var json = JObject.Parse(_response!.Content!);
        var token = json.SelectToken(propertyPath);
        Assert.That(token, Is.Not.Null, $"JSON should have property at path '{propertyPath}'");
        Assert.That(token!.ToString(), Is.EqualTo(expectedValue), 
            $"Property '{propertyPath}' should be '{expectedValue}'");
    }

    [Then(@"the response JSON array ""(.*)"" should have (.*) items")]
    public void ThenTheResponseJsonArrayShouldHaveItems(string propertyPath, int count)
    {
        Assert.That(_response?.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");
        var json = JObject.Parse(_response!.Content!);
        var array = json.SelectToken(propertyPath) as JArray;
        Assert.That(array, Is.Not.Null, $"JSON should have array at path '{propertyPath}'");
        Assert.That(array!.Count, Is.EqualTo(count), $"Array should have {count} items");
    }

    [Then(@"I store response property ""(.*)"" as ""(.*)""")]
    public void ThenIStoreResponsePropertyAs(string propertyPath, string key)
    {
        Assert.That(_response?.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");
        var json = JObject.Parse(_response!.Content!);
        var token = json.SelectToken(propertyPath);
        Assert.That(token, Is.Not.Null, $"JSON should have property at path '{propertyPath}'");
        _scenarioContext[key] = token!.ToString();
    }

    [AfterScenario]
    public void CleanupApiClient()
    {
        _apiClient?.Dispose();
    }
}
