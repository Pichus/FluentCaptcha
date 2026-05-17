using FluentCaptcha.CloudflareTurnstile;
using FluentCaptcha.Core;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Dummy;
using FluentCaptcha.NSwag;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApiDocument((options, serviceProvider) =>
{
    options.IntegrateWithFluentCaptcha(serviceProvider);
});
builder.Services.AddFluentCaptcha(options =>
{
    options.UseCloudflareTurnstile(cloudflareOptions =>
    {
        // cloudflareOptions.SecretKey = CloudflareTurnstileConstants.TestSecretKeys.AlwaysPassValidation;
    });

    options.AddDummy();

    options.DefaultCaptchaResponseTokenSource = CaptchaResponseTokenSource.RequestHeader;

    options.DefaultCaptchaResponseTokenRequestHeaderName = "token";
});
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();


if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.Run();