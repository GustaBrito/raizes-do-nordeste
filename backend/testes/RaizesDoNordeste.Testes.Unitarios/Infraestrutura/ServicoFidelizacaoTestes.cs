using FluentAssertions;
using RaizesDoNordeste.Infraestrutura.Servicos;
using Xunit;

namespace RaizesDoNordeste.Testes.Unitarios.Infraestrutura;

public class ServicoFidelizacaoTestes
{
    private readonly ServicoFidelizacao _servico = new();

    [Theory]
    [InlineData(29.99, 0)]
    [InlineData(30.00, 30)]
    [InlineData(100.00, 100)]
    [InlineData(150.50, 150)]
    public void CalcularPontos_DeveRetornarPontosCorretos(decimal valor, int pontosEsperados)
    {
        var pontos = _servico.CalcularPontos(valor);

        pontos.Should().Be(pontosEsperados);
    }

    [Fact]
    public void CalcularPontos_AbaixoDoMinimo_DeveRetornarZero()
    {
        var pontos = _servico.CalcularPontos(20m);

        pontos.Should().Be(0);
    }
}
