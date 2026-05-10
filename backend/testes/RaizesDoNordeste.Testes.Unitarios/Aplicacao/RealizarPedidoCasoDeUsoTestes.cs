using FluentAssertions;
using Moq;
using RaizesDoNordeste.Aplicacao.CasosDeUso.RealizarPedido;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Enumeracoes;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Aplicacao;

public class RealizarPedidoCasoDeUsoTestes
{
    private readonly Mock<IRepositorioCliente> _repositorioClienteMock = new();
    private readonly Mock<IRepositorioPedido> _repositorioPedidoMock = new();
    private readonly Mock<IServicoPagamento> _servicoPagamentoMock = new();
    private readonly Mock<IServicoFidelizacao> _servicoFidelizacaoMock = new();
    private readonly RealizarPedidoCasoDeUso _casoDeUso;

    public RealizarPedidoCasoDeUsoTestes()
    {
        _casoDeUso = new RealizarPedidoCasoDeUso(
            _repositorioClienteMock.Object,
            _repositorioPedidoMock.Object,
            _servicoPagamentoMock.Object,
            _servicoFidelizacaoMock.Object
        );
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveRetornarPedidoConfirmado()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("Maria", "maria@email.com", "11999999999", true);
        var entrada = CriarEntradaValida(clienteId);

        _repositorioClienteMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(cliente);
        _servicoPagamentoMock.Setup(s => s.ProcessarAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new ResultadoPagamento(true, "TRX-001", null));
        _servicoFidelizacaoMock.Setup(s => s.CalcularPontos(It.IsAny<decimal>())).Returns(50);

        var resultado = await _casoDeUso.ExecutarAsync(entrada);

        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPedido.EmPreparacao);
        resultado.PontosAdicionados.Should().Be(50);
        resultado.CodigoTransacaoPagamento.Should().Be("TRX-001");
    }

    [Fact]
    public async Task ExecutarAsync_ComClienteInexistente_DeveLancarClienteNaoEncontradoException()
    {
        var clienteId = Guid.NewGuid();
        var entrada = CriarEntradaValida(clienteId);

        _repositorioClienteMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync((Cliente?)null);

        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<ClienteNaoEncontradoException>();
    }

    [Fact]
    public async Task ExecutarAsync_ComPagamentoRecusado_DeveLancarInvalidOperationException()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("João", "joao@email.com", "11999999999", true);
        var entrada = CriarEntradaValida(clienteId);

        _repositorioClienteMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(cliente);
        _servicoPagamentoMock.Setup(s => s.ProcessarAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new ResultadoPagamento(false, null, "Cartão recusado pela operadora."));

        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cartão recusado*");
    }

    private static RealizarPedidoEntrada CriarEntradaValida(Guid clienteId)
        => new(
            clienteId,
            Guid.NewGuid(),
            CanalAtendimento.Aplicativo,
            "cartao_credito",
            new[] { new ItemPedidoEntrada(Guid.NewGuid(), "X-Nordeste", 2, 25.90m) }
        );
}
