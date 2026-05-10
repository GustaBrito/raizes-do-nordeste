using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.Aplicacao.CasosDeUso.ConsultarFidelizacao;
using RaizesDoNordeste.Aplicacao.CasosDeUso.ResgatarPontos;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;

namespace RaizesDoNordeste.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FidelizacaoController : ControllerBase
{
    private readonly IConsultarFidelizacaoCasoDeUso _consultarFidelizacao;
    private readonly IResgatarPontosCasoDeUso _resgatarPontos;

    public FidelizacaoController(
        IConsultarFidelizacaoCasoDeUso consultarFidelizacao,
        IResgatarPontosCasoDeUso resgatarPontos)
    {
        _consultarFidelizacao = consultarFidelizacao;
        _resgatarPontos = resgatarPontos;
    }

    [HttpGet("{clienteId:guid}")]
    [ProducesResponseType(typeof(SaldoFidelizacaoSaida), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarSaldo(Guid clienteId)
    {
        try
        {
            var saldo = await _consultarFidelizacao.ExecutarAsync(clienteId);
            return Ok(saldo);
        }
        catch (ClienteNaoEncontradoException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPost("resgatar")]
    [ProducesResponseType(typeof(SaldoFidelizacaoSaida), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResgatarPontos([FromBody] ResgatarPontosEntrada entrada)
    {
        try
        {
            var saldoAtualizado = await _resgatarPontos.ExecutarAsync(entrada);
            return Ok(saldoAtualizado);
        }
        catch (ClienteNaoEncontradoException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (DominioContinuarException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
