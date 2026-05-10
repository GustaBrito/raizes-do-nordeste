using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.ConsultarFidelizacao;

public class ConsultarFidelizacaoCasoDeUso : IConsultarFidelizacaoCasoDeUso
{
    private readonly IRepositorioCliente _repositorioCliente;

    public ConsultarFidelizacaoCasoDeUso(IRepositorioCliente repositorioCliente)
    {
        _repositorioCliente = repositorioCliente;
    }

    public async Task<SaldoFidelizacaoSaida> ExecutarAsync(Guid clienteId)
    {
        var cliente = await _repositorioCliente.ObterPorIdAsync(clienteId);
        if (cliente is null)
            throw new ClienteNaoEncontradoException(clienteId);

        return new SaldoFidelizacaoSaida(
            ClienteId: cliente.Id,
            NomeCliente: cliente.Nome,
            SaldoPontos: cliente.PontosFidelizacao
        );
    }
}
