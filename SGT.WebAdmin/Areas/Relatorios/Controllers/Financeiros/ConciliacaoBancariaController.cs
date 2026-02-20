using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/ConciliacaoBancaria")]
    public class ConciliacaoBancariaController : BaseController
    {
		#region Construtores

		public ConciliacaoBancariaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R270_ConciliacaoBancaria;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Código", "Codigo", 5, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Data", "DescricaoData", 8, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Nº Doc.", "Documento", 5, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Débito", "ValorDebito", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Crédito", "ValorCredito", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Código Plano Conta", "CodigoPlanoConta", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Plano", "PlanoConta", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Descrição Plano", "PlanoContaDescricao", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Movimento", "TipoMovimento", 15, Models.Grid.Align.center, true, false, false, true, true);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Conciliação Bancárria", "Financeiros", "ConciliacaoBancariaPendente.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "TipoMovimento", "asc", Codigo, unitOfWork, false, false);
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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConciliacaoBancaria filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                }

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConciliacaoBancaria> listaConciliacaoBancaria = repExtratoBancario.RelatorioConciliacaoBancaria(filtrosPesquisa, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repExtratoBancario.ContarRelatorioConciliacaoBancaria(filtrosPesquisa));

                grid.AdicionaRows(listaConciliacaoBancaria);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConciliacaoBancaria filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                }

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
                _ = Task.Factory.StartNew(() => GerarRelatorioConciliacaoBancaria(filtrosPesquisa, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        private async Task GerarRelatorioConciliacaoBancaria(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConciliacaoBancaria filtrosPesquisa, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConciliacaoBancaria> listaConciliacaoBancaria = repExtratoBancario.RelatorioConciliacaoBancaria(filtrosPesquisa, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaConciliacaoBancaria, unitOfWork, identificacaoCamposRPT);
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repositorioTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = filtrosPesquisa.CodigoTipoMovimento > 0 ? repositorioTipoMovimento.BuscarPorCodigo(filtrosPesquisa.CodigoTipoMovimento) : null;

                if (filtrosPesquisa.CodigoPlano.Count() > 0)
                {
                    foreach (int codigoPlano in filtrosPesquisa.CodigoPlano)
                    {
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoConta", repPlanoConta.BuscarPorCodigo(codigoPlano).Descricao + " (" + codigoPlano + ") ", true));
                    }

                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoConta", false));

                if (filtrosPesquisa.DataInicial > DateTime.MinValue)
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyy"), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

                if (filtrosPesquisa.DataFinal > DateTime.MinValue)
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyy"), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DebitoCredito", filtrosPesquisa.DebitoCredito.ObterDescricao()));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Observacao", filtrosPesquisa.Observacao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", tipoMovimento?.Descricao ?? string.Empty));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroDocumento", filtrosPesquisa.NumeroDocumento));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoMovimento", filtrosPesquisa.CodigoMovimento > 0 ? filtrosPesquisa.CodigoMovimento.ToString() : string.Empty));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Financeiros/ConciliacaoBancaria", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/ConciliacaoBancaria", parametros, relatorioControleGeracao, relatorioTemp, listaConciliacaoBancaria, unitOfWork, identificacaoCamposRPT);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConciliacaoBancaria ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConciliacaoBancaria()
            {
                CodigoPlano = Request.GetListParam<int>("Plano"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DebitoCredito = Request.GetEnumParam<DebitoCredito>("DebitoCredito"),
                Observacao = Request.GetStringParam("Observacao"),
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                CodigoMovimento = Request.GetIntParam("CodigoMovimento")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoData")
                propriedadeOrdenar = "Data";
            return propriedadeOrdenar;
        }
    }
}
