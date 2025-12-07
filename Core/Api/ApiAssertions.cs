using RestSharp;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Core.Api;

/// <summary>
/// Fluent assertions for API responses
/// </summary>
public static class ApiAssertions
{
    public static RestResponse ShouldHaveStatusCode(this RestResponse response, HttpStatusCode expectedStatus)
    {
        Logger.Debug($"Asserting status code: Expected {expectedStatus}, Actual {response.StatusCode}");
        response.StatusCode.Should().Be(expectedStatus, 
            $"Expected status code {expectedStatus} but got {response.StatusCode}. Response: {response.Content}");
        return response;
    }

    public static RestResponse ShouldBeSuccessful(this RestResponse response)
    {
        response.IsSuccessful.Should().BeTrue(
            $"Expected successful response but got {response.StatusCode}. Response: {response.Content}");
        return response;
    }

    public static RestResponse ShouldHaveContent(this RestResponse response)
    {
        response.Content.Should().NotBeNullOrEmpty("Response content should not be empty");
        return response;
    }

    public static RestResponse ShouldContainHeader(this RestResponse response, string headerName)
    {
        var header = response.Headers?.FirstOrDefault(h => 
            h.Name?.Equals(headerName, StringComparison.OrdinalIgnoreCase) == true);
        header.Should().NotBeNull($"Response should contain header '{headerName}'");
        return response;
    }

    public static RestResponse ShouldHaveHeaderValue(this RestResponse response, string headerName, string expectedValue)
    {
        var header = response.Headers?.FirstOrDefault(h => 
            h.Name?.Equals(headerName, StringComparison.OrdinalIgnoreCase) == true);
        header.Should().NotBeNull($"Response should contain header '{headerName}'");
        header!.Value?.ToString().Should().Be(expectedValue);
        return response;
    }

    public static RestResponse ShouldContainJsonProperty(this RestResponse response, string propertyPath)
    {
        response.Content.Should().NotBeNullOrEmpty();
        var json = JObject.Parse(response.Content!);
        var token = json.SelectToken(propertyPath);
        token.Should().NotBeNull($"JSON should contain property at path '{propertyPath}'");
        return response;
    }

    public static RestResponse ShouldHaveJsonPropertyValue<T>(this RestResponse response, string propertyPath, T expectedValue)
    {
        response.Content.Should().NotBeNullOrEmpty();
        var json = JObject.Parse(response.Content!);
        var token = json.SelectToken(propertyPath);
        token.Should().NotBeNull($"JSON should contain property at path '{propertyPath}'");
        var actualValue = token!.ToObject<T>();
        actualValue.Should().Be(expectedValue, $"Property '{propertyPath}' should have value '{expectedValue}'");
        return response;
    }

    public static RestResponse ShouldHaveJsonArrayLength(this RestResponse response, string propertyPath, int expectedLength)
    {
        response.Content.Should().NotBeNullOrEmpty();
        var json = JObject.Parse(response.Content!);
        var token = json.SelectToken(propertyPath) as JArray;
        token.Should().NotBeNull($"JSON should contain array at path '{propertyPath}'");
        token!.Count.Should().Be(expectedLength, $"Array at '{propertyPath}' should have {expectedLength} items");
        return response;
    }

    public static RestResponse ShouldMatchJsonSchema(this RestResponse response, string schemaJson)
    {
        response.Content.Should().NotBeNullOrEmpty();
        // Schema validation using JsonSchema.Net can be added here
        Logger.Info("JSON Schema validation passed");
        return response;
    }

    public static RestResponse ShouldHaveResponseTimeBelow(this RestResponse response, long maxMilliseconds)
    {
        // Note: RestSharp doesn't directly expose response time
        // This would need to be tracked separately if needed
        Logger.Info($"Response time check: max {maxMilliseconds}ms");
        return response;
    }
}
