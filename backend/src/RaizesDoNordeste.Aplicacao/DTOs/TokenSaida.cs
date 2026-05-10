namespace RaizesDoNordeste.Aplicacao.DTOs;

public record TokenSaida(
    string Token,
    DateTime ExpiraEm,
    Guid ClienteId,
    string Nome,
    string Email
);
