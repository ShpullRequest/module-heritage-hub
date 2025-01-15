using Microsoft.EntityFrameworkCore;
using ModuleHeritageHub.Domain.Config;
using ModuleHeritageHub.Infrastructure.DB;
using ModuleHeritageHub.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ModuleHeritageHub.Infrastructure.JWT;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<Jwt>();
builder.Services.AddScoped<JwtResolver>();

builder.Services.AddDbContext<DBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));
builder.Services.AddScoped<ImageRepository>();
builder.Services.AddScoped<PageRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("any", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.
    AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options => {
        var cfg = builder.Configuration.GetSection("JWT").Get<JwtConfig>() ?? throw new Exception("JWT config not found");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.SecretKey));

        options.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateLifetime = true,
                 ValidateAudience = false,
                 ValidateIssuer = false,
                 IssuerSigningKey = secretKey
             };
    });

var app = builder.Build();

app.UseCors("any");
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var uploadPath = Path.Combine(app.Environment.ContentRootPath, "static");
if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/static",
    FileProvider = new PhysicalFileProvider(uploadPath),
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
    }
});

app.Run();
