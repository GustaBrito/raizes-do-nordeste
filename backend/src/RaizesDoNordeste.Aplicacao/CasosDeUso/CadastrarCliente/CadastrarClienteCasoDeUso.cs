using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.CadastrarCliente;

public class CadastrarClienteCasoDeUso : ICadastrarClienteCasoDeUso
{
    private const int TAMANHO_MINIMO_SENHA = 8;

    private readonly IRepositorioCliente _repositorioCliente;
    private readonly IServicoSenha _servicoSenha;

    public CadastrarClienteCasoDeUso(
        IRepositorioCliente repositorioCliente,
        IServicoSenha servicoSenha)
    {
        _repositorioCliente = repositorioCliente;
        _servicoSenha = servicoSenha;
    }

    public async Task<Guid> ExecutarAsync(CadastrarClienteEntrada entrada)
    {
        var emailJaCadastrado = await _repositorioCliente.ExisteEmailAsync(entrada.Email);
        if (emailJaCadastrado)
            throw new DominioContinuarException($"O e-mail '{entrada.Email}' já está cadastrado.");

        var cliente = new Cliente(entrada.Nome, entrada.Email, entrada.Telefone, entrada.ConsentimentoLgpd);

        if (!string.IsNullOrEmpty(entrada.Senha))
        {
            if (entrada.Senha.Length < TAMANHO_MINIMO_SENHA)
                throw new DominioContinuarException($"A senha deve ter pelo menos {TAMANHO_MINIMO_SENHA} caracteres.");

            var hashSenha = _servicoSenha.GerarHash(entrada.Senha);
            cliente.DefinirSenhaHash(hashSenha);
        }

        await _repositorioCliente.AdicionarAsync(cliente);

        return cliente.Id;
    }
}
