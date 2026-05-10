using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RaizesDoNordeste.Aplicacao.DTOs;
using Xunit;

namespace RaizesDoNordeste.Testes.Integracao.API;

[Trait("Categoria", "Regressao")]
public class ClientesApiTestes : IClassFixture<FabricaAplicacaoTeste>
{
    private readonly HttpClient _clienteHttp;

    public ClientesApiTestes(FabricaAplicacaoTeste fabrica)
    {
        _clienteHttp = fabrica.CreateClient();
    }

    [Fact]
    public async Task PostCliente_ComDadosValidos_DeveRetornar201()
    {
        var entrada = new CadastrarClienteEntrada(
            "Ana Souza", "ana.souza@email.com", "11988887777", true);

        var resposta = await _clienteHttp.PostAsJsonAsync("/api/clientes", entrada);

        resposta.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostCliente_SemConsentimentoLgpd_DeveRetornar400()
    {
        var entrada = new CadastrarClienteEntrada(
            "Carlos Melo", "carlos@email.com", "11977776666", false);

        var resposta = await _clienteHttp.PostAsJsonAsync("/api/clientes", entrada);

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var corpo = await resposta.Content.ReadAsStringAsync();
        corpo.Should().Contain("consentimento");
    }

    [Fact]
    public async Task PostCliente_ComEmailDuplicado_DeveRetornar400()
    {
        var entrada = new CadastrarClienteEntrada(
            "Lucas Lima", "lucas.duplicado@email.com", "11966665555", true);

        await _clienteHttp.PostAsJsonAsync("/api/clientes", entrada);
        var respostaSegunda = await _clienteHttp.PostAsJsonAsync("/api/clientes", entrada);

        respostaSegunda.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var corpo = await respostaSegunda.Content.ReadAsStringAsync();
        corpo.Should().Contain("já está cadastrado");
    }

    [Fact]
    public async Task PostCliente_ComEmailInvalido_DeveRetornar400()
    {
        var entrada = new CadastrarClienteEntrada(
            "Teste Email", "email-invalido-sem-arroba", "11955554444", true);

        var resposta = await _clienteHttp.PostAsJsonAsync("/api/clientes", entrada);

        resposta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
