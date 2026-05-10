namespace RaizesDoNordeste.Dominio.Excecoes;

public class DominioContinuarException : Exception
{
    public DominioContinuarException(string mensagem) : base(mensagem) { }
}
