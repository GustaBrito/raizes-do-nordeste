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

public class RealizarPedidoCasoDeUsoBordaTestes
{
    private readonly Mock<IRepositorioCliente> _repositorioClienteMock = new();
    private readonly Mock<IRepositorioPedido> _repositorioPedidoMock = new();
    private readonly Mock<IServicoPagamento> _servicoPagamentoMock = new();
    private readonly Mock<IServicoFidelizacao> _servicoFidelizacaoMock = new();
    private readonly RealizarPedidoCasoDeUso _casoDeUso;

    public RealizarPedidoCasoDeUsoBordaTestes()
    {
        _casoDeUso = new RealizarPedidoCasoDeUso(
            _repositorioClienteMock.Object,
            _repositorioPedidoMock.Object,
            _servicoPagamentoMock.Object,
            _servicoFidelizacaoMock.Object
        );
    }

    [Fact]
    public async Task ExecutarAsync_ComFidelizacaoRetornandoUmPonto_DeveRetornarPedidoComUmPonto()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("Fernanda", "fernanda@email.com", "11999999999", true);
        var entrada = CriarEntradaComUmItem(clienteId, quantidade: 1, preco: 30.00m);

        _repositorioClienteMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(cliente);
        _servicoPagamentoMock.Setup(s => s.ProcessarAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new ResultadoPagamento(true, "TRX-MIN", null));
        _servicoFidelizacaoMock.Setup(s => s.CalcularPontos(It.IsAny<decimal>())).Returns(1);

        var resultado = await _casoDeUso.ExecutarAsync(entrada);

        resultado.PontosAdicionados.Should().Be(1);
        resultado.Status.Should().Be(StatusPedido.EmPreparacao);
    }

    [Fact]
    public async Task ExecutarAsync_ComPedidoSemItens_DeveLancarPedidoInvalidoException()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("Ricardo", "ricardo@email.com", "11999999999", true);
        var entrada = new RealizarPedidoEntrada(
            clienteId,
            Guid.NewGuid(),
            CanalAtendimento.Balcao,
            "dinheiro",
            Array.Empty<ItemPedidoEntrada>()
        );

        _repositorioClienteMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(cliente);
        _servicoPagamentoMock.Setup(s => s.ProcessarAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new ResultadoPagamento(true, "TRX-VAZIO", null));

        var acao = async () => await _casoDeUso.ExecutarAsync(entrada);

        await acao.Should().ThrowAsync<PedidoInvalidoException>()
            .WithMessage("*ao menos um item*");
    }

    [Fact]
    public async Task ExecutarAsync_ComPagamentoBemSucedido_DevePersistirPedidoEAtualizarCliente()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("Luiza", "luiza@email.com", "11999999999", true);
        var entrada = CriarEntradaComUmItem(clienteId, quantidade: 2, preco: 20.00m);

        _repositorioClienteMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(cliente);
        _servicoPagamentoMock.Setup(s => s.ProcessarAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new ResultadoPagamento(true, "TRX-LUIZA", null));
        _servicoFidelizacaoMock.Setup(s => s.CalcularPontos(It.IsAny<decimal>())).Returns(8);

        await _casoDeUso.ExecutarAsync(entrada);

        _repositorioPedidoMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
        _repositorioClienteMock.Verify(r => r.AtualizarAsync(cliente), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComMultiplosItens_DeveCalcularValorTotalCorretamente()
    {
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("Marcos", "marcos@email.com", "11999999999", true);
        var entrada = new RealizarPedidoEntrada(
            clienteId,
            Guid.NewGuid(),
            CanalAtendimento.Web,
            "pix",
            new[]
            {
                new ItemPedidoEntrada(Guid.NewGuid(), "Baiao de Dois", 1, 35.00m),
                new ItemPedidoEntrada(Guid.NewGuid(), "Suco de Umbu", 2, 12.00m),
                new ItemPedidoEntrada(Guid.NewGuid(), "Bolo de Macaxeira", 1, 18.00m)
            }
        );

        _repositorioClienteMock.Setup(r => r.ObterPorIdAsync(clienteId)).ReturnsAsync(cliente);
        _servicoPagamentoMock.Setup(s => s.ProcessarAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .ReturnsAsync(new ResultadoPagamento(true, "TRX-MARCOS", null));
        _servicoFidelizacaoMock.Setup(s => s.CalcularPontos(It.IsAny<decimal>())).Returns(77);

        var resultado = await _casoDeUso.ExecutarAsync(entrada);

        // 35 + (2*12) + 18 = 77.00
        resultado.ValorTotal.Should().Be(77.00m);
        resultado.NumeroPedido.Should().StartWith("RDN-");
    }

    private static RealizarPedidoEntrada CriarEntradaComUmItem(
        Guid clienteId, int quantidade, decimal preco)
        => new(
            clienteId,
            Guid.NewGuid(),
            CanalAtendimento.Aplicativo,
            "pix",
            new[] { new ItemPedidoEntrada(Guid.NewGuid(), "Carne do Sol", quantidade, preco) }
        );
}
