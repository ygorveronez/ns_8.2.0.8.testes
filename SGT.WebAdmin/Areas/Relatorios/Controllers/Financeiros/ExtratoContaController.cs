using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/ExtratoConta")]
    public class ExtratoContaController : BaseController
    {
		#region Construtores

		public ExtratoContaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R014_ExtratoConta;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Extrato de Conta", "Financeiros", "ExtratoConta.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Data", "asc", "Plano", "asc", Codigo, unitOfWork, false, false);
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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.ExtratoConta servicoRelatorioExtratoConta = new Servicos.Embarcador.Relatorios.Financeiros.ExtratoConta(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioExtratoConta.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoConta> listaExtratoConta, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaExtratoConta);

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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

            grid.AdicionarCabecalho("Código", "Codigo", 5, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Data", "DataFormatada", 8, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Base", "DataBaseFormatada", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Documento", "DescricaoTipoDocumento", 8, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Nº Doc.", "NumeroDocumento", 5, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Centro Result./Custo", "CentroResultado", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Plano", "Plano", 10, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Descrição Plano", "PlanoDescricao", 15, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Contra Partida", "PlanoContraPartida", 10, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Descrição Contra Partida", "PlanoDescricaoContraPartida", 15, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Entrada", "ValorDebito", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Saída", "ValorCredito", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Saldo", "Saldo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Saldo Anterior", "SaldoAnterior", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Código Plano Conta", "CodigoPlanoConta", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoFavorecido", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Pessoa", "PessoaFavorecido", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Pessoa Título", "PessoaTitulo", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº Doc. Título", "NumeroDocumentoTitulo", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo Doc. Título", "DocumentoTitulo", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Acréscimo Título", "AcrescimoTitulo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Desconto Título", "DescontoTitulo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Vlr. Pago Título", "ValorPagoTitulo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Data Vencimento titulo", "DataVencimentoTituloFormatada", 8, Models.Grid.Align.right, false, false);


            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
            {
                grid.AdicionarCabecalho("Moeda", "MoedaCotacaoBancoCentralFormatada", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Valor Moeda", "ValorMoedaCotacao", 8, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Entrada Moeda", "ValorDebitoMoedaEstrangeira", 8, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Saída Moeda", "ValorCreditoMoedaEstrangeira", 8, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Saldo Moeda", "SaldoMoedaEstrangeira", 8, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Saldo Anterior Moeda", "SaldoAnteriorMoedaEstrangeira", 8, Models.Grid.Align.right, false, false);
            }

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataBaseInicial = Request.GetDateTimeParam("DataBaseInicial"),
                DataBaseFinal = Request.GetDateTimeParam("DataBaseFinal"),
                CodigosPlanoContaAnalitica = Request.GetListParam<int>("Plano"),
                CodigoMovimento = Request.GetIntParam("Codigo"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                CodigoColaborador = Request.GetIntParam("Colaborador"),
                CodigosGrupoPessoa = Request.GetListParam<int>("GrupoPessoa"),
                CnpjPessoa = Request.GetListParam<double>("Pessoa"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                TipoDebitoCredito = Request.GetEnumParam<DebitoCredito>("TipoDebitoCredito"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0,
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                CodigoPlanoContaSintetica = Request.GetIntParam("PlanoContaSintetica"),
                CodigoPlanoContaContrapartida = Request.GetIntParam("PlanoContaContrapartida"),
                MoedaCotacaoBancoCentral = Request.GetEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral"),
            };

            if (this.Usuario != null && this.Usuario.PlanoConta != null)
            {
                filtrosPesquisa.CodigosPlanoContaAnalitica = new List<int>() { this.Usuario.PlanoConta.Codigo };
                filtrosPesquisa.CodigoPlanoContaSintetica = 0;
            }

            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            if (filtrosPesquisa.CodigoCentroResultado > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centro = repCentroResultado.BuscarPorCodigo(filtrosPesquisa.CodigoCentroResultado);
                filtrosPesquisa.CentroResultado = centro.Plano;
                filtrosPesquisa.CentroResultadoPai = centro.AnaliticoSintetico == AnaliticoSintetico.Sintetico;
            }

            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            if (filtrosPesquisa.CodigosPlanoContaAnalitica.Count > 0)
                filtrosPesquisa.PlanosContaAnalitica = repPlanoConta.BuscarPlanosPorCodigos(filtrosPesquisa.CodigosPlanoContaAnalitica);
            if (filtrosPesquisa.CodigoPlanoContaSintetica > 0)
                filtrosPesquisa.PlanoContaSintetica = repPlanoConta.BuscarPlanoPorCodigo(filtrosPesquisa.CodigoPlanoContaSintetica);

            return filtrosPesquisa;
        }

        #endregion
    }
}
