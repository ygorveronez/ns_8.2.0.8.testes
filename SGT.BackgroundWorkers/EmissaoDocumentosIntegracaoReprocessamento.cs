using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 6000)]

    public class EmissaoDocumentosIntegracaoReprocessamento : LongRunningProcessBase<EmissaoDocumentosIntegracaoReprocessamento>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SolicitarEmissaoDocumentosAutorizadosReprocessamento(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
        }

        private void SolicitarEmissaoDocumentosAutorizadosReprocessamento(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int segundos = -configuracao.TempoSegundosParaInicioEmissaoDocumentos;
#if DEBUG
                segundos = 1;
#endif
                int quantidadeRegistros = 20;

                List<int> cargas = repCarga.BuscarCodigosCargasAutorizadasEmissaoNaEtapaNFe(DateTime.Now.AddSeconds(segundos), quantidadeRegistros, Dominio.Enumeradores.LoteCalculoFrete.Reprocessamento, configuracao.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, "DataInicioGeracaoCTes", "asc");
                cargas.AddRange(repCarga.BuscaCodigosCargasAutorizadasEmissaoNaEtapaDeFrete(DateTime.Now.AddSeconds(segundos), quantidadeRegistros, Dominio.Enumeradores.LoteCalculoFrete.Reprocessamento, configuracao.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos, configuracao.ExigirCargaRoteirizada, 0, 0, 0, 0));

                //se for multiEmbarcador agora pode ser que precise autorizar a emissão dos CT-es para filial emissora.
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    cargas.AddRange(repCarga.BuscarCodigosCargasAutorizadasEmissaoCTesSubContratacaoFilialEmissora(5, Dominio.Enumeradores.LoteCalculoFrete.Reprocessamento, "DataInicioGeracaoCTes", "asc"));

                for (var i = 0; i < cargas.Count; i++)
                {
                    Servicos.Log.TratarErro("Iniciando emissão documentos " + cargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosReprocessamento");

                    serCarga.ValidarEmissaoDocumentosCarga(cargas[i], unitOfWork, tipoServicoMultisoftware, webServiceConsultaCTe, 1, false);
                    serHubCarga.InformarCargaAtualizada(cargas[i], Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);

                    unitOfWork.FlushAndClear();

                    Servicos.Log.TratarErro("Finalizada emissão documentos " + cargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosReprocessamento");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

    }
}
