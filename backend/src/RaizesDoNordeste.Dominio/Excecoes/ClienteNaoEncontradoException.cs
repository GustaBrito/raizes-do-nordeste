namespace RaizesDoNordeste.Dominio.Excecoes;

public class ClienteNaoEncontradoException : Exception
{
    public ClienteNaoEncontradoException(Guid clienteId)
        : base($"Cliente com ID {clienteId} não encontrado.") { }
}
