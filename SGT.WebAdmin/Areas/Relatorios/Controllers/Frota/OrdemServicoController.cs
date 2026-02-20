using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frota
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frota/OrdemServico")]
    public class OrdemServicoController : BaseController
    {
		#region Construtores

		public OrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R202_OrdemServico;

        private readonly decimal TamanhoColunaPequena = 1.75m;
        private readonly decimal TamanhoColunaGrande = 5.50m;
        private readonly decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Ordens de Serviço", "Frota", "RelatorioOrdemServico.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.OrdemServico servicoRelatorioOrdemServico = new Servicos.Embarcador.Relatorios.Frotas.OrdemServico(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioOrdemServico.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico> listaExtratoConta, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaExtratoConta);

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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data", "DataFormatada", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Local de Manutenção", "LocalManutencao", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cidade do Local", "CidadeLocalManutencao", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF do Local", "UFLocalManutencao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Operador", "Operador", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Operador Finalizou OS.", "OperadorFechamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Manutenção", "DescricaoTipoManutencao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Tipo", "Tipo", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Equipamento", "Equipamento", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.Prop("ValorProdutos").Nome("Vlr. Produtos Orçado").Tamanho(TamanhoColunaPequena).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorServicos").Nome("Vlr. Serviços Orçado").Tamanho(TamanhoColunaPequena).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorTotal").Nome("Vlr. Total Orçado").Tamanho(TamanhoColunaPequena).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorProdutosFechamento").Nome("Vlr. Produtos Realizado").Tamanho(TamanhoColunaPequena).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorServicosFechamento").Nome("Vlr. Serviços Realizado").Tamanho(TamanhoColunaPequena).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorTotalFechamento").Nome("Vlr. Total Realizado").Tamanho(TamanhoColunaPequena).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.AdicionarCabecalho("Serviços", "Servicos", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("KM", "QuilometragemVeiculo", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Horímetro", "Horimetro", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);

            grid.AdicionarCabecalho("Grupo Serviço", "GrupoServico", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Segmento", "Segmento", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ da Pessoa", "CPFCNPJPessoa", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Doc. Entrada", "DocumentoEntrada", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Diferença Produtos Orç. X Real.", "DiferencaTotais", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Data/Hora Inclusão", "DataHoraInclusaoFormatada", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Diferença Valor Orçado X Realizado", "DiferencaValorOrcadoRealizado", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Mecânico", "Mecanicos", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Previsto", "TempoPrevisto", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Tempo Executado", "TempoExecutado", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Condição de Pagamento", "CondicaoPagamento", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Prioridade", "PrioridadeDescricao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Data Limite Execução", "DataLimiteExecucaoFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data/Hora Liberação", "DataLiberacaoFormatada", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data/Hora Fechamento", "DataFechamentoFormatada", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Produto(s)", "Produtos", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo de Produto", "GrupoProdutos", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade de Produto", "QuantidadeProduto", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Unitário Produto", "ValorUnitarioProduto", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total Produto", "ValorTotalProduto", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Fornecedor", "NomeFornecedorDocumentoEntrada", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor NF", "ValorNF", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Ordem Compra", "NumeroOrdemCompra", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Produto", "CodigoProduto", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico()
            {
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                LocaisManutencao = Request.GetListParam<double>("LocalManutencao"),
                Motoristas = Request.GetListParam<int>("Motorista"),
                NumeroFinal = Request.GetNullableIntParam("NumeroFinal"),
                NumeroInicial = Request.GetNullableIntParam("NumeroInicial"),
                Servicos = Request.GetListParam<int>("Servico"),
                Situacao = Request.GetListEnumParam<SituacaoOrdemServicoFrota>("Situacao"),
                TipoManutencao = Request.GetListEnumParam<TipoManutencaoOrdemServicoFrota>("TipoManutencao"),
                Tipos = Request.GetListParam<long>("Tipo"),
                Veiculos = Request.GetListParam<int>("Veiculo"),
                Equipamentos = Request.GetListParam<int>("Equipamento"),
                TipoOrdemServico = Request.GetNullableEnumParam<TipoOficina>("TipoOrdemServico"),
                MarcaVeiculo = Request.GetIntParam("MarcaVeiculo"),
                ModeloVeiculo = Request.GetIntParam("ModeloVeiculo"),
                GrupoServicos = Request.GetListParam<int>("GrupoServico"),
                CentroResultados = Request.GetListParam<int>("CentroResultado"),
                Segmentos = Request.GetListParam<int>("Segmento"),
                CidadesPessoa = Request.GetListParam<int>("CidadePessoa"),
                UFsPessoa = Request.GetListParam<string>("UFPessoa"),
                OperadorLancamentoDocumento = Request.GetIntParam("OperadorLancamentoDocumento"),
                OperadorFinalizouDocumento = Request.GetIntParam("OperadorFinalizaDocumento"),
                DataInicialInclusao = Request.GetNullableDateTimeParam("DataInicialInclusao"),
                DataFinalInclusao = Request.GetNullableDateTimeParam("DataFinalInclusao"),
                Mecanicos = Request.GetListParam<int>("Mecanicos"),
                DataInicialLimiteExecucao = Request.GetNullableDateTimeParam("DataInicialLimiteExecucao"),
                DataFinalLimiteExecucao = Request.GetNullableDateTimeParam("DataFinalLimiteExecucao"),
                Prioridade = Request.GetEnumParam<PrioridadeOrdemServico>("Prioridade"),
                DataLiberacaoInicio = Request.GetNullableDateTimeParam("DataLiberacaoInicio"),
                DataLiberacaoFim = Request.GetNullableDateTimeParam("DataLiberacaoFim"),
                DataFechamentoInicio = Request.GetNullableDateTimeParam("DataFechamentoInicio"),
                DataFechamentoFim = Request.GetNullableDateTimeParam("DataFechamentoFim"),
                DataReaberturaInicio = Request.GetNullableDateTimeParam("DataReaberturaInicio"),
                DataReaberturaFim = Request.GetNullableDateTimeParam("DataReaberturaFim"),
                CodigosGrupoProdutoTMS = Request.GetListParam<int>("GrupoProdutoTMS"),
                CodigosProdutoTMS = Request.GetListParam<int>("ProdutoTMS"),
            };
        }

        #endregion
    }
}
