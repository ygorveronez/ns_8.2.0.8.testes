using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]
    public class EmissaoDocumentosNFe : LongRunningProcessBase<EmissaoDocumentosNFe>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        /// <summary>
        /// São as cargas que devem ser emitidas após o Envio das NF-es;
        /// </summary>
        private void SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

            int segundos = -configuracao.TempoSegundosParaInicioEmissaoDocumentos;
            int tipoEnvio = configuracao.CodigoTipoEnvioEmissaoCTe;

            List<int> codigosCargas = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCarga.BuscarCodigosCargasAutorizadasEmissaoNaEtapaNFe(DateTime.Now.AddSeconds(segundos), limiteRegistros, Dominio.Enumeradores.LoteCalculoFrete.Padrao, configuracao.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, "DataInicioGeracaoCTes", "asc"));

            for (var i = 0; i < codigosCargas.Count; i++)
            {
                try
                {
                    Servicos.Log.GravarInfo("1 - Iniciando emissao documentos " + codigosCargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe");

                    serCarga.ValidarEmissaoDocumentosCarga(codigosCargas[i], unitOfWork, tipoServicoMultisoftware, webServiceConsultaCTe, tipoEnvio, true);
                    serHubCarga.InformarCargaAtualizada(codigosCargas[i], Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigosCargas[i]);

                    Servicos.Log.GravarInfo("15 - Finalizada emissao documentos " + codigosCargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe");
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();

                    Servicos.Log.TratarErro(excecao);

                    servicoOrquestradorFila.RegistroComFalha(codigosCargas[i], excecao.Message);
                }

                unitOfWork.FlushAndClear();
            }
        }

        #endregion Métodos Privados
    }
}