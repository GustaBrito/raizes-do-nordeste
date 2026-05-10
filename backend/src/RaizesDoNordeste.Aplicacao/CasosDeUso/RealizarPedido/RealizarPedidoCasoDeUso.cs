using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.RealizarPedido;

public class RealizarPedidoCasoDeUso : IRealizarPedidoCasoDeUso
{
    private readonly IRepositorioCliente _repositorioCliente;
    private readonly IRepositorioPedido _repositorioPedido;
    private readonly IServicoPagamento _servicoPagamento;
    private readonly IServicoFidelizacao _servicoFidelizacao;

    public RealizarPedidoCasoDeUso(
        IRepositorioCliente repositorioCliente,
        IRepositorioPedido repositorioPedido,
        IServicoPagamento servicoPagamento,
        IServicoFidelizacao servicoFidelizacao)
    {
        _repositorioCliente = repositorioCliente;
        _repositorioPedido = repositorioPedido;
        _servicoPagamento = servicoPagamento;
        _servicoFidelizacao = servicoFidelizacao;
    }

    public async Task<RealizarPedidoSaida> ExecutarAsync(RealizarPedidoEntrada entrada)
    {
        var cliente = await _repositorioCliente.ObterPorIdAsync(entrada.ClienteId)
            ?? throw new ClienteNaoEncontradoException(entrada.ClienteId);

        var pedido = new Pedido(entrada.ClienteId, entrada.FranquiaId, entrada.Canal);

        foreach (var itemEntrada in entrada.Itens)
        {
            var item = new ItemPedido(
                itemEntrada.ProdutoId,
                itemEntrada.NomeProduto,
                itemEntrada.Quantidade,
                itemEntrada.PrecoUnitario
            );
            pedido.AdicionarItem(item);
        }

        var resultadoPagamento = await _servicoPagamento.ProcessarAsync(
            pedido.Id, pedido.ValorTotal, entrada.MetodoPagamento);

        if (!resultadoPagamento.Sucesso)
            throw new InvalidOperationException($"Falha no pagamento: {resultadoPagamento.MensagemErro}");

        var numeroPedido = GerarNumeroPedido();
        pedido.Confirmar(numeroPedido);

        var pontosAdicionados = _servicoFidelizacao.CalcularPontos(pedido.ValorTotal);
        cliente.AdicionarPontos(pontosAdicionados);

        await _repositorioPedido.AdicionarAsync(pedido);
        await _repositorioCliente.AtualizarAsync(cliente);

        return new RealizarPedidoSaida(
            pedido.Id,
            pedido.NumeroPedido!,
            pedido.Status,
            pedido.ValorTotal,
            pontosAdicionados,
            resultadoPagamento.CodigoTransacao!
        );
    }

    private static string GerarNumeroPedido()
        => $"RDN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
