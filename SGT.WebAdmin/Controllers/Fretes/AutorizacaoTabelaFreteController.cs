using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "RegrasAprovacao", "PesquisaObservacoesTabelaFreteCliente" }, "Fretes/AutorizacaoTabelaFrete", "Fretes/VigenciaTabelaFreteAnexo")]
    public class AutorizacaoTabelaFreteController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete,
        Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete,
        Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao
    >
    {
        #region Construtores

        public AutorizacaoTabelaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFreteAlteracao = Request.GetIntParam("Codigo");
                List<int> codigosTabelaFreteAlteracao = Request.GetListParam<int>("Codigos");
                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete;

                if (codigosTabelaFreteAlteracao.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> tabelasFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigos(codigosTabelaFreteAlteracao);
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = tabelasFreteAlteracao.Select(alteracao => alteracao.TabelaFrete).Distinct().ToList();

                    if (tabelasFrete.Count > 1)
                        throw new ControllerException("Não é possível selecionar tabelas de frete diferentes.");

                    tabelaFrete = tabelasFrete.FirstOrDefault();
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigo(codigoTabelaFreteAlteracao, auditavel: false);
                    tabelaFrete = tabelaFreteAlteracao?.TabelaFrete;
                }

                if (tabelaFrete == null)
                    throw new ControllerException("Tabela de frete não encontrada.");

                Models.Grid.Grid grid = ObterGridTabelaFreteClientePendenteAprovacao(tabelaFrete);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = new Dominio.Entidades.Embarcador.Relatorios.Relatorio() { Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>() };
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = new Models.Grid.Relatorio().RetornoGridPadraoRelatorio(grid, relatorio);

                return new JsonpResult(retorno);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados dos valores alterados da tabela de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCodigosTabelaFreteAlteracaoSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosTabelaFreteAlteracao = ObterCodigosOrigensSelecionadas(unitOfWork);

                return new JsonpResult(codigosTabelaFreteAlteracao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os registros selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaValoresAlteradosTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                int codigoTabelaFreteAlteracao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = codigoTabelaFreteAlteracao > 0 ? repositorioTabelaFreteAlteracao.BuscarPorCodigo(codigoTabelaFreteAlteracao, auditavel: false) : null;

                if ((tabelaFreteAlteracao == null) || (tabelaFreteAlteracao.SituacaoAlteracao == SituacaoAlteracaoTabelaFrete.Aprovada))
                {
                    grid.AdicionaRows(new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>());
                    grid.setarQuantidadeTotal(0);

                    return new JsonpResult(grid);
                }

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();
                List<int> codigosTabelasFreteCliente = new List<int>();

                if (configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(unitOfWork);
                    codigosTabelasFreteCliente = repositorioTabelaFreteClienteAlteracao.BuscarCodigosTabelasFreteClientePorAlteracao(tabelaFreteAlteracao.Codigo);
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = tabelaFreteAlteracao.TabelaFrete.ParametroBase,
                    CodigoTabelaFrete = tabelaFreteAlteracao.TabelaFrete.Codigo,
                    CodigosTabelasFreteCliente = codigosTabelasFreteCliente,
                    TipoRegistro = TipoRegistroAjusteTabelaFrete.Alterados,
                    ExibirSomenteAguardandoAprovacao = true,
                    SomenteRegistrosComValores = configuracaoTabelaFrete.MostrarRegistroSomenteComValoresNaAprovacao
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                int totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = totalRegistros > 0 ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os valores alterados da tabela de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaValoresAlteradosTabelaFreteSelecionadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosTabelaFreteAlteracao = Request.GetListParam<int>("Codigos");
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                if (codigosTabelaFreteAlteracao.Count == 0)
                {
                    grid.AdicionaRows(new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>());
                    grid.setarQuantidadeTotal(0);
                    return new JsonpResult(grid);
                }

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> tabelasFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigos(codigosTabelaFreteAlteracao);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = tabelasFreteAlteracao.Select(alteracao => alteracao.TabelaFrete).Distinct().ToList();

                if (tabelasFrete.Count > 1)
                    throw new ControllerException("Não é possível selecionar tabelas de frete diferentes");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                List<int> codigosTabelasFreteCliente = repositorioTabelaFreteClienteAlteracao.BuscarCodigosTabelasFreteClientePorAlteracoes(codigosTabelaFreteAlteracao);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = tabelasFrete.FirstOrDefault().ParametroBase,
                    CodigosTabelasFreteCliente = codigosTabelasFreteCliente,
                    TipoRegistro = TipoRegistroAjusteTabelaFrete.Alterados,
                    ExibirSomenteAguardandoAprovacao = false,
                    SomenteRegistrosComValores = configuracaoTabelaFrete.MostrarRegistroSomenteComValoresNaAprovacao
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                int totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = totalRegistros > 0 ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os valores alterados da tabela de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaObservacoesTabelaFreteCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente();
            filtrosPesquisa.CodigoTabelaFrete = Request.GetIntParam("Codigo");
            filtrosPesquisa.SituacaoTabelaFrete = SituacaoAtivoPesquisa.Ativo;

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação", "ObservacaoInterna", 30, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                int codigoTabelaFreteCliente = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                int totalRegistros = repTabelaFreteCliente.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> listaTabelaFreteCliente = totalRegistros > 0 ? repTabelaFreteCliente.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                var listaTabelaFreteClienteRetornar = (
                    from obj in listaTabelaFreteCliente
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao,
                        obj.ObservacaoInterna
                    }
                ).ToList();

                grid.AdicionaRows(listaTabelaFreteClienteRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarPlanilhaExcel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFreteAlteracao = Request.GetIntParam("Codigo");
                List<string> colunasVisiveis = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>((string)Request.Params("Colunas"));
                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigo(codigoTabelaFreteAlteracao, false);

                if (tabelaFreteAlteracao == null)
                    return new JsonpResult(false, "Nenhum registro encontrado.");

                Models.Grid.Grid grid = ObterGridTabelaFreteClientePendenteAprovacao(tabelaFreteAlteracao.TabelaFrete);

                foreach (Models.Grid.Head colunaExistente in grid.header)
                    colunaExistente.visible = colunasVisiveis.Contains(colunaExistente.name);

                grid.tituloExportacao = "Valores Alterados";
                grid.group = new Models.Grid.Group();

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();
                List<int> codigosTabelasFreteCliente = new List<int>();

                if (configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(unitOfWork);
                    codigosTabelasFreteCliente = repositorioTabelaFreteClienteAlteracao.BuscarCodigosTabelasFreteClientePorAlteracao(tabelaFreteAlteracao.Codigo);
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = tabelaFreteAlteracao.TabelaFrete.ParametroBase,
                    CodigoTabelaFrete = tabelaFreteAlteracao.TabelaFrete.Codigo,
                    CodigosTabelasFreteCliente = codigosTabelasFreteCliente,
                    TipoRegistro = TipoRegistroAjusteTabelaFrete.Alterados,
                    ExibirSomenteAguardandoAprovacao = true,
                    SomenteRegistrosComValores = configuracaoTabelaFrete.MostrarRegistroSomenteComValoresNaAprovacao
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                // TODO: ToList Cast
                List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> retorno = repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta).ToList();

                retorno.ForEach(x => x.SetGerandoExcel(true));

                grid.AdicionaRows(retorno);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarPlanilhaExcelTodasTabelas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosTabelaFreteAlteracao = Request.GetListParam<int>("Codigos");
                List<string> colunasVisiveis = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>((string)Request.Params("Colunas"));
                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> tabelasFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigos(codigosTabelaFreteAlteracao);

                if (tabelasFreteAlteracao.Count == 0)
                    return new JsonpResult(false, "Nenhum registro encontrado.");

                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = tabelasFreteAlteracao.Select(alteracao => alteracao.TabelaFrete).Distinct().ToList();

                if (tabelasFrete.Count > 1)
                    throw new ControllerException("Não é possível selecionar tabelas de frete diferentes");

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete.FirstOrDefault();
                Models.Grid.Grid grid = ObterGridTabelaFreteClientePendenteAprovacao(tabelaFrete);

                foreach (Models.Grid.Head colunaExistente in grid.header)
                    colunaExistente.visible = colunasVisiveis.Contains(colunaExistente.name) ? true : false;

                grid.tituloExportacao = "Valores Alterados";
                grid.group = new Models.Grid.Group();

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                List<int> codigosTabelasFreteCliente = repositorioTabelaFreteClienteAlteracao.BuscarCodigosTabelasFreteClientePorAlteracoes(codigosTabelaFreteAlteracao);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = tabelaFrete.ParametroBase,
                    CodigosTabelasFreteCliente = codigosTabelasFreteCliente,
                    TipoRegistro = TipoRegistroAjusteTabelaFrete.Alterados,
                    ExibirSomenteAguardandoAprovacao = false,
                    SomenteRegistrosComValores = configuracaoTabelaFrete.MostrarRegistroSomenteComValoresNaAprovacao
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                // TODO: ToList Cast
                List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> retorno = repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta).ToList();

                retorno.ForEach(x => x.SetGerandoExcel(true));

                grid.AdicionaRows(retorno);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFreteAlteracao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigo(codigoTabelaFreteAlteracao, auditavel: false);

                if (tabelaFreteAlteracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tabelaFreteAlteracao.Codigo,
                    CodigoTabelaFrete = tabelaFreteAlteracao.TabelaFrete.Codigo,
                    tabelaFreteAlteracao.TabelaFrete.Descricao,
                    Situacao = tabelaFreteAlteracao.SituacaoAlteracao,
                    SituacaoDescricao = tabelaFreteAlteracao.SituacaoAlteracao.ObterDescricaoPorTabelaFrete()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao ObterFiltrosPesquisa(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao()
            {
                CodigoTabelaFrete = Request.GetIntParam("Tabela"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                SituacaoAlteracao = Request.GetNullableEnumParam<SituacaoAlteracaoTabelaFrete>("Situacao")
            };

            if (configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    filtrosPesquisa.CodigoUsuario = Usuario.Codigo;
                    filtrosPesquisa.TipoAprovadorRegra = TipoAprovadorRegra.Transportador;
                }
                else
                    filtrosPesquisa.TipoAprovadorRegra = TipoAprovadorRegra.Usuario;
            }

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridTabelaFreteClientePendenteAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            int NumeroMaximoComplementos = 15;
            int UltimaColunaDinamica = 1;
            decimal TamanhoColunasValores = 1.75m;
            decimal TamanhoColunasParticipantes = 5.50m;
            decimal TamanhoColunasEnderecoParticipantes = 3m;

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            bool visibleTMS = true;

            grid.AdicionarCabecalho("Código", "CodigoIntegracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                visibleTMS = false;
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportadorFormatado", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Transportador", "DescricaoEmpresa", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Rota Frete Origem", "RotaFreteOrigem", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Rota Frete Destino", "RotaFrete", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            }

            grid.AdicionarCabecalho("Remetente", "DescricaoRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, visibleTMS);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Estado Origem", "EstadoOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Região Origem", "RegiaoOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CEP Origem", "CEPOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("País Origem", "PaisOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("Destinatário", "DescricaoDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, visibleTMS);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Região Destino", "RegiaoDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Estado Destino", "EstadoDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CEP Destino", "CEPDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("País Destino", "PaisDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            if (tabelaFrete.ParametroBase.HasValue)
                grid.AdicionarCabecalho("Base (" + tabelaFrete.ParametroBase.Value.ObterDescricao() + ")", "ParametroBase", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Inicio Vigência", "DataInicial", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Fim Vigência", "DataFinal", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Tipo Pagamento", "DescricaoTipoPagamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            if (tabelaFrete.NumeroEntregas.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega))
            {
                grid.AdicionarCabecalho("Entrega", "NumeroEntrega", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Entrega", "DescricaoValorEntrega", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TipoEmbalagens.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem))
            {
                grid.AdicionarCabecalho("Tipo de Embalagem", "TipoEmbalagem", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo de Embalagem", "DescricaoValorTipoEmbalagem", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.PesosTransportados.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso))
            {
                grid.AdicionarCabecalho("Peso", "DescricaoPeso", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Peso", "DescricaoValorPeso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

                if (tabelaFrete.PermiteValorAdicionalPorPesoExcedente)
                {
                    grid.AdicionarCabecalho($"Valor a Cada {tabelaFrete.PesoExcecente.ToString("n3")}{tabelaFrete.PesosTransportados.Select(o => o.UnidadeMedida.Sigla).FirstOrDefault()} Excedente", "DescricaoValorPesoExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho($"Antigo Valor a Cada {tabelaFrete.PesoExcecente.ToString("n3")}{tabelaFrete.PesosTransportados.Select(o => o.UnidadeMedida.Sigla).FirstOrDefault()} Excedente", "DescricaoAntigoValorPesoExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                }
            }

            if (tabelaFrete.Distancias.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia))
            {
                grid.AdicionarCabecalho("Distância", "DescricaoDistancia", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Distância", "DescricaoValorDistancia", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TiposCarga.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga))
            {
                grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo Carga", "DescricaoValorTipoCarga", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosReboque.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque))
            {
                grid.AdicionarCabecalho("Reboque", "ModeloReboque", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Reboque", "DescricaoValorModeloReboque", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosTracao.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao))
            {
                grid.AdicionarCabecalho("Tração", "ModeloTracao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tração", "DescricaoValorModeloTracao", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Ajudantes.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante))
            {
                grid.AdicionarCabecalho("Ajudante", "NumeroAjudante", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Ajudante", "DescricaoValorAjudante", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Pallets.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets))
            {
                grid.AdicionarCabecalho("Pallet", "NumeroPallets", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Pallet", "DescricaoValorPallets", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Tempos.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo))
            {
                grid.AdicionarCabecalho("Tempo", "HoraTempo", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tempo", "DescricaoValorTempo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            for (int i = 0; i < tabelaFrete.Componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(tabelaFrete.Componentes[i].ComponenteFrete.Descricao, "DescricaoValorComponente" + UltimaColunaDinamica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo);
                    UltimaColunaDinamica++;
                }
                else
                    break;
            }

            grid.AdicionarCabecalho("Valor Total", "ValorTotal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);

            if (tabelaFrete.PossuiMinimoGarantido)
                grid.AdicionarCabecalho("Valor Mínimo", "ValorMinimo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorMaximo)
                grid.AdicionarCabecalho("Valor Máximo", "ValorMaximo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorBase)
                grid.AdicionarCabecalho("Valor Base", "DescricaoValorBase", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            grid.AdicionarCabecalho("Código Contrato", "CodigoContrato", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Descrição Contrato", "DescricaoContrato", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo da Carga", "GrupoDaCarga", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual", "PercentualRotaFormatado", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Canal de venda", "CanalVenda", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Canal de entrega", "CanalEntrega", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade de Entrega", "QuantidadeEntregas", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Item Retorno Integração", "ItemCodigoRetornoIntegracao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Status Aprovação", "StatusAprovacao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);

            return grid;
        }

        private dynamic ObterTabelaFreteAlteracaoRetornar(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao> listaTabelaFreteClienteAlteracao)
        {
            Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao tabelaFreteClienteAlteracao = listaTabelaFreteClienteAlteracao.Where(o => o.TabelaFreteAlteracao.Codigo == tabelaFreteAlteracao.Codigo).FirstOrDefault();
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = tabelaFreteClienteAlteracao?.TabelaFreteCliente;

            return new
            {
                tabelaFreteAlteracao.Codigo,
                CodigoIntegracao = tabelaFreteCliente?.Codigo ?? 0,
                tabelaFreteAlteracao.TabelaFrete?.Descricao,
                Origem = tabelaFreteCliente?.DescricaoOrigem ?? string.Empty,
                Destino = tabelaFreteCliente?.DescricaoDestino ?? string.Empty,
                Vigencia = tabelaFreteCliente?.DescricaoVigencia ?? string.Empty,
                Situacao = tabelaFreteAlteracao?.SituacaoAlteracao.ObterDescricaoPorTabelaFrete() ?? string.Empty
            };
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao origem)
        {
            return origem.SituacaoAlteracao == SituacaoAlteracaoTabelaFrete.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> listaTabelaFreteAlteracao;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork).BuscarConfiguracaoPadrao();
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao filtrosPesquisa = ObterFiltrosPesquisa(configuracaoAprovacao);
                Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete repositorioAprovacaoAlcada = new Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete(unitOfWork);

                listaTabelaFreteAlteracao = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaTabelaFreteAlteracao.Remove(new Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaTabelaFreteAlteracao = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaTabelaFreteAlteracao.Add(repositorioTabelaFreteAlteracao.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from tabelaFreteAlteracao in listaTabelaFreteAlteracao select tabelaFreteAlteracao.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork).BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Código", propriedade: "CodigoIntegracao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tabela", propriedade: "Descricao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                if (configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
                {
                    grid.AdicionarCabecalho(descricao: "Origem", propriedade: "Origem", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Destino", propriedade: "Destino", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Vigência", propriedade: "Vigencia", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                }

                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao filtrosPesquisa = ObterFiltrosPesquisa(configuracaoAprovacao);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete repositorio = new Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> listaTabelaFreteAlteracao;
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao> listaTabelaFreteClienteAlteracao;

                if (totalRegistros > 0)
                {
                    listaTabelaFreteAlteracao = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);

                    if (configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
                    {
                        List<int> codigosTabelaFreteAlteracao = listaTabelaFreteAlteracao.Select(alteracao => alteracao.Codigo).ToList();
                        Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(unitOfWork);

                        listaTabelaFreteClienteAlteracao = repositorioTabelaFreteClienteAlteracao.BuscarPorAlteracoes(codigosTabelaFreteAlteracao);
                    }
                    else
                        listaTabelaFreteClienteAlteracao = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>();
                }
                else
                {
                    listaTabelaFreteAlteracao = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>();
                    listaTabelaFreteClienteAlteracao = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>();
                }

                var listaTabelaFreteRetornar = (
                    from tabelaFreteAlteracao in listaTabelaFreteAlteracao
                    select ObterTabelaFreteAlteracaoRetornar(tabelaFreteAlteracao, listaTabelaFreteClienteAlteracao)
                ).ToList();

                grid.AdicionaRows(listaTabelaFreteRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao origem, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            if (origem.SituacaoAlteracao != SituacaoAlteracaoTabelaFrete.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
            Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoAprovacaoAlcada = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (servicoAprovacaoAlcada.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                {
                    origem.SituacaoAlteracao = SituacaoAlteracaoTabelaFrete.Aprovada;

                    servicoAprovacaoAlcada.LiberarAlteracaoTabelaFreteCliente(origem);
                    servicoTabelaFreteCliente.AdicionarComponentesFrete(origem.TabelaFrete);
                }
            }
            else
            {
                origem.SituacaoAlteracao = SituacaoAlteracaoTabelaFrete.Reprovada;

                servicoAprovacaoAlcada.ReprovarAlteracaoTabelaFreteCliente(origem);
            }

            repositorioTabelaFreteAlteracao.Atualizar(origem);
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CodigoIntegracao")
                return "Codigo";

            if (propriedadeOrdenar == "Descricao")
                return "TabelaFrete.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}