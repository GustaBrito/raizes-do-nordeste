namespace RaizesDoNordeste.Dominio.Interfaces;

public interface IServicoPagamento
{
    Task<ResultadoPagamento> ProcessarAsync(Guid pedidoId, decimal valor, string metodoPagamento);
}

public record ResultadoPagamento(bool Sucesso, string? CodigoTransacao, string? MensagemErro);
