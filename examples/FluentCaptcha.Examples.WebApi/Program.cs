using FluentCaptcha.CloudflareTurnstile;
using FluentCaptcha.Core;
using FluentCaptcha.Dummy;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddFluentCaptcha(captchaOptions =>
{
    captchaOptions.UseCloudflareTurnstile(cloudflareOptions =>
    {
        cloudflareOptions.SecretKey = CloudflareTurnstileConstants.TestSecretKeys.AlwaysPassValidation;
    });
    captchaOptions.AddDummy();
});
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();