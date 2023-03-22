using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapiV2.Helpers;
using System.Text.Json.Serialization;
using webapiV2.Authorization;
using webapiV2.Services.Accounts;
using webapiV2.Services.Emails;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers()
    .AddJsonOptions(x => {
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// database connection kısmı
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<DataContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ayar nesneleri kısmı
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAuthentication(auth => {
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(br => {
    br.RequireHttpsMetadata = false;
    br.SaveToken = true;
    br.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
                builder.Configuration.GetSection("Secret").ToString()
            )
        ),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// configure HTTP request pipeline
{
    // generated swagger json and swagger ui middleware
    //if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET Sign-up and Verification API"));
    //}

    // Configure the HTTP request pipeline.
    // global cors policy
    app.UseCors(x => x
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        );

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();
    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.UseHttpsRedirection();
    //app.UseCookiePolicy();
    app.UseAuthorization();

    app.MapControllers();
}
app.Run();