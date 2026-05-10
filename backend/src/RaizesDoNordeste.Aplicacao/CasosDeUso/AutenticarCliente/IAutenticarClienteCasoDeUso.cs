using RaizesDoNordeste.Aplicacao.DTOs;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.AutenticarCliente;

public interface IAutenticarClienteCasoDeUso
{
    Task<TokenSaida> ExecutarAsync(LoginEntrada entrada);
}
