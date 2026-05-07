using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FluentCaptcha.Core.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentCaptcha.Core;

public class CloudflareCaptchaValidator : ICaptchaValidator
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CloudflareCaptchaValidator> _logger;
    private readonly CloudflareTurnstileCaptchaOptions _turnstileCaptchaOptions;

    public CloudflareCaptchaValidator(HttpClient httpClient, ILogger<CloudflareCaptchaValidator> logger,
        IOptions<CloudflareTurnstileCaptchaOptions> turnstileCaptchaOptions)
    {
        _httpClient = httpClient;
        _logger = logger;
        _turnstileCaptchaOptions = turnstileCaptchaOptions.Value;
    }

    public async Task<CaptchaValidationResult> ValidateAsync(string captchaResponseToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "secret", _turnstileCaptchaOptions.SecretKey },
            { "response", captchaResponseToken }
        };

        // if (!string.IsNullOrEmpty(remoteIp)) parameters.Add("remoteip", remoteIp);

        var postContent = new FormUrlEncodedContent(parameters);

        using var response = await SendTokenVerificationRequestAsync(postContent);

        if (!response.IsSuccessStatusCode)
        {
            const string errorMessage = "Cloudflare Turnstile API verification failed.";
            _logger.LogError(
                errorMessage + " Status code: {StatusCode}. Response: {Response}",
                response.StatusCode,
                await response.Content.ReadAsStringAsync());

            return CaptchaValidationResult.Failure(errorMessage);
        }

        var deserializedResponse = await DeserializeResponseAsync(response);

        if (!deserializedResponse.Success)
        {
            if (deserializedResponse.ErrorCodes is null || deserializedResponse.ErrorCodes.Count == 0)
                return CaptchaValidationResult.Failure("CAPTCHA token validation failed. No error codes provided.");

            return CaptchaValidationResult.Failure(deserializedResponse.ErrorCodes);
        }

        return CaptchaValidationResult.Success();
    }


    private async Task<HttpResponseMessage> SendTokenVerificationRequestAsync(FormUrlEncodedContent request)
    {
        return await _httpClient.PostAsync(_turnstileCaptchaOptions.SiteVerifyUrl, request);
        // try
        // {
        //     var response = await _httpClient.PostAsync(_turnstileCaptchaOptions.SiteVerifyUrl, request);
        //
        //     return Result.Ok(response);
        // }
        // catch (InvalidOperationException invalidOperationException)
        // {
        //     const string errorMessage = "Invalid Cloudflare Turnstile API endpoint URL format.";
        //     _logger.LogCritical(invalidOperationException, errorMessage);
        //
        //     return Result.Fail(new InternalError(errorMessage));
        // }
        // catch (Exception requestException) when (requestException is HttpRequestException or TaskCanceledException)
        // {
        //     const string errorMessage = "Cloudflare Turnstile API request failed.";
        //     _logger.LogError(requestException, errorMessage);
        //
        //     return Result.Fail(new InternalError(errorMessage));
        // }
    }

    private async Task<CaptchaVerificationResponse> DeserializeResponseAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<CaptchaVerificationResponse>() ??
               new CaptchaVerificationResponse();
        // try
        // {
        //     var deserializedResponse = await response.Content.ReadFromJsonAsync<CaptchaVerificationResponse>();
        //
        //     if (deserializedResponse is null)
        //     {
        //         const string errorMessage = "Failed to deserialize Cloudflare Turnstile API response. Result is null.";
        //         _logger.LogError(errorMessage);
        //
        //         return Result.Fail(new InternalError(errorMessage));
        //     }
        //
        //     return deserializedResponse!;
        // }
        // catch (Exception exception)
        // {
        //     const string errorMessage = "Failed to deserialize Cloudflare Turnstile API response.";
        //
        //     _logger.LogError(exception, errorMessage);
        //
        //     return Result.Fail(new InternalError(errorMessage));
        // }
    }

    private class CaptchaVerificationResponse
    {
        [JsonPropertyName("success")] public bool Success { get; init; }

        [JsonPropertyName("error-codes")] public List<string>? ErrorCodes { get; init; }
    }
}