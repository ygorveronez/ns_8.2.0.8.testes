using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.MDFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Mdfe/MDFesAverbados")]
    public class MDFesAverbadosController : BaseController
    {
		#region Construtores

		public MDFesAverbadosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R255_MDFesAverbados;
        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoLayout = 6;
        private int NumeroMaximoComplementos = 60;
        private decimal TamanhoColunasValores = (decimal) 1.75;
        private decimal TamanhoColunasParticipantes = (decimal) 5.50;
        private decimal TamanhoColunasLocalidades = 3;
        private decimal TamanhoColunasData = 3;
        private decimal TamanhoColunasDescritivos = (decimal) 5.50;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de MDF-e(s) averbados", "MDFe", "MDFesAverbados.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadraoAsync(unitOfWork, cancellationToken), relatorio);

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
                DateTime.TryParseExact(Request.Params("DataInicialEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinalEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                if (!ValidarPeriodoRelatorio(dataInicial, dataFinal, out string mensagem))
                    return new JsonpResult(false, mensagem);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.MDFes.MDFesAverbados servicoRelatorioMDFeAverbados = new Servicos.Embarcador.Relatorios.MDFes.MDFesAverbados(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioMDFeAverbados.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.MDFe.MDFesAverbados> listaMDFesAverbados, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaMDFesAverbados);
                grid.setarQuantidadeTotal(countRegistros);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime.TryParseExact(Request.Params("DataInicialEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinalEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                if (!ValidarPeriodoRelatorio(dataInicial, dataFinal, out string mensagem))
                    return new JsonpResult(false, mensagem);

                string stringConexao = _conexao.StringConexao;
                int codigoEmpresa = Empresa?.Codigo ?? 0;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        private async Task<Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio()
            {
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                Status = Request.GetNullableEnumParam<Dominio.Enumeradores.StatusAverbacaoMDFe>("Status"),
                CodigoSeguradora = Request.GetIntParam("Seguradora"),
                CodigoTransportador = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Request.GetIntParam("Transportador") : this.Empresa.Codigo,
                CodigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                CodigosFiliais = await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken),
                CodigosRecebedores = await ObterListaCnpjCpfRecebedorPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken)
            };

            return filtrosPesquisa;
        }

        private async Task<Models.Grid.Grid> ObterGridPadraoAsync(Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            UltimaColunaDinanica = 1;

            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho, cancellationToken);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho, cancellationToken);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho, cancellationToken);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = await repComponenteFrete.BuscarTodosAtivosAsync();
            List<Dominio.Entidades.LayoutEDI> layoutes = await repLayoutEDI.BuscarParaRelatoriosAsync();

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            // --------------------- descricao - prop ---------- tamanho -------------- alinhamento ------ pOrd- ocTbt- ocPh - pAgrp- visivel
            grid.AdicionarCabecalho("Número", "Numero", 1, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "Serie", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Data de Emissão", "DataEmissaoFormatada", TamanhoColunasData, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", TamanhoColunasData, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estado de Carregamento", "EstadoCarregamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Estado de Descarregamento", "EstadoDescarregamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Carga", "Carga", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo da Operação", "TipoOperacao", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Veículos", "Veiculos", 3, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Motoristas", "Motoristas", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("CNPJ Empresa", "CNPJEmpresa", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Empresa", "NomeEmpresa", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, true);
            }
            else
            {
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJEmpresa", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Transportador", "NomeEmpresa", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, true);
            }

            grid.AdicionarCabecalho("Seguradora", "Seguradora", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Averbadora", "DescricaoAverbadora", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Apólice", "Apolice", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Averbação", "Averbacao", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Averbação", "DataAverbacaoFormatada", TamanhoColunasData, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação MDF-e", "DescricaoSituacaoMDFe", TamanhoColunasData, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Mercadoria", "ValorMercadoria", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso", "Peso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);

            return grid;
        }

        #endregion
    }
}
