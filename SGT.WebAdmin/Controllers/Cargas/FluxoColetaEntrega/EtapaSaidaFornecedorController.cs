using SGTAdmin.Controllers;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaSaidaFornecedorController : BaseController
    {
		#region Construtores

		public EtapaSaidaFornecedorController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor repEtapaSaidaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor etapaSaidaFornecedor = repEtapaSaidaFornecedor.BuscarPorFluxoColetaEntrega(codigo);

                bool etapaLiberada = true;
                if (etapaSaidaFornecedor.FluxoColetaEntrega.EtapasOrdenadas[etapaSaidaFornecedor.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaFornecedor)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    etapaSaidaFornecedor.Codigo,
                    Carga = servicoCarga.ObterNumeroCarga(etapaSaidaFornecedor.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    DataInformada = !etapaLiberada ? etapaSaidaFornecedor.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    EtapaLiberada = etapaLiberada,
                    etapaSaidaFornecedor.Observacao
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

                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor repEtapaSaidaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor etapaSaidaFornecedor = repEtapaSaidaFornecedor.BuscarPorCodigo(codigo, true);

                if (etapaSaidaFornecedor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime.TryParseExact(Request.Params("DataInformada"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dataInformada);


                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaFornecedor;

                bool inserindoEtapa = true;
                if (etapaSaidaFornecedor.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual != etapa)
                    inserindoEtapa = false;


                DateTime? etapaTempoAnterior = null;
                if (inserindoEtapa)
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaSaidaFornecedor.FluxoColetaEntrega, unitOfWork);
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaSaidaFornecedor.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = DateTime.Now;
                if (!inserindoEtapa)
                    dataAtual = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaSaidaFornecedor.FluxoColetaEntrega, etapa, unitOfWork);

                if (etapaTempoAnterior != null && etapaTempoAnterior > dataInformada)
                    return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                if (dataInformada > dataAtual)
                    return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");

                etapaSaidaFornecedor.DataInformada = dataInformada;
                etapaSaidaFornecedor.Observacao = Request.Params("Observacao");

                repEtapaSaidaFornecedor.Atualizar(etapaSaidaFornecedor);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaSaidaFornecedor.FluxoColetaEntrega, null, "Informou Etapa Saida do Fornecedor", unitOfWork);

                if (inserindoEtapa)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaSaidaFornecedor.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                else
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaSaidaFornecedor.FluxoColetaEntrega, etapa, unitOfWork);

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
