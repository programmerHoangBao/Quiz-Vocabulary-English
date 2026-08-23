using back_end.Configurations;
using back_end.Configurations.Settings;
using back_end.Data;
using back_end.Services.Interfaces;
using back_end.Services;
using Microsoft.EntityFrameworkCore;
using back_end.Repositories.Interfaces;
using back_end.Repositories;
using back_end.RabbitMQ.Interfaces;
using back_end.RabbitMQ;
using Microsoft.Extensions.Options;
using back_end.Middleware;
using back_end.Profiles;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  

builder.Services.AddControllers();
builder.Services.AddApiBehaviorConfiguration();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Format json input
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddDbContext<DBContext>(options =>
   options.UseNpgsql(
       builder.Configuration.GetConnectionString("DefaultConnection")
   )
);

builder.Services.Configure<AppSetting>(builder.Configuration.GetSection(AppSetting.SectionName));
builder.Services.Configure<SecuritySetting>(builder.Configuration.GetSection(SecuritySetting.SectionName));
builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection(SmtpSetting.SectionName));
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection(RabbitMQSetting.SectionName));

builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSetting>>().Value);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SecuritySetting>>().Value);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SmtpSetting>>().Value);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<RabbitMQSetting>>().Value);

//Add scoped for repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<IVocaboluryRepository, VocaboluryRepository>();

//Add scoped for services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IVocaboluryService, VocaboluryService>();
builder.Services.AddScoped<IFileService, FileService>();

//Background Services
builder.Services.AddHostedService<ExpiredUserCleanupService>();
builder.Services.AddHostedService<RabbitMqConsumer>();

// Auto mapper
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Jwt setting
var securitySetting = builder.Configuration
    .GetSection(SecuritySetting.SectionName)
    .Get<SecuritySetting>();
builder.Services.AddJWTAuthentication(securitySetting!);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
