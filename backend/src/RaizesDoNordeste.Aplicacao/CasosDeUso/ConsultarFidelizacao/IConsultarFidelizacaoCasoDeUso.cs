using RaizesDoNordeste.Aplicacao.DTOs;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.ConsultarFidelizacao;

public interface IConsultarFidelizacaoCasoDeUso
{
    Task<SaldoFidelizacaoSaida> ExecutarAsync(Guid clienteId);
}
