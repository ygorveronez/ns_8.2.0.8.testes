using SGTAdmin.Controllers;
using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaSaidaCDController : BaseController
    {
		#region Construtores

		public EtapaSaidaCDController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD repEtapaSaidaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD etapaSaidaCD = repEtapaSaidaCD.BuscarPorFluxoColetaEntrega(codigo);

                bool etapaLiberada = true;
                if (etapaSaidaCD.FluxoColetaEntrega.EtapasOrdenadas[etapaSaidaCD.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    etapaSaidaCD.Codigo,
                    CodigoPedido = etapaSaidaCD.FluxoColetaEntrega.Carga.Pedidos.FirstOrDefault().Pedido.Codigo,
                    Carga = servicoCarga.ObterNumeroCarga(etapaSaidaCD.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    DataInformada = !etapaLiberada ? etapaSaidaCD.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    EtapaLiberada = etapaLiberada,
                    etapaSaidaCD.Observacao
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

                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD repEtapaSaidaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD etapaSaidaCD = repEtapaSaidaCD.BuscarPorCodigo(codigo, true);

                if (etapaSaidaCD == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime.TryParseExact(Request.Params("DataInformada"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dataInformada);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.SaidaCD;

                bool inserindoEtapa = true;
                if (etapaSaidaCD.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual != etapa)
                    inserindoEtapa = false;
                
                DateTime? etapaTempoAnterior = null;
                if (inserindoEtapa)
                {
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    serCarga.AdicionarIntegracaoBuonny(etapaSaidaCD.FluxoColetaEntrega.Carga, unitOfWork);
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaSaidaCD.FluxoColetaEntrega, unitOfWork);
                }
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaSaidaCD.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = DateTime.Now;
                if (!inserindoEtapa)
                    dataAtual = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaSaidaCD.FluxoColetaEntrega, etapa, unitOfWork);

                if (etapaTempoAnterior != null && etapaTempoAnterior > dataInformada)
                    return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                if (dataInformada > dataAtual)
                    return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");

                etapaSaidaCD.DataInformada = dataInformada;
                etapaSaidaCD.Observacao = Request.Params("Observacao");

                repEtapaSaidaCD.Atualizar(etapaSaidaCD);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaSaidaCD.FluxoColetaEntrega, null, "Informou Etapa Saida CD", unitOfWork);
                
                if (inserindoEtapa)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaSaidaCD.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                else
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaSaidaCD.FluxoColetaEntrega, etapa, unitOfWork);

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
