using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CentroCarregamento")]
    public class CentroCarregamentoController : BaseController
    {
        #region Construtores

        public CentroCarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("LimiteCarregamentos", false);
                grid.AdicionarCabecalho("TipoCapacidadeCarregamentoPorPeso", false);
                grid.AdicionarCabecalho("ExibirVisualizacaoDosTiposDeOperacao", false);
                grid.AdicionarCabecalho("TipoCapacidadeCarregamento", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CentroCarregamento.Filial, "Filial", 25, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> listaCentroCarregamento = repCentroCarregamento.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repCentroCarregamento.ContarConsulta(filtrosPesquisa));

                var lista = (
                    from centroCarregamento in listaCentroCarregamento
                    select new
                    {
                        centroCarregamento.Codigo,
                        CodigoFilial = centroCarregamento.Filial.Codigo,
                        centroCarregamento.TipoCapacidadeCarregamentoPorPeso,
                        centroCarregamento.ExibirVisualizacaoDosTiposDeOperacao,
                        centroCarregamento.Descricao,
                        Filial = centroCarregamento.Filial.Descricao,
                        centroCarregamento.DescricaoAtivo,
                        centroCarregamento.LimiteCarregamentos,
                        centroCarregamento.TipoCapacidadeCarregamento,
                    }
                ).ToList();

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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPeriodoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoFilial = Request.GetIntParam("Filial");
                int codigoTipoCarga = Request.GetIntParam("TipoCarga");
                DateTime? dataCarregamento = Request.GetNullableDateTimeParam("Data");
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = null;

                if (codigoCentroCarregamento > 0)
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);
                else
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(codigoTipoCarga, codigoFilial, ativo: true);

                if ((centroCarregamento == null) || !dataCarregamento.HasValue)
                    periodosCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();
                else
                {
                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoDia = (from o in centroCarregamento.ExcecoesCapacidadeCarregamento where o.Data == dataCarregamento.Value.Date select o).FirstOrDefault();

                    if (excecaoDia != null)
                        periodosCarregamento = (from o in excecaoDia.PeriodosCarregamento orderby o.HoraInicio select o).ToList();
                    else
                    {
                        DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataCarregamento.Value);
                        List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodos = repPeriodoCarregamento.BuscarPorCentroCarregamento(centroCarregamento.Codigo);
                        periodosCarregamento = (from o in periodos where o.Dia == diaSemana orderby o.HoraInicio select o).ToList();
                    }
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("DataCarregamento", false);
                grid.AdicionarCabecalho("DataHoraInicio", false);
                grid.AdicionarCabecalho("HoraInicio", false);
                grid.AdicionarCabecalho("HoraTermino", false);
                grid.AdicionarCabecalho("InicioCarregamento", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.CarregamentoSimultaneo, "CapacidadeCarregamentoSimultaneo", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.Tolerancia, "ToleranciaExcessoTempo", 15, Models.Grid.Align.center, false);

                if (centroCarregamento?.TipoCapacidadeCarregamentoPorPeso == TipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.CapacidadeTotal, "CapacidadeCarregamento", 18, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.CapacidadeDisponivel, "CapacidadeDisponivel", 18, Models.Grid.Align.center, false);
                }

                var periodosCarregamentoRetornar = (
                    from periodoCarregamento in periodosCarregamento
                    orderby periodoCarregamento.HoraInicio
                    select ObterPeriodoCarregamento(periodoCarregamento, centroCarregamento, dataCarregamento.Value, configuracaoEmbarcador, unitOfWork)
                ).ToList();

                grid.AdicionaRows(periodosCarregamentoRetornar);
                grid.setarQuantidadeTotal(periodosCarregamento.Count());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> listaCentros = repCentroCarregamento.BuscarTodos();
                var retorno = (from obj in listaCentros select new { value = obj.Codigo, text = obj.Descricao }).ToList();

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                if (Request.GetIntParam("TempoAguardarAprovacaoTransportador") < Request.GetIntParam("TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente"))
                    return new JsonpResult(false, "O tempo de aguardar aprovação do transportador não pode ser menor que o tempo de confirmação do transportador.");

                unidadeDeTrabalho.Start();

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamento();

                PreencherCentroCarregamento(centroCarregamento, unidadeDeTrabalho);

                repCentroCarregamento.Inserir(centroCarregamento, Auditado);

                SetarVeiculos(centroCarregamento, unidadeDeTrabalho);
                SetarTransportadoresAutorizadosLiberarFaturamento(ref centroCarregamento, unidadeDeTrabalho);
                SetarTiposCarga(centroCarregamento, unidadeDeTrabalho);
                SetarTiposCargaBloquearLiberacaoAutomaticaParaTransportadoras(centroCarregamento, unidadeDeTrabalho);
                SalvarTemposCarregamento(centroCarregamento, null, unidadeDeTrabalho);
                SalvarEmails(centroCarregamento, null, unidadeDeTrabalho);
                SalvarPeriodosCarregamento(centroCarregamento, null, unidadeDeTrabalho);
                SalvarConfiguracaoLances(centroCarregamento, null, unidadeDeTrabalho);
                SalvarOfertaCarga(centroCarregamento, null, unidadeDeTrabalho);
                SalvarPrevisoesCarregamento(centroCarregamento, null, unidadeDeTrabalho);
                SalvarDisponibilidadeFrota(centroCarregamento, null, unidadeDeTrabalho);
                SalvarLimiteCarregamento(centroCarregamento, null, unidadeDeTrabalho);
                AtualizarControlesExpedicao(centroCarregamento, null, unidadeDeTrabalho);
                AtualizarDocas(centroCarregamento, unidadeDeTrabalho);
                AtualizarAcoesManobra(centroCarregamento, unidadeDeTrabalho);
                AtualizarTransportadores(centroCarregamento, unidadeDeTrabalho);
                AtualizarTransportadoresTerceiros(centroCarregamento, unidadeDeTrabalho);
                SalvarNotificacoes(centroCarregamento, unidadeDeTrabalho);
                AtualizarTipoOperacao(centroCarregamento, unidadeDeTrabalho);
                AtualizarAutomatizacaoNaoComparecimento(centroCarregamento, null, unidadeDeTrabalho);
                SalvarProdutividade(centroCarregamento, unidadeDeTrabalho);
                SalvarPunicao(centroCarregamento, unidadeDeTrabalho);
                SalvarConfiguracaoPadrao(centroCarregamento, unidadeDeTrabalho);

                repCentroCarregamento.Atualizar(centroCarregamento);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorCodigo(codigo, true);

                if (Request.GetIntParam("TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente") < Request.GetIntParam("TempoAguardarAprovacaoTransportador"))
                    return new JsonpResult(false, "O tempo de aguardar confirmação do transportador não pode ser menor que o tempo de aprovação do transportador.");

                if (centroCarregamento == null)
                    return new JsonpResult(true, false, Localization.Resources.Logistica.CentroCarregamento.CentroDeCarregamentoNaoEncontrado);

                unidadeDeTrabalho.Start();

                PreencherCentroCarregamento(centroCarregamento, unidadeDeTrabalho);

                repCentroCarregamento.Atualizar(centroCarregamento);

                SetarVeiculos(centroCarregamento, unidadeDeTrabalho);
                SetarTransportadoresAutorizadosLiberarFaturamento(ref centroCarregamento, unidadeDeTrabalho);
                SetarTiposCarga(centroCarregamento, unidadeDeTrabalho);
                SetarTiposCargaBloquearLiberacaoAutomaticaParaTransportadoras(centroCarregamento, unidadeDeTrabalho);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repCentroCarregamento.Atualizar(centroCarregamento, Auditado);

                SalvarTemposCarregamento(centroCarregamento, historico, unidadeDeTrabalho);
                SalvarEmails(centroCarregamento, historico, unidadeDeTrabalho);
                SalvarPeriodosCarregamento(centroCarregamento, historico, unidadeDeTrabalho);
                SalvarConfiguracaoLances(centroCarregamento, null, unidadeDeTrabalho);
                SalvarOfertaCarga(centroCarregamento, historico, unidadeDeTrabalho);
                SalvarPrevisoesCarregamento(centroCarregamento, historico, unidadeDeTrabalho);
                SalvarDisponibilidadeFrota(centroCarregamento, historico, unidadeDeTrabalho);
                SalvarLimiteCarregamento(centroCarregamento, historico, unidadeDeTrabalho);
                AtualizarControlesExpedicao(centroCarregamento, historico, unidadeDeTrabalho);
                AtualizarDocas(centroCarregamento, unidadeDeTrabalho);
                AtualizarAcoesManobra(centroCarregamento, unidadeDeTrabalho);
                AtualizarTransportadores(centroCarregamento, unidadeDeTrabalho);
                AtualizarTransportadoresTerceiros(centroCarregamento, unidadeDeTrabalho);
                SalvarNotificacoes(centroCarregamento, unidadeDeTrabalho);
                AtualizarTipoOperacao(centroCarregamento, unidadeDeTrabalho);
                AtualizarAutomatizacaoNaoComparecimento(centroCarregamento, historico, unidadeDeTrabalho);
                SalvarProdutividade(centroCarregamento, unidadeDeTrabalho);
                SalvarPunicao(centroCarregamento, unidadeDeTrabalho);
                SalvarConfiguracaoPadrao(centroCarregamento, unidadeDeTrabalho);

                repCentroCarregamento.Atualizar(centroCarregamento);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCargaEPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.CentroCarregamento.NaoFoiPossivelEncontrarCarga);

                int codigoPedido = Request.GetIntParam("Pedido");
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);

                if (pedido == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.CentroCarregamento.NaoFoiPossivelEncontrarPedido);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

                if ((carga.Filial != null) && (pedido.Filial != null) && (carga.Filial.Codigo == pedido.Filial.Codigo) && !string.IsNullOrWhiteSpace(pedido.RefEXPTransferencia))
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferenciado = repositorioCargaPedido.BuscarPorCargaENumeroEXP(carga.Codigo, pedido.RefEXPTransferencia);

                    if (cargaPedidoReferenciado != null)
                    {
                        Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                        filial = repositorioFilial.BuscarPorCNPJ(pedido.Destinatario?.CPF_CNPJ_SemFormato ?? "");
                    }
                }
                else
                    filial = pedido.Filial;

                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(carga.TipoDeCarga?.Codigo ?? 0, filial?.Codigo ?? 0, ativo: true, carga);

                return new JsonpResult(new
                {
                    CentroCarregamento = new { Codigo = centroCarregamento?.Codigo ?? 0, Descricao = centroCarregamento?.Descricao ?? "" }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.CentroCarregamento.OcorreuUmaFalhaAoBuscarDadosPorPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                var repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);
                var repositorioLancesCarregamento = new Repositorio.Embarcador.Logistica.LancesCarregamento(unidadeDeTrabalho);
                var repositorioOfertaCarga = new Repositorio.Embarcador.Logistica.CentroCarregamentoOfertaCarga(unidadeDeTrabalho);
                var repositorioCentroCarregamentoTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(unidadeDeTrabalho);
                var repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unidadeDeTrabalho);
                var repositorioCentroCarregamentoProdutividade = new Repositorio.Embarcador.Logistica.CentroCarregamentoProdutividade(unidadeDeTrabalho);
                var repositorioCentroCarregamentoPunicao = new Repositorio.Embarcador.Logistica.CentroCarregamentoPunicao(unidadeDeTrabalho);

                var centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigo);

                if (centroCarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var listaLances = repositorioLancesCarregamento.BuscarPorCentroCarregamento(centroCarregamento.Codigo);
                var periodos = repPeriodoCarregamento.BuscarPorCentroCarregamento(centroCarregamento.Codigo);
                var ofertasCarga = repositorioOfertaCarga.BuscarPorCentroCarregamento(centroCarregamento.Codigo);
                var centroCarregamentoProdutividades = repositorioCentroCarregamentoProdutividade.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo);
                var centroCarregamentoPunicao = repositorioCentroCarregamentoPunicao.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo);

                var retorno = new
                {
                    Codigo = !duplicar ? centroCarregamento.Codigo : 0,
                    centroCarregamento.Descricao,
                    centroCarregamento.Ativo,
                    centroCarregamento.Observacao,
                    centroCarregamento.NumeroDocas,
                    centroCarregamento.IndicarTemposVeiculos,
                    centroCarregamento.TempoBloqueioEscolhaTransportador,
                    centroCarregamento.TempoEncostaDoca,
                    centroCarregamento.TempoToleranciaPedidoRoteirizar,
                    centroCarregamento.GerarCarregamentosAlemDaDispFrota,
                    centroCarregamento.AgruparPedidosMesmoDestinatario,
                    centroCarregamento.GerarCarregamentoDoisDias,
                    centroCarregamento.CarregamentoTempoMaximoRota,
                    centroCarregamento.QuantidadeMaximaEntregasRoteirizar,
                    centroCarregamento.QuantidadeMaximaPedidosSessaoRoteirizar,
                    centroCarregamento.UtilizarDispFrotaCentroDescCliente,
                    centroCarregamento.ConsiderarTempoDeslocamentoPrimeiraEntrega,
                    centroCarregamento.MontagemCarregamentoPedidoProduto,
                    centroCarregamento.MontagemCarregamentoPedidoIntegral,
                    centroCarregamento.TipoRoteirizacaoColetaEntrega,
                    centroCarregamento.TipoMontagemCarregamentoVRP,
                    centroCarregamento.SimuladorFreteCriterioSelecaoTransportador,
                    centroCarregamento.TipoOcupacaoMontagemCarregamentoVRP,
                    centroCarregamento.TipoResumoCarregamento,
                    centroCarregamento.NivelQuebraProdutoRoteirizar,
                    centroCarregamento.TempoMinutosEscolhaAutomaticaCotacao,
                    centroCarregamento.PercentualMaximoDiferencaValorCotacao,
                    centroCarregamento.PercentualMinimoDiferencaValorCotacao,
                    centroCarregamento.PercentualToleranciaPesoCarregamento,
                    centroCarregamento.LimiteRecorrencia,
                    centroCarregamento.PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao,
                    centroCarregamento.TempoEmMinutosLiberacao,
                    CapacidadeCarregamentoSegunda = centroCarregamento.CapacidadeCarregamentoSegunda > 0 ? centroCarregamento.CapacidadeCarregamentoSegunda.ToString("n0") : "",
                    CapacidadeCarregamentoTerca = centroCarregamento.CapacidadeCarregamentoTerca > 0 ? centroCarregamento.CapacidadeCarregamentoTerca.ToString("n0") : "",
                    CapacidadeCarregamentoQuarta = centroCarregamento.CapacidadeCarregamentoQuarta > 0 ? centroCarregamento.CapacidadeCarregamentoQuarta.ToString("n0") : "",
                    CapacidadeCarregamentoQuinta = centroCarregamento.CapacidadeCarregamentoQuinta > 0 ? centroCarregamento.CapacidadeCarregamentoQuinta.ToString("n0") : "",
                    CapacidadeCarregamentoSexta = centroCarregamento.CapacidadeCarregamentoSexta > 0 ? centroCarregamento.CapacidadeCarregamentoSexta.ToString("n0") : "",
                    CapacidadeCarregamentoSabado = centroCarregamento.CapacidadeCarregamentoSabado > 0 ? centroCarregamento.CapacidadeCarregamentoSabado.ToString("n0") : "",
                    CapacidadeCarregamentoDomingo = centroCarregamento.CapacidadeCarregamentoDomingo > 0 ? centroCarregamento.CapacidadeCarregamentoDomingo.ToString("n0") : "",
                    CapacidadeCarregamentoCubagemSegunda = centroCarregamento.CapacidadeCarregamentoCubagemSegunda > 0 ? centroCarregamento.CapacidadeCarregamentoCubagemSegunda.ToString("n0") : "",
                    CapacidadeCarregamentoCubagemTerca = centroCarregamento.CapacidadeCarregamentoCubagemTerca > 0 ? centroCarregamento.CapacidadeCarregamentoCubagemTerca.ToString("n0") : "",
                    CapacidadeCarregamentoCubagemQuarta = centroCarregamento.CapacidadeCarregamentoCubagemQuarta > 0 ? centroCarregamento.CapacidadeCarregamentoCubagemQuarta.ToString("n0") : "",
                    CapacidadeCarregamentoCubagemQuinta = centroCarregamento.CapacidadeCarregamentoCubagemQuinta > 0 ? centroCarregamento.CapacidadeCarregamentoCubagemQuinta.ToString("n0") : "",
                    CapacidadeCarregamentoCubagemSexta = centroCarregamento.CapacidadeCarregamentoCubagemSexta > 0 ? centroCarregamento.CapacidadeCarregamentoCubagemSexta.ToString("n0") : "",
                    CapacidadeCarregamentoCubagemSabado = centroCarregamento.CapacidadeCarregamentoCubagemSabado > 0 ? centroCarregamento.CapacidadeCarregamentoCubagemSabado.ToString("n0") : "",
                    CapacidadeCarregamentoCubagemDomingo = centroCarregamento.CapacidadeCarregamentoCubagemDomingo > 0 ? centroCarregamento.CapacidadeCarregamentoCubagemDomingo.ToString("n0") : "",
                    centroCarregamento.ToleranciaAtrasoSegunda,
                    centroCarregamento.ToleranciaAtrasoTerca,
                    centroCarregamento.ToleranciaAtrasoQuarta,
                    centroCarregamento.ToleranciaAtrasoQuinta,
                    centroCarregamento.ToleranciaAtrasoSexta,
                    centroCarregamento.ToleranciaAtrasoSabado,
                    centroCarregamento.ToleranciaAtrasoDomingo,
                    centroCarregamento.VincularMotoristaFilaCarregamentoManualmente,
                    centroCarregamento.ExibirDetalhesCargaJanelaCarregamentoTransportador,
                    centroCarregamento.OcultarEdicaoDataHora,
                    centroCarregamento.DiasAdicionaisAlocacaoCargaJanelaCarregamento,
                    centroCarregamento.TipoJanelaCarregamento,
                    centroCarregamento.TipoPedidoMontagemCarregamento,
                    centroCarregamento.TipoEdicaoPalletProdutoMontagemCarregamento,
                    centroCarregamento.ExibirSomenteJanelaCarregamento,
                    centroCarregamento.TipoOrdenacaoJanelaCarregamento,
                    Latitude = centroCarregamento.Latitude ?? string.Empty,
                    Longitude = centroCarregamento.Longitude ?? string.Empty,
                    centroCarregamento.DistanciaMinimaEntrarFilaCarregamento,
                    centroCarregamento.TempoAguardarConfirmacaoTransportador,
                    centroCarregamento.PermitirTransportadorInformarValorFrete,
                    centroCarregamento.ManterComponentesTabelaFrete,
                    centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso,
                    centroCarregamento.TipoCapacidadeCarregamentoPorPeso,
                    centroCarregamento.TipoCapacidadeCarregamento,
                    centroCarregamento.LimiteCarregamentos,
                    centroCarregamento.CodigoIntegracao,
                    centroCarregamento.CargasComoExcedentesNaJanela,
                    centroCarregamento.NaoExibirValorFretePortalTransportador,
                    centroCarregamento.PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador,
                    centroCarregamento.NaoValidarIntegracaoGR,
                    centroCarregamento.PermitirSelecaoPeriodoCarregamentoJanelaCarregamentoTransportador,
                    centroCarregamento.PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador,
                    centroCarregamento.PermitirInformarAreaVeiculoJanelaCarregamento,
                    centroCarregamento.GerarGuaritaMesmoSemVeiculoInformado,
                    centroCarregamento.SeDataInformadaForInferiorDataAtualUtilizarDataAtualComoReferenciaHorarioInicialJanelaCarregamento,
                    centroCarregamento.GerarJanelaCarregamentoDestino,
                    centroCarregamento.PermitirGeracaoJanelaParaCargaRedespacho,
                    centroCarregamento.UtilizarNumeroReduzidoDeColunas,
                    centroCarregamento.BloquearVeiculoSemEspelhamento,
                    centroCarregamento.BloquearVeiculoSemEspelhamentoTelaCarga,
                    centroCarregamento.EnviarEmailParaTransportadorAoDisponibilizarCarga,
                    centroCarregamento.TermoAceite,
                    centroCarregamento.CamposVisiveisTransportador,
                    centroCarregamento.CamposVisiveisJanela,
                    centroCarregamento.NotificarSomenteAlteracaoCotacao,
                    centroCarregamento.NaoEnviarNotificacaoCargaRejeitadaParaTransportador,
                    centroCarregamento.NaoBloquearCapacidadeExcedida,
                    centroCarregamento.ObservacaoRetira,
                    centroCarregamento.LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota,
                    centroCarregamento.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido,
                    centroCarregamento.PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento,
                    centroCarregamento.ConsiderarPesoPalletPesoTotalCarga,
                    centroCarregamento.NaoGerarCarregamentoForaCapacidadeModeloVeicularCarga,
                    centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga,
                    EmpresaPadrao = centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga ? new { centroCarregamento.ConfiguracaoPadrao?.EmpresaPadrao?.Codigo, centroCarregamento.ConfiguracaoPadrao?.EmpresaPadrao?.Descricao } : null,
                    TipoOperacaoPadrao = centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga ? new { centroCarregamento.ConfiguracaoPadrao?.TipoOperacaoPadrao?.Codigo, centroCarregamento.ConfiguracaoPadrao?.TipoOperacaoPadrao?.Descricao } : null,
                    VeiculoPadrao = centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga ? new { centroCarregamento.ConfiguracaoPadrao?.VeiculoPadrao?.Codigo, centroCarregamento.ConfiguracaoPadrao?.VeiculoPadrao?.Descricao } : null,
                    ModeloVeicularPadrao = centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga ? new { centroCarregamento.ConfiguracaoPadrao?.ModeloVeicularCargaPadrao?.Codigo, centroCarregamento.ConfiguracaoPadrao?.ModeloVeicularCargaPadrao?.Descricao } : null,
                    MotoristaPadrao = centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga ? new { centroCarregamento.ConfiguracaoPadrao?.MotoristaPadrao?.Codigo, centroCarregamento.ConfiguracaoPadrao?.MotoristaPadrao?.Descricao, CPF = centroCarregamento.ConfiguracaoPadrao?.MotoristaPadrao?.CPF ?? string.Empty } : null,
                    Configuracao = new
                    {
                        centroCarregamento.JanelaCarregamentoAbaPendentes,
                        centroCarregamento.JanelaCarregamentoAbaExcedentes,
                        centroCarregamento.JanelaCarregamentoAbaReservas,
                        centroCarregamento.JanelaCarregamentoExibirSituacaoPatio,
                        centroCarregamento.EscolherHorarioCarregamentoPorLista,
                        centroCarregamento.ExibirVisualizacaoDosTiposDeOperacao,
                        centroCarregamento.PermiteTransportadorVisualizarMenorLanceLeilao,
                        centroCarregamento.PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes,
                        centroCarregamento.NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga,
                        centroCarregamento.RepassarCargaCasoNaoExistaVeiculoDisponivel,
                        centroCarregamento.DataCarregamentoObrigatoriaMontagemCarga,
                        centroCarregamento.NaoPermitirAlterarDataCarregamentoCarga,
                        centroCarregamento.PermiteTransportadorSelecionarHorarioCarregamento,
                        centroCarregamento.IntervaloSelecaoHorarioCarregamentoTransportador,
                        centroCarregamento.TempoMaximoModificarHorarioCarregamentoTransportador,
                        centroCarregamento.BloquearComponentesDeFreteJanelaCarregamentoTransportador,
                        centroCarregamento.ExibirNotasFiscaisJanelaCarregamentoTransportador,
                        centroCarregamento.BloquearTrocaDataListaHorarios,
                        centroCarregamento.ExibirDadosAvancadosJanelaCarregamento,
                        centroCarregamento.PermitirTransportadorImprimirOrdemColeta,
                        centroCarregamento.RetornarJanelaCarregamentoParaAgLiberacaoParaTransportadoresAposRejeicaoDoTransportador,
                        centroCarregamento.ToleranciaDataRetroativa,
                        centroCarregamento.LimiteAlteracoesHorarioTransportador,
                        centroCarregamento.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento,
                        centroCarregamento.ExigirTermoAceiteTransporte,
                        centroCarregamento.PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador,
                        centroCarregamento.ExigirTransportadorConfirmarMDFeNaoEncerradoForaDoSistema,
                        centroCarregamento.ExigirTransportadorInformarMotivoAoRejeitarCarga,
                        centroCarregamento.ExibirFilialJanelaCarregamentoTransportador,
                        centroCarregamento.NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos,
                        centroCarregamento.PermiteTransportadorVisualizarColocacaoDentreLancesLeilao,
                        centroCarregamento.ExigirConfirmacaoParticipacaoLeilao,
                        centroCarregamento.PermitirQueTransportadorAltereHorarioDoCarregamento,
                        centroCarregamento.EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente,
                        centroCarregamento.DiasAtrasoPermitidosPedidosAgendamentoPedidos,
                        centroCarregamento.MensagemConfirmacaoLeilao,
                        centroCarregamento.PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados,
                        centroCarregamento.NaoPermitirAgendarCargasNoMesmoDia,
                        centroCarregamento.ExigirChecklistAoConfirmarDadosTransporteMultiTransportador,
                        centroCarregamento.GerarControleVisualizacaoTransportadorasTerceiros,
                    },
                    UsuariosNotificacao = (
                        from obj in centroCarregamento.UsuariosNotificacao
                        select new
                        {
                            obj.Codigo,
                            obj.Nome,
                            obj.CPF_Formatado
                        }
                    ).ToList(),
                    Filial = new { Codigo = centroCarregamento.Filial?.Codigo ?? 0, Descricao = centroCarregamento.Filial?.Descricao ?? string.Empty },
                    MotivoAdvertenciaChegadaEmAtraso = new { Codigo = centroCarregamento.MotivoAdvertenciaChegadaEmAtraso?.Codigo ?? 0, Descricao = centroCarregamento.MotivoAdvertenciaChegadaEmAtraso?.Descricao ?? string.Empty },
                    centroCarregamento.TempoToleranciaChegadaAtraso,
                    centroCarregamento.TempoToleranciaCargaFechada,
                    centroCarregamento.BloqueioMarcacaoInteresseAntesDiasVencimentoCertificadoApoliceSeguro,
                    centroCarregamento.HabilitarTermoChegadaHorario,
                    centroCarregamento.TermoChegadaHorario,
                    centroCarregamento.HorasTrabalho,
                    HoraInicioViagemPrevista = centroCarregamento.HoraInicioViagemPrevista.HasValue ? centroCarregamento.HoraInicioViagemPrevista.Value.ToString(@"hh\:mm") : "",
                    TiposCarga = (
                        from obj in centroCarregamento.TiposCarga
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            CodigoIntegracao = obj.CodigoTipoCargaEmbarcador
                        }
                    ).ToList(),
                    Veiculos = (
                        from veiculo in centroCarregamento.Veiculos
                        select new
                        {
                            veiculo.Codigo,
                            veiculo.Placa
                        }
                    ).ToList(),
                    TransportadoresAutorizadosLiberarFaturamento = (
                        from obj in centroCarregamento.TransportadoresAutorizadosLiberarFaturamento
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.RazaoSocial
                        }
                    ).ToList(),
                    TemposCarregamento = (
                        from o in centroCarregamento.TemposCarregamento
                        select new
                        {
                            Codigo = !duplicar ? o.Codigo : 0,
                            CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                            CodigoTipoCarga = o.TipoCarga.Codigo,
                            CodigoModeloVeicularIntegracao = o.ModeloVeicular.CodigoIntegracao,
                            CodigoTipoCargaIntegracao = o.TipoCarga.CodigoTipoCargaEmbarcador,
                            DescricaoModeloVeicular = o.ModeloVeicular.Descricao,
                            DescricaoTipoCarga = o.TipoCarga.Descricao,
                            HoraInicio = o.HoraInicio.HasValue ? o.HoraInicio.Value.ToString(@"hh\:mm") : "",
                            HoraTermino = o.HoraTermino.HasValue ? o.HoraTermino.Value.ToString(@"hh\:mm") : "",
                            o.PeriodoCarregamento,
                            Tempo = o.Tempo.ToString("n0"),
                            QuantidadeMaximaEntregasRoteirizar = o.QuantidadeMaximaEntregasRoteirizar.ToString("n0"),
                            QuantidadeVagasOcuparGradeNaCarregamento = (o.QuantidadeVagasOcuparGradeNaCarregamento > 0) ? o.QuantidadeVagasOcuparGradeNaCarregamento.ToString("n0") : "",
                            QuantidadeMinimaEntregasRoteirizar = o.QuantidadeMinimaEntregasRoteirizar.ToString("n0")
                        }
                    ).ToList(),
                    Lances = new
                    {
                        centroCarregamento.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente,
                        ListaLances = (
                            from obj in listaLances
                            select new
                            {
                                Codigo = !duplicar ? obj.Codigo : 0,
                                obj.NumeroLanceDe,
                                obj.NumeroLanceAte,
                                obj.PorcentagemLance
                            }).ToList()
                    },
                    centroCarregamento.EnviarNotificacoesPorEmail,
                    centroCarregamento.EnviarNotificacoesCargasRejeitadasPorEmail,
                    centroCarregamento.EnviarEmailAlertaLeilaoParaTransportadorOfertado,
                    centroCarregamento.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente,
                    centroCarregamento.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado,
                    Emails = (
                        from obj in centroCarregamento.Emails
                        select new
                        {
                            Codigo = !duplicar ? obj.Codigo : 0,
                            obj.Email
                        }
                    ).ToList(),
                    PeriodosCarregamento = (
                        from obj in periodos
                        where obj.ExcecaoCapacidadeCarregamento == null
                        select new
                        {
                            Codigo = !duplicar ? obj.Codigo : 0,
                            DiaSemana = obj.Dia,
                            HoraInicio = string.Format("{0:00}:{1:00}", obj.HoraInicio.Hours, obj.HoraInicio.Minutes),
                            HoraTermino = string.Format("{0:00}:{1:00}", obj.HoraTermino.Hours, obj.HoraTermino.Minutes),
                            obj.ToleranciaExcessoTempo,
                            obj.CapacidadeCarregamentoSimultaneo,
                            CapacidadeCarregamentoVolume = (obj.CapacidadeCarregamentoVolume > 0) ? obj.CapacidadeCarregamentoVolume.ToString("n0") : "",
                            CapacidadeCarregamentoCubagem = (obj.CapacidadeCarregamentoCubagem > 0) ? obj.CapacidadeCarregamentoCubagem.ToString("n0") : "",
                        }
                    ).ToList(),
                    PrevisoesCarregamento = (
                        from obj in centroCarregamento.PrevisoesCarregamento
                        where obj.ExcecaoCapacidadeCarregamento == null
                        select new
                        {
                            Codigo = !duplicar ? obj.Codigo : 0,
                            Descricao = obj.Descricao ?? string.Empty,
                            DiaSemana = obj.Dia,
                            Rota = new
                            {
                                obj.Rota.Codigo,
                                obj.Rota.Descricao
                            },
                            ModelosVeiculos = (from modelo in obj.ModelosVeiculos
                                               select new
                                               {
                                                   Codigo = modelo.Codigo,
                                                   Descricao = modelo.Descricao
                                               }).ToList(),
                            obj.QuantidadeCargas,
                            obj.QuantidadeCargasExcedentes
                        }
                    ).ToList(),
                    DisponibilidadesFrota = (
                        from obj in centroCarregamento.DisponibilidadesFrota
                        select new
                        {
                            Codigo = !duplicar ? obj.Codigo : 0,
                            DiaSemana = obj.Dia,
                            obj.Quantidade,
                            ModeloVeicular = new
                            {
                                obj.ModeloVeicular.Codigo,
                                obj.ModeloVeicular.Descricao
                            },
                            Transportador = new
                            {
                                obj.Transportador?.Codigo,
                                obj.Transportador?.Descricao
                            }
                        }
                    ).ToList(),
                    LimitesCarregamento = (
                        from o in centroCarregamento.LimitesCarregamento
                        select new
                        {
                            Codigo = !duplicar ? o.Codigo : 0,
                            DiaSemana = o.Dia,
                            o.DiasAntecedencia,
                            HoraLimite = o.HoraLimite.ToString(@"hh\:mm"),
                            TipoCarga = new
                            {
                                o.TipoCarga.Codigo,
                                o.TipoCarga.Descricao
                            }
                        }
                    ).ToList(),
                    ControleExpedicao = (
                        from controleExpedicao in centroCarregamento.ExpedicoesCarregamento
                        select new
                        {
                            Codigo = !duplicar ? controleExpedicao.Codigo : 0,
                            DiaSemana = controleExpedicao.Dia,
                            Produto = new { Codigo = controleExpedicao.ProdutoEmbarcador?.Codigo ?? 0, Descricao = controleExpedicao.ProdutoEmbarcador?.Descricao ?? "" },
                            ClienteDestino = new { Codigo = controleExpedicao.ClienteDestino?.Codigo ?? 0, Descricao = controleExpedicao.ClienteDestino?.Descricao ?? "" },
                            Quantidade = controleExpedicao.Quantidade.ToString("n0"),
                            ModelosVeicularesCargaExclusivo = (
                                from modeloVeicular in controleExpedicao.ModelosVeicularesCargaExclusivo
                                select new
                                {
                                    modeloVeicular.ModeloVeicularCarga.Codigo,
                                    modeloVeicular.ModeloVeicularCarga.Descricao
                                }
                            ).ToList()
                        }
                    ).ToList(),
                    Doca = new
                    {
                        Dados = new
                        {
                            centroCarregamento.LimiteCargasPorLocalCarregamento
                        },
                        Docas = (
                            from doca in centroCarregamento.Docas
                            select new
                            {
                                Codigo = !duplicar ? doca.Codigo : 0,
                                doca.CodigoIntegracao,
                                doca.Descricao,
                                doca.Numero
                            }
                        ).ToList()
                    },
                    ManobraAcao = new
                    {
                        Dados = new
                        {
                            AcaoManobraPadraoInicioCarregamento = new { Codigo = centroCarregamento.AcaoManobraPadraoInicioCarregamento?.Codigo ?? 0, Descricao = centroCarregamento.AcaoManobraPadraoInicioCarregamento?.Descricao ?? "" },
                            AcaoManobraPadraoInicioReversa = new { Codigo = centroCarregamento.AcaoManobraPadraoInicioReversa?.Codigo ?? 0, Descricao = centroCarregamento.AcaoManobraPadraoInicioReversa?.Descricao ?? "" },
                            AcaoManobraPadraoFimCarregamento = new { Codigo = centroCarregamento.AcaoManobraPadraoFimCarregamento?.Codigo ?? 0, Descricao = centroCarregamento.AcaoManobraPadraoFimCarregamento?.Descricao ?? "" },
                            AcaoManobraPadraoFimReversa = new { Codigo = centroCarregamento.AcaoManobraPadraoFimReversa?.Codigo ?? 0, Descricao = centroCarregamento.AcaoManobraPadraoFimReversa?.Descricao ?? "" },
                            centroCarregamento.UtilizarControleManobra
                        },
                        AcoesManobra = (
                            from manobraAcao in centroCarregamento.AcoesManobra
                            select new
                            {
                                Codigo = !duplicar ? manobraAcao.Codigo : 0,
                                CodigoManobraAcao = manobraAcao.Acao.Codigo,
                                DescricaoManobraAcao = manobraAcao.Acao.Descricao,
                                manobraAcao.TempoToleranciaInicioManobra
                            }
                        ).ToList()
                    },
                    DadosTransportador = new
                    {
                        centroCarregamento.PermitirMatrizSelecionarFilial,
                        centroCarregamento.LiberarCargaManualmenteParaTransportadores,
                        centroCarregamento.LiberarCargaAutomaticamenteParaTransportadoras,
                        centroCarregamento.LiberarCargaAutomaticamenteParaTransportadorasForaRota,
                        centroCarregamento.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente,
                        centroCarregamento.PermitirLiberarCargaTransportadorExclusivo,
                        TipoTransportadorCentroCarregamento = centroCarregamento.TipoTransportador,
                        TipoTransportadorSecundarioCentroCarregamento = centroCarregamento.TipoTransportadorSecundario,
                        TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente = (centroCarregamento.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente > 0) ? centroCarregamento.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente.ToString("n0") : "",
                        TempoAguardarAprovacaoTransportador = (centroCarregamento.TempoAguardarAprovacaoTransportador > 0) ? centroCarregamento.TempoAguardarAprovacaoTransportador.ToString("n0") : "",
                        TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente = (centroCarregamento.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente > 0) ? centroCarregamento.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente.ToString("n0") : "",
                        ListaCentroCarregamentoTransportador = (
                            from centroCarregamentotransportador in centroCarregamento.Transportadores
                            select new
                            {
                                Codigo = !duplicar ? centroCarregamentotransportador.Codigo : 0,
                                Transportador = new
                                {
                                    centroCarregamentotransportador.Transportador.Codigo,
                                    centroCarregamentotransportador.Transportador.Descricao
                                },
                                ClientesDestino = (
                                    from clienteDestino in centroCarregamentotransportador.ClientesDestino
                                    select new
                                    {
                                        clienteDestino.Codigo,
                                        clienteDestino.Descricao
                                    }
                                ).ToList(),
                                LocalidadesDestino = (
                                    from localidadeDestino in centroCarregamentotransportador.LocalidadesDestino
                                    select new
                                    {
                                        localidadeDestino.Codigo,
                                        localidadeDestino.Descricao,
                                        Estado = !string.IsNullOrWhiteSpace(localidadeDestino.Estado.Abreviacao) ? localidadeDestino.Estado.Abreviacao : localidadeDestino.Estado.Sigla,
                                    }
                                ).ToList(),
                                TiposCarga = (
                                    from tipoDeCarga in centroCarregamentotransportador.TiposDeCarga
                                    select new
                                    {
                                        tipoDeCarga.Codigo,
                                        tipoDeCarga.Descricao,
                                    }
                                ).ToList()
                            }
                        ).ToList(),
                        TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras = (
                            from tipoCarga in centroCarregamento.TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras
                            select new
                            {
                                tipoCarga.Codigo,
                                tipoCarga.Descricao
                            }
                        ).ToList()
                    },
                    DadosTransportadorTerceiro = new
                    {
                        centroCarregamento.LiberarCargaManualmenteParaTransportadoresTerceiros,
                        centroCarregamento.LiberarCargaAutomaticamenteParaTransportadorasTerceiros,
                        TipoTransportadorTerceiroCentroCarregamento = centroCarregamento.TipoTransportadorTerceiro,
                        ListaCentroCarregamentoTransportadorTerceiro = (
                            from centroCarregamentotransportadorTerceiro in centroCarregamento.TransportadoresTerceiros
                            select new
                            {
                                Codigo = !duplicar ? centroCarregamentotransportadorTerceiro.Codigo : 0,
                                Transportador = new
                                {
                                    centroCarregamentotransportadorTerceiro.Transportador.Codigo,
                                    centroCarregamentotransportadorTerceiro.Transportador.Descricao
                                }
                            }
                        ).ToList(),
                        TipoTransportadorTerceiroSecundarioCentroCarregamento = centroCarregamento.TipoTransportadorTerceiroSecundario,
                        TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente = (centroCarregamento.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente > 0) ? centroCarregamento.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente.ToString("n0") : "",

                    },
                    DadosMotorista = new
                    {
                        centroCarregamento.LimiteCargasPorMotoristaPorDia,
                        centroCarregamento.LimiteDeCargasAtivasPorMotorista,
                    },
                    DadosVeiculo = new
                    {
                        centroCarregamento.LimiteCargasPorVeiculoPorDia
                    },
                    TipoOperacoes = (
                        from obj in repositorioCentroCarregamentoTipoOperacao.BuscarPorCentro(codigo)
                        select new
                        {
                            Codigo = !duplicar ? obj.Codigo : 0,
                            CodigoTipoOperacao = obj.TipoOperacao.Codigo,
                            TipoOperacao = obj.TipoOperacao.Descricao,
                            Tipo = obj.Tipo,
                            TipoDescricao = CentroCarregamentoTipoOperacaoTipoHelper.ObterDescricao(obj.Tipo)
                        }
                    ).ToList(),
                    NaoComparecimento = ObterAutomatizacaoNaoComparecimento(centroCarregamento, duplicar, unidadeDeTrabalho),
                    ProdutividadeCarregamentos = (
                    from obj in centroCarregamentoProdutividades
                    select new
                    {
                        Codigo = !duplicar ? obj.Codigo : 0,
                        GrupoPessoas = new { Codigo = obj.GrupoPessoas?.Codigo ?? 0, Descricao = obj.GrupoPessoas?.Descricao ?? "" },
                        TipoOperacao = new { Codigo = obj.TipoOperacao?.Codigo ?? 0, Descricao = obj.TipoOperacao?.Descricao ?? "" },
                        Transportador = new { Codigo = obj.Transportador?.Codigo ?? 0, Descricao = obj.Transportador?.Descricao ?? "" },
                        obj.Picking,
                        obj.Separacao,
                        obj.Carregamento,
                        obj.HorasTrabalho
                    }
                    ).ToList(),
                    OfertaCarga = new
                    {
                        centroCarregamento.AtivarRegraParaOfertarCarga,
                        ofertasCarga.FirstOrDefault()?.PeriodoDiferenciadoShare,
                        DataInicialPeriodoDiferenciadoShare = ofertasCarga.FirstOrDefault()?.DataInicialPeriodoDiferenciadoShare?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFinalPeriodoDiferenciadoShare = ofertasCarga.FirstOrDefault()?.DataFinalPeriodoDiferenciadoShare?.ToString("dd/MM/yyyy") ?? string.Empty,
                        OfertasCarga = (
                            from obj in ofertasCarga
                            select new
                            {
                                Codigo = !duplicar ? obj.Codigo : 0,
                                obj.Prioridade,
                                RegraValue = obj.Regra,
                                RegraDescricao = obj.Regra.ObterDescricao()
                            }).ToList()
                    },
                    PunicoesCarregamento = (from obj in centroCarregamentoPunicao
                                            select new
                                            {
                                                Codigo = obj.Codigo,
                                                TipoFrota = obj.TipoFrota,
                                                TipoFrotaDescricao = obj.TipoFrotaDescricao,
                                                HorasPunicao = (int)obj.TempoPunicao,
                                            }
                    ).ToList(),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.TempoCarregamento repTempoCarregamento = new Repositorio.Embarcador.Logistica.TempoCarregamento(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorCodigo(codigo);

                centroCarregamento.TemposCarregamento = null;
                centroCarregamento.Emails = null;
                centroCarregamento.PeriodosCarregamento = null;
                centroCarregamento.PrevisoesCarregamento = null;
                centroCarregamento.DisponibilidadesFrota = null;
                centroCarregamento.LimitesCarregamento = null;
                centroCarregamento.TiposCarga = null;
                centroCarregamento.TransportadoresAutorizadosLiberarFaturamento = null;
                centroCarregamento.ExcecoesCapacidadeCarregamento = null;
                centroCarregamento.ExpedicoesCarregamento = null;

                repCentroCarregamento.Deletar(centroCarregamento, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Logistica.CentroCarregamento.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizalo);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);

            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTempoCarregamento();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                string dados = Request.GetStringParam("Dados");
                List<dynamic> objetoImportacao = new List<dynamic>();
                int contador = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTipoCarga = (from obj in linhas[i].Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();
                        if (colunaTipoCarga == null)
                            throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TipoDeCargaObrigatorio);

                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoCarga.BuscarPorCodigoEmbarcador((string)colunaTipoCarga.Valor) ?? throw new ControllerException("Tipo de carga não foi encontrado");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaModeloVeicular = (from obj in linhas[i].Colunas where obj.NomeCampo == "ModeloVeicular" select obj).FirstOrDefault();
                        if (colunaModeloVeicular == null)
                            throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.ModeloVeicularObrigatorio);

                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao((string)colunaModeloVeicular.Valor) ?? throw new ControllerException("Modelo veicular não foi encontrado");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTempo = (from obj in linhas[i].Colunas where obj.NomeCampo == "Tempo" select obj).FirstOrDefault();
                        if (colunaModeloVeicular == null)
                            throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TempoObrigatorio);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaMaximoEntregas = (from obj in linhas[i].Colunas where obj.NomeCampo == "MaximoEntregas" select obj).FirstOrDefault();
                        int quantidadeMaximaEntregasRoteirizar = 0;
                        if (colunaMaximoEntregas != null && !string.IsNullOrWhiteSpace(colunaMaximoEntregas.Valor))
                            quantidadeMaximaEntregasRoteirizar = ((string)colunaMaximoEntregas.Valor).ToInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaInicio = (from obj in linhas[i].Colunas where obj.NomeCampo == "Inicio" select obj).FirstOrDefault();
                        TimeSpan? horaInicio = null;
                        if (colunaInicio != null && !string.IsNullOrWhiteSpace(colunaInicio.Valor))
                            horaInicio = ((string)colunaInicio.Valor).ToTime();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTermino = (from obj in linhas[i].Colunas where obj.NomeCampo == "Termino" select obj).FirstOrDefault();
                        TimeSpan? horaTermino = null;
                        if (colunaTermino != null && !string.IsNullOrWhiteSpace(colunaTermino.Valor))
                            horaTermino = ((string)colunaTermino.Valor).ToTime();

                        objetoImportacao.Add(new
                        {
                            CodigoTipoCarga = tipoCarga.Codigo,
                            CodigoTipoCargaIntegracao = tipoCarga.CodigoTipoCargaEmbarcador,
                            DescricaoTipoCarga = tipoCarga.Descricao,
                            CodigoModeloVeicular = modeloVeicular.Codigo,
                            CodigoModeloVeicularIntegracao = modeloVeicular.CodigoIntegracao,
                            DescricaoModeloVeicular = modeloVeicular.Descricao,
                            HoraInicio = horaInicio?.ToString(@"hh\:mm") ?? string.Empty,
                            HoraTermino = horaTermino?.ToString(@"hh\:mm") ?? string.Empty,
                            QuantidadeMaximaEntregasRoteirizar = quantidadeMaximaEntregasRoteirizar,
                            Tempo = ((string)colunaTempo.Valor).ToInt(),
                        });
                        contador++;

                        retornoImportacao.Retornolinhas.Add(new RetonoLinha()
                        {
                            indice = i,
                            processou = true
                        });
                    }
                    catch (ControllerException ex2)
                    {
                        retornoImportacao.Retornolinhas.Add(new RetonoLinha()
                        {
                            indice = i,
                            mensagemFalha = ex2.Message,
                            processou = false
                        });
                        Servicos.Log.TratarErro(ex2);
                    }
                }

                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;
                retornoImportacao.Retorno = objetoImportacao;

                return new JsonpResult(retornoImportacao);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(true, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarDadosCarregamento()
        {
            try
            {
                var grid = ObterGridDadosCarregamento();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"Dados de Carregamento.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        #endregion

        #region Métodos Privados

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoTempoCarregamento()
        {
            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Logistica.CentroCarregamento.TipoDeCarga, Propriedade = "TipoCarga", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Logistica.CentroCarregamento.ModeloVeicular, Propriedade = "ModeloVeicular", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Logistica.CentroCarregamento.Tempo, Propriedade = "Tempo", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Logistica.CentroCarregamento.MaximoEntregas, Propriedade = "MaximoEntregas", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Logistica.CentroCarregamento.Inicio, Propriedade = "Inicio", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Logistica.CentroCarregamento.Termino, Propriedade = "Termino", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }

        private Models.Grid.Grid ObterGridDadosCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.TipoDeCarga, "TipoCarga", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.CodigoTipoDeCarga, "CodigoTipoCarga", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.ModeloVeicular, "ModeloVeicular", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.CodigoModeloVeicular, "CodigoModeloVeicular", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.Tempo, "Tempo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.MaximoEntregas, "MaximoEntregas", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.Inicio, "Inicio", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroCarregamento.Termino, "Termino", 20, Models.Grid.Align.left, true);

                List<dynamic> dadosExportacao = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Dados"));

                var retorno = dadosExportacao.Select(dado => new
                {
                    TipoCarga = dado.DescricaoTipoCarga,
                    CodigoTipoCarga = dado.CodigoTipoCargaIntegracao,
                    ModeloVeicular = dado.DescricaoModeloVeicular,
                    CodigoModeloVeicular = dado.CodigoModeloVeicularIntegracao,
                    Tempo = dado.Tempo,
                    MaximoEntregas = dado.QuantidadeMaximaEntregasRoteirizar,
                    Inicio = dado.HoraInicio,
                    Termino = dado.HoraTermino,
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(dadosExportacao.Count());

                return grid;
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void PreencherCentroCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);
            Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repMotivoAdvertenciaTransportador = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unidadeDeTrabalho);

            bool ativo, liberarCargaAutomaticamenteParaTransportadoras, indicarTemposVeiculos, permitirTransportadorInformarValorFrete;
            bool.TryParse(Request.Params("Ativo"), out ativo);
            bool.TryParse(Request.Params("LiberarCargaAutomaticamenteParaTransportadoras"), out liberarCargaAutomaticamenteParaTransportadoras);
            bool.TryParse(Request.Params("IndicarTemposVeiculos"), out indicarTemposVeiculos);
            bool.TryParse(Request.Params("PermitirTransportadorInformarValorFrete"), out permitirTransportadorInformarValorFrete);
            bool.TryParse(Request.Params("CargasComoExcedentesNaJanela"), out bool cargasComoExcedentesNaJanela);

            int codigoFilial, numeroDocas, tempoBloqueioEscolhaTransportador, tempoEmMinutosLiberacao, tempoEncostaDoca, tempoToleranciaPedidoRoteirizar, motivoAdvertenciaChegadaEmAtraso;
            int.TryParse(Request.Params("Filial"), out codigoFilial);
            int.TryParse(Request.Params("NumeroDocas"), out numeroDocas);
            int.TryParse(Request.Params("TempoBloqueioEscolhaTransportador"), out tempoBloqueioEscolhaTransportador);
            int.TryParse(Request.Params("TempoEmMinutosLiberacao"), out tempoEmMinutosLiberacao);
            int.TryParse(Request.Params("TempoEncostaDoca"), out tempoEncostaDoca);
            int.TryParse(Request.Params("TempoToleranciaPedidoRoteirizar"), out tempoToleranciaPedidoRoteirizar);

            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento tipoTransportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento.Todos;
            Enum.TryParse(Request.Params("TipoTransportadorCentroCarregamento"), out tipoTransportador);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento tipoTransportadorTerceiro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroCarregamento.Todos;
            Enum.TryParse(Request.Params("TipoTransportadorTerceiroCentroCarregamento"), out tipoTransportadorTerceiro);

            centroCarregamento.Ativo = ativo;
            centroCarregamento.Descricao = descricao;
            centroCarregamento.Filial = repFilial.BuscarPorCodigo(codigoFilial);
            centroCarregamento.NumeroDocas = numeroDocas;
            centroCarregamento.IndicarTemposVeiculos = indicarTemposVeiculos;
            centroCarregamento.PermitirTransportadorInformarValorFrete = permitirTransportadorInformarValorFrete;
            centroCarregamento.ManterComponentesTabelaFrete = Request.GetBoolParam("ManterComponentesTabelaFrete");
            centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso = Request.GetBoolParam("UtilizarCapacidadeCarregamentoPorPeso");
            centroCarregamento.TipoCapacidadeCarregamentoPorPeso = Request.GetNullableEnumParam<TipoCapacidadeCarregamentoPorPeso>("TipoCapacidadeCarregamentoPorPeso");
            centroCarregamento.TipoCapacidadeCarregamento = Request.GetNullableEnumParam<TipoCapacidadeCarregamento>("TipoCapacidadeCarregamento");
            centroCarregamento.LimiteCarregamentos = Request.GetEnumParam<LimiteCarregamentosCentroCarregamento>("LimiteCarregamentos");
            centroCarregamento.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            centroCarregamento.TempoBloqueioEscolhaTransportador = tempoBloqueioEscolhaTransportador;
            centroCarregamento.TempoEncostaDoca = tempoEncostaDoca;
            centroCarregamento.TempoToleranciaPedidoRoteirizar = tempoToleranciaPedidoRoteirizar;
            centroCarregamento.QuantidadeMaximaEntregasRoteirizar = Request.GetIntParam("QuantidadeMaximaEntregasRoteirizar", 999);
            centroCarregamento.QuantidadeMaximaPedidosSessaoRoteirizar = Request.GetIntParam("QuantidadeMaximaPedidosSessaoRoteirizar");
            centroCarregamento.UtilizarDispFrotaCentroDescCliente = Request.GetBoolParam("UtilizarDispFrotaCentroDescCliente");
            centroCarregamento.ConsiderarTempoDeslocamentoPrimeiraEntrega = Request.GetBoolParam("ConsiderarTempoDeslocamentoPrimeiraEntrega");
            centroCarregamento.GerarCarregamentosAlemDaDispFrota = Request.GetBoolParam("GerarCarregamentosAlemDaDispFrota");
            centroCarregamento.AgruparPedidosMesmoDestinatario = Request.GetBoolParam("AgruparPedidosMesmoDestinatario");
            centroCarregamento.GerarCarregamentoDoisDias = Request.GetBoolParam("GerarCarregamentoDoisDias");
            centroCarregamento.CarregamentoTempoMaximoRota = Request.GetIntParam("CarregamentoTempoMaximoRota");
            centroCarregamento.MontagemCarregamentoPedidoProduto = Request.GetBoolParam("MontagemCarregamentoPedidoProduto");
            centroCarregamento.MontagemCarregamentoPedidoIntegral = Request.GetBoolParam("MontagemCarregamentoPedidoIntegral");
            //centroCarregamento.MontagemCarregamentoColetaEntrega = Request.GetBoolParam("MontagemCarregamentoColetaEntrega");
            centroCarregamento.TipoRoteirizacaoColetaEntrega = Request.GetEnumParam<TipoRoteirizacaoColetaEntrega>("TipoRoteirizacaoColetaEntrega");
            centroCarregamento.TempoMinutosEscolhaAutomaticaCotacao = Request.GetIntParam("TempoMinutosEscolhaAutomaticaCotacao");
            centroCarregamento.PercentualMaximoDiferencaValorCotacao = Request.GetDecimalParam("PercentualMaximoDiferencaValorCotacao");
            centroCarregamento.PercentualMinimoDiferencaValorCotacao = Request.GetDecimalParam("PercentualMinimoDiferencaValorCotacao");
            centroCarregamento.PercentualToleranciaPesoCarregamento = Request.GetDecimalParam("PercentualToleranciaPesoCarregamento");
            centroCarregamento.PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao = Request.GetIntParam("PontuacaoDescontarTransportadorPorEscolhaAutomaticaCotacao");
            centroCarregamento.LimiteRecorrencia = Request.GetIntParam("LimiteRecorrencia");
            centroCarregamento.Observacao = observacao;
            centroCarregamento.TipoTransportador = tipoTransportador;
            centroCarregamento.TipoTransportadorSecundario = Request.GetNullableEnumParam<TipoTransportadorCentroCarregamento>("TipoTransportadorSecundarioCentroCarregamento");
            centroCarregamento.PermitirLiberarCargaTransportadorExclusivo = Request.GetBoolParam("PermitirLiberarCargaTransportadorExclusivo");
            centroCarregamento.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente = Request.GetBoolParam("LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente");
            centroCarregamento.LiberarCargaManualmenteParaTransportadores = Request.GetBoolParam("LiberarCargaManualmenteParaTransportadores");
            centroCarregamento.PermitirMatrizSelecionarFilial = Request.GetBoolParam("PermitirMatrizSelecionarFilial");
            centroCarregamento.LiberarCargaAutomaticamenteParaTransportadoras = liberarCargaAutomaticamenteParaTransportadoras;
            centroCarregamento.LiberarCargaAutomaticamenteParaTransportadorasForaRota = Request.GetBoolParam("LiberarCargaAutomaticamenteParaTransportadorasForaRota");
            centroCarregamento.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente = Request.GetBoolParam("AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente");
            centroCarregamento.TempoEmMinutosLiberacao = tempoEmMinutosLiberacao;
            centroCarregamento.TempoAguardarConfirmacaoTransportador = Request.GetIntParam("TempoAguardarConfirmacaoTransportador");
            centroCarregamento.TempoAguardarAprovacaoTransportador = Request.GetIntParam("TempoAguardarAprovacaoTransportador");
            centroCarregamento.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente = Request.GetIntParam("TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente");
            centroCarregamento.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente = Request.GetIntParam("TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente");
            centroCarregamento.TipoTransportadorTerceiro = tipoTransportadorTerceiro;
            centroCarregamento.LiberarCargaAutomaticamenteParaTransportadorasTerceiros = Request.GetBoolParam("LiberarCargaAutomaticamenteParaTransportadorasTerceiros");
            centroCarregamento.LiberarCargaManualmenteParaTransportadoresTerceiros = Request.GetBoolParam("LiberarCargaManualmenteParaTransportadoresTerceiros");
            centroCarregamento.CapacidadeCarregamentoSegunda = Request.GetIntParam("CapacidadeCarregamentoSegunda");
            centroCarregamento.CapacidadeCarregamentoTerca = Request.GetIntParam("CapacidadeCarregamentoTerca");
            centroCarregamento.CapacidadeCarregamentoQuarta = Request.GetIntParam("CapacidadeCarregamentoQuarta");
            centroCarregamento.CapacidadeCarregamentoQuinta = Request.GetIntParam("CapacidadeCarregamentoQuinta");
            centroCarregamento.CapacidadeCarregamentoSexta = Request.GetIntParam("CapacidadeCarregamentoSexta");
            centroCarregamento.CapacidadeCarregamentoSabado = Request.GetIntParam("CapacidadeCarregamentoSabado");
            centroCarregamento.CapacidadeCarregamentoDomingo = Request.GetIntParam("CapacidadeCarregamentoDomingo");
            centroCarregamento.CapacidadeCarregamentoCubagemSegunda = Request.GetIntParam("CapacidadeCarregamentoCubagemSegunda");
            centroCarregamento.CapacidadeCarregamentoCubagemTerca = Request.GetIntParam("CapacidadeCarregamentoCubagemTerca");
            centroCarregamento.CapacidadeCarregamentoCubagemQuarta = Request.GetIntParam("CapacidadeCarregamentoCubagemQuarta");
            centroCarregamento.CapacidadeCarregamentoCubagemQuinta = Request.GetIntParam("CapacidadeCarregamentoCubagemQuinta");
            centroCarregamento.CapacidadeCarregamentoCubagemSexta = Request.GetIntParam("CapacidadeCarregamentoCubagemSexta");
            centroCarregamento.CapacidadeCarregamentoCubagemSabado = Request.GetIntParam("CapacidadeCarregamentoCubagemSabado");
            centroCarregamento.CapacidadeCarregamentoCubagemDomingo = Request.GetIntParam("CapacidadeCarregamentoCubagemDomingo");
            centroCarregamento.ToleranciaAtrasoSegunda = Request.GetIntParam("ToleranciaAtrasoSegunda");
            centroCarregamento.ToleranciaAtrasoTerca = Request.GetIntParam("ToleranciaAtrasoTerca");
            centroCarregamento.ToleranciaAtrasoQuarta = Request.GetIntParam("ToleranciaAtrasoQuarta");
            centroCarregamento.ToleranciaAtrasoQuinta = Request.GetIntParam("ToleranciaAtrasoQuinta");
            centroCarregamento.ToleranciaAtrasoSexta = Request.GetIntParam("ToleranciaAtrasoSexta");
            centroCarregamento.ToleranciaAtrasoSabado = Request.GetIntParam("ToleranciaAtrasoSabado");
            centroCarregamento.ToleranciaAtrasoDomingo = Request.GetIntParam("ToleranciaAtrasoDomingo");
            centroCarregamento.JanelaCarregamentoAbaPendentes = Request.GetBoolParam("JanelaCarregamentoAbaPendentes");
            centroCarregamento.JanelaCarregamentoAbaExcedentes = Request.GetBoolParam("JanelaCarregamentoAbaExcedentes");
            centroCarregamento.JanelaCarregamentoAbaReservas = Request.GetBoolParam("JanelaCarregamentoAbaReservas");
            centroCarregamento.JanelaCarregamentoExibirSituacaoPatio = Request.GetBoolParam("JanelaCarregamentoExibirSituacaoPatio");
            centroCarregamento.PermiteMarcarCargaComoNaoComparecimento = Request.GetBoolParam("PermiteMarcarCargaComoNaoComparecimento");
            centroCarregamento.EscolherHorarioCarregamentoPorLista = centroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas && Request.GetBoolParam("EscolherHorarioCarregamentoPorLista");
            centroCarregamento.DataCarregamentoObrigatoriaMontagemCarga = Request.GetBoolParam("DataCarregamentoObrigatoriaMontagemCarga");
            centroCarregamento.Latitude = Request.GetStringParam("Latitude");
            centroCarregamento.Longitude = Request.GetStringParam("Longitude");
            centroCarregamento.DistanciaMinimaEntrarFilaCarregamento = Request.GetIntParam("DistanciaMinimaEntrarFilaCarregamento");
            centroCarregamento.CargasComoExcedentesNaJanela = cargasComoExcedentesNaJanela;
            centroCarregamento.VincularMotoristaFilaCarregamentoManualmente = Request.GetBoolParam("VincularMotoristaFilaCarregamentoManualmente");
            centroCarregamento.ExibirDetalhesCargaJanelaCarregamentoTransportador = Request.GetBoolParam("ExibirDetalhesCargaJanelaCarregamentoTransportador");
            centroCarregamento.OcultarEdicaoDataHora = Request.GetBoolParam("OcultarEdicaoDataHora");
            centroCarregamento.UtilizarControleManobra = Request.GetBoolParam("UtilizarControleManobra");
            centroCarregamento.PermitirSelecaoPeriodoCarregamentoJanelaCarregamentoTransportador = Request.GetBoolParam("PermitirSelecaoPeriodoCarregamentoJanelaCarregamentoTransportador");
            centroCarregamento.PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador = Request.GetBoolParam("PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador");
            centroCarregamento.PermitirInformarAreaVeiculoJanelaCarregamento = Request.GetBoolParam("PermitirInformarAreaVeiculoJanelaCarregamento");
            centroCarregamento.GerarGuaritaMesmoSemVeiculoInformado = Request.GetBoolParam("GerarGuaritaMesmoSemVeiculoInformado");
            centroCarregamento.SeDataInformadaForInferiorDataAtualUtilizarDataAtualComoReferenciaHorarioInicialJanelaCarregamento = Request.GetBoolParam("SeDataInformadaForInferiorDataAtualUtilizarDataAtualComoReferenciaHorarioInicialJanelaCarregamento");
            centroCarregamento.GerarJanelaCarregamentoDestino = Request.GetBoolParam("GerarJanelaCarregamentoDestino");
            centroCarregamento.PermitirGeracaoJanelaParaCargaRedespacho = Request.GetBoolParam("PermitirGeracaoJanelaParaCargaRedespacho");
            centroCarregamento.UtilizarNumeroReduzidoDeColunas = Request.GetBoolParam("UtilizarNumeroReduzidoDeColunas");
            centroCarregamento.AcaoManobraPadraoInicioCarregamento = ObterManobraAcao(Request.GetIntParam("AcaoManobraPadraoInicioCarregamento"), unidadeDeTrabalho);
            centroCarregamento.AcaoManobraPadraoInicioReversa = ObterManobraAcao(Request.GetIntParam("AcaoManobraPadraoInicioReversa"), unidadeDeTrabalho);
            centroCarregamento.AcaoManobraPadraoFimCarregamento = ObterManobraAcao(Request.GetIntParam("AcaoManobraPadraoFimCarregamento"), unidadeDeTrabalho);
            centroCarregamento.AcaoManobraPadraoFimReversa = ObterManobraAcao(Request.GetIntParam("AcaoManobraPadraoFimReversa"), unidadeDeTrabalho);
            centroCarregamento.PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador = Request.GetBoolParam("PermitirInformarValorFreteCargasAtribuidasAoTransportadorNaJanelaCarregamentoTransportador");
            centroCarregamento.NaoValidarIntegracaoGR = Request.GetBoolParam("NaoValidarIntegracaoGR");
            centroCarregamento.TempoToleranciaChegadaAtraso = Request.GetIntParam("TempoToleranciaChegadaAtraso");
            centroCarregamento.TempoToleranciaCargaFechada = Request.GetIntParam("TempoToleranciaCargaFechada");
            centroCarregamento.BloqueioMarcacaoInteresseAntesDiasVencimentoCertificadoApoliceSeguro = Request.GetIntParam("BloqueioMarcacaoInteresseAntesDiasVencimentoCertificadoApoliceSeguro");
            centroCarregamento.HoraInicioViagemPrevista = Request.GetNullableTimeParam("HoraInicioViagemPrevista");
            centroCarregamento.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento = Request.GetIntParam("TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento");
            motivoAdvertenciaChegadaEmAtraso = Request.GetIntParam("MotivoAdvertenciaChegadaEmAtraso");
            if (motivoAdvertenciaChegadaEmAtraso > 0)
                centroCarregamento.MotivoAdvertenciaChegadaEmAtraso = repMotivoAdvertenciaTransportador.BuscarPorCodigo(motivoAdvertenciaChegadaEmAtraso, false);
            centroCarregamento.ExibirVisualizacaoDosTiposDeOperacao = centroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas && Request.GetBoolParam("ExibirVisualizacaoDosTiposDeOperacao");
            centroCarregamento.PermiteTransportadorVisualizarMenorLanceLeilao = Request.GetBoolParam("PermiteTransportadorVisualizarMenorLanceLeilao");
            centroCarregamento.NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga = Request.GetBoolParam("NaoPermiteTransportadorSugerirHorarioSeClientePossuirJanelaDescarga");
            centroCarregamento.PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes = Request.GetBoolParam("PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes");
            centroCarregamento.RepassarCargaCasoNaoExistaVeiculoDisponivel = Request.GetBoolParam("RepassarCargaCasoNaoExistaVeiculoDisponivel");
            centroCarregamento.DiasAdicionaisAlocacaoCargaJanelaCarregamento = Request.GetIntParam("DiasAdicionaisAlocacaoCargaJanelaCarregamento");
            centroCarregamento.TipoJanelaCarregamento = Request.GetEnumParam<TipoJanelaCarregamento>("TipoJanelaCarregamento");
            centroCarregamento.TipoPedidoMontagemCarregamento = Request.GetEnumParam<TipoPedidoMontagemCarregamento>("TipoPedidoMontagemCarregamento");
            centroCarregamento.TipoEdicaoPalletProdutoMontagemCarregamento = Request.GetEnumParam<TipoEdicaoPalletProdutoMontagemCarregamento>("TipoEdicaoPalletProdutoMontagemCarregamento");
            centroCarregamento.TipoMontagemCarregamentoVRP = Request.GetEnumParam<TipoMontagemCarregamentoVRP>("TipoMontagemCarregamentoVRP");
            centroCarregamento.SimuladorFreteCriterioSelecaoTransportador = Request.GetEnumParam<SimuladorFreteCriterioSelecaoTransportador>("SimuladorFreteCriterioSelecaoTransportador");
            centroCarregamento.TipoOcupacaoMontagemCarregamentoVRP = Request.GetEnumParam<TipoOcupacaoMontagemCarregamentoVRP>("TipoOcupacaoMontagemCarregamentoVRP");
            centroCarregamento.TipoResumoCarregamento = Request.GetEnumParam<TipoResumoCarregamento>("TipoResumoCarregamento");
            centroCarregamento.NivelQuebraProdutoRoteirizar = Request.GetEnumParam<NivelQuebraProdutoRoteirizar>("NivelQuebraProdutoRoteirizar");
            centroCarregamento.ExibirSomenteJanelaCarregamento = Request.GetBoolParam("ExibirSomenteJanelaCarregamento");
            centroCarregamento.TipoOrdenacaoJanelaCarregamento = Request.GetEnumParam<TipoOrdenacaoJanelaCarregamento>("TipoOrdenacaoJanelaCarregamento");
            centroCarregamento.LimiteCargasPorLocalCarregamento = Request.GetIntParam("LimiteCargasPorLocalCarregamento");
            centroCarregamento.LimiteCargasPorLocalCarregamento = Request.GetIntParam("BloquearVeiculoSemEspelhamentoTelaCarga");
            centroCarregamento.BloquearVeiculoSemEspelhamento = Request.GetBoolParam("BloquearVeiculoSemEspelhamento");
            centroCarregamento.BloquearVeiculoSemEspelhamentoTelaCarga = Request.GetBoolParam("BloquearVeiculoSemEspelhamentoTelaCarga");
            centroCarregamento.EnviarEmailParaTransportadorAoDisponibilizarCarga = Request.GetBoolParam("EnviarEmailParaTransportadorAoDisponibilizarCarga");
            centroCarregamento.NaoPermitirAlterarDataCarregamentoCarga = Request.GetBoolParam("NaoPermitirAlterarDataCarregamentoCarga");
            centroCarregamento.PermiteTransportadorSelecionarHorarioCarregamento = Request.GetBoolParam("PermiteTransportadorSelecionarHorarioCarregamento");
            centroCarregamento.IntervaloSelecaoHorarioCarregamentoTransportador = Request.GetIntParam("IntervaloSelecaoHorarioCarregamentoTransportador");
            centroCarregamento.TempoMaximoModificarHorarioCarregamentoTransportador = Request.GetIntParam("TempoMaximoModificarHorarioCarregamentoTransportador");
            centroCarregamento.ToleranciaDataRetroativa = Request.GetIntParam("ToleranciaDataRetroativa");
            centroCarregamento.LimiteAlteracoesHorarioTransportador = Request.GetIntParam("LimiteAlteracoesHorarioTransportador");
            centroCarregamento.BloquearComponentesDeFreteJanelaCarregamentoTransportador = Request.GetBoolParam("BloquearComponentesDeFreteJanelaCarregamentoTransportador");
            centroCarregamento.ExibirNotasFiscaisJanelaCarregamentoTransportador = Request.GetBoolParam("ExibirNotasFiscaisJanelaCarregamentoTransportador");
            centroCarregamento.BloquearTrocaDataListaHorarios = Request.GetBoolParam("BloquearTrocaDataListaHorarios");
            centroCarregamento.ExibirDadosAvancadosJanelaCarregamento = Request.GetBoolParam("ExibirDadosAvancadosJanelaCarregamento");
            centroCarregamento.PermitirTransportadorImprimirOrdemColeta = Request.GetBoolParam("PermitirTransportadorImprimirOrdemColeta");
            centroCarregamento.RetornarJanelaCarregamentoParaAgLiberacaoParaTransportadoresAposRejeicaoDoTransportador = Request.GetBoolParam("RetornarJanelaCarregamentoParaAgLiberacaoParaTransportadoresAposRejeicaoDoTransportador");
            centroCarregamento.LimiteCargasPorMotoristaPorDia = Request.GetIntParam("LimiteCargasPorMotoristaPorDia");
            centroCarregamento.LimiteDeCargasAtivasPorMotorista = Request.GetIntParam("LimiteDeCargasAtivasPorMotorista");
            centroCarregamento.LimiteCargasPorVeiculoPorDia = Request.GetIntParam("LimiteCargasPorVeiculoPorDia");
            centroCarregamento.ExigirTermoAceiteTransporte = Request.GetBoolParam("ExigirTermoAceiteTransporte");
            centroCarregamento.PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador = Request.GetBoolParam("PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador");
            centroCarregamento.ExigirTransportadorConfirmarMDFeNaoEncerradoForaDoSistema = Request.GetBoolParam("ExigirTransportadorConfirmarMDFeNaoEncerradoForaDoSistema");
            centroCarregamento.ExigirTransportadorInformarMotivoAoRejeitarCarga = Request.GetBoolParam("ExigirTransportadorInformarMotivoAoRejeitarCarga");
            centroCarregamento.NotificarSomenteAlteracaoCotacao = Request.GetBoolParam("NotificarSomenteAlteracaoCotacao");
            centroCarregamento.NaoEnviarNotificacaoCargaRejeitadaParaTransportador = Request.GetBoolParam("NaoEnviarNotificacaoCargaRejeitadaParaTransportador");
            centroCarregamento.TermoAceite = Request.GetStringParam("TermoAceite");
            centroCarregamento.CamposVisiveisTransportador = Request.GetStringParam("CamposVisiveisTransportador");
            centroCarregamento.CamposVisiveisJanela = Request.GetStringParam("CamposVisiveisJanela");
            centroCarregamento.HabilitarTermoChegadaHorario = Request.GetBoolParam("HabilitarTermoChegadaHorario");
            centroCarregamento.TermoChegadaHorario = Request.GetStringParam("TermoChegadaHorario");
            centroCarregamento.HorasTrabalho = Request.GetIntParam("HorasTrabalho");
            centroCarregamento.ExibirFilialJanelaCarregamentoTransportador = Request.GetBoolParam("ExibirFilialJanelaCarregamentoTransportador");
            centroCarregamento.NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos = Request.GetBoolParam("NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos");
            centroCarregamento.PermiteTransportadorVisualizarColocacaoDentreLancesLeilao = Request.GetBoolParam("PermiteTransportadorVisualizarColocacaoDentreLancesLeilao");
            centroCarregamento.AtivarRegraParaOfertarCarga = Request.GetBoolParam("AtivarRegraParaOfertarCarga");
            centroCarregamento.ExigirConfirmacaoParticipacaoLeilao = Request.GetBoolParam("ExigirConfirmacaoParticipacaoLeilao");
            centroCarregamento.PermitirQueTransportadorAltereHorarioDoCarregamento = Request.GetBoolParam("PermitirQueTransportadorAltereHorarioDoCarregamento");
            centroCarregamento.EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente = Request.GetBoolParam("EscolherPrimeiroTransportadorAoMarcarInteresseAutomaticamente");
            centroCarregamento.DiasAtrasoPermitidosPedidosAgendamentoPedidos = Request.GetIntParam("DiasAtrasoPermitidosPedidosAgendamentoPedidos");
            centroCarregamento.NaoBloquearCapacidadeExcedida = Request.GetBoolParam("NaoBloquearCapacidadeExcedida");
            centroCarregamento.ObservacaoRetira = Request.GetStringParam("ObservacaoRetira");
            centroCarregamento.MensagemConfirmacaoLeilao = Request.GetStringParam("MensagemConfirmacaoLeilao");
            centroCarregamento.PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados = Request.GetBoolParam("PermiteTransportadorEnviarIntegracaoGRDocumentosReprovados");
            centroCarregamento.LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota = Request.GetBoolParam("LiberarJanelaCarregamentoSomenteComAgendamentoRealizadoClienteAgendadoSemNota");
            centroCarregamento.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido = Request.GetBoolParam("ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido");
            centroCarregamento.PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento = Request.GetBoolParam("PegarObrigatoriamenteHorarioDaPrimeiraColetaParaDataDeCarregamento");
            centroCarregamento.ConsiderarPesoPalletPesoTotalCarga = Request.GetBoolParam("ConsiderarPesoPalletPesoTotalCarga");
            centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga = Request.GetBoolParam("PreencherAutomaticamenteDadosCentroTelaMontagemCarga");
            centroCarregamento.NaoGerarCarregamentoForaCapacidadeModeloVeicularCarga = Request.GetBoolParam("NaoGerarCarregamentoForaCapacidadeModeloVeicularCarga");
            centroCarregamento.NaoPermitirAgendarCargasNoMesmoDia = Request.GetBoolParam("NaoPermitirAgendarCargasNoMesmoDia");
            centroCarregamento.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado = Request.GetBoolParam("EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado");
            centroCarregamento.ExigirChecklistAoConfirmarDadosTransporteMultiTransportador = Request.GetBoolParam("ExigirChecklistAoConfirmarDadosTransporteMultiTransportador");
            centroCarregamento.GerarControleVisualizacaoTransportadorasTerceiros = Request.GetBoolParam("GerarControleVisualizacaoTransportadorasTerceiros");
            centroCarregamento.TipoTransportadorTerceiroSecundario = Request.GetNullableEnumParam<TipoTransportadorCentroCarregamento>("TipoTransportadorTerceiroSecundarioCentroCarregamento");
            centroCarregamento.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente = Request.GetIntParam("TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente");
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroCarregamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroCarregamento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Todos),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                SomenteCentrosOperadorLogistica = Request.GetBoolParam("SomenteCentrosOperadorLogistica"),
                SomenteCentrosManobra = Request.GetBoolParam("SomenteCentrosManobra")
            };

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
            if (filtrosPesquisa.SomenteCentrosOperadorLogistica && !(operadorLogistica?.CentrosCarregamento.Any() ?? true))
                filtrosPesquisa.SomenteCentrosOperadorLogistica = false;

            filtrosPesquisa.CodigoOperadorLogistica = operadorLogistica?.Codigo ?? 0;

            int codigoFilial = Request.GetIntParam("Filial");

            filtrosPesquisa.CodigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return propriedadeOrdenar += ".Descricao";

            return propriedadeOrdenar;
        }

        private void SalvarNotificacoes(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

            List<int> codigosUsuarios = Request.GetListParam<int>("UsuariosNotificacao");
            List<Dominio.Entidades.Usuario> usuarios = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), "");

            if (centroCarregamento.UsuariosNotificacao == null)
                centroCarregamento.UsuariosNotificacao = new List<Dominio.Entidades.Usuario>();

            centroCarregamento.UsuariosNotificacao.Clear();

            foreach (Dominio.Entidades.Usuario usuarioNotificacao in usuarios)
                centroCarregamento.UsuariosNotificacao.Add(usuarioNotificacao);
        }

        private void SetarTiposCarga(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            dynamic tiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCarga"));

            centroCarregamento.TiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            foreach (var tipoCarga in tiposCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaSalvar = repositorioTipoDeCarga.BuscarPorCodigo(((string)tipoCarga.Codigo).ToInt());
                centroCarregamento.TiposCarga.Add(tipoCargaSalvar);
            }
        }

        private void SetarTiposCargaBloquearLiberacaoAutomaticaParaTransportadoras(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            dynamic tiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTipoCargaBloquearLiberacaoAutomaticaParaTransportadoras"));

            centroCarregamento.TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            foreach (var tipoCarga in tiposCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaSalvar = repositorioTipoDeCarga.BuscarPorCodigo(((string)tipoCarga.Codigo).ToInt());
                centroCarregamento.TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras.Add(tipoCargaSalvar);
            }
        }

        private void SetarTransportadoresAutorizadosLiberarFaturamento(ref Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            dynamic transportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TransportadoresAutorizadosLiberarFaturamento"));

            centroCarregamento.TransportadoresAutorizadosLiberarFaturamento = new List<Dominio.Entidades.Empresa>();

            foreach (var transportador in transportadores)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo((int)transportador.Codigo);
                centroCarregamento.TransportadoresAutorizadosLiberarFaturamento.Add(empresa);
            }
        }

        private void SetarVeiculos(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            var repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Veiculos"));

            centroCarregamento.Veiculos = new List<Dominio.Entidades.Veiculo>();

            foreach (var veiculo in veiculos)
            {
                var veiculoAdicionar = repositorioVeiculo.BuscarPorCodigo((int)veiculo.Codigo) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.VeiculoNaoEncontrado);
                var centroCarregamentoVeiculoDuplicado = repositorioCentroCarregamento.BuscarPorVeiculo(centroCarregamento.Codigo, veiculoAdicionar.Codigo);

                if (centroCarregamentoVeiculoDuplicado != null)
                    throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.VeiculoComPlacaJaEstaAdicionadoNoCentroDeCarregamento, veiculoAdicionar.Placa, centroCarregamentoVeiculoDuplicado.Descricao));

                centroCarregamento.Veiculos.Add(veiculoAdicionar);
            }
        }

        private void SalvarTemposCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.TempoCarregamento repTempoCarregamento = new Repositorio.Embarcador.Logistica.TempoCarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
            dynamic temposCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TemposCarregamento"));

            if (centroCarregamento.TemposCarregamento != null && centroCarregamento.TemposCarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var tempoCarregamento in temposCarregamento)
                {
                    int? codigo = ((string)tempoCarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento> temposCarregamentoDeletar = (from obj in centroCarregamento.TemposCarregamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < temposCarregamentoDeletar.Count; i++)
                    repTempoCarregamento.Deletar(temposCarregamentoDeletar[i], historico != null ? Auditado : null, historico);
            }
            else
                centroCarregamento.TemposCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>();

            foreach (var tempoCarregamento in temposCarregamento)
            {
                int? codigo = ((string)tempoCarregamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.TempoCarregamento tempo = codigo.HasValue ? repTempoCarregamento.BuscarPorCodigo(codigo.Value, true) : null;

                if (tempo == null)
                    tempo = new Dominio.Entidades.Embarcador.Logistica.TempoCarregamento();

                tempo.CentroCarregamento = centroCarregamento;
                tempo.HoraInicio = ((string)tempoCarregamento.HoraInicio).ToNullableTime();
                tempo.HoraTermino = ((string)tempoCarregamento.HoraTermino).ToNullableTime();
                tempo.ModeloVeicular = repModeloVeicular.BuscarPorCodigo(((string)tempoCarregamento.CodigoModeloVeicular).ToInt());
                tempo.TipoCarga = repTipoCarga.BuscarPorCodigo(((string)tempoCarregamento.CodigoTipoCarga).ToInt());
                tempo.Tempo = ((string)tempoCarregamento.Tempo).ToInt();
                tempo.QuantidadeMaximaEntregasRoteirizar = ((string)tempoCarregamento.QuantidadeMaximaEntregasRoteirizar).ToInt();
                tempo.QuantidadeVagasOcuparGradeNaCarregamento = ((string)tempoCarregamento.QuantidadeVagasOcuparGradeNaCarregamento).ToInt();
                tempo.QuantidadeMinimaEntregasRoteirizar = ((string)tempoCarregamento.QuantidadeMinimaEntregasRoteirizar).ToInt();

                if (tempo.Codigo > 0)
                    repTempoCarregamento.Atualizar(tempo, historico != null ? Auditado : null, historico);
                else
                    repTempoCarregamento.Inserir(tempo, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarEmails(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoEmail repEmail = new Repositorio.Embarcador.Logistica.CentroCarregamentoEmail(unidadeDeTrabalho);

            centroCarregamento.EnviarNotificacoesPorEmail = Request.GetBoolParam("EnviarNotificacoesPorEmail");
            centroCarregamento.EnviarNotificacoesCargasRejeitadasPorEmail = Request.GetBoolParam("EnviarNotificacoesCargasRejeitadasPorEmail");
            centroCarregamento.EnviarEmailAlertaLeilaoParaTransportadorOfertado = Request.GetBoolParam("EnviarEmailAlertaLeilaoParaTransportadorOfertado");
            centroCarregamento.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente = Request.GetBoolParam("EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente");

            dynamic emails = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Emails"));

            if (centroCarregamento.Emails != null && centroCarregamento.Emails.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var email in emails)
                    if (email.Codigo != null)
                        codigos.Add((int)email.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail> emailsDeletar = (from obj in centroCarregamento.Emails where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < emailsDeletar.Count; i++)
                    repEmail.Deletar(emailsDeletar[i], historico != null ? Auditado : null, historico);
            }
            else
            {
                centroCarregamento.Emails = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail>();
            }

            foreach (var email in emails)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail emailAdd = email.Codigo != null ? repEmail.BuscarPorCodigo((int)email.Codigo, true) : null;

                if (emailAdd == null)
                    emailAdd = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail();

                emailAdd.CentroCarregamento = centroCarregamento;
                emailAdd.Email = (string)email.Email;

                if (emailAdd.Codigo > 0)
                    repEmail.Atualizar(emailAdd, historico != null ? Auditado : null, historico);
                else
                    repEmail.Inserir(emailAdd, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarLimiteCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento repositorioLimiteCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
            dynamic limitesCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LimitesCarregamento"));

            if (centroCarregamento.LimitesCarregamento?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var limiteCarregamento in limitesCarregamento)
                {
                    int? codigo = ((string)limiteCarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento> limitesCarregamentoDeletar = (from o in centroCarregamento.LimitesCarregamento where !codigos.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento limiteCarregamento in limitesCarregamentoDeletar)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, $"Excluído o limite de carregamento de {limiteCarregamento.TipoCarga?.Descricao ?? string.Empty} de {limiteCarregamento.Dia.ObterDescricaoResumida()}", unidadeDeTrabalho);
                    repositorioLimiteCarregamento.Deletar(limiteCarregamento, historico != null ? Auditado : null, historico);
                }
            }
            else
                centroCarregamento.LimitesCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento>();

            foreach (var limiteCarregamento in limitesCarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento limiteCarregamentoSalvar;
                int? codigo = ((string)limiteCarregamento.Codigo).ToNullableInt();

                if (codigo.HasValue && codigo.Value > 0)
                {
                    limiteCarregamentoSalvar = repositorioLimiteCarregamento.BuscarPorCodigo(codigo.Value, true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.LimiteDeCarregamentoNaoEncontrado);

                    if (limiteCarregamentoSalvar.Codigo > 0)
                        limiteCarregamentoSalvar.Initialize();
                }
                else
                    limiteCarregamentoSalvar = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento();

                limiteCarregamentoSalvar.CentroCarregamento = centroCarregamento;
                limiteCarregamentoSalvar.Dia = ((string)limiteCarregamento.DiaSemana).ToEnum<DiaSemana>();
                limiteCarregamentoSalvar.DiasAntecedencia = ((string)limiteCarregamento.DiasAntecedencia).ToInt();
                limiteCarregamentoSalvar.HoraLimite = ((string)limiteCarregamento.HoraLimite).ToTime();
                limiteCarregamentoSalvar.TipoCarga = repositorioTipoCarga.BuscarPorCodigo(((string)limiteCarregamento.TipoCarga.Codigo).ToInt()) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TipoDeCargaDoLimiteDeCarregamentoNaoEncontrado);

                if (limiteCarregamentoSalvar.Codigo > 0)
                {
                    if (limiteCarregamentoSalvar.IsChanged())
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, limiteCarregamentoSalvar.GetChanges(), $"Alterado o limite de carregamento de {limiteCarregamentoSalvar.TipoCarga?.Descricao ?? string.Empty} de {limiteCarregamentoSalvar.Dia.ObterDescricaoResumida()}", unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                    repositorioLimiteCarregamento.Atualizar(limiteCarregamentoSalvar, historico != null ? Auditado : null, historico);
                }
                else
                    repositorioLimiteCarregamento.Inserir(limiteCarregamentoSalvar, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarConfiguracaoLances(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.LancesCarregamento repositorioLancesCarregamento = new Repositorio.Embarcador.Logistica.LancesCarregamento(unidadeDeTrabalho);
            dynamic lancesCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Lances"));

            List<int> codigos = new List<int>();
            List<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento> lancesAdicionar = new List<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento>();

            foreach (var lanceCarregamento in lancesCarregamento)
            {
                int? codigo = ((string)lanceCarregamento.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    codigos.Add(codigo.Value);
                else
                {
                    lancesAdicionar.Add(new Dominio.Entidades.Embarcador.Logistica.LancesCarregamento()
                    {
                        CentroCarregamento = centroCarregamento,
                        NumeroLanceDe = ((string)lanceCarregamento.NumeroLanceDe).ToInt(),
                        NumeroLanceAte = ((string)lanceCarregamento.NumeroLanceAte).ToInt(),
                        PorcentagemLance = ((string)lanceCarregamento.PorcentagemLance).ToDecimal(),
                    });
                }
            }

            List<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento> lancesDeletar = repositorioLancesCarregamento.BuscarListaDeletarPorCentroCarregamento(centroCarregamento.Codigo, codigos);

            foreach (Dominio.Entidades.Embarcador.Logistica.LancesCarregamento lance in lancesDeletar)
                repositorioLancesCarregamento.Deletar(lance, historico != null ? Auditado : null, historico);

            foreach (Dominio.Entidades.Embarcador.Logistica.LancesCarregamento lance in lancesAdicionar)
                repositorioLancesCarregamento.Inserir(lance, historico != null ? Auditado : null, historico);
        }

        private void SalvarOfertaCarga(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoOfertaCarga repOfertaCarga = new Repositorio.Embarcador.Logistica.CentroCarregamentoOfertaCarga(unidadeDeTrabalho);
            dynamic ofertasCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OfertaCarga"));
            bool periodoDiferenciadoShare = Request.GetBoolParam("PeriodoDiferenciadoShare");
            DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicialPeriodoDiferenciadoShare");
            DateTime? dataFinal = Request.GetNullableDateTimeParam("DataInicialPeriodoDiferenciadoShare");
            if (periodoDiferenciadoShare)
            {
                if (!dataInicial.HasValue)
                    throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.NecessarioInformarDataInicialPeriodoShare);
                if (!dataFinal.HasValue)
                    throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.NecessarioInformarDataInicialPeriodoShare);
            }

            List<int> codigos = new List<int>();
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga> ofertasCargaAdicionar = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga>();

            foreach (var ofertaCarga in ofertasCarga)
            {
                int? codigo = ((string)ofertaCarga.Codigo).ToNullableInt();

                if (codigo.HasValue)
                {
                    codigos.Add(codigo.Value);
                }
                else
                {
                    ofertasCargaAdicionar.Add(new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga()
                    {
                        CentroCarregamento = centroCarregamento,
                        Regra = ofertaCarga.RegraValue,
                        Prioridade = ofertaCarga.Prioridade,
                        PeriodoDiferenciadoShare = periodoDiferenciadoShare,
                        DataInicialPeriodoDiferenciadoShare = dataInicial,
                        DataFinalPeriodoDiferenciadoShare = dataFinal
                    });
                }
            }

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga> ofertasCargaDeletar = repOfertaCarga.BuscarListaDeletarPorCentroCarregamento(centroCarregamento.Codigo, codigos);

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga ofertaCarga in ofertasCargaDeletar)
                repOfertaCarga.Deletar(ofertaCarga, historico != null ? Auditado : null, historico);

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga ofertaCarga in ofertasCargaAdicionar)
                repOfertaCarga.Inserir(ofertaCarga, historico != null ? Auditado : null, historico); ;

            foreach (var ofertaCarga in ofertasCarga)
            {
                int? codigo = ((string)ofertaCarga.Codigo).ToNullableInt();

                if (codigo.HasValue)
                {
                    if (!ofertasCargaAdicionar.Select(x => x.Codigo).Contains(codigo.Value) && !ofertasCargaDeletar.Select(x => x.Codigo).Contains(codigo.Value))
                    {
                        var ofertaAtualizar = repOfertaCarga.BuscarPorCodigo(codigo.Value, true);

                        if (ofertaAtualizar != null)
                        {
                            ofertaAtualizar.PeriodoDiferenciadoShare = periodoDiferenciadoShare;
                            ofertaAtualizar.DataInicialPeriodoDiferenciadoShare = dataInicial;
                            ofertaAtualizar.DataFinalPeriodoDiferenciadoShare = dataFinal;

                            repOfertaCarga.Atualizar(ofertaAtualizar, historico != null ? Auditado : null, historico);
                        }

                    }
                }
            }
        }

        private void SalvarPeriodosCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unidadeDeTrabalho);
            dynamic periodosCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PeriodosCarregamento"));

            if (centroCarregamento.PeriodosCarregamento != null && centroCarregamento.PeriodosCarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var periodoCarregamento in periodosCarregamento)
                    if (periodoCarregamento.Codigo != null)
                        codigos.Add((int)periodoCarregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoDeletar = (from obj in centroCarregamento.PeriodosCarregamento where !codigos.Contains(obj.Codigo) && obj.ExcecaoCapacidadeCarregamento == null && obj.ExclusividadeCarregamento == null select obj).ToList();

                for (var i = 0; i < periodosCarregamentoDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, $"Excluído o período de carregamento de {periodosCarregamentoDeletar[i].HoraInicio.ToString(@"hh\:mm")} às {periodosCarregamentoDeletar[i].HoraTermino.ToString(@"hh\:mm")} de {periodosCarregamentoDeletar[i].DiaSemana}", unidadeDeTrabalho);
                    repPeriodoCarregamento.Deletar(periodosCarregamentoDeletar[i], historico != null ? Auditado : null, historico);
                }
            }
            else
                centroCarregamento.PeriodosCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            foreach (var periodoCarregamento in periodosCarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo = periodoCarregamento.Codigo != null ? repPeriodoCarregamento.BuscarPorCodigo((int)periodoCarregamento.Codigo, true) : null;

                if (periodo == null)
                    periodo = new Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento();
                else
                    periodo.Initialize();

                periodo.CentroCarregamento = centroCarregamento;
                periodo.CapacidadeCarregamentoVolume = ((string)periodoCarregamento.CapacidadeCarregamentoVolume).ToInt();
                periodo.CapacidadeCarregamentoCubagem = ((string)periodoCarregamento.CapacidadeCarregamentoCubagem).ToInt();
                periodo.CapacidadeCarregamentoSimultaneo = ((string)periodoCarregamento.CapacidadeCarregamentoSimultaneo).ToInt();
                periodo.Dia = (DiaSemana)periodoCarregamento.DiaSemana;
                periodo.HoraInicio = TimeSpan.ParseExact((string)periodoCarregamento.HoraInicio, "g", null, System.Globalization.TimeSpanStyles.None);
                periodo.HoraTermino = TimeSpan.ParseExact((string)periodoCarregamento.HoraTermino, "g", null, System.Globalization.TimeSpanStyles.None);
                periodo.ToleranciaExcessoTempo = ((string)periodoCarregamento.ToleranciaExcessoTempo).ToInt();

                if (periodo.Codigo > 0)
                {
                    if (periodo.IsChanged())
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, periodo.GetChanges(), $"Alterado o periodo de carregamento de {periodo.HoraInicio.ToString(@"hh\:mm")} às {periodo.HoraTermino.ToString(@"hh\:mm")} de {periodo.DiaSemana}", unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                    repPeriodoCarregamento.Atualizar(periodo, historico != null ? Auditado : null, historico);
                }
                else
                    repPeriodoCarregamento.Inserir(periodo, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarPrevisoesCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PrevisaoCarregamento repPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unidadeDeTrabalho);
            dynamic previsoesCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PrevisoesCarregamento"));

            if (centroCarregamento.PrevisoesCarregamento != null && centroCarregamento.PrevisoesCarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var previsaoCarregamento in previsoesCarregamento)
                    if (previsaoCarregamento.Codigo != null)
                        codigos.Add((int)previsaoCarregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> previsoesCarregamentoDeletar = (from obj in centroCarregamento.PrevisoesCarregamento where !codigos.Contains(obj.Codigo) && obj.ExcecaoCapacidadeCarregamento == null select obj).ToList();

                for (var i = 0; i < previsoesCarregamentoDeletar.Count; i++)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> reservasCargaGrupoPessoa = repReservaCargaGrupoPessoa.BuscarPorPrevisao(previsoesCarregamentoDeletar[i].Codigo);
                    foreach (Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reservaCargaGrupoPessoa in reservasCargaGrupoPessoa)
                        repReservaCargaGrupoPessoa.Deletar(reservaCargaGrupoPessoa);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, $"Excluído a previsão de carregamento {previsoesCarregamentoDeletar[i].Descricao} de {previsoesCarregamentoDeletar[i].Dia.ObterDescricaoResumida()}", unidadeDeTrabalho);
                    repPrevisaoCarregamento.Deletar(previsoesCarregamentoDeletar[i], historico != null ? Auditado : null, historico);
                }
            }
            else
                centroCarregamento.PrevisoesCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            foreach (var previsaoCarregamento in previsoesCarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsao = previsaoCarregamento.Codigo != null ? repPrevisaoCarregamento.BuscarPorCodigo((int)previsaoCarregamento.Codigo, true) : null;

                if (previsao == null)
                    previsao = new Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento();
                else
                    previsao.Initialize();

                previsao.CentroCarregamento = centroCarregamento;
                previsao.Rota = new Dominio.Entidades.RotaFrete() { Codigo = (int)previsaoCarregamento.Rota.Codigo };
                previsao.QuantidadeCargas = (int)previsaoCarregamento.QuantidadeCargas;
                previsao.QuantidadeCargasExcedentes = (int)previsaoCarregamento.QuantidadeCargasExcedentes;
                previsao.Dia = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)previsaoCarregamento.DiaSemana;
                previsao.Descricao = (string)previsaoCarregamento.Descricao;

                previsao.ModelosVeiculos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                var modelosVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(previsaoCarregamento.ModelosVeiculos.ToString());

                foreach (var modeloVeiculo in modelosVeiculos)
                    previsao.ModelosVeiculos.Add(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modeloVeiculo.Codigo });

                if (previsao.Codigo > 0)
                {
                    if (previsao.IsChanged())
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, previsao.GetChanges(), $"Alterado a previsão de carregamento {previsao.Descricao} de {previsao.Dia.ObterDescricaoResumida()}", unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                    repPrevisaoCarregamento.Atualizar(previsao, historico != null ? Auditado : null, historico);
                }
                else
                    repPrevisaoCarregamento.Inserir(previsao, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarDisponibilidadeFrota(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota repCentroCarregamentoDisponibilidadeFrota = new Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            dynamic disponibilidadesFrota = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DisponibilidadesFrota"));

            if (centroCarregamento != null)
            {
                if (centroCarregamento.DisponibilidadesFrota != null && centroCarregamento.DisponibilidadesFrota.Count > 0)
                {
                    List<int> codigos = new List<int>();

                    foreach (var disponibilidadeFrota in disponibilidadesFrota)
                        if (disponibilidadeFrota.Codigo != null)
                            codigos.Add((int)disponibilidadeFrota.Codigo);

                    List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadesFrotaDeletar = (from obj in centroCarregamento.DisponibilidadesFrota where !codigos.Contains(obj.Codigo) select obj).ToList();

                    for (var i = 0; i < disponibilidadesFrotaDeletar.Count; i++)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, $"Excluído a disponibilidade da frota {disponibilidadesFrotaDeletar[i].ModeloVeicular?.Descricao ?? string.Empty} de {disponibilidadesFrotaDeletar[i].Dia.ObterDescricaoResumida()}", unidadeDeTrabalho);
                        repCentroCarregamentoDisponibilidadeFrota.Deletar(disponibilidadesFrotaDeletar[i], historico != null ? Auditado : null, historico);
                    }
                }
                else
                {
                    centroCarregamento.DisponibilidadesFrota = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>();
                }
            }

            foreach (var disponibilidadeFrota in disponibilidadesFrota)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota disponibilidade = disponibilidadeFrota.Codigo != null ? repCentroCarregamentoDisponibilidadeFrota.BuscarPorCodigo((int)disponibilidadeFrota.Codigo, true) : null;

                if (disponibilidade == null)
                    disponibilidade = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota();
                else
                    disponibilidade.Initialize();

                disponibilidade.CentroCarregamento = centroCarregamento;
                disponibilidade.Dia = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)disponibilidadeFrota.DiaSemana;
                disponibilidade.Quantidade = disponibilidadeFrota.Quantidade;
                disponibilidade.ModeloVeicular = repModeloVeicular.BuscarPorCodigo((int)disponibilidadeFrota.ModeloVeicular.Codigo);
                if (disponibilidadeFrota.Transportador?.Codigo > 0)
                    disponibilidade.Transportador = repEmpresa.BuscarPorCodigo((int)disponibilidadeFrota.Transportador.Codigo);

                if (disponibilidade.Codigo > 0)
                {
                    if (disponibilidade.IsChanged())
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, disponibilidade.GetChanges(), $"Alterado a disponibilidade da frota {disponibilidade.ModeloVeicular?.Descricao ?? string.Empty} de {disponibilidade.Dia.ObterDescricaoResumida()}", unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                    repCentroCarregamentoDisponibilidadeFrota.Atualizar(disponibilidade, historico != null ? Auditado : null, historico);
                }
                else
                    repCentroCarregamentoDisponibilidadeFrota.Inserir(disponibilidade, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarProdutividade(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoProdutividade repositorioCentroCarregamentoProdutividade = new Repositorio.Embarcador.Logistica.CentroCarregamentoProdutividade(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            dynamic dynProdutividadeCarregamentos = JsonConvert.DeserializeObject<dynamic>(Request.Params("ProdutividadeCarregamentos"));
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade> centroCarregamentoProdutividades = repositorioCentroCarregamentoProdutividade.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo);
            bool possuiProdutividade = false;

            if (centroCarregamentoProdutividades.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centroCarregamentoProdutividade in dynProdutividadeCarregamentos)
                {
                    int codigo = ((string)centroCarregamentoProdutividade.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade> deletar = (from obj in centroCarregamentoProdutividades where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < deletar.Count; i++)
                    repositorioCentroCarregamentoProdutividade.Deletar(deletar[i], Auditado);
            }

            foreach (dynamic centroCarregamentoProdutividade in dynProdutividadeCarregamentos)
            {
                int codigo = ((string)centroCarregamentoProdutividade.Codigo).ToInt();
                int codigoGrupoPessoas = ((string)centroCarregamentoProdutividade.GrupoPessoas.Codigo).ToInt();
                int codigoTipoOperacao = ((string)centroCarregamentoProdutividade.TipoOperacao.Codigo).ToInt();
                int codigoTransportador = ((string)centroCarregamentoProdutividade.Transportador.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade centroCarregamentoProdutividadeEntidade = codigo > 0 ? repositorioCentroCarregamentoProdutividade.BuscarPorCodigo(codigo, true) : null;

                if (centroCarregamentoProdutividadeEntidade == null)
                    centroCarregamentoProdutividadeEntidade = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade();

                centroCarregamentoProdutividadeEntidade.CentroCarregamento = centroCarregamento;
                centroCarregamentoProdutividadeEntidade.GrupoPessoas = repositorioGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
                centroCarregamentoProdutividadeEntidade.TipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
                centroCarregamentoProdutividadeEntidade.Transportador = repositorioEmpresa.BuscarPorCodigo(codigoTransportador);
                centroCarregamentoProdutividadeEntidade.Picking = ((string)centroCarregamentoProdutividade.Picking).ToInt();
                centroCarregamentoProdutividadeEntidade.Separacao = ((string)centroCarregamentoProdutividade.Separacao).ToInt();
                centroCarregamentoProdutividadeEntidade.Carregamento = ((string)centroCarregamentoProdutividade.Carregamento).ToInt();
                centroCarregamentoProdutividadeEntidade.HorasTrabalho = ((string)centroCarregamentoProdutividade.HorasTrabalho).ToInt();

                if (centroCarregamentoProdutividadeEntidade.Codigo > 0)
                    repositorioCentroCarregamentoProdutividade.Atualizar(centroCarregamentoProdutividadeEntidade, Auditado);
                else
                    repositorioCentroCarregamentoProdutividade.Inserir(centroCarregamentoProdutividadeEntidade, Auditado);

                possuiProdutividade = true;
            }

            if (possuiProdutividade && (centroCarregamento.HorasTrabalho <= 0))
                throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.HorasTrabalhoNaoInformadaParaProdutividadesCadastradas);
        }

        private void SalvarPunicao(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            var punicoesCarregamento = JsonConvert.DeserializeObject<dynamic>(Request.Params("PunicoesCarregamento"));

            if (punicoesCarregamento == null || punicoesCarregamento.Count == 0)
                return;

            var repositorio = new Repositorio.Embarcador.Logistica.CentroCarregamentoPunicao(unitOfWork);
            var punicoesExistentes = repositorio.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo);
            var punicoesSalvar = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao>();

            foreach (var item in punicoesCarregamento)
            {
                var punicaoItem = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao()
                {
                    Codigo = ((string)item.Codigo).ToInt(),
                    TipoFrota = (TipoFrota?)((string)item.TipoFrota).ToInt(),
                    TempoPunicao = (decimal)item.HorasPunicao
                };

                punicoesSalvar.Add(punicaoItem);
            }

            // Se existir, remove as punições que não estão na lista
            if (punicoesExistentes.Count > 0)
            {
                var codigos = new List<int>();

                foreach (dynamic pc in punicoesCarregamento)
                    codigos.Add(((string)pc.Codigo).ToInt());

                var deletarRemovidos = punicoesExistentes.Where(pe => !codigos.Contains(pe.Codigo)).ToList();

                foreach (var del in deletarRemovidos)
                    repositorio.Deletar(del, null);
            }

            foreach (var punicao in punicoesSalvar)
            {
                var entidade = repositorio.BuscarPorCodigo(punicao.Codigo, false) ?? new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao();

                entidade.CentroCarregamento = centroCarregamento;
                entidade.TipoFrota = punicao.TipoFrota;
                entidade.TempoPunicao = punicao.TempoPunicao;

                if (entidade.Codigo == 0)
                    repositorio.Inserir(entidade, null);
                else
                    repositorio.Atualizar(entidade, null);
            }

        }

        private void SalvarConfiguracaoPadrao(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (centroCarregamento.PreencherAutomaticamenteDadosCentroTelaMontagemCarga)
            {
                int.TryParse(Request.Params("EmpresaPadrao"), out int empresaPadrao);
                int.TryParse(Request.Params("VeiculoPadrao"), out int veiculoPadrao);
                int.TryParse(Request.Params("ModeloVeicularPadrao"), out int modeloVeiculoPadrao);
                int.TryParse(Request.Params("TipoOperacaoPadrao"), out int tipoOperacaoPadrao);
                int.TryParse(Request.Params("MotoristaPadrao"), out int motoristaPadrao);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamentoConfigPadrao repConfigPadrao = new Repositorio.Embarcador.Logistica.CentroCarregamentoConfigPadrao(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoConfigPadrao objConfiguracaoPadrao = centroCarregamento.Codigo > 0 ? repConfigPadrao.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo) ?? new() : new();

                objConfiguracaoPadrao.CentroCarregamento = centroCarregamento;
                objConfiguracaoPadrao.EmpresaPadrao = repEmpresa.BuscarPorCodigo(empresaPadrao);
                objConfiguracaoPadrao.VeiculoPadrao = repVeiculo.BuscarPorCodigo(veiculoPadrao);
                objConfiguracaoPadrao.ModeloVeicularCargaPadrao = repModeloVeicular.BuscarPorCodigo(modeloVeiculoPadrao);
                objConfiguracaoPadrao.TipoOperacaoPadrao = repTipoOperacao.BuscarPorCodigo(tipoOperacaoPadrao);
                objConfiguracaoPadrao.MotoristaPadrao = repMotorista.BuscarPorCodigo(motoristaPadrao);

                var nenhumObjetoPreenchido = objConfiguracaoPadrao.EmpresaPadrao == null &&
                                             objConfiguracaoPadrao.VeiculoPadrao == null &&
                                             objConfiguracaoPadrao.TipoOperacaoPadrao == null &&
                                             objConfiguracaoPadrao.MotoristaPadrao == null;

                if (objConfiguracaoPadrao.Codigo == 0 && !nenhumObjetoPreenchido)
                    repConfigPadrao.Inserir(objConfiguracaoPadrao, null);
                else
                {
                    if (!nenhumObjetoPreenchido)
                        repConfigPadrao.Atualizar(objConfiguracaoPadrao, null);
                    else
                        repConfigPadrao.Deletar(objConfiguracaoPadrao, null);
                }
            }
            else
            {
                Repositorio.Embarcador.Logistica.CentroCarregamentoConfigPadrao repConfigPadrao = new Repositorio.Embarcador.Logistica.CentroCarregamentoConfigPadrao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoConfigPadrao objConfiguracaoPadrao = centroCarregamento.Codigo > 0 ? repConfigPadrao.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo) ?? new() : new();

                if (objConfiguracaoPadrao.Codigo > 0)
                    repConfigPadrao.Deletar(objConfiguracaoPadrao, null);
            }
        }

        #endregion

        #region Métodos Privados de Consulta

        private Dominio.Entidades.Cliente ObterCliente(double cpfCnpjCliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repositorio = new Repositorio.Cliente(unidadeDeTrabalho);

            return repositorio.BuscarPorCPFCNPJ(cpfCnpjCliente) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.ClienteNaoEncontrado);
        }

        private Dominio.Entidades.Localidade ObterLocalidade(int localidade, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repositorio = new Repositorio.Localidade(unidadeDeTrabalho);

            return repositorio.BuscarPorCodigo(localidade) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.LocalidadeNaoEncontrada);
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoDeCarga(int tipoDecarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorio = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            return repositorio.BuscarPorCodigo(tipoDecarga) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TipoDeCargaNaoEncontrado);
        }

        private Dominio.Entidades.Embarcador.Logistica.ManobraAcao ObterManobraAcao(dynamic acaoManobra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            int codigoManobraAcao = ((string)acaoManobra.CodigoManobraAcao).ToInt();

            if (codigoManobraAcao == 0)
                throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraDeveSerInformada);

            return ObterManobraAcao(codigoManobraAcao, unidadeDeTrabalho) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraNaoEncontrada);
        }

        private Dominio.Entidades.Embarcador.Logistica.ManobraAcao ObterManobraAcao(int codigoManobraAcao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.ManobraAcao repositorio = new Repositorio.Embarcador.Logistica.ManobraAcao(unidadeDeTrabalho);

            return repositorio.BuscarPorCodigo(codigoManobraAcao);
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularCarga(int codigoModeloVeicularCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);

            return repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.ModeloVeicularNaoEncontrado);
        }

        private dynamic ObterPeriodoCarregamento(Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dataCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            bool exibirHoraDataCarregamentoEDescarregamento = configuracaoEmbarcador.InformaApoliceSeguroMontagemCarga || configuracaoEmbarcador.InformaHorarioCarregamentoMontagemCarga;
            int capacidadeCarregamentoTotal = 0;
            int capacidadeCarregamentoCubagemTotal = 0;
            int capacidadeDisponivel = 0;

            if (centroCarregamento?.TipoCapacidadeCarregamentoPorPeso == TipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorioCapacidadeCarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(unitOfWork);

                int capacidadeCarregamentoVolume = periodoCarregamento.CapacidadeCarregamentoVolume;
                int capacidadeCarregamentoCubagem = periodoCarregamento.CapacidadeCarregamentoCubagem;

                int capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoPorPeriodo(centroCarregamento.Codigo, dataCarregamento, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);
                int capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarPesoTotalCarregamentoPeriodo(0, centroCarregamento.Codigo, dataCarregamento, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);

                if (centroCarregamento?.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
                    capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoPeriodo(0, centroCarregamento.Codigo, dataCarregamento, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);

                capacidadeCarregamentoTotal = capacidadeCarregamentoVolume + capacidadeCarregamentoAdicional;
                capacidadeCarregamentoCubagemTotal = capacidadeCarregamentoCubagem + capacidadeCarregamentoAdicional;
                capacidadeDisponivel = capacidadeCarregamentoTotal - capacidadeUtilizada;
            }

            return new
            {
                periodoCarregamento.Codigo,
                Descricao = periodoCarregamento.DescricaoPeriodo,
                DataCarregamento = dataCarregamento.ToString("dd/MM/yyyy"),
                DataHoraInicio = dataCarregamento.Date.Add(periodoCarregamento.HoraInicio).ToString("dd/MM/yyyy HH:mm"),
                HoraInicio = periodoCarregamento.HoraInicio.ToString(@"hh\:mm"),
                HoraTermino = periodoCarregamento.HoraTermino.ToString(@"hh\:mm"),
                InicioCarregamento = exibirHoraDataCarregamentoEDescarregamento ? dataCarregamento.Date.Add(periodoCarregamento.HoraInicio).ToString("dd/MM/yyyy HH:mm") : dataCarregamento.Date.Add(periodoCarregamento.HoraInicio).ToString("dd/MM/yyyy"),
                periodoCarregamento.ToleranciaExcessoTempo,
                periodoCarregamento.CapacidadeCarregamentoSimultaneo,
                CapacidadeCarregamentoVolume = (capacidadeCarregamentoTotal > 0) ? capacidadeCarregamentoTotal.ToString("n0") : "",
                CapacidadeCarregamentoCubagem = (capacidadeCarregamentoCubagemTotal > 0) ? capacidadeCarregamentoCubagemTotal.ToString("n0") : "",
                CapacidadeDisponivel = (capacidadeCarregamentoTotal > 0) ? capacidadeDisponivel.ToString("n0") : "",
            };
        }

        private Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ObterProdutoEmbarcador(int codigoProdutoEmbarcador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unidadeDeTrabalho);

            return repositorioProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.ProdutoNaoEncontrado);
        }

        private Dominio.Entidades.Empresa ObterTransportador(int codigoTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repositorio = new Repositorio.Empresa(unidadeDeTrabalho);

            return repositorio.BuscarPorCodigo(codigoTransportador) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TransportadorNaoEncontrado);
        }

        private Dominio.Entidades.Cliente ObterTransportadorTerceiro(double codigoTransportadorTerceiro, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repositorio = new Repositorio.Cliente(unidadeDeTrabalho);

            return repositorio.BuscarPorCPFCNPJ(codigoTransportadorTerceiro) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TransportadorNaoEncontrado);
        }

        #endregion

        #region Métodos Privados - Ações de Manobra

        private void AtualizarAcoesManobra(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic acoesManobra = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("AcoesManobra"));

            ExcluirAcoesManobraRemovidas(centroCarregamento, acoesManobra, unidadeDeTrabalho);
            SalvarAcoesManobraAdicionadasOuAtualizadas(centroCarregamento, acoesManobra, unidadeDeTrabalho);
        }

        private void ExcluirAcoesManobraRemovidas(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic acoesManobra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (centroCarregamento.AcoesManobra != null)
            {
                Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao repositorioCentroCarregamentoManobraAcao = new Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var acaoManobra in acoesManobra)
                {
                    int? codigo = ((string)acaoManobra.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao> listaAcaoManobraRemover = (from manobraAcao in centroCarregamento.AcoesManobra where !listaCodigosAtualizados.Contains(manobraAcao.Codigo) select manobraAcao).ToList();

                foreach (var manobraAcao in listaAcaoManobraRemover)
                {
                    repositorioCentroCarregamentoManobraAcao.Deletar(manobraAcao);
                }

                if (listaAcaoManobraRemover.Count > 0)
                {
                    string descricaoAcao = listaAcaoManobraRemover.Count == 1 ? Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraRemovida : Localization.Resources.Logistica.CentroCarregamento.MultiplasAcoesDeManobraRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosAcaoManobra(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao centroCarregamentoManobraAcao, dynamic acaoManobra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!centroCarregamentoManobraAcao.IsInitialized())
                centroCarregamentoManobraAcao.Acao = ObterManobraAcao(acaoManobra, unidadeDeTrabalho);

            centroCarregamentoManobraAcao.TempoToleranciaInicioManobra = ((string)acaoManobra.TempoToleranciaInicioManobra).ToNullableInt();
        }

        private void SalvarAcoesManobraAdicionadasOuAtualizadas(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic acoesManobra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao repositorioCentroCarregamentoManobraAcao = new Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao> acoesManobraCadastradasOuAtualizadas = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>();
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var acaoManobra in acoesManobra)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao centroCarregamentoManobraAcao;
                int? codigo = ((string)acaoManobra.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    centroCarregamentoManobraAcao = repositorioCentroCarregamentoManobraAcao.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraNaoEncontrada);
                else
                    centroCarregamentoManobraAcao = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao() { CentroCarregamento = centroCarregamento };

                PreencherDadosAcaoManobra(centroCarregamentoManobraAcao, acaoManobra, unidadeDeTrabalho);
                ValidarDadosAcaoManobraDuplicado(acoesManobraCadastradasOuAtualizadas, centroCarregamentoManobraAcao);

                acoesManobraCadastradasOuAtualizadas.Add(centroCarregamentoManobraAcao);

                if (codigo.HasValue)
                {
                    totalRegistrosAtualizados += centroCarregamentoManobraAcao.GetChanges().Count > 0 ? 1 : 0;
                    repositorioCentroCarregamentoManobraAcao.Atualizar(centroCarregamentoManobraAcao);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioCentroCarregamentoManobraAcao.Inserir(centroCarregamentoManobraAcao);
                }
            }

            if (centroCarregamento.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraAtualizada : Localization.Resources.Logistica.CentroCarregamento.MultiplasAcoesDeManobraAtualizadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraAdicionada : Localization.Resources.Logistica.CentroCarregamento.MultiplasAcoesDeManobraAdicionadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void ValidarDadosAcaoManobraDuplicado(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao> acoesManobraCadastradasOuAtualizadas, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao centroCarregamentoManobraAcao)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao centroCarregamentoManobraAcaoDuplicada = (from acaoManobra in acoesManobraCadastradasOuAtualizadas where acaoManobra.Acao.Codigo == centroCarregamentoManobraAcao.Acao.Codigo select acaoManobra).FirstOrDefault();

            if (centroCarregamentoManobraAcaoDuplicada != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraDuplicada, centroCarregamentoManobraAcaoDuplicada.Acao.Descricao));
        }

        #endregion

        #region Métodos Privados - Controles de Expedição

        private void AtualizarControlesExpedicao(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic listaControleExpedicao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ControleExpedicao"));

            ExcluirControlesExpedicaoRemovidos(centroCarregamento, listaControleExpedicao, historico, unidadeDeTrabalho);
            SalvarControlesExpedicaoAdicionadosOuAtualizados(centroCarregamento, listaControleExpedicao, historico, unidadeDeTrabalho);
        }

        private void ExcluirControlesExpedicaoRemovidos(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic listaControleExpedicao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (centroCarregamento.ExpedicoesCarregamento == null)
                return;

            Repositorio.Embarcador.Logistica.ExpedicaoCarregamento repositorioControleExpedicao = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga repositorioControleExpedicaoModeloVeicularCarga = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga(unidadeDeTrabalho);
            List<int> listaCodigosAtualizados = new List<int>();

            foreach (var controleExpedicao in listaControleExpedicao)
            {
                int? codigo = ((string)controleExpedicao.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> listaControleExpedicaoRemover = (from controleExpedicao in centroCarregamento.ExpedicoesCarregamento where !listaCodigosAtualizados.Contains(controleExpedicao.Codigo) select controleExpedicao).ToList();

            foreach (Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento controleExpedicao in listaControleExpedicaoRemover)
            {
                repositorioControleExpedicaoModeloVeicularCarga.DeletarPorControleExpedicao(controleExpedicao.Codigo);
                repositorioControleExpedicao.Deletar(controleExpedicao, historico != null ? Auditado : null, historico);
            }
        }

        private void ExcluirModelosVeicularesCargaExcluisivoRemovidos(Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento controleExpedicao, dynamic listaModelosVeicularesCargaExclusivo, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if ((controleExpedicao.ModelosVeicularesCargaExclusivo == null) || (controleExpedicao.ModelosVeicularesCargaExclusivo.Count == 0))
                return;

            Repositorio.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga repositorioControleExpedicaoModeloVeicularCarga = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga(unidadeDeTrabalho);
            List<int> listaCodigosAtualizados = new List<int>();

            foreach (var modelosVeicularesCargaExclusivo in listaModelosVeicularesCargaExclusivo)
                listaCodigosAtualizados.Add((int)modelosVeicularesCargaExclusivo.Codigo);

            List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga> listaModeloVeicularCargaExclusivoRemover = (from modeloVeicularesCargaExclusivo in controleExpedicao.ModelosVeicularesCargaExclusivo where !listaCodigosAtualizados.Contains(modeloVeicularesCargaExclusivo.ModeloVeicularCarga.Codigo) select modeloVeicularesCargaExclusivo).ToList();

            foreach (var modeloVeicularesCargaExclusivo in listaModeloVeicularCargaExclusivoRemover)
                repositorioControleExpedicaoModeloVeicularCarga.Deletar(modeloVeicularesCargaExclusivo, historico != null ? Auditado : null, historico);
        }

        private void PreencherDadosControleExpedicao(Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento controleExpedicaoSalvar, dynamic controleExpedicao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            controleExpedicaoSalvar.ProdutoEmbarcador = ObterProdutoEmbarcador((int)controleExpedicao.Produto.Codigo, unidadeDeTrabalho);
            controleExpedicaoSalvar.ClienteDestino = ObterCliente((double)controleExpedicao.ClienteDestino.Codigo, unidadeDeTrabalho);
            controleExpedicaoSalvar.Quantidade = ((string)controleExpedicao.Quantidade).ToInt();
            controleExpedicaoSalvar.Dia = ((string)controleExpedicao.DiaSemana).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();
        }

        private void SalvarControlesExpedicaoAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic listaControleExpedicao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.ExpedicaoCarregamento repositorioControleExpedicao = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamento(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> listaControleExpedicaoCadastradoOuAtualizado = new List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento>();

            foreach (var controleExpedicao in listaControleExpedicao)
            {
                Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento controleExpedicaoSalvar;
                int? codigo = ((string)controleExpedicao.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    controleExpedicaoSalvar = repositorioControleExpedicao.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.ControleDeExpedicaoNaoEncontrado);
                else
                    controleExpedicaoSalvar = new Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento() { CentroCarregamento = centroCarregamento };

                PreencherDadosControleExpedicao(controleExpedicaoSalvar, controleExpedicao, unidadeDeTrabalho);
                ValidarDadosControleExpedicaoDuplicado(listaControleExpedicaoCadastradoOuAtualizado, controleExpedicaoSalvar);

                listaControleExpedicaoCadastradoOuAtualizado.Add(controleExpedicaoSalvar);

                if (codigo.HasValue)
                {
                    repositorioControleExpedicao.Atualizar(controleExpedicaoSalvar, historico != null ? Auditado : null, historico);
                    ExcluirModelosVeicularesCargaExcluisivoRemovidos(controleExpedicaoSalvar, controleExpedicao.ModelosVeicularesCargaExclusivo, historico, unidadeDeTrabalho);
                }
                else
                    repositorioControleExpedicao.Inserir(controleExpedicaoSalvar, historico != null ? Auditado : null, historico);

                SalvarModelosVeicularesCargaExclusivoAdicionadosOuAtualizados(controleExpedicaoSalvar, controleExpedicao.ModelosVeicularesCargaExclusivo, historico, unidadeDeTrabalho);
            }
        }

        private void SalvarModelosVeicularesCargaExclusivoAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento controleExpedicao, dynamic listaModelosVeicularesCargaExclusivo, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga repositorioControleExpedicaoModeloVeicularCarga = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga(unidadeDeTrabalho);

            foreach (var modeloVeicularCargaExclusivo in listaModelosVeicularesCargaExclusivo)
            {
                int codigoModeloVeicularCarga = (int)modeloVeicularCargaExclusivo.Codigo;

                if (!(controleExpedicao.ModelosVeicularesCargaExclusivo?.Any(o => o.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga) ?? false))
                {
                    Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga modeloVeicularCargaExclusivoSalvar = new Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga()
                    {
                        ExpedicaoCarregamento = controleExpedicao,
                        ModeloVeicularCarga = ObterModeloVeicularCarga(codigoModeloVeicularCarga, unidadeDeTrabalho)
                    };

                    repositorioControleExpedicaoModeloVeicularCarga.Inserir(modeloVeicularCargaExclusivoSalvar, historico != null ? Auditado : null, historico);
                }
            }
        }

        private void ValidarDadosControleExpedicaoDuplicado(List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> listaControleExpedicaoCadastradoOuAtualizado, Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento controleExpedicaoSalvar)
        {
            Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento controleExpedicaoDuplicado = (
                from controleExpedicao in listaControleExpedicaoCadastradoOuAtualizado
                where (
                    (controleExpedicao.Dia == controleExpedicaoSalvar.Dia) &&
                    (controleExpedicao.ClienteDestino.CPF_CNPJ == controleExpedicaoSalvar.ClienteDestino.CPF_CNPJ) &&
                    (controleExpedicao.ProdutoEmbarcador.Codigo == controleExpedicaoSalvar.ProdutoEmbarcador.Codigo)
                )
                select controleExpedicao
            ).FirstOrDefault();

            if (controleExpedicaoDuplicado != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.ControleDeExpedicaoDuplicado, controleExpedicaoDuplicado.Descricao));
        }

        #endregion

        #region Métodos Privados - Docas

        private void AtualizarDocas(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic docas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Docas"));

            ExcluirDocasRemovidas(centroCarregamento, docas, unidadeDeTrabalho);
            SalvarDocasAdicionadasOuAtualizadas(centroCarregamento, docas, unidadeDeTrabalho);
        }

        private void ExcluirDocasRemovidas(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic docas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (centroCarregamento.Docas != null)
            {
                Repositorio.Embarcador.Logistica.CentroCarregamentoDoca repositorioDoca = new Repositorio.Embarcador.Logistica.CentroCarregamentoDoca(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var doca in docas)
                {
                    int? codigo = ((string)doca.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca> listaDocaRemover = (from doca in centroCarregamento.Docas where !listaCodigosAtualizados.Contains(doca.Codigo) select doca).ToList();

                foreach (var doca in listaDocaRemover)
                {
                    repositorioDoca.Deletar(doca);
                }

                if (listaDocaRemover.Count > 0)
                {
                    string descricaoAcao = listaDocaRemover.Count == 1 ? Localization.Resources.Logistica.CentroCarregamento.DocaRemovida : Localization.Resources.Logistica.CentroCarregamento.MultiplasDocasRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosDoca(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca centroCarregamentoDoca, dynamic doca, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            centroCarregamentoDoca.Numero = ((string)doca.Numero).ToNullableInt() ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.NumeroDaDocaNaoInformada);
            centroCarregamentoDoca.CodigoIntegracao = (string)doca.CodigoIntegracao;
            centroCarregamentoDoca.Descricao = (string)doca.Descricao;

            if (string.IsNullOrWhiteSpace(centroCarregamentoDoca.Descricao))
                throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.DescricaoDaDocaNaoInformado);

            if (string.IsNullOrWhiteSpace(centroCarregamentoDoca.CodigoIntegracao))
                throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.CodigoDeIntegracaoDaDocaNaoInformado);
        }

        private void SalvarDocasAdicionadasOuAtualizadas(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic docas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoDoca repositorioDoca = new Repositorio.Embarcador.Logistica.CentroCarregamentoDoca(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca> docasCadastradasOuAtualizadas = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca>();
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var doca in docas)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca centroCarregamentoDoca;
                int? codigo = ((string)doca.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    centroCarregamentoDoca = repositorioDoca.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.DocaNaoEncontrada);
                else
                    centroCarregamentoDoca = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca() { CentroCarregamento = centroCarregamento };

                PreencherDadosDoca(centroCarregamentoDoca, doca, unidadeDeTrabalho);
                ValidarDadosDocaDuplicados(docasCadastradasOuAtualizadas, repositorioDoca, centroCarregamentoDoca);

                docasCadastradasOuAtualizadas.Add(centroCarregamentoDoca);

                if (codigo.HasValue)
                {
                    totalRegistrosAtualizados += centroCarregamentoDoca.GetChanges().Count > 0 ? 1 : 0;
                    repositorioDoca.Atualizar(centroCarregamentoDoca);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioDoca.Inserir(centroCarregamentoDoca);
                }
            }

            if (centroCarregamento.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Logistica.CentroCarregamento.DocaAtualizada : Localization.Resources.Logistica.CentroCarregamento.MultiplasDocasAtualizadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Logistica.CentroCarregamento.DocaAdicionada : Localization.Resources.Logistica.CentroCarregamento.MultiplasDocasAdicionadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void ValidarDadosDocaDuplicados(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca> docasCadastradasOuAtualizadas, Repositorio.Embarcador.Logistica.CentroCarregamentoDoca repositorioDoca, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca centroCarregamentoDoca)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca centroCarregamentoDocaCodigoIntegracaoDuplicado = repositorioDoca.BuscarPorCodigoIntegracaoDuplicado(centroCarregamentoDoca.Codigo, centroCarregamentoDoca.CodigoIntegracao);

            if (centroCarregamentoDocaCodigoIntegracaoDuplicado != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.CodigoDeIntegracaoDeDocaJaEstaSendoUtilizadoNoCentroDeCarregamento, centroCarregamentoDocaCodigoIntegracaoDuplicado.CodigoIntegracao, centroCarregamentoDocaCodigoIntegracaoDuplicado.CentroCarregamento.Descricao));

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca centroCarregamentoDocaNumeroDuplicado = (from doca in docasCadastradasOuAtualizadas where (doca.Numero == centroCarregamentoDoca.Numero) select doca).FirstOrDefault();

            if (centroCarregamentoDocaNumeroDuplicado != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.NumeroDeDocaDuplicado, centroCarregamentoDoca.Numero));
        }

        #endregion

        #region Métodos Privados - Tipo Operação

        private void AtualizarTipoOperacao(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic tiposOperacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoOperacoes"));

            ExcluirTipoOperacaoRemovidas(centroCarregamento, tiposOperacoes, unidadeDeTrabalho);
            SalvarTiposOperacaoAdicionadasOuAtualizadas(centroCarregamento, tiposOperacoes, unidadeDeTrabalho);
        }

        private void ExcluirTipoOperacaoRemovidas(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic tiposOperacoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> existentes = repositorioTipoOperacao.BuscarPorCentro(centroCarregamento.Codigo);

            if (existentes.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var tipoOperacao in tiposOperacoes)
                {
                    int? codigo = ((string)tipoOperacao.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> listaDocaRemover = (from obj in existentes where !listaCodigosAtualizados.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao item in listaDocaRemover)
                {
                    repositorioTipoOperacao.Deletar(item);
                }

                if (listaDocaRemover.Count > 0)
                {
                    string descricaoAcao = listaDocaRemover.Count == 1 ? Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacaoRemovida : Localization.Resources.Logistica.CentroCarregamento.MultiplosTiposDeOperacaoRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosTipoOperacao(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao, dynamic tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            centroCarregamentoTipoOperacao.Codigo = ((string)tipoOperacao.Codigo).ToInt();
            centroCarregamentoTipoOperacao.TipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = ((string)tipoOperacao.CodigoTipoOperacao).ToInt() };
            centroCarregamentoTipoOperacao.Tipo = (CentroCarregamentoTipoOperacaoTipo)((string)tipoOperacao.Tipo).ToInt();
        }

        private void SalvarTiposOperacaoAdicionadasOuAtualizadas(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic tiposOperacoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> tiposOperacaoCadastradasOuAtualizadas = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao>();
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var doca in tiposOperacoes)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao;
                int? codigo = ((string)doca.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    centroCarregamentoTipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacaoNaoEncontrado);
                else
                    centroCarregamentoTipoOperacao = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao() { CentroCarregamento = centroCarregamento };

                PreencherDadosTipoOperacao(centroCarregamentoTipoOperacao, doca, unidadeDeTrabalho);
                ValidarDadosTipoOperacaoDuplicados(tiposOperacaoCadastradasOuAtualizadas, repositorioTipoOperacao, centroCarregamentoTipoOperacao);

                tiposOperacaoCadastradasOuAtualizadas.Add(centroCarregamentoTipoOperacao);

                if (codigo.HasValue)
                {
                    totalRegistrosAtualizados += centroCarregamentoTipoOperacao.GetChanges().Count > 0 ? 1 : 0;
                    repositorioTipoOperacao.Atualizar(centroCarregamentoTipoOperacao);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioTipoOperacao.Inserir(centroCarregamentoTipoOperacao);
                }
            }

            if (centroCarregamento.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacaoAtualizado : Localization.Resources.Logistica.CentroCarregamento.MultiplosTiposDeOperacaoAtualizados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacaoAdicionada : Localization.Resources.Logistica.CentroCarregamento.MultiplosTiposDeOperacaoAdicionados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void ValidarDadosTipoOperacaoDuplicados(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> itensCadastradasOuAtualizadas, Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioCentroCarregamentoTipoOperacao, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacaoDuplicado = (from obj in itensCadastradasOuAtualizadas where (obj.TipoOperacao.Codigo == centroCarregamentoTipoOperacao.TipoOperacao.Codigo) select obj).FirstOrDefault();

            if (centroCarregamentoTipoOperacaoDuplicado != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacaoDuplicado, centroCarregamentoTipoOperacao.TipoOperacao.Descricao));
        }

        #endregion

        #region Métodos Privados - Transportadores

        private void AtualizarTransportadores(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic listaCentroCarregamentoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCentroCarregamentoTransportador"));

            ExcluirTransportadoresRemovidos(centroCarregamento, listaCentroCarregamentoTransportador, unidadeDeTrabalho);
            SalvarTransportadoresAdicionadosOuAtualizados(centroCarregamento, listaCentroCarregamentoTransportador, unidadeDeTrabalho);
        }

        private void ExcluirTransportadoresRemovidos(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic listaCentroCarregamentoTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (centroCarregamento.Transportadores != null)
            {
                Repositorio.Embarcador.Logistica.CentroCarregamentoTransportador repositorioCentroCarregamentoTransportador = new Repositorio.Embarcador.Logistica.CentroCarregamentoTransportador(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var centroCarregamentoTransportador in listaCentroCarregamentoTransportador)
                {
                    int? codigo = ((string)centroCarregamentoTransportador.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador> listaCentroCarregamentoTransportadorRemover = (from centroCarregamentoTransportador in centroCarregamento.Transportadores where !listaCodigosAtualizados.Contains(centroCarregamentoTransportador.Codigo) select centroCarregamentoTransportador).ToList();

                foreach (var centroCarregamentoTransportador in listaCentroCarregamentoTransportadorRemover)
                {
                    foreach (var clienteDestino in centroCarregamentoTransportador.ClientesDestino.ToList())
                        centroCarregamentoTransportador.ClientesDestino.Remove(clienteDestino);

                    foreach (var localidadeDestino in centroCarregamentoTransportador.LocalidadesDestino.ToList())
                        centroCarregamentoTransportador.LocalidadesDestino.Remove(localidadeDestino);

                    foreach (var tipoDeCarga in centroCarregamentoTransportador.TiposDeCarga.ToList())
                        centroCarregamentoTransportador.TiposDeCarga.Remove(tipoDeCarga);

                    repositorioCentroCarregamentoTransportador.Deletar(centroCarregamentoTransportador);
                }

                if (listaCentroCarregamentoTransportadorRemover.Count > 0)
                {
                    string descricaoAcao = listaCentroCarregamentoTransportadorRemover.Count == 1 ? Localization.Resources.Logistica.CentroCarregamento.TransportadorRemovido : Localization.Resources.Logistica.CentroCarregamento.MultiplosTransprotadoresRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosTransportador(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador centroCarregamentoTransportadorSalvar, dynamic centroCarregamentoTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            int codigoTransportador = ((string)centroCarregamentoTransportador.Transportador).ToInt();

            centroCarregamentoTransportadorSalvar.Transportador = ObterTransportador(codigoTransportador, unidadeDeTrabalho);
            centroCarregamentoTransportadorSalvar.ClientesDestino = new List<Dominio.Entidades.Cliente>();
            centroCarregamentoTransportadorSalvar.LocalidadesDestino = new List<Dominio.Entidades.Localidade>();
            centroCarregamentoTransportadorSalvar.TiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            foreach (var clienteDestino in centroCarregamentoTransportador.ClientesDestino)
            {
                double cpfCnpjClienteDestino = ((string)clienteDestino).ToDouble();
                Dominio.Entidades.Cliente clienteDestinoAdicionar = ObterCliente(cpfCnpjClienteDestino, unidadeDeTrabalho);

                centroCarregamentoTransportadorSalvar.ClientesDestino.Add(clienteDestinoAdicionar);
            }

            foreach (var localidadeDestino in centroCarregamentoTransportador.LocalidadesDestino)
            {
                int codigoLocalidadeDestino = ((string)localidadeDestino).ToInt();
                Dominio.Entidades.Localidade localidadeDestinoAdicionar = ObterLocalidade(codigoLocalidadeDestino, unidadeDeTrabalho);

                centroCarregamentoTransportadorSalvar.LocalidadesDestino.Add(localidadeDestinoAdicionar);
            }

            foreach (var tipoCarga in centroCarregamentoTransportador.TiposCarga)
            {
                int codigoTipoCarga = ((string)tipoCarga).ToInt();
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaAdicionar = ObterTipoDeCarga(codigoTipoCarga, unidadeDeTrabalho);

                centroCarregamentoTransportadorSalvar.TiposDeCarga.Add(tipoCargaAdicionar);
            }
        }

        private void SalvarTransportadoresAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic listaCentroCarregamentoTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoTransportador repositorioCentroCarregamentoTransportador = new Repositorio.Embarcador.Logistica.CentroCarregamentoTransportador(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador> transportadoresCadastradosOuAtualizados = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador>();
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var centroCarregamentoTransportador in listaCentroCarregamentoTransportador)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador centroCarregamentoTransportadorSalvar;
                int? codigo = ((string)centroCarregamentoTransportador.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    centroCarregamentoTransportadorSalvar = repositorioCentroCarregamentoTransportador.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TransportadorNaoEncontrado);
                else
                    centroCarregamentoTransportadorSalvar = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador() { CentroCarregamento = centroCarregamento };

                PreencherDadosTransportador(centroCarregamentoTransportadorSalvar, centroCarregamentoTransportador, unidadeDeTrabalho);
                ValidarDadosTransportadorDuplicado(transportadoresCadastradosOuAtualizados, centroCarregamentoTransportadorSalvar);

                transportadoresCadastradosOuAtualizados.Add(centroCarregamentoTransportadorSalvar);

                if (codigo.HasValue)
                {
                    totalRegistrosAtualizados += centroCarregamentoTransportadorSalvar.GetChanges().Count > 0 ? 1 : 0;
                    repositorioCentroCarregamentoTransportador.Atualizar(centroCarregamentoTransportadorSalvar);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioCentroCarregamentoTransportador.Inserir(centroCarregamentoTransportadorSalvar);
                }
            }

            if (centroCarregamento.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Logistica.CentroCarregamento.TransportadorAtualizado : Localization.Resources.Logistica.CentroCarregamento.MultiplosTransportadoresAtualizados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Logistica.CentroCarregamento.TransportadorAdicionado : Localization.Resources.Logistica.CentroCarregamento.MultiplosTransportadoresAdicionados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void ValidarDadosTransportadorDuplicado(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador> transportadoresCadastradosOuAtualizados, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador centroCarregamentoTransportadorSalvar)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador centroCarregamentoTransportadorDuplicado = (from centroCarregamentoTransportador in transportadoresCadastradosOuAtualizados where centroCarregamentoTransportador.Transportador.Codigo == centroCarregamentoTransportadorSalvar.Transportador.Codigo select centroCarregamentoTransportador).FirstOrDefault();

            if (centroCarregamentoTransportadorDuplicado != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.TransportadorDuplicado, centroCarregamentoTransportadorDuplicado.Transportador.Descricao));
        }

        #endregion

        #region Métodos Privados - Transportadores Terceiros

        private void AtualizarTransportadoresTerceiros(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic listaCentroCarregamentoTransportadorTerceiro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCentroCarregamentoTransportadorTerceiro"));

            ExcluirTransportadoresTerceirosRemovidos(centroCarregamento, listaCentroCarregamentoTransportadorTerceiro, unidadeDeTrabalho);
            SalvarTransportadoresTerceirosAdicionadosOuAtualizados(centroCarregamento, listaCentroCarregamentoTransportadorTerceiro, unidadeDeTrabalho);
        }

        private void ExcluirTransportadoresTerceirosRemovidos(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic listaCentroCarregamentoTransportadorTerceiro, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (centroCarregamento.TransportadoresTerceiros != null)
            {
                Repositorio.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro repositorioCentroCarregamentoTransportadorTerceiro = new Repositorio.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var centroCarregamentoTransportadorTerceiro in listaCentroCarregamentoTransportadorTerceiro)
                {
                    int? codigo = ((string)centroCarregamentoTransportadorTerceiro.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro> listaCentroCarregamentoTransportadorTerceiroRemover = (from centroCarregamentoTransportadorTerceiro in centroCarregamento.TransportadoresTerceiros where !listaCodigosAtualizados.Contains(centroCarregamentoTransportadorTerceiro.Codigo) select centroCarregamentoTransportadorTerceiro).ToList();

                foreach (var centroCarregamentoTransportadorTerceiro in listaCentroCarregamentoTransportadorTerceiroRemover)
                    repositorioCentroCarregamentoTransportadorTerceiro.Deletar(centroCarregamentoTransportadorTerceiro);

                if (listaCentroCarregamentoTransportadorTerceiroRemover.Count > 0)
                {
                    string descricaoAcao = listaCentroCarregamentoTransportadorTerceiroRemover.Count == 1 ? Localization.Resources.Logistica.CentroCarregamento.TransportadorRemovido : Localization.Resources.Logistica.CentroCarregamento.MultiplosTransprotadoresRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosTransportadorTerceiro(Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro centroCarregamentoTransportadorTerceiroSalvar, dynamic centroCarregamentoTransportadorTerceiro, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            double codigoTransportadorTerceiro = ((string)centroCarregamentoTransportadorTerceiro.Transportador).ToDouble();

            centroCarregamentoTransportadorTerceiroSalvar.Transportador = ObterTransportadorTerceiro(codigoTransportadorTerceiro, unidadeDeTrabalho);
        }

        private void SalvarTransportadoresTerceirosAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, dynamic listaCentroCarregamentoTransportadorTerceiro, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro repositorioCentroCarregamentoTransportadorTerceiro = new Repositorio.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro> transportadoresTerceirosCadastradosOuAtualizados = new List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro>();
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var centroCarregamentoTransportadorTerceiro in listaCentroCarregamentoTransportadorTerceiro)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro centroCarregamentoTransportadorTerceiroSalvar;
                int? codigo = ((string)centroCarregamentoTransportadorTerceiro.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    centroCarregamentoTransportadorTerceiroSalvar = repositorioCentroCarregamentoTransportadorTerceiro.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.TransportadorNaoEncontrado);
                else
                    centroCarregamentoTransportadorTerceiroSalvar = new Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro() { CentroCarregamento = centroCarregamento };

                PreencherDadosTransportadorTerceiro(centroCarregamentoTransportadorTerceiroSalvar, centroCarregamentoTransportadorTerceiro, unidadeDeTrabalho);
                ValidarDadosTransportadorTerceiroDuplicado(transportadoresTerceirosCadastradosOuAtualizados, centroCarregamentoTransportadorTerceiroSalvar);

                transportadoresTerceirosCadastradosOuAtualizados.Add(centroCarregamentoTransportadorTerceiroSalvar);

                if (codigo.HasValue)
                {
                    totalRegistrosAtualizados += centroCarregamentoTransportadorTerceiroSalvar.GetChanges().Count > 0 ? 1 : 0;
                    repositorioCentroCarregamentoTransportadorTerceiro.Atualizar(centroCarregamentoTransportadorTerceiroSalvar);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioCentroCarregamentoTransportadorTerceiro.Inserir(centroCarregamentoTransportadorTerceiroSalvar);
                }
            }

            if (centroCarregamento.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Logistica.CentroCarregamento.TransportadorAtualizado : Localization.Resources.Logistica.CentroCarregamento.MultiplosTransportadoresAtualizados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Logistica.CentroCarregamento.TransportadorAdicionado : Localization.Resources.Logistica.CentroCarregamento.MultiplosTransportadoresAdicionados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, centroCarregamento, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void ValidarDadosTransportadorTerceiroDuplicado(List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro> transportadoresTerceirosCadastradosOuAtualizados, Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro centroCarregamentoTransportadorTerceiroSalvar)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro centroCarregamentoTransportadorTerceiroDuplicado = (from centroCarregamentoTransportadorTerceiro in transportadoresTerceirosCadastradosOuAtualizados where centroCarregamentoTransportadorTerceiro.Transportador.Codigo == centroCarregamentoTransportadorTerceiroSalvar.Transportador.Codigo select centroCarregamentoTransportadorTerceiro).FirstOrDefault();

            if (centroCarregamentoTransportadorTerceiroDuplicado != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.TransportadorDuplicado, centroCarregamentoTransportadorTerceiroDuplicado.Transportador.Descricao));
        }

        #endregion

        #region Automatização de No-show

        private void AtualizarAutomatizacaoNaoComparecimento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaAutomatizacaoNaoComparecimento = JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAutomatizacaoNaoComparecimento"));

            ExcluirAutomatizacaoNaoComparecimento(listaAutomatizacaoNaoComparecimento, centroCarregamento, historico, unitOfWork);
            SalvarAutomatizacaoNaoComparecimento(listaAutomatizacaoNaoComparecimento, centroCarregamento, historico, unitOfWork);
        }

        private void ExcluirAutomatizacaoNaoComparecimento(dynamic listaAutomatizacaoNaoComparecimento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento repositorioAutomatizacaoNaoComparecimento = new Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> automatizacoesNaoComparecimentoCadastradas = repositorioAutomatizacaoNaoComparecimento.BuscarPorCentroCarregamento(centroCarregamento.Codigo);

            if (automatizacoesNaoComparecimentoCadastradas.Count == 0)
                return;

            List<int> codigos = new List<int>();

            foreach (var automatizacaoNaoComparecimento in listaAutomatizacaoNaoComparecimento)
            {
                int? codigo = ((string)automatizacaoNaoComparecimento.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    codigos.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> automatizacoesNaoComparecimentoDeletar = (from o in automatizacoesNaoComparecimentoCadastradas where !codigos.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimentoDeletar in automatizacoesNaoComparecimentoDeletar)
                repositorioAutomatizacaoNaoComparecimento.Deletar(automatizacaoNaoComparecimentoDeletar, historico != null ? Auditado : null, historico);
        }

        private dynamic ObterAutomatizacaoNaoComparecimento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, bool duplicar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento repositorioAutomatizacaoNaoComparecimento = new Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> automatizacoesNaoComparecimentoCadastradas = repositorioAutomatizacaoNaoComparecimento.BuscarPorCentroCarregamento(centroCarregamento.Codigo);

            return new
            {
                Dados = new
                {
                    centroCarregamento.PermiteMarcarCargaComoNaoComparecimento
                },
                ListaAutomatizacaoNaoComparecimento = (
                    from o in automatizacoesNaoComparecimentoCadastradas
                    select new
                    {
                        Codigo = !duplicar ? o.Codigo : 0,
                        o.Gatilho,
                        o.HorasTolerancia,
                        o.BloquearCarga,
                        o.EnviarEmailTransportador,
                        o.RetornarCargaParaExcedente
                    }
                ).ToList()
            };
        }

        private void SalvarAutomatizacaoNaoComparecimento(dynamic listaAutomatizacaoNaoComparecimento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento repositorioAutomatizacaoNaoComparecimento = new Repositorio.Embarcador.Logistica.AutomatizacaoNaoComparecimento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> automatizacoesNaoComparecimentoSalvas = new List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento>();

            foreach (var automatizacaoNaoComparecimento in listaAutomatizacaoNaoComparecimento)
            {
                Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimentoSalvar;
                int? codigo = ((string)automatizacaoNaoComparecimento.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    automatizacaoNaoComparecimentoSalvar = repositorioAutomatizacaoNaoComparecimento.BuscarPorCodigo(codigo.Value, true) ?? throw new ControllerException(Localization.Resources.Logistica.CentroCarregamento.AutomatizacaoDeNoShowNaoEncontrada);
                else
                    automatizacaoNaoComparecimentoSalvar = new Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento()
                    {
                        DataCadastro = DateTime.Now
                    };

                automatizacaoNaoComparecimentoSalvar.BloquearCarga = ((string)automatizacaoNaoComparecimento.BloquearCarga).ToBool();
                automatizacaoNaoComparecimentoSalvar.CentroCarregamento = centroCarregamento;
                automatizacaoNaoComparecimentoSalvar.EnviarEmailTransportador = ((string)automatizacaoNaoComparecimento.EnviarEmailTransportador).ToBool();
                automatizacaoNaoComparecimentoSalvar.Gatilho = ((string)automatizacaoNaoComparecimento.Gatilho).ToEnum<GatilhoAutomatizacaoNaoComparecimento>();
                automatizacaoNaoComparecimentoSalvar.HorasTolerancia = ((string)automatizacaoNaoComparecimento.HorasTolerancia).ToInt();
                automatizacaoNaoComparecimentoSalvar.RetornarCargaParaExcedente = ((string)automatizacaoNaoComparecimento.RetornarCargaParaExcedente).ToBool();

                ValidarAutomatizacaoNaoComparecimentoDuplicada(automatizacaoNaoComparecimentoSalvar, automatizacoesNaoComparecimentoSalvas);

                if (automatizacaoNaoComparecimentoSalvar.Codigo > 0)
                    repositorioAutomatizacaoNaoComparecimento.Atualizar(automatizacaoNaoComparecimentoSalvar, historico != null ? Auditado : null, historico);
                else
                    repositorioAutomatizacaoNaoComparecimento.Inserir(automatizacaoNaoComparecimentoSalvar, historico != null ? Auditado : null, historico);

                automatizacoesNaoComparecimentoSalvas.Add(automatizacaoNaoComparecimentoSalvar);
            }
        }

        private void ValidarAutomatizacaoNaoComparecimentoDuplicada(Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimentoSalvar, List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> automatizacoesNaoComparecimentoSalvas)
        {
            Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento automatizacaoNaoComparecimentoDuplicada = (from o in automatizacoesNaoComparecimentoSalvas where o.Gatilho == automatizacaoNaoComparecimentoSalvar.Gatilho select o).FirstOrDefault();

            if (automatizacaoNaoComparecimentoDuplicada != null)
                throw new ControllerException(string.Format(Localization.Resources.Logistica.CentroCarregamento.AutomatizacaoDeNoShowComGatilhoDuplicada, automatizacaoNaoComparecimentoDuplicada.Gatilho.ObterDescricao()));
        }

        #endregion Automatização de No-show
    }
}
