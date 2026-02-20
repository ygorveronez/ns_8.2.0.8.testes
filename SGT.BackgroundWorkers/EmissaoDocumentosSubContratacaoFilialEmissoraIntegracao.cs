using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class EmissaoDocumentosSubContratacaoFilialEmissoraIntegracao : LongRunningProcessBase<EmissaoDocumentosSubContratacaoFilialEmissoraIntegracao>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        /// <summary>
        /// Emissão Documentos CT-es Filial Emissora
        /// </summary>
        private void SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            /// Se for MultiEmbarcador, agora pode ser que precise autorizar a emissão dos CT-es para Filial Emissora.
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao);

            List<int> codigosCargas = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCarga.BuscarCodigosCargasAutorizadasEmissaoCTesSubContratacaoFilialEmissora(limiteRegistros, Dominio.Enumeradores.LoteCalculoFrete.Integracao, "DataInicioGeracaoCTes", "asc"));

            for (var i = 0; i < codigosCargas.Count; i++)
            {
                try
                {
                    Servicos.Log.GravarInfo("1 - Iniciando emissao documentos " + codigosCargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao");

                    serCarga.ValidarEmissaoDocumentosCarga(codigosCargas[i], unitOfWork, tipoServicoMultisoftware, webServiceConsultaCTe, 1, false);
                    serHubCarga.InformarCargaAtualizada(codigosCargas[i], Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigosCargas[i]);

                    Servicos.Log.GravarInfo("15 - Finalizada emissao documentos " + codigosCargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao");
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