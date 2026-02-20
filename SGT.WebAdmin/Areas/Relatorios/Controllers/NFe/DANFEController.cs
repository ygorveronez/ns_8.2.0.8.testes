using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
    [Area("Relatorios")]
    [CustomAuthorize("NotasFiscais/NotaFiscalEletronica")]
    public class DANFEController : BaseController
    {
        #region Construtores

        public DANFEController(Conexao conexao) : base(conexao) { }

        #endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R017_DANFE;

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "DANFE", "NFe", "DANFE.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "CodigoNota", "desc", "", "", Codigo, unitOfWork, false, true, 6);
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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoNFe = Request.GetIntParam("Codigo");
                int codigoTitulo = Request.GetIntParam("CodigoTitulo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal = null;

                if (codigoTitulo > 0)
                    notaFiscal = repNotaFiscal.BuscarPorTitulo(codigoTitulo);
                else
                    notaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNFe);

                if (notaFiscal == null)
                    return new JsonpResult(false, "Nota fiscal não localizada.");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                relatorioTemp.PropriedadeOrdena = propOrdena;

                string stringConexao = _conexao.StringConexao;

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);

                _ = Task.Factory.StartNew(() => serNotaFiscalEletronica.GerarRelatorioDANFE(notaFiscal, agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao));

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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Cód. Produto", "CodigoProduto", 10, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Descrição dos Produtos/Serviços", "DescricaoItem", 25, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("NCM", "CodigoNCMItem", 8, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("CST/CSOSN", "DescricaoCSTItem", 8, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("CFOP", "CodigoCFOPItem", 5, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Unid.", "DescricaoUnidadeItem", 5, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Quantidade", "QuantidadeItem", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("V. Unitário", "ValorUnitarioItem", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("V. Total", "ValorTotalItem", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("B.C. ICMS", "BCICMSItem", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMSItem", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor IPI", "ValorIPIItem", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("% ICMS", "AliquotaICMSItem", 5, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("% IPI", "AliquotaIPIItem", 5, Models.Grid.Align.left, false, true);

            return grid;
        }

        #endregion
    }
}
