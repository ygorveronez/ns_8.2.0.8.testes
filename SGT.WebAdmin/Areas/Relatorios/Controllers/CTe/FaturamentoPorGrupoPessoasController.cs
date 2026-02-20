using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/FaturamentoPorGrupoPessoas")]
    public class FaturamentoPorGrupoPessoasController : BaseController
    {
		#region Construtores

		public FaturamentoPorGrupoPessoasController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R007_FaturamentoCTesPorGrupoPessoas;

        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasTexto = (decimal)5.50;
        private int NumeroMaximoComplementos = 60;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;

                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório Faturamento de CT-es por Grupo de Pessoas", "CTe", "FaturamentoPorGrupoPessoas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "GrupoPessoas", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.FaturamentoPorGrupoPessoas servicoRelatorioFaturamentoGrupoPessoas = new Servicos.Embarcador.Relatorios.CTes.FaturamentoPorGrupoPessoas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioFaturamentoGrupoPessoas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.FaturamentoPorGrupoPessoas> listaFaturamento, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaFaturamento);

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
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
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

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = repComponenteFrete.BuscarTodosAtivos();

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Inicio da Prestação", "InicioPrestacao", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Fim da Prestação", "FimPrestacao", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Propriedade do Veículo", "PropriedadeVeiculo", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Proprietário do Veículo", "ProprietarioVeiculo", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Qtd. CT-es", "QuantidadeCTes", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do Serviço", "ValorServico", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do ICMS", "ValorICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do Frete S/ Imposto", "SemImposto", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Volumes", "Volumes", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Digitado", "PesoDigitado", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Faturado", "PesoFaturado", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vlr. Frete / Peso Fat.", "VlrFretePesFat", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Mercadoria", "ValorMercadoria", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("% Frete / Vlr. Mercadoria", "PercVlrFreteVlrMerc", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Peso Fat. / Qtd. CT-es", "PesFatQtdCTe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vlr. Frete S/ Imposto / Qtd. CT-es", "VlrFreteQtdCTe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            for (int i = 0; i < componentes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = componentes[i];

                if (i < NumeroMaximoComplementos)
                    grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + (i + 1).ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum, componentes[i].Codigo);
                else
                    break;
            }

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorGrupoPessoas()
            {
                CodigosGruposPessoas = Request.GetListParam<int>("GruposPessoas"),
                CodigosModeloDocumentoFiscal = Request.GetListParam<int>("ModeloDocumentoFiscal"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataInicialAutorizacao = Request.GetDateTimeParam("DataInicialAutorizacao"),
                DataFinalAutorizacao = Request.GetDateTimeParam("DataFinalAutorizacao"),
                PropriedadeVeiculo = Request.GetStringParam("PropriedadeVeiculo"),
                SomenteCTesDeMinutas = Request.GetBoolParam("SomenteCTesDeMinutas"),
                DocumentoFaturavel = Request.GetBoolParam("DocumentoFaturavel"),
                VinculoCarga = Request.GetBoolParam("VinculoCarga"),
                TipoAmbiente = this.Empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Producao,
                TipoProposta = Request.GetNullableEnumParam<TipoPropostaMultimodal>("TipoProposta")
            };
        }

        #endregion
    }
}

