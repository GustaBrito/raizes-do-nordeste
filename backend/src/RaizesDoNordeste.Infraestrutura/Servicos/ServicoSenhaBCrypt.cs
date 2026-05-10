using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Infraestrutura.Servicos;

public class ServicoSenhaBCrypt : IServicoSenha
{
    private const int FATOR_TRABALHO = 11;

    public string GerarHash(string senhaPlana)
    {
        if (string.IsNullOrWhiteSpace(senhaPlana))
            throw new ArgumentException("A senha não pode ser vazia.", nameof(senhaPlana));

        return BCrypt.Net.BCrypt.HashPassword(senhaPlana, FATOR_TRABALHO);
    }

    public bool VerificarSenha(string senhaPlana, string hashArmazenado)
    {
        if (string.IsNullOrWhiteSpace(senhaPlana) || string.IsNullOrWhiteSpace(hashArmazenado))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(senhaPlana, hashArmazenado);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
