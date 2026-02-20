using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Repositorio.Embarcador.Patrimonio;
using QRCoder.Extensions;

namespace SGT.WebAdmin.Controllers.PedidosVendas
{
    [CustomAuthorize("PedidosVendas/PedidoVenda", "Atendimentos/Chamado", "Agendas/AgendaTarefa",
        "Atendimentos/Atendimento", "Agendas/Agenda")]
    public class PedidoVendaController : BaseController
    {
        #region Construtores

        public PedidoVendaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pessoa", "Cliente", 30, Models.Grid.Align.left, true);

                if (filtrosPesquisa.TipoPedidoVenda == 0 || filtrosPesquisa.TipoPedidoVenda == TipoPedidoVenda.Todos)
                    grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 15, Models.Grid.Align.left, true);
                if (!filtrosPesquisa.StatusPedidoVenda.HasValue ||
                    filtrosPesquisa.StatusPedidoVenda == StatusPedidoVenda.AbertaFinalizada)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 15, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Valor", "ValorTotal", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("CondicaoPagamentoCodigo", false);
                grid.AdicionarCabecalho("CondicaoPagamentoDescricao", false);

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> listaPedidoVenda = repPedidoVenda.Consulta(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repPedidoVenda.ContaConsulta(filtrosPesquisa));

