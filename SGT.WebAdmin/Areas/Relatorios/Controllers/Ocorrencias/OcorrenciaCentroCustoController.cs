using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Ocorrencias
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Ocorrencias/OcorrenciaCentroCusto")]
    public class OcorrenciaCentroCustoController : BaseController
    {
		#region Construtores

		public OcorrenciaCentroCustoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R198_OcorrenciaCentroCusto;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Ocorrências por Centro de Custo", "Ocorrencias", "OcorrenciaCentroCusto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "NumeroOcorrencia", "desc", "", "", Codigo, unitOfWork, true, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorio = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorioPorCentroCusto(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaCentroCusto> listaOcorrencias = (totalRegistros > 0) ? repositorio.ConsultarRelatorioPorCentroCusto(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaCentroCusto>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaOcorrencias);

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
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaCentroCusto> dataSourceOcorrencia = repositorioOcorrencia.ConsultarRelatorioPorCentroCusto(filtrosPesquisa, propriedades, parametrosConsulta);
                ReportRequest.WithType(ReportType.Dinamic)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DataSource", dataSourceOcorrencia.ToJson())
                    .AddExtraData("ControleGeracao", relatorioControleGeracao)
                    .AddExtraData("RelatorioTemporario", relatorioTemporario)
                    .CallReport();
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataOcorrenciaInicial = Request.GetNullableDateTimeParam("DataOcorrenciaInicial"),
                DataOcorrenciaLimite = Request.GetNullableDateTimeParam("DataOcorrenciaLimite"),
                NumeroOcorrencia = Request.GetIntParam("NumeroOcorrencia")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Ocorrência", "NumeroOcorrencia", 6, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 6, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Transportador", "CnpjTransportadorFormatado", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "RazaoSocialTransportador", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CT-e", "NumeroCte", 6, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor CT-e", "ValorReceberCte", 6, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Código Centro de Custo", "CodigoIntegracaoCentroCusto", 6, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("CPF/CNPJ Centro de Custo", "CpfCnpjCentroCustoFormatado", 8, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Centro de Custo", "DescricaoCentroCusto", 10, Models.Grid.Align.left, true, false, false, false, true);

            return grid;
        }

        private List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoOcorrencia", filtrosPesquisa.DataOcorrenciaInicial, filtrosPesquisa.DataOcorrenciaLimite));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrencia", filtrosPesquisa.NumeroOcorrencia));

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", carga.CodigoCargaEmbarcador));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", false));

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}
