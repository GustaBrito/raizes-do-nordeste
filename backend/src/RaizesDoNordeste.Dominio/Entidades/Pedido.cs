using RaizesDoNordeste.Dominio.Enumeracoes;
using RaizesDoNordeste.Dominio.Excecoes;

namespace RaizesDoNordeste.Dominio.Entidades;

public class Pedido : EntidadeBase
{
    private readonly List<ItemPedido> _itens = new();

    public Guid ClienteId { get; private set; }
    public Guid FranquiaId { get; private set; }
    public CanalAtendimento Canal { get; private set; }
    public StatusPedido Status { get; private set; }
    public string? NumeroPedido { get; private set; }
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();
    public decimal ValorTotal => _itens.Sum(i => i.Subtotal);

    private Pedido() { }

    public Pedido(Guid clienteId, Guid franquiaId, CanalAtendimento canal)
    {
        ClienteId = clienteId;
        FranquiaId = franquiaId;
        Canal = canal;
        Status = StatusPedido.Recebido;
    }

    public void AdicionarItem(ItemPedido item)
    {
        if (Status != StatusPedido.Recebido)
            throw new PedidoInvalidoException("Não é possível adicionar itens a um pedido já confirmado.");

        _itens.Add(item);
        MarcarAtualizado();
    }

    public void Confirmar(string numeroPedido)
    {
        if (!_itens.Any())
            throw new PedidoInvalidoException("Um pedido deve conter ao menos um item.");

        NumeroPedido = numeroPedido;
        Status = StatusPedido.EmPreparacao;
        MarcarAtualizado();
    }

    public void MarcarProntoParaRetirada()
    {
        if (Status != StatusPedido.EmPreparacao)
            throw new PedidoInvalidoException("O pedido precisa estar em preparação para ser marcado como pronto.");

        Status = StatusPedido.ProntoParaRetirada;
        MarcarAtualizado();
    }

    public void Cancelar()
    {
        if (Status == StatusPedido.Entregue)
            throw new PedidoInvalidoException("Não é possível cancelar um pedido já entregue.");

        Status = StatusPedido.Cancelado;
        MarcarAtualizado();
    }
}
