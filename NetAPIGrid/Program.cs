using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NetAPIGrid.Service;
using System.Text;
using Microsoft.OpenApi.Models;
using Hangfire;
using NetAPIGrid.Context;
using Microsoft.EntityFrameworkCore;
using NetAPIGrid.Models;
using Microsoft.SharePoint.Portal.Topology;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GRID_LVL_SEVEN_PROD")));

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter your JWT Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme,
        }
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {jwtSecurityScheme,Array.Empty<string>() }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        //ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

});



builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AES>();
builder.Services.AddScoped<MyJobService>();

//builder.Services.AddDbContext<GRID_LVL_SEVEN_DBContext>(options => options.UseSqlServer(
//        builder.Configuration.GetConnectionString("GRID_LVL_SEVEN_PROD")
//    ));

//builder.Services.AddHangfire(config =>
//{
//    config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
//    {
//        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
//        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
//        QueuePollInterval = TimeSpan.Zero,
//        UseRecommendedIsolationLevel = true,
//        DisableGlobalLocks = true
//    });
//});
//builder.Services.AddHangfireServer();
builder.Services.AddHttpClient<MyJobService>();

//builder.Services.AddHangfire(config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
//.UseSimpleAssemblyNameTypeSerializer()
//.UseRecommendedSerializerSettings()
//.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
//{
//    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
//    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
//    QueuePollInterval = TimeSpan.Zero,
//    UseRecommendedIsolationLevel = true,
//    DisableGlobalLocks = true
//}));

builder.Services.AddHangfire(config =>
config.UseSqlServerStorage(builder.Configuration.GetConnectionString("GRID_LVL_SEVEN_STAGE")));
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseOpenApi();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHangfireDashboard();
//RecurringJob.AddOrUpdate<MyJobService>("my-recurring-job", service => service.RunInsertLogsEndPoint(), "0 * * * *");
//RecurringJob.AddOrUpdate<MyJobService>("my-recurring-job", service => service.RunInsertLogsEndPoint(), "*/30 * * * *");
//RecurringJob.AddOrUpdate<MyJobService>("my-recurring-job", service => service.RunInsertLogsEndPoint(), Cron.MinuteInterval(1));
//RecurringJob.AddOrUpdate<MyJobService>("my-recurring-job2", service => service.DeleteFiles(), Cron.Weekly(DayOfWeek.Sunday,1));
BackgroundJob.Enqueue<MyJobService>( service => service.RunInsertLogsEndPoint());
app.Run();
