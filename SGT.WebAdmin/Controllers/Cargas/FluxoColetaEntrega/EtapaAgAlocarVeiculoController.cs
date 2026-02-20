using SGTAdmin.Controllers;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaAgAlocarVeiculoController : BaseController
    {
		#region Construtores

		public EtapaAgAlocarVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo repEtapaAgAlocarVeiculo = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo etapaAgAlocarVeiculo = repEtapaAgAlocarVeiculo.BuscarPorFluxoColetaEntrega(codigo);

                bool etapaLiberada = true;
                if (etapaAgAlocarVeiculo.FluxoColetaEntrega.EtapasOrdenadas[etapaAgAlocarVeiculo.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.PendenciaAlocarVeiculo)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    etapaAgAlocarVeiculo.Codigo,
                    Carga = servicoCarga.ObterNumeroCarga(etapaAgAlocarVeiculo.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    DataInformada = !etapaLiberada ? etapaAgAlocarVeiculo.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    EtapaLiberada = etapaLiberada,
                    etapaAgAlocarVeiculo.Observacao
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
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo repEtapaAgAlocarVeiculo = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo etapaAgAlocarVeiculo = repEtapaAgAlocarVeiculo.BuscarPorCodigo(codigo, true);

                if (etapaAgAlocarVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime.TryParseExact(Request.Params("DataInformada"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dataInformada);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.PendenciaAlocarVeiculo;

                bool inserindoEtapa = true;
                if (etapaAgAlocarVeiculo.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual != etapa)
                    inserindoEtapa = false;


                DateTime? etapaTempoAnterior = null;
                if (inserindoEtapa)
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaAgAlocarVeiculo.FluxoColetaEntrega, unitOfWork);
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaAgAlocarVeiculo.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = DateTime.Now;
                if (!inserindoEtapa)
                    dataAtual = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaAgAlocarVeiculo.FluxoColetaEntrega, etapa, unitOfWork);

                if (dataAtual == null)
                    dataAtual = DateTime.Now;


                etapaAgAlocarVeiculo.DataInformada = dataInformada;
                etapaAgAlocarVeiculo.Observacao = Request.Params("Observacao");

                if (etapaTempoAnterior != null && etapaTempoAnterior > dataInformada)
                    return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                if (dataInformada > dataAtual)
                    return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");

                unitOfWork.Start();

                repEtapaAgAlocarVeiculo.Atualizar(etapaAgAlocarVeiculo);
                if (inserindoEtapa)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaAgAlocarVeiculo.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                else
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaAgAlocarVeiculo.FluxoColetaEntrega, etapa, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaAgAlocarVeiculo.FluxoColetaEntrega, null, "Informou Etapa Aguardando alocação de Veiculo", unitOfWork);

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
