using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/CompraVendaNCM")]
    public class CompraVendaNCMController : BaseController
    {
		#region Construtores

		public CompraVendaNCMController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R080_CompraVendaNCM;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Compras e Vendas por NCM", "NFe", "CompraVendaNCM.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "CodigoProduto", "asc", "", "", Codigo, unitOfWork, true, true);
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
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                int codigoProduto, codigoCidade;
                int.TryParse(Request.Params("Produto"), out codigoProduto);
                int.TryParse(Request.Params("Cidade"), out codigoCidade);

                string ncms = Request.Params("NCM");
                string estado = Request.Params("Estado");

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCompraVenda tipoMovimento;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CompraVendaNCM> listaCompraVendaNCM = repNotaFiscal.RelatorioCompraVendaNCM(codigoEmpresa, codigoProduto, codigoCidade, ncms, estado, tipoMovimento, dataInicial, dataFinal, this.Usuario.Empresa.TipoAmbiente, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscal.ContarRelatorioCompraVendaNCM(codigoEmpresa, codigoProduto, codigoCidade, ncms, estado, tipoMovimento, dataInicial, dataFinal, this.Usuario.Empresa.TipoAmbiente));

                grid.AdicionaRows(listaCompraVendaNCM);

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

                int codigoProduto, codigoCidade;
                int.TryParse(Request.Params("Produto"), out codigoProduto);
                int.TryParse(Request.Params("Cidade"), out codigoCidade);

                string ncms = Request.Params("NCM");
                string estado = Request.Params("Estado");

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCompraVenda tipoMovimento;
                Enum.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

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
                _ = Task.Factory.StartNew(() => GerarRelatorioCompraVendaNCM(codigoEmpresa, codigoProduto, codigoCidade, ncms, estado, tipoMovimento, dataInicial, dataFinal, tipoAmbiente, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioCompraVendaNCM(int codigoEmpresa, int codigoProduto, int codigoCidade, string ncms, string estado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCompraVenda tipoMovimento, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CompraVendaNCM> listaCompraVendaNCM = repNotaFiscal.RelatorioCompraVendaNCM(codigoEmpresa, codigoProduto, codigoCidade, ncms, estado, tipoMovimento, dataInicial, dataFinal, tipoAmbiente, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                if ((int)tipoMovimento >= 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", tipoMovimento.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", false));

                if (codigoProduto > 0)
                {
                    Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigoProduto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", "(" + produto.Codigo.ToString() + ") " + produto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

                if (codigoCidade > 0)
                {
                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoCidade);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cidade", "(" + localidade.Codigo.ToString() + ") " + localidade.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cidade", false));

                if (!string.IsNullOrWhiteSpace(estado))
                {
                    Dominio.Entidades.Estado uf = repEstado.BuscarPorSigla(estado);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Estado", "(" + uf.Sigla.ToString() + ") " + uf.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Estado", false));

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NCM", ncms));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", dataInicial, dataFinal));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa));

                serRelatorio.GerarRelatorioDinamico("Relatorios/NFe/CompraVendaNCM",parametros,relatorioControleGeracao, relatorioTemp, listaCompraVendaNCM, unitOfWork, identificacaoCamposRPT);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Cód. Produto", "CodigoProduto", 3, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Produto", "Produto", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CST COFINS", "COFINS", 3, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("CST PIS", "PIS", 3, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("NCM", "NCM", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 3, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("N° Doc.", "Numero", 3, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("UN", "UnidadeMedidaFormatada", 2, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Qtd.", "Quantidade", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Unit.", "ValorUnitario", 4, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Vlr. Total", "Valor", 4, Models.Grid.Align.right, true, true);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "UnidadeMedidaFormatada")
                return "UnidadeMedida";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
