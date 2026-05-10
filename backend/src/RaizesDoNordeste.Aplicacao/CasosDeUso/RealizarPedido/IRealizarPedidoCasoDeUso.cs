using RaizesDoNordeste.Aplicacao.DTOs;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.RealizarPedido;

public interface IRealizarPedidoCasoDeUso
{
    Task<RealizarPedidoSaida> ExecutarAsync(RealizarPedidoEntrada entrada);
}
