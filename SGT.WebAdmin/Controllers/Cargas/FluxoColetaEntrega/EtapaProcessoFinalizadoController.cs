using SGTAdmin.Controllers;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaProcessoFinalizadoController : BaseController
    {
		#region Construtores

		public EtapaProcessoFinalizadoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado repEtapaProcessoFinalizado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado etapaProcessoFinalizado = repEtapaProcessoFinalizado.BuscarPorFluxoColetaEntrega(codigo);


                bool etapaLiberada = true;
                if (etapaProcessoFinalizado.FluxoColetaEntrega.EtapasOrdenadas[etapaProcessoFinalizado.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Finalizado || etapaProcessoFinalizado.EtapaLiberada)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    etapaProcessoFinalizado.Codigo,
                    Carga = servicoCarga.ObterNumeroCarga(etapaProcessoFinalizado.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    DataInformada = !etapaLiberada ? etapaProcessoFinalizado.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    EtapaLiberada = etapaLiberada,
                    etapaProcessoFinalizado.Observacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado repEtapaProcessoFinalizado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado etapaProcessoFinalizado = repEtapaProcessoFinalizado.BuscarPorCodigo(codigo, true);

                if (etapaProcessoFinalizado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime.TryParseExact(Request.Params("DataInformada"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dataInformada);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Finalizado;

                bool inserindoEtapa = true;
                if (etapaProcessoFinalizado.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual != etapa)
                    inserindoEtapa = false;


                DateTime? etapaTempoAnterior = null;
                if (inserindoEtapa)
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaProcessoFinalizado.FluxoColetaEntrega, unitOfWork);
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaProcessoFinalizado.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = DateTime.Now;
                if (!inserindoEtapa)
                    dataAtual = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaProcessoFinalizado.FluxoColetaEntrega, etapa, unitOfWork);

                if (etapaTempoAnterior != null && etapaTempoAnterior > dataInformada)
                    return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                if (dataInformada > dataAtual)
                    return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");

                etapaProcessoFinalizado.DataInformada = dataInformada;
                etapaProcessoFinalizado.Observacao = Request.Params("Observacao");

                repEtapaProcessoFinalizado.Atualizar(etapaProcessoFinalizado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaProcessoFinalizado.FluxoColetaEntrega, null, "Informou Etapa de Finalização", unitOfWork);
                
                if (inserindoEtapa)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaProcessoFinalizado.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                else
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaProcessoFinalizado.FluxoColetaEntrega, etapa, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
