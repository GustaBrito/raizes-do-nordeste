using RaizesDoNordeste.Aplicacao.DTOs;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.ResgatarPontos;

public interface IResgatarPontosCasoDeUso
{
    Task<SaldoFidelizacaoSaida> ExecutarAsync(ResgatarPontosEntrada entrada);
}
