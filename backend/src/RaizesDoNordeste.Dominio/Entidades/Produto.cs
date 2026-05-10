using RaizesDoNordeste.Dominio.Excecoes;

namespace RaizesDoNordeste.Dominio.Entidades;

public class Produto : EntidadeBase
{
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public bool Disponivel { get; private set; }
    public Guid FranquiaId { get; private set; }

    private Produto() { }

    public Produto(string nome, string descricao, decimal preco, Guid franquiaId)
    {
        ValidarNome(nome);
        ValidarPreco(preco);

        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        FranquiaId = franquiaId;
        Disponivel = true;
    }

    public void AtualizarPreco(decimal novoPreco)
    {
        ValidarPreco(novoPreco);
        Preco = novoPreco;
        MarcarAtualizado();
    }

    public void AlterarDisponibilidade(bool disponivel)
    {
        Disponivel = disponivel;
        MarcarAtualizado();
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DominioContinuarException("O nome do produto não pode ser vazio.");
    }

    private static void ValidarPreco(decimal preco)
    {
        if (preco <= 0)
            throw new DominioContinuarException("O preço do produto deve ser maior que zero.");
    }
}
