using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesDoNordeste.Aplicacao.CasosDeUso.CadastrarCliente;
using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ICadastrarClienteCasoDeUso _cadastrarCliente;
    private readonly IRepositorioPedido _repositorioPedido;

    public ClientesController(ICadastrarClienteCasoDeUso cadastrarCliente, IRepositorioPedido repositorioPedido)
    {
        _cadastrarCliente = cadastrarCliente;
        _repositorioPedido = repositorioPedido;
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CadastrarCliente([FromBody] CadastrarClienteEntrada entrada)
    {
        try
        {
            var clienteId = await _cadastrarCliente.ExecutarAsync(entrada);
            return CreatedAtAction(nameof(CadastrarCliente), new { id = clienteId }, new { id = clienteId });
        }
        catch (DominioContinuarException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{clienteId:guid}/pedidos")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPedidos(Guid clienteId)
    {
        var pedidos = await _repositorioPedido.ListarPorClienteAsync(clienteId);
        var resultado = pedidos.Select(p => new
        {
            numeroPedido = p.NumeroPedido,
            dataPedido = p.CriadoEm,
            status = p.Status.ToString(),
            quantidadeItens = p.Itens.Count,
            valorTotal = p.ValorTotal
        });
        return Ok(resultado);
    }
}
