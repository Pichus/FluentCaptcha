using FluentCaptcha.CloudflareTurnstile;
using FluentCaptcha.Core;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Dummy;
using FluentCaptcha.NSwag;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApiDocument(options =>
{
    options.IntegrateWithFluentCaptcha();
});
builder.Services.AddFluentCaptcha(options =>
{
    options.UseCloudflareTurnstile(cloudflareOptions =>
    {
        cloudflareOptions.SecretKey = CloudflareTurnstileConstants.TestSecretKeys.AlwaysPassValidation;
    });

    options.AddDummy();

    options.DefaultCaptchaResponseTokenSource = CaptchaResponseTokenSource.RequestBody;
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