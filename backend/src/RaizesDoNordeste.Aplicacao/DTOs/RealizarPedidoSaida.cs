using RaizesDoNordeste.Dominio.Enumeracoes;

namespace RaizesDoNordeste.Aplicacao.DTOs;

public record RealizarPedidoSaida(
    Guid PedidoId,
    string NumeroPedido,
    StatusPedido Status,
    decimal ValorTotal,
    int PontosAdicionados,
    string CodigoTransacaoPagamento
);
