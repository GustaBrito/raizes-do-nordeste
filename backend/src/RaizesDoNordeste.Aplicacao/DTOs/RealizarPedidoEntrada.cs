using RaizesDoNordeste.Dominio.Enumeracoes;

namespace RaizesDoNordeste.Aplicacao.DTOs;

public record RealizarPedidoEntrada(
    Guid ClienteId,
    Guid FranquiaId,
    CanalAtendimento Canal,
    string MetodoPagamento,
    IEnumerable<ItemPedidoEntrada> Itens
);

public record ItemPedidoEntrada(
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario
);
