using FluentAssertions;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Dominio;

public class ClienteTestes
{
    [Fact]
    public void NovoCliente_ComDadosValidos_DeveCriarComSucesso()
    {
        var cliente = new Cliente("Maria Silva", "maria@email.com", "11999999999", true);

        cliente.Nome.Should().Be("Maria Silva");
        cliente.PontosFidelizacao.Should().Be(0);
        cliente.ConsentimentoLgpd.Should().BeTrue();
        cliente.DataConsentimentoLgpd.Should().NotBeNull();
    }

    [Fact]
    public void NovoCliente_SemConsentimentoLgpd_DeveLancarExcecao()
    {
        var acao = () => new Cliente("João", "joao@email.com", "11999999999", false);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*consentimento*");
    }

    [Fact]
    public void NovoCliente_ComEmailInvalido_DeveLancarExcecao()
    {
        var acao = () => new Cliente("João", "email-invalido", "11999999999", true);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*e-mail*inválido*");
    }

    [Fact]
    public void AdicionarPontos_ComValorPositivo_DeveAtualizarSaldo()
    {
        var cliente = new Cliente("Ana", "ana@email.com", "11999999999", true);

        cliente.AdicionarPontos(150);

        cliente.PontosFidelizacao.Should().Be(150);
    }

    [Fact]
    public void ResgatarPontos_ComSaldoInsuficiente_DeveLancarExcecao()
    {
        var cliente = new Cliente("Pedro", "pedro@email.com", "11999999999", true);
        cliente.AdicionarPontos(100);

        var acao = () => cliente.ResgatarPontos(200);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*Saldo*insuficiente*");
    }

    [Fact]
    public void ResgatarPontos_ComSaldoSuficiente_DeveDebitarPontos()
    {
        var cliente = new Cliente("Paula", "paula@email.com", "11999999999", true);
        cliente.AdicionarPontos(500);

        cliente.ResgatarPontos(200);

        cliente.PontosFidelizacao.Should().Be(300);
    }
}
