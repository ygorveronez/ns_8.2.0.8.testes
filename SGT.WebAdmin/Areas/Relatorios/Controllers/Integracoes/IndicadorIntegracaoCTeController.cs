using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;
using Dominio.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Integracoes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Integracoes/IndicadorIntegracaoCTe")]
    public class IndicadorIntegracaoCTeController : BaseController
    {
		#region Construtores

		public IndicadorIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R216_IndicadorIntegracaoCTe;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pagamentos/Escrituração de CT-e", "Integracoes", "IndicadorIntegracaoCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarIntegradoras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dictionary<int, Dominio.Entidades.WebService.Integradora> integradoras = ObterIntegradoras(unitOfWork);

                var integradorasRetornar = (
                    from o in integradoras
                    select new { Numero = o.Key, o.Value.Descricao }
                );

                return new JsonpResult(integradorasRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as integradoras.");
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
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorio = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorio(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Integracao.IndicadorIntegracaoCTe> listaIndicadorIntegracaoCTe = totalRegistros > 0 ? repositorio.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Integracao.IndicadorIntegracaoCTe>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaIndicadorIntegracaoCTe);

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
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
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

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTeo = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Integracao.IndicadorIntegracaoCTe> dataSourceIndicadorIntegracaoCTe = repositorioIndicadorIntegracaoCTeo.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta);
                List<Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Integracoes/IndicadorIntegracaoCTe", parametros,relatorioControleGeracao, relatorioTemporario, dataSourceIndicadorIntegracaoCTe, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dictionary<int, Dominio.Entidades.WebService.Integradora> integradoras = ObterIntegradoras(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataEmissaoInicio = Request.GetNullableDateTimeParam("DataEmissaoInicio"),
                DataEmissaoLimite = Request.GetNullableDateTimeParam("DataEmissaoLimite"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroCTe = Request.GetIntParam("NumeroCTe")
            };

            foreach (KeyValuePair<int, Dominio.Entidades.WebService.Integradora> integradora in integradoras.OrderBy(o => o.Key))
            {
                filtrosPesquisa.GetType().GetProperty($"CodigoIntegradora{integradora.Key}")?.SetValue(filtrosPesquisa, integradora.Value.Codigo);
                filtrosPesquisa.GetType().GetProperty($"DescricaoIntegradora{integradora.Key}")?.SetValue(filtrosPesquisa, integradora.Value.Descricao);
                filtrosPesquisa.GetType().GetProperty($"Integrado{integradora.Key}")?.SetValue(filtrosPesquisa, Request.GetEnumParam($"Integrado{integradora.Key}", OpcaoSimNaoPesquisa.Todos));
            }

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Dictionary<int, Dominio.Entidades.WebService.Integradora> integradoras = ObterIntegradoras(unitOfWork);
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Número CT-e", "NumeroCTe", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Série CT-e", "SerieCTe", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão CT-e", "DataEmissaoCTe", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Chave CT-e", "ChaveCTe", 20, Models.Grid.Align.center, false, false, false, false, false).UtilizarFormatoTexto(true);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Documento", "TipoDocumentoDescripcao", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Status do Documento", "StatusDocumentosDescripcao", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor de Frete Sem Imposto", "ValorFreteSemImposto", 20, Models.Grid.Align.left, false, false, false, false, false);


            foreach (KeyValuePair<int, Dominio.Entidades.WebService.Integradora> integradora in integradoras.OrderBy(o => o.Key))
            {
                grid.AdicionarCabecalho($"Integrado {integradora.Value.Descricao}", $"Integrado{integradora.Key}", 10, Models.Grid.Align.center);
                grid.AdicionarCabecalho($"Data Integração {integradora.Value.Descricao}", $"DataIntegracao{integradora.Key}", 12, Models.Grid.Align.center);
            }

            return grid;
        }

        private Dictionary<int, Dominio.Entidades.WebService.Integradora> ObterIntegradoras(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork);
            List<Dominio.Entidades.WebService.Integradora> integradoras = repositorioIntegradora.BuscarPorIndicarIntegracao();
            Dictionary<int, Dominio.Entidades.WebService.Integradora> integradorasRetornar = new Dictionary<int, Dominio.Entidades.WebService.Integradora>();

            for (int i = 0; i < Math.Min(integradoras.Count, 5); i++)
                integradorasRetornar.Add(i + 1, integradoras[i]);

            return integradorasRetornar;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa)
        {
            List<Parametro> parametros = new List<Parametro>();
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Empresa transportador = (filtrosPesquisa.CodigoTransportador > 0) ? repositorioTransportador.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = (filtrosPesquisa.CodigoFilial > 0) ? repositorioFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;

            parametros.Add(new Parametro("PeriodoEmissao", filtrosPesquisa.DataEmissaoInicio, filtrosPesquisa.DataEmissaoLimite));
            parametros.Add(new Parametro("CodigoCargaEmbarcador", filtrosPesquisa.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("Transportador", transportador?.Descricao));

            for (var i = 1; i <= 5; i++)
            {
                OpcaoSimNaoPesquisa integrado = (OpcaoSimNaoPesquisa)filtrosPesquisa.GetType().GetProperty($"Integrado{i}")?.GetValue(filtrosPesquisa);
                string descricaoIntegradora = (string)filtrosPesquisa.GetType().GetProperty($"DescricaoIntegradora{i}")?.GetValue(filtrosPesquisa);

                parametros.Add(new Parametro() { NomeParametro = $"Integrado{i}", IDParametro = $"Integrado{i}Par", IDTextoParametro = $"Integrado{i}Text", ValorParametro = integrado.ObterDescricao(), Visivel = (integrado != OpcaoSimNaoPesquisa.Todos) });
                parametros.Add(new Parametro() { NomeParametro = $"Integradora{i}", ValorParametro = descricaoIntegradora ?? "" });
            }

            return parametros;
        }

        #endregion
    }
}
