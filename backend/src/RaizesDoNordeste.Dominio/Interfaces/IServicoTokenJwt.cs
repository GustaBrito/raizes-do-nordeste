using RaizesDoNordeste.Dominio.Entidades;

namespace RaizesDoNordeste.Dominio.Interfaces;

public interface IServicoTokenJwt
{
    TokenGerado GerarToken(Cliente cliente);
}

public record TokenGerado(string Token, DateTime ExpiraEm);
