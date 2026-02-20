using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.FaturamentosMensais
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/FaturamentosMensais/CobrancaMensal")]
    public class CobrancaMensalController : BaseController
    {
		#region Construtores

		public CobrancaMensalController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R082_CobrancaMensal;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Finalização", "DataFinalizacao", 4, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Fatura", "DataFatura", 4, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Dia Fatura", "DiaFatura", 2, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor Fatura", "ValorFatura", 4, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Cód. Título", "CodigoTitulo", 3, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Nº Boleto", "Boleto", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Nº NFe", "Nota", 3, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Nº NFSe", "NotaServico", 3, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo Faturamento", "GrupoFaturamento", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Observação Fatura", "Observacao", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 4, Models.Grid.Align.left, true, false, false, true, true);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Cobranças Mensais", "FaturamentoMensal", "CobrancaMensal.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "DataFatura", "desc", "", "", Codigo, unitOfWork, true, true);
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

                int codigoGrupoFaturamento, codigoServico, dia, codigoConfiguracaoBoleto;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("Dia"), out dia);
                int.TryParse(Request.Params("ConfiguracaoBoleto"), out codigoConfiguracaoBoleto);

                DateTime dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal;
                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params("DataVencimentoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoInicial);
                DateTime.TryParseExact(Request.Params("DataVencimentoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoFinal);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status;
                Enum.TryParse(Request.Params("Status"), out status);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.CobrancaMensal> listaCobrancaMensal = repFaturamentoMensal.RelatorioCobrancaMensal(this.Usuario.Empresa.Codigo, codigoGrupoFaturamento, codigoServico, dia, codigoConfiguracaoBoleto, cnpjPessoa, status, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturamentoMensal.ContarRelatorioCobrancaMensal(this.Usuario.Empresa.Codigo, codigoGrupoFaturamento, codigoServico, dia, codigoConfiguracaoBoleto, cnpjPessoa, status, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal));

                var lista = (from obj in listaCobrancaMensal
                             select new
                             {
                                 DataVencimento = obj.DataVencimento > DateTime.MinValue ? obj.DataVencimento.ToString("dd/MM/yyyy") : string.Empty,
                                 DataFinalizacao = obj.DataFinalizacao > DateTime.MinValue ? obj.DataFinalizacao.ToString("dd/MM/yyyy") : string.Empty,
                                 DataFatura = obj.DataFatura > DateTime.MinValue ? obj.DataFatura.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.DiaFatura,
                                 obj.ValorFatura,
                                 obj.CodigoTitulo,
                                 obj.Boleto,
                                 obj.Nota,
                                 obj.NotaServico,
                                 obj.Pessoa,
                                 obj.GrupoFaturamento,
                                 obj.Observacao,
                                 obj.DescricaoStatus
                             }).ToList();

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
                await unitOfWork.StartAsync(cancellationToken);

                int codigoGrupoFaturamento, codigoServico, dia, codigoConfiguracaoBoleto;
                int.TryParse(Request.Params("GrupoFaturamento"), out codigoGrupoFaturamento);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("Dia"), out dia);
                int.TryParse(Request.Params("ConfiguracaoBoleto"), out codigoConfiguracaoBoleto);

                DateTime dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal;
                DateTime.TryParseExact(Request.Params("DataEmissaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params("DataEmissaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params("DataVencimentoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoInicial);
                DateTime.TryParseExact(Request.Params("DataVencimentoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoFinal);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status;
                Enum.TryParse(Request.Params("Status"), out status);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioCobrancaMensal(codigoGrupoFaturamento, codigoServico, dia, codigoConfiguracaoBoleto, cnpjPessoa, status, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioCobrancaMensal(int codigoGrupoFaturamento, int codigoServico, int dia, int codigoConfiguracaoBoleto, double cnpjPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.CobrancaMensal> listaCobrancaMensal = repFaturamentoMensal.RelatorioCobrancaMensal(this.Usuario.Empresa.Codigo, codigoGrupoFaturamento, codigoServico, dia, codigoConfiguracaoBoleto, cnpjPessoa, status, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                //var lista = (from obj in listaCobrancaMensal
                //             select new
                //             {
                //                 DataVencimento = obj.DataVencimento > DateTime.MinValue ? obj.DataVencimento.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataFinalizacao = obj.DataFinalizacao > DateTime.MinValue ? obj.DataFinalizacao.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataFatura = obj.DataFatura > DateTime.MinValue ? obj.DataFatura.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.DiaFatura,
                //                 obj.ValorFatura,
                //                 obj.CodigoTitulo,
                //                 obj.Boleto,
                //                 obj.Nota,
                //                 obj.NotaServico,
                //                 obj.Pessoa,
                //                 obj.GrupoFaturamento,
                //                 obj.Observacao,
                //                 obj.DescricaoStatus
                //             }).ToList();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, lista, unitOfWork, identificacaoCamposRPT);
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repGrupoFaturamento = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repConfiguracaoBoleto = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);

                if (codigoGrupoFaturamento > 0)
                {
                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo grupoFaturamento = repGrupoFaturamento.BuscarPorCodigo(codigoGrupoFaturamento);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoFaturamento", "(" + grupoFaturamento.Codigo.ToString() + ") " + grupoFaturamento.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoFaturamento", false));

                if (cnpjPessoa > 0)
                {
                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + pessoa.Codigo.ToString() + ") " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (codigoServico > 0)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = repServico.BuscarPorCodigo(codigoServico);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", "(" + servico.Codigo.ToString() + ") " + servico.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", false));

                if (dia > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Dia", dia.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Dia", false));

                if (codigoConfiguracaoBoleto > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao configuracaoBoleto = repConfiguracaoBoleto.BuscarPorCodigo(codigoConfiguracaoBoleto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ConfiguracaoBoleto", "(" + configuracaoBoleto.Codigo.ToString() + ") " + configuracaoBoleto.DescricaoBanco, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ConfiguracaoBoleto", false));

                if (dataEmissaoInicial != DateTime.MinValue || dataEmissaoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataEmissaoInicial != DateTime.MinValue ? dataEmissaoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataEmissaoFinal != DateTime.MinValue ? "até " + dataEmissaoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", false));

                if (dataVencimentoInicial != DateTime.MinValue || dataVencimentoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataVencimentoInicial != DateTime.MinValue ? dataVencimentoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataVencimentoFinal != DateTime.MinValue ? "até " + dataVencimentoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", false));

                if ((int)status > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", status.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", false));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/FaturamentosMensais/CobrancaMensal", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/FaturamentosMensais/CobrancaMensal", parametros, relatorioControleGeracao, relatorioTemp, listaCobrancaMensal, unitOfWork, identificacaoCamposRPT);

                await unitOfWork.DisposeAsync();
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
    }
}
