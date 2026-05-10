using FluentAssertions;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Enumeracoes;
using RaizesDoNordeste.Dominio.Excecoes;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Dominio;

public class PedidoComportamentoTestes
{
    private readonly Guid _clienteId = Guid.NewGuid();
    private readonly Guid _franquiaId = Guid.NewGuid();

    private ItemPedido CriarItemValido()
        => new(Guid.NewGuid(), "Carne de Sol", 1, 38.00m);

    private Pedido CriarPedidoConfirmado()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Balcao);
        pedido.AdicionarItem(CriarItemValido());
        pedido.Confirmar("RDN-TESTE-001");
        return pedido;
    }

    [Fact]
    public void Cancelar_ComPedidoRecebido_DeveAlterarStatusParaCancelado()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Aplicativo);
        pedido.AdicionarItem(CriarItemValido());

        pedido.Cancelar();

        pedido.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public void Cancelar_ComPedidoEmPreparacao_DeveAlterarStatusParaCancelado()
    {
        var pedido = CriarPedidoConfirmado();

        pedido.Cancelar();

        pedido.Status.Should().Be(StatusPedido.Cancelado);
    }

    [Fact]
    public void MarcarProntoParaRetirada_ComPedidoRecebido_DeveLancarPedidoInvalidoException()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Web);

        var acao = () => pedido.MarcarProntoParaRetirada();

        acao.Should().Throw<PedidoInvalidoException>()
            .WithMessage("*em preparação*");
    }

    [Fact]
    public void MarcarProntoParaRetirada_ComPedidoEmPreparacao_DeveAlterarStatusParaProntoParaRetirada()
    {
        var pedido = CriarPedidoConfirmado();

        pedido.MarcarProntoParaRetirada();

        pedido.Status.Should().Be(StatusPedido.ProntoParaRetirada);
    }

    [Fact]
    public void AdicionarItem_ComMultiplosItens_DeveAcumularCorretamente()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Totem);
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), "Tapioca", 2, 12.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), "Cafe", 3, 5.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), "Bolo de Rolo", 1, 18.50m));

        pedido.Itens.Should().HaveCount(3);
        pedido.ValorTotal.Should().Be(57.50m); // (2*12) + (3*5) + (1*18.50) = 24 + 15 + 18.50
    }

    [Fact]
    public void NovoPedido_DevePreservarCanalAtendimentoInformado()
    {
        var pedido = new Pedido(_clienteId, _franquiaId, CanalAtendimento.Pickup);

        pedido.Canal.Should().Be(CanalAtendimento.Pickup);
        pedido.ClienteId.Should().Be(_clienteId);
        pedido.FranquiaId.Should().Be(_franquiaId);
    }
}
