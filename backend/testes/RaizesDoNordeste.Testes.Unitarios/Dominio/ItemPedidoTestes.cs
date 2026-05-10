using FluentAssertions;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Excecoes;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Dominio;

public class ItemPedidoTestes
{
    [Fact]
    public void NovoItem_ComDadosValidos_DeveCalcularSubtotalCorretamente()
    {
        var item = new ItemPedido(Guid.NewGuid(), "X-Nordeste", 3, 25.90m);

        item.Subtotal.Should().Be(77.70m);
    }

    [Fact]
    public void NovoItem_ComQuantidadeZero_DeveLancarExcecao()
    {
        var acao = () => new ItemPedido(Guid.NewGuid(), "X-Nordeste", 0, 25.90m);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*quantidade*maior que zero*");
    }

    [Fact]
    public void NovoItem_ComPrecoNegativo_DeveLancarExcecao()
    {
        var acao = () => new ItemPedido(Guid.NewGuid(), "X-Nordeste", 1, -10m);

        acao.Should().Throw<DominioContinuarException>()
            .WithMessage("*preço unitário*maior que zero*");
    }
}
