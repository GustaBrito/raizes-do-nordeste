using RaizesDoNordeste.Dominio.Entidades;

namespace RaizesDoNordeste.Dominio.Interfaces;

public interface IRepositorioPedido
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task<Pedido?> ObterPorNumeroPedidoAsync(string numeroPedido);
    Task<IEnumerable<Pedido>> ListarPorClienteAsync(Guid clienteId);
    Task AdicionarAsync(Pedido pedido);
    Task AtualizarAsync(Pedido pedido);
}