                var lista = (from p in listaPedidoVenda
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 DataEmissao = p.DataEmissao.Value.ToDateTimeString(),
                                 Cliente = p.Cliente?.Nome ?? string.Empty,
                                 p.DescricaoTipo,
                                 p.DescricaoStatus,
                                 ValorTotal = p.ValorTotal.ToString("n2"),
                                 CondicaoPagamentoCodigo = p.CondicaoPagamentoPadrao?.Codigo ?? 0,
                                 CondicaoPagamentoDescricao = p.CondicaoPagamentoPadrao?.Descricao ?? string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                int funcionario = Request.GetIntParam("Funcionario");
                double.TryParse(Request.Params("Pessoa"), out double pessoa);
                decimal.TryParse(Request.Params("ValorProdutos"), out decimal valorProdutos);
                decimal.TryParse(Request.Params("ValorServicos"), out decimal valorServicos);
                decimal.TryParse(Request.Params("ValorTotal"), out decimal valorTotal);
                int.TryParse(Request.Params("CondicaoPagamentoPadrao"), out int codigoCondicaoPagamentoPadrao);

                DateTime dataEntrega = Request.GetDateTimeParam("DataEntrega");

                StatusPedidoVenda statusPedidoVenda = StatusPedidoVenda.Aberta;
                TipoPedidoVenda tipoPedidoVenda = Request.GetEnumParam<TipoPedidoVenda>("Tipo");
                statusPedidoVenda = Request.GetEnumParam<StatusPedidoVenda>("Status");

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);

                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda;
                if (codigo > 0)
                    pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigo, true);
                else
                {
                    pedidoVenda = new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda();
                    pedidoVenda.Numero = repPedidoVenda.BuscarUltimoNumero(this.Usuario.Empresa.Codigo) + 1;
                    pedidoVenda.Empresa = this.Usuario.Empresa;
                }

                if (dataEntrega.Date != DateTime.Now.Date && statusPedidoVenda == StatusPedidoVenda.Finalizada && pedidoVenda.Empresa.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual)
                    throw new ControllerException("Só é permitido finalizar o Pedido se a data de entrega for a data atual");

                pedidoVenda.Status = statusPedidoVenda;

                pedidoVenda.DataEmissao = Request.GetDateTimeParam("DataEmissao");
                pedidoVenda.DataEntrega = dataEntrega;
                pedidoVenda.Tipo = tipoPedidoVenda;

                pedidoVenda.Observacao = Request.GetStringParam("Observacao");
                pedidoVenda.Referencia = Request.GetStringParam("Referencia");
                pedidoVenda.ValorProdutos = valorProdutos;
                pedidoVenda.ValorServicos = valorServicos;
                pedidoVenda.ValorTotal = valorTotal;

                pedidoVenda.Cliente = repCliente.BuscarPorCPFCNPJ(pessoa);
                pedidoVenda.Funcionario = repUsuario.BuscarPorCodigo(funcionario);
                pedidoVenda.FormaPagamento = Request.GetStringParam("FormaPagamento");
                pedidoVenda.CondicaoPagamentoPadrao = codigoCondicaoPagamentoPadrao > 0 ? repCondicaoPagamento.BuscarPorCodigo(codigoCondicaoPagamentoPadrao) : null;

                if (codigo > 0)
                    repPedidoVenda.Atualizar(pedidoVenda, Auditado);
                else
                    repPedidoVenda.Inserir(pedidoVenda, Auditado);

                SalvarListaItens(pedidoVenda, unitOfWork);
                SalvarListaParcelas(pedidoVenda, unitOfWork);

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    pedidoVenda.Codigo,
                    pedidoVenda.Status
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Salvar.");
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
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda =
                    new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda =
                    repPedidoVenda.BuscarPorCodigo(codigo);

                if (pedidoVenda == null)
                    return new JsonpResult(false, true, "Pedido não encontrado.");

                object retorno = new
                {
                    pedidoVenda.Codigo,
                    Numero = pedidoVenda.Numero.ToString("n0"),
                    pedidoVenda.Tipo,
                    pedidoVenda.Status,
                    Pessoa = pedidoVenda.Cliente != null
                        ? new { Codigo = pedidoVenda.Cliente.CPF_CNPJ, Descricao = pedidoVenda.Cliente.Nome }
                        : null,
                    Funcionario = pedidoVenda.Funcionario != null
                        ? new { Codigo = pedidoVenda.Funcionario.Codigo, Descricao = pedidoVenda.Funcionario.Nome }
                        : null,
                    DataEmissao = pedidoVenda.DataEmissao.Value.ToDateTimeString(),
                    DataEntrega = pedidoVenda.DataEntrega.Value.ToDateTimeString(),
                    pedidoVenda.Observacao,
                    pedidoVenda.Referencia,
                    pedidoVenda.ValorProdutos,
                    pedidoVenda.ValorServicos,
                    pedidoVenda.ValorTotal,
                    pedidoVenda.FormaPagamento,
                    CondicaoPagamentoCodigo = pedidoVenda.CondicaoPagamentoPadrao?.Codigo ?? 0,
                    CondicaoPagamentoDescricao = pedidoVenda.CondicaoPagamentoPadrao?.Descricao ?? string.Empty,
                    ListaItens = pedidoVenda.Itens != null
                        ? (from obj in pedidoVenda.Itens
                           select new
                           {
                               Codigo = obj.Codigo,
                               Produto = obj.Produto != null ? obj.Produto?.Codigo : 0,
                               Servico = obj.Servico != null ? obj.Servico?.Codigo : 0,
                               CodigoPedidoVenda = obj.PedidoVenda.Codigo,
                               obj.CodigoItem,
                               obj.DescricaoItem,
                               obj.NumeroItemOrdemCompra,
                               obj.NumeroOrdemCompra,
                               Quantidade =
                                   obj.Quantidade.ToString("n" +
                                                           pedidoVenda.Empresa.CasasQuantidadeProdutoNFe.ToString()),
                               ValorUnitario =
                                   obj.ValorUnitario.ToString(
                                       "n" + pedidoVenda.Empresa.CasasValorProdutoNFe.ToString()),
                               ValorTotalItem = obj.ValorTotal.ToString("n2"),
                               CodigoNCM = obj.Produto != null ? obj.Produto?.CodigoNCM : string.Empty,
                               Observacao = obj.Observacao ?? string.Empty
                           }).ToList()
                        : null,
                    ListaParcelas = pedidoVenda.Parcelas != null
                        ? (from obj in pedidoVenda.Parcelas
                           select new
                           {
                               obj.Codigo,
                               obj.Sequencia,
                               Parcela = obj.Sequencia,
                               Valor = obj.Valor.ToString("n2"),
                               Desconto = obj.Desconto.ToString("n2"),
                               DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                               FormaTitulo = obj.Forma
                           }).ToList()
                        : null,
                    QuantidadeParcelas = pedidoVenda.Parcelas != null ? pedidoVenda.Parcelas.Count : 0
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BaixarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                string nomeEmpresa = Cliente.NomeFantasia;
                string stringConexao = _conexao.StringConexao;

                if (this.Usuario.Empresa.TipoImpressaoPedidoVenda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedidoVenda.Contrato)
                {
                    Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R113_PedidoVendaContrato, TipoServicoMultisoftware);
                    if (relatorio == null)
                        relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R113_PedidoVendaContrato, TipoServicoMultisoftware, "Relatorio de Pedido - Contrato", "PedidoVenda", "PedidoVendaContrato.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                    Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                    IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato> dadosPedidoVenda = repPedidoVenda.RelatorioPedidoVendaContrato(codigo);
                    if (dadosPedidoVenda.Count > 0)
                    {
                        Task.Factory.StartNew(() => GerarRelatorioPedidoVendaContrato(codigo, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosPedidoVenda));
                        return new JsonpResult(true);
                    }
                    else
                    {
                        return new JsonpResult(false, false, "Nenhum registro de pedido de vendas para regar o relatório.");
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R039_PedidoVenda, TipoServicoMultisoftware);
                    if (relatorio == null)
                        relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R039_PedidoVenda, TipoServicoMultisoftware, "Relatorio de Pedido", "PedidoVenda", "PedidoVenda.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                    Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                    IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVenda> dadosPedidoVenda = repPedidoVenda.RelatorioPedidoVenda(codigo);
                    IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaParcela> dadosPedidoVendaParcela = repPedidoVenda.RelatorioPedidoVendaParcela(codigo);
                    if (dadosPedidoVenda.Count > 0)
                    {
                        Task.Factory.StartNew(() => GerarRelatorioPedidoVenda(codigo, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosPedidoVenda, dadosPedidoVendaParcela));
                        return new JsonpResult(true);
                    }
                    else
                    {
                        return new JsonpResult(false, false, "Nenhum registro de pedido de vendas para regar o relatório.");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                string nomeEmpresa = Cliente.NomeFantasia;
                string stringConexao = _conexao.StringConexao;

                if (this.Usuario.Empresa.TipoImpressaoPedidoVenda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedidoVenda.Contrato)
                {
                    Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R113_PedidoVendaContrato, TipoServicoMultisoftware);
                    if (relatorio == null)
                        relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R113_PedidoVendaContrato, TipoServicoMultisoftware, "Relatorio de Pedido - Contrato", "PedidoVendaContrato", "PedidoVendaContrato.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                    Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                    IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato> dadosPedidoVenda = repPedidoVenda.RelatorioPedidoVendaContrato(codigo);
                    if (dadosPedidoVenda.Count > 0)
                    {
                        Task.Factory.StartNew(() => GerarRelatorioPedidoVendaContrato(codigo, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosPedidoVenda, true));
                        return new JsonpResult(true);
                    }
                    else
                    {
                        return new JsonpResult(false, false, "Nenhum registro de pedido de vendas para regar o relatório.");
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R039_PedidoVenda, TipoServicoMultisoftware);
                    if (relatorio == null)
                        relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R039_PedidoVenda, TipoServicoMultisoftware, "Relatorio de Pedido", "PedidoVenda", "PedidoVenda.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                    Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                    IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVenda> dadosPedidoVenda = repPedidoVenda.RelatorioPedidoVenda(codigo);
                    IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaParcela> dadosPedidoVendaParcela = repPedidoVenda.RelatorioPedidoVendaParcela(codigo);
                    if (dadosPedidoVenda.Count > 0)
                    {
                        Task.Factory.StartNew(() => GerarRelatorioPedidoVenda(codigo, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosPedidoVenda, dadosPedidoVendaParcela, true));
                        return new JsonpResult(true);
                    }
                    else
                    {
                        return new JsonpResult(false, false, "Nenhum registro de pedido de vendas para regar o relatório.");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar por e-mail. " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarFuncionarioLogado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                object retorno = null;

                retorno = new
                {
                    Usuario.Codigo,
                    Usuario.Nome
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o Funcionário Logado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificaClienteTemEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte =
                    new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email =
                    repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(this.Usuario.Empresa.Codigo);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");
                else
                {
                    double pessoa;
                    double.TryParse(Request.Params("Pessoa"), out pessoa);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(pessoa);

                    if (cliente != null)
                    {
                        List<string> emails = new List<string>();
                        if (!string.IsNullOrWhiteSpace(cliente.Email))
                            emails.Add(cliente.Email);

                        for (int a = 0; a < cliente.Emails.Count; a++)
                        {
                            if (!string.IsNullOrWhiteSpace(cliente.Emails[a].Email) &&
                                cliente.Emails[a].EmailStatus == "A")
                                emails.Add(cliente.Emails[a].Email);
                        }

                        if (emails.Count > 0)
                            return new JsonpResult(true);
                        else
                            return new JsonpResult(false, "Não há e-mail cadastrado no cliente.");
                    }
                    else
                        return new JsonpResult(false, "Cliente não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar se o Cliente possui e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPedidoVenda();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin =
                new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();
                unitOfWorkAdmin.Start();

                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPedidoVenda();
                List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda> itensPedidoVenda =
                    new List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request,
                    itensPedidoVenda, ((dados) =>
                    {
                        Servicos.Embarcador.PedidosVendas.ImportacaoPedidoVenda servicoPedidoVendaImportar =
                            new Servicos.Embarcador.PedidosVendas.ImportacaoPedidoVenda(dados, unitOfWork,
                                TipoServicoMultisoftware, ConfiguracaoEmbarcador, Empresa, unitOfWorkAdmin);
                        return servicoPedidoVendaImportar.ObterPedidoVendaImportar();
                    }));

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda =
                    new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repPedidoItens =
                    new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda importacaoPedidoVenda =
                    itensPedidoVenda.LastOrDefault();
                if (importacaoPedidoVenda == null)
                    return new JsonpResult(false, "Sem dados para importar!");

                Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda =
                    new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda();
                pedidoVenda.Numero = repPedidoVenda.BuscarUltimoNumero(this.Usuario.Empresa.Codigo) + 1;
                pedidoVenda.Empresa = Usuario.Empresa;
                pedidoVenda.Funcionario = Usuario;
                pedidoVenda.Tipo = TipoPedidoVenda.Pedido;
                pedidoVenda.Status = StatusPedidoVenda.Aberta;
                pedidoVenda.ValorServicos = 0;

                pedidoVenda.ValorProdutos = RetornarValorTotalProdutos(itensPedidoVenda);
                pedidoVenda.ValorTotal = pedidoVenda.ValorProdutos;

                pedidoVenda.Cliente = importacaoPedidoVenda.Cliente;
                pedidoVenda.DataEmissao = importacaoPedidoVenda.DataEmissao;
                pedidoVenda.DataEntrega = importacaoPedidoVenda.DataPrevisao;
                if (!string.IsNullOrWhiteSpace(importacaoPedidoVenda.NumeroOrcamento))
                    pedidoVenda.Observacao = importacaoPedidoVenda.Observacao + " - Importado de AlmaQuote, número: " +
                                             importacaoPedidoVenda.NumeroOrcamento;
                else
                    pedidoVenda.Observacao = importacaoPedidoVenda.Observacao;

                pedidoVenda.Referencia = importacaoPedidoVenda.Referencia;
                pedidoVenda.FormaPagamento = importacaoPedidoVenda.FormaPagamento;

                if (!permiteInserir)
                    return new JsonpResult(false, "Não permitido inserir!");

                repPedidoVenda.Inserir(pedidoVenda, Auditado);

                foreach (Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda itens in
                         itensPedidoVenda)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens pedidoVendaItens =
                        new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens();
                    pedidoVendaItens.PedidoVenda = pedidoVenda;
                    pedidoVendaItens.CodigoItem = itens.Produto.CodigoProduto;
                    pedidoVendaItens.Produto = itens.Produto;
                    pedidoVendaItens.NumeroItemOrdemCompra = itens.NumeroItemOrdemCompra;
                    pedidoVendaItens.NumeroOrdemCompra = itens.NumeroOrdemCompra;
                    if (itens.DescricaoItem != "")
                        pedidoVendaItens.DescricaoItem = itens.DescricaoItem;
                    else
                        pedidoVendaItens.DescricaoItem = itens.Produto.Descricao;

                    pedidoVendaItens.Quantidade = itens.Quantidade;
                    pedidoVendaItens.ValorUnitario = itens.ValorUnitario;

                    if (itens.ValorTotal == 0)
                        pedidoVendaItens.ValorTotal = itens.Quantidade * itens.ValorUnitario;
                    else
                        pedidoVendaItens.ValorTotal = itens.ValorTotal;

                    repPedidoItens.Inserir(pedidoVendaItens);
                    totalRegistrosImportados++;
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarListaItens(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda,
            Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repItensPedidoVenda =
                new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unidadeDeTrabalho);

            repItensPedidoVenda.DeletarPorPedido(pedidoVenda.Codigo);

            dynamic listaItens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItens"));
            if (listaItens != null)
            {
                foreach (var itemPedido in listaItens)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens itensPedidoVenda =
                        new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens();
                    itensPedidoVenda.CodigoItem = (string)itemPedido.CodigoItem;
                    itensPedidoVenda.DescricaoItem = (string)itemPedido.DescricaoItem;
                    itensPedidoVenda.NumeroOrdemCompra = (string)itemPedido.NumeroOrdemCompra;
                    itensPedidoVenda.NumeroItemOrdemCompra = (string)itemPedido.NumeroItemOrdemCompra;
                    itensPedidoVenda.Quantidade = Utilidades.Decimal.Converter((string)itemPedido.Quantidade);
                    itensPedidoVenda.ValorUnitario = Utilidades.Decimal.Converter((string)itemPedido.ValorUnitario);
                    itensPedidoVenda.ValorTotal = Utilidades.Decimal.Converter((string)itemPedido.ValorTotalItem);
                    itensPedidoVenda.Observacao = (string)itemPedido.Observacao;

                    if (itemPedido.Produto != null)
                        if ((int)itemPedido.Produto > 0)
                            itensPedidoVenda.Produto = new Dominio.Entidades.Produto()
                            { Codigo = (int)itemPedido.Produto };
                    if (itemPedido.Servico != null)
                        if ((int)itemPedido.Servico > 0)
                            itensPedidoVenda.Servico = new Dominio.Entidades.Embarcador.NotaFiscal.Servico()
                            { Codigo = (int)itemPedido.Servico };
                    itensPedidoVenda.PedidoVenda = pedidoVenda;

                    repItensPedidoVenda.Inserir(itensPedidoVenda);
                }
            }
        }

        private void SalvarListaParcelas(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda,
            Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaParcela repParcelasPedidoVenda =
                new Repositorio.Embarcador.PedidoVenda.PedidoVendaParcela(unidadeDeTrabalho);

            repParcelasPedidoVenda.DeletarPorPedido(pedidoVenda.Codigo);

            dynamic parcelas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaParcelas") ?? string.Empty);
            if (parcelas != null)
            {
                foreach (var parcelaPedido in parcelas)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela parcelaPedidoVenda =
                        new Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela();

                    parcelaPedidoVenda.Sequencia = (int)parcelaPedido.Sequencia;
                    parcelaPedidoVenda.Valor = Utilidades.Decimal.Converter((string)parcelaPedido.Valor);
                    parcelaPedidoVenda.Desconto = Utilidades.Decimal.Converter((string)parcelaPedido.Desconto);

                    DateTime dataVencimento = new DateTime();
                    DateTime.TryParseExact((string)parcelaPedido.DataVencimento, "dd/MM/yyyy", null,
                        System.Globalization.DateTimeStyles.None, out dataVencimento);
                    parcelaPedidoVenda.DataVencimento = dataVencimento;

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                    Enum.TryParse((string)parcelaPedido.FormaTitulo, out formaTitulo);
                    parcelaPedidoVenda.Forma = formaTitulo;

                    parcelaPedidoVenda.PedidoVenda = pedidoVenda;

                    repParcelasPedidoVenda.Inserir(parcelaPedidoVenda);
                }
            }
        }

        private void GerarRelatorioPedidoVenda(int codigoPedidoVenda, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVenda> dadosPedidoVenda, IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaParcela> dadosPedidoVendaParcela, bool enviarEmail = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Servicos.Embarcador.PedidosVendas.PedidosVendas serPedidoVenda = new Servicos.Embarcador.PedidosVendas.PedidosVendas(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ProdutoFoto repProdutoFoto = new Repositorio.ProdutoFoto(unitOfWork);

            try
            {
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);
                string mensagem = "";

                foreach (var itemPedido in dadosPedidoVenda)
                {
                    itemPedido.PossuiFoto = false;
                    if (itemPedido.CodigoProduto > 0)
                    {
                        Dominio.Entidades.ProdutoFoto produtoFoto = repProdutoFoto.BuscarPorProduto(itemPedido.CodigoProduto);
                        if (produtoFoto != null)
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(produtoFoto.CaminhoArquivo))
                            {
                                itemPedido.PossuiFoto = true;
                                itemPedido.Foto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(produtoFoto.CaminhoArquivo);
                            }
                        }
                    }
                }

                var report = ReportRequest.WithType(ReportType.PedidoVenda)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoPedidoVenda", codigoPedidoVenda)
                    .AddExtraData("nomeEmpresa", nomeEmpresa)
                    .AddExtraData("dadosPedidoVenda", dadosPedidoVenda.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CaminhoLogoDacte", empresaRelatorio.CaminhoLogoDacte)
                    .AddExtraData("dadosPedidoVendaParcela", dadosPedidoVendaParcela.ToJson())
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(report.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, report.ErrorMessage);
                else if (enviarEmail)
                    EnviarEmailPedido(unitOfWork, codigoPedidoVenda, report.FullPath);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void EnviarEmailPedido(Repositorio.UnitOfWork unitOfWork, int codigoPedido, string caminhoRelatorio)
        {
            System.Threading.Thread.Sleep(4000);

            Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;
            Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(empresa.Codigo);

            if (email == null)
                throw new Exception("Não há um e-mail configurado para realizar o envio.");

            Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedido = repPedidoVenda.BuscarPorCodigo(codigoPedido);
            if (pedido != null)
            {
                string assunto = "";
                string mensagemEmail = "";
                if (empresa.TipoImpressaoPedidoVenda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedidoVenda.Contrato)
                {
                    assunto = "Pedido/Contrato da " + pedido.Empresa.NomeFantasia;
                    mensagemEmail = "Olá,<br/><br/>Segue em anexo o Pedido/Contrato da Empresa: " + pedido.Empresa.NomeFantasia + ".<br/><br/>";
                }
                else
                {
                    assunto = "Pedido de Venda da " + pedido.Empresa.NomeFantasia;
                    mensagemEmail = "Olá,<br/><br/>Segue em anexo o Pedido de Venda da Empresa: " + pedido.Empresa.NomeFantasia + ".<br/><br/>";
                }

                mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
                string mensagemErro = "Erro ao enviar e-mail";

                if (!string.IsNullOrWhiteSpace(caminhoRelatorio) && Utilidades.IO.FileStorageService.Storage.Exists(caminhoRelatorio))
                {
                    List<string> emails = new List<string>();
                    if (!string.IsNullOrWhiteSpace(pedido.Cliente.Email))
                        emails.AddRange(pedido.Cliente.Email.Split(';').ToList());

                    for (int a = 0; a < pedido.Cliente.Emails.Count; a++)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = pedido.Cliente.Emails[a];
                        if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                            emails.Add(outroEmail.Email);
                    }

                    if (!string.IsNullOrWhiteSpace(empresa.Email) && empresa.StatusEmail == "A")
                        emails.AddRange(empresa.Email.Split(';').ToList());

                    if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) && empresa.StatusEmailAdministrativo == "A")
                        emails.AddRange(empresa.EmailAdministrativo.Split(';').ToList());

                    emails = emails.Distinct().ToList();
                    if (emails.Count > 0)
                    {
                        byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoRelatorio);
                        bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdf), System.IO.Path.GetFileName(caminhoRelatorio), "application/pdf") }, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, empresa.Codigo);
                        if (!sucesso)
                            throw new Exception("Problemas ao enviar o pedido por e-mail: " + mensagemErro);
                    }
                    else
                        throw new Exception("Cliente do pedido não possui e-mail cadastrado.");
                }
                else
                    throw new Exception("Não foi possível localizar o arquivo PDF do pedido.");
            }
            else
                throw new Exception("Pedido não localizado para enviar e-mail.");
        }

        private void GerarRelatorioPedidoVendaContrato(int codigoPedidoVenda, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato> dadosPedidoVenda, bool enviarEmail = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Servicos.Embarcador.PedidosVendas.PedidoVendaContrato serPedidoVendaContrato = new Servicos.Embarcador.PedidosVendas.PedidoVendaContrato(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            try
            {
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                var report = ReportRequest.WithType(ReportType.PedidoVendaContrato)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoPedidoVenda", codigoPedidoVenda)
                    .AddExtraData("nomeEmpresa", nomeEmpresa)
                    .AddExtraData("dadosPedidoVenda", dadosPedidoVenda.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CaminhoLogoDacte", empresaRelatorio.CaminhoLogoDacte)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(report.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, report.ErrorMessage);
                else if (enviarEmail)
                    EnviarEmailPedido(unitOfWork, codigoPedidoVenda, report.FullPath);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa =
                new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda()
                {
                    CodigoFuncionario = Request.GetIntParam("Funcionario"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    NumeroInicial = Request.GetIntParam("NumeroInicial"),
                    NumeroFinal = Request.GetIntParam("NumeroFinal"),
                    CnpjCpfCliente = Request.GetDoubleParam("Pessoa"),
                    DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                    DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                    DataEntregaInicial = Request.GetDateTimeParam("DataEntregaInicial"),
                    DataEntregaFinal = Request.GetDateTimeParam("DataEntregaFinal"),
                    StatusPedidoVenda = Request.GetNullableEnumParam<StatusPedidoVenda>("Status"),
                    TipoPedidoVenda = Request.GetEnumParam<TipoPedidoVenda>("Tipo"),
                    CodigoEmpresa = Empresa.Codigo
                };

            if (filtrosPesquisa.StatusPedidoVenda == StatusPedidoVenda.AbertaFinalizada &&
                Usuario.Empresa.PermitirImportarApenasPedidoVendaFinalizado)
                filtrosPesquisa.StatusPedidoVenda = StatusPedidoVenda.Finalizada;

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("DescricaoTipo"))
                return "Tipo";
            else if (propriedadeOrdenar.Equals("DescricaoStatus"))
                return "Status";

            return propriedadeOrdenar;
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoPedidoVenda()
        {
            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 1,
                Descricao = "Data de Emissão",
                Propriedade = "DataEmissao",
                Tamanho = tamanho,
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 2,
                Descricao = "Data Previsão",
                Propriedade = "DataPrevisao",
                Tamanho = tamanho,
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 3,
                Descricao = "CNPJ/CPF",
                Tamanho = tamanho,
                Propriedade = "CNPJ",
                Obrigatorio = true,
                Regras = new List<string> { "required" }
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 4,
                Descricao = "Observação",
                Tamanho = tamanho,
                Propriedade = "Observacao",
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 5,
                Descricao = "Referência",
                Tamanho = tamanho,
                Propriedade = "Referencia",
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 6,
                Descricao = "Forma de Pagamento",
                Tamanho = tamanho,
                Propriedade = "FormaPagamento",
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 7,
                Descricao = "Código Produto",
                Propriedade = "CodigoProduto",
                Tamanho = tamanho,
                Obrigatorio = true,
                Regras = new List<string> { "required" }
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 8,
                Descricao = "Descrição do Item",
                Tamanho = tamanho,
                Propriedade = "DescricaoItem",
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 9,
                Descricao = "Quantidade",
                Tamanho = tamanho,
                Obrigatorio = true,
                Propriedade = "Quantidade",
                Regras = new List<string> { "required" }
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 10,
                Descricao = "Valor Unitário",
                Tamanho = tamanho,
                Obrigatorio = true,
                Propriedade = "ValorUnitario",
                Regras = new List<string> { "required" }
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 11,
                Descricao = "Valor Total",
                Tamanho = tamanho,
                CampoInformacao = true,
                Propriedade = "ValorTotal"
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 12,
                Descricao = "Número Orçamento",
                Tamanho = tamanho,
                Propriedade = "NumeroOrcamento",
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 13,
                Descricao = "Número Ordem Compra",
                Tamanho = tamanho,
                Propriedade = "NumeroOrdemCompra",
                CampoInformacao = true
            });
            configuracoes.Add(new ConfiguracaoImportacao()
            {
                Id = 14,
                Descricao = "Número Item Ordem Compra",
                Tamanho = tamanho,
                Propriedade = "NumeroItemOrdemCompra",
                CampoInformacao = true
            });


            return configuracoes;
        }

        private decimal RetornarValorTotalProdutos(List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda> itens)
        {
            decimal valorTotal = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda item in itens)
                valorTotal = valorTotal + (item.Quantidade * item.ValorUnitario);

            return valorTotal;
        }

        #endregion
    }
}
