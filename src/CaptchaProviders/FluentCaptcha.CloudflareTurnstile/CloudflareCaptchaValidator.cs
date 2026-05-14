using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentCaptcha.CloudflareTurnstile;

public class CloudflareCaptchaValidator : ICaptchaValidator
{
    private readonly CloudflareTurnstileOptions _cloudflareTurnstileOptions;
    private readonly CloudflareTurnstileHttpClient _httpClient;
    private readonly ILogger<CloudflareCaptchaValidator> _logger;

    public CloudflareCaptchaValidator(
        CloudflareTurnstileHttpClient httpClient,
        ILogger<CloudflareCaptchaValidator> logger,
        IOptions<CloudflareTurnstileOptions> turnstileCaptchaOptions)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cloudflareTurnstileOptions = turnstileCaptchaOptions.Value;
    }

    public async Task<CaptchaValidationResult> ValidateAsync(
        string captchaResponseToken,
        string? remoteIp = null,
        string? expectedAction = null,
        CancellationToken cancellationToken = default)
    {
        var maxNumberOfAttempts = 1;

        if (_cloudflareTurnstileOptions.RetryOnFailure)
        {
            maxNumberOfAttempts += _cloudflareTurnstileOptions.MaxRetryCount;
        }

        for (var currentAttemptNumber = 1; currentAttemptNumber <= maxNumberOfAttempts; currentAttemptNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var response = await _httpClient.PerformRequestToCloudflareTurnstileApi(
                    captchaResponseToken, remoteIp, cancellationToken);

                var clientAction = response.Action;

                var expectedActionIsSet = expectedAction is not null;

                var clientActionReceived = clientAction is not null;

                if (expectedActionIsSet && !clientActionReceived)
                {
                    const string error =
                        "Expected captcha action is set, but no actual action is received from the client.";
                    _logger.LogWarning(error);
                }

                if (clientActionReceived && !expectedActionIsSet)
                {
                    const string error =
                        "Actual action is received from the client, but not expected action is configured for this endpoint.";
                    _logger.LogWarning(error);
                }

                if (clientActionReceived && expectedActionIsSet && clientAction != expectedAction)
                {
                    return CaptchaValidationResult.Failure(
                        "CAPTCHA token validation failed. Provided action does not match the expected one");
                }

                var result = ResponseToResult(response);

                if (result.IsSuccess || currentAttemptNumber == maxNumberOfAttempts)
                {
                    return result;
                }
            }
            catch (FluentCaptchaNetworkErrorException networkErrorException)
            {
                if (currentAttemptNumber == maxNumberOfAttempts)
                {
                    return CaptchaValidationResult.Failure(networkErrorException.Message);
                }
            }

            await Task.Delay((int)Math.Pow(2, currentAttemptNumber) * 1000, cancellationToken);
        }

        return CaptchaValidationResult.Failure("CAPTCHA token validation failed. No error codes provided.");
    }

    private static CaptchaValidationResult ResponseToResult(CloudflareTurnstileCaptchaVerificationResponse response)
    {
        if (!response.Success)
        {
            if (response.ErrorCodes is null || response.ErrorCodes.Count == 0)
            {
                return CaptchaValidationResult.Failure("CAPTCHA token validation failed. No error codes provided.");
            }

            return CaptchaValidationResult.Failure(response.ErrorCodes);
        }

        return CaptchaValidationResult.Success();
    }
}