using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Canhotos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Canhotos/HistoricoMovimentacaoCanhoto")]
    public class HistoricoMovimentacaoCanhotoController : BaseController
    {
		#region Construtores

		public HistoricoMovimentacaoCanhotoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R291_HistoricoMovimentacaoCanhoto;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;
        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Historico Movimentação Canhoto", "Canhotos", "HistoricoMovimentacaoCanhoto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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
                grid.indiceColunaOrdena = 1;
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Canhotos.HistoricoMovimentacaoCanhoto servicoHistoricoMovimentacao = new Servicos.Embarcador.Relatorios.Canhotos.HistoricoMovimentacaoCanhoto(unitOfWork, TipoServicoMultisoftware, Cliente);
                servicoHistoricoMovimentacao.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.HistoricoMovimentacaoCanhoto> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa = ObterFiltrosPesquisa();
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o reltároio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número canhoto", "NumeroCanhoto", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de upload", "DataUploadFormatada", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de Aprovação", "DataAprovacaoFormatada", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de Rejeição", "DataRejeicaoFormatada", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de Confirmação Entrega ", "DataConfirmacaoEntregaFormatada", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de Reversão", "DataReversaoFormatada", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de Recebimento Físico", "DataRecebimentoFisicoFormatada", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Usuário", "Usuario", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Motivo Rejeição", "MotivoRejeicao", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Série canhoto", "SerieCanhoto", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("CNPJ Emitente", "CNPJEmitenteFormatado", 3m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Nome Emitente", "NomeEmitente", 3m, Models.Grid.Align.left, false, true);




            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto ObterFiltrosPesquisa()
        {

            return new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto
            {
                DataAprovacao = Request.GetDateTimeParam("DataAprovacao"),
                DataConfirmacaoEntrega = Request.GetDateTimeParam("DataConfirmacaoEntrega"),
                DataRecebimentoFisico = Request.GetDateTimeParam("DataRecebimentoFisico"),
                DataReversao = Request.GetDateTimeParam("DataReversao"),
                DataUpload = Request.GetDateTimeParam("DataUpload"),
                DataRejeicao = Request.GetDateTimeParam("DataRejeicao"),
                MotivoRejeicao = Request.GetIntParam("MotivoRejeicao"),
                Usuario =  Request.GetIntParam("Usuario"),
                NumeroCanhoto = Request.GetIntParam("NumeroCanhoto"),
                CodigoEmitente = Request.GetStringParam("Emitente")
            };
        }

        #endregion

    }
}
