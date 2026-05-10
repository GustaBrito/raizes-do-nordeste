using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Infraestrutura.Servicos;

public class ServicoPagamentoMock : IServicoPagamento
{
    private const decimal LIMITE_VALOR_SIMULADO = 10000m;

    public async Task<ResultadoPagamento> ProcessarAsync(Guid pedidoId, decimal valor, string metodoPagamento)
    {
        await Task.Delay(100); // Simula latência de API externa

        if (valor > LIMITE_VALOR_SIMULADO)
            return new ResultadoPagamento(false, null, "Valor acima do limite permitido para simulação.");

        var codigoTransacao = $"TRX-MOCK-{Guid.NewGuid().ToString()[..12].ToUpper()}";
        return new ResultadoPagamento(true, codigoTransacao, null);
    }
}
