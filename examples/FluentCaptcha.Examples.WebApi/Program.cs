using FluentCaptcha.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddFluentCaptcha()
    .AddCloudflareCaptchaProvider(options => { options.SecretKey = "1x0000000000000000000000000000000AA"; });
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.Run();