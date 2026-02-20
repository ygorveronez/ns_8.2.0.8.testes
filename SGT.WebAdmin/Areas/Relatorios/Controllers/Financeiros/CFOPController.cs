using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/CFOP")]
    public class CFOPController : BaseController
    {
		#region Construtores

		public CFOPController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R190_CFOP;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de CFOP", "Financeiros", "CFOP.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroCFOP", "asc", "", "", Codigo, unitOfWork, true, false);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.CFOP servicoRelatorioCFOP = new Servicos.Embarcador.Relatorios.Financeiros.CFOP(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCFOP.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CFOP> listaCFOP, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCFOP);

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
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP filtrosPesquisa = ObterFiltrosPesquisa();

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

            grid.AdicionarCabecalho("CFOP", "NumeroCFOP", 9, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Extensão", "Extensao", 5, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Tipo", "TipoFormatado", 9, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Uso", "USO", 10, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Reversão", "REVERSAO", 10, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Desconto", "USODESCONTO", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Desconto", "REVERSAODESCONTO", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Outras Despesa", "USOOUTRASDESPESAS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Outras Despesas", "REVERSAOOUTRASDESPESAS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Frete", "USOFRETE", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Frete", "REVERSAOFRETE", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("ICMS", "USOICMS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão ICMS", "REVERSAOICMS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("PIS", "USOPIS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão PIS", "REVERSAOPIS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("COFINS", "USOCOFINS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão COFINS", "REVERSAOCOFINS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("IPI", "USOIPI", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão IPI", "REVERSAOIPI", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("ICMS ST", "USOICMSST", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão ICMS ST", "REVERSAOICMSST", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Diferencial", "USODIFERENCIAL", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Diferencial", "REVERSAODIFERENCIAL", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Seguro", "USOSEGURO", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Seguro", "REVERSAOSEGURO", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Frete Fora", "USOFRETEFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Frete fota", "REVERSAOFRETEFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Outras Fora", "USOOUTRASFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Outras Fora", "REVERSAOOUTRASFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Desconto Fora", "USODESCONTOFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Desconto Fora", "REVERSAODESCONTOFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Imposto Fora", "USOIMPOSTOFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Imposto Fora", "REVERSAOIMPOSTOFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Dif. Frete Fota", "USODIFERENCIALFRETEFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Dif. Frete Fora", "REVERSAODIFERENCIALFRETEFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("ICMS Frete Fora", "USOICMSFRETEFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão ICMS Frete Fora", "REVERSAOICMSFRETEFORA", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Custo", "USOCUSTO", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Custp", "REVERSAOCUSTO", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. PIS", "USORETENCAOPIS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão PIS", "REVERSAORETENCAOPIS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. COFINS", "USORETENCAOCOFINS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão COFINS", "REVERSAORETENCAOCOFINS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. INSS", "USORETENCAOINSS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão INSS", "REVERSAORETENCAOINSS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. IPI", "USORETENCAOIPI", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Ret. IPI", "REVERSAORETENCAOIPI", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. CSLL", "USORETENCAOCSLL", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Ret. CSLL", "REVERSAORETENCAOCSLL", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. Outras", "USORETENCAOOUTRAS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Ret. Outras", "REVERSAORETENCAOOUTRAS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. PIS", "USOTITULORETENCAOPIS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. PIS", "REVERSAOTITULORETENCAOPIS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. COFINS", "USOTITULORETENCAOCOFINS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. COFINS", "REVERSAOTITULORETENCAOCOFINS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. INSS", "USOTITULORETENCAOINSS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. INSS", "REVERSAOTITULORETENCAOINSS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. IPI", "USOTITULORETENCAOIPI", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. IPI", "REVERSAOTITULORETENCAOIPI", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. CSLL", "USOTITULORETENCAOCSLL", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. CSLL", "REVERSAOTITULORETENCAOCSLL", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. Outras", "USOTITULORETENCAOOUTRAS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. Outras", "REVERSAOTITULORETENCAOOUTRAS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. ISS", "USOTITULORETENCAOISS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. ISS", "REVERSAOTITULORETENCAOISS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tit. Ret. IR", "USOTITULORETENCAOIR", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Tit. Ret. IR", "REVERSAOTITULORETENCAOIR", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. ISS", "USORETENCAOISS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Ret. ISS", "REVERSAORETENCAOISS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Ret. IR", "USORETENCAOIR", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Reversão Ret. IR", "REVERSAORETENCAOIR", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Gera Estoque", "GeraEstoqueFormatado", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("CST/CSOSN ICMS", "CSTICMSFormatado", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("% Alíquota Interna", "AliquotaInterna", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("% Alíquota Interestadual", "AliquotaInterestadual", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota do Diferencial", "AliquotaDiferencial", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Bloqueio no Documento de Entrada", "BloqueioDocumentoEntradaFormatado", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Retenção PIS", "AliquotaRetencaoPIS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Retenção COFINS", "AliquotaRetencaoCOFINS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Retenção INSS", "AliquotaRetencaoINSS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Retenção IPI", "AliquotaRetencaoIPI", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Retenção CSLL", "AliquotaRetencaoCSLL", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Outras Retenções", "AliquotaOutrasRetencoes", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Retenção IR", "AliquotaRetencaoIR", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Alíquota Retenção ISS", "AliquotaRetençãoISS", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Realizar o rateio para a despesa do veículo", "RealizarRateioDespesaVeiculoFormatado", 10, Models.Grid.Align.left, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP()
            {
                CodigoCFOP = Request.GetIntParam("CFOP"),
                Extensao = Request.GetStringParam("Extensao"),
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetStringParam("Status"),
                TipoCFOP = Request.GetEnumParam<TipoCFOP>("Tipo"),
                GerarEstoque = Request.GetBoolParam("GerarEstoque"),
                RealizaRateioDespesaVeiculo = Request.GetBoolParam("RealizaRateioDespesaVeiculo")
            };
        }

        #endregion
    }
}
