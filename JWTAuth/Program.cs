using System.Reflection;
using System.Text;
using Asp.Versioning;
using JWTAuth.Core;
using JWTAuth.Core.Swagger;
using JWTAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Ref: https://mohsen.es/api-versioning-and-swagger-in-asp-net-core-7-0-fe45f67d8419
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(c =>
{
    // Add a custom operation filter which sets default values. Ref: https://mohsen.es/api-versioning-and-swagger-in-asp-net-core-7-0-fe45f67d8419
    c.OperationFilter<SwaggerDefaultValues>();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.DocInclusionPredicate((docName, apiDesc) => apiDesc.GroupName == docName);

    // Uses full schema names to avoid v1/v2/v3 schema collisions
    // see: https://github.com/domaindrivendev/Swashbuckle/issues/442
    c.CustomSchemaIds(x => x.FullName);
});


// Asp.Versioning.Mvc.ApiExplorer  
// Asp.Versioning.Mvc 
builder.Services.AddApiVersioning(config =>
    {
        config.DefaultApiVersion = new ApiVersion(1, 0);
        // indicating whether a default version is assumed when a client does not provide an API version.
        config.AssumeDefaultVersionWhenUnspecified = true;
        config.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        // https://stackoverflow.com/questions/58601931/api-version-value-by-default-in-swagger-ui
        // Tells swagger to replace the version in the controller route  
        options.SubstituteApiVersionInUrl = true;

        // format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";

        // if we have both parts, decided how to format the group
        // from the example: "Sales - v1"   // https://github.com/dotnet/aspnet-api-versioning/discussions/1036
        options.FormatGroupName = (group, version) => $"{group}-{version}";
    });


// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0
builder.Services.Configure<AppSettingsOptions>(
    builder.Configuration.GetSection(AppSettingsOptions.AppSettings));

// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn?view=aspnetcore-8.0&tabs=windows
// Microsoft.AspNetCore.Authentication.JwtBearer
builder.Services.AddAuthorization(); 
// builder.Services.AddAuthentication("Bearer").AddJwtBearer();
// Jwt configuration starts here https://medium.com/@vndpal/how-to-implement-jwt-token-authentication-in-net-core-6-ab7f48470f5c
var jwtIssuer = builder.Configuration.GetSection("AppSettings:Jwt:Issuer").Get<string>();
var jwtAudience = builder.Configuration.GetSection("AppSettings:Jwt:Audience").Get<string>();
var jwtKey = builder.Configuration.GetSection("AppSettings:Jwt:SecretKey").Get<string>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    if (jwtKey != null)
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
});
// Jwt configuration ends here

builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IBookService, BookService>(); 


// https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-8.0
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddRequestTimeouts();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // build a swagger endpoint for each discovered API version
        foreach (var description in app.DescribeApiVersions())
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    
        options.DisplayOperationId();
        options.DisplayRequestDuration();
    }); 
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
