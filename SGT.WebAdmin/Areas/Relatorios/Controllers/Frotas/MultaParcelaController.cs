using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/MultaParcela")]
    public class MultaParcelaController : BaseController
    {
		#region Construtores

		public MultaParcelaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R275_MultaParcela;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Parcelas de Infrações", "Frotas", "MultaParcela.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Veiculo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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

        public async Task<IActionResult> Pesquisa(CancellationToken  cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.MultaParcela servicoRelatorioMultaParcela = new Servicos.Embarcador.Relatorios.Frotas.MultaParcela(unitOfWork, TipoServicoMultisoftware, Cliente, cancellationToken);

                var lista = await servicoRelatorioMultaParcela.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(lista.Count);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa = ObterFiltrosPesquisa();

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

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                DataVencimentoInicialPagar = Request.GetDateTimeParam("DataVencimentoInicialPagar"),
                DataVencimentoFinalPagar = Request.GetDateTimeParam("DataVencimentoFinalPagar"),
                DataLimiteInicial = Request.GetDateTimeParam("DataLimiteInicial"),
                DataLimiteFinal = Request.GetDateTimeParam("DataLimiteFinal"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                NumeroMulta = Request.GetIntParam("NumeroMulta"),
                CodigoCidade = Request.GetIntParam("Cidade"),
                CodigosTipoInfracoes = Request.GetListParam<int>("TipoInfracao"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoTitulo = Request.GetIntParam("Titulo"),
                TipoTipoInfracao = Request.GetStringParam("TipoTipoInfracao"),
                NivelInfracao = Request.GetStringParam("NivelInfracao"),
                NumeroAtuacao = Request.GetStringParam("NumeroAtuacao"),
                TipoOcorrenciaInfracao = Request.GetStringParam("TipoOcorrenciaInfracao"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                CnpjFornecedorPagar = Request.GetDoubleParam("FornecedorPagar"),
                PagoPor = Request.GetEnumParam<ResponsavelPagamentoInfracao>("PagoPor"),
                StatusMulta = Request.GetEnumParam<SituacaoInfracao>("StatusMulta"),
                DataLancamentoInicial = Request.GetDateTimeParam("DataLancamentoInicial"),
                DataLancamentoFinal = Request.GetDateTimeParam("DataLancamentoFinal")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Atuação", "NumeroAtuacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Multa", "NumeroMulta", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data", "DescricaoDataMulta", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Pago Por", "DescricaoPagoPor", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Local Infração", "LocalInfracao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Cidade", "Cidade", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de Infração", "TipoInfracao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Vencimento", "DescricaoVencimento", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Compensação", "DescricaoCompensacao", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Até Vencimento", "ValorAteVencimento", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Após Vencimento", "ValorAposVencimento", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Cod. Título", "CodigoTitulo", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Status Título", "DescricaoSituacaoTitulo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Saldo Título", "SaldoTitulo", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Status Multa", "DescricaoStatusMulta", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº Matrícula", "NumeroMatriculaMotorista", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Parcela", "NumeroParcela", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Título Parcela", "TituloParcela", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vcto Pagar", "DescricaoVencimentoPagar", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Fornecedor", "FornecedorPagar", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nível", "DescricaoNivel", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Tipo Infração", "ValorTipoInfracao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Pontos Tipo Infração", "PontosTipoInfracao", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("% Red. Comissão", "ReducaoComissao", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Cód. Integração Motorista", "CodigoIntegracaoMotorista", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
