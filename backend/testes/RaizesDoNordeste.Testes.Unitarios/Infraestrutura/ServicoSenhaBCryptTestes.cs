using FluentAssertions;
using RaizesDoNordeste.Infraestrutura.Servicos;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Infraestrutura;

public class ServicoSenhaBCryptTestes
{
    private readonly ServicoSenhaBCrypt _servico = new();

    [Fact]
    public void GerarHash_ComSenhaValida_DeveRetornarHashNaoVazio()
    {
        var hash = _servico.GerarHash("MinhaSenha123");

        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe("MinhaSenha123");
        hash.Should().StartWith("$2");
    }

    [Fact]
    public void VerificarSenha_ComSenhaCorreta_DeveRetornarVerdadeiro()
    {
        var hash = _servico.GerarHash("MinhaSenha123");

        var resultado = _servico.VerificarSenha("MinhaSenha123", hash);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void VerificarSenha_ComSenhaIncorreta_DeveRetornarFalso()
    {
        var hash = _servico.GerarHash("MinhaSenha123");

        var resultado = _servico.VerificarSenha("SenhaErrada", hash);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void VerificarSenha_ComHashInvalido_DeveRetornarFalso()
    {
        var resultado = _servico.VerificarSenha("qualquer", "hash-corrompido");

        resultado.Should().BeFalse();
    }

    [Fact]
    public void GerarHash_ComSenhaVazia_DeveLancarExcecao()
    {
        var acao = () => _servico.GerarHash(string.Empty);

        acao.Should().Throw<ArgumentException>();
    }
}
