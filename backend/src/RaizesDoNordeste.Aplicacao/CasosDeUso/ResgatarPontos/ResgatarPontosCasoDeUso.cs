using RaizesDoNordeste.Aplicacao.DTOs;
using RaizesDoNordeste.Dominio.Excecoes;
using RaizesDoNordeste.Dominio.Interfaces;

namespace RaizesDoNordeste.Aplicacao.CasosDeUso.ResgatarPontos;

public class ResgatarPontosCasoDeUso : IResgatarPontosCasoDeUso
{
    private readonly IRepositorioCliente _repositorioCliente;

    public ResgatarPontosCasoDeUso(IRepositorioCliente repositorioCliente)
    {
        _repositorioCliente = repositorioCliente;
    }

    public async Task<SaldoFidelizacaoSaida> ExecutarAsync(ResgatarPontosEntrada entrada)
    {
        var cliente = await _repositorioCliente.ObterPorIdAsync(entrada.ClienteId);
        if (cliente is null)
            throw new ClienteNaoEncontradoException(entrada.ClienteId);

        cliente.ResgatarPontos(entrada.QuantidadePontos);
        await _repositorioCliente.AtualizarAsync(cliente);

        return new SaldoFidelizacaoSaida(
            ClienteId: cliente.Id,
            NomeCliente: cliente.Nome,
            SaldoPontos: cliente.PontosFidelizacao
        );
    }
}
