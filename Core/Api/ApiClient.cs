using RestSharp;
using Newtonsoft.Json;
using CS_Selenium_SpecFlow.Core.Configuration;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Core.Api;

/// <summary>
/// REST API client for API testing
/// </summary>
public class ApiClient : IDisposable
{
    private readonly RestClient _client;
    private readonly Dictionary<string, string> _defaultHeaders;
    private bool _disposed;

    public RestResponse? LastResponse { get; private set; }

    public ApiClient(string? baseUrl = null)
    {
        var url = baseUrl ?? ConfigurationManager.ApiBaseUrl;
        
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("API Base URL is not configured. Set API_BASE_URL environment variable or configure in appsettings.json");
        }

        try
        {
            _client = new RestClient(url);
            _defaultHeaders = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Accept", "application/json" }
            };
            
            Logger.Info($"API Client initialized with base URL: {url}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to initialize API client with URL: {url}");
            throw new InvalidOperationException($"Failed to initialize API client. Invalid base URL: {url}", ex);
        }
    }

    #region Header Management

    public void AddDefaultHeader(string name, string value)
    {
        _defaultHeaders[name] = value;
    }

    public void SetBearerToken(string token)
    {
        _defaultHeaders["Authorization"] = $"Bearer {token}";
    }

    public void SetApiKey(string apiKey, string headerName = "X-API-Key")
    {
        _defaultHeaders[headerName] = apiKey;
    }

    public void RemoveHeader(string name)
    {
        _defaultHeaders.Remove(name);
    }

    public void ClearHeaders()
    {
        _defaultHeaders.Clear();
    }

    #endregion

    #region Request Methods

    public async Task<RestResponse> GetAsync(string endpoint, Dictionary<string, string>? queryParams = null)
    {
        var request = CreateRequest(endpoint, Method.Get);
        
        if (queryParams != null)
        {
            foreach (var param in queryParams)
            {
                request.AddQueryParameter(param.Key, param.Value);
            }
        }

        return await ExecuteAsync(request);
    }

    public async Task<RestResponse> PostAsync(string endpoint, object? body = null)
    {
        var request = CreateRequest(endpoint, Method.Post);
        
        if (body != null)
        {
            request.AddJsonBody(body);
        }

        return await ExecuteAsync(request);
    }

    public async Task<RestResponse> PutAsync(string endpoint, object? body = null)
    {
        var request = CreateRequest(endpoint, Method.Put);
        
        if (body != null)
        {
            request.AddJsonBody(body);
        }

        return await ExecuteAsync(request);
    }

    public async Task<RestResponse> PatchAsync(string endpoint, object? body = null)
    {
        var request = CreateRequest(endpoint, Method.Patch);
        
        if (body != null)
        {
            request.AddJsonBody(body);
        }

        return await ExecuteAsync(request);
    }

    public async Task<RestResponse> DeleteAsync(string endpoint)
    {
        var request = CreateRequest(endpoint, Method.Delete);
        return await ExecuteAsync(request);
    }

    #endregion

    #region Synchronous Methods

    public RestResponse Get(string endpoint, Dictionary<string, string>? queryParams = null)
    {
        return GetAsync(endpoint, queryParams).GetAwaiter().GetResult();
    }

    public RestResponse Post(string endpoint, object? body = null)
    {
        return PostAsync(endpoint, body).GetAwaiter().GetResult();
    }

    public RestResponse Put(string endpoint, object? body = null)
    {
        return PutAsync(endpoint, body).GetAwaiter().GetResult();
    }

    public RestResponse Patch(string endpoint, object? body = null)
    {
        return PatchAsync(endpoint, body).GetAwaiter().GetResult();
    }

    public RestResponse Delete(string endpoint)
    {
        return DeleteAsync(endpoint).GetAwaiter().GetResult();
    }

    #endregion

    #region Helper Methods

    private RestRequest CreateRequest(string endpoint, Method method)
    {
        var request = new RestRequest(endpoint, method);
        
        foreach (var header in _defaultHeaders)
        {
            request.AddHeader(header.Key, header.Value);
        }

        return request;
    }

    private async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Resource))
        {
            throw new ArgumentException("API endpoint cannot be null or empty");
        }

        try
        {
            Logger.Info($"API Request: {request.Method} {request.Resource}");
            
            LastResponse = await _client.ExecuteAsync(request);
            
            Logger.Info($"API Response: {(int)LastResponse.StatusCode} {LastResponse.StatusCode}");
            Logger.Debug($"Response Body: {LastResponse.Content}");

            // Log error responses
            if (LastResponse.ErrorException != null)
            {
                Logger.Error(LastResponse.ErrorException, $"API request failed: {LastResponse.ErrorMessage}");
            }

            return LastResponse;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to execute API request: {request.Method} {request.Resource}");
            throw new InvalidOperationException($"API request failed: {request.Method} {request.Resource}", ex);
        }
    }

    public T? DeserializeResponse<T>(RestResponse? response = null)
    {
        var resp = response ?? LastResponse;
        
        if (resp?.Content == null)
        {
            Logger.Warning("Attempted to deserialize null or empty response");
            return default;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(resp.Content);
        }
        catch (JsonException ex)
        {
            Logger.Error(ex, $"Failed to deserialize response to type {typeof(T).Name}");
            throw new InvalidOperationException($"Failed to deserialize API response to {typeof(T).Name}", ex);
        }
    }

    public dynamic? DeserializeResponseDynamic(RestResponse? response = null)
    {
        var resp = response ?? LastResponse;
        
        if (resp?.Content == null)
        {
            Logger.Warning("Attempted to deserialize null or empty response");
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<dynamic>(resp.Content);
        }
        catch (JsonException ex)
        {
            Logger.Error(ex, "Failed to deserialize response to dynamic object");
            throw new InvalidOperationException("Failed to deserialize API response", ex);
        }
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _client.Dispose();
            }
            _disposed = true;
        }
    }

    #endregion
}
