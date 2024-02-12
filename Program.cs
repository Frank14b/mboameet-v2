using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API.Interfaces;
using API.Services;
using API.Seeders;
using API.DTOs;
using API.Data;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.AppHub;
// using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddRazorPages();


builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MboaMeet Backend API", Version = "v1.0.0" });

        // Add JWT bearer authentication scheme
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });
        // c.OperationFilter<SwaggerAuthorizeOperationFilter>();
    });

string? connectionString = builder.Configuration["MONGODB_URI"];

if (connectionString == null)
{
    Console.WriteLine("You must set your 'MONGODB_URI' environment variable.");
    Environment.Exit(0);
}

var client = new MongoClient(connectionString);

var db = client.GetDatabase("mboameet");

builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseMongoDB(db.Client, db.DatabaseNamespace.DatabaseName);
});

builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, Options =>
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"] ?? "")),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

string[] _Auth_Type_User = { "Admin", "User" };
string[] _Auth_Type_Admin = { "Admin" };

builder.Services.AddAuthorization(Options =>
    {
        Options.AddPolicy("IsAdmin", policy => policy.RequireClaim("Type", _Auth_Type_Admin));
        Options.AddPolicy("IsUser", policy => policy.RequireClaim("Type", _Auth_Type_User));
    }
);

// add cors policy
string clientUrl = builder.Configuration["ClientUrl"] ?? "";

builder.Services.AddCors(Options =>
{
    Options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins(clientUrl) // Replace with your React app's URL
               .SetIsOriginAllowed((host) => true)
               .AllowAnyHeader()
               .WithMethods("GET", "POST")
               .AllowCredentials();
    });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1.0.0");
    });
}

app.UseCors("AllowReactApp");

//register middlewares
// app.UseMiddleware<ExceptionMiddleware>();
// app.UseMiddleware<RoleAccessMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
// register hub routes
app.MapHub<ChatHub>("/chathub");
app.Run();

// app.UseStaticFiles();
// app.UseRouting();
// app.MapRazorPages();
