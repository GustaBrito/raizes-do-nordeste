using RaizesDoNordeste.Dominio.Excecoes;

namespace RaizesDoNordeste.Dominio.Entidades;

public class Cliente : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public string? SenhaHash { get; private set; }
    public int PontosFidelizacao { get; private set; }
    public bool ConsentimentoLgpd { get; private set; }
    public DateTime? DataConsentimentoLgpd { get; private set; }

    private Cliente() { }

    public Cliente(string nome, string email, string? telefone, bool consentimentoLgpd)
    {
        ValidarNome(nome);
        ValidarEmail(email);

        if (!consentimentoLgpd)
            throw new DominioContinuarException("O consentimento com a política de privacidade é obrigatório.");

        Nome = nome;
        Email = email;
        Telefone = telefone;
        PontosFidelizacao = 0;
        ConsentimentoLgpd = true;
        DataConsentimentoLgpd = DateTime.UtcNow;
    }

    public void DefinirSenhaHash(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DominioContinuarException("O hash de senha não pode ser vazio.");

        SenhaHash = senhaHash;
        MarcarAtualizado();
    }

    public void AdicionarPontos(int pontos)
    {
        if (pontos <= 0)
            throw new DominioContinuarException("A quantidade de pontos deve ser positiva.");

        PontosFidelizacao += pontos;
        MarcarAtualizado();
    }

    public void ResgatarPontos(int pontos)
    {
        if (pontos <= 0)
            throw new DominioContinuarException("A quantidade de pontos deve ser positiva.");
        if (pontos > PontosFidelizacao)
            throw new DominioContinuarException("Saldo de pontos insuficiente para resgate.");

        PontosFidelizacao -= pontos;
        MarcarAtualizado();
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DominioContinuarException("O nome do cliente não pode ser vazio.");
    }

    private static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DominioContinuarException("O e-mail informado é inválido.");
    }
}
