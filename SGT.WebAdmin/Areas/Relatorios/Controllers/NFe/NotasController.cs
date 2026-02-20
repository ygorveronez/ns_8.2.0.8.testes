using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/Notas")]
    public class NotasController : BaseController
    {
		#region Construtores

		public NotasController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R173_Notas;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Notas", "NFe", "Notas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFe.Notas servicoRelatorioNotas = new Servicos.Embarcador.Relatorios.NFe.Notas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNotas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.Notas> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);

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

            grid.AdicionarCabecalho("CFOP", "CFOP", 5, Models.Grid.Align.right, true, false, false, true, true);


            grid.AdicionarCabecalho("Vlr. ICMS", "ValorICMS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. ICMS ST", "ValorICMSST", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. PIS", "ValorPIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. COFINS", "ValorCOFINS", 2, Models.Grid.Align.right, true, false);

            grid.AdicionarCabecalho("Retenção PIS", "RetencaoPIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção COFINS", "RetencaoCOFNIS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção INSS", "RetencaoINSS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção IPI", "RetencaoIPI", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção CSLL", "RetencaoCSLL", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção OUTRAS", "RetencaoOUTRAS", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção IR", "RetencaoIR", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Retenção ISS", "RetencaoISS", 2, Models.Grid.Align.right, true, false);

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
                grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", 8, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Operador Lançamento Doc.", "OperadorLancamentoDocumento", 8, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Operador Finalizou Doc.", "OperadorFinalizouDocumento", 8, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("CNPJ da Filial", "CNPJFilialFormatado", 8, Models.Grid.Align.left, true, false, false, true, false);
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
                grid.AdicionarCabecalho("CentroResultado", false);
                grid.AdicionarCabecalho("OperadorLancamentoDocumento", false);
                grid.AdicionarCabecalho("OperadorFinalizouDocumento", false);
                grid.AdicionarCabecalho("CNPJFilialFormatado", false);
            }

            grid.AdicionarCabecalho("Base ST Retido", "BaseSTRetido", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. ST Retido", "ValorSTRetido", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Vlr. Imp. Fora", "ValorImpostosFora", 2, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Equipamento", "Equipamento", 2, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Ordem de Serviço", "OrdemServico", 3, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Ordem de Compra", "OrdemCompra", 3, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Finalização", "DataFinalizacao", 4, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Finalizado Automaticamente", "DocFinalizadoAutomaticamente", 8, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Motivo Cancelamento", "MotivoCancelamento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Categoria", "CategoriaPessoa", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Litros", "TotalLitrosAbastecimento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor", "ValorTotalAbastecimento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Status Lançamento", "StatusLancamento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 5, Models.Grid.Align.right, true, false, false, true, true);

            return grid;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                Serie = Request.GetIntParam("Serie"),
                CodigosNaturezaOperacao = Request.GetListParam<int>("NaturezaOperacao"),
                CodigoModelo = Request.GetIntParam("Modelo"),
                CodigoEmpresaFilial = Request.GetIntParam("Empresa"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoSegmento = Request.GetIntParam("Segmento"),
                CodigosModeloDocumentoFiscal = Request.GetListParam<int>("ModeloDocumentoFiscal"),
                StatusNotaEntrada = Request.GetEnumParam<SituacaoDocumentoEntrada>("StatusNotaEntrada"),
                SituacaoFinanceiraNotaEntrada = Request.GetEnumParam<StatusTitulo>("SituacaoFinanceiraNotaEntrada"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataEntradaInicial = Request.GetDateTimeParam("DataEntradaInicial"),
                DataEntradaFinal = Request.GetDateTimeParam("DataEntradaFinal"),
                TipoMovimento = Request.GetEnumParam<TipoEntradaSaida>("TipoMovimento"),
                Chave = Request.GetStringParam("Chave"),
                EstadoEmitente = Request.GetStringParam("EstadoEmitente"),
                CodigoEquipamento = Request.GetIntParam("Equipamento"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                TipoAmbiente = this.Usuario.Empresa.TipoAmbiente,
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                OperadorLancamentoDocumento = Request.GetIntParam("OperadorLancamentoDocumento"),
                OperadorFinalizouDocumento = Request.GetIntParam("OperadorFinalizaDocumento"),
                DataInicialFinalizacao = Request.GetDateTimeParam("DataInicialFinalizacao"),
                DataFinalFinalizacao = Request.GetDateTimeParam("DataFinalFinalizacao"),
                DocFinalizadoAutomaticamente = Request.GetIntParam("DocFinalizadoAutomaticamente"),
                Categoria = Request.GetIntParam("Categoria"),

            };

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork, cancellationToken);
            if (filtrosPesquisa.CodigoModelo > 0)
            {
                var result = await repModelo.BuscarPorIdAsync(filtrosPesquisa.CodigoModelo);
                filtrosPesquisa.NumeroModeloNF = result.Numero;
            }

            return filtrosPesquisa;
        }

        #endregion
    }
}
