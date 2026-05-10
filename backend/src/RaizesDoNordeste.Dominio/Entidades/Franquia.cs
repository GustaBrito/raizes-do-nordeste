using RaizesDoNordeste.Dominio.Excecoes;

namespace RaizesDoNordeste.Dominio.Entidades;

public class Franquia : EntidadeBase
{
    public string Nome { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }
    public string Endereco { get; private set; }
    public bool Ativa { get; private set; }

    private Franquia() { }

    public Franquia(string nome, string cidade, string estado, string endereco)
    {
        ValidarCampo(nome, "O nome da franquia não pode ser vazio.");
        ValidarCampo(cidade, "A cidade não pode ser vazia.");
        ValidarCampo(estado, "O estado não pode ser vazio.");
        ValidarCampo(endereco, "O endereço não pode ser vazio.");

        Nome = nome;
        Cidade = cidade;
        Estado = estado;
        Endereco = endereco;
        Ativa = true;
    }

    public void Desativar()
    {
        Ativa = false;
        MarcarAtualizado();
    }

    private static void ValidarCampo(string valor, string mensagem)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioContinuarException(mensagem);
    }
}
