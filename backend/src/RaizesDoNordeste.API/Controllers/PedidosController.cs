using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.Aplicacao.CasosDeUso.RealizarPedido;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IRealizarPedidoCasoDeUso _realizarPedido;
    private readonly IRepositorioPedido _repositorioPedido;

    public PedidosController(IRealizarPedidoCasoDeUso realizarPedido, IRepositorioPedido repositorioPedido)
    {
        _realizarPedido = realizarPedido;
        _repositorioPedido = repositorioPedido;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RealizarPedidoSaida), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarPedido([FromBody] RealizarPedidoEntrada entrada)
    {
        try
        {
            var resultado = await _realizarPedido.ExecutarAsync(entrada);
            return CreatedAtAction(nameof(RealizarPedido), new { id = resultado.PedidoId }, resultado);
        }
        catch (ClienteNaoEncontradoException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (PedidoInvalidoException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{numeroPedido}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterStatus(string numeroPedido)
    {
        var pedido = await _repositorioPedido.ObterPorNumeroPedidoAsync(numeroPedido);
        if (pedido is null)
            return NotFound(new { mensagem = $"Pedido '{numeroPedido}' não encontrado." });

        return Ok(new
        {
            numeroPedido = pedido.NumeroPedido,
            status = pedido.Status.ToString(),
            valorTotal = pedido.ValorTotal,
            pontosAdicionados = (int)pedido.ValorTotal
        });
    }
}
