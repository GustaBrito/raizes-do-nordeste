using RaizesDoNordeste.Aplicacao.DTOs;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.CadastrarCliente;

public interface ICadastrarClienteCasoDeUso
{
    Task<Guid> ExecutarAsync(CadastrarClienteEntrada entrada);
}
