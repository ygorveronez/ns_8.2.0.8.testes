using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pessoas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pessoas/FuncionarioComissao")]
    public class FuncionarioComissaoController : BaseController
    {
		#region Construtores

		public FuncionarioComissaoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R174_FuncionarioComissao;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo").Visibilidade(false);
            grid.Prop("Numero").Nome("Número").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).OrdAgr(true, true);
            grid.Prop("Funcionario").Nome("Vendedor").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Operador").Nome("Operador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("DataGeracao").Nome("Geração").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).OrdAgr(true, true);
            grid.Prop("DataInicial").Nome("Data Inicial").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("DataFinal").Nome("Data Final").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("ValorTotalFinal").Nome("Valor Total").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("PercentualComissao").Nome("% Comissão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("PercentualComissaoAcrescimo").Nome("+ Comissão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("PercentualComissaoTotal").Nome("% Total").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("ValorComissao").Nome("Valor Comissão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Comissão de Funcionário", "Pessoas", "FuncionarioComissao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissao> listaReport = null;
                List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissaoTitulo> subReportTitulo = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                ExecutarBusca(ref listaReport, ref subReportTitulo, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (from o in listaReport
                             select new
                             {
                                 o.Codigo,
                                 o.Numero,
                                 o.Funcionario,
                                 o.Operador,
                                 DataGeracao = o.DataGeracao.ToString("dd/MM/yyyy"),
                                 DataInicial = o.DataInicial.ToString("dd/MM/yyyy"),
                                 DataFinal = o.DataFinal.ToString("dd/MM/yyyy"),
                                 o.ValorTotalFinal,
                                 o.PercentualComissao,
                                 o.PercentualComissaoAcrescimo,
                                 o.PercentualComissaoTotal,
                                 o.ValorComissao
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

                List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissao> listaReport = null;
                List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissaoTitulo> subReportTitulo = new List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissaoTitulo>();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref subReportTitulo, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Pessoas/FuncionarioComissao",parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("FuncionarioComissaoTitulo", subReportTitulo) }, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissao> reportResult, ref List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissaoTitulo> subReportTitulo, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);

            DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
            int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
            int.TryParse(Request.Params("Funcionario"), out int funcionario);
            int.TryParse(Request.Params("Operador"), out int operador);
            int.TryParse(Request.Params("Titulo"), out int titulo);
            int.TryParse(Request.Params("Fatura"), out int fatura);

            bool.TryParse(Request.Params("ExibirTitulos"), out bool exibirTitulos);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao situacaoAux))
                situacao = situacaoAux;

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinal != DateTime.MinValue ? "até " + dataFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

                if (numeroInicial > 0 || numeroFinal > 0)
                {
                    string numero = "";
                    numero += numeroInicial > 0 ? numeroInicial.ToString("n0") + " " : "";
                    numero += numeroFinal > 0 ? "até " + numeroFinal.ToString("n0") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", numero, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", false));

                if (funcionario > 0)
                {
                    Dominio.Entidades.Usuario _funcionario = repUsuario.BuscarPorCodigo(funcionario);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Funcionario", _funcionario.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Funcionario", false));

                if (operador > 0)
                {
                    Dominio.Entidades.Usuario _operador = repUsuario.BuscarPorCodigo(operador);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", _operador.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", false));

                if (titulo > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo _titulo = repTitulo.BuscarPorCodigo(titulo);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", _titulo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", false));

                if (fatura > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura _fatura = repFatura.BuscarPorCodigo(fatura);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", _fatura.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", false));

                if (exibirTitulos)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirTitulos", "Sim", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirTitulos", false));

                if (situacao.HasValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissaoHelper.ObterDescricao(situacao.Value), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

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
            
            reportResult = repFuncionarioComissao.ConsultarRelatorioFuncionarioComissao(codigoEmpresa, dataInicial, dataFinal, numeroInicial, numeroFinal, funcionario, operador, titulo, fatura, exibirTitulos, situacao, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repFuncionarioComissao.ContarConsultaRelatorioFuncionarioComissao(codigoEmpresa, dataInicial, dataFinal, numeroInicial, numeroFinal, funcionario, operador, titulo, fatura, exibirTitulos, situacao, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);

            if (exibirTitulos)
            {
                List<int> codigosFuncionarioComissao = new List<int>();
                foreach (var funcionarioComissao in reportResult)
                {
                    codigosFuncionarioComissao.Add(funcionarioComissao.Codigo);
                }
                // TODO: ToList cast
                subReportTitulo = repFuncionarioComissao.ConsultarTitulosRelatorioFuncionarioComissao(codigosFuncionarioComissao).ToList();
            }
        }
    }
}
