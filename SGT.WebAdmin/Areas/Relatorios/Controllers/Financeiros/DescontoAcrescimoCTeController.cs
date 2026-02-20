using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/DescontoAcrescimoCTe")]
    public class DescontoAcrescimoCTeController : BaseController
    {
		#region Construtores

		public DescontoAcrescimoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R043_DescontoAcrescimoCTe;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Número CTe", "NumeroCTe", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "SerieCTe", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Grupo Tomador", "Grupo", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Emissão do CTe", "DataEmissaoCTe", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Recebimento", "DataPagamentoTitulo", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Número Fatura", "NumeroFatura", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Números Títulos", "NumeroTitulo", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor a Receber do CTe", "ValorReceber", 10, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Acréscimo", "ValorAcrescimo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Motivo Acréscimo", "MotivoAcrescimo", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor Desconto", "ValorDesconto", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Motivo Desconto", "MotivoDesconto", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor Pago", "ValorPago", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Status", "StatusTitulo", 8, Models.Grid.Align.center, false, false);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Descontos e Acréscimos Aplicados em Conhecimentos", "Financeiros", "DescontoAcrescimoCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroCTe", "asc", "", "", Codigo, unitOfWork, true, false);
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

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status;
                Enum.TryParse(Request.Params("Status"), out status);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoCTe = 0, codigoFatura = 0, codigoGrupoPessoa = 0, codigoGrupoCTe = 0;
                int.TryParse(Request.Params("ConhecimentoDeTransporte"), out codigoCTe);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                int.TryParse(Request.Params("GrupoPessoaCTe"), out codigoGrupoCTe);

                DateTime dataInicialEmissao, dataFinalEmissao, dataInicialPagamento, dataFinalPagamento;

                DateTime.TryParse(Request.Params("DataInicialEmissao"), out dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out dataFinalEmissao);
                DateTime.TryParse(Request.Params("DataInicialPagamento"), out dataInicialPagamento);
                DateTime.TryParse(Request.Params("DataFinalPagamento"), out dataFinalPagamento);

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoCTe> listaDescontoAcrescimoCTe = repTitulo.RelatorioDescontoAcrescimoCTe(codigoGrupoCTe, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialPagamento, dataFinalPagamento, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarDescontoAcrescimoCTe(codigoGrupoCTe, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialPagamento, dataFinalPagamento));

                var lista = from obj in listaDescontoAcrescimoCTe
                            select new
                            {
                                obj.NumeroCTe,
                                obj.SerieCTe,
                                obj.Tomador,
                                obj.Grupo,
                                DataEmissaoCTe = obj.DataEmissaoCTe != null && obj.DataEmissaoCTe > DateTime.MinValue ? obj.DataEmissaoCTe.ToString("dd/MM/yyyy") : string.Empty,
                                DataPagamentoTitulo = obj.DataPagamentoTitulo != null && obj.DataPagamentoTitulo > DateTime.MinValue ? obj.DataPagamentoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                                obj.NumeroFatura,
                                obj.NumeroTitulo,
                                obj.ValorReceber,
                                obj.ValorAcrescimo,
                                obj.MotivoAcrescimo,
                                obj.ValorDesconto,
                                obj.MotivoDesconto,
                                obj.ValorPago,
                                obj.StatusTitulo
                            };

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

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status;
                Enum.TryParse(Request.Params("Status"), out status);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoCTe = 0, codigoFatura = 0, codigoGrupoPessoa = 0, codigoGrupoCTe = 0;
                int.TryParse(Request.Params("ConhecimentoDeTransporte"), out codigoCTe);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                int.TryParse(Request.Params("GrupoPessoaCTe"), out codigoGrupoCTe);

                DateTime dataInicialEmissao, dataFinalEmissao, dataInicialPagamento, dataFinalPagamento;

                DateTime.TryParse(Request.Params("DataInicialEmissao"), out dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out dataFinalEmissao);
                DateTime.TryParse(Request.Params("DataInicialPagamento"), out dataInicialPagamento);
                DateTime.TryParse(Request.Params("DataFinalPagamento"), out dataFinalPagamento);

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
                _ = Task.Factory.StartNew(() => GerarRelatorioDescontoAcrescimoCTe(codigoGrupoCTe, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialPagamento, dataFinalPagamento, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioDescontoAcrescimoCTe(int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialPagamento, DateTime dataFinalPagamento, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoCTe> listaDescontoAcrescimoCTe = repTitulo.RelatorioDescontoAcrescimoCTe(codigoGrupoCTe, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialPagamento, dataFinalPagamento, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                var lista = from obj in listaDescontoAcrescimoCTe
                            select new
                            {
                                obj.NumeroCTe,
                                obj.SerieCTe,
                                obj.Tomador,
                                obj.Grupo,
                                DataEmissaoCTe = obj.DataEmissaoCTe != null && obj.DataEmissaoCTe > DateTime.MinValue ? obj.DataEmissaoCTe.ToString("dd/MM/yyyy") : string.Empty,
                                DataPagamentoTitulo = obj.DataPagamentoTitulo != null && obj.DataPagamentoTitulo > DateTime.MinValue ? obj.DataPagamentoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                                obj.NumeroFatura,
                                obj.NumeroTitulo,
                                obj.ValorReceber,
                                obj.ValorAcrescimo,
                                obj.MotivoAcrescimo,
                                obj.ValorDesconto,
                                obj.MotivoDesconto,
                                obj.ValorPago,
                                obj.StatusTitulo
                            };

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, lista, unitOfWork, identificacaoCamposRPT);
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", "(" + cte.Numero + ") " + cte.Chave, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", false));

                if (codigoFatura > 0)
                {
                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", "(" + fatura.Codigo + ") " + fatura.Numero, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", false));

                if (codigoGrupoPessoa > 0)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", false));

                if (cnpjPessoa > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if ((int)status > 0)
                {
                    if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Em Aberto", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Quitado", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Todos", true));

                if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy") + " até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Todos", true));

                if (dataInicialPagamento > DateTime.MinValue && dataFinalPagamento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "De " + dataInicialPagamento.ToString("dd/MM/yyyy") + " até " + dataFinalPagamento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialPagamento > DateTime.MinValue && dataFinalPagamento == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "De " + dataInicialPagamento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialPagamento == DateTime.MinValue && dataFinalPagamento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "Até " + dataFinalPagamento.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "Todos", true));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                if (codigoGrupoCTe > 0)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(codigoGrupoCTe);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoaCTe", grupo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoaCTe", false));

                // serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);
                // serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Financeiros/DescontoAcrescimoCTe", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/DescontoAcrescimoCTe", parametros, relatorioControleGeracao, relatorioTemp, lista, unitOfWork, identificacaoCamposRPT);

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
