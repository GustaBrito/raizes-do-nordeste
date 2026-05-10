using FluentAssertions;
using Moq;
using RaizesDoNordeste.Aplicacao.CasosDeUso.ConsultarFidelizacao;
using RaizesDoNordeste.Aplicacao.CasosDeUso.ResgatarPontos;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Aplicacao;

public class ConsultarFidelizacaoCasoDeUsoTestes
{
    private readonly Mock<IRepositorioCliente> _repositorioMock = new();

    [Fact]
    public async Task ConsultarFidelizacao_ComClienteValido_DeveRetornarSaldo()
    {
        var cliente = new Cliente("Pedro", "pedro@email.com", "11999999999", true);
        cliente.AdicionarPontos(150);
        _repositorioMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

        var casoDeUso = new ConsultarFidelizacaoCasoDeUso(_repositorioMock.Object);
        var saida = await casoDeUso.ExecutarAsync(cliente.Id);

        saida.SaldoPontos.Should().Be(150);
        saida.NomeCliente.Should().Be("Pedro");
    }

    [Fact]
    public async Task ConsultarFidelizacao_ComClienteInexistente_DeveLancarExcecao()
    {
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

        var casoDeUso = new ConsultarFidelizacaoCasoDeUso(_repositorioMock.Object);
        var acao = async () => await casoDeUso.ExecutarAsync(Guid.NewGuid());

        await acao.Should().ThrowAsync<ClienteNaoEncontradoException>();
    }

    [Fact]
    public async Task ResgatarPontos_ComSaldoSuficiente_DeveDebitar()
    {
        var cliente = new Cliente("Clara", "clara@email.com", "11999999999", true);
        cliente.AdicionarPontos(500);
        _repositorioMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

        var casoDeUso = new ResgatarPontosCasoDeUso(_repositorioMock.Object);
        var saida = await casoDeUso.ExecutarAsync(new ResgatarPontosEntrada(cliente.Id, 200));

        saida.SaldoPontos.Should().Be(300);
        _repositorioMock.Verify(r => r.AtualizarAsync(cliente), Times.Once);
    }

    [Fact]
    public async Task ResgatarPontos_ComSaldoInsuficiente_DeveLancarExcecao()
    {
        var cliente = new Cliente("Diego", "diego@email.com", "11999999999", true);
        cliente.AdicionarPontos(100);
        _repositorioMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);

        var casoDeUso = new ResgatarPontosCasoDeUso(_repositorioMock.Object);
        var acao = async () =>
            await casoDeUso.ExecutarAsync(new ResgatarPontosEntrada(cliente.Id, 500));

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*Saldo*insuficiente*");
    }
}
