using FluentAssertions;
using Moq;
using RaizesDoNordeste.Aplicacao.CasosDeUso.CadastrarCliente;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Aplicacao;

public class CadastrarClienteCasoDeUsoTestes
{
    private readonly Mock<IRepositorioCliente> _repositorioClienteMock = new();
    private readonly Mock<IServicoSenha> _servicoSenhaMock = new();
    private readonly CadastrarClienteCasoDeUso _casoDeUso;

    public CadastrarClienteCasoDeUsoTestes()
    {
        _casoDeUso = new CadastrarClienteCasoDeUso(
            _repositorioClienteMock.Object,
            _servicoSenhaMock.Object);
    }

    // Caminho feliz: cadastro com dados válidos e senha
    [Fact]
    public async Task Executar_ComDadosValidos_DeveRetornarIdDoClienteCriado()
    {
        _repositorioClienteMock
            .Setup(r => r.ExisteEmailAsync("novo@email.com"))
            .ReturnsAsync(false);
        _servicoSenhaMock
            .Setup(s => s.GerarHash("Senha@123"))
            .Returns("hash-da-senha");

        var entrada = new CadastrarClienteEntrada(
            "Maria Silva", "novo@email.com", "11988881234", true, "Senha@123");

        var id = await _casoDeUso.ExecutarAsync(entrada);

        id.Should().NotBeEmpty();
        _repositorioClienteMock.Verify(r => r.AdicionarAsync(It.IsAny<RaizesDoNordeste.Dominio.Entidades.Cliente>()), Times.Once);
    }

    // Segurança: e-mail duplicado é rejeitado antes de criar o cliente
    [Fact]
    public async Task Executar_ComEmailJaCadastrado_DeveLancarDominioContinuarException()
    {
        _repositorioClienteMock
            .Setup(r => r.ExisteEmailAsync("duplicado@email.com"))
            .ReturnsAsync(true);

        var entrada = new CadastrarClienteEntrada(
            "Carlos", "duplicado@email.com", "11911112222", true);

        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*já está cadastrado*");
    }

    // LGPD (Art. 7, inciso I): cadastro sem consentimento deve ser bloqueado no domínio
    [Fact]
    public async Task Executar_SemConsentimentoLgpd_DeveLancarDominioContinuarException()
    {
        _repositorioClienteMock
            .Setup(r => r.ExisteEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var entrada = new CadastrarClienteEntrada(
            "Joao", "joao@email.com", "11933334444", false);

        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*consentimento*");
    }

    // Validação: senha curta demais deve ser rejeitada antes do hash
    [Fact]
    public async Task Executar_ComSenhaCurta_DeveLancarDominioContinuarException()
    {
        _repositorioClienteMock
            .Setup(r => r.ExisteEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var entrada = new CadastrarClienteEntrada(
            "Pedro", "pedro@email.com", "11955556666", true, "abc");

        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*8 caracteres*");
    }
}
