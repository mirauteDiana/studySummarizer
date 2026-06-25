using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StudySummarizer.API.Authentication;
using StudySummarizer.Domain.Entities;
using StudySummarizer.Application.Interfaces;
using StudySummarizer.Application.Mappings;
using StudySummarizer.Application.Services;
using StudySummarizer.Application.Validators;
using StudySummarizer.Domain.Interfaces;
using StudySummarizer.Infrastructure.LlamaClient;
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
{
    o.UseInlineDefinitionsForEnums();
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });
    o.DocumentFilter<BearerSecurityDocumentFilter>();
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
        else
        {
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? throw new InvalidOperationException("Configuration key 'Cors:AllowedOrigins' is required in non-Development environments.");
            policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
        }
    }));

var dbFileName = builder.Configuration["Database:FileName"]
    ?? throw new InvalidOperationException("Configuration key 'Database:FileName' is required.");
var dbPath = Path.Combine(builder.Environment.ContentRootPath, dbFileName);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var uploadFolder = builder.Configuration["Storage:UploadPath"]
    ?? throw new InvalidOperationException("Configuration key 'Storage:UploadPath' is required.");
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, uploadFolder);
builder.Services.AddSingleton<IFileStorageService>(_ => new LocalFileStorageService(uploadPath));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<DocumentMappingProfile>());

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

builder.Services.AddScoped<ISummaryRepository, SummaryRepository>();
builder.Services.AddScoped<ISummarizationService, SummarizationService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
builder.Services.AddHttpClient<ILlamaClient, OllamaClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
});

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddOptions<JwtOptions>()
    .Bind(jwtSection)
    .ValidateDataAnnotations()
    .Validate(
        jwt => Encoding.UTF8.GetByteCount(jwt.Secret) >= JwtOptions.MinimumSecretBytes,
        $"Jwt:Secret must be at least {JwtOptions.MinimumSecretBytes} bytes when encoded as UTF-8.")
    .ValidateOnStart();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException("Configuration section 'Jwt' is required.");

        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

builder.WebHost.ConfigureKestrel(options =>
    options.Limits.MaxRequestBodySize = 20 * 1024 * 1024);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
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
    app.UseSwaggerUI(o =>
    {
        o.ConfigObject.PersistAuthorization = true;
    });
}

app.UseCors();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
