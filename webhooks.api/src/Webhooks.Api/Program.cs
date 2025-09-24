using Webhooks.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterMessageQueuing(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

