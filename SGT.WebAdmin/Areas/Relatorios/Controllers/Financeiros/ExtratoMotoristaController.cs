using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Servicos.Extensions;
using Utilidades.Extensions;
using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/ExtratoMotorista")]
    public class ExtratoMotoristaController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista>
    {
		#region Construtores

		public ExtratoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

		Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R040_ExtratoMotorista;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Extrato por Motorista", "Financeiros", "ExtratoMotorista.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "", "", "", "", codigoRelatorio, unitOfWork, false, false);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.ExtratoMotorista servicoRelatorioExtratoMotorista = new Servicos.Embarcador.Relatorios.Financeiros.ExtratoMotorista(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioExtratoMotorista.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista> lista, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorioMovimento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int codigoPlano = 0, codigo = 0;
                int.TryParse(Request.Params("Plano"), out codigoPlano);
                int.TryParse(Request.Params("Codigo"), out codigo);

                int codigoColaborador = 0;
                int.TryParse(Request.Params("Motorista"), out codigoColaborador);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista> listaExtratoMotorista = repMovimentoFinanceiro.RelatorioExtratoMotorista(false, 0, this.Usuario.Empresa.Codigo, codigoPlano, dataInicial, dataFinal, "", "", "", "", 0, 0, false, false, codigoColaborador);
                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista> relatorioMovimentosMotorista = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista>();
                for (int i = 0; i < listaExtratoMotorista.Count(); i++)
                {
                    Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista dado = new Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista();
                    dado.Codigo = listaExtratoMotorista[i].Codigo;
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(listaExtratoMotorista[i].CodigoMotorista);

                    if (motorista.Banco != null)
                        dado.Banco = motorista.Banco.Descricao + " (" + motorista.Banco.Numero + ")";
                    else
                        dado.Banco = "";
                    dado.DigitoAgencia = motorista.DigitoAgencia;
                    dado.Nome = motorista.Nome;
                    dado.CodigoIntegracao = motorista.CodigoIntegracao;
                    dado.CPF = motorista.CPF_Formatado;
                    dado.NumeroAgencia = motorista.Agencia;
                    dado.NumeroConta = motorista.NumeroConta;

                    if (listaExtratoMotorista[i].Entrada > 0)
                        dado.Valor = listaExtratoMotorista[i].Entrada;
                    else
                        dado.Valor = listaExtratoMotorista[i].Saida * -1;

                    if (motorista.TipoContaBanco != null)
                        dado.TipoConta = motorista.DescricaoTipoContaBanco;
                    else
                        dado.TipoConta = "";
                    dado.DataMovimentacao = listaExtratoMotorista[i].Data;

                    relatorioMovimentosMotorista.Add(dado);
                }
                if (relatorioMovimentosMotorista != null && relatorioMovimentosMotorista.Count > 0)
                    await GerarRelatorio(relatorioMovimentosMotorista, cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Codigo = Request.GetIntParam("Codigo"),
                CodigoPlanoConta = Request.GetIntParam("Plano"),
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0
            };
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private async Task GerarRelatorio(List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista> relatorioMovimentosMotorista, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R050_Movimento_Motorista, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R050_Movimento_Motorista, TipoServicoMultisoftware, "Relatorio Movimentos ao Motorista", "Financeiros", "MovimentoMotorista.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                _ = Task.Factory.StartNew(() => GerarRelatorioMovimentoMotorista(relatorioMovimentosMotorista, stringConexao, relatorioControleGeracao, CancellationToken.None));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task GerarRelatorioMovimentoMotorista(List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioMovimentosMotorista> relatorioMovimentosMotorista, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                ReportRequest.WithType(ReportType.MovimentacaoMotorista)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("RelatorioMovimentosMotorista", relatorioMovimentosMotorista.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CodigoUsuario", Usuario.Codigo)
                    .CallReport();
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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "Codigo", 5, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Data", "DataFormatada", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Documento", "TipoDocumento", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº Doc.", "NumeroDocumento", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Conta Entrada", "PlanoDebito", 15, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Conta Saída", "PlanoCredito", 15, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Entrada", "Entrada", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Saída", "Saida", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Saldo", "Saldo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Saldo Anterior", "SaldoAnterior", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Tipo Movimento", "TipoMovimento", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Despesa", "Despesa", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Último Acerto", "DataUltimoAcertoFormatada", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 10, Models.Grid.Align.left, false, false);

            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioExtratoMotorista> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
