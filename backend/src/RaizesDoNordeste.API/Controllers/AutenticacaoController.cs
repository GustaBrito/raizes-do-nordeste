using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.Aplicacao.CasosDeUso.AutenticarCliente;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;

namespace RaizesDoNordeste.API.Controllers;

[ApiController]
[Route("api/autenticacao")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticarClienteCasoDeUso _autenticar;

    public AutenticacaoController(IAutenticarClienteCasoDeUso autenticar)
    {
        _autenticar = autenticar;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenSaida), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginEntrada entrada)
    {
        try
        {
            var token = await _autenticar.ExecutarAsync(entrada);
            return Ok(token);
        }
        catch (DominioContinuarException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }
}
