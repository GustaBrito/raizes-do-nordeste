using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Infraestrutura.Servicos;

public class ServicoFidelizacao : IServicoFidelizacao
{
    private const decimal VALOR_MINIMO_PARA_PONTOS = 30m;
    private const int PONTOS_POR_REAL = 1;

    public int CalcularPontos(decimal valorPedido)
    {
        if (valorPedido < VALOR_MINIMO_PARA_PONTOS)
            return 0;

        return (int)(valorPedido * PONTOS_POR_REAL);
    }
}
