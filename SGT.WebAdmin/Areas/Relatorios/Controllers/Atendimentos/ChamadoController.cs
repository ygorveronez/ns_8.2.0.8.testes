using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Atendimentos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Atendimentos/Chamado")]
    public class ChamadoController : BaseController
    {
		#region Construtores

		public ChamadoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R073_Chamado;

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Chamados", "Atendimentos", "Chamado.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "DataChamado", "desc", "", "", Codigo, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Atendimento.Chamado servicoRelatorioChamado = new Servicos.Embarcador.Relatorios.Atendimento.Chamado(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioChamado.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Atendimentos.RelatorioChamado> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado filtrosPesquisa = ObterFiltrosPesquisa();

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

        #region Métodos Privads

        private Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Atendimento.FiltroPesquisaRelatorioChamado()
            {
                Tela = Request.GetIntParam("Tela"),
                Modulo = Request.GetIntParam("Modulo"),
                Sistema = Request.GetIntParam("Sistema"),
                Tipo = Request.GetIntParam("Tipo"),
                Funcionario = Request.GetIntParam("Funcionario"),
                Empresa = Request.GetIntParam("Empresa"),
                EmpresaFilho = Request.GetIntParam("EmpresaFilho"),
                Solicitante = Request.GetIntParam("Solicitante"),
                Titulo = Request.GetStringParam("Titulo"),
                DataChamadoInicial = Request.GetDateTimeParam("DataChamadoInicial"),
                DataChamadoFinal = Request.GetDateTimeParam("DataChamadoFinal"),
                DataAtendimentoInicial = Request.GetDateTimeParam("DataAtendimentoInicial"),
                DataAtendimentoFinal = Request.GetDateTimeParam("DataAtendimentoFinal"),
                Status = Request.GetEnumParam<StatusAtendimentoTarefa>("Status"),
                Prioridade = Request.GetEnumParam<PrioridadeAtendimento>("Prioridade"),
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Data Chamado", "DataChamado", 5, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Atendimento", "DataAtendimento", 5, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Prioridade", "DescricaoPrioridade", 3, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Tela", "Tela", 6, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Modulo", "Modulo", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Sistema", "Sistema", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo", "Tipo", 5, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Empresa", "Empresa", 6, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Empresa Filho", "EmpresaFilho", 6, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Título", "Titulo", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Motivo/Problema", "Motivo", 15, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 6, Models.Grid.Align.left, false, true);

            grid.AdicionarCabecalho("Atendente", "Funcionario", 6, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Colaborador", "Pessoa", 6, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Observação Suporte", "ObservacaoSuporte", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Justificativa Suporte", "Justificativa", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Solicitante", "Solicitante", 6, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        #endregion
    }
}
