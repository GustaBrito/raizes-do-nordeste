using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Interfaces;
using RaizesDoNordeste.Infraestrutura.Persistencia;

namespace RaizesDoNordeste.Infraestrutura.Repositorios;

public class RepositorioPedido : IRepositorioPedido
{
    private readonly ContextoBancoDados _contexto;

    public RepositorioPedido(ContextoBancoDados contexto) => _contexto = contexto;

    public async Task<Pedido?> ObterPorIdAsync(Guid id)
        => await _contexto.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Pedido?> ObterPorNumeroPedidoAsync(string numeroPedido)
        => await _contexto.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido);

    public async Task<IEnumerable<Pedido>> ListarPorClienteAsync(Guid clienteId)
        => await _contexto.Pedidos.Include(p => p.Itens).Where(p => p.ClienteId == clienteId).ToListAsync();

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _contexto.Pedidos.AddAsync(pedido);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pedido pedido)
    {
        _contexto.Pedidos.Update(pedido);
        await _contexto.SaveChangesAsync();
    }
}
