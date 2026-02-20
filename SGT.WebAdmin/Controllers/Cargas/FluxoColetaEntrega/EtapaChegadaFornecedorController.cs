using SGTAdmin.Controllers;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaChegadaFornecedorController : BaseController
    {
		#region Construtores

		public EtapaChegadaFornecedorController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor repEtapaChegadaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);


                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor etapaChegadaFornecedor = repEtapaChegadaFornecedor.BuscarPorFluxoColetaEntrega(codigo);
                
                bool etapaLiberada = true;
                if (etapaChegadaFornecedor.FluxoColetaEntrega.EtapasOrdenadas[etapaChegadaFornecedor.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    etapaChegadaFornecedor.Codigo,
                    Carga = servicoCarga.ObterNumeroCarga(etapaChegadaFornecedor.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    DataInformada = !etapaLiberada ? etapaChegadaFornecedor.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    EtapaLiberada = etapaLiberada,
                    etapaChegadaFornecedor.Observacao
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

                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor repEtapaChegadaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor etapaChegadaFornecedor = repEtapaChegadaFornecedor.BuscarPorCodigo(codigo, true);

                if (etapaChegadaFornecedor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime.TryParseExact(Request.Params("DataInformada"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dataInformada);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.ChegadaFornecedor;

                bool inserindoEtapa = true;
                if (etapaChegadaFornecedor.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual != etapa)
                    inserindoEtapa = false;


                DateTime? etapaTempoAnterior = null;
                if (inserindoEtapa)
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaChegadaFornecedor.FluxoColetaEntrega, unitOfWork);
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaChegadaFornecedor.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = DateTime.Now;
                if (!inserindoEtapa)
                    dataAtual = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaChegadaFornecedor.FluxoColetaEntrega, etapa, unitOfWork);

                if (etapaTempoAnterior != null && etapaTempoAnterior > dataInformada)
                    return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                if (dataInformada > dataAtual)
                    return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");

                etapaChegadaFornecedor.DataInformada = dataInformada;
                etapaChegadaFornecedor.Observacao = Request.Params("Observacao");

                repEtapaChegadaFornecedor.Atualizar(etapaChegadaFornecedor);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaChegadaFornecedor.FluxoColetaEntrega, null, "Informou Etapa Chegada no Fornecedor", unitOfWork);

                if (inserindoEtapa)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaChegadaFornecedor.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                else
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaChegadaFornecedor.FluxoColetaEntrega, etapa, unitOfWork);

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
