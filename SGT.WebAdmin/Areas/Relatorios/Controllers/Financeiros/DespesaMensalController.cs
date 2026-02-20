using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/DespesaMensal")]
    public class DespesaMensalController : BaseController
    {
		#region Construtores

		public DespesaMensalController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R180_DespesaMensal;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Pessoa").Nome("Pessoa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Descricao").Nome("Descrição").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("DiaProvisao").Nome("Dia Provisão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("ValorProvisao").Nome("Valor Provisão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("TipoDespesa").Nome("Tipo Despesa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("DataGeracao").Nome("Data Geração").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("CodigoTitulo").Nome("Código Título").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("ValorTitulo").Nome("Valor Título").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("DataPagamento").Nome("Data Pag.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Despesas Mensais", "Financeiros", "DespesaMensal.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DespesaMensal> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (from o in listaReport
                             select new
                             {
                                 o.Codigo,
                                 o.Pessoa,
                                 o.Descricao,
                                 o.DiaProvisao,
                                 o.ValorProvisao,
                                 o.TipoDespesa,
                                 DataGeracao = o.DataGeracao.ToString("dd/MM/yyyy"),
                                 o.CodigoTitulo,
                                 o.ValorTitulo,
                                 DataPagamento = o.DataPagamento.ToString("dd/MM/yyyy")
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

                GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, cancellationToken);

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
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DespesaMensal> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Financeiros/DespesaMensal", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/DespesaMensal", parametros, relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DespesaMensal> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal repDespesaMensal = new Repositorio.Embarcador.Financeiro.DespesaMensal.DespesaMensal(unitOfWork);

            DateTime.TryParseExact(Request.Params("DataInicialGeracao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialGeracao);
            DateTime.TryParseExact(Request.Params("DataFinalGeracao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinalGeracao);
            DateTime.TryParseExact(Request.Params("DataInicialPagamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialPagamento);
            DateTime.TryParseExact(Request.Params("DataFinalPagamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinalPagamento);

            int codigoEmpresa = 0;
            int.TryParse(Request.Params("TipoDespesa"), out int tipoDespesa);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            double.TryParse(Request.Params("Pessoa"), out double pessoa);

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

                if (dataInicialGeracao != DateTime.MinValue || dataFinalGeracao != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicialGeracao != DateTime.MinValue ? dataInicialGeracao.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinalGeracao != DateTime.MinValue ? "até " + dataFinalGeracao.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataGeracao", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataGeracao", false));

                if (dataInicialPagamento != DateTime.MinValue || dataFinalPagamento != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicialPagamento != DateTime.MinValue ? dataInicialPagamento.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinalPagamento != DateTime.MinValue ? "até " + dataFinalPagamento.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", false));

                if (pessoa > 0)
                {
                    Dominio.Entidades.Cliente _pessoa = repPessoa.BuscarPorCPFCNPJ(pessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", _pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (tipoDespesa > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira _tipoDespesa = repTipoDespesaFinanceira.BuscarPorCodigo(tipoDespesa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDespesa", _tipoDespesa.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDespesa", false));

                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                if (!string.IsNullOrWhiteSpace(propAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", propAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));
            }
            #endregion
            // TODO: ToList Cast
            reportResult = repDespesaMensal.ConsultarRelatorioDespesaMensal(codigoEmpresa, dataInicialGeracao, dataFinalGeracao, dataInicialPagamento, dataFinalPagamento, tipoDespesa, pessoa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repDespesaMensal.ContarConsultaRelatorioDespesaMensal(codigoEmpresa, dataInicialGeracao, dataFinalGeracao, dataInicialPagamento, dataFinalPagamento, tipoDespesa, pessoa, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);
        }
    }
}
