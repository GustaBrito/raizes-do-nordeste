using RaizesDoNordeste.Dominio.Entidades;

namespace RaizesDoNordeste.Dominio.Interfaces;

public interface IRepositorioCliente
{
    Task<Cliente?> ObterPorIdAsync(Guid id);
    Task<Cliente?> ObterPorEmailAsync(string email);
    Task<bool> ExisteEmailAsync(string email);
    Task AdicionarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
}
