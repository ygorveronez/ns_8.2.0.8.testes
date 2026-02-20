using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/RetornoBoleto")]
    public class RetornoBoletoController : BaseController
    {
		#region Construtores

		public RetornoBoletoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R065_RetornoBoleto;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Retorno de Boleto", "Financeiros", "RetornoBoleto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NossoNumero", "asc", "", "", Codigo, unitOfWork, true, false);
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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.RetornoBoleto servicoRelatorioRetornoBoleto = new Servicos.Embarcador.Relatorios.Financeiros.RetornoBoleto(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioRetornoBoleto.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
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

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto()
            {
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                DataInicialImportacao = Request.GetDateTimeParam("DataInicialImportacao"),
                DataFinalImportacao = Request.GetDateTimeParam("DataFinalImportacao"),
                DataInicialOcorrencia = Request.GetDateTimeParam("DataInicialOcorrencia"),
                DataFinalOcorrencia = Request.GetDateTimeParam("DataFinalOcorrencia"),
                BoletoConfiguracao = Request.GetIntParam("BoletoConfiguracao"),
                BoletoComando = Request.GetIntParam("BoletoComando"),
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Nº Comando", "Comando", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Comando", "DescricaoComando", 15, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Boleto Retorno", "NossoNumero", 10, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Nº Banco", "Banco", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 8, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Data Ocorrentia", "DataOcorrencia", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor Retorno", "ValorRetorno", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Documento", "ValorDocumento", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Juros", "ValorJuros", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Outras", "ValorOutrasDespesas", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Tarifa", "ValorTarifa", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Recebido", "ValorRecebido", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Data Crédito", "DataCredito", 8, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Cód. Rejeição", "CodigoRejeicao", 4, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Baixa", "DataBaixa", 8, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Importação", "DataImportacao", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Título", "CodigoTitulo", 8, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Vencimento Título", "VencimentoTitulo", 8, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Emissão Título", "EmissaoTitulo", 8, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Valor Título", "ValorTitulo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Nº Boleto Título", "NossoNumeroTitulo", 10, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Nº Sequencia", "Sequencia", 20, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Cliente", "Cliente", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Empresa", "Empresa", 15, Models.Grid.Align.left, false, false);

            return grid;
        }

        #endregion
    }
}
