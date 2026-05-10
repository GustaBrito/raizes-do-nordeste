using RaizesDoNordeste.Dominio.Excecoes;

namespace RaizesDoNordeste.Dominio.Entidades;

public class ItemPedido : EntidadeBase
{
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; }
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => PrecoUnitario * Quantidade;

    private ItemPedido() { }

    public ItemPedido(Guid produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        ValidarQuantidade(quantidade);
        ValidarPrecoUnitario(precoUnitario);

        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    private static void ValidarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new DominioContinuarException("A quantidade do item deve ser maior que zero.");
    }

    private static void ValidarPrecoUnitario(decimal precoUnitario)
    {
        if (precoUnitario <= 0)
            throw new DominioContinuarException("O preço unitário deve ser maior que zero.");
    }
}
