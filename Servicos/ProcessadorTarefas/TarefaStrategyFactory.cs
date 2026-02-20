using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Servicos.ProcessadorTarefas.Estrategias;
using System;

namespace Servicos.ProcessadorTarefas;

public class TarefaStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TarefaStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TInterface ObterStrategy<TInterface>(TipoRequest tipoRequest) where TInterface : class
    {
        Type tipoStrategy = tipoRequest switch
        {
            TipoRequest.AdicionarPedidoEmLote => typeof(AdicionarPedidoEmLoteStrategy),
            TipoRequest.GerarCarregamento => typeof(GerarCarregamentoStrategy),
            TipoRequest.GerarCarregamentoComRedespachos => typeof(GerarCarregamentoComRedespachoStrategy),
            TipoRequest.EnviarDigitalizacaoCanhotoEmLote => typeof(EnviarDigitalizacaoCanhotoStrategy),
            TipoRequest.AdicionarAtendimentoEmLote => typeof(AdicionarAtendimentoStrategy),
            TipoRequest.GerarCarregamentoRoteirizacaoEmLote => typeof(GerarCarregamentoRoteirizacaoEmLoteStrategy),
            _ => throw new ServicoException($"Estratégia '{tipoRequest}' não encontrada.")
        };

        TInterface strategy = (TInterface)_serviceProvider.GetService(tipoStrategy);

        if (strategy == null)
            throw new ServicoException($"Não foi possível resolver a estratégia para '{tipoRequest}'.");

        return strategy;
    }
}
