using Microsoft.EntityFrameworkCore;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Interfaces;
using RaizesDoNordeste.Infraestrutura.Persistencia;

namespace RaizesDoNordeste.Infraestrutura.Repositorios;

public class RepositorioCliente : IRepositorioCliente
{
    private readonly ContextoBancoDados _contexto;

    public RepositorioCliente(ContextoBancoDados contexto) => _contexto = contexto;

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
        => await _contexto.Clientes.FindAsync(id);

    public async Task<Cliente?> ObterPorEmailAsync(string email)
        => await _contexto.Clientes.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<bool> ExisteEmailAsync(string email)
        => await _contexto.Clientes.AnyAsync(c => c.Email == email);

    public async Task AdicionarAsync(Cliente cliente)
    {
        await _contexto.Clientes.AddAsync(cliente);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        _contexto.Clientes.Update(cliente);
        await _contexto.SaveChangesAsync();
    }
}
