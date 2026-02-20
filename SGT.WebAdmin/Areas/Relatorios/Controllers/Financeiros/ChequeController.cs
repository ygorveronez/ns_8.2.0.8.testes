using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/Cheque")]
    public class ChequeController : BaseController
    {
		#region Construtores

		public ChequeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R159_Cheque;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Cheques", "Financeiros", "Cheque.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.Cheque servicoRelatorioCheque = new Servicos.Embarcador.Relatorios.Financeiros.Cheque(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCheque.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Cheque> listaCheque, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCheque);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa = ObterFiltrosPesquisa();

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
        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio()
            {
                CodigoTitulo = Request.GetIntParam("Titulo"),
                CpfCnpjPessoa = Request.GetDoubleParam("Pessoa"),
                DataCompensacaoInicio = Request.GetNullableDateTimeParam("DataCompensacaoInicio"),
                DataCompensacaoLimite = Request.GetNullableDateTimeParam("DataCompensacaoLimite"),
                DataTransacaoInicio = Request.GetNullableDateTimeParam("DataTransacaoInicio"),
                DataTransacaoLimite = Request.GetNullableDateTimeParam("DataTransacaoLimite"),
                DataVencimentoInicio = Request.GetNullableDateTimeParam("DataVencimentoInicio"),
                DataVencimentoLimite = Request.GetNullableDateTimeParam("DataVencimentoLimite"),
                NumeroCheque = Request.GetStringParam("NumeroCheque"),
                Status = Request.GetNullableEnumParam<StatusCheque>("Status"),
                Tipo = Request.GetNullableEnumParam<TipoCheque>("Tipo"),
                ValorInicio = Request.GetDecimalParam("ValorInicio"),
                ValorLimite = Request.GetDecimalParam("ValorLimite"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                Banco = Request.GetIntParam("Banco")
            };
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("CNPJ", "CnpjEmpresa", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Empresa", "RazaoSocialEmpresa", 16, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ", "CpfCnpjPessoaFormatado", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Pessoa", "NomePessoa", 16, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Banco", "DescricaoBanco", 12, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Agência", "NumeroAgencia", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Dígito", "DigitoAgencia", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Conta", "NumeroConta", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Status", "StatusDescricao", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo", "TipoDescricao", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Nº Cheque", "NumeroCheque", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Cadastro", "DataCadastroFormatada", 12, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Transação", "DataTransacaoFormatada", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Vencimento", "DataVencimentoFormatada", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Compensação", "DataCompensacaoFormatada", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 16, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Título", "NumeroTitulo", 16, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        #endregion
    }
}
