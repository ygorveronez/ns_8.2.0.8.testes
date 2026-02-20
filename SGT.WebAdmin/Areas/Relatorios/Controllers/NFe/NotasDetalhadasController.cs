using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/NotasDetalhadas")]
    public class NotasDetalhadasController : BaseController
    {
		#region Construtores

		public NotasDetalhadasController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R084_NotasDetalhadas;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Notas Detalhadas", "NFe", "NotasDetalhadas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
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

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFe.NotasDetalhadas servicoRelatorioNotasDetalhadas = new Servicos.Embarcador.Relatorios.NFe.NotasDetalhadas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNotasDetalhadas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasDetalhadas> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAcerto);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
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

            grid.AdicionarCabecalho("Número", "Numero", 3, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Série", "Serie", 2, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", 4, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Entrada", "DataEntradaFormatada", 4, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cidade", "Cidade", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Modelo", "Modelo", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Natureza Operação", "NaturezaOperacao", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº Chave", "Chave", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 3, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Vlr. Total NF", "ValorTotal", 3, Models.Grid.Align.right, true, true);

            grid.AdicionarCabecalho("Cód. Prod./Serv.", "CodigoProduto", 3, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Produto/Serviço", "Produto", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UN", "UnidadeMedidaFormatada", 2, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CFOP", "CFOP", 5, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Qtd.", "Quantidade", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Unit.", "ValorUnitario", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Total", "Valor", 4, Models.Grid.Align.right, true, true);

            grid.AdicionarCabecalho("Vlr. Unit. Liquido", "ValorUnitarioLiquido", 4, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Vlr. Total Liquido", "ValorLiquido", 4, Models.Grid.Align.right, false, false);

            grid.AdicionarCabecalho("CST ICMS", "CstICMS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("BC ICMS", "BaseICMS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Alí. ICMS", "AliquotaICMS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. ICMS", "ValorICMS", 2, Models.Grid.Align.right, true, false);

            grid.AdicionarCabecalho("BC ICMS ST", "BaseICMSST", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("MVA", "MVA", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Alí. ICMS ST", "AliquotaICMSST", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. ICMS ST", "ValorICMSST", 2, Models.Grid.Align.right, true, false);

            grid.AdicionarCabecalho("CST PIS", "CstPIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("BC PIS", "BasePIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Alí. PIS", "AliquotaPIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. PIS", "ValorPIS", 2, Models.Grid.Align.right, true, false);

            grid.AdicionarCabecalho("CST COFINS", "CstCOFINS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("BC COFINS", "BaseCOFINS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Alí. COFINS", "AliquotaCOFINS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. COFINS", "ValorCOFINS", 2, Models.Grid.Align.right, true, false);

            grid.AdicionarCabecalho("Retenção PIS", "RetencaoPIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção COFINS", "RetencaoCOFNIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção INSS", "RetencaoINSS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção IPI", "RetencaoIPI", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção CSLL", "RetencaoCSLL", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção OUTRAS", "RetencaoOUTRAS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção IR", "RetencaoIR", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção ISS", "RetencaoISS", 2, Models.Grid.Align.right, true, false);

            grid.AdicionarCabecalho("CST IPI", "CstIPI", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("BC IPI", "BaseIPI", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Alí. IPI", "AliquotaIPI", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. IPI", "ValorIPI", 2, Models.Grid.Align.right, true, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Filial", "Empresa", 15, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Situação Financeira Nota", "SituacaoFinanceiraNota", 10, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Data Vencimento Títulos", "DataVencimento", 10, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Data Pagamento Títulos", "DataPagamento", 10, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Estado Pessoa", "EstadoPessoa", 5, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("CNPJ/CPF Pessoa", "CPFCNPJFormatado", 8, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Tipo Veículo", "TipoVeiculo", 10, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Segmento", "Segmento", 8, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Cód. Produto", "ProdutoCodigoProduto", 3, Models.Grid.Align.center, true, false, false, true, false);
            }
            else
            {
                grid.AdicionarCabecalho("Veiculo", false);
                grid.AdicionarCabecalho("Empresa", false);
                grid.AdicionarCabecalho("SituacaoFinanceiraNota", false);
                grid.AdicionarCabecalho("DataVencimento", false);
                grid.AdicionarCabecalho("DataPagamento", false);
                grid.AdicionarCabecalho("EstadoPessoa", false);
                grid.AdicionarCabecalho("CPFCNPJFormatado", false);
                grid.AdicionarCabecalho("TipoVeiculo", false);
                grid.AdicionarCabecalho("Segmento", false);
                grid.AdicionarCabecalho("ProdutoCodigoProduto", false);
            }

            grid.AdicionarCabecalho("Desconto", "Desconto", 4, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Base ST Retido", "BaseSTRetido", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. ST Retido", "ValorSTRetido", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Vlr. Imp. Fora", "ValorImpostosFora", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Tip. Movimento", "TipoMovimento", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Equipamento", "Equipamento", 3, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Km Abastecimento", "KmAbastecimento", 3, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Horímetro", "Horimetro", 3, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Regime Tributário", "RegimeTributarioFormatada", 3, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("UN Fornecedor", "UnidadeMedidaFornecedor", 3, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Qtd. Fornecedor", "QuantidadeFornecedor", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. Unit. Fornecedor", "ValorUnitarioFornecedor", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Operador de Lançamento", "OperadorLancamentoDocumento", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Operador de Finalização", "OperadorFinalizaDocumento", 2, Models.Grid.Align.right, true, false);

            grid.AdicionarCabecalho("Serviço", "Servico", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Localidade Prestação Serviço", "LocalidadePrestacaoServico", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Documento", "TipoDocumentoFormatada", 3, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CST Serviço", "CSTServicoFormatada", 4, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota Simples Nacional", "AliquotaSimplesNacional", 4, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Documento Fiscal Proveniente Simples Nacional", "DocumentoFiscalProvenienteSimplesNacionalFormatado", 2, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tributa ISS no Município", "TributaISSNoMunicipioFormatado", 2, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Tabela", "ValorAbastecimentoTabelaFornecedor", 2, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor com Divergência", "ValorAbastecimentoComDivergenciaFormatdo", 2, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Local de Armazenamento", "LocalArmazenamento", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Descrição CFOP", "DescricaoCFOP", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Finalização", "DataFinalizacao", 10, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Outras Despesas", "OutrasDespesas", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Seguro", "ValorSeguro", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Diferencial", "ValorDiferencial", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Abastecimento", "DataAbastecimentoFormatada", 10, Models.Grid.Align.center, false, false, false, false, false);

            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilialFormatado", 8, Models.Grid.Align.left,true, false, false, false, false);

            grid.AdicionarCabecalho("N° Ordem de Compra", "OrdemCompra", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("N° Ordem de Serviço", "OrdemServico", 8, Models.Grid.Align.center, true, false, false, false, false);

            grid.AdicionarCabecalho("Custo Unitário", "CustoUnitario", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Custo Total", "CustoTotal", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("CST ICMS do Fornecedor", "CstIcmsFornecedor", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("CFOP do Fornecedor", "CfopFornecedor", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Base de Cálculo ICMS do Fornecedor", "BaseCalculoICMSFornecedor", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota ICMS Fornecedor", "AliquotaICMSFornecedor", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ICMS do Fornecedor", "ValorICMSFornecedor", 8, Models.Grid.Align.center, true, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                Serie = Request.GetIntParam("Serie"),
                CodigosNaturezaOperacao = Request.GetListParam<int>("NaturezaOperacao"),
                CodigoProduto = Request.GetIntParam("Produto"),
                CodigoServico = Request.GetIntParam("Servico"),
                CodigoModelo = Request.GetIntParam("Modelo"),
                CodigoEmpresaFilial = Request.GetIntParam("Empresa"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigosGrupoProduto = Request.GetListParam<int>("GrupoProduto"),
                CodigoSegmento = Request.GetIntParam("Segmento"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CodigosModeloDocumentoFiscal = Request.GetListParam<int>("ModeloDocumentoFiscal"),
                CodigosTipoMovimento = Request.GetListParam<int>("TipoMovimentoItem"),
                StatusNotaEntrada = Request.GetEnumParam<SituacaoDocumentoEntrada>("StatusNotaEntrada"),
                SituacaoFinanceiraNotaEntrada = Request.GetEnumParam<StatusTitulo>("SituacaoFinanceiraNotaEntrada"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataEntradaInicial = Request.GetDateTimeParam("DataEntradaInicial"),
                DataEntradaFinal = Request.GetDateTimeParam("DataEntradaFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                TipoMovimento = Request.GetEnumParam<TipoEntradaSaida>("TipoMovimento"),
                Chave = Request.GetStringParam("Chave"),
                EstadoEmitente = Request.GetStringParam("EstadoEmitente"),
                CodigoEquipamento = Request.GetIntParam("Equipamento"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa?.Codigo ?? 0 : 0,
                TipoAmbiente = this.Usuario.Empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Nenhum,
                OperadorLancamentoEntrada = Request.GetIntParam("OperadorLancamentoEntrada"),
                OperadorFinalizaEntrada = Request.GetIntParam("OperadorFinalizaEntrada"),
                NotasComDiferencaDeValorTabelaFornecedor = Request.GetBoolParam("NotasComDiferencaDeValorTabelaFornecedor"),
                DataFinalizacaoInicial = Request.GetDateTimeParam("DataFinalizacaoInicial"),
                DataFinalizacaoFinal = Request.GetDateTimeParam("DataFinalizacaoFinal")
            };

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            if (filtrosPesquisa.CodigoModelo > 0)
                filtrosPesquisa.NumeroModeloNF = new Repositorio.ModeloDocumentoFiscal(unitOfWork).BuscarPorId(filtrosPesquisa.CodigoModelo).Numero;

            return filtrosPesquisa;
        }

        #endregion
    }
}
