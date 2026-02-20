using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Servicos.ProcessadorTarefas.Etapas;
using System;

namespace Servicos.ProcessadorTarefas
{
    public class EtapaStateFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EtapaStateFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public EtapaState ObterEtapa(TipoEtapaTarefa tipoEtapaTarefa)
        {
            Type tipoEtapa = tipoEtapaTarefa switch
            {
                TipoEtapaTarefa.QuebrarRequest => typeof(QuebrarRequest),
                TipoEtapaTarefa.AdicionarPedido => typeof(AdicionarPedido),
                TipoEtapaTarefa.GerarCarregamento => typeof(GerarCarregamento),
                TipoEtapaTarefa.GerarCarregamentoComRedespachos => typeof(GerarCarregamentoComRedespacho),
                TipoEtapaTarefa.RetornarIntegracao => typeof(RetornarIntegracao),
                TipoEtapaTarefa.FecharCarga => typeof(FecharCarga),
                TipoEtapaTarefa.EnviarDigitalizacaoCanhoto => typeof(EnviarDigitalizacaoCanhoto),
                TipoEtapaTarefa.AdicionarAtendimento => typeof(AdicionarAtendimento),
                TipoEtapaTarefa.GerarCarregamentoRoterizacao => typeof(GerarCarregamentoRoteirizacaoEmLote),
                _ => throw new ServicoException($"Etapa '{tipoEtapaTarefa}' não encontrada.")
            };

            EtapaState etapa = _serviceProvider.GetService(tipoEtapa) as EtapaState;

            if (etapa == null)
                throw new ServicoException($"Não foi possível resolver a etapa '{tipoEtapaTarefa}'.");

            return etapa;
        }
    }
}
