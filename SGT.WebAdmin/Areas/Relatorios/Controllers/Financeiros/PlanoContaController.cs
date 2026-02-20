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
	[CustomAuthorize("Relatorios/Financeiros/PlanoConta")]
    public class PlanoContaController : BaseController
    {
		#region Construtores

		public PlanoContaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R013_PlanoConta;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Plano", "Plano", 10, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Tipo Conta", "DescricaoAnaliticoSintetico", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Cód. Integração", "PlanoContabilidade", 10, Models.Grid.Align.left, false, true);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Plano de Contas", "Financeiros", "PlanoConta.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Plano", "asc", "", "", Codigo, unitOfWork, false, false);
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

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo;
                Enum.TryParse(Request.Params("Tipo"), out tipo);

                int codigoPlanoSintetico = 0;
                int.TryParse(Request.Params("PlanoSintetico"), out codigoPlanoSintetico);

                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoCOnta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoSintetico = null;
                if (codigoPlanoSintetico > 0)
                    planoSintetico = repPlanoCOnta.BuscarPorCodigo(codigoPlanoSintetico);
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoConta> listaPlanoConta = repPlanoCOnta.RelatorioPlanoConta(codigoEmpresa, planoSintetico != null ? planoSintetico.Plano : "", tipo, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPlanoCOnta.ContarRelatorioPlanoConta(codigoEmpresa, planoSintetico != null ? planoSintetico.Plano : "", tipo));

                grid.AdicionaRows(listaPlanoConta);

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

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo;
                Enum.TryParse(Request.Params("Tipo"), out tipo);

                int codigoPlanoSintetico = 0;
                int.TryParse(Request.Params("PlanoSintetico"), out codigoPlanoSintetico);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

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
                _ = Task.Factory.StartNew(() => GerarRelatorioPlanoConta(codigoEmpresa, codigoPlanoSintetico, tipo, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioPlanoConta(int codigoEmpresa, int codigoContaSintetica, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoSintetico = null;
                if (codigoContaSintetica > 0)
                    planoSintetico = repPlanoConta.BuscarPorCodigo(codigoContaSintetica);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoConta> listaPlanoConta = repPlanoConta.RelatorioPlanoConta(codigoEmpresa, planoSintetico != null ? planoSintetico.Plano : "", tipo, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                //var lista = from obj in listaPlanoConta
                //            select new
                //            {
                //                obj.Codigo,
                //                obj.Plano,
                //                obj.Descricao,
                //                obj.DescricaoAnaliticoSintetico,
                //                obj.PlanoContabilidade
                //            };

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                if (codigoContaSintetica > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = repPlanoConta.BuscarPorCodigo(codigoContaSintetica);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoConta", "(" + planoConta.Plano + ") " + planoConta.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlanoConta", false));

                if ((int)tipo > 0)
                {
                    if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Analitico)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoConta", "ANALITICO", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoConta", "SINTÉTICO", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoConta", "TODOS", true));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/PlanoConta",parametros,relatorioControleGeracao, relatorioTemp, listaPlanoConta, unitOfWork, identificacaoCamposRPT);

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
