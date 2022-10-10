using Microsoft.AspNetCore.Authentication.JwtBearer; //1
using Microsoft.IdentityModel.Tokens; //2
using Microsoft.OpenApi.Models;
using System.Text; //3

var builder = WebApplication.CreateBuilder(args);
var reglaCORS = "reglaCORS";

builder.Services.AddCors(option =>
option.AddPolicy(name: reglaCORS,
builder =>
{
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); //permite cualquier origen, header o metodo de ejecucion
}
)
);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    In = ParameterLocation.Header,
    Description = "Insertar JWT Token",
    Name = "Autorizacion",
    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "bearer"
}));

builder.Services.AddSwaggerGen(w =>
w.AddSecurityRequirement(
    new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
    }   }
    ));



//obtener secret key
builder.Configuration.AddJsonFile("appsettings.json");
var secretkey = builder.Configuration.GetSection("settings").GetSection("secretkey").ToString();
var keyBytes = Encoding.UTF8.GetBytes(secretkey); //convertir secret key en bytes
//--------------------------------------------------------------------------------------------------------

//implementar JWT
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = false;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, //validacion por usuario
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    
//}

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors(reglaCORS);

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
