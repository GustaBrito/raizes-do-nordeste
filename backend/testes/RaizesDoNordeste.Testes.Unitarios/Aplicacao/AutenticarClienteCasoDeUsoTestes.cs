using FluentAssertions;
using Moq;
using RaizesDoNordeste.Aplicacao.CasosDeUso.AutenticarCliente;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Aplicacao;

public class AutenticarClienteCasoDeUsoTestes
{
    private readonly Mock<IRepositorioCliente> _repositorioClienteMock = new();
    private readonly Mock<IServicoSenha> _servicoSenhaMock = new();
    private readonly Mock<IServicoTokenJwt> _servicoTokenMock = new();
    private readonly AutenticarClienteCasoDeUso _casoDeUso;

    public AutenticarClienteCasoDeUsoTestes()
    {
        _casoDeUso = new AutenticarClienteCasoDeUso(
            _repositorioClienteMock.Object,
            _servicoSenhaMock.Object,
            _servicoTokenMock.Object);
    }

    [Fact]
    public async Task Executar_ComCredenciaisValidas_DeveRetornarToken()
    {
        var cliente = CriarClientePersistido();
        _repositorioClienteMock
            .Setup(r => r.ObterPorEmailAsync("ana@email.com"))
            .ReturnsAsync(cliente);
        _servicoSenhaMock
            .Setup(s => s.VerificarSenha("SenhaForte123", "hash-salvo"))
            .Returns(true);
        _servicoTokenMock
            .Setup(s => s.GerarToken(cliente))
            .Returns(new TokenGerado("token-gerado", DateTime.UtcNow.AddHours(1)));

        var saida = await _casoDeUso.ExecutarAsync(new LoginEntrada("ana@email.com", "SenhaForte123"));

        saida.Token.Should().Be("token-gerado");
        saida.ClienteId.Should().Be(cliente.Id);
        saida.Nome.Should().Be("Ana");
    }

    [Fact]
    public async Task Executar_ComEmailInexistente_DeveLancarExcecaoDeCredenciais()
    {
        _repositorioClienteMock
            .Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Cliente?)null);

        var acao = async () =>
            await _casoDeUso.ExecutarAsync(new LoginEntrada("inexistente@email.com", "qualquer"));

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*Credenciais inválidas*");
    }

    [Fact]
    public async Task Executar_ComSenhaIncorreta_DeveLancarExcecaoDeCredenciais()
    {
        var cliente = CriarClientePersistido();
        _repositorioClienteMock
            .Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(cliente);
        _servicoSenhaMock
            .Setup(s => s.VerificarSenha(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var acao = async () =>
            await _casoDeUso.ExecutarAsync(new LoginEntrada("ana@email.com", "errada"));

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*Credenciais inválidas*");
    }

    [Fact]
    public async Task Executar_ComEmailVazio_DeveLancarExcecaoDeCredenciais()
    {
        var acao = async () =>
            await _casoDeUso.ExecutarAsync(new LoginEntrada(string.Empty, "qualquer"));

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*Credenciais inválidas*");
    }

    [Fact]
    public async Task Executar_ComClienteSemSenhaHash_DeveLancarExcecaoDeCredenciais()
    {
        var cliente = new Cliente("Bruno", "bruno@email.com", "11999999999", true);
        _repositorioClienteMock
            .Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(cliente);

        var acao = async () =>
            await _casoDeUso.ExecutarAsync(new LoginEntrada("bruno@email.com", "qualquer"));

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*Credenciais inválidas*");
    }

    private static Cliente CriarClientePersistido()
    {
        var cliente = new Cliente("Ana", "ana@email.com", "11999999999", true);
        cliente.DefinirSenhaHash("hash-salvo");
        return cliente;
    }
}
