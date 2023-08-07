using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Interfaces;
using WebApplication1.Middlewares;
using WebApplication1.Repository;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR().AddStackExchangeRedis("redis://localhost:6379/0");

//builder.Services.AddSingleton(sp =>
//{
//    var redisConnection = ConnectionMultiplexer.Connect("redis://localhost:6379/0");
//    return redisConnection;
//});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

builder.Services.AddScoped<IUserConnectionService, UserConnectionService>();


builder.Services.AddScoped<LoggingMiddleware>();

builder.Services.AddDbContext<ChatContext>(opt =>
 opt.UseSqlServer(builder.Configuration.GetConnectionString("ChatContext")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ChatContext>()
            .AddDefaultTokenProviders();

//builder.Services.AddCors(options => { options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
builder =>
{
    builder.AllowAnyMethod().AllowAnyHeader()
    .WithOrigins("http://localhost:4200")
    .AllowCredentials();
}));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ChatContext>();
    context.Database.EnsureCreated();
    // DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<LoggingMiddleware>();
app.UseCors("CorsPolicy");
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();


