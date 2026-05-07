using FluentCaptcha.CloudflareTurnstile;
using FluentCaptcha.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddFluentCaptcha(captchaOptions =>
{
    captchaOptions.UseCloudflareTurnstile();
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