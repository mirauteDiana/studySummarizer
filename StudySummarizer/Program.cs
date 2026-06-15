using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using StudySummarizer.Application.Mappings;
using Microsoft.EntityFrameworkCore;
using StudySummarizer.Application.Interfaces;
using StudySummarizer.Application.Services;
using StudySummarizer.Application.Validators;
using StudySummarizer.Domain.Interfaces;
using StudySummarizer.Infrastructure.Persistence;
using StudySummarizer.Infrastructure.Repositories;
using StudySummarizer.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
    });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UploadDocumentFormValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.UseInlineDefinitionsForEnums());

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        else
            // TODO: replace with explicit allowed origins before deploying to production
            policy.WithOrigins("https://localhost").AllowAnyMethod().AllowAnyHeader();
    }));

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "studysummarizer.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
builder.Services.AddSingleton<IFileStorageService>(_ => new LocalFileStorageService(uploadPath));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<DocumentMappingProfile>());

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

builder.WebHost.ConfigureKestrel(options =>
    options.Limits.MaxRequestBodySize = 20 * 1024 * 1024);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseExceptionHandler(err => err.Run(async ctx =>
{
    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
    ctx.Response.ContentType = "application/problem+json";
    await ctx.Response.WriteAsJsonAsync(new
    {
        type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
        title = "An unexpected error occurred.",
        status = 500
    });
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
