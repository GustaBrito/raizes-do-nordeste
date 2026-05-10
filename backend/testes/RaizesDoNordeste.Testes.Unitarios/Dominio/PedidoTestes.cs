using FluentAssertions;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Enumeracoes;
using RaizesDoNordeste.Dominio.Excecoes;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Dominio;

public class PedidoTestes
{
    private readonly Guid _clienteId = Guid.NewGuid();
    private readonly Guid _franquiaId = Guid.NewGuid();

    private ItemPedido CriarItemValido()
        => new(Guid.NewGuid(), "X-Nordeste", 2, 25.90m);

    [Fact]
    public void NovoPedido_DeveIniciarComStatusRecebido()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Aplicativo);

        pedido.Status.Should().Be(StatusPedido.Recebido);
    }

    [Fact]
    public void AdicionarItem_ComPedidoRecebido_DeveAdicionarComSucesso()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Web);
        var item = CriarItemValido();

        pedido.AdicionarItem(item);

        pedido.Itens.Should().HaveCount(1);
        pedido.ValorTotal.Should().Be(51.80m);
    }

    [Fact]
    public void Confirmar_ComPedidoSemItens_DeveLancarPedidoInvalidoException()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Totem);

        var acao = () => pedido.Confirmar("RDN-001");

        acao.Should().Throw<PedidoInvalidoException>()
            .WithMessage("*ao menos um item*");
    }

    [Fact]
    public void Confirmar_ComItens_DeveAlterarStatusParaEmPreparacao()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Balcao);
        pedido.AdicionarItem(CriarItemValido());

        pedido.Confirmar("RDN-20240101-ABCD1234");

        pedido.Status.Should().Be(StatusPedido.EmPreparacao);
        pedido.NumeroPedido.Should().Be("RDN-20240101-ABCD1234");
    }

    [Fact]
    public void AdicionarItem_ComPedidoJaConfirmado_DeveLancarPedidoInvalidoException()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Pickup);
        pedido.AdicionarItem(CriarItemValido());
        pedido.Confirmar("RDN-001");

        var acao = () => pedido.AdicionarItem(CriarItemValido());

        acao.Should().Throw<PedidoInvalidoException>()
            .WithMessage("*pedido já confirmado*");
    }

    [Fact]
    public void Cancelar_ComPedidoEntregue_DeveLancarPedidoInvalidoException()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Aplicativo);
        pedido.AdicionarItem(CriarItemValido());
        pedido.Confirmar("RDN-001");
        pedido.MarcarProntoParaRetirada();

        // Simula entrega via reflection para o teste
        typeof(Pedido)
            .GetProperty(nameof(Pedido.Status))!
            .SetValue(pedido, StatusPedido.Entregue);

        var acao = () => pedido.Cancelar();

        acao.Should().Throw<PedidoInvalidoException>()
            .WithMessage("*pedido já entregue*");
    }

    [Fact]
    public void ValorTotal_ComMultiplosItens_DeveCalcularCorretamente()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Web);
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), "X-Nordeste", 2, 25.90m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), "Suco", 3, 8.00m));

        pedido.ValorTotal.Should().Be(75.80m);
    }
}
