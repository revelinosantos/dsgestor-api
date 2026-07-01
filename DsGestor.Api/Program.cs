using System.Text;
using System.Text.Json.Serialization;
using DsGestor.Application.Services;
using DsGestor.Domain.Repositories;
using DsGestor.Infrastructure.Data;
using DsGestor.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext Oracle
var oracleConnectionString = builder.Configuration.GetConnectionString("OracleDsGestor");

builder.Services.AddDbContext<DsGestorDbContext>(options =>
{
    options.UseOracle(oracleConnectionString);
});

// Repositórios
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IVendedorRepository, VendedorRepository>();
builder.Services.AddScoped<ISupervisorRepository, SupervisorRepository>();
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
builder.Services.AddScoped<ILancamentoDetRepository, LancamentoDetRepository>();
builder.Services.AddScoped<IPromocaoRepository, PromocaoRepository>();
builder.Services.AddScoped<IPromocaoItemRepository, PromocaoItemRepository>();
builder.Services.AddScoped<IPromocaoClienteRepository, PromocaoClienteRepository>();
builder.Services.AddScoped<IPromocaoOrigemPedidoRepository, PromocaoOrigemPedidoRepository>();
builder.Services.AddScoped<ICoteFacilPedidoRepository, CoteFacilPedidoRepository>();
builder.Services.AddScoped<ICoteFacilConsultaRepository, CoteFacilConsultaRepository>();
builder.Services.AddScoped<ICoteFacilProdutoRepository, CoteFacilProdutoRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();


// JWT Auth
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key não configurada");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// Autorização com base em perfil (Role)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("GerenteOuAcima", policy => policy.RequireRole("Admin", "Gerente"));
    options.AddPolicy("CoteFacil", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("integration", "cotefacil");
    });
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DsGestor API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});


builder.Services.AddCors(options =>
{   
    options.AddPolicy("DevCors", policy =>
    {
 
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    
   
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevCors");

//app.UseHttpsRedirection();


app.UseAuthentication();   
app.UseAuthorization();

app.MapControllers();

app.Run();
