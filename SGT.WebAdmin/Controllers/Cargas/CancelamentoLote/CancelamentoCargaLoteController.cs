using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SGT.WebAdmin.Controllers.Cargas.CancelamentoLote
{
    [CustomAuthorize("Cargas/CancelamentoCargaLote")]
    public class CancelamentoCargaLoteController : BaseController
    {
		#region Construtores

		public CancelamentoCargaLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasParaCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCargaLote filtrosPesquisa = ObterFiltrosPesquisa();

                if (filtrosPesquisa.CodigoTerminalOrigem > 0 && filtrosPesquisa.CodigoPedidoViagemDirecao > 0)
                {
                    if (filtrosPesquisa.TipoPropostaMultimodal == null || filtrosPesquisa.TipoPropostaMultimodal.Count == 0)
                        return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCargaLote.FavorInformeMenosTipoProposta);
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.NumeroCarga, "CodigoCargaEmbarcador", 7, Models.Grid.Align.left, true);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.NumeroBooking, "NumeroBooking", 7, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.NavioViagemDirecao, "PedidoViagemDirecao", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.TerminalOrigem, "TerminalOrigem", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.TerminalDestino, "TerminalDestino", 10, Models.Grid.Align.left, false);
                }
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.Remetente, "Remetente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.SituacaoCarga, "DescricaoSituacaoCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.Origem, "Origem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.Destinatario, "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.Destino, "Destino", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.TipoOperacao, "TipoOperacao", 10, Models.Grid.Align.left, false);

                if (!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.TipoDeCarga, "TipoCarga", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.ModeloVeicular, "ModeloVeicular", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.Cte, "Cte", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.EmpresaFilial, "EmpresaFilial", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.Veiculo, "Veiculo", 10, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.Motorista, "Motorista", 10, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.DataEmissaoCTe, "DataEmissaoCte", 10, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.DataCriacaoCarga, "DataCriacaoCarga", 10, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCargaLote.ValorFrete, "ValorFrete", 10, Models.Grid.Align.right, false, false);
                }

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "CancelamentoCargaLote/PesquisaCargasParaCancelamento", "grid-pesquisa-cargas-para-cancelamento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = repCarga.ConsultarCargasPendenteCancelamento(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repCarga.ContarConsultaCargasPendenteCancelamento(filtrosPesquisa));

                var lista = (from p in listaCargas
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoCargaEmbarcador,
                                 NumeroBooking = p.DadosSumarizados?.Bookings ?? "",
                                 PedidoViagemDirecao = p.PedidoViagemNavio?.Descricao ?? "",
                                 TerminalOrigem = p.TerminalOrigem?.Descricao ?? "",
                                 TerminalDestino = p.TerminalDestino?.Descricao ?? "",
                                 Remetente = p.DadosSumarizados?.Remetentes ?? "",
                                 DescricaoSituacaoCarga = (integracaoIntercab?.AtivarNovosFiltrosConsultaCarga ?? false) ? p.DescricaoSituacaoMercanteCarga : p.DescricaoSituacaoCarga,
                                 Origem = p.DadosSumarizados?.Origens ?? "",
                                 Destinatario = p.DadosSumarizados?.Destinatarios ?? "",
                                 Destino = p.DadosSumarizados?.Destinos ?? "",
                                 TipoOperacao = p.TipoOperacao?.Descricao ?? "",
                                 TipoCarga = p.TipoDeCarga?.Descricao ?? "",
                                 ModeloVeicular = p.ModeloVeicularCarga?.Descricao ?? "",
                                 Cte = p.NumerosCTes ?? "",
                                 EmpresaFilial = p.Empresa?.Descricao ?? "",
                                 Veiculo = p.Veiculo?.Descricao ?? "",
                                 Motorista = p.DadosSumarizados?.Motoristas ?? "",
                                 DataEmissaoCte = string.Join(", ", p.CargaCTes?.Select(o => o.CTe?.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? "").ToList()),
                                 DataCriacaoCarga = p.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") ?? "",
                                 p.ValorFrete
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarCancelamentoLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasCargaLote = ObterPermissoesPersonalizadas("Cargas/CancelamentoCargaLote");

                bool liberarCancelamentoCargaBloqueada = false;
                bool liberarCancelamentoCargaBloqueadaLote = false;
                bool duplicarCarga = !Request.GetBoolParam("NaoDuplicarCarga");

                if (!duplicarCarga && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_NaoDuplicarCarga))
                    throw new ControllerException($"Usuário sem permissão para marcar a opção \"{(ConfiguracaoEmbarcador.TrocarPreCargaPorCarga ? "Não Duplicar a Carga?" : "Não retornar para pré carga")}\".");

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_CancelarCargaBloqueada))
                    liberarCancelamentoCargaBloqueada = true;

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CancelamentoCargaLote_LiberarCancelamentoCargaBloqueada))
                    liberarCancelamentoCargaBloqueadaLote = true;

                int codigoJustificativa = Request.GetIntParam("Justificativa");
                long codigoJustificativaCancelamentoCarga = Request.GetLongParam("JustificativaCancelamentoCarga");

                string motivo = Request.Params("Motivo");
                bool consultarTodos = Request.GetBoolParam("ConsultarTodos");
                motivo = WebUtility.UrlDecode(motivo);
                motivo = motivo.Replace("  ", "");

                if ((string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 20) && !ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCargaLote.MotivoDevePossuirMaisVintecaracteres);

                if ((string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 15) && ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCargaLote.MotivoDevePossuirMaisQuinzeCaracteresSemDoisEspacosEntrePalavras);

                List<int> codigosCargas = new List<int>();
                codigosCargas = RetornaCodigosCarga(unitOfWork, consultarTodos);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repJustificativaCancelamentoCarga = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigos(codigosCargas);
                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = codigoJustificativa > 0 ? repJustificativa.BuscarPorCodigo(codigoJustificativa) : null;
                Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga justificativaCancelamentoCarga = codigoJustificativaCancelamentoCarga > 0 ? repJustificativaCancelamentoCarga.BuscarPorCodigo(codigoJustificativaCancelamentoCarga, false) : null;

                if (cargas?.Count > 0)
                {
                    string listaCargasBloqueadas = string.Empty;

                    cargas = cargas.OrderByDescending(o => o.CargaSVM).ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        if (!liberarCancelamentoCargaBloqueadaLote && carga.BloquearCancelamentoCarga)
                        {
                            if (!string.IsNullOrWhiteSpace(listaCargasBloqueadas))
                                listaCargasBloqueadas += " / ";

                            listaCargasBloqueadas += $"{carga.CodigoCargaEmbarcador}";
                            continue;
                        }

                        AdicionarCancelamento(carga, unitOfWork, liberarCancelamentoCargaBloqueada, duplicarCarga, motivo, justificativa, justificativaCancelamentoCarga);
                    }

                    if (cargas.Count == 1 && !string.IsNullOrWhiteSpace(listaCargasBloqueadas))
                         return new JsonpResult(false, true, $"A carga {listaCargasBloqueadas} está bloqueada e não foi enviada para cancelamento.");
                    else if (!string.IsNullOrWhiteSpace(listaCargasBloqueadas))
                        return new JsonpResult(true, true, $"As cargas {listaCargasBloqueadas} estão bloqueadas e não foram enviadas para cancelamento.");
                    else
                        return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCargaLote.NenhumaCargaSelecionada);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCargaLote.OcorreuFalhaGerarCancelamentoLote);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodosTipoPropostaMultimodal()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<dynamic> retorno = new List<dynamic>();

                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, text = Localization.Resources.Cargas.CancelamentoCargaLote.NoventaTresCargaFechada });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, text = Localization.Resources.Cargas.CancelamentoCargaLote.NovantaQuatroCargaFracionada });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder, text = Localization.Resources.Cargas.CancelamentoCargaLote.NovanetaCincoFeeder });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS, text = Localization.Resources.Cargas.CancelamentoCargaLote.NovantaSeisVAS });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder, text = Localization.Resources.Cargas.CancelamentoCargaLote.EmbarqueCertoFeeder });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem, text = Localization.Resources.Cargas.CancelamentoCargaLote.EmbarqueCertoCabotagem });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem, text = Localization.Resources.Cargas.CancelamentoCargaLote.NoShowCabotagem });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade, text = Localization.Resources.Cargas.CancelamentoCargaLote.FaturamentoContabilidade });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem, text = Localization.Resources.Cargas.CancelamentoCargaLote.DemurrageCabotagem });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem, text = Localization.Resources.Cargas.CancelamentoCargaLote.DetentionCabotagem });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<int> RetornaCodigosCarga(Repositorio.UnitOfWork unidadeDeTrabalho, bool consultarTodos)
        {
            List<int> listaCodigos = new List<int>();
            if (!consultarTodos && !string.IsNullOrWhiteSpace(Request.Params("ListaCargas")))
            {
                dynamic listaCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCargas"));
                if (listaCarga != null)
                {
                    foreach (var carga in listaCarga)
                    {
                        listaCodigos.Add(int.Parse((string)carga.Codigo));
                    }
                }
                else
                    listaCodigos = RetornaTodosCodigosCarga(unidadeDeTrabalho);
            }
            else
            {
                listaCodigos = RetornaTodosCodigosCarga(unidadeDeTrabalho);
            }
            return listaCodigos;
        }

        private List<int> RetornaTodosCodigosCarga(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCargaLote filtrosPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            List<int> listaCodigos = repCarga.ConsultarCodigosCargasPendenteCancelamento(filtrosPesquisa);

            return listaCodigos;
        }

        private void AdicionarCancelamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, bool liberarCancelamentoCargaBloqueada, bool duplicarCarga, string motivo, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga justificativaCancelamentoCarga)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);
            Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfigEmbarcador.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga tipoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento;

            string msgRejeicaoCancelamento = "";

            if (duplicarCarga)
            {
                if (carga.CargaSVM || carga.CargaTakeOrPay)
                    duplicarCarga = false;
                else if (!configuracaoEmbarcador.TrocarPreCargaPorCarga)
                {
                    if (carga.CargaDePreCarga)
                        msgRejeicaoCancelamento = "Não é possível retornar uma pré carga para pré carga.";

                    if (!carga.CargaDePreCargaFechada)
                        msgRejeicaoCancelamento = "Não é possível retornar uma carga que nunca foi pré carga para pré carga.";

                    if (carga.CargaDePreCargaEmFechamento)
                        msgRejeicaoCancelamento = "Não é possível retornar uma carga durante o processo de fechamento para pré carga.";
                }
                else
                {
                    bool permitirDuplicarCarga = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) || configuracaoEmbarcador.SempreDuplicarCargaCancelada;

                    if (!permitirDuplicarCarga)
                        duplicarCarga = false;
                }
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && duplicarCarga && repCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo) && repCargaDocumentoParaEmissaoNFSManual.ExistePorCarga(carga.Codigo))
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.NaoPossivelEfetuarCancelamentoAnulacaoDuplicandoUmaCargaImportadaEmbarcadorTenhaDocumentosParaemissaoNFSManualVinculadosNecessarioCancelarCargaDuplicarGerarRegistrosManualmentePosteriormente;

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repCargaCTe.ExisteCTeComTituloPorCarga(carga.Codigo))
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.NaoPossivelSolicitarCancelamentoCargaPoisExistePrevisaoPagamentoParaCTes;

            if (carga.CargaMDFes.Any(o => o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento))
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.NaoPossivelAdicionarCancelamentoAnulacaoEstaCargaMDFeEncerramentoCancelamentoParaMesma;

            if (!liberarCancelamentoCargaBloqueada && carga.BloquearCancelamentoCarga)
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.CargaEncontraBloqueadaDevidoTerGeradoDocumentacaoFavorSoliciteLiberacaoOperadorResponsavel;

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.CargaSelecionadaEstaCancelamentoCancelada;

            if (Servicos.Embarcador.Carga.Carga.IsCargaBloqueada(carga, unidadeTrabalho))
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.CargaBloqueada;

            if (tipoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento && carga.Ocorrencias.Any(o => o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada && o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada && o.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao))
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExistemOcorrenciasParaCargaNaoEstaoCanceladasNaoSendoPossivelRealizarCancelamentoCarga;

            int quantidade = repCargaDocumentoParaEmissaoNFSManual.ContarPorCargaVinculadasNFSManual(carga.Codigo);
            if (quantidade > 0)
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExitemNFSsManuaisParaEstaCargaNãoEstaoCanceladasNaoSendoPossivelRealizarCancelamentoAnulacaoCarga;

            if (tipoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento)
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in carga.CargaMDFes.ToList())
                {
                    if (cargaMDFe.MDFe != null && cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorMDFe(cargaMDFe.MDFe.Codigo);
                        if (cargaMDFeManual != null)
                            msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExistemMDFesManuaisParaEstaCargaNaoEstaoCanceladasNaoSendoPossivelRealizarCancelamentoCarga;
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualCarga = repCargaMDFeManual.BuscarPorCarga(carga.Codigo);
                if (cargaMDFeManualCarga != null)
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExistemMDFesManuaisParaEstaCargaNaoEstaoCanceladasNaoSendoPossivelRealizarCancelamentoCarga;

                if (repCargaIntegracaoValePedagio.VerificarSeExisteValePedagioPorStatus(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Encerrada))
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExistemValesPedagiosEncerradosParaCargaNaoSendoPossivelSolicitarCancelamento;

                if (repPagamentoProvedorCarga.VerificarSeExisteCargaEmPagamentoProvedor(carga.Codigo))
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExistemPagamentoProvedorComCargaEmAberto;                

                //if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                //{
                //    List<string> cargasVinculadas = repPedidoCTeParaSubContratacao.ContemCargaSubcontratada(carga.Codigo);
                //    if (cargasVinculadas != null && cargasVinculadas.Count > 0)
                //        msgRejeicaoCancelamento += " / Existem cargas vinculadas para esta carga que não estão canceladas, não sendo possível o cancelamento da carga. Número(s): " + string.Join(", ", cargasVinculadas);
                //}

                if (!carga.CargaTransbordo && repCargaCTe.ExisteCTeVinculadoATransbordo(carga.Codigo))
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExistemCTesDestaCargaVinculadosUmaCargatransbordoNaoSendoPossivelCancelarMesmaCanceleCargaTransbordoPrimeiro;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);

            if (cargaCancelamento != null)
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.ExisteUmaSolicitacaoCancelamentoParaEstaCarga;

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unidadeTrabalho);

            if (!(serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware) || configuracaoEmbarcador.PermitirCancelamentoTotalCarga || (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && operadorLogistica.SupervisorLogistica)))
                msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.PossivelSolicitarCancelamentoUmaCargaAposLiberaLaParaFaturamentoNecessarioSoliciteResponsavelFaturamentoFacaCancelamentoMesmaErp;
            else
            {
                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && carga.ExigeNotaFiscalParaCalcularFrete && !carga.TipoOperacao.PermiteImportarDocumentosManualmente && !operadorLogistica.SupervisorLogistica)
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.PossivelSolicitarCancelamentoUmaCargaAposLiberaLaParaFaturamentoNecessarioSoliciteResponsavelFaturamentoFacaCancelamentoMesmaErp;
            }

            if (carga.FreteDeTerceiro && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (carga.CargaCTes.Any(o => o.CIOTs.Any(c => c.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto || c.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem)))
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.CargaPossuiCiotAbertoNaoSendoPossivelCancelamentoCanceleCiotParaEfetuarCancelamentoCarga;

                if (carga.CargaCTes.Any(o => o.CIOTs.Any(c => c.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)))
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.CargaPossuiUmCiotEncerradoNãoSendoPossivelCcancelamento;
            }

            if (carga.EmpresaFilialEmissora != null)
            {
                if (carga.Pedidos.Any(obj => obj.CargaPedidoProximoTrecho != null
                && (obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento
                )))
                {
                    msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.CargaPossuiUmRedespachoAutorizadoCancelarEssaCargaNecessarioCancelarPrimeiroCargasRedespacho;
                }
                else
                {
                    //se faltam apenas 5 minutos para emissão não permite cancelar sem cancelar o proximo trecho
                    if (carga.Pedidos.Any(obj => obj.CargaPedidoProximoTrecho != null && obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                        && (obj.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe.HasValue && obj.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe.Value >= DateTime.Now.AddMinutes(-5))))
                        msgRejeicaoCancelamento += Localization.Resources.Cargas.CancelamentoCargaLote.RedespachoDestaViagemSeraEmitidoDentroMinutosDestaFormaNecessarioCancelarProximoTrechoPrimeiroParaCancelarEssaViagem;
                }
            }

            msgRejeicaoCancelamento = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unidadeTrabalho).ValidarPermissaoCancelarCarga(carga, true);

            unidadeTrabalho.Start();

            cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento
            {
                Carga = carga,
                DataCancelamento = DateTime.Now,
                DuplicarCarga = duplicarCarga,
                MotivoCancelamento = motivo,
                Tipo = tipoCancelamento,
                Usuario = Usuario,
                EnviouAverbacoesCTesParaCancelamento = true,
                SituacaoCargaNoCancelamento = carga.SituacaoCarga,
                CancelarDocumentosEmitidosNoEmbarcador = true,
                Situacao = string.IsNullOrWhiteSpace(msgRejeicaoCancelamento) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento,
                MensagemRejeicaoCancelamento = msgRejeicaoCancelamento
            };

            cargaCancelamento.Justificativa = justificativa;
            cargaCancelamento.JustificativaCancelamentoCarga = justificativaCancelamentoCarga;

            if (configuracaoEmbarcador.LiberarPedidosParaMontagemCargaCancelada && cargaCancelamento.Carga.Carregamento != null && !cargaCancelamento.Carga.CargaDePreCargaFechada && !carga.CargaDePreCargaEmFechamento)
                cargaCancelamento.LiberarPedidosParaMontagemCarga = !cargaCancelamento.DuplicarCarga;

            repCargaCancelamento.Inserir(cargaCancelamento);

            GerarLogCancelamento(cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCargaCancelamento.Emissao, unidadeTrabalho);
            GerarIntegracoesCancelamento(ref cargaCancelamento, unidadeTrabalho);
            servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, TipoServicoMultisoftware);

            repCargaCancelamento.Atualizar(cargaCancelamento);

            string mensagemAuditoriaNaoDuplicarCarga = cargaCancelamento.DuplicarCarga ? string.Empty : $" ({Localization.Resources.Cargas.CancelamentoCargaLote.MarcouOParametroNaoDuplicarACarga})";
            string mensagemAuditoria = $"{Localization.Resources.Cargas.CancelamentoCargaLote.AdicionouCancelamentoLoteCarga}{mensagemAuditoriaNaoDuplicarCarga}";

            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, mensagemAuditoria, unidadeTrabalho);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento.Carga, null, mensagemAuditoria, unidadeTrabalho);
            unidadeTrabalho.CommitChanges();

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaVinculadaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(cargaCancelamento.Carga.Codigo, cargaCancelamento.Carga.Empresa?.Codigo ?? 0);
            if (cargaVinculadaJanelaCarregamentoTransportador != null && cargaVinculadaJanelaCarregamentoTransportador.UsuarioResponsavel != null && cargaVinculadaJanelaCarregamentoTransportador.CargaJanelaCarregamento != null)
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, this.Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);
                string mensagem = Localization.Resources.Cargas.CancelamentoCargaLote.SolicitadoCancelamentoCarga + cargaCancelamento.Carga.CodigoCargaEmbarcador;
                serNotificacao.GerarNotificacao(cargaVinculadaJanelaCarregamentoTransportador.UsuarioResponsavel, cargaVinculadaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo, "Logistica/JanelaCarregamento", mensagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.janelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unidadeTrabalho);
            }

        }

        private void GerarLogCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCargaCancelamento tipo, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoLog repCargaCancelamentoLog = new Repositorio.Embarcador.Cargas.CargaCancelamentoLog(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog log = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog()
            {
                Acao = tipo,
                CargaCancelamento = cargaCancelamento,
                Data = DateTime.Now,
                Usuario = Usuario
            };

            repCargaCancelamentoLog.Inserir(log);
        }

        private void GerarIntegracoesCancelamento(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento.AdicionarIntegracoesCarga(cargaCancelamento, unidadeTrabalho, TipoServicoMultisoftware))
                return;

            if (cargaCancelamento.Carga.EmpresaFilialEmissora != null && cargaCancelamento.Carga.EmpresaFilialEmissora.TransportadorLayoutsEDI.Any(obj => obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO))
                cargaCancelamento.GerouIntegracao = true;
            else if (cargaCancelamento.Carga.Empresa != null && cargaCancelamento.Carga.Empresa.TransportadorLayoutsEDI.Any(obj => obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO))
                cargaCancelamento.GerouIntegracao = true;
            else
                cargaCancelamento.GerouIntegracao = false;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCargaLote ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCargaLote filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCargaLote()
            {
                DataInicioEmissao = Request.GetDateTimeParam("DataInicioEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                CnpjTomador = Request.GetDoubleParam("Tomador"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao"),
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                TipoPropostaMultimodal = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoPropostaMultimodal"),
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("SituacaoCarga"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                CnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CnpjRemetente = Request.GetDoubleParam("Remetente"),
                CodigosCarga = Request.GetListParam<int>("NumeroCarga"),
                DataCriacaoCarga = Request.GetDateTimeParam("DataCriacaoCarga")
            };

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal || filtrosPesquisa.Situacoes.Count == 0)
            {
                filtrosPesquisa.Situacoes = new List<SituacaoCarga>
                    {
                        SituacaoCarga.NaLogistica,
                        SituacaoCarga.Nova,
                        SituacaoCarga.CalculoFrete,
                        SituacaoCarga.AgTransportador,
                        SituacaoCarga.AgNFe,
                        SituacaoCarga.PendeciaDocumentos,
                        SituacaoCarga.AgImpressaoDocumentos,
                        SituacaoCarga.ProntoTransporte,
                        SituacaoCarga.EmTransporte,
                        SituacaoCarga.LiberadoPagamento,
                        SituacaoCarga.Encerrada,
                        SituacaoCarga.AgIntegracao,
                        SituacaoCarga.EmTransbordo
                    };
            }

            return filtrosPesquisa;
        }

        #endregion
    }
}
