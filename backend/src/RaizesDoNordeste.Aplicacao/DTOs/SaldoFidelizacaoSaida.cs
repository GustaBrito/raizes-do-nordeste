namespace RaizesDoNordeste.Aplicacao.DTOs;

public record SaldoFidelizacaoSaida(
    Guid ClienteId,
    string NomeCliente,
    int SaldoPontos
);
