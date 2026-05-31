using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sentry.Profiling;
using Swashbuckle.AspNetCore.Filters;
using TFGBack._01_Presentation.Handler;
using TFGBack._02_Domain.Infrastructure.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts;
using TFGBack._02_Domain.ServiceLibrary.Impl.Impl;
using TFGBack._03_Infrastructure.Data;
using TFGBack._03_Infrastructure.Database;
using TFGBack._03_Infrastructure;
using TFGBack._03_Infrastructure.Impl;
using TFGBack._03_Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IUserServiceRepository, UserServiceRepository>();
builder.Services.AddScoped<IBoardServiceRepository, BoardServiceRepository>();
builder.Services.AddScoped<ITaskServiceRepository, TaskServiceRepository>();
builder.Services.AddScoped<ICommentServiceRepository, CommentServiceRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BasicAuth", Version = "v1" });
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                      new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        new string[] {}
                }
            });
});

//SENTRY
builder.WebHost.UseSentry(options =>
{
    var configuration = builder.Configuration;
    options.Dsn = configuration.GetConnectionString("Dsn");

    // Debugging and tracing options
    options.Debug = true;
    options.TracesSampleRate = 1.0;
    options.ProfilesSampleRate = 1.0;

    // Profiling integration
    options.AddIntegration(new ProfilingIntegration(
        TimeSpan.FromMilliseconds(500)
    ));
});

//AUTH
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

//SQL
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options => {
    options.AllowAnyMethod();
    options.AllowAnyHeader();
    options.AllowAnyOrigin();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
