using FluentCaptcha.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluentCaptcha.CloudflareTurnstile;

public sealed class CloudflareTurnstileHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CloudflareTurnstileHttpClient> _logger;
    private readonly CloudflareTurnstileOptions _options;

    public CloudflareTurnstileHttpClient(
        HttpClient httpClient,
        ILogger<CloudflareTurnstileHttpClient> logger,
        IOptions<CloudflareTurnstileOptions> options)
    {
        _options = options.Value;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CloudflareTurnstileCaptchaVerificationResponse> PerformRequestToCloudflareTurnstileApi(
        string captchaResponseToken,
        string? remoteIp = null,
        CancellationToken cancellationToken = default)
    {
        var idempotencyKey = Guid.NewGuid().ToString();

        var parameters = new Dictionary<string, string>
        {
            { "secret", _options.SecretKey },
            { "response", captchaResponseToken },
            { "idempotency_key", idempotencyKey }
        };

        if (!string.IsNullOrEmpty(remoteIp))
        {
            parameters.Add("remoteip", remoteIp);
        }

        var postContent = new FormUrlEncodedContent(parameters);

        using var response = await SendTokenVerificationRequestAsync(postContent, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            const string errorMessage = "Cloudflare Turnstile API verification failed.";
            _logger.LogError(
                errorMessage + " Status code: {StatusCode}. Response: {Response}",
                response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));

            throw new FluentCaptchaNetworkErrorException(errorMessage);
        }

        var deserializedResponse = await DeserializeResponseAsync(response, cancellationToken);

        return deserializedResponse;
    }

    private async Task<HttpResponseMessage> SendTokenVerificationRequestAsync(
        FormUrlEncodedContent request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(_options.SiteVerifyUrl, request, cancellationToken);

            return response;
        }
        catch (HttpRequestException httpRequestException)
        {
            const string errorMessage = "Cloudflare Turnstile API request failed.";
            _logger.LogError(httpRequestException, errorMessage);

            throw new FluentCaptchaNetworkErrorException(errorMessage, httpRequestException);
        }
        catch (Exception invalidUriException)
            when (invalidUriException is InvalidOperationException or UriFormatException)
        {
            const string errorMessage = "Invalid Cloudflare Turnstile API endpoint URL format.";
            _logger.LogCritical(invalidUriException, errorMessage);

            throw new FluentCaptchaConfigurationException(errorMessage, invalidUriException);
        }
    }

    private async Task<CloudflareTurnstileCaptchaVerificationResponse> DeserializeResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deserializedResponse =
                await response.Content.ReadFromJsonAsync<CloudflareTurnstileCaptchaVerificationResponse>(
                    cancellationToken);

            if (deserializedResponse is null)
            {
                const string errorMessage = "Failed to deserialize Cloudflare Turnstile API response. Result is null.";
                _logger.LogError(errorMessage);

                throw new FluentCaptchaErrorException(errorMessage);
            }

            return deserializedResponse;
        }
        catch (Exception exception)
            when (exception is JsonException or NotSupportedException or ArgumentNullException)
        {
            const string errorMessage = "Failed to deserialize Cloudflare Turnstile API response.";
            _logger.LogError(exception, errorMessage);

            throw new FluentCaptchaErrorException(errorMessage, exception);
        }
    }
}

public sealed class CloudflareTurnstileCaptchaVerificationResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("action")]
    public string? Action { get; init; }

    [JsonPropertyName("error-codes")]
    public List<string>? ErrorCodes { get; init; }
}