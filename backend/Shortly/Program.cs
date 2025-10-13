using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shortly.Configuration.Options;
using Shortly.Domain.Identity;
using Shortly.Domain.Services;
using Shortly.Infrastructure.Data;
using Shortly.Infrastructure.Security;
using Shortly.Infrastructure.Services;
using Shortly.Infrastructure.Utilities;

namespace Shortly;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // controller
        builder.Services.AddControllers();

        // swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // db
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
        });

        // options
        builder.Services.AddOptionsWithValidateOnStart<DefaultAdminOptions>()
            .Bind(builder.Configuration.GetSection("DefaultAdmin"))
            .ValidateDataAnnotations();

        builder.Services.AddOptionsWithValidateOnStart<ShortCodeOptions>()
            .Bind(builder.Configuration.GetSection("ShortCode"))
            .ValidateDataAnnotations();

        builder.Services.AddOptionsWithValidateOnStart<SecurityOptions>()
            .Bind(builder.Configuration.GetSection("Security"))
            .ValidateDataAnnotations();

        builder.Services.AddOptionsWithValidateOnStart<SecretDerivationOptions>()
            .Bind(builder.Configuration.GetSection("Security:SecretDerivation"))
            .ValidateDataAnnotations()
            .Validate(config => HashAlgorithmParser.TryGetHashAlgorithmByName(config.HashAlgorithmName, out _));

        builder.Services.AddOptionsWithValidateOnStart<JwtOptions>()
            .Bind(builder.Configuration.GetSection("Security:Jwt"))
            .ValidateDataAnnotations();

        // identity
        builder.Services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;

            options.User.RequireUniqueEmail = true;

            options.ClaimsIdentity.EmailClaimType = ClaimTypes.Email;
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
            options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
            options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;
        }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        // services
        builder.Services.AddSingleton<IGeolocationService, MaxMindGeolocationService>();
        builder.Services.AddSingleton<ISecretDerivationService, SecretDerivationService>();
        builder.Services.AddSingleton<IShortCodeGenerator, ShortCodeGenerator>();
        builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

        builder.Services.AddScoped<IAccessKeysService, AccessKeyService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<IShortLinksService, ShortLinksService>();
        builder.Services.AddScoped<IShortLinkEngagementsService, ShortLinkEngagementsService>();

        // auth
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        builder.Services.AddAuthorization();

        // configure jwt
        builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<ISecretDerivationService, IOptions<JwtOptions>>((options, secrets, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                var secret = secrets.GetJwtSigningKey();
                var key = new SymmetricSecurityKey(secret);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(jwt.ClockSkewMinutes),
                    IssuerSigningKey = key,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        // cors
        var origin = builder.Configuration.GetRequiredSection("Frontend").GetValue<string>("Origin")!;
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });

            //options.AddPolicy("frontend", policy =>
            //{
            //    policy.WithOrigins(origin)
            //        .AllowAnyHeader()
            //        .AllowAnyMethod()
            //        .AllowCredentials();
            //});
        });

        // build
        var app = builder.Build();

        await app.Services.MigrateAndSeedDbAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors();
        
        app.UseAuthentication();
        app.UseAuthorization();

        // add middleware

        app.MapControllers();

        await app.RunAsync();
    }
}
