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

    public class EmissaoDocumentosSubContratacaoFilialEmissora : LongRunningProcessBase<EmissaoDocumentosSubContratacaoFilialEmissora>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        /// <summary>
        /// Emissão Documentos CT-es Filial Emissora
        /// </summary>
        private void SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            /// Se for MultiEmbarcador, agora pode ser que precise autorizar a emissão dos CT-es para Filial Emissora.
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

            int tipoEnvio = configuracao.CodigoTipoEnvioEmissaoCTe;

            List<int> codigosCargas = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCarga.BuscarCodigosCargasAutorizadasEmissaoCTesSubContratacaoFilialEmissora(limiteRegistros, Dominio.Enumeradores.LoteCalculoFrete.Padrao, "DataInicioGeracaoCTes", "asc"));

            for (var i = 0; i < codigosCargas.Count; i++)
            {
                try
                {
                    Servicos.Log.GravarInfo("1 - Iniciando emissao documentos " + codigosCargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora");

                    serCarga.ValidarEmissaoDocumentosCarga(codigosCargas[i], unitOfWork, tipoServicoMultisoftware, webServiceConsultaCTe, tipoEnvio, true);
                    serHubCarga.InformarCargaAtualizada(codigosCargas[i], Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigosCargas[i]);

                    Servicos.Log.GravarInfo("15 - Finalizada emissao documentos " + codigosCargas[i].ToString(), "SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora");
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