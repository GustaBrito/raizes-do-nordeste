namespace RaizesDoNordeste.Aplicacao.DTOs;

public record CadastrarClienteEntrada(
    string Nome,
    string Email,
    string? Telefone,
    bool ConsentimentoLgpd,
    string? Senha = null
);
