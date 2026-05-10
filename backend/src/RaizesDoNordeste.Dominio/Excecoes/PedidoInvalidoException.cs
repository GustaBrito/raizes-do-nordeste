namespace RaizesDoNordeste.Dominio.Excecoes;

public class PedidoInvalidoException : Exception
{
    public PedidoInvalidoException(string mensagem) : base(mensagem) { }
}
