namespace RaizesDoNordeste.Dominio.Interfaces;

public interface IServicoSenha
{
    string GerarHash(string senhaPlana);
    bool VerificarSenha(string senhaPlana, string hashArmazenado);
}
