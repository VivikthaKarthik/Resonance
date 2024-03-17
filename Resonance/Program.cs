using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using ResoClassAPI.Authentication;
using Serilog;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Services;
using Microsoft.OpenApi.Models;
using AutoMapper;
using Resonance;
using ResoClassAPI.Middleware;
using ResoClassAPI.Models.Domain;
using ResoClassAPI.Utilities.Interfaces;
using ResoClassAPI.Utilities;
using ResoClassAPI.Interceptors;
using Microsoft.AspNetCore.Authorization;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<ISubTopicService, SubTopicService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IScheduledExamService, ScheduledExamService>();

builder.Services.AddScoped<IExcelReader, ExcelReader>();
builder.Services.AddScoped<IWordReader, WordReader>();
builder.Services.AddScoped<IAwsHandler, AwsHandler>();
builder.Services.AddSingleton<AuditInterceptor>();
builder.Services.AddSingleton<IAuthorizationHandler, HasPermissionHandler>();
IMapper mapper = MapperConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
    .AllowAnyHeader();
}));

//AWSOptions options = new AWSOptions
//{
//    Credentials = new BasicAWSCredentials("", "")
//};
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>(); 
//builder.Services.Configure<AWSOptions>(builder.Configuration.GetSection("AWS"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, AuthTokenHandler>(JwtBearerDefaults.AuthenticationScheme, null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.Requirements.Add(new HasPermissionRequirement("Admin")));
});

builder.Services.AddDbContext<ResoClassContext>((sp, options) =>
{
    var auditableInterceptor = sp.GetService<AuditInterceptor>();

    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString")).AddInterceptors(auditableInterceptor);
});

var app = builder.Build();

// Configure the HTTP request pipeline. 
//if (app.Environment.IsDevelopment()) UnComment this line when deploying in Prod
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("MyPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
