using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.AutenticarCliente;

public class AutenticarClienteCasoDeUso : IAutenticarClienteCasoDeUso
{
    private const string MENSAGEM_CREDENCIAIS_INVALIDAS = "Credenciais inválidas. Tente novamente.";

    private readonly IRepositorioCliente _repositorioCliente;
    private readonly IServicoSenha _servicoSenha;
    private readonly IServicoTokenJwt _servicoToken;

    public AutenticarClienteCasoDeUso(
        IRepositorioCliente repositorioCliente,
        IServicoSenha servicoSenha,
        IServicoTokenJwt servicoToken)
    {
        _repositorioCliente = repositorioCliente;
        _servicoSenha = servicoSenha;
        _servicoToken = servicoToken;
    }

    public async Task<TokenSaida> ExecutarAsync(LoginEntrada entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada.Email) || string.IsNullOrWhiteSpace(entrada.Senha))
            throw new DominioContinuarException(MENSAGEM_CREDENCIAIS_INVALIDAS);

        var cliente = await _repositorioCliente.ObterPorEmailAsync(entrada.Email);
        if (cliente is null || string.IsNullOrEmpty(cliente.SenhaHash))
            throw new DominioContinuarException(MENSAGEM_CREDENCIAIS_INVALIDAS);

        var senhaCorreta = _servicoSenha.VerificarSenha(entrada.Senha, cliente.SenhaHash);
        if (!senhaCorreta)
            throw new DominioContinuarException(MENSAGEM_CREDENCIAIS_INVALIDAS);

        var token = _servicoToken.GerarToken(cliente);

        return new TokenSaida(
            Token: token.Token,
            ExpiraEm: token.ExpiraEm,
            ClienteId: cliente.Id,
            Nome: cliente.Nome,
            Email: cliente.Email
        );
    }
}
