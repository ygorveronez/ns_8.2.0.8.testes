using Dominio.Entidades.Embarcador.Cargas.MontagemCarga;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
    public class SessaoRoteirizadorController : BaseController
    {
        #region Construtores

        public SessaoRoteirizadorController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Sessao, "Codigo", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Filial, "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Usuario, "Nome", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Inicio, "Inicio", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Fim, "Fim", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.De, "DataInicial", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Ate, "DataFinal", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.QuantidadePedidos, "QtdePedidos", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeCarregamentos, "QtdeCarregamentos", 10, Models.Grid.Align.left, false);

                if (filtrosPesquisa.Situacao == SituacaoSessaoRoteirizador.Todas)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Situacao, "DescricaoSituacao", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador> result = repSessaoRoteirizador.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSessaoRoteirizador.ContarConsulta(filtrosPesquisa));

                List<int> codigosSessoes = (from sessao in result
                                            select sessao.Codigo).ToList();

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> totaisPedidosSessoesRoteirizador = codigosSessoes.Count > 0 ? repSessaoRoteirizadorPedido.QuantidadePedidosSessoesRoteirizador(codigosSessoes) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> totaisCarregamentosSessoesRoteirizador = codigosSessoes.Count > 0 ? repSessaoRoteirizador.QtdeCarregamentos(codigosSessoes) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();

                var lista = (
                    from p in result
                    select new
                    {
                        p.Codigo,
                        p.Filial.Descricao,
                        p.Usuario.Nome,
                        p.Inicio,
                        p.Fim,
                        DataInicial = p.DataInicial?.ToString("dd/MM/yyyy"),
                        DataFinal = p.DataFinal?.ToString("dd/MM/yyyy"),
                        QtdePedidos = (from total in totaisPedidosSessoesRoteirizador where total.Codigo == p.Codigo select total.Total).FirstOrDefault(),              // repSessaoRoteirizadorPedido.QtdePedidosSessaoRoteirizador(p.Codigo),
                        QtdeCarregamentos = (from total in totaisCarregamentosSessoesRoteirizador where total.Codigo == p.Codigo select total.Total).FirstOrDefault(),  // repSessaoRoteirizador.QtdeCarregamentos(p.Codigo),
                        p.DescricaoSituacao
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
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);

                //A cada usuários que abre a sessão, vamos mudar o usuário atual para bloquear a alteração dos carregamentos
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = repSessao.BuscarPorCodigo(codigo);
                sessao.UsuarioAtual = this.Usuario;

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repCentroCarregamento.BuscarPorFiliais(new List<int> { sessao.Filial.Codigo });
                bool dataCarregamentoObrigatoriaMontagemCarga = centrosCarregamento.Exists(x => x.DataCarregamentoObrigatoriaMontagemCarga == true);

                if (string.IsNullOrEmpty(sessao.Parametros))
                    sessao.Parametros = this.ObterJsonParametrosCentroCarregamento(sessao, centrosCarregamento, unitOfWork);
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros objeto = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros>(sessao.Parametros);
                    sessao.Parametros = Newtonsoft.Json.JsonConvert.SerializeObject(objeto);
                }

                repSessao.Atualizar(sessao);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidosSessao = repSessaoPedidos.PedidosSessaoRoteirizador(codigo, false);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> totaisCarregamentosSessoesRoteirizador = repSessao.QtdeCarregamentos(new List<int> { codigo });

                var preencherAutomaticamenteDadosCentroTelaMontagemCarga = centrosCarregamento.Any(x => x.PreencherAutomaticamenteDadosCentroTelaMontagemCarga);

                var result = new
                {
                    sessao.Codigo,
                    Filial = sessao.Filial.Codigo,
                    Descricao = sessao.Filial.Descricao + (sessao.Expedidor == null ? "" : " ( " + sessao.Expedidor?.Descricao ?? "" + ")"),
                    Expedidor = new
                    {
                        Codigo = sessao?.Expedidor?.Codigo ?? 0,
                        Descricao = sessao?.Expedidor?.Descricao ?? ""
                    },
                    sessao.RoteirizacaoRedespacho,
                    sessao.Usuario.Nome,
                    sessao.Inicio,
                    sessao.Fim,
                    sessao.MontagemCarregamentoPedidoProduto,
                    sessao.TipoRoteirizacaoColetaEntrega,
                    sessao.TipoMontagemCarregamentoPedidoProduto,
                    sessao.Parametros,
                    sessao.SituacaoSessaoRoteirizador,
                    DataInicial = sessao.DataInicial?.ToString("dd/MM/yyyy"),
                    DataFinal = sessao.DataFinal?.ToString("dd/MM/yyyy"),
                    QtdePedidos = pedidosSessao.Count,
                    QtdeCarregamentos = (from total in totaisCarregamentosSessoesRoteirizador where total.Codigo == sessao.Codigo select total.Total).FirstOrDefault(),
                    Destinatarios = (from c in pedidosSessao
                                     select new
                                     {
                                         CPF_CNPJ = c.Pedido?.Destinatario?.CPF_CNPJ ?? 0,
                                         Nome = c.Pedido?.Destinatario?.Nome ?? string.Empty,
                                         CodigoIntegracao = c.Pedido?.Destinatario?.CodigoIntegracao ?? string.Empty
                                     }).Distinct().OrderBy(x => x.Nome).ToList(),
                    TiposDeCarga = (from c in pedidosSessao
                                    select new
                                    {
                                        Codigo = c.Pedido.TipoDeCarga?.Codigo ?? 0,
                                        Descricao = c.Pedido.TipoDeCarga?.Descricao ?? "",
                                    }).Distinct().OrderBy(x => x.Descricao).ToList(),
                    DataCarregamentoObrigatoriaMontagemCarga = dataCarregamentoObrigatoriaMontagemCarga,
                    EscolherHorarioCarregamentoPorLista = centrosCarregamento.Any(x => x.EscolherHorarioCarregamentoPorLista),
                    TipoMontagemCarregamentoVRP = (from o in centrosCarregamento select o.TipoMontagemCarregamentoVRP).FirstOrDefault(),
                    TipoPedidoMontagemCarregamento = (from o in centrosCarregamento select o.TipoPedidoMontagemCarregamento)?.FirstOrDefault() ?? TipoPedidoMontagemCarregamento.Card,
                    TipoEdicaoPalletProdutoMontagemCarregamento = (from o in centrosCarregamento select o.TipoEdicaoPalletProdutoMontagemCarregamento)?.FirstOrDefault() ?? TipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado,
                    SimuladorFreteCriterioSelecaoTransportador = (from o in centrosCarregamento select o.SimuladorFreteCriterioSelecaoTransportador).FirstOrDefault(),
                    ConsiderarPesoPalletPesoTotalCarga = centrosCarregamento.Any(x => x.ConsiderarPesoPalletPesoTotalCarga),
                    PreencherAutomaticamenteDadosCentroTelaMontagemCarga = preencherAutomaticamenteDadosCentroTelaMontagemCarga,
                    MotoristaPadrao = preencherAutomaticamenteDadosCentroTelaMontagemCarga ? new
                    {
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.MotoristaPadrao?.Codigo,
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.MotoristaPadrao?.Descricao,
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.MotoristaPadrao?.CPF,
                    } : null,
                    TipoOperacaoPadrao = preencherAutomaticamenteDadosCentroTelaMontagemCarga ? new
                    {
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.TipoOperacaoPadrao?.Codigo,
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.TipoOperacaoPadrao?.Descricao,
                    } : null,
                    VeiculoPadrao = preencherAutomaticamenteDadosCentroTelaMontagemCarga ? new
                    {
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.VeiculoPadrao?.Codigo,
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.VeiculoPadrao?.Descricao,
                    } : null,
                    ModeloVeicularPadrao = preencherAutomaticamenteDadosCentroTelaMontagemCarga ? new
                    {
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.ModeloVeicularCargaPadrao?.Codigo,
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.ModeloVeicularCargaPadrao?.Descricao,
                    } : null,
                    EmpresaPadrao = preencherAutomaticamenteDadosCentroTelaMontagemCarga ? new
                    {
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.EmpresaPadrao?.Codigo,
                        centrosCarregamento.FirstOrDefault()?.ConfiguracaoPadrao?.EmpresaPadrao?.Descricao,
                    } : null
                };

                return new JsonpResult(result);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarSessaoDeRoteirizacaoPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = repSessao.BuscarPorCodigo(codigo, true);
                if (sessao.SituacaoSessaoRoteirizador != SituacaoSessaoRoteirizador.Iniciada)
                    return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoCancelarUmaSessaoDeRoteirizacaoComSituacaoDiferenteDaIniciada);

                if (sessao.UsuarioAtual.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoCancelarSessaoDeRoteirizacaoPoisElaEstaAbertaParaUsuario, sessao.UsuarioAtual.Nome));

                //Não pode existir carregamento com situação em montagem para a sessão
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosPendentes = Servicos.Embarcador.Carga.MontagemCarga.Carregamento.ValidarCarregamentosSessaoRoteirizador(codigo, SituacaoCarregamento.EmMontagem, unitOfWork);
                if (carregamentosPendentes.Count > 0)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemCarregamentosEmMontagemSessaoNaoPodeSerCancelada, carregamentosPendentes.Count, string.Join(",", (from obj in carregamentosPendentes select obj.Codigo).Distinct().ToList())));

                carregamentosPendentes = Servicos.Embarcador.Carga.MontagemCarga.Carregamento.ValidarCarregamentosSessaoRoteirizador(codigo, SituacaoCarregamento.GerandoCargaBackground, unitOfWork);
                if (carregamentosPendentes.Count > 0)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemCarregamentosComSituacaoGerandoCargaSessaoNaoPodeSerCancelada, carregamentosPendentes.Count, string.Join(",", (from obj in carregamentosPendentes select obj.Codigo).Distinct().ToList())));

                //var carregamentosFinalizados = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork).CarregamentosSessaoRoteirizador(codigo, SituacaoCarregamento.Fechado);
                var carregamentosFinalizados = Servicos.Embarcador.Carga.MontagemCarga.Carregamento.ValidarCarregamentosSessaoRoteirizador(codigo, SituacaoCarregamento.Fechado, unitOfWork);
                if (carregamentosFinalizados.Count > 0)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemCarregamentosFinalizadosSessaoNaoPodeSerCancelada, carregamentosFinalizados.Count, string.Join(",", (from obj in carregamentosFinalizados select obj.Codigo).Distinct().ToList())));

                //Validar se existe algum pedido da sessão sem carga gerada...
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                int qtde_pedidos_carga = repSessaoRoteirizadorPedido.QtdePedidosSessaoRoteirizadorCarga(codigo);
                if (qtde_pedidos_carga > 0)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemPedidosRelacionadosAsCargasSessaoNaoPodeSerCancelada, qtde_pedidos_carga, repSessaoRoteirizadorPedido.CargasPedidosSessaoRoteirizadorCarga(codigo)));

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> sessaoRoteirizadorPedidoProdutos = repSessaoRoteirizadorPedidoProduto.BuscarPorSessaoRoteirizador(sessao.Codigo);

                bool temControlePorEstoqueArmazem = sessaoRoteirizadorPedidoProdutos.Exists(sr => sr.PedidoProduto.FilialArmazem != null);

                if (temControlePorEstoqueArmazem)
                    RetornarQuantidadeProdutoEmSessaoParaEstoqueArmazem(sessaoRoteirizadorPedidoProdutos, unitOfWork);

                //Excluindo os registros da tabela de Roteirização automática... presos...
                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido repMontagemCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido(unitOfWork);
                repMontagemCarregamentoPedido.DeletarTodos(sessao.Codigo);

                //Excluindo os registros da tabela de sessão pedido...
                repSessaoRoteirizadorPedido.DeletarTodos(sessao.Codigo);

                sessao.SituacaoSessaoRoteirizador = SituacaoSessaoRoteirizador.Cancelada;
                sessao.Fim = DateTime.Now;
                repSessao.Atualizar(sessao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, sessao, null, Localization.Resources.Cargas.MontagemCargaMapa.SessaoDeRoteirizacaoCancelada, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoCancelarSessaoDeRoteirizacaoPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = repSessao.BuscarPorCodigo(codigo, true);
                if (sessao.SituacaoSessaoRoteirizador != SituacaoSessaoRoteirizador.Iniciada)
                    return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoFinalizarUmaSessaoDeRoteirizacaoComSituacaoDiferenteDeIniciada);

                if (sessao.UsuarioAtual.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoFinalizarSessaoDeRoteirizacaoPoiselaEstaAbertaParaUsuario, sessao.UsuarioAtual.Nome));

                //Não pode existir carregamento com situação em montagem para a sessão
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosPendentes = Servicos.Embarcador.Carga.MontagemCarga.Carregamento.ValidarCarregamentosSessaoRoteirizador(codigo, SituacaoCarregamento.EmMontagem, unitOfWork);
                if (carregamentosPendentes.Count > 0)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemCarregamentosEmMontagemSessaoNaoPodeSerFinalizada, carregamentosPendentes.Count, string.Join(",", (from obj in carregamentosPendentes select obj.NumeroCarregamento).Distinct().ToList())));

                carregamentosPendentes = Servicos.Embarcador.Carga.MontagemCarga.Carregamento.ValidarCarregamentosSessaoRoteirizador(codigo, SituacaoCarregamento.GerandoCargaBackground, unitOfWork);
                if (carregamentosPendentes.Count > 0)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemCarregamentosComSituacaoGerandoCargaSessaoNaoPodeSerFinalizada, carregamentosPendentes.Count, string.Join(",", (from obj in carregamentosPendentes select obj.NumeroCarregamento).Distinct().ToList())));

                //Validar se existe algum pedido da sessão sem carga gerada...
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                int qtde_pedidos_sem_carga = repSessaoRoteirizadorPedido.QtdePedidosSessaoRoteirizadorCarga(codigo, true);
                if (qtde_pedidos_sem_carga > 0)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.ExistemPedidosRelacionadosSessaoDeRoteirizacaoSemCargaMesmaNaoPodeSerFinalizada, qtde_pedidos_sem_carga));

                sessao.SituacaoSessaoRoteirizador = SituacaoSessaoRoteirizador.Finalizada;
                sessao.Fim = DateTime.Now;
                repSessao.Atualizar(sessao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, sessao, null, Localization.Resources.Cargas.MontagemCargaMapa.SessaoDeRoteirizacaoFinalizada, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoFinalizarSessaoDeRoteirizacaoPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSessaoRoteirizador = int.Parse(Request.Params("Codigo"));
                List<int> codigosPedidos = Request.GetListParam<int>("pedidos");
                bool cancelarReserva = Request.GetBoolParam("CancelarReserva");
                bool cancelarPedidos = Request.GetBoolParam("CancelarPedidos");

                if (codigosPedidos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NenhumPedidoInformadoParaRemoverDaSessaoDeRoteirizacao);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork).BuscarPorCodigo(codigoSessaoRoteirizador, false);
                if (sessao.SituacaoSessaoRoteirizador != SituacaoSessaoRoteirizador.Iniciada)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverPedidosDeUmaSessaoDeRoteirizacaoComSituacaoDiferenteDaIniciada);

                if (sessao.UsuarioAtual.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverPedidosDaSessaoDeRoteirizacaoPoisElaEstaAbertaParaUsuario, sessao.UsuarioAtual.Nome));

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                // Pedidos não pode estar em nenhum carregamento,
                // Porem, precisamos verificar se pertence a sessão....
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repositorioSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem repositorioCarregamentoRoteirizacaoPontosPassagem = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repositorioCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(unitOfWork);

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidosSessao = repositorioSessaoRoteirizadorPedido.BuscarSessaoRoteirizadorPedidos(codigoSessaoRoteirizador, codigosPedidos);

                //#36346, comentado pois usuários reclamando que estava gerando esse alerta.. acredito que alguem já tenha removido 
                // os pedidos da sessao, sendo assim a consulta não retorna os pedidos já removidos, com isso, apresenha o alerta
                // pois nem todos os pedidos retorna.....
                //if (pedidosSessao.Count != codigosPedidos.Count)
                //    return new JsonpResult(false, true, "Um ou mais pedidos não pertencem a sessão de roteirização informada.");

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = repositorioCarregamentoPedido.BuscarPorPedidosEmMontagem(codigoSessaoRoteirizador, codigosPedidos);
                if (carregamentosPedidos.Count > 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.OsPedidosInformadosPossuemCarregamentoComSituacaoEmMontagemNaoPodemoSerRemovidos);

                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);

                //11602
                if (cancelarReserva)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidosQuebra = pedidosSessao.FindAll(x => x.Pedido.QuebraMultiplosCarregamentos == false);
                    if (pedidosQuebra?.Count > 0)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoCancelarReservaDosPedidosPoisOsMesmosNaoPermitemQuebraNaRoteirizacao, string.Join(", ", (from nro in pedidosQuebra
                                                                                                                                                                                                                               select nro.Pedido.NumeroPedidoEmbarcador).ToArray())));
                }

                int erro = 0;

                if (cancelarPedidos)
                {
                    // Validar se os pedido selecionados não estão em nenhum carregamento.
                    carregamentosPedidos = repositorioCarregamentoPedido.BuscarTodosCarregamentosPorPedidos(codigosPedidos, false, false);
                    if (carregamentosPedidos.Count > 0)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverCancelarPedidosDeUmaSessaoDeRoteirizacaoComCarregamento, string.Join(", ", (from obj in carregamentosPedidos
                                                                                                                                                                                                                         select obj.Pedido.NumeroPedidoEmbarcador).ToArray())));

                    // Validar se os pedidos não possuem NF... pedido.PedidoNotasParciais.count > 0
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosComNf = (from obj in pedidosSessao where ((obj.Pedido?.PedidoNotasParciais?.Count ?? 0) > 0) select obj.Pedido).ToList();
                    if (pedidosComNf.Count > 0)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoRemoverCancelarPedidosDeUmaSessaoDeRoteirizacaoComDadosNotaFiscal, string.Join(", ", (from obj in pedidosComNf
                                                                                                                                                                                                                            select obj.NumeroPedidoEmbarcador).ToArray())));
                    try
                    {
                        unitOfWork.Start();
                        repositorioSessaoRoteirizadorPedido.AtualizarSituacao(codigoSessaoRoteirizador, codigosPedidos, SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);

                        Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                        foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido sessaoRoteirizadorPedido in pedidosSessao)
                        {
                            sessaoRoteirizadorPedido.Pedido.ControleNumeracao = sessaoRoteirizadorPedido.Pedido.Codigo;
                            sessaoRoteirizadorPedido.Pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                            repositorioPedido.Atualizar(sessaoRoteirizadorPedido.Pedido);

                            string descricaoSessao = ((sessao?.Codigo ?? 0) == 0 ? "" : sessao.Codigo.ToString() + " - ") + sessao?.Descricao ?? "";

                            //#45088 - Se tiver um pedido do "TRAZ" relacionado.. devemos cancelar tambem...
                            if (sessaoRoteirizadorPedido.Pedido.PedidoDevolucao != null)
                            {
                                sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.ControleNumeracao = sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.Codigo;
                                sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.SituacaoPedido = SituacaoPedido.Cancelado;
                                sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.ControleNumeracao = sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.Codigo;
                                repositorioPedido.Atualizar(sessaoRoteirizadorPedido.Pedido.PedidoDevolucao);

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, sessaoRoteirizadorPedido.Pedido.PedidoDevolucao, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CanceladoPedidoNaSessaoDeRoteirizacao, descricaoSessao), unitOfWork);
                            }

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, sessaoRoteirizadorPedido.Pedido, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CanceladoPedidoNaSessaoDeRoteirizacao, descricaoSessao), unitOfWork);
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }

                }
                else if (!cancelarReserva)
                    repositorioSessaoRoteirizadorPedido.AtualizarSituacao(codigoSessaoRoteirizador, codigosPedidos, SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);
                else
                {
                    Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                    List<int> pedidosComCarregamento = repPedidos.BuscarPorCodigosComCarregamento(codigosPedidos);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidosNaoAtendido = repositorioPedidoProduto.ProdutosPedidosNaoAtendidosTotalmente(codigosPedidos);

                    //Consulta todos os carregamentos pedido produto;;
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosCarregadoPedidos = repositorioCarregamentoPedidoProduto.BuscarPorPedidos(codigosPedidos);

                    Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                    List<TipoIntegracao> listaTiposIntegracao = new List<TipoIntegracao>() { TipoIntegracao.Digibee, TipoIntegracao.TelhaNorte };

                    List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(listaTiposIntegracao, null);

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido sessaoRoteirizadorPedido in pedidosSessao)
                    {
                        try
                        {
                            unitOfWork.FlushAndClear();
                            unitOfWork.Start();

                            sessaoRoteirizadorPedido.Situacao = SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao;
                            //Assai cancela reserva...
                            if (cancelarReserva)
                            {
                                // 2 - Os pedidos que foram cancelados integralmente, ou seja, que não foram incluídos em nenhum carregamento devem ficar com o status de cancelado ?
                                var existeCarregamentoDoPedido = pedidosComCarregamento?.Exists(p => p == sessaoRoteirizadorPedido.Pedido.Codigo) ?? false;
                                if (!existeCarregamentoDoPedido)
                                {
                                    sessaoRoteirizadorPedido.Pedido.ControleNumeracao = sessaoRoteirizadorPedido.Pedido.Codigo;
                                    sessaoRoteirizadorPedido.Pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                                    repPedidos.Atualizar(sessaoRoteirizadorPedido.Pedido);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, sessaoRoteirizadorPedido.Pedido, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CanceladoReservaDoPedidoNaSessaoDeRoteirizacao, sessao?.Descricao), unitOfWork);

                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidoNaoAtendido = (from produto in produtosPedidosNaoAtendido
                                                                                                                          where produto.Pedido.Codigo == sessaoRoteirizadorPedido.Pedido.Codigo
                                                                                                                          select produto).ToList();

                                    CancelarReserva(sessaoRoteirizadorPedido.Pedido, produtosPedidoNaoAtendido, tiposIntegracao, sessao.Usuario, unitOfWork);

                                    //#45088 - Se tiver um pedido do "TRAZ" relacionado.. devemos cancelar tambem...
                                    if (sessaoRoteirizadorPedido.Pedido.PedidoDevolucao != null)
                                    {
                                        sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.ControleNumeracao = sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.Codigo;
                                        sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.SituacaoPedido = SituacaoPedido.Cancelado;
                                        repPedidos.Atualizar(sessaoRoteirizadorPedido.Pedido.PedidoDevolucao);

                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, sessaoRoteirizadorPedido.Pedido.PedidoDevolucao, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CanceladoPedidoNaSessaoDeRoteirizacao, sessao?.Descricao), unitOfWork);

                                        produtosPedidoNaoAtendido = repositorioPedidoProduto.ProdutosPedidosNaoAtendidosTotalmente(new List<int> { sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.Codigo });

                                        CancelarReserva(sessaoRoteirizadorPedido.Pedido.PedidoDevolucao, produtosPedidoNaoAtendido, tiposIntegracao, sessao.Usuario, unitOfWork);
                                    }
                                }
                                else
                                {
                                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidoNaoAtendido = (from prod in produtosPedidosNaoAtendido
                                                                                                                          where prod.Pedido.Codigo == sessaoRoteirizadorPedido.Pedido.Codigo
                                                                                                                          select prod).ToList();
                                    if (produtosPedidoNaoAtendido?.Count > 0)
                                    {
                                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosCarregadoPedido = (from prod in produtosCarregadoPedidos
                                                                                                                                                     where prod.PedidoProduto.Pedido.Codigo == sessaoRoteirizadorPedido.Pedido.Codigo
                                                                                                                                                     select prod).ToList();

                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosCancelar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in produtosPedidoNaoAtendido)
                                        {
                                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtoCarregado = produtosCarregadoPedido?.FindAll(x => x.PedidoProduto.Codigo == produto.Codigo);
                                            decimal qtde = 0;
                                            decimal peso = 0;
                                            decimal pallet = 0;
                                            decimal metro = 0;
                                            if (produtoCarregado != null)
                                            {
                                                qtde = produtoCarregado.Sum(x => x.Quantidade);
                                                peso = produtoCarregado.Sum(x => x.Peso);
                                                pallet = produtoCarregado.Sum(x => x.QuantidadePallet);
                                                metro = produtoCarregado.Sum(x => x.MetroCubico);
                                            }

                                            if (qtde < produto.Quantidade || peso < produto.PesoTotal || pallet < produto.QuantidadePalet || metro < produto.MetroCubico)
                                            {
                                                produtosCancelar.Add(new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
                                                {
                                                    Codigo = produto.Codigo,
                                                    MetroCubico = produto.MetroCubico - metro,
                                                    Quantidade = produto.Quantidade - qtde,
                                                    QuantidadePalet = produto.QuantidadePalet - pallet,
                                                    PesoUnitario = produto.PesoUnitario,
                                                    PesoTotalEmbalagem = produto.PesoTotalEmbalagem
                                                });

                                                //Se teve algum carregamento do produto.. vamos atualizar as qtdes do pedido/produto.
                                                if (qtde + peso + pallet + metro > 0)
                                                {
                                                    produto.Quantidade = qtde;
                                                    produto.QuantidadePalet = pallet;
                                                    produto.MetroCubico = metro;
                                                    repositorioPedidoProduto.Atualizar(produto);
                                                }
                                            }
                                        }

                                        CancelarReserva(sessaoRoteirizadorPedido.Pedido, produtosCancelar, tiposIntegracao, sessao.Usuario, unitOfWork);

                                        // Aqui, vamos descontar do peso total do pedido... o peso que foi cancelado a reserva...
                                        decimal pesoTotalCancelado = (from obj in produtosCancelar select obj.PesoTotal).Sum();
                                        if (pesoTotalCancelado > 0)
                                        {
                                            sessaoRoteirizadorPedido.Pedido.PesoTotal -= pesoTotalCancelado;
                                            sessaoRoteirizadorPedido.Pedido.PesoSaldoRestante -= pesoTotalCancelado;
                                            sessaoRoteirizadorPedido.Pedido.PedidoTotalmenteCarregado = (sessaoRoteirizadorPedido.Pedido.PesoSaldoRestante <= (decimal)0.5);
                                            repPedidos.Atualizar(sessaoRoteirizadorPedido.Pedido);
                                            //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                                            Servicos.Log.TratarErro($"Pedido {sessaoRoteirizadorPedido.Pedido.NumeroPedidoEmbarcador} - Atualizou saldo pedido {sessaoRoteirizadorPedido.Pedido.PesoSaldoRestante} - Peso Total.: {sessaoRoteirizadorPedido.Pedido.PesoTotal} - Totalmente carregado.: {sessaoRoteirizadorPedido.Pedido.PedidoTotalmenteCarregado}. SessaoRoteirizadorController.RemoverPedidos", "SaldoPedido");
                                        }

                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, sessaoRoteirizadorPedido.Pedido, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CanceladoReservaParcialDoPedidoNaSessaoDeRoteirizacao, sessao?.Descricao), unitOfWork);
                                    }

                                    //#45088 - Se tiver um pedido do "TRAZ" relacionado.. devemos cancelar tambem...
                                    if (sessaoRoteirizadorPedido.Pedido.PedidoDevolucao != null)
                                    {
                                        sessaoRoteirizadorPedido.Pedido.ControleNumeracao = sessaoRoteirizadorPedido.Pedido.Codigo;
                                        sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.SituacaoPedido = SituacaoPedido.Cancelado;
                                        repPedidos.Atualizar(sessaoRoteirizadorPedido.Pedido.PedidoDevolucao);

                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, sessaoRoteirizadorPedido.Pedido.PedidoDevolucao, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.CanceladoPedidoNaSessaoDeRoteirizacao, sessao?.Descricao), unitOfWork);

                                        produtosPedidoNaoAtendido = repositorioPedidoProduto.ProdutosPedidosNaoAtendidosTotalmente(new List<int> { sessaoRoteirizadorPedido.Pedido.PedidoDevolucao.Codigo });

                                        CancelarReserva(sessaoRoteirizadorPedido.Pedido.PedidoDevolucao, produtosPedidoNaoAtendido, tiposIntegracao, sessao.Usuario, unitOfWork);
                                    }
                                }
                            }

                            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> sessaoRoteirizadorPedidoProduto = repositorioSessaoRoteirizadorPedidoProduto.BuscarSessaoRoteirizadorPorPedido(sessaoRoteirizadorPedido.Pedido.Codigo);
                            bool temControlePorArmazem = sessaoRoteirizadorPedidoProduto.Exists(sr => sr.PedidoProduto.FilialArmazem != null);
                            if (temControlePorArmazem)
                                RetornarQuantidadeProdutoEmSessaoParaEstoqueArmazem(sessaoRoteirizadorPedidoProduto, unitOfWork);

                            repositorioSessaoRoteirizadorPedido.Atualizar(sessaoRoteirizadorPedido);

                            unitOfWork.CommitChanges();
                        }
                        catch (Exception ex2)
                        {
                            erro++;
                            unitOfWork.Rollback();
                            Servicos.Log.TratarErro(ex2);
                        }
                    }
                }

                if (pedidosSessao.Count() > 0 && configuracaoEmbarcador.RoteirizacaoObrigatoriaMontagemCarga)
                {
                    //precisamos deletar a roteirizacao do carregamento para forçar nova roteirizacao apos a remoção de pedidos da sessao.
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosPendentes = repositorioCarregamento.CarregamentosSessaoRoteirizador(codigoSessaoRoteirizador);

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in carregamentosPendentes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repositorioCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);
                        if (carregamentoRoteirizacao != null)
                        {
                            repositorioCarregamentoRoteirizacaoClientesRota.DeletarPorCarregamentoRoteirizado(carregamentoRoteirizacao.Codigo);
                            repositorioCarregamentoRoteirizacaoPontosPassagem.DeletarPorCarregamentoRoteirizado(carregamentoRoteirizacao.Codigo);
                            repositorioCarregamentoRoteirizacao.Deletar(carregamentoRoteirizacao);
                        }
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, sessao, null, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.RemovidoPedidosDaSessaoDeRoteirizacao, pedidosSessao?.Count ?? 0), unitOfWork);

                if (erro == codigosPedidos.Count)
                    return new JsonpResult(false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelPedidos, (cancelarReserva ? Localization.Resources.Cargas.MontagemCargaMapa.CancelarReservaDos : Localization.Resources.Cargas.MontagemCargaMapa.RemoverOs)));
                else if (erro > 0)
                    return new JsonpResult(erro, false, string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoFoiPossivelPedidosDoisParametros, (cancelarReserva ? Localization.Resources.Cargas.MontagemCargaMapa.CancelarReservaDe : Localization.Resources.Gerais.Geral.Remover), erro.ToString()));
                else
                    return new JsonpResult(true, (cancelarReserva ? Localization.Resources.Cargas.MontagemCargaMapa.CancelamentoDeReservaRealizadoComSucesso : Localization.Resources.Cargas.MontagemCargaMapa.PedidosRemovidosDaSessaoComSucesso));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoRemoverOsPedidosDaSessaoDeRoteirizacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void CancelarReserva(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedidoNaoAtendido, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
                Servicos.Embarcador.Integracao.IntegracaoPedido.GerarIntegracaoPedidoCancelamentoReserva(tipoIntegracao, pedido, produtosPedidoNaoAtendido, usuario, unitOfWork);
        }

        /// <summary>
        /// Procedimento para consultar os ´pedidos inconsistentes de uma sessão do roteirizador quando ocorreu algum erro no processo de montagem de carregamento automático.
        ///  Será retornado somente se o pedido não estiver em nenhum carregamento da sessão, pois o usuário pode ter adicionado manualmente.
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAuthenticate]
        public async Task<IActionResult> ObterPedidosInconsistentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> pedidos = repSessaoPedidos.PedidosInconsistentesCarregamentoAutomatico(codigo);
                var result = new
                {
                    Registros = (from obj in pedidos
                                 select new
                                 {
                                     obj.Pedido.Codigo,
                                     obj.Situacao,
                                     DescricaoSituacao = SituacaoSessaoRoteirizadorPedidoHelper.ObterDescricao(obj.Situacao)
                                 }).ToList()
                };
                return new JsonpResult(result);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoBuscarSessaoDeRoteirizacaoPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region "Simulações de Frete"

        [AllowAuthenticate]
        public async Task<IActionResult> AgruparCarregamentosSimuladorFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSessaoRoteirizador = Request.GetIntParam("Codigo");
                List<int> codigosMontagemCarregamentoBlocoSimuladorFrete = Request.GetListParam<int>("Codigos");

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete repositorioMontagemCarregamentoBlocoSimuladorFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = repositorioMontagemCarregamentoBlocoSimuladorFrete.BuscarPorCodigos(codigosMontagemCarregamentoBlocoSimuladorFrete);
                if (montagemCarregamentoBlocoSimuladorFretes.Count != codigosMontagemCarregamentoBlocoSimuladorFrete.Count)
                    return new JsonpResult(false, "Não foi possível identificar um ou mais simulador de frete selecionado.");

                List<Dominio.Entidades.Empresa> transportadores = (from obj in montagemCarregamentoBlocoSimuladorFretes select obj.Transportador).Distinct().ToList();
                if (transportadores.Count > 1)
                    return new JsonpResult(false, "Não é permitido agrupar carregamentos de Transportadores distintos.");

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = (from obj in montagemCarregamentoBlocoSimuladorFretes select obj.TipoOperacao).Distinct().ToList();
                if (tiposOperacoes.Count > 1)
                    return new JsonpResult(false, "Não é permitido agrupar carregamentos de Tipos de operações distintas.");

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao = null;
                // Agora, precisar ver no centro de carregamento, os tipos de operação...
                if (tiposOperacoes.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                    Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                    Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = repositorioSessaoRoteirizador.BuscarPorCodigo(codigoSessaoRoteirizador);
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(sessaoRoteirizador?.Filial?.Codigo ?? 0);
                    List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> carregamentoTipoOperacoes = repositorioTipoOperacao.BuscarPorCentro(centroCarregamento?.Codigo ?? 0);

                    centroCarregamentoTipoOperacao = (from obj in carregamentoTipoOperacoes where obj.TipoOperacao.Codigo == tiposOperacoes[0].Codigo select obj).FirstOrDefault();

                    if (centroCarregamentoTipoOperacao == null)
                        return new JsonpResult(false, $"Não foi possível identificar a parametrização do centro de carregamento para o tipo de operação {tiposOperacoes[0].Descricao}.");

                    if (centroCarregamentoTipoOperacao.Tipo != CentroCarregamentoTipoOperacaoTipo.TotalCliente)
                        return new JsonpResult(false, $"Somente tipo de operação com carregamento Total do cliente é permitido agrupar.");
                }

                // Agora, achar os carregamentos dos blocos, e agrupar ambos em um carregamento..
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete montagemCarregamentoBlocoSimuladorFrete in montagemCarregamentoBlocoSimuladorFretes)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repositorioMontagemCarregamento.BuscarPorMontagemCarregamentoBloco(montagemCarregamentoBlocoSimuladorFrete.Bloco.Codigo);
                    if (carregamento == null)
                        return new JsonpResult(false, $"Não foi possível o carregamento do bloco {montagemCarregamentoBlocoSimuladorFrete.Bloco.Codigo}.");

                    if (carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                        return new JsonpResult(false, $"Não é permitido alterar o vencedor do carregamento {carregamento.NumeroCarregamento} pois a situação do carregamento está {SituacaoCarregamentoHelper.ObterDescricao(carregamento.SituacaoCarregamento)}.");

                    carregamentos.Add(carregamento);
                }

                // Gerar um novo carregamento, e cancelar os anteriores..
                unitOfWork.Start();

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento novoCarregamento = servicoMontagemCarga.AgruparCarregamentos(carregamentos, tiposOperacoes.FirstOrDefault(), centroCarregamentoTipoOperacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, $"Carregamentos agrupados com sucesso.");
            }
            catch (ServicoException exe)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exe);

                return new JsonpResult(false, true, exe.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as simulações de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AgruparVencedoresSimuladorFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSessaoRoteirizador = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco repositorioMontagemCarregamentoBloco = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete repositorioMontagemCarregamentoBlocoSimuladorFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                //Buscar todos os carregamentos pendentes da sessão (Em Montagem)
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosPendentes = repositorioMontagemCarregamento.CarregamentosSessaoRoteirizador(codigoSessaoRoteirizador);
                carregamentosPendentes = (from obj in carregamentosPendentes where obj.MontagemCarregamentoBloco != null select obj).ToList();

                if (carregamentosPendentes.Count == 0)
                    return new JsonpResult(false, $"Nenhum carregamento pendente para agrupar os vencedores.");

                List<int> codigosBlocos = (from obj in carregamentosPendentes where obj.MontagemCarregamentoBloco != null select obj.MontagemCarregamentoBloco.Codigo).Distinct().ToList();

                //Buscando todos os blocos da sessão referentes aos carregamentos pendentes......
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> montagemCarregamentoBlocos = repositorioMontagemCarregamentoBloco.BuscarPorCodigos(codigosBlocos);
                if (montagemCarregamentoBlocos.Count == 0)
                    return new JsonpResult(false, $"Nenhum bloco de simulação de frete encontrado para os carregamentos pendentes.");

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = repositorioMontagemCarregamentoBlocoSimuladorFrete.BuscarPorBlocos(codigosBlocos);

                var grupoVencedores = (from osf in montagemCarregamentoBlocoSimuladorFretes
                                       join mcb in montagemCarregamentoBlocos on osf.Bloco.Codigo equals mcb.Codigo
                                       join cpe in carregamentosPendentes on mcb.Codigo equals cpe.MontagemCarregamentoBloco.Codigo into j1
                                       where osf.Vencedor && osf.Tipo == CentroCarregamentoTipoOperacaoTipo.TotalCliente
                                       from j2 in j1.DefaultIfEmpty()
                                       group j2 by new { CodigoTransportador = osf.Transportador.Codigo, CodigoTipoOperacao = osf.TipoOperacao.Codigo } into grouped
                                       select new
                                       {
                                           grouped.Key.CodigoTransportador,
                                           grouped.Key.CodigoTipoOperacao,
                                           Count = grouped.Count()
                                       }
                                      ).ToList();

                // Filtrando apenas os transportadores que venceram mais de uma disputa...
                grupoVencedores = (from obj in grupoVencedores where obj.Count > 1 select obj).ToList();

                if (grupoVencedores.Count == 0)
                    return new JsonpResult(false, $"Nenhum transportador venceu mais de um bloco de simulação de frete.");

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao centroCarregamentoTipoOperacao = null;

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Logistica.CentroCarregamentoTipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = repositorioSessaoRoteirizador.BuscarPorCodigo(codigoSessaoRoteirizador);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(sessaoRoteirizador?.Filial?.Codigo ?? 0);
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTipoOperacao> carregamentoTipoOperacoes = repositorioTipoOperacao.BuscarPorCentro(centroCarregamento?.Codigo ?? 0);

                centroCarregamentoTipoOperacao = (from obj in carregamentoTipoOperacoes where obj.TipoOperacao.Codigo == (from to in grupoVencedores select to.CodigoTipoOperacao).FirstOrDefault() select obj).FirstOrDefault();

                if (centroCarregamentoTipoOperacao.Tipo != CentroCarregamentoTipoOperacaoTipo.TotalCliente)
                    return new JsonpResult(false, $"Somente tipo de operação com carregamento Total do cliente é permitido agrupar.");

                int agrupadosComSucesso = 0;
                string msgErro = string.Empty;

                foreach (var grupo in grupoVencedores)
                {
                    try
                    {
                        unitOfWork.Start();

                        Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

                        // Agora.. precisamos achar os carregamentos deste vencedor...
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = (from obj in carregamentosPendentes
                                                                                                              where obj.Empresa.Codigo == grupo.CodigoTransportador
                                                                                                              select obj).ToList();

                        if (carregamentos.Count < 2)
                            continue;

                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento novoCarregamento = servicoMontagemCarga.AgruparCarregamentos(carregamentos, (from car in carregamentos select car.TipoOperacao).FirstOrDefault(), centroCarregamentoTipoOperacao);

                        unitOfWork.CommitChanges();

                        agrupadosComSucesso++;
                    }
                    catch (ServicoException exe)
                    {
                        unitOfWork.Rollback();
                        msgErro = exe.Message;
                        Servicos.Log.TratarErro(exe);
                    }
                    catch (Exception exe)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(exe);
                    }
                }

                if (agrupadosComSucesso > 0)
                    return new JsonpResult(true, $"Foram agrupados {agrupadosComSucesso} carregamentos com sucesso.");
                else
                    return new JsonpResult(false, "Nenhum agrupamento foi realizado. " + msgErro);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao agrupar os vencedores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> CancelarSimuladorFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSessaoRoteirizador = Request.GetIntParam("Codigo");
                List<int> codigosMontagemCarregamentoBlocoSimuladorFrete = Request.GetListParam<int>("Codigos");

                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete repositorioMontagemCarregamentoBlocoSimuladorFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> montagemCarregamentoBlocoSimuladorFretes = repositorioMontagemCarregamentoBlocoSimuladorFrete.BuscarPorCodigos(codigosMontagemCarregamentoBlocoSimuladorFrete);

                List<int> listaCodigoCarregamento = new List<int>();
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete montagemCarregamentoBlocoSimuladorFrete in montagemCarregamentoBlocoSimuladorFretes)
                {
                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repositorioMontagemCarregamento.BuscarPorMontagemCarregamentoBloco(montagemCarregamentoBlocoSimuladorFrete.Bloco.Codigo);
                    if (carregamento == null)
                        return new JsonpResult(false, $"Não foi possível o carregamento do bloco {montagemCarregamentoBlocoSimuladorFrete.Bloco.Codigo}.");

                    if (carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                        return new JsonpResult(false, $"Não é permitido alterar o vencedor do carregamento {carregamento.NumeroCarregamento} pois a situação do carregamento está {SituacaoCarregamentoHelper.ObterDescricao(carregamento.SituacaoCarregamento)}.");

                    listaCodigoCarregamento.Add(carregamento.Codigo);
                }

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga serMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

                unitOfWork.Start();

                string msg = string.Empty;
                bool valida = false;
                bool retorno = serMontagem.CancelarCarregamentos(listaCodigoCarregamento, ref valida, ref msg, false, this.Usuario, true, false, unitOfWork);

                if (retorno)
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, $"Simulações de frete e carregamento cancelado com sucesso.");
                }
                else
                {
                    return new JsonpResult(false, true, msg);
                }
            }
            catch (ServicoException exe)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exe.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as simulações de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarVencedorSimulacaoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMontagemCarregamentoBlocoSimuladorFrete = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete repositorioMontagemCarregamentoBlocoSimuladorFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete montagemCarregamentoBlocoSimuladorFrete = repositorioMontagemCarregamentoBlocoSimuladorFrete.BuscarPorCodigo(codigoMontagemCarregamentoBlocoSimuladorFrete, false);
                if (montagemCarregamentoBlocoSimuladorFrete == null)
                    return new JsonpResult(false, "Não foi possível identificar o simulador de frete selecionado.");

                if (montagemCarregamentoBlocoSimuladorFrete.Vencedor)
                    return new JsonpResult(false, "O simulador de frete selecionado já é o vencedor.");

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repositorioMontagemCarregamento.BuscarPorMontagemCarregamentoBloco(montagemCarregamentoBlocoSimuladorFrete.Bloco.Codigo);

                if (carregamento == null)
                    carregamento = montagemCarregamentoBlocoSimuladorFrete.Bloco.Carregamento;

                if (carregamento == null)
                    return new JsonpResult(false, $"Não foi possível localizar o carregamento do bloco {montagemCarregamentoBlocoSimuladorFrete.Bloco.Codigo}.");

                if (carregamento.SituacaoCarregamento != SituacaoCarregamento.EmMontagem)
                    return new JsonpResult(false, $"Não é permitido alterar o vencedor do carregamento {carregamento.NumeroCarregamento} pois a situação do carregamento está {SituacaoCarregamentoHelper.ObterDescricao(carregamento.SituacaoCarregamento)}.");

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete montagemCarregamentoBlocoSimuladorFreteVencedorBloco = repositorioMontagemCarregamentoBlocoSimuladorFrete.BuscarVencedorBloco(montagemCarregamentoBlocoSimuladorFrete.Bloco.Codigo);

                unitOfWork.Start();

                if (montagemCarregamentoBlocoSimuladorFreteVencedorBloco != null)
                {
                    montagemCarregamentoBlocoSimuladorFreteVencedorBloco.Vencedor = false;
                    repositorioMontagemCarregamentoBlocoSimuladorFrete.Atualizar(montagemCarregamentoBlocoSimuladorFreteVencedorBloco);
                }

                carregamento.TipoDeCarga = montagemCarregamentoBlocoSimuladorFrete.TipoDeCarga;
                carregamento.ModeloVeicularCarga = montagemCarregamentoBlocoSimuladorFrete.ModeloVeicularCarga;
                carregamento.TipoOperacao = montagemCarregamentoBlocoSimuladorFrete.TipoOperacao;
                carregamento.Empresa = montagemCarregamentoBlocoSimuladorFrete.Transportador;
                carregamento.ValorFrete = montagemCarregamentoBlocoSimuladorFrete.ValorTotalSimulacao;
                carregamento.ExigeIsca = montagemCarregamentoBlocoSimuladorFrete.ExigeIsca;

                repositorioMontagemCarregamento.Atualizar(carregamento);

                montagemCarregamentoBlocoSimuladorFrete.Vencedor = true;
                repositorioMontagemCarregamentoBlocoSimuladorFrete.Atualizar(montagemCarregamentoBlocoSimuladorFrete);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, $"Vencedor do frete do carregamento {carregamento.NumeroCarregamento} alterado com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as simulações de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterSimulacoesFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSessaoRoteirizador = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco repositorioMontagemCarregamentoBloco = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido repositorioMontagemCarregamentoBlocoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete repositorioMontagemCarregamentoBlocoSimuladorFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido repositorioMontagemCarregamentoBlocoSimuladorFretePedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repositorioSessaoRoteirizador = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessaoRoteirizador = repositorioSessaoRoteirizador.BuscarPorCodigo(codigoSessaoRoteirizador);

                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = servicoMontagemCarga.ObterSessaoRoteirizadorParametros(sessaoRoteirizador);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco> montagemCarregamentoBlocos = repositorioMontagemCarregamentoBloco.BuscarPorSessaoRoteirizador(codigoSessaoRoteirizador);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete> simuladorFretes = repositorioMontagemCarregamentoBlocoSimuladorFrete.BuscarPorBlocos((from o in montagemCarregamentoBlocos select o.Codigo).ToList());
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simuladorFretesPedidos = repositorioMontagemCarregamentoBlocoSimuladorFretePedido.BuscarPorBlocosSimuladoresFretes((from o in simuladorFretes select o.Codigo).ToList());
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = repositorioCarregamentoPedido.BuscarTodosCarregamentosPorPedidos((from obj in simuladorFretesPedidos select obj.Pedido.Codigo).ToList(), false, false);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tipoOperacoes = repositorioTipoOperacao.BuscarTodosAtivos();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorCarregamentos(carregamentosPedidos.Select(x => x.Carregamento.Codigo).Distinct().ToList());

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteCarregamento> carregamentos = (from ped in simuladorFretesPedidos
                                                                                                                        where ped.SimuladorFrete.Vencedor
                                                                                                                        select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteCarregamento()
                                                                                                                        {
                                                                                                                            //CodigoBloco = ped.SimuladorFrete.Bloco.Codigo,
                                                                                                                            CodigoSimuladorFrete = ped.SimuladorFrete.Codigo,
                                                                                                                            CodigoPedido = ped.Pedido.Codigo,
                                                                                                                            DataCarregamento = (from car in carregamentosPedidos where car.Pedido.Codigo == ped.Pedido.Codigo select car.Carregamento.DataCarregamentoCarga).FirstOrDefault(),
                                                                                                                            CodigoCarregamento = (from car in carregamentosPedidos where car.Pedido.Codigo == ped.Pedido.Codigo select car.Carregamento.Codigo).FirstOrDefault(),
                                                                                                                            NumeroCarregamento = (from car in carregamentosPedidos where car.Pedido.Codigo == ped.Pedido.Codigo select car.Carregamento.NumeroCarregamento).FirstOrDefault(),
                                                                                                                            ExigeIsca = (from car in carregamentosPedidos where car.Pedido.Codigo == ped.Pedido.Codigo && car.Carregamento.ExigeIsca select car.Carregamento.ExigeIsca).Any()
                                                                                                                        }).ToList();

                simuladorFretes = simuladorFretes.OrderBy(x => x.Bloco.Codigo).ThenBy(x => x.Ranking).ThenByDescending(x => x.ModeloVeicularCarga.CapacidadePesoTransporte).ToList();

                var result = new
                {
                    Registros = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteResult>()
                };

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete obj in simuladorFretes)
                {

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosSimuladorFretes = (from ped in simuladorFretesPedidos
                                                                                                where ped.SimuladorFrete.Codigo == obj.Codigo
                                                                                                select ped.Pedido).ToList();

                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteCarregamento> carregamentosSimulador = (from car in carregamentos
                                                                                                                                     where car.CodigoSimuladorFrete == obj.Codigo
                                                                                                                                     select car).ToList();
                    List<int> codigosCarregamento = (from c in carregamentosSimulador
                                                     select c.CodigoCarregamento).Distinct().ToList();

                    if (codigosCarregamento.Count == 0 && obj.Bloco.Carregamento != null)
                        codigosCarregamento.Add(obj.Bloco.Carregamento.Codigo);

                    List<long> destinatariosCarregamento = (from cp in carregamentosPedidos
                                                            where codigosCarregamento.Contains(cp.Carregamento.Codigo)
                                                            select cp.Pedido.Destinatario.Codigo).Distinct().ToList();

                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteResult item = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteResult()
                    {
                        Codigo = obj.Codigo,
                        Cliente = ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete || destinatariosCarregamento.Count == 1 ? obj.Bloco.Cliente.Descricao : string.Empty),
                        Transportador = obj.Transportador.Descricao,
                        TipoDeCarga = obj.TipoDeCarga?.Descricao ?? obj.Bloco?.Carregamento?.TipoDeCarga?.Descricao ?? string.Empty,
                        NumeroCarregamento = ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete ? string.Join(", ", (from car in carregamentosSimulador select car.NumeroCarregamento).Distinct().ToList()) : obj.Bloco?.Carregamento?.NumeroCarregamento ?? string.Empty),
                        GrossSales = (from ped in pedidosSimuladorFretes select ped.GrossSales).Sum(),
                        ValorMinimoCargaCliente = (from ped in pedidosSimuladorFretes select ped.Destinatario.ValorMinimoCarga).FirstOrDefault() ?? 0,
                        Destino = string.Join(", ", (from ped in pedidosSimuladorFretes select ped.Destino.Descricao).Distinct().ToList()),
                        Estado = string.Join(", ", (from ped in pedidosSimuladorFretes select ped.Destino.Estado.Sigla).Distinct().ToList()),
                        Regiao = string.Join(", ", (from ped in pedidosSimuladorFretes select ped.Destino.Estado.RegiaoBrasil.Descricao).Distinct().ToList()),
                        ExigeIsca = IsExigeIsca(obj, carregamentosSimulador),
                        ModeloVeicular = obj.ModeloVeicularCarga.Descricao,
                        TipoOperacao = obj.TipoOperacao.Descricao,
                        PesoTotal = (from ped in pedidosSimuladorFretes select ped.PesoTotal).Sum(),
                        MetroCubicoTotal = (from ped in pedidosSimuladorFretes select ped.CubagemTotal).Sum(),
                        QuantidadePalletTotal = (from ped in pedidosSimuladorFretes select ped.TotalPallets).Sum(),
                        VolumesTotal = (from ped in pedidosSimuladorFretes select ped.QtVolumes).Sum(),
                        Quantidade = obj.Quantidade,
                        Ranking = obj.Ranking,
                        DataCarregamento = ((from car in carregamentosSimulador select car.DataCarregamento).Distinct().FirstOrDefault() ?? obj.Bloco?.Carregamento?.DataCarregamentoCarga)?.ToString("dd/MM/yyyy") ?? "",
                        ValorTotal = obj.ValorTotal.ToString("c"),
                        ValorTotalSimulacao = obj.ValorTotalSimulacao.ToString("c"),
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBlocoHelper.ObterDescricao(obj.Bloco.Situacao),
                        Vencedor = (obj.Vencedor ? "SIM" : "NÃO"),
                        Observacao = obj.Bloco.Observacao,
                        LeadTime = obj.LeadTime,
                        ValorLimiteNaCargaTipoOperacao = (from toc in tipoOperacoes
                                                          where toc.Codigo == obj.TipoOperacao.Codigo
                                                          select toc.ConfiguracaoCarga?.ValorLimiteNaCarga ?? 0).FirstOrDefault(),
                        Limite = string.Empty,
                        DT_Enable = true,
                        DT_RowId = obj.Codigo
                    };

                    item.CargaGerada = IsCargaGerada(obj.Codigo, cargas, simuladorFretesPedidos, carregamentosPedidos);
                    item.Expedicao = (item.CargaGerada ? "SIM" : "NÃO");

                    string rowColor = string.Empty;
                    item.Limite = SimuladorFreteLimite(obj, simuladorFretesPedidos, tipoOperacoes, ref rowColor, false);
                    item.DT_RowColor = rowColor;

                    result.Registros.Add(item);
                }

                //Adicionando a lis da blocos que apresentou erro ao gerar a simulação de frete...
                montagemCarregamentoBlocos = montagemCarregamentoBlocos.FindAll(x => !simuladorFretes.Any(s => s.Bloco.Codigo == x.Codigo));
                montagemCarregamentoBlocos = montagemCarregamentoBlocos.OrderBy(x => x.Carregamento?.NumeroCarregamento ?? string.Empty).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido> montagemCarregamentoBlocoPedidos = repositorioMontagemCarregamentoBlocoPedido.BuscarPorBlocos((from o in montagemCarregamentoBlocos select o.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco obj in montagemCarregamentoBlocos)
                {

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosSimuladorFretes = (from ped in montagemCarregamentoBlocoPedidos
                                                                                                where ped.Bloco.Codigo == obj.Codigo
                                                                                                select ped.Pedido).ToList();

                    List<int> codigosCarregamento = new List<int>() { obj.Carregamento?.Codigo ?? 0 };

                    List<long> destinatariosCarregamento = (from cp in pedidosSimuladorFretes
                                                            select cp.Destinatario.Codigo).Distinct().ToList();

                    Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteResult item = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteResult()
                    {
                        Codigo = obj.Codigo,
                        Cliente = ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete || destinatariosCarregamento.Count == 1 ? obj.Cliente.Descricao : string.Empty),
                        Transportador = obj.Transportador?.Descricao ?? string.Empty,
                        TipoDeCarga = obj.Carregamento?.TipoDeCarga?.Descricao ?? (from o in pedidosSimuladorFretes where o.TipoDeCarga != null select o.TipoDeCarga?.Descricao)?.FirstOrDefault() ?? string.Empty,
                        NumeroCarregamento = obj.Carregamento?.NumeroCarregamento ?? string.Empty,
                        GrossSales = (from ped in pedidosSimuladorFretes select ped.GrossSales).Sum(),
                        ValorMinimoCargaCliente = (from ped in pedidosSimuladorFretes select ped.Destinatario.ValorMinimoCarga).FirstOrDefault() ?? 0,
                        Destino = string.Join(", ", (from ped in pedidosSimuladorFretes select ped.Destino.Descricao).Distinct().ToList()),
                        Estado = string.Join(", ", (from ped in pedidosSimuladorFretes select ped.Destino.Estado.Sigla).Distinct().ToList()),
                        Regiao = string.Join(", ", (from ped in pedidosSimuladorFretes select ped.Destino.Estado.RegiaoBrasil.Descricao).Distinct().ToList()),
                        ExigeIsca = obj.Carregamento?.ExigeIsca ?? false ? "SIM" : "NÃO",
                        ModeloVeicular = obj.Carregamento?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                        TipoOperacao = obj.Carregamento?.TipoOperacao?.Descricao ?? string.Empty,
                        PesoTotal = obj.PesoTotal,
                        MetroCubicoTotal = obj.MetroCubicoTotal,
                        QuantidadePalletTotal = obj.QuantidadePalletTotal,
                        VolumesTotal = (int)obj.VolumesTotal,
                        Quantidade = 0,
                        Ranking = 0,
                        DataCarregamento = obj.Carregamento?.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? string.Empty,
                        ValorTotal = 0.ToString("c"),
                        ValorTotalSimulacao = 0.ToString("c"),
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBlocoHelper.ObterDescricao(obj.Situacao),
                        Vencedor = "NÃO",
                        Observacao = obj.Observacao,
                        LeadTime = obj.LeadTimeFretes,
                        ValorLimiteNaCargaTipoOperacao = (from toc in tipoOperacoes
                                                          where toc.Codigo == (obj.Carregamento?.TipoOperacao?.Codigo ?? 0)
                                                          select toc.ConfiguracaoCarga?.ValorLimiteNaCarga ?? 0).FirstOrDefault(),
                        Limite = string.Empty,
                        DT_Enable = true,
                        DT_RowId = obj.Codigo,
                        DT_RowColor = ""
                    };

                    item.CargaGerada = IsCargaGerada(obj.Codigo, cargas, simuladorFretesPedidos, carregamentosPedidos);
                    item.Expedicao = (item.CargaGerada ? "SIM" : "NÃO");

                    result.Registros.Add(item);
                }

                //result.Registros.AddRange(
                //    (from obj in montagemCarregamentoBlocos
                //     select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteResult()
                //     {
                //         Codigo = obj.Codigo,
                //         Cliente = ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete ? obj.Cliente.Descricao : string.Empty),
                //         Transportador = obj.Transportador?.Descricao ?? string.Empty,
                //         TipoDeCarga = obj.Carregamento?.TipoDeCarga?.Descricao ?? string.Empty,
                //         NumeroCarregamento = obj.Carregamento?.NumeroCarregamento ?? string.Empty,
                //         CargaGerada = false,
                //         Expedicao = "NÃO",
                //         GrossSales = 0m,
                //         ValorMinimoCargaCliente = 0m,
                //         Destino = string.Empty,
                //         Estado = string.Empty,
                //         ExigeIsca = string.Empty,
                //         ModeloVeicular = string.Empty,
                //         TipoOperacao = string.Empty,
                //         PesoTotal = obj.PesoTotal,
                //         MetroCubicoTotal = obj.MetroCubicoTotal,
                //         QuantidadePalletTotal = obj.QuantidadePalletTotal,
                //         VolumesTotal = (int)obj.VolumesTotal,
                //         Quantidade = 0,
                //         Ranking = 0,
                //         DataCarregamento = ((sessaoRoteirizadorParametros?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum) == TipoMontagemCarregamentoVRP.SimuladorFrete ? obj.Carregamento?.DataCarregamentoCarga?.ToString("dd/MM/yyyy") : string.Empty),
                //         ValorTotalSimulacao = 0.ToString("c"),
                //         Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBlocoHelper.ObterDescricao(obj.Situacao),
                //         Vencedor = "NÃO",
                //         Observacao = obj.Observacao,
                //         LeadTime = obj.LeadTimeFretes,
                //         ValorLimiteNaCargaTipoOperacao = 0m,
                //         Limite = string.Empty,
                //         DT_Enable = true,
                //         DT_RowId = obj.Codigo,
                //         DT_RowColor = ""
                //     }).ToList());

                return new JsonpResult(result);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as simulações de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// Procedimento para alterar os parametros do centro de carregamento para a sessão em específico.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SalvarParametrosSessaoRoteirizador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string parametros = Request.GetStringParam("Parametros");

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                //A cada usuários que abre a sessão, vamos mudar o usuário atual para bloquear a alteração dos carregamentos
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = repSessao.BuscarPorCodigo(codigo);
                sessao.UsuarioAtual = this.Usuario;
                sessao.Parametros = parametros;
                repSessao.Atualizar(sessao);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoSalvarOsParametrosDaSessao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterParametrosSessaoRoteirizador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador repSessao = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);

                //A cada usuários que abre a sessão, vamos mudar o usuário atual para bloquear a alteração dos carregamentos
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao = repSessao.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repCentroCarregamento.BuscarPorFiliais(new List<int> { sessao.Filial.Codigo });
                bool dataCarregamentoObrigatoriaMontagemCarga = centrosCarregamento.Exists(x => x.DataCarregamentoObrigatoriaMontagemCarga == true);

                string parametros = this.ObterJsonParametrosCentroCarregamento(sessao, centrosCarregamento, unitOfWork);

                return new JsonpResult(parametros);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoSalvarOsParametrosDaSessao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Pré filtros

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPreFiltrosMontagemCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                string filtro = Request.GetStringParam("Filtro");
                Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa tipoFiltro = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa>("TipoFiltro", Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa.MontagemCarregamento);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Codigo, "Codigo", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Nome, "NomeFiltro", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Usuario, "Nome", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Dados", false);

                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
                // #18857 - Teste rejeitado. Deixar que os filtros fiquem disponíveis para os demais usuários.
                List<Dominio.Entidades.Global.FiltroPesquisa> registros = repositorioFiltroPesquisa.BuscarFiltrosPesquisa(tipoFiltro, 0 /* this.Usuario.Codigo */, filtro);

                var lista = (
                    from p in registros
                    select new
                    {
                        p.Codigo,
                        NomeFiltro = p.Modelo?.Descricao ?? p.NomeFiltro,
                        p.Usuario.Nome,
                        p.Dados
                    }
                ).ToList();

                grid.setarQuantidadeTotal(lista.Count);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPreFiltrosMontagemCarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFiltroPesquisa = Request.GetIntParam("CodigoFiltro");
                Repositorio.FiltroPesquisa repositorioFiltroPesquisa = new Repositorio.FiltroPesquisa(unitOfWork);
                Repositorio.FiltroPesquisaModelo repositorioFiltroPesquisaModelo = new Repositorio.FiltroPesquisaModelo(unitOfWork);

                Dominio.Entidades.Global.FiltroPesquisa filtroPesquisa = repositorioFiltroPesquisa.BuscarPorCodigo(codigoFiltroPesquisa, false);

                await unitOfWork.StartAsync(cancellationToken);

                if (filtroPesquisa.Modelo != null)
                    repositorioFiltroPesquisaModelo.Deletar(filtroPesquisa.Modelo);

                repositorioFiltroPesquisa.Deletar(filtroPesquisa);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoExcluirFiltroDePesquisa);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarPreFiltrosMontagemCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFiltroPesquisa = Request.GetIntParam("CodigoFiltro");
                string nomeFiltro = Request.GetStringParam("NomeFiltro");
                string dadosFiltro = Request.GetStringParam("FiltroPesquisa");
                Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa tipoFiltro = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa>("TipoFiltro", Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa.MontagemCarregamento);

                SalvarFiltroPesquisa(codigoFiltroPesquisa, nomeFiltro, tipoFiltro, dadosFiltro, false, unitOfWork);
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoSalvarOsFiltrosDePesquisa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverPedidoProdutoDaSessao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoSessaoRoteirizador = Request.GetIntParam("CodigoSessaoRoteirizador");
                int codigoPedidoProduto = Request.GetIntParam("CodigoPedidoProduto");

                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repositorioSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem repositorioProdutoEmbarcadorEstoqueArmazem = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto sessaoRoteirizadorPedidoProduto = await repositorioSessaoRoteirizadorPedidoProduto.BuscarPorSessaoRoteirizadorEPedidoProdutoAsync(codigoSessaoRoteirizador, codigoPedidoProduto);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem produtoEmbarcadorEstoqueArmazem = await repositorioProdutoEmbarcadorEstoqueArmazem.BuscarPorFilialProdutoArmazemAsync(sessaoRoteirizadorPedidoProduto.PedidoProduto.FilialArmazem?.Filial?.Codigo ?? 0, sessaoRoteirizadorPedidoProduto.PedidoProduto.Produto.Codigo, sessaoRoteirizadorPedidoProduto.PedidoProduto?.FilialArmazem?.Codigo ?? 0);

                if (produtoEmbarcadorEstoqueArmazem != null)
                {
                    produtoEmbarcadorEstoqueArmazem.EstoqueSessaoRoterizacao -= sessaoRoteirizadorPedidoProduto.QuantidadeProdutoSessao;
                    produtoEmbarcadorEstoqueArmazem.EstoqueDisponivel += sessaoRoteirizadorPedidoProduto.QuantidadeProdutoSessao;

                    await repositorioProdutoEmbarcadorEstoqueArmazem.AtualizarAsync(produtoEmbarcadorEstoqueArmazem);
                }

                sessaoRoteirizadorPedidoProduto.PedidoProduto.Pedido.ItensAtualizados = true;
                await repositorioPedido.AtualizarAsync(sessaoRoteirizadorPedidoProduto.PedidoProduto.Pedido);

                await repositorioSessaoRoteirizadorPedidoProduto.DeletarAsync(sessaoRoteirizadorPedidoProduto);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um erro ao remover o produto da sessão");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private bool IsCargaGerada(int codigoSimuladorFrete, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, List<MontagemCarregamentoBlocoSimuladorFretePedido> simuladorFretesPedidos, List<CarregamentoPedido> carregamentosPedidos)
        {
            List<MontagemCarregamentoBlocoSimuladorFretePedido> simuladoresFretePedidoDoSimulador = (from obj in simuladorFretesPedidos where obj.SimuladorFrete.Codigo == codigoSimuladorFrete select obj).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = simuladoresFretePedidoDoSimulador.Select(s => s.Pedido).Distinct().ToList();
            List<Carregamento> carregamentos = carregamentosPedidos.Where(o => pedidos.Select(x => x.Codigo).Contains(o.Pedido.Codigo)).Select(x => x.Carregamento).Distinct().ToList();

            return cargas.Any(c => carregamentos.Select(cc => cc.Codigo).Contains(c.Carregamento.Codigo));
        }

        private string IsExigeIsca(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete montagemCarregamentoBlocoSimuladorFrete, List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteCarregamento> carregamentos)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SimuladorFreteCarregamento tmp = (from car in carregamentos where car.CodigoSimuladorFrete == montagemCarregamentoBlocoSimuladorFrete.Codigo select car).FirstOrDefault();
            if (tmp != null)
                return tmp.ExigeIsca ? "SIM" : "NÃO";

            return montagemCarregamentoBlocoSimuladorFrete.ExigeIsca ? "SIM" : "NÃO";
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSessaoRoteirizador()
            {
                Situacao = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Todas),
                CodigoFilial = Request.GetIntParam("CodigoFilial"),
                CodigoUsuario = Request.GetIntParam("CodigoUsuario"),
                NumeroSessao = Request.GetIntParam("NumeroSessao")
            };
            return filtrosPesquisa;
        }

        private string ObterJsonParametrosCentroCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador sessao, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = centrosCarregamento?.FirstOrDefault();

            if (centroCarregamento == null)
                throw new Exception(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.NaoExisteCentroDeCarregamentoCadastradoParaFilial, sessao.Filial.Descricao));

            List<int> codigosCentroCarregamento = centrosCarregamento.Select(o => o.Codigo).ToList();
            Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota repositorioCentroCarregamentoDisponibilidadeFrota = new Repositorio.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota(unitOfWork);
            DateTime dataEntrega = (sessao.DataInicial.HasValue ? sessao.DataInicial.Value : sessao.Inicio.Date);
            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataEntrega);

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> disponibilidadeFrotas = repositorioCentroCarregamentoDisponibilidadeFrota.BuscarPorCentrosDeCarregamentoEDia(codigosCentroCarregamento, diaSemana);
            List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento> temposCarregamentoCentro = centroCarregamento?.TemposCarregamento.ToList() ?? new List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>();

            // Aqui, vamos levantar todos os parametros do centro...
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros sessaoRoteirizadorParametros = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros()
            {
                AgruparPedidosMesmoDestinatario = centroCarregamento?.AgruparPedidosMesmoDestinatario ?? false,
                CarregamentoTempoMaximoRota = centroCarregamento?.CarregamentoTempoMaximoRota ?? 0,
                ConsiderarTempoDeslocamentoCD = centroCarregamento?.ConsiderarTempoDeslocamentoPrimeiraEntrega ?? false,
                GerarCarregamentoDoisDias = centroCarregamento?.GerarCarregamentoDoisDias ?? false,
                GerarCarregamentosAlemDaDispFrota = centroCarregamento?.GerarCarregamentosAlemDaDispFrota ?? false,
                MontagemCarregamentoPedidoProduto = centroCarregamento?.MontagemCarregamentoPedidoProduto ?? false,
                NivelQuebraProdutoRoteirizar = centroCarregamento?.NivelQuebraProdutoRoteirizar ?? NivelQuebraProdutoRoteirizar.Caixa,
                QuantidadeMaximaEntregasRoteirizar = centroCarregamento?.QuantidadeMaximaEntregasRoteirizar ?? 999,
                TipoMontagemCarregamentoVRP = centroCarregamento?.TipoMontagemCarregamentoVRP ?? TipoMontagemCarregamentoVRP.Nenhum,
                TipoOcupacaoMontagemCarregamentoVRP = centroCarregamento?.TipoOcupacaoMontagemCarregamentoVRP ?? TipoOcupacaoMontagemCarregamentoVRP.Peso,
                UtilizarDispFrotaCentroDescCliente = centroCarregamento?.UtilizarDispFrotaCentroDescCliente ?? false,
                DisponibilidadesFrota = (from disp in disponibilidadeFrotas
                                         select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosDisponibilidadeFrota()
                                         {
                                             Codigo = disp.Codigo,
                                             CodigoModeloVeicular = disp.ModeloVeicular?.Codigo ?? 0,
                                             DescricaoModeloVeicular = disp.ModeloVeicular?.Descricao ?? string.Empty,
                                             CodigoTransportador = disp.Transportador?.Codigo ?? 0,
                                             DescricaoTransportador = disp.Transportador?.Descricao ?? string.Empty,
                                             Quantidade = disp.Quantidade,
                                             QuantidadeUtilizar = disp.Quantidade
                                         }).ToList(),
                TemposCarregamento = (from tempo in temposCarregamentoCentro
                                      select new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametrosTempoCarregamento()
                                      {
                                          Codigo = tempo.Codigo,
                                          CodigoModeloVeicular = tempo.ModeloVeicular?.Codigo ?? 0,
                                          DescricaoModeloVeicular = tempo.ModeloVeicular?.Descricao ?? string.Empty,
                                          CodigoTipoCarga = tempo.TipoCarga?.Codigo ?? 0,
                                          DescricaoTipoCarga = tempo.TipoCarga?.Descricao ?? string.Empty,
                                          Quantidade = tempo.QuantidadeMaximaEntregasRoteirizar,
                                          QuantidadeMinima = tempo.QuantidadeMinimaEntregasRoteirizar,
                                          QuantidadeMinimaUtilizar = tempo.QuantidadeMinimaEntregasRoteirizar,
                                          QuantidadeUtilizar = tempo.QuantidadeMaximaEntregasRoteirizar
                                      }).ToList()
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(sessaoRoteirizadorParametros);
        }

        private string SimuladorFreteLimite(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete obj,
                                      List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simuladorFretesPedidos,
                                      List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tipoOperacoes,
                                      ref string rowColorString,
                                      bool rowColor = true)
        {
            decimal valorTotalPedidos = (from ped in simuladorFretesPedidos
                                         where ped.SimuladorFrete.Codigo == obj.Codigo
                                         select ped.Pedido.GrossSales).Sum();

            decimal valorTotalMercadoriasPedidos = (from ped in simuladorFretesPedidos
                                                    where ped.SimuladorFrete.Codigo == obj.Codigo
                                                    select ped.Pedido.ValorTotalNotasFiscais).Sum();

            decimal valorMinimoDestinatario = (from ped in simuladorFretesPedidos
                                               where ped.SimuladorFrete.Codigo == obj.Codigo
                                               select ped.Pedido.Destinatario.ValorMinimoCarga).FirstOrDefault() ?? 0;

            if (valorTotalPedidos < valorMinimoDestinatario)
            {
                rowColorString = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;
                return (rowColor ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning : Localization.Resources.Cargas.MontagemCargaMapa.NaoAtendido);
            }

            decimal valorLimiteCarga = (from toc in tipoOperacoes
                                        where toc.Codigo == obj.TipoOperacao.Codigo && toc.ConfiguracaoCarga.ValorLimiteNaCarga > 0
                                        select toc.ConfiguracaoCarga.ValorLimiteNaCarga)?.FirstOrDefault() ?? 99999999;

            if (valorLimiteCarga == 0)
                valorLimiteCarga = 99999999;

            if (valorTotalMercadoriasPedidos > valorLimiteCarga)
            {
                rowColorString = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
                return (rowColor ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger : Localization.Resources.Cargas.MontagemCargaMapa.Excedido);
            }

            return (rowColor ? string.Empty : Localization.Resources.Cargas.MontagemCargaMapa.Atendido);
        }

        private void RetornarQuantidadeProdutoEmSessaoParaEstoqueArmazem(List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> sessaoRoteirizadorPedidoProdutos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem repositorioProdutoEmbarcadorEstoqueArmazem = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem(unitOfWork);
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem> listaProdutosEmbarcadorEstoqueArmazem = repositorioProdutoEmbarcadorEstoqueArmazem.BuscarPorCodigosProduto(sessaoRoteirizadorPedidoProdutos.Select(sr => sr.PedidoProduto.Produto.Codigo).Distinct().ToList());

            if (listaProdutosEmbarcadorEstoqueArmazem.Count == 0) return;

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto sessaoRoteirizadorPedidoProduto in sessaoRoteirizadorPedidoProdutos)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem produtoEmbarcadorEstoqueArmazem = listaProdutosEmbarcadorEstoqueArmazem.Find(pe => pe.Filial.Codigo == sessaoRoteirizadorPedidoProduto.PedidoProduto.FilialArmazem.Filial.Codigo
                                                                                                                                                                         && pe.Produto.Codigo == sessaoRoteirizadorPedidoProduto.PedidoProduto.Produto.Codigo
                                                                                                                                                                         && pe.Armazem.Codigo == sessaoRoteirizadorPedidoProduto.PedidoProduto.FilialArmazem.Codigo);
                if (produtoEmbarcadorEstoqueArmazem != null)
                {
                    produtoEmbarcadorEstoqueArmazem.EstoqueSessaoRoterizacao -= sessaoRoteirizadorPedidoProduto.QuantidadeProdutoSessao;
                    produtoEmbarcadorEstoqueArmazem.EstoqueDisponivel += sessaoRoteirizadorPedidoProduto.QuantidadeProdutoSessao;

                    repositorioProdutoEmbarcadorEstoqueArmazem.Atualizar(produtoEmbarcadorEstoqueArmazem);
                }
            }
        }

        #endregion
    }
}
