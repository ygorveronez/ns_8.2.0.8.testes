using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Compras
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Compras/RequisicaoMercadoria")]
    public class RequisicaoMercadoriaController : BaseController
    {
		#region Construtores

		public RequisicaoMercadoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R154_RelRequisicaoMercadoria;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Requisição de Mercadoria", "Compras", "ReportRequisicaoMercadoria.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Compras.RequisicaoMercadoria servicoRelatorioRequisicaoMercadoria = new Servicos.Embarcador.Relatorios.Compras.RequisicaoMercadoria(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioRequisicaoMercadoria.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioRequisicaoMercadoria> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa = ObterFiltrosPesquisa();

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        private Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Empresa"),
                Produto = Request.GetIntParam("Produto"),
                GrupoProduto = Request.GetIntParam("GrupoProduto"),
                Colaborador = Request.GetIntParam("Colaborador"),
                FuncionarioRequisitado = Request.GetIntParam("FuncionarioRequisitado"),
                Motivo = Request.GetIntParam("Motivo"),
                Situacao = Request.GetListEnumParam<SituacaoRequisicaoMercadoria>("Situacao"),
                Tipo = Request.GetEnumParam<ModoRequisicaoMercadoria>("Tipo"),
                SetorFuncionario = Request.GetIntParam("SetorFuncionario")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Numero").Nome("Número").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).OrdAgr(true, true);
            grid.Prop("DataFormatada").Nome("Data").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Colaborador").Nome("Colaborador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("FuncionarioRequisitado").Nome("Funcionário Requisitado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("Empresa").Nome("Empresa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("Motivo").Nome("Motivo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("DescricaoSituacao").Nome("Situação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, false);
            grid.Prop("DescricaoTipo").Nome("Tipo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Produto").Nome("Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("GrupoProduto").Nome("Grupo Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("SetorFuncionario").Nome("Setor do funcionario").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);


            return grid;
        }

        #endregion
    }
}
