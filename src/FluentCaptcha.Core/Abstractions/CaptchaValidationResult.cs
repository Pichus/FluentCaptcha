namespace FluentCaptcha.Core.Abstractions;

public class CaptchaValidationResult
{
    private CaptchaValidationResult(IEnumerable<CaptchaValidationError> captchaValidationErrors)
    {
        CaptchaValidationErrors = captchaValidationErrors;
    }

    private CaptchaValidationResult()
    {
        CaptchaValidationErrors = [];
    }

    public bool IsSuccess => !IsFailure;

    public bool IsFailure => CaptchaValidationErrors.Any();

    public IEnumerable<CaptchaValidationError> CaptchaValidationErrors { get; }

    public static CaptchaValidationResult Success()
    {
        return new CaptchaValidationResult();
    }

    public static CaptchaValidationResult Failure(CaptchaValidationError error)
    {
        return new CaptchaValidationResult([error]);
    }

    public static CaptchaValidationResult Failure(IEnumerable<CaptchaValidationError> errors)
    {
        return new CaptchaValidationResult(errors);
    }

    public static CaptchaValidationResult Failure(string error)
    {
        return new CaptchaValidationResult([new CaptchaValidationError(error)]);
    }

    public static CaptchaValidationResult Failure(IEnumerable<string> errors)
    {
        return new CaptchaValidationResult(errors.Select(error => new CaptchaValidationError(error)));
    }
}