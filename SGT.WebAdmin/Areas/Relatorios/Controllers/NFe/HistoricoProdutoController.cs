using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/HistoricoProduto")]
    public class HistoricoProdutoController : BaseController
    {
		#region Construtores

		public HistoricoProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R079_HistoricoProduto;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Histórico de Produtos", "NFe", "HistoricoProduto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "CodigoProduto", "asc", "", "", Codigo, unitOfWork, true, true);
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

                int codigoProduto;
                int.TryParse(Request.Params("Produto"), out codigoProduto);
                int.TryParse(Request.Params("GrupoProduto"), out int codigoGrupoProduto);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                TipoEntradaSaida tipoMovimento;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                else
                    int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Dominio.Enumeradores.TipoAmbiente? tipoAmbiente = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoProduto> listaHistoricoProduto = repNotaFiscal.RelatorioHistoricoProduto(codigoEmpresa, codigoProduto, tipoMovimento, dataInicial, dataFinal, tipoAmbiente, codigoGrupoProduto, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite, TipoServicoMultisoftware);
                grid.setarQuantidadeTotal(repNotaFiscal.ContarRelatorioHistoricoProduto(codigoEmpresa, codigoProduto, tipoMovimento, dataInicial, dataFinal, tipoAmbiente, codigoGrupoProduto, TipoServicoMultisoftware));

                grid.AdicionaRows(listaHistoricoProduto);

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
                await unitOfWork.StartAsync(cancellationToken);

                int codigoProduto;
                int.TryParse(Request.Params("Produto"), out codigoProduto);
                int.TryParse(Request.Params("GrupoProduto"), out int codigoGrupoProduto);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                TipoEntradaSaida tipoMovimento;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                else
                    int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Dominio.Enumeradores.TipoAmbiente? tipoAmbiente = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                relatorioTemp.PropriedadeOrdena = ObterPropriedadeOrdenar(relatorioTemp.PropriedadeOrdena);
                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioHistoricoProduto(tipoAmbiente, codigoEmpresa, codigoProduto, tipoMovimento, dataInicial, dataFinal, codigoGrupoProduto, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioHistoricoProduto(Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, int codigoEmpresa, int codigoProduto, TipoEntradaSaida tipoMovimento, DateTime dataInicial, DateTime dataFinal, int codigoGrupoProduto, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoProduto> listaHistoricoProduto = repNotaFiscal.RelatorioHistoricoProduto(codigoEmpresa, codigoProduto, tipoMovimento, dataInicial, dataFinal, tipoAmbiente, codigoGrupoProduto, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, TipoServicoMultisoftware, false);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", tipoMovimento.ObterDescricao()));

                if (codigoProduto > 0)
                {
                    Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigoProduto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", "(" + produto.Codigo.ToString() + ") " + produto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

                if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinal != DateTime.MinValue ? "até " + dataFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                if (codigoGrupoProduto > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProduto = repGrupoProduto.BuscarPorCodigo(codigoGrupoProduto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", grupoProduto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/NFe/HistoricoProduto",parametros,relatorioControleGeracao, relatorioTemp, listaHistoricoProduto, unitOfWork, identificacaoCamposRPT);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Cód. Produto", "CodigoProduto", 3, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Produto", "Produto", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data", "DataFormatada", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("N° Doc.", "Numero", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Qtd.", "QuantidadeFormatada", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Unit.", "ValorUnitario", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Total", "Valor", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Empresa", "Empresa", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 10, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "QuantidadeFormatada")
                return "Quantidade";
            if (propriedadeOrdenar == "DataFormatada")
                return "Data";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
