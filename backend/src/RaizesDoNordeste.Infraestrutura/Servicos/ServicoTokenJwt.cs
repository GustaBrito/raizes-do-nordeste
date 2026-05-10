using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RaizesDoNordeste.Dominio.Entidades;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Infraestrutura.Servicos;

public class ServicoTokenJwt : IServicoTokenJwt
{
    private readonly ConfiguracaoTokenJwt _configuracao;

    public ServicoTokenJwt(ConfiguracaoTokenJwt configuracao)
    {
        _configuracao = configuracao;
    }

    public TokenGerado GerarToken(Cliente cliente)
    {
        var manipuladorToken = new JwtSecurityTokenHandler();
        var bytesChave = Encoding.UTF8.GetBytes(_configuracao.ChaveSecreta);
        var expiraEm = DateTime.UtcNow.AddMinutes(_configuracao.MinutosValidade);

        var descricaoToken = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, cliente.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, cliente.Email),
                new Claim("nome", cliente.Nome),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = expiraEm,
            Issuer = _configuracao.Emissor,
            Audience = _configuracao.Audiencia,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(bytesChave),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenSeguranca = manipuladorToken.CreateToken(descricaoToken);
        var tokenSerializado = manipuladorToken.WriteToken(tokenSeguranca);

        return new TokenGerado(tokenSerializado, expiraEm);
    }
}

public class ConfiguracaoTokenJwt
{
    public string ChaveSecreta { get; set; } = string.Empty;
    public string Emissor { get; set; } = "RaizesDoNordeste";
    public string Audiencia { get; set; } = "RaizesDoNordesteClientes";
    public int MinutosValidade { get; set; } = 60;
}
