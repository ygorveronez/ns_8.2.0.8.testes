using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.PedidosVendas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/PedidosVendas/PedidoVenda")]
    public class PedidoVendaController : BaseController
    {
		#region Construtores

		public PedidoVendaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R067_PedidoOrdemVenda;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pedidos e Ordens de Serviço", "PedidoVenda", "PedidoOrdemVenda.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "Numero", "", codigoRelatorio, unitOfWork, true, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repositorio = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorio(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVenda> listaVeiculo = totalRegistros > 0 ? repositorio.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVenda>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaVeiculo);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repositorio = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVenda> dataSource = repositorio.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta);
                IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVendaItens> listaItens = new List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVendaItens>();

                if (filtrosPesquisa.ExibirItens)
                    listaItens = repositorio.ConsultarItensRelatorio(filtrosPesquisa);

                List<Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa, parametrosConsulta);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/PedidosVendas/PedidoVenda",parametros,relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork, null, new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("Itens", listaItens) }, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Número", "Numero", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Entrega", "DataEntregaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Vendedor", "Vendedor", _tamanhoColunaGrande, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Vlr. Total", "ValorTotal", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vlr. Tot. Produtos", "ValorProdutos", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vlr. Tot. Serviços", "ValorServicos", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Descrição Item", "DescricaoItem", _tamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Código Item", "CodigoItem", _tamanhoColunaPequena, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Qtd.", "QuantidadeItem", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Unitário", "ValorUnitarioItem", _tamanhoColunaPequena, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Valor Total Item", "ValorTotalItem", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Fornecedor Serviço", "FornecedorServico", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Funcionário Serviço", "FuncionarioServico", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Nota Fiscal", "NotasFiscais", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Referência", "Referencia", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", _tamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Número Interno", "NumeroInterno", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("KM Total", "KMTotal", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Hora Total", "HotaTotal", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoVendedor = Request.GetIntParam("Vendedor"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigoServico = Request.GetIntParam("Servico"),
                CodigoFuncionarioServico = Request.GetIntParam("FuncionarioServico"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                CnpjFornecedorServico = Request.GetDoubleParam("FornecedorServico"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataEntregaInicial = Request.GetDateTimeParam("DataEntregaInicial"),
                DataEntregaFinal = Request.GetDateTimeParam("DataEntregaFinal"),
                Status = Request.GetEnumParam<StatusPedidoVenda>("Status"),
                Tipo = Request.GetEnumParam<TipoPedidoVenda>("Tipo"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                ExibirItens = Request.GetBoolParam("ExibirItens"),
                NumeroInternoInicial = Request.GetIntParam("NumeroInternoInicial"),
                NumeroInternoFinal = Request.GetIntParam("NumeroInternoFinal"),
            };
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);

            Dominio.Entidades.Usuario funcionario = filtrosPesquisa.CodigoVendedor > 0 ? repFuncionario.BuscarPorCodigo(filtrosPesquisa.CodigoVendedor) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Produto produto = filtrosPesquisa.CodigoProduto > 0 ? repProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto) : null;
            Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = filtrosPesquisa.CodigoServico > 0 ? repServico.BuscarPorCodigo(filtrosPesquisa.CodigoServico) : null;
            Dominio.Entidades.Usuario funcionarioServico = filtrosPesquisa.CodigoFuncionarioServico > 0 ? repFuncionario.BuscarPorCodigo(filtrosPesquisa.CodigoFuncionarioServico) : null;
            Dominio.Entidades.Cliente fornecedorServico = filtrosPesquisa.CnpjFornecedorServico > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjFornecedorServico) : null;
            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CnpjPessoa > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoa) : null;

            parametros.Add(new Parametro("Numero", filtrosPesquisa.NumeroInicial, filtrosPesquisa.NumeroFinal));
            parametros.Add(new Parametro("Vendedor", funcionario?.Nome));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Parametro("Produto", produto?.Descricao));
            parametros.Add(new Parametro("Servico", servico?.Descricao));
            parametros.Add(new Parametro("FuncionarioServico", funcionarioServico?.Nome));
            parametros.Add(new Parametro("FornecedorServico", fornecedorServico?.Descricao));
            parametros.Add(new Parametro("Cliente", cliente?.Descricao));
            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("DataEntrega", filtrosPesquisa.DataEntregaInicial, filtrosPesquisa.DataEntregaFinal));
            parametros.Add(new Parametro("Status", filtrosPesquisa.Status.ObterDescricao()));
            parametros.Add(new Parametro("Tipo", filtrosPesquisa.Tipo.ObterDescricao()));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));
            parametros.Add(new Parametro("ExibirItens", filtrosPesquisa.ExibirItens ? "Sim" : "Não"));
            parametros.Add(new Parametro("NumeroInterno", filtrosPesquisa.NumeroInternoInicial, filtrosPesquisa.NumeroInternoFinal));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataEmissaoFormatada")
                return "DataEmissao";
            if (propriedadeOrdenar == "DataEntregaFormatada")
                return "DataEntrega";
            if (propriedadeOrdenar == "DescricaoStatus")
                return "Status";
            if (propriedadeOrdenar == "DescricaoTipo")
                return "Tipo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
