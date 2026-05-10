using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Enumeracoes;
using Xunit;

namespace RaizesDoNordeste.Testes.Integracao.API;

[Trait("Categoria", "Regressao")]
public class PedidosApiTestes : IClassFixture<FabricaAplicacaoTeste>
{
    private readonly HttpClient _clienteHttp;

    private static readonly JsonSerializerOptions _opcoesJson = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public PedidosApiTestes(FabricaAplicacaoTeste fabrica)
    {
        _clienteHttp = fabrica.CreateClient();
    }

    // CT-PED-01 — Realizar pedido válido retorna 201 com dados do pedido
    [Fact]
    public async Task PostPedido_ComDadosValidos_DeveRetornar201ComPedidoConfirmado()
    {
        var email = $"ped-valido-{Guid.NewGuid()}@email.com";
        var clienteId = await CadastrarEObterIdAsync(email, "SenhaForte123");
        var token = await ObterTokenAsync(email, "SenhaForte123");

        _clienteHttp.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var entrada = CriarEntradaValida(clienteId);
        var resposta = await _clienteHttp.PostAsJsonAsync("/api/pedidos", entrada);

        resposta.StatusCode.Should().Be(HttpStatusCode.Created);
        var corpo = await resposta.Content.ReadFromJsonAsync<RealizarPedidoSaida>(_opcoesJson);
        corpo.Should().NotBeNull();
        corpo!.NumeroPedido.Should().StartWith("RDN-");
        corpo.PedidoId.Should().NotBeEmpty();
        corpo.ValorTotal.Should().Be(51.80m); // 2 × 25.90
    }

    // CT-PED-02 — Realizar pedido para cliente inexistente retorna 404
    [Fact]
    public async Task PostPedido_ComClienteInexistente_DeveRetornar404()
    {
        var email = $"ped-404-{Guid.NewGuid()}@email.com";
        await CadastrarEObterIdAsync(email, "SenhaForte123");
        var token = await ObterTokenAsync(email, "SenhaForte123");

        _clienteHttp.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var entrada = CriarEntradaValida(Guid.NewGuid()); // ID de cliente inexistente
        var resposta = await _clienteHttp.PostAsJsonAsync("/api/pedidos", entrada);

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var corpo = await resposta.Content.ReadAsStringAsync();
        corpo.Should().Contain("mensagem");
    }

    // CT-PED-03 — Pedido sem token retorna 401
    [Fact]
    public async Task PostPedido_SemToken_DeveRetornar401()
    {
        _clienteHttp.DefaultRequestHeaders.Authorization = null;
        var entrada = CriarEntradaValida(Guid.NewGuid());
        var resposta = await _clienteHttp.PostAsJsonAsync("/api/pedidos", entrada);

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CT-PED-04 — Obter status de pedido existente retorna 200
    [Fact]
    public async Task GetStatus_ComNumeroPedidoExistente_DeveRetornar200ComStatus()
    {
        var email = $"ped-status-{Guid.NewGuid()}@email.com";
        var clienteId = await CadastrarEObterIdAsync(email, "SenhaForte123");
        var token = await ObterTokenAsync(email, "SenhaForte123");

        _clienteHttp.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Cria o pedido primeiro
        var entrada = CriarEntradaValida(clienteId);
        var respostaPedido = await _clienteHttp.PostAsJsonAsync("/api/pedidos", entrada);
        respostaPedido.EnsureSuccessStatusCode();
        var pedidoCriado = await respostaPedido.Content.ReadFromJsonAsync<RealizarPedidoSaida>(_opcoesJson);

        // Consulta o status
        var resposta = await _clienteHttp.GetAsync($"/api/pedidos/{pedidoCriado!.NumeroPedido}/status");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var corpo = await resposta.Content.ReadAsStringAsync();
        corpo.Should().Contain(pedidoCriado.NumeroPedido);
    }

    // CT-PED-05 — Obter status de pedido inexistente retorna 404
    [Fact]
    public async Task GetStatus_ComNumeroPedidoInexistente_DeveRetornar404()
    {
        var email = $"ped-status-404-{Guid.NewGuid()}@email.com";
        await CadastrarEObterIdAsync(email, "SenhaForte123");
        var token = await ObterTokenAsync(email, "SenhaForte123");

        _clienteHttp.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var resposta = await _clienteHttp.GetAsync("/api/pedidos/RDN-INEXISTENTE/status");

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var corpo = await resposta.Content.ReadAsStringAsync();
        corpo.Should().Contain("não encontrado");
    }

    // CT-PED-06 — Listar pedidos do cliente autenticado retorna 200
    [Fact]
    public async Task GetPedidosCliente_ComClienteComPedidos_DeveRetornar200ComLista()
    {
        var email = $"ped-listar-{Guid.NewGuid()}@email.com";
        var clienteId = await CadastrarEObterIdAsync(email, "SenhaForte123");
        var token = await ObterTokenAsync(email, "SenhaForte123");

        _clienteHttp.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Cria um pedido para o cliente
        var resPedido = await _clienteHttp.PostAsJsonAsync("/api/pedidos", CriarEntradaValida(clienteId));
        resPedido.EnsureSuccessStatusCode();

        // Lista os pedidos do cliente
        var resposta = await _clienteHttp.GetAsync($"/api/clientes/{clienteId}/pedidos");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var corpo = await resposta.Content.ReadAsStringAsync();
        corpo.Should().Contain("numeroPedido");
    }

    // CT-PED-07 — Listar pedidos sem token retorna 401
    [Fact]
    public async Task GetPedidosCliente_SemToken_DeveRetornar401()
    {
        _clienteHttp.DefaultRequestHeaders.Authorization = null;
        var resposta = await _clienteHttp.GetAsync($"/api/clientes/{Guid.NewGuid()}/pedidos");
        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private RealizarPedidoEntrada CriarEntradaValida(Guid clienteId)
        => new(
            clienteId,
            Guid.NewGuid(),
            CanalAtendimento.Aplicativo,
            "cartao_credito",
            new[] { new ItemPedidoEntrada(Guid.NewGuid(), "X-Nordeste", 2, 25.90m) }
        );

    private async Task<Guid> CadastrarEObterIdAsync(string email, string senha)
    {
        var entrada = new CadastrarClienteEntrada(
            "Usuario Pedido", email, "11999999999", true, senha);
        var resposta = await _clienteHttp.PostAsJsonAsync("/api/clientes", entrada);
        resposta.EnsureSuccessStatusCode();
        var corpo = await resposta.Content.ReadFromJsonAsync<IdResposta>();
        return corpo!.Id;
    }

    private async Task<string> ObterTokenAsync(string email, string senha)
    {
        var resposta = await _clienteHttp.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginEntrada(email, senha));
        resposta.EnsureSuccessStatusCode();
        var token = await resposta.Content.ReadFromJsonAsync<TokenSaida>();
        return token!.Token;
    }

    private record IdResposta(Guid Id);
}
