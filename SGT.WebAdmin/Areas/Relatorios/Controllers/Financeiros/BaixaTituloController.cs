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
	[CustomAuthorize("Relatorios/Financeiros/BaixaTitulo")]
    public class BaixaTituloController : BaseController
    {
		#region Construtores

		public BaixaTituloController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R171_BaixaTitulo;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo").Nome("Código").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("DataBaixa").Nome("Data Baixa").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).OrdAgr(true, true);
            grid.Prop("CPFCNPJPessoaFormatado").Nome("CNPJ/CPF Pessoa").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Ord(false);
            grid.Prop("RazaoPessoa").Nome("Razão Pessoa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("NumeroDocumentoOriginal").Nome("Num. Doc.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoDocumentoOriginal").Nome("Tipo Doc.").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ValorOriginal").Nome("Valor Original").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("ValorBaixa").Nome("Valor Baixa").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("ValorAcrescimoBaixa").Nome("Vlr. Acrésc.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("ValorDescontoBaixa").Nome("Vlr. Desc.").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);

            grid.Prop("PlanoCredito").Nome("Plano Crédito").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("DescricaoPlanoCredito").Nome("Desc. Plano Crédito").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PlanoDebito").Nome("Plano Débito").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("DescricaoPlanoDebito").Nome("Desc. Plano Débito").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PlanoConta").Nome("Plano Baixa").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("DescricaoPlanoConta").Nome("Desc. Plano Baixa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Observacao").Nome("Observação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Baixa de Títulos", "Financeiros", "BaixaTitulo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BaixaTitulo> listaReport = null;
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
                                 DataBaixa = o.DataBaixa.ToString("dd/MM/yyyy"),
                                 o.CPFCNPJPessoaFormatado,
                                 o.RazaoPessoa,
                                 o.NumeroDocumentoOriginal,
                                 o.TipoDocumentoOriginal,
                                 o.ValorOriginal,
                                 o.ValorBaixa,
                                 o.ValorAcrescimoBaixa,
                                 o.ValorDescontoBaixa,
                                 o.PlanoCredito,
                                 o.DescricaoPlanoCredito,
                                 o.PlanoDebito,
                                 o.DescricaoPlanoDebito,
                                 o.PlanoConta,
                                 o.DescricaoPlanoConta,
                                 o.Observacao
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

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BaixaTitulo> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Financeiros/BaixaTitulo", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/BaixaTitulo", parametros, relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BaixaTitulo> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            int codigoEmpresa = this.Usuario.Empresa.Codigo;
            int.TryParse(Request.Params("TipoPagamentoRecebimento"), out int tipoPagamentoRecebimento);

            Enum.TryParse(Request.Params("TipoTitulo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipo);

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);

                if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinal != DateTime.MinValue ? "até " + dataFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

                if ((int)tipo > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTituloHelper.ObterDescricao(tipo), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", false));

                if (tipoPagamentoRecebimento > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento _tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(tipoPagamentoRecebimento);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPagamentoRecebimento", _tipoPagamentoRecebimento.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPagamentoRecebimento", false));

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
            // TODO: ToList cast
            reportResult = repTituloBaixa.ConsultarRelatorioBaixaTitulo(codigoEmpresa, dataInicial, dataFinal, tipoPagamentoRecebimento, tipo, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repTituloBaixa.ContarConsultaRelatorioBaixaTitulo(codigoEmpresa, dataInicial, dataFinal, tipoPagamentoRecebimento, tipo, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);
        }
    }
}
