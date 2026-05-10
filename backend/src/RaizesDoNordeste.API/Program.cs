using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RaizesDoNordeste.Aplicacao.CasosDeUso.AutenticarCliente;
using RaizesDoNordeste.Aplicacao.CasosDeUso.CadastrarCliente;
using RaizesDoNordeste.Aplicacao.CasosDeUso.ConsultarFidelizacao;
using RaizesDoNordeste.Aplicacao.CasosDeUso.RealizarPedido;
using RaizesDoNordeste.Aplicacao.CasosDeUso.ResgatarPontos;
using RaizesDoNordeste.Dominio.Interfaces;
using RaizesDoNordeste.Infraestrutura.Persistencia;
using RaizesDoNordeste.Infraestrutura.Repositorios;
using RaizesDoNordeste.Infraestrutura.Servicos;

var construtor = WebApplication.CreateBuilder(args);

// Railway injeta PORT — mapear para ASP.NET Core
var porta = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(porta))
    construtor.WebHost.UseUrls($"http://+:{porta}");

// ============================================================
// Configuração de Autenticação JWT
// ============================================================
var configuracaoJwt = new ConfiguracaoTokenJwt
{
    ChaveSecreta = construtor.Configuration["Jwt:ChaveSecreta"]
        ?? "raizes-do-nordeste-chave-secreta-academica-uninter-2026-min-32-bytes",
    Emissor = construtor.Configuration["Jwt:Emissor"] ?? "RaizesDoNordeste",
    Audiencia = construtor.Configuration["Jwt:Audiencia"] ?? "RaizesDoNordesteClientes",
    MinutosValidade = int.Parse(construtor.Configuration["Jwt:MinutosValidade"] ?? "60")
};
construtor.Services.AddSingleton(configuracaoJwt);

construtor.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.RequireHttpsMetadata = false;
        opcoes.SaveToken = true;
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuracaoJwt.Emissor,
            ValidAudience = configuracaoJwt.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuracaoJwt.ChaveSecreta))
        };
    });

construtor.Services.AddAuthorization();

// ============================================================
// Controllers + Swagger (com suporte a Bearer)
// ============================================================
construtor.Services.AddControllers()
    .AddJsonOptions(opcoes =>
        opcoes.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));
construtor.Services.AddEndpointsApiExplorer();
construtor.Services.AddSwaggerGen(opcoes =>
{
    opcoes.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Raízes do Nordeste API",
        Version = "v1",
        Description = "API da plataforma multicanal Raízes do Nordeste — projeto UNINTER 2026."
    });

    opcoes.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe o token JWT no formato: Bearer {seu-token}"
    });

    opcoes.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ============================================================
// Banco de dados (EF Core InMemory para fins acadêmicos)
// ============================================================
construtor.Services.AddDbContext<ContextoBancoDados>(opcoes =>
    opcoes.UseInMemoryDatabase("RaizesDoNordeste"));

// ============================================================
// Injeção de dependência: repositórios, serviços e casos de uso
// ============================================================
construtor.Services.AddScoped<IRepositorioCliente, RepositorioCliente>();
construtor.Services.AddScoped<IRepositorioPedido, RepositorioPedido>();
construtor.Services.AddScoped<IServicoPagamento, ServicoPagamentoMock>();
construtor.Services.AddScoped<IServicoFidelizacao, ServicoFidelizacao>();
construtor.Services.AddSingleton<IServicoSenha, ServicoSenhaBCrypt>();
construtor.Services.AddSingleton<IServicoTokenJwt, ServicoTokenJwt>();

construtor.Services.AddScoped<ICadastrarClienteCasoDeUso, CadastrarClienteCasoDeUso>();
construtor.Services.AddScoped<IAutenticarClienteCasoDeUso, AutenticarClienteCasoDeUso>();
construtor.Services.AddScoped<IRealizarPedidoCasoDeUso, RealizarPedidoCasoDeUso>();
construtor.Services.AddScoped<IConsultarFidelizacaoCasoDeUso, ConsultarFidelizacaoCasoDeUso>();
construtor.Services.AddScoped<IResgatarPontosCasoDeUso, ResgatarPontosCasoDeUso>();

// ============================================================
// CORS — Dev: localhost | Produção: aceita qualquer origem (Vercel)
// ============================================================
construtor.Services.AddCors(opcoes =>
    opcoes.AddPolicy("Frontend", politica =>
    {
        politica.AllowAnyHeader().AllowAnyMethod();
        if (construtor.Environment.IsProduction())
            politica.SetIsOriginAllowed(_ => true);
        else
            politica.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175");
    }));

var app = construtor.Build();

// ============================================================
// Cabeçalhos de segurança (mitigação OWASP A05 — ver relatório ZAP)
// ============================================================
app.Use(async (contexto, proximo) =>
{
    contexto.Response.Headers["X-Content-Type-Options"] = "nosniff";
    contexto.Response.Headers["X-Frame-Options"] = "DENY";
    contexto.Response.Headers["Referrer-Policy"] = "no-referrer";
    await proximo();
});

app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
