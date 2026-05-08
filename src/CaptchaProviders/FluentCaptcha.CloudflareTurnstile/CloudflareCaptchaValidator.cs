using FluentCaptcha.CloudflareTurnstile.Options;
using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluentCaptcha.CloudflareTurnstile;

public class CloudflareCaptchaValidator : ICaptchaValidator
{
    private readonly CloudflareTurnstileOptions _cloudflareTurnstileOptions;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CloudflareCaptchaValidator> _logger;

    public CloudflareCaptchaValidator(HttpClient httpClient, ILogger<CloudflareCaptchaValidator> logger,
        IOptions<CloudflareTurnstileOptions> turnstileCaptchaOptions)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cloudflareTurnstileOptions = turnstileCaptchaOptions.Value;
    }

    public async Task<CaptchaValidationResult> ValidateAsync(
        string captchaResponseToken,
        string? remoteIp = null,
        CancellationToken cancellationToken = default)
    {
        var idempotencyKey = Guid.NewGuid().ToString();

        var parameters = new Dictionary<string, string>
        {
            { "secret", _cloudflareTurnstileOptions.SecretKey },
            { "response", captchaResponseToken },
            { "idempotency_key", idempotencyKey }
        };

        if (!string.IsNullOrEmpty(remoteIp))
        {
            parameters.Add("remoteip", remoteIp);
        }

        var postContent = new FormUrlEncodedContent(parameters);

        var maxAttempts = 1;

        if (_cloudflareTurnstileOptions.RetryOnFailure)
        {
            maxAttempts += _cloudflareTurnstileOptions.MaxRetryCount;
        }

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using var response = await SendTokenVerificationRequestAsync(postContent, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    const string errorMessage = "Cloudflare Turnstile API verification failed.";
                    _logger.LogError(
                        errorMessage + " Status code: {StatusCode}. Response: {Response}",
                        response.StatusCode,
                        await response.Content.ReadAsStringAsync(cancellationToken));

                    return CaptchaValidationResult.Failure(errorMessage);
                }

                var deserializedResponse = await DeserializeResponseAsync(response, cancellationToken);

                if (deserializedResponse.Success)
                {
                    return CaptchaValidationResult.Success();
                }

                if (attempt == maxAttempts)
                {
                    if (deserializedResponse.ErrorCodes is null || deserializedResponse.ErrorCodes.Count == 0)
                    {
                        return CaptchaValidationResult.Failure(
                            "CAPTCHA token validation failed. No error codes provided.");
                    }

                    return CaptchaValidationResult.Failure(deserializedResponse.ErrorCodes);
                }

                await Task.Delay((int)Math.Pow(2, attempt) * 1000, cancellationToken);
            }
            catch (FluentCaptchaNetworkErrorException networkErrorException)
            {
                if (attempt == maxAttempts)
                {
                    return CaptchaValidationResult.Failure(networkErrorException.Message);
                }
            }
        }

        return CaptchaValidationResult.Failure("CAPTCHA token validation failed. No error codes provided.");
    }


    private async Task<HttpResponseMessage> SendTokenVerificationRequestAsync(
        FormUrlEncodedContent request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                _cloudflareTurnstileOptions.SiteVerifyUrl, request, cancellationToken);

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

    private async Task<CaptchaVerificationResponse> DeserializeResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var deserializedResponse =
                await response.Content.ReadFromJsonAsync<CaptchaVerificationResponse>(cancellationToken);

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

    private class CaptchaVerificationResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("error-codes")]
        public List<string>? ErrorCodes { get; init; }
    }
}