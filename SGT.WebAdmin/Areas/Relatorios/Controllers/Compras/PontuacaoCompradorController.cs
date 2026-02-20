using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Compras
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Compras/PontuacaoComprador")]
    public class PontuacaoCompradorController : BaseController
    {
		#region Construtores

		public PontuacaoCompradorController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R120_PontuacaoComprador;

        private Models.Grid.Grid GridPadrao()
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };
            grid.Prop("Comprador").Nome("Comprador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Produto").Nome("Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("DataRequisicao").Nome("Data Requisição").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("DataOrdemCompra").Nome("Data Ordem Compra").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("QtdDias").Nome("Qtd. Dias").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pontuação de Comprador", "Compras", "PontuacaoComprador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Comprador", "desc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                List<Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (from o in listaReport
                             select new
                             {
                                 o.Comprador,
                                 o.Produto,
                                 o.DataOrdemCompra,
                                 o.DataRequisicao,
                                 QtdDias = o.DataOrdemCompra - o.DataRequisicao
                             }).ToList();

                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(lista);

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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                await GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, cancellationToken);

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

        private async Task GerarRelatorioAsync(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo, cancellationToken);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                List<Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);
                var lista = (from o in listaReport
                             select new
                             {
                                 o.Comprador,
                                 o.Produto,
                                 o.DataOrdemCompra,
                                 o.DataRequisicao,
                                 QtdDias = o.DataOrdemCompra - o.DataRequisicao
                             }).ToList();

                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, lista, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Compras/PontuacaoComprador", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Compras/PontuacaoComprador", parametros, relatorioControleGeracao, relatorioTemp, lista, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena == "SituacaoDescricao") propOrdena = "Situacao";
        }

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros,
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

            DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

            int.TryParse(Request.Params("Comprador"), out int comprador);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador aux = new Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador();

                if (dataInicial != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", dataInicial.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

                if (dataFinal != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", dataFinal.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));

                if (comprador > 0)
                {
                    Dominio.Entidades.Usuario _operador = repUsuario.BuscarPorCodigo(comprador);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Comprador", _operador.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Comprador", false));
            }
            #endregion

            SetarPropriedadeOrdenacao(ref propOrdena);
            // TODO: ToList cast
            reportResult = repRequisicaoMercadoria.RelatorioPontuacaoComprador(codigoEmpresa, comprador, dataInicial, dataFinal, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repRequisicaoMercadoria.ContarRelatorioPontuacaoComprador(codigoEmpresa, comprador, dataInicial, dataFinal);
        }
    }
}
