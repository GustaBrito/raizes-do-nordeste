using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RaizesDoNordeste.Aplicacao.DTOs;
using Xunit;

namespace RaizesDoNordeste.Testes.Integracao.API;

[Trait("Categoria", "Regressao")]
public class AutenticacaoApiTestes : IClassFixture<FabricaAplicacaoTeste>
{
    private readonly HttpClient _clienteHttp;

    public AutenticacaoApiTestes(FabricaAplicacaoTeste fabrica)
    {
        _clienteHttp = fabrica.CreateClient();
    }

    // CT-SIS-01 (positivo) — Login com credenciais válidas
    [Fact]
    public async Task PostLogin_ComCredenciaisValidas_DeveRetornar200ComToken()
    {
        var email = $"auth-valido-{Guid.NewGuid()}@email.com";
        await CadastrarClienteAsync(email, "SenhaForte123");

        var resposta = await _clienteHttp.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginEntrada(email, "SenhaForte123"));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await resposta.Content.ReadFromJsonAsync<TokenSaida>();
        token.Should().NotBeNull();
        token!.Token.Should().NotBeNullOrEmpty();
        token.ClienteId.Should().NotBeEmpty();
    }

    // CT-SIS-02 (negativo) — Login com senha inválida
    [Fact]
    public async Task PostLogin_ComSenhaIncorreta_DeveRetornar401()
    {
        var email = $"auth-senha-errada-{Guid.NewGuid()}@email.com";
        await CadastrarClienteAsync(email, "SenhaCorreta123");

        var resposta = await _clienteHttp.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginEntrada(email, "SenhaIncorreta999"));

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var corpo = await resposta.Content.ReadAsStringAsync();
        corpo.Should().Contain("Credenciais inválidas");
    }

    [Fact]
    public async Task PostLogin_ComEmailInexistente_DeveRetornar401()
    {
        var resposta = await _clienteHttp.PostAsJsonAsync(
            "/api/autenticacao/login",
            new LoginEntrada("nao-existe@email.com", "qualquer"));

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CT-SIS-08 (negativo) — Acesso sem autenticação a área restrita
    [Fact]
    public async Task GetFidelizacao_SemToken_DeveRetornar401()
    {
        var resposta = await _clienteHttp.GetAsync($"/api/fidelizacao/{Guid.NewGuid()}");

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task CadastrarClienteAsync(string email, string senha)
    {
        var entrada = new CadastrarClienteEntrada(
            "Usuario Teste", email, "11999999999", true, senha);
        var resposta = await _clienteHttp.PostAsJsonAsync("/api/clientes", entrada);
        resposta.EnsureSuccessStatusCode();
    }
}
