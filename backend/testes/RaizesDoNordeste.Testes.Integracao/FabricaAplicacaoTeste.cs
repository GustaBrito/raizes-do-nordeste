using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaizesDoNordeste.Infraestrutura.Persistencia;

namespace RaizesDoNordeste.Testes.Integracao;

public class FabricaAplicacaoTeste : WebApplicationFactory<Program>
{
    // Nome do banco gerado UMA ÚNICA vez por instância da fábrica.
    // Se fosse gerado dentro do lambda de AddDbContext, cada escopo
    // receberia um banco diferente e quebraria o compartilhamento de
    // dados entre chamadas HTTP da mesma execução de teste.
    private readonly string _nomeBancoDados = $"TesteIntegracao-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(servicos =>
        {
            var descritor = servicos.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ContextoBancoDados>));

            if (descritor != null)
                servicos.Remove(descritor);

            servicos.AddDbContext<ContextoBancoDados>(opcoes =>
                opcoes.UseInMemoryDatabase(_nomeBancoDados));
        });
    }
}
