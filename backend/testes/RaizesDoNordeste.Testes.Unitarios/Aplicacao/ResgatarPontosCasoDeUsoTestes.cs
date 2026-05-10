using FluentAssertions;
using Moq;
using RaizesDoNordeste.Aplicacao.CasosDeUso.ResgatarPontos;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Aplicacao;

public class ResgatarPontosCasoDeUsoTestes
{
    private readonly Mock<IRepositorioCliente> _repositorioClienteMock = new();
    private readonly ResgatarPontosCasoDeUso _casoDeUso;

    public ResgatarPontosCasoDeUsoTestes()
    {
        _casoDeUso = new ResgatarPontosCasoDeUso(_repositorioClienteMock.Object);
    }

    // Caminho feliz: resgate com saldo suficiente
    [Fact]
    public async Task Executar_ComSaldoSuficiente_DeveRetornarSaldoAtualizado()
    {
        var cliente = new Cliente("Ana", "ana@email.com", "11999999999", true);
        cliente.AdicionarPontos(200);
        _repositorioClienteMock
            .Setup(r => r.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        var entrada = new ResgatarPontosEntrada(cliente.Id, 150);
        var saida = await _casoDeUso.ExecutarAsync(entrada);

        saida.SaldoPontos.Should().Be(50); // 200 - 150 = 50
        saida.NomeCliente.Should().Be("Ana");
        _repositorioClienteMock.Verify(r => r.AtualizarAsync(cliente), Times.Once);
    }

    // Caso de borda: saldo insuficiente deve ser rejeitado no domínio
    [Fact]
    public async Task Executar_ComSaldoInsuficiente_DeveLancarDominioContinuarException()
    {
        var cliente = new Cliente("Bruno", "bruno@email.com", "11988887777", true);
        cliente.AdicionarPontos(50);
        _repositorioClienteMock
            .Setup(r => r.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        var entrada = new ResgatarPontosEntrada(cliente.Id, 100); // Tenta resgatar mais que o saldo
        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<DominioContinuarException>()
            .WithMessage("*Saldo*insuficiente*");
    }

    // Segurança: cliente inexistente não pode resgatar pontos
    [Fact]
    public async Task Executar_ComClienteInexistente_DeveLancarClienteNaoEncontradoException()
    {
        var idInexistente = Guid.NewGuid();
        _repositorioClienteMock
            .Setup(r => r.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Cliente?)null);

        var entrada = new ResgatarPontosEntrada(idInexistente, 10);
        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<ClienteNaoEncontradoException>();
    }
}
