using FluentAssertions;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Dominio;

public class ClientePontosTestes
{
    private static Cliente CriarClienteValido(string nome = "Joana", string email = "joana@email.com")
        => new(nome, email, "11999999999", true);

    [Fact]
    public void AdicionarPontos_ComValorZero_DeveLancarDominioContinuarException()
    {
        var cliente = CriarClienteValido();

        var acao = () => cliente.AdicionarPontos(0);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*quantidade de pontos*positiva*");
    }

    [Fact]
    public void AdicionarPontos_ComValorNegativo_DeveLancarDominioContinuarException()
    {
        var cliente = CriarClienteValido();

        var acao = () => cliente.AdicionarPontos(-10);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*quantidade de pontos*positiva*");
    }

    [Fact]
    public void ResgatarPontos_ComValorExatoAoSaldo_DeveZerarSaldo()
    {
        var cliente = CriarClienteValido();
        cliente.AdicionarPontos(300);

        cliente.ResgatarPontos(300);

        cliente.PontosFidelizacao.Should().Be(0);
    }

    [Fact]
    public void ResgatarPontos_ComValorZero_DeveLancarDominioContinuarException()
    {
        var cliente = CriarClienteValido();
        cliente.AdicionarPontos(100);

        var acao = () => cliente.ResgatarPontos(0);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*quantidade de pontos*positiva*");
    }

    [Theory]
    [InlineData(100, 50, 150)]
    [InlineData(0, 200, 200)]
    [InlineData(500, 1, 501)]
    public void AdicionarPontos_EmMultiplasOperacoes_DeveAcumularCorretamente(
        int pontosIniciais, int pontosAdicionais, int totalEsperado)
    {
        var cliente = CriarClienteValido();
        if (pontosIniciais > 0)
            cliente.AdicionarPontos(pontosIniciais);

        cliente.AdicionarPontos(pontosAdicionais);

        cliente.PontosFidelizacao.Should().Be(totalEsperado);
    }

    [Fact]
    public void NovoCliente_ComNomeVazio_DeveLancarDominioContinuarException()
    {
        var acao = () => new Cliente("", "valido@email.com", null, true);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*nome*vazio*");
    }

    [Fact]
    public void DefinirSenhaHash_ComHashValido_DeveAtribuirSenhaHash()
    {
        var cliente = CriarClienteValido();

        cliente.DefinirSenhaHash("$2a$11$hashgerado");

        cliente.SenhaHash.Should().Be("$2a$11$hashgerado");
    }

    [Fact]
    public void DefinirSenhaHash_ComHashVazio_DeveLancarDominioContinuarException()
    {
        var cliente = CriarClienteValido();

        var acao = () => cliente.DefinirSenhaHash("   ");

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*hash de senha*vazio*");
    }
}
