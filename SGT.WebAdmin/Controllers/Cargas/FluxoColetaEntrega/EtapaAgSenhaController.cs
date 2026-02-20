using SGTAdmin.Controllers;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoColetaEntrega
{
    [CustomAuthorize("Cargas/FluxoColetaEntrega")]
    public class EtapaAgSenhaController : BaseController
    {
		#region Construtores

		public EtapaAgSenhaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha repEtapaAgSenha = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha(unitOfWork);

                int.TryParse(Request.Params("CodigoColetaEntrega"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha etapaAgSenha = repEtapaAgSenha.BuscarPorFluxoColetaEntrega(codigo);

                bool etapaLiberada = true;
                if (etapaAgSenha.FluxoColetaEntrega.EtapasOrdenadas[etapaAgSenha.FluxoColetaEntrega.EtapaAtual].EtapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha)
                    etapaLiberada = false;

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = etapaAgSenha.FluxoColetaEntrega.Carga.Pedidos.FirstOrDefault().Pedido;

                var retorno = new
                {
                    etapaAgSenha.Codigo,
                    Carga = servicoCarga.ObterNumeroCarga(etapaAgSenha.FluxoColetaEntrega.Carga, configuracaoEmbarcador),
                    DataInformada = !etapaLiberada ? etapaAgSenha.DataInformada.ToString("dd/MM/yyyy HH:mm") : "",
                    DataColeta = pedido.DataInicialColeta.HasValue ? pedido.DataInicialColeta.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    etapaAgSenha.Senha,
                    EtapaLiberada = etapaLiberada,
                    DataColetaLiberada = DataColetaLiberada(etapaAgSenha.FluxoColetaEntrega),
                    etapaAgSenha.Observacao
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
                Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha repEtapaAgSenha = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha etapaAgSenha = repEtapaAgSenha.BuscarPorCodigo(codigo, true);

                if (etapaAgSenha == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime dataInformada = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params("DataInformada"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out dataInformada);

                DateTime dataColeta = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params("DataColeta"), "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out dataColeta);
                if (DataColetaLiberada(etapaAgSenha.FluxoColetaEntrega))
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = etapaAgSenha.FluxoColetaEntrega.Carga.Pedidos.FirstOrDefault().Pedido;
                    dataColeta = pedido.DataInicialColeta ?? DateTime.MinValue;
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.AgSenha;

                bool inserindoEtapa = true;
                if (etapaAgSenha.FluxoColetaEntrega.EtapaFluxoColetaEntregaEtapaAtual != etapa)
                    inserindoEtapa = false;

                DateTime? etapaTempoAnterior = null;
                if (inserindoEtapa)
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaAgSenha.FluxoColetaEntrega, unitOfWork);
                else
                    etapaTempoAnterior = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataEtapaAnterior(etapaAgSenha.FluxoColetaEntrega, etapa, unitOfWork);

                DateTime? dataAtual = DateTime.Now;
                if (!inserindoEtapa)
                    dataAtual = Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ObterDataProximaEtapa(etapaAgSenha.FluxoColetaEntrega, etapa, unitOfWork);

                if (dataAtual == null)
                    dataAtual = DateTime.Now;

                string observacao = Request.Params("Observacao") ?? string.Empty;
                string senha = Request.Params("Senha") ?? string.Empty;

                etapaAgSenha.Observacao = observacao;
                etapaAgSenha.Senha = senha;

                if (dataInformada != DateTime.MinValue)
                {
                    if (etapaTempoAnterior != null && etapaTempoAnterior > dataInformada)
                        return new JsonpResult(false, true, "A data informada não pode ser inferior a data anterior.");

                    if (dataInformada > dataAtual)
                        return new JsonpResult(false, true, "A data informada não pode ser superior a data atual.");


                    if (string.IsNullOrWhiteSpace(senha))
                        return new JsonpResult(false, true, "Senha é obrigatória.");

                    etapaAgSenha.DataInformada = dataInformada;

                    unitOfWork.Start();

                    repEtapaAgSenha.Atualizar(etapaAgSenha);
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ReplicarSenha(etapaAgSenha.FluxoColetaEntrega, senha, unitOfWork);


                    if (dataColeta != DateTime.MinValue)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = etapaAgSenha.FluxoColetaEntrega.Carga.Pedidos.FirstOrDefault().Pedido;
                        pedido.DataInicialColeta = dataColeta;
                        repPedido.Atualizar(pedido);
                    }

                    if (inserindoEtapa)
                        Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(etapaAgSenha.FluxoColetaEntrega.Carga, etapa, unitOfWork);
                    else
                        Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AtualizarTempoEtapas(etapaAgSenha.FluxoColetaEntrega, etapa, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaAgSenha.FluxoColetaEntrega, null, "Informou Etapa Aguardando Senha", unitOfWork);

                    unitOfWork.CommitChanges();
                }
                else
                {
                    if (dataColeta != DateTime.MinValue)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = etapaAgSenha.FluxoColetaEntrega.Carga.Pedidos.FirstOrDefault().Pedido;
                        pedido.DataInicialColeta = dataColeta;
                        repPedido.Atualizar(pedido);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, etapaAgSenha.FluxoColetaEntrega, null, "Alterou a Data da Coleta", unitOfWork);
                    }
                    else
                    {
                        return new JsonpResult(false, true, "É obrigatório informar a data da senha.");
                    }
                }

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

        private bool DataColetaLiberada(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega)
        {
            // Etapa fica liberada até que chegue na etapa do cte
            int indexCTe = (from o in fluxoColetaEntrega.EtapasOrdenadas where o.EtapaFluxoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTe select o).FirstOrDefault()?.Ordem ?? -1;

            return indexCTe < 0 || fluxoColetaEntrega.EtapaAtual < indexCTe;
        }
    }
}
