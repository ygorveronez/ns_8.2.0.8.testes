
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "DetalhesAutorizacao", "BuscarValoresSemParar", "VerificarAjustarPedagiosComSemParar", "ExportarPesquisa", "BuscarDadosRelatorio", "ConfiguracaoImportacao", "PesquisaAjuste", "AbrirAjuste", "ConsultarAutorizacoes" }, new string[] { "Fretes/AjusteTabelaFrete", "Fretes/ConsultaReajusteTabelaFrete" })]
    public class AjusteTabelaFreteController : BaseController
    {
		#region Construtores

		public AjusteTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Propriedades

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorioCU = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R004_TabelaFreteValor;
        private int UltimaColunaDinamica = 1;
        private int NumeroMaximoComplementos = 15;
        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasParticipantes = (decimal)5.50;
        private decimal TamanhoColunasEnderecoParticipantes = 3;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoTabelaFrete;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
                int.TryParse(Request.Params("DataVigencia"), out int codigoVigencia);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete situacao = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Todas);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "TabelaFrete")
                    propOrdenar = "TabelaFrete.Descricao";

                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajustes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

                int rowCount = repAjusteTabelaFrete.ContarConsulta(codigoTabelaFrete, dataInicial, dataFinal, codigoVigencia, situacao);

                if (rowCount > 0)
                    ajustes = repAjusteTabelaFrete.Consultar(codigoTabelaFrete, dataInicial, dataFinal, codigoVigencia, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(rowCount);

                grid.AdicionaRows((from obj in ajustes
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       TabelaFrete = obj.TabelaFrete.Descricao,
                                       DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                                       DataAjuste = obj.DataAjuste.HasValue ? obj.DataAjuste.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                       Situacao = obj.DescricaoSituacao
                                   }).ToList());

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarPlanilhaExcelAjustesSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosTabelaFreteAjustes = Request.GetListParam<int>("Codigos");
                List<string> colunasVisiveis = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>((string)Request.Params("Colunas"));

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> tabelasFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigos(codigosTabelaFreteAjustes);

                if (tabelasFreteAlteracao.Count == 0)
                    return new JsonpResult(false, "Nenhum registro encontrado.");

                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = tabelasFreteAlteracao.Select(alteracao => alteracao.TabelaFrete).Distinct().ToList();

                if (tabelasFrete.Count > 1)
                    throw new ControllerException("Não é possível selecionar tabelas de frete diferentes");

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete.FirstOrDefault();
                Models.Grid.Grid grid = GridPadrao(tabelaFrete);

                foreach (Models.Grid.Head colunaExistente in grid.header)
                    colunaExistente.visible = colunasVisiveis.Contains(colunaExistente.name) ? true : false;

                grid.tituloExportacao = "Valores Alterados";
                grid.group = new Models.Grid.Group();

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);         
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                List<int> codigosTabelasFreteCliente = repositorioTabelaFreteCliente.BuscarCodigosTabelasFreteClientePorAjuste(codigosTabelaFreteAjustes);
                 
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = tabelasFrete.FirstOrDefault().ParametroBase,
                    CodigosTabelasFreteCliente = codigosTabelasFreteCliente,
                    CodigosAjustesTabelaFrete = codigosTabelaFreteAjustes,
                    TipoRegistro = TipoRegistroAjusteTabelaFrete.Alterados
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

        public async Task<IActionResult> BuscarDadosRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete = 0;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

                if (tabelaFrete == null)
                    return new JsonpResult(false, "Tabela de frete não encontrada.");

                unitOfWork.Start();

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigoRelatorio = tabelaFrete.Relatorios.FirstOrDefault()?.Codigo ?? 0;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = serRelatorio.BuscarConfiguracaoPadrao(codigoControleRelatorioCU, TipoServicoMultisoftware, "Relatório de Tabelas de Frete", "Fretes", "ConsultaTabelaFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Remetente", "asc", "", "", codigoRelatorio, unitOfWork, true, true, 8);

                Models.Grid.Grid grid = GridPadrao(tabelaFrete);

                if (!mdlRelatorio.ConferirColunasDinamicas(ref relatorio, ref grid, ref UltimaColunaDinamica, NumeroMaximoComplementos, TamanhoColunasValores, "DescricaoValorComponente"))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não é possível carregar esse relatório pois existem componentes que foram inativados, por favor crie um novo padrão.");
                }

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(grid, relatorio);

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosRelatorioAlteracoesTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFreteAlteracao = Request.GetIntParam("Codigo");
                List<int> codigosTabelaFreteAlteracao = Request.GetListParam<int>("Codigos");
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete;

                if (codigosTabelaFreteAlteracao.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> tabelasFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPorCodigos(codigosTabelaFreteAlteracao);
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = tabelasFreteAlteracao.Select(alteracao => alteracao.TabelaFrete).Distinct().ToList();

                    if (tabelasFrete.Count > 1)
                        throw new ControllerException("Não é possível selecionar tabelas de frete diferentes.");

                    tabelaFrete = tabelasFrete.FirstOrDefault();
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete tabelaFreteAjuste = repositorioTabelaFreteAlteracao.BuscarPorCodigo(codigoTabelaFreteAlteracao, auditavel: false);
                    tabelaFrete = tabelaFreteAjuste?.TabelaFrete;
                }

                if (tabelaFrete == null)
                    throw new ControllerException("Tabela de frete não encontrada.");

                Models.Grid.Grid grid = GridPadrao(tabelaFrete);
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

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = repAjusteTabelaFrete.BuscarPorCodigo(codigo);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracoesTabelaFrete(unitOfWork, ajuste);

                return new JsonpResult(configuracoes.ToList());
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as colunas para importação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAjuste()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);
                int.TryParse(Request.Params("DataVigencia"), out int codigoVigencia);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete situacao = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Todas);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                //bool exibirSomenteRegistrosComReajuste = Request.GetBoolParam("ExibirSomenteRegistrosComReajuste");

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "TabelaFrete")
                    propOrdenar = "TabelaFrete.Descricao";

                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajustes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

                int rowCount = repAjusteTabelaFrete.ContarConsulta(codigoTabelaFrete, dataInicial, dataFinal, codigoVigencia, situacao);

                if (rowCount > 0)
                    ajustes = repAjusteTabelaFrete.Consultar(codigoTabelaFrete, dataInicial, dataFinal, codigoVigencia, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                //if (ajustes.Count > 0 && exibirSomenteRegistrosComReajuste)
                //    ajustes = repTabelaFreteCliente.BuscarTabelaFreteClienteComValorReajuste(ajustes.Select(o => o.Codigo).ToList());

                grid.setarQuantidadeTotal(rowCount);

                grid.AdicionaRows((from obj in ajustes
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       TabelaFrete = obj.TabelaFrete.Descricao,
                                       DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                                       DataAjuste = obj.DataAjuste.HasValue ? obj.DataAjuste.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                       Situacao = obj.DescricaoSituacao
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                int totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = (totalRegistros > 0) ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                filtrosPesquisa.IsRelatorio = true;

                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repositorioRelatorio.BuscarPorCodigo(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = servicoRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                unitOfWork.CommitChanges();

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                string stringConexao = _conexao.StringConexao;

                Task.Factory.StartNew(() => GerarRelatorioAjusteTabelaFrete(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AbrirAjuste()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Frete.AjusteTabelaFrete serAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

            try
            {
                int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");
                int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");
                int codigoVigencia = Request.GetIntParam("DataVigencia");
                int codigoEmpresaExclusiva = Request.GetIntParam("EmpresaExclusiva");
                int motivoReajuste = Request.GetIntParam("MotivoReajuste");
                int codigoContratoTransporteFrete = Request.GetIntParam("ContratoTransporteFrete");
                List<double> remetente = Request.GetListParam<double>("Remetente");
                List<double> destinatario = Request.GetListParam<double>("Destinatario");
                List<double> tomador = Request.GetListParam<double>("Tomador");
                List<int> codigoLocalidadeOrigem = Request.GetListParam<int>("LocalidadeOrigem");
                List<int> codigoLocalidadeDestino = Request.GetListParam<int>("LocalidadeDestino");
                List<int> codigoRegiaoDestino = Request.GetListParam<int>("RegiaoDestino");
                List<int> codigoTipoCarga = Request.GetListParam<int>("TipoCarga");
                List<int> codigoModeloTracao = Request.GetListParam<int>("ModeloTracao");
                List<int> codigoModeloReboque = Request.GetListParam<int>("ModeloReboque");
                List<int> codigoTipoOperacao = Request.GetListParam<int>("TipoOperacao");
                List<int> codigoEmpresa = Request.GetListParam<int>("Empresa");
                List<int> codigoRotaFreteDestino = Request.GetListParam<int>("RotaFreteDestino");
                List<int> codigoRotaFreteOrigem = Request.GetListParam<int>("RotaFreteOrigem");
                List<int> codigoCanalVenda = Request.GetListParam<int>("CanalVenda");
                List<int> codigoCanalEntrega = Request.GetListParam<int>("CanalEntrega");
                List<string> estadoDestino = Request.GetListParam<string>("EstadoDestino");
                List<string> estadoOrigem = Request.GetListParam<string>("EstadoOrigem");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao? tipoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao>("TipoPagamento");
                bool tabelaComCargaRealizada = Request.GetBoolParam("TabelaComCargaRealizada");
                bool apenasRegistrosComValor = Request.GetBoolParam("SomenteRegistrosComValores");
                bool utilizarBuscaNasLocalidadesPorEstadoOrigem = Request.GetBoolParam("UtilizarBuscaNasLocalidadesPorEstadoOrigem");
                bool utilizarBuscaNasLocalidadesPorEstadoDestino = Request.GetBoolParam("UtilizarBuscaNasLocalidadesPorEstadoDestino");
                bool ajustarPedagiosComSemParar = Request.GetBoolParam("AjustarPedagiosComSemParar");

                if (codigoEmpresaExclusiva > 0)
                    codigoEmpresa = new List<int>() { codigoEmpresaExclusiva };

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalVenda repCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
                Repositorio.Embarcador.Frete.MotivoReajuste repMotivoReajuste = new Repositorio.Embarcador.Frete.MotivoReajuste(unitOfWork);
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigenciaTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete repTempoEtapaAjusteTabelaFrete = new Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);

                unitOfWork.Start();

                // Cria entidade do ajuste
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete
                {
                    ApenasTabelasComCargasRealizadas = tabelaComCargaRealizada,
                    SomenteRegistrosComValores = apenasRegistrosComValor,
                    DataCriacao = DateTime.Now,
                    Remetentes = repCliente.BuscarPorCPFCNPJs(remetente),
                    Destinatarios = repCliente.BuscarPorCPFCNPJs(destinatario),
                    Origens = repLocalidade.BuscarPorCodigos(codigoLocalidadeOrigem),
                    Destinos = repLocalidade.BuscarPorCodigos(codigoLocalidadeDestino),
                    UFsOrigem = repEstado.BuscarPorSiglas(estadoOrigem),
                    UFsDestinos = repEstado.BuscarPorSiglas(estadoDestino),
                    GrupoPessoas = codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null,
                    ModelosReboque = repModeloVeiculo.BuscarPorCodigos(codigoModeloReboque),
                    ModelosTracao = repModeloVeiculo.BuscarPorCodigos(codigoModeloTracao),
                    UtilizarBuscaNasLocalidadesPorEstadoDestino = utilizarBuscaNasLocalidadesPorEstadoDestino,
                    UtilizarBuscaNasLocalidadesPorEstadoOrigem = utilizarBuscaNasLocalidadesPorEstadoOrigem,
                    RegioesDestino = repRegiao.BuscarPorCodigos(codigoRegiaoDestino),
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.EmCriacao,
                    Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAjusteTabelaFrete.Criacao,
                    TabelaFrete = codigoTabelaFrete > 0 ? repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete) : null,
                    TiposCarga = repTipoCarga.BuscarPorCodigos(codigoTipoCarga),
                    TiposOperacao = repTipoOperacao.BuscarPorCodigos(codigoTipoOperacao),
                    RotasFreteDestino = repRotaFrete.BuscarPorCodigos(codigoRotaFreteDestino),
                    RotasFreteOrigem = repRotaFrete.BuscarPorCodigos(codigoRotaFreteOrigem),
                    Empresas = repEmpresa.BuscarPorCodigos(codigoEmpresa),
                    ContratoTransporteFrete = repContratoTransporteFrete.BuscarPorCodigo(codigoContratoTransporteFrete),
                    TipoPagamento = tipoPagamento,
                    Tomadores = repCliente.BuscarPorCPFCNPJs(tomador),
                    Vigencia = codigoVigencia > 0 ? repVigenciaTabelaFrete.BuscarPorCodigo(codigoVigencia) : null,
                    MotivoReajuste = repMotivoReajuste.BuscarPorCodigo(motivoReajuste),
                    Numero = repAjusteTabelaFrete.BuscarUltimoNumero() + 1,
                    Criador = this.Usuario,
                    AjustarPedagiosComSemParar = ajustarPedagiosComSemParar,
                    CanaisVenda = repCanalVenda.BuscarPorCodigos(codigoCanalVenda),
                    CanaisEntrega = repCanalEntrega.BuscarPorCodigos(codigoCanalEntrega)
                };

                if (ConfiguracaoEmbarcador.ReplicarAjusteTabelaFreteTodasTabelas)
                {
                    if (ajusteTabelaFrete.Vigencia != null && ajusteTabelaFrete.Vigencia.DataFinal.HasValue && ajusteTabelaFrete.Vigencia.DataInicial > DateTime.Now) //(ajusteTabelaFrete.NovaVigencia != null || ajusteTabelaFrete.NovaVigenciaIndefinida.HasValue))
                    {
                        Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaExiste = repAjusteTabelaFrete.BuscarPorVigencia(ajusteTabelaFrete.Vigencia.Codigo);
                        
                        if (ajusteTabelaExiste != null)
                            throw new ControllerException("Já foi criado um ajuste para essa vigência, por favor, selecione registros da nova vigencia gerada para reajuste.");
                    }
                }

                ValidarInformacoesObrigatorias(ajusteTabelaFrete, unitOfWork);

                repAjusteTabelaFrete.Inserir(ajusteTabelaFrete);

                // Cria controle de tempo
                Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete tempoEtapa = new Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete
                {
                    AjusteTabelaFrete = ajusteTabelaFrete,
                    Etapa = ajusteTabelaFrete.Etapa,
                    Entrada = DateTime.Now,
                    Saida = null
                };

                // Insere controle de etapa
                repTempoEtapaAjusteTabelaFrete.Inserir(tempoEtapa);

                //List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFrete = repTabelaFreteCliente.BuscarTabelasParaAjuste(0, codigoTabelaFrete, codigoVigencia, remetente, destinatario, tomador, codigoLocalidadeOrigem, estadoOrigem, codigoLocalidadeDestino, estadoDestino, codigoRegiaoDestino, codigoTipoOperacao, codigoRotaFreteOrigem, codigoRotaFreteDestino, codigoEmpresa, tipoPagamento, tabelaComCargaRealizada, utilizarBuscaNasLocalidadesPorEstadoOrigem, utilizarBuscaNasLocalidadesPorEstadoDestino);
                //foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete in tabelasFrete)
                //    Servicos.Embarcador.Frete.TabelaFreteCliente.DuplicarParaAjuste(tabelaFrete, ajusteTabelaFrete, codigoModeloTracao, codigoModeloReboque, codigoTipoCarga, true, unitOfWork);

                //if (configuracao.ReplicarAjusteTabelaFreteTodasTabelas)
                //{
                //    ReplicarTodasTabelasDaVigencia(codigoTabelaFrete, codigoVigencia, tipoPagamento, tabelaComCargaRealizada, utilizarBuscaNasLocalidadesPorEstadoOrigem, utilizarBuscaNasLocalidadesPorEstadoDestino, tabelasFrete, ajusteTabelaFrete, codigoModeloTracao, codigoModeloReboque, codigoTipoCarga, unitOfWork);
                //}

                serAjusteTabelaFrete.SalvarLog(ajusteTabelaFrete, this.Usuario);
                unitOfWork.CommitChanges();

                return new JsonpResult(new { ajusteTabelaFrete.Codigo });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar o preview.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AplicarAjustes()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Frete.AjusteTabelaFrete serAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);

            try
            {
                int codigo, codigoVigencia;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("Vigencia"), out codigoVigencia);

                List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete> itensAjuste = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete>>(Request.Params("ItensAjuste"));

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);
                Repositorio.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores repAjusteTabelaFreteProcessamentoValores = new Repositorio.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo, true);

                if (ajusteTabelaFrete == null)
                    return new JsonpResult(false, true, "Ajuste não encontrado.");

                if (ajusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Pendente)
                    return new JsonpResult(false, true, "A situação do ajuste não permite essa operação.");

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores processamentoValores = new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores()
                {
                    AjusteTabelaFrete = ajusteTabelaFrete,
                    Usuario = this.Usuario,
                    ItensAjuste = Request.Params("ItensAjuste"),
                    Data = DateTime.Now
                };

                if (codigoVigencia > 0)
                    ajusteTabelaFrete.NovaVigencia = ajusteTabelaFrete.TabelaFrete.Vigencias.Where(o => o.Codigo == codigoVigencia).FirstOrDefault();

                ajusteTabelaFrete.NovaVigenciaIndefinida = Request.GetNullableDateTimeParam("NovaVigenciaIndefinida");
                ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.EmAjuste;

                if (ConfiguracaoEmbarcador.ReplicarAjusteTabelaFreteTodasTabelas)
                {
                    if (ajusteTabelaFrete.Vigencia != null && (ajusteTabelaFrete.NovaVigencia != null || (ajusteTabelaFrete.NovaVigenciaIndefinida.HasValue && ajusteTabelaFrete.Vigencia.DataInicial > DateTime.Now)))
                    {
                        Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaExiste = repAjusteTabelaFrete.BuscarPorVigencia(ajusteTabelaFrete.Vigencia.Codigo);
                        if (ajusteTabelaExiste != null && ajusteTabelaFrete.Codigo != ajusteTabelaExiste.Codigo)
                            return new JsonpResult(false, false, "Já foi criado um ajuste para essa vigência, por favor, cancele esse reajuste e selecione registros da nova vigencia gerada para reajuste.");
                    }
                }

                ValidarVigencia(ajusteTabelaFrete, unidadeTrabalho);

                unidadeTrabalho.Start();
                repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);
                repAjusteTabelaFreteProcessamentoValores.Inserir(processamentoValores);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ajusteTabelaFrete, ajusteTabelaFrete.GetChanges(), "Aplicou o ajuste.", unidadeTrabalho);
                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao aplicar os ajustes.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ResetarAjustes()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Frete.AjusteTabelaFrete serAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeTrabalho);
                Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemBaseCalculo = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeTrabalho);
                Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (ajusteTabelaFrete == null)
                    return new JsonpResult(false, true, "Ajuste não encontrado.");

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasAjuste = repTabelaFreteCliente.BuscarTabelasPorAjuste(ajusteTabelaFrete.Codigo);

                unidadeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaAjuste in tabelasAjuste)
                {
                    if (tabelaAjuste.TabelaOriginaria != null)
                    {
                        if (tabelaAjuste.Vigencia.Codigo != tabelaAjuste.TabelaOriginaria.Vigencia.Codigo)
                            tabelaAjuste.Vigencia = tabelaAjuste.TabelaOriginaria.Vigencia;

                        repTabelaFreteCliente.Atualizar(tabelaAjuste);

                        if (configuracao.ReplicarAjusteTabelaFreteTodasTabelas && !tabelaAjuste.AplicarAlteracoesDoAjuste)
                            continue;

                        if (ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue)
                        {
                            foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in tabelaAjuste.ParametrosBaseCalculo)
                            {
                                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in parametro.ItensBaseCalculo)
                                {
                                    item.Valor = item.ValorOriginal;
                                    repItemBaseCalculo.Atualizar(item);
                                }

                                parametro.ValorBase = parametro.ValorBaseOriginal;
                                parametro.ValorMaximo = parametro.ValorMaximoOriginal;
                                parametro.ValorMinimoGarantido = parametro.ValorMinimoGarantidoOriginal;
                                parametro.ValorAjudanteExcedente = parametro.ValorAjudanteExcedenteOriginal;
                                parametro.ValorEntregaExcedente = parametro.ValorEntregaExcedenteOriginal;
                                parametro.ValorPalletExcedente = parametro.ValorPalletExcedenteOriginal;
                                parametro.ValorPesoExcedente = parametro.ValorPesoExcedenteOriginal;
                                parametro.ValorQuilometragemExcedente = parametro.ValorQuilometragemExcedenteOriginal;

                                repParametroBaseCalculoTabelaFrete.Atualizar(parametro);
                            }
                        }
                        else
                        {
                            foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in tabelaAjuste.ItensBaseCalculo)
                            {
                                item.Valor = item.ValorOriginal;
                                repItemBaseCalculo.Atualizar(item);
                            }

                            tabelaAjuste.ValorBase = tabelaAjuste.ValorBaseOriginal;
                            tabelaAjuste.ValorMaximo = tabelaAjuste.ValorMaximoOriginal;
                            tabelaAjuste.ValorMinimoGarantido = tabelaAjuste.ValorMinimoGarantidoOriginal;
                            tabelaAjuste.ValorAjudanteExcedente = tabelaAjuste.ValorAjudanteExcedenteOriginal;
                            tabelaAjuste.ValorEntregaExcedente = tabelaAjuste.ValorEntregaExcedenteOriginal;
                            tabelaAjuste.ValorPalletExcedente = tabelaAjuste.ValorPalletExcedenteOriginal;
                            tabelaAjuste.ValorPesoExcedente = tabelaAjuste.ValorPesoExcedenteOriginal;
                            tabelaAjuste.ValorQuilometragemExcedente = tabelaAjuste.ValorQuilometragemExcedenteOriginal;

                            repTabelaFreteCliente.Atualizar(tabelaAjuste);
                        }
                    }
                    else
                    {
                        if (ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue)
                        {
                            foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in tabelaAjuste.ParametrosBaseCalculo)
                            {
                                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in parametro.ItensBaseCalculo)
                                    repItemBaseCalculo.Deletar(item);

                                repParametroBaseCalculoTabelaFrete.Deletar(parametro);
                            }
                        }
                        else
                        {
                            foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in tabelaAjuste.ItensBaseCalculo)
                                repItemBaseCalculo.Deletar(item);
                        }

                        repTabelaFreteCliente.Deletar(tabelaAjuste);
                    }
                }

                serAjusteTabelaFrete.SalvarLog(ajusteTabelaFrete, this.Usuario);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao retornar aos valores originais.");
            }
        }

        public async Task<IActionResult> CancelarAjuste()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Frete.AjusteTabelaFrete serAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo);

                if (ajusteTabelaFrete == null)
                    return new JsonpResult(false, true, "Ajuste não encontrado.");

                if (ajusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Pendente)
                    return new JsonpResult(false, true, "A situação do ajuste não permite essa operação.");

                unidadeTrabalho.Start();

                ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Cancelado;

                repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);

                serAjusteTabelaFrete.SalvarLog(ajusteTabelaFrete, this.Usuario);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar o ajuste.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarAjuste()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            try
            {
                unidadeTrabalho.Start();

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo);

                if (ajusteTabelaFrete == null)
                    throw new ControllerException("Ajuste não encontrado.");

                if (ajusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Pendente)
                    throw new ControllerException("A situação do ajuste não permite essa operação.");

                ajusteTabelaFrete.NovaVigenciaIndefinida = Request.GetNullableDateTimeParam("NovaVigenciaIndefinida");

                if (ConfiguracaoEmbarcador.ObrigarVigenciaNoAjusteFrete && !ajusteTabelaFrete.NovaVigenciaIndefinida.HasValue)
                    throw new ControllerException("É obrigatório informar uma nova vigência.");

                ValidarVigencia(ajusteTabelaFrete, unidadeTrabalho);
                ValidarInformacoesObrigatorias(ajusteTabelaFrete, unidadeTrabalho);

                // Verifica regras de aprovação

                repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);

                List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> regras = Servicos.Embarcador.Frete.AutorizacaoAjusteTabelaFrete.VerificarRegrasAutorizacaoAjuste(ajusteTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete.AprovacaoReajuste, unidadeTrabalho);
                if (regras.Count() == 0)
                {
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    if (configuracaoTMS.ObrigatorioRegrasOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim)
                    {
                        ajusteTabelaFrete.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAjusteTabelaFrete.AgAprovacao;
                        ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.SemRegraAprovacao;
                    }
                    else
                    {
                        ajusteTabelaFrete.SituacaoAposProcessamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Finalizado;
                        ajusteTabelaFrete.UsuarioAprovador = this.Usuario;
                        ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.EmProcessamento;
                        ajusteTabelaFrete.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAjusteTabelaFrete.Finalizada;
                        //serAjusteTabelaFrete.FinalizarAjuste(ajusteTabelaFrete, this.Usuario, unidadeTrabalho);
                    }
                }
                else
                {
                    ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.AgAprovacao;
                    ajusteTabelaFrete.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAjusteTabelaFrete.AgAprovacao;
                    Servicos.Embarcador.Frete.AutorizacaoAjusteTabelaFrete.CriarRegrasAutorizacao(regras, ajusteTabelaFrete, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, unidadeTrabalho);
                }

                repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao finalizar o ajuste.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarAjusteValores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo, true);

                if (ajusteTabelaFrete == null)
                    return new JsonpResult(false, true, "Ajuste não encontrado.");

                if (ajusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.ProblemaAjuste)
                    return new JsonpResult(false, true, "A situação do ajuste não permite essa operação.");

                ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.EmAjuste;

                unitOfWork.Start();
                repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ajusteTabelaFrete, "Reenviou para aplicar ajustes.", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao finalizar o ajuste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarAjuste()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo, true);

                if (ajusteTabelaFrete == null)
                    return new JsonpResult(false, true, "Ajuste não encontrado.");

                if (ajusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.ProblemaProcessamento)
                    return new JsonpResult(false, true, "A situação do ajuste não permite essa operação.");

                ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.EmProcessamento;

                unitOfWork.Start();
                repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ajusteTabelaFrete, "Reenviou para reprocessar.", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao finalizar o ajuste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarCriacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo, true);

                if (ajusteTabelaFrete == null)
                    return new JsonpResult(false, true, "Ajuste não encontrado.");

                if (ajusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.ProblemaCriacao)
                    return new JsonpResult(false, true, "A situação do ajuste não permite essa operação.");

                ajusteTabelaFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.EmCriacao;

                unitOfWork.Start();
                repAjusteTabelaFrete.Atualizar(ajusteTabelaFrete);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ajusteTabelaFrete, "Reenviou para criação.", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao finalizar o ajuste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo);

                if (ajusteTabelaFrete == null)
                    return new JsonpResult(false, true, "Ajuste não encontrado.");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                Dominio.Entidades.Empresa empresaExclusiva = (configuracaoTabelaFrete.ObrigatorioInformarTransportadorAjusteTabelaFrete || configuracaoTabelaFrete.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete) ? ajusteTabelaFrete.Empresas.FirstOrDefault() : null;

                var retorno = new
                {
                    ajusteTabelaFrete.Codigo,
                    ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoOrigem,
                    ajusteTabelaFrete.UtilizarBuscaNasLocalidadesPorEstadoDestino,
                    DataAjuste = ajusteTabelaFrete.DataAjuste.HasValue ? ajusteTabelaFrete.DataAjuste.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataCriacao = ajusteTabelaFrete.DataCriacao.ToString("dd/MM/yyyy"),
                    NovaVigenciaIndefinida = ajusteTabelaFrete.NovaVigenciaIndefinida?.ToString("dd/MM/yyyy") ?? string.Empty,
                    SomenteRegistrosComValores = ajusteTabelaFrete.SomenteRegistrosComValores,
                    MotivoReajuste = new
                    {
                        Codigo = ajusteTabelaFrete.MotivoReajuste?.Codigo ?? 0,
                        Descricao = ajusteTabelaFrete.MotivoReajuste?.Descricao ?? string.Empty
                    },
                    ajusteTabelaFrete.Situacao,
                    TabelaComCargaRealizada = ajusteTabelaFrete.ApenasTabelasComCargasRealizadas,
                    TabelaFrete = new
                    {
                        ajusteTabelaFrete.TabelaFrete.Codigo,
                        ajusteTabelaFrete.TabelaFrete.Descricao
                    },
                    ContratoTransporteFrete = new
                    {
                        Codigo = ajusteTabelaFrete.ContratoTransporteFrete?.Codigo ?? 0,
                        Descricao = ajusteTabelaFrete.ContratoTransporteFrete?.Descricao ?? "",
                    },
                    DataInicialContrato = ajusteTabelaFrete.ContratoTransporteFrete?.DataInicio.ToDateString() ?? "",
                    DataFinalContrato = ajusteTabelaFrete.ContratoTransporteFrete?.DataFim.ToDateString() ?? "",
                    ajusteTabelaFrete.TipoPagamento,
                    ItensAjuste = ObterItensAjusteTabelaFrete(ajusteTabelaFrete),
                    DataVigencia = new
                    {
                        Codigo = ajusteTabelaFrete.Vigencia?.Codigo ?? 0,
                        Descricao = ajusteTabelaFrete.Vigencia != null ? ajusteTabelaFrete.Vigencia.Descricao : ""
                    },

                    Detalhes = DetalhasAjuste(ajusteTabelaFrete, unidadeTrabalho),


                    Destinatario = (from obj in ajusteTabelaFrete.Destinatarios
                                    select new
                                    {
                                        Codigo = obj.CPF_CNPJ,
                                        obj.Descricao
                                    }).ToList(),
                    LocalidadeDestino = (from obj in ajusteTabelaFrete.Destinos
                                         select new
                                         {
                                             obj.Codigo,
                                             Descricao = obj.DescricaoCidadeEstado
                                         }).ToList(),
                    EstadoDestino = (from obj in ajusteTabelaFrete.UFsDestinos
                                     select new
                                     {
                                         obj.Codigo,
                                         Descricao = obj.Sigla
                                     }).ToList(),
                    EstadoOrigem = (from obj in ajusteTabelaFrete.UFsOrigem
                                    select new
                                    {
                                        obj.Codigo,
                                        Descricao = obj.Sigla
                                    }).ToList(),
                    ModeloReboque = (from obj in ajusteTabelaFrete.ModelosReboque
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.Descricao
                                     }).ToList(),
                    ModeloTracao = (from obj in ajusteTabelaFrete.ModelosTracao
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.Descricao
                                    }).ToList(),
                    ajusteTabelaFrete.Numero,
                    LocalidadeOrigem = (from loc in ajusteTabelaFrete.Origens
                                        select new
                                        {
                                            loc.Codigo,
                                            Descricao = loc.DescricaoCidadeEstado
                                        }).ToList(),
                    RegiaoDestino = (from obj in ajusteTabelaFrete.RegioesDestino
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.Descricao
                                     }).ToList(),
                    Remetente = (from obj in ajusteTabelaFrete.Remetentes
                                 select new
                                 {
                                     Codigo = obj.CPF_CNPJ,
                                     obj.Descricao
                                 }).ToList(),
                    TipoCarga = (from obj in ajusteTabelaFrete.TiposCarga
                                 select new
                                 {
                                     obj.Codigo,
                                     obj.Descricao
                                 }).ToList(),
                    TipoOperacao = (from obj in ajusteTabelaFrete.TiposOperacao
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.Descricao
                                    }).ToList(),
                    Tomador = (from obj in ajusteTabelaFrete.Tomadores
                               select new
                               {
                                   Codigo = obj.CPF_CNPJ,
                                   obj.Descricao
                               }).ToList(),
                    RotaFreteDestino = (from obj in ajusteTabelaFrete.RotasFreteDestino
                                        select new
                                        {
                                            obj.Codigo,
                                            obj.Descricao
                                        }).ToList(),
                    RotaFreteOrigem = (from obj in ajusteTabelaFrete.RotasFreteOrigem
                                       select new
                                       {
                                           obj.Codigo,
                                           obj.Descricao
                                       }).ToList(),
                    Empresa = (from obj in ajusteTabelaFrete.Empresas
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao
                               }).ToList(),
                    CanalVenda = (from obj in ajusteTabelaFrete.CanaisVenda
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.Descricao
                                  }).ToList(),
                    CanalEntrega = (from obj in ajusteTabelaFrete.CanaisEntrega
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.Descricao
                                  }).ToList(),
                    EmpresaExclusiva = new { Codigo = empresaExclusiva?.Codigo ?? 0, Descricao = empresaExclusiva?.Descricao ?? "" },
                    ajusteTabelaFrete.AjustarPedagiosComSemParar
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os detalhes do ajuste.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarValorItem()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoItem = Request.GetIntParam("CodigoItem");
                Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItem = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = repositorioItem.BuscarPorCodigo(codigoItem, true);

                if (item == null)
                    return new JsonpResult(false, true, "O item não foi encontrado para edição. Atualize a página e tente novamente.");

                item.Valor = Request.GetDecimalParam("ValorItem");

                repositorioItem.Atualizar(item, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o valor do item.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarValorFixo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAjuste = Request.GetIntParam("Ajuste");
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = repositorioAjusteTabelaFrete.BuscarPorCodigo(codigoAjuste);

                if (ajuste == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o ajuste.");

                int codigoParametro = Request.GetIntParam("CodigoItem");
                string info = Request.GetStringParam("Info");
                decimal valor = Request.GetDecimalParam("ValorItem");

                if (ajuste.TabelaFrete.ParametroBase.HasValue)
                {
                    Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repositorioParametro = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repositorioParametro.BuscarPorCodigo(codigoParametro);

                    if (parametro == null)
                        return new JsonpResult(false, true, "O item não foi encontrado para edição. Atualize a página e tente novamente.");

                    switch (info)
                    {
                        case "AjudanteExcedente":
                            parametro.ValorAjudanteExcedente = valor;
                            break;

                        case "DistanciaExcedente":
                            parametro.ValorQuilometragemExcedente = valor;
                            break;

                        case "EntregaExcedente":
                            parametro.ValorEntregaExcedente = valor;
                            break;

                        case "PalletsExcedente":
                            parametro.ValorPalletExcedente = valor;
                            break;

                        case "PesoExcedente":
                            parametro.ValorPesoExcedente = valor;
                            break;

                        case "ValorBase":
                            parametro.ValorBase = valor;
                            break;
                    }

                    repositorioParametro.Atualizar(parametro);
                }
                else
                {
                    Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete = repositorioTabelaFreteCliente.BuscarPorCodigo(codigoParametro);

                    if (tabelaFrete == null)
                        return new JsonpResult(false, true, "O item não foi encontrado para edição. Atualize a página e tente novamente.");

                    switch (info)
                    {
                        case "AjudanteExcedente":
                            tabelaFrete.ValorAjudanteExcedente = valor;
                            break;

                        case "DistanciaExcedente":
                            tabelaFrete.ValorQuilometragemExcedente = valor;
                            break;

                        case "EntregaExcedente":
                            tabelaFrete.ValorEntregaExcedente = valor;
                            break;

                        case "PalletsExcedente":
                            tabelaFrete.ValorPalletExcedente = valor;
                            break;

                        case "PesoExcedente":
                            tabelaFrete.ValorPesoExcedente = valor;
                            break;

                        case "ValorBase":
                            tabelaFrete.ValorBase = valor;
                            break;
                    }

                    repositorioTabelaFreteCliente.Atualizar(tabelaFrete);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o valor do item.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarSimulacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjuste = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unidadeTrabalho);
                Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacao repSimulacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacao(unidadeTrabalho);
                Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem repItemSimulacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem(unidadeTrabalho);
                Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente repItemSimulacaoComponente = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente(unidadeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unidadeTrabalho);
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao simulacao = repSimulacao.BuscarPorAjuste(codigo);

                if (simulacao != null)
                {
                    List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem> itensSimulacao = repItemSimulacao.BuscarPorSimulacao(simulacao.Codigo);

                    for (var i = 0; i < itensSimulacao.Count; i++)
                    {
                        List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente> componentes = repItemSimulacaoComponente.BuscarPorItem(itensSimulacao[i].Codigo);

                        for (var x = 0; x < componentes.Count; x++)
                            repItemSimulacaoComponente.Deletar(componentes[x]);

                        repItemSimulacao.Deletar(itensSimulacao[i]);
                    }

                    repSimulacao.Deletar(simulacao);
                }

                simulacao = new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacao();
                simulacao.Ajuste = repAjuste.BuscarPorCodigo(codigo);
                simulacao.DataCriacao = DateTime.Now;
                simulacao.DataInicial = dataInicial;
                simulacao.DataFinal = dataFinal;
                simulacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSimulacaoAjusteTabelaFrete.Gerando;

                repSimulacao.Inserir(simulacao);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasAjuste = repTabelaFreteCliente.BuscarTabelasPorAjuste(codigo, true);

                if (tabelasAjuste.Count <= 0)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não existem tabelas de frete vinculadas ao ajuste para realizar a simulacão.");
                }

                int countItensSimulacao = 0;

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaAjuste in tabelasAjuste)
                {
                    if (tabelaAjuste.TabelaOriginaria != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente> tabelasFreteCarga = repCargaTabelaFreteCliente.BuscarPorTabelaEPeriodo(tabelaAjuste.TabelaOriginaria.Codigo, dataInicial, dataFinal);

                        countItensSimulacao += tabelasFreteCarga.Count;

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFrete in tabelasFreteCarga)
                        {
                            Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem item = new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem();
                            item.TabelaAjuste = tabelaAjuste;
                            item.Simulacao = simulacao;
                            item.TabelaCarga = cargaTabelaFrete;

                            repItemSimulacao.Inserir(item);
                        }
                    }
                }

                if (countItensSimulacao <= 0)
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não existem cargas utilizando estas tabelas de frete para o período selecionado.");
                }

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R034_SimulacaoFrete, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unidadeTrabalho);

                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R034_SimulacaoFrete, TipoServicoMultisoftware, "Relatorio de Simulação de Fretes", "Fretes", "SimulacaoFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unidadeTrabalho, false, true);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                string stringConexao = _conexao.StringConexao;

                Task.Factory.StartNew(() => { GerarRelatorioSimulacao(simulacao.Codigo, TipoServicoMultisoftware, relatorioControleGeracao, stringConexao); });

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar a simulação.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

                int codAjuste = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Justificativa", false);
                grid.AdicionarCabecalho("MotivoRejeicao", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Etapa
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete.AprovacaoReajuste;

                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> listaAutorizacao = repAjusteTabelaFreteAutorizacao.ConsultarAutorizacoesPorAjusteEEtapa(codAjuste, etapa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repAjusteTabelaFreteAutorizacao.ContarConsultaAutorizacoesPorAjusteEEtapa(codAjuste, etapa);

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 PrioridadeAprovacao = obj.RegrasAutorizacaoTabelaFrete?.PrioridadeAprovacao ?? 0,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome ?? "",
                                 Regra = obj.RegrasAutorizacaoTabelaFrete?.Descricao,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 Justificativa = obj.Justificativa?.Descricao ?? "",
                                 MotivoRejeicao = obj.MotivoRejeicao?.Descricao ?? "",
                                 DT_RowColor = CorAprovacao(obj)
                             }).ToList();


                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarRegrasEtapas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositoriso
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                // Converte parametros
                int codigo = 0;
                int.TryParse(Request.Params("AjusteTabelaFrete"), out codigo);

                // Busca Ocorrencia
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = repAjusteTabelaFrete.BuscarPorCodigo(codigo);

                // Valida
                if (ajuste == null)
                    return new JsonpResult(false, true, "Registro não encontrada.");

                // Verifica qual regras consultar
                bool atualizaAjuste = false;
                if (ajuste.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.SemRegraAprovacao)
                {
                    // Busca se ha regras e cria
                    if (VerificarRegrasAutorizacaoAjuste(ajuste, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete.AprovacaoReajuste, TipoServicoMultisoftware, unitOfWork))
                        atualizaAjuste = true;
                }


                // Retorno de informacoes
                var retorno = new
                {
                    Situacao = ajuste.Situacao
                };

                // Atualiza a ocorrencia
                if (atualizaAjuste)
                    repAjusteTabelaFrete.Atualizar(ajuste);
                else
                    retorno = null;

                // Finaliza instancia
                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao ajusteAutorizacao = repAjusteTabelaFreteAutorizacao.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    ajusteAutorizacao.Codigo,
                    Regra = ajusteAutorizacao.RegrasAutorizacaoTabelaFrete.Descricao,
                    Situacao = ajusteAutorizacao.DescricaoSituacao,
                    Usuario = ajusteAutorizacao.Usuario?.Nome ?? "",

                    Data = ajusteAutorizacao.Data.HasValue ? ajusteAutorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Justificativa = ajusteAutorizacao.Justificativa?.Descricao ?? string.Empty,
                    MotivoRejeicao = ajusteAutorizacao.MotivoRejeicao?.Descricao ?? string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(ajusteAutorizacao.Motivo) ? ajusteAutorizacao.Motivo : string.Empty,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {


                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemBaseCalculo = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculo = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.VigenciaTabelaFrete repVigenciaTabelaFrete = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Servicos.Embarcador.Frete.AjusteTabelaFrete serAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                var Parametro = JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));

                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = repAjusteTabelaFrete.BuscarPorCodigo((int)Parametro.Codigo);

                string erro = string.Empty;
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasAjuste = repTabelaFreteCliente.BuscarTabelasPorAjuste(ajuste.Codigo);

                //(Rodrigo) bambiarra dos inferno pra deixar em memoria pra não dar session close, não tenho tempo pra fazer direito e vão a merda !
                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete in tabelasAjuste)
                {
                    List<Dominio.Entidades.Localidade> origensGambi = tabelaFrete.Origens.ToList();
                    List<Dominio.Entidades.Localidade> destinosnosGambi = tabelaFrete.Destinos.ToList();
                }

                int contador = 0;
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.Start();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coltransportador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJTransportadora" select obj).FirstOrDefault();
                        Dominio.Entidades.Empresa empresa = null;
                        if (coltransportador != null)
                        {
                            string cnpjTransportador = long.Parse(coltransportador.Valor).ToString("d14");
                            empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador);
                            if (empresa == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O cnpj do transportador não foi localizado na base multisoftware.", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                            else
                            {
                                if (ajuste.Empresas != null && ajuste.Empresas.Count > 0 && ajuste.Empresas.All(obj => obj.Codigo != empresa.Codigo))
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O transportador informado não pertence as regras do ajuste.", i));
                                    unitOfWork.Rollback();
                                    continue;
                                }
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaRotaFreteOrigem = (from obj in linha.Colunas where obj.NomeCampo == "RotaFreteOrigem" select obj).FirstOrDefault();
                        Dominio.Entidades.RotaFrete rotaFreteOrigem = null;
                        if (colunaRotaFreteOrigem != null)
                        {
                            rotaFreteOrigem = repRotaFrete.BuscarPorCodigoIntegracao(colunaRotaFreteOrigem.Valor);

                            if (rotaFreteOrigem == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A rota de frete de origem informada não foi localizado na base multisoftware.", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                            else
                            {
                                if (ajuste.RotasFreteOrigem?.Count > 0 && ajuste.RotasFreteOrigem.All(obj => obj.Codigo != rotaFreteOrigem.Codigo))
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A rota de frete de origem informada não pertence as regras do ajuste.", i));
                                    unitOfWork.Rollback();
                                    continue;
                                }
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaOrigem = (from obj in linha.Colunas where obj.NomeCampo == "Origem" select obj).FirstOrDefault();
                        List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
                        if (colunaOrigem != null)
                        {
                            string valor = colunaOrigem.Valor;
                            string[] splitOrigens = valor.Split('/');
                            bool falha = false;
                            for (int o = 0; o < splitOrigens.Length; o++)
                            {
                                if (splitOrigens[o].Contains("-"))
                                {
                                    string localidade = splitOrigens[o].Split('-')[0];
                                    string estado = splitOrigens[o].Split('-')[1];
                                    Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(localidade.Trim()), estado.Trim());
                                    if (origem == null)
                                    {
                                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não foi possível encontrar uma localidade com o nome " + splitOrigens[o] + " ", i));
                                        unitOfWork.Rollback();
                                        falha = true;
                                        break;
                                    }

                                    origens.Add(origem);
                                }
                                else
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("As localidade e a UF devem ser separadas pelo caractere - ", i));
                                    unitOfWork.Rollback();
                                    falha = true;
                                    break;
                                }
                            }
                            if (falha)
                                continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDestino = (from obj in linha.Colunas where obj.NomeCampo == "Destino" select obj).FirstOrDefault();
                        List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
                        if (colunaDestino != null)
                        {
                            string valor = colunaDestino.Valor;
                            string[] splitDestinos = valor.Split('/');
                            bool falha = false;
                            for (int o = 0; o < splitDestinos.Length; o++)
                            {
                                if (splitDestinos[o].Contains("-"))
                                {
                                    string localidade = splitDestinos[o].Split('-')[0];
                                    string estado = splitDestinos[o].Split('-')[1];
                                    Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorDescricaoEUF(localidade.Trim(), estado.Trim());
                                    if (destino == null)
                                    {
                                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não foi possível encontrar uma localidade com o nome " + splitDestinos[o] + " ", i));
                                        unitOfWork.Rollback();
                                        falha = true;
                                        break;
                                    }
                                    destinos.Add(destino);
                                }
                                else
                                {

                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("As localidade e a UF devem ser separadas pelo caractere - ", i));
                                    falha = true;
                                    unitOfWork.Rollback();
                                    break;
                                }
                            }
                            if (falha)
                                continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaRotaFreteDestino = (from obj in linha.Colunas where obj.NomeCampo == "RotaFreteDestino" select obj).FirstOrDefault();
                        Dominio.Entidades.RotaFrete rotaFreteDestino = null;
                        if (colunaRotaFreteDestino != null)
                        {
                            rotaFreteDestino = repRotaFrete.BuscarPorCodigoIntegracao(colunaRotaFreteDestino.Valor);
                            if (rotaFreteDestino == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A rota de frete de destino informada não foi localizado na base multisoftware.", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                            else
                            {
                                if (ajuste.RotasFreteDestino?.Count > 0 && ajuste.RotasFreteDestino.All(obj => obj.Codigo != rotaFreteDestino.Codigo))
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A rota de frete de destino informada não pertence as regras do ajuste.", i));
                                    unitOfWork.Rollback();
                                    continue;
                                }
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeicular = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeicular" select obj).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = null;
                        if (colModeloVeicular != null)
                        {
                            modeloVeiculo = repModeloVeicularCarga.buscarPorCodigoIntegracao(colModeloVeicular.Valor);
                            if (modeloVeiculo == null)
                            {
                                modeloVeiculo = repModeloVeicularCarga.buscarPorDescricao(colModeloVeicular.Valor);
                                if (modeloVeiculo == null)
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O modelo veícular informado não foi localizado na base multisoftware.", i));
                                    unitOfWork.Rollback();
                                    continue;
                                }
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colInicioVigencia = (from obj in linha.Colunas where obj.NomeCampo == "InicioVigencia" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerminoVigencia = (from obj in linha.Colunas where obj.NomeCampo == "TerminoVigencia" select obj).FirstOrDefault();

                        if ((colInicioVigencia != null && colTerminoVigencia == null) || (colInicioVigencia == null && colTerminoVigencia != null))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Para informar a vigência é necessário informar o inicio e o fim da vigência.", i));
                            unitOfWork.Rollback();
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCNPJClienteOrigem = (from obj in linha.Colunas where obj.NomeCampo == "CNPJClienteOrigem" select obj).FirstOrDefault();
                        Dominio.Entidades.Cliente clienteOrigem = null;
                        if (colunaCNPJClienteOrigem != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(colunaCNPJClienteOrigem.Valor)))
                        {
                            clienteOrigem = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(colunaCNPJClienteOrigem.Valor)));
                            if (clienteOrigem == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O cliente origem " + colunaCNPJClienteOrigem.Valor + " não não foi localizado na base multisoftware.", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                        }


                        Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigenciaTabelaFrete = null;

                        if (colInicioVigencia != null && colTerminoVigencia != null)
                        {
                            DateTime dataInicial, dataFinal;
                            DateTime.TryParseExact((string)colInicioVigencia.Valor, "dd/MM/yyyy", null, DateTimeStyles.None, out dataInicial);
                            DateTime.TryParseExact((string)colTerminoVigencia.Valor, "dd/MM/yyyy", null, DateTimeStyles.None, out dataFinal);

                            if (dataInicial == DateTime.MinValue)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A data de inicio da vigência não está no padrão correto (dd/mm/aaaa)", i));
                                unitOfWork.Rollback();
                                continue;
                            }

                            if (dataFinal == DateTime.MinValue)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A data de fim da vigência não está no padrão correto (dd/mm/aaaa)", i));
                                unitOfWork.Rollback();
                                continue;
                            }

                            vigenciaTabelaFrete = repVigenciaTabelaFrete.BuscarPorData(dataInicial, dataInicial, ajuste.TabelaFrete.Codigo);
                            if (vigenciaTabelaFrete == null)
                            {
                                vigenciaTabelaFrete = new Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete();
                                vigenciaTabelaFrete.DataInicial = dataInicial;
                                vigenciaTabelaFrete.DataFinal = dataFinal;
                                vigenciaTabelaFrete.TabelaFrete = ajuste.TabelaFrete;
                                repVigenciaTabelaFrete.Inserir(vigenciaTabelaFrete);
                            }

                        }
                        else if (ajuste.Vigencia == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatório informar a vigência de alteração", i));
                            unitOfWork.Rollback();
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                        if (colTipoOperacao != null)
                        {
                            tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(colTipoOperacao.Valor);
                            if (tipoOperacao == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tipo de operação informado não foi localizado na base multisoftware.", i));
                                unitOfWork.Rollback();
                                continue;
                            }
                            else
                            {
                                if (ajuste.TiposOperacao != null && ajuste.TiposOperacao.Count > 0 && ajuste.TiposOperacao.All(obj => obj.Codigo != tipoOperacao.Codigo))
                                {
                                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tipo de operacao informado não pertence as regras do ajuste.", i));
                                    unitOfWork.Rollback();
                                    continue;
                                }
                            }
                        }

                        if (vigenciaTabelaFrete == null)
                            vigenciaTabelaFrete = ajuste.Vigencia;

                        HashSet<int> codigosOrigens = new HashSet<int>(origens.Select(obj => obj.Codigo));
                        HashSet<int> codigosDestinos = new HashSet<int>(destinos.Select(obj => obj.Codigo));

                        List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> ajustes = (
                            from obj in tabelasAjuste
                            where (
                                (empresa != null && obj.Empresa.Codigo == empresa.Codigo || empresa == null) &&
                                (obj.Vigencia.Codigo == vigenciaTabelaFrete.Codigo) &&
                                (rotaFreteOrigem != null && obj.RotasOrigem.Any(r => r.Codigo == rotaFreteOrigem.Codigo) || rotaFreteOrigem == null) &&
                                (rotaFreteDestino != null && obj.RotasDestino.Any(r => r.Codigo == rotaFreteDestino.Codigo) || rotaFreteDestino == null) &&
                                obj.Origens.Count == origens.Count && obj.Origens.Where(or => !codigosOrigens.Contains(or.Codigo)).Count() == 0 &&
                                obj.Destinos.Count == destinos.Count && obj.Destinos.Where(des => !codigosDestinos.Contains(des.Codigo)).Count() == 0 &&
                                (tipoOperacao != null && obj.TiposOperacao.Any(o => o.Codigo == tipoOperacao.Codigo) || tipoOperacao == null) &&
                                (clienteOrigem != null && obj.ClientesOrigem.Any(o => o.CPF_CNPJ == clienteOrigem.CPF_CNPJ) || clienteOrigem == null)
                            )
                            select obj
                        ).ToList();

                        Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(ajuste.TabelaFrete.Codigo);

                        if (ajustes.Count() == 0)
                        {
                            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaCliente = new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente();
                            tabelaCliente.Empresa = empresa;
                            tabelaCliente.Ativo = true;

                            if (rotaFreteOrigem != null)
                            {
                                if (tabelaCliente.RotasOrigem == null)
                                    tabelaCliente.RotasOrigem = new List<Dominio.Entidades.RotaFrete>();

                                tabelaCliente.RotasOrigem.Add(rotaFreteOrigem);
                            }

                            if (rotaFreteDestino != null)
                            {
                                if (tabelaCliente.RotasDestino == null)
                                    tabelaCliente.RotasDestino = new List<Dominio.Entidades.RotaFrete>();

                                tabelaCliente.RotasDestino.Add(rotaFreteDestino);
                            }

                            if (origens.Count > 0)
                            {
                                tabelaCliente.Origens = origens;
                                tabelaCliente.DescricaoOrigem = string.Join(" / ", (from obj in origens select obj.Descricao).ToList());
                            }


                            if (destinos.Count > 0)
                            {
                                tabelaCliente.Destinos = destinos;
                                tabelaCliente.DescricaoDestino = string.Join(" / ", (from obj in destinos select obj.Descricao).ToList());
                            }


                            if (tipoOperacao != null)
                            {
                                if (tabelaCliente.TiposOperacao == null)
                                    tabelaCliente.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                                tabelaCliente.TiposOperacao.Add(tipoOperacao);
                            }

                            if (clienteOrigem != null)
                            {
                                if (tabelaCliente.ClientesOrigem == null)
                                    tabelaCliente.ClientesOrigem = new List<Dominio.Entidades.Cliente>();
                                tabelaCliente.ClientesOrigem.Add(clienteOrigem);
                            }


                            tabelaCliente.AplicarAlteracoesDoAjuste = true;
                            tabelaCliente.TabelaFrete = ajuste.TabelaFrete;
                            tabelaCliente.AjusteTabelaFrete = ajuste;
                            tabelaCliente.Vigencia = vigenciaTabelaFrete;
                            tabelaCliente.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Ajuste;
                            repTabelaFreteCliente.Inserir(tabelaCliente);

                            if (!tabelaCliente.TabelaFrete.ParametroBase.HasValue)// se não tem parametro base, criar os parametros da tabela
                            {
                                serAjusteTabelaFrete.AdicionarParamentrosPadroesTabela(tabelaCliente, tabelaFrete, null);
                            }
                            ajustes.Add(tabelaCliente);
                            tabelasAjuste.Add(tabelaCliente);
                        }

                        foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente ajusteFiltrado in ajustes)
                        {
                            if (ajusteFiltrado.TabelaFrete.ParametroBase.HasValue)
                            {
                                List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> parametrosFiltrados = null;
                                if (ajusteFiltrado.TabelaFrete.ParametroBase.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque)
                                {
                                    parametrosFiltrados = repParametroBaseCalculo.BuscarPorTabelaFrete(ajusteFiltrado.Codigo);
                                    parametrosFiltrados = (from obj in parametrosFiltrados where obj.CodigoObjeto == modeloVeiculo.Codigo select obj).ToList();
                                    if (parametrosFiltrados.Count == 0)
                                    {
                                        parametrosFiltrados = new List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>();
                                        Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroBase = new Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete();
                                        parametroBase.CodigoObjeto = modeloVeiculo.Codigo;
                                        parametroBase.TabelaFrete = ajusteFiltrado;
                                        repParametroBaseCalculo.Inserir(parametroBase);
                                        parametrosFiltrados.Add(parametroBase);
                                        serAjusteTabelaFrete.AdicionarParamentrosPadroesTabela(ajusteFiltrado, tabelaFrete, parametroBase);
                                    }
                                }
                                else
                                {
                                    parametrosFiltrados = new List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>();
                                }

                                foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in parametrosFiltrados)
                                {
                                    List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensBaseCalculo = repItemBaseCalculo.BuscarPorParametro(parametro.Codigo);
                                    foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itensBaseCalculo)
                                    {
                                        decimal valor = 0;
                                        string nomeCampo = item.TipoObjeto.ToString();
                                        if (item.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete)
                                            nomeCampo += "_" + item.CodigoObjeto;

                                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna dadosColuna = (from obj in linha.Colunas where obj.NomeCampo == nomeCampo select obj).FirstOrDefault();

                                        if (dadosColuna != null)
                                        {
                                            decimal.TryParse(dadosColuna.Valor, out valor);
                                            item.Valor = valor;
                                            repItemBaseCalculo.Atualizar(item);
                                        }
                                    }

                                    if (ajusteFiltrado.TabelaFrete.PossuiValorBase)
                                    {
                                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna dadosColuna = (from obj in linha.Colunas where obj.NomeCampo == "ValorBase" select obj).FirstOrDefault();
                                        if (dadosColuna != null)
                                        {
                                            decimal valorbase = Utilidades.Decimal.Converter(dadosColuna.Valor);
                                            parametro.ValorBase = valorbase;
                                            repParametroBaseCalculo.Atualizar(parametro);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensBaseCalculo = repItemBaseCalculo.BuscarPorTabelaFrete(ajusteFiltrado.Codigo);
                                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itensBaseCalculo)
                                {
                                    decimal valor = 0;
                                    string nomeCampo = item.TipoObjeto.ToString();
                                    if (item.TipoObjeto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete)
                                        nomeCampo += "_" + item.CodigoObjeto;

                                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna dadosColuna = (from obj in linha.Colunas where obj.NomeCampo == nomeCampo select obj).FirstOrDefault();

                                    if (dadosColuna != null)
                                    {
                                        decimal.TryParse(dadosColuna.Valor, out valor);
                                        item.Valor = valor;
                                        repItemBaseCalculo.Atualizar(item);
                                    }
                                }
                            }
                        }
                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);
                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro(ex2);
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                        continue;
                    }
                }

                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;
                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IntegrarAjuste()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = repAjusteTabelaFrete.BuscarPorCodigo(codigo);

                // Valida
                if (ajuste == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                EfetuarIntegracaoAvaria(ajuste, unitOfWork);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao integrar o reajuste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarAjustarPedagiosComSemParar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo > 0)
                {
                    Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigo);
                    if (tabelaFrete.ParametroBase == TipoParametroBaseTabelaFrete.ModeloReboque || tabelaFrete.ParametroBase == TipoParametroBaseTabelaFrete.ModeloTracao)
                    {
                        int total = tabelaFrete.Componentes.Count();
                        for (int i = 0; i < total; i++)
                        {
                            if (tabelaFrete.Componentes[i].ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.PEDAGIO)
                            {
                                return new JsonpResult(true);
                            }
                        }
                    }
                }
                return new JsonpResult(false);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao integrar o reajuste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarValoresSemParar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                if (codigo > 0)
                {
                    Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete = repAjusteTabelaFrete.BuscarPorCodigo(codigo);
                    if (ajusteTabelaFrete != null && ajusteTabelaFrete.AjustarPedagiosComSemParar)
                    {
                        Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unitOfWork);
                        Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componentePedagio = repComponenteFreteTabelaFrete.BuscarPorTabelaFrete(ajusteTabelaFrete.TabelaFrete.Codigo, TipoComponenteFrete.PEDAGIO);
                        if (componentePedagio != null)
                        {

                            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFreteCliente = repTabelaFreteCliente.BuscarTabelasPorAjuste(codigo);
                            List<int> codRotas = tabelasFreteCliente.Where(x => x.RotaFrete != null).Select(x => x.RotaFrete.Codigo).Distinct().ToList();

                            int total = tabelasFreteCliente.Count();
                            int totalRotas = codRotas.Count();
                            int totalTarifasAplicadasNoAjuste = 0;

                            for (int a = 0; a < totalRotas; a++)
                            {
                                int codRota = codRotas[a];

                                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasdefreteClienteDaRota = tabelasFreteCliente.Where(x => x.RotaFrete?.Codigo == codRota).Distinct().ToList();

                                if (codRota > 0 && (tabelasdefreteClienteDaRota != null && tabelasdefreteClienteDaRota.Count > 0))
                                {
                                    // Consulta a tarifa por modelo veicular para a rota (passando por todas as praças)
                                    Repositorio.Embarcador.Logistica.PracaPedagioTarifa repPracaPedagioTarifa = new Repositorio.Embarcador.Logistica.PracaPedagioTarifa(unitOfWork);
                                    List<Dominio.ObjetosDeValor.Embarcador.Frete.TarifaModeloVeicular> tarifasModeloVeicularSumatizadas = repPracaPedagioTarifa.BuscarSumarizadasPorRotaFrete(codRota);
                                    int totalTarifas = tarifasModeloVeicularSumatizadas.Count();
                                    if (totalTarifas > 0)
                                    {

                                        // Consulta os items de valor do componente 
                                        Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);
                                        IList<Dominio.ObjetosDeValor.Embarcador.Frete.CodigoModeloVeicularCargaItem> codigosModeloVeicularCargaItem = repItemParametroBaseCalculoTabelaFrete.BuscarCodigosModeloVeicularItemPorTabelasFreteCliente(componentePedagio.Codigo, (from obj in tabelasdefreteClienteDaRota select obj.Codigo).ToList());
                                        total = codigosModeloVeicularCargaItem.Count();
                                        for (int i = 0; i < total; i++)
                                        {
                                            // Identifica a tarifa para o modelo veicular
                                            decimal? valorTarifa = null;
                                            for (int j = 0; j < totalTarifas; j++)
                                            {
                                                if (tarifasModeloVeicularSumatizadas[j].ModeloVeicularCarga.Codigo == codigosModeloVeicularCargaItem[i].CodigoModeloVeicularCarga)
                                                {
                                                    valorTarifa = tarifasModeloVeicularSumatizadas[j].Tarifa;
                                                    break;
                                                }
                                            }
                                            if (valorTarifa.HasValue)
                                            {
                                                totalTarifasAplicadasNoAjuste++;
                                                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = repItemParametroBaseCalculoTabelaFrete.BuscarPorCodigo(codigosModeloVeicularCargaItem[i].CodigoItemParametroBaseCalculoTabelaFrete);
                                                item.Valor = valorTarifa.Value;
                                                repItemParametroBaseCalculoTabelaFrete.Atualizar(item);
                                            }
                                        }
                                    }
                                }
                            }

                            if (totalTarifasAplicadasNoAjuste > 0)
                            {
                                unitOfWork.CommitChanges();
                                return new JsonpResult(true, totalTarifasAplicadasNoAjuste + " tarifas de pedágio Sem Parar aplicadas no ajuste.");
                            }
                        }
                    }
                }
                return new JsonpResult(false, "Nenhuma tarifa de pedágio Sem Parar encontrada.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os valores da Sem Parar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void GerarRelatorioSimulacao(int codigoSimulacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, string stringConexao)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unidadeTrabalho);

            try
            {
                Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem repItemSimulacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem(unidadeTrabalho);

                Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unidadeTrabalho);

                List<int> codigosItens = repItemSimulacao.BuscarCodigosPorSimulacao(codigoSimulacao);

                for (var i = 0; i < codigosItens.Count; i++)
                {
                    unidadeTrabalho.Start();

                    Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem item = repItemSimulacao.BuscarPorCodigo(codigosItens[i]);

                    svcFreteCliente.SimularCalculoFrete(item, tipoServicoMultisoftware);

                    unidadeTrabalho.CommitChanges();

                    if (i > 0 && (i % 20) == 0)
                    {
                        unidadeTrabalho.Dispose();
                        unidadeTrabalho = null;

                        unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                        repItemSimulacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem(unidadeTrabalho);
                    }
                }

                var result = ReportRequest.WithType(ReportType.SimulacaoFrete)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("CodigoSimulacao", codigoSimulacao)
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unidadeTrabalho, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unidadeTrabalho, ex);
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        private Models.Grid.Grid GridPadrao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            UltimaColunaDinamica = 1;

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            bool visibleTMS = true;

            grid.AdicionarCabecalho("Código", "CodigoIntegracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                visibleTMS = false;
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
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
            grid.AdicionarCabecalho("Código Contrato", "CodigoContrato", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Descrição Contrato", "DescricaoContrato", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Grupo de carga", "DescricaoGrupoCarga", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Percentual", "PercentualRotaPorModeloVeicularCarga", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Canal Venda", "CanalVenda", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Canal Entrega", "CanalEntrega", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Quantidade Entrega", "QuantidadeEntregasPorModeloVeicularCarga", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
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
                grid.AdicionarCabecalho("Antigo Valor Entrega", "DescricaoAntigoValorEntrega", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Entrega", "PercentualDiferencaAtualizacaoValorEntrega", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();

                if (tabelaFrete.PermiteValorAdicionalPorEntregaExcedente)
                {
                    grid.AdicionarCabecalho("Valor a Cada Entrega Excedente", "DescricaoValorEntregaExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho("Antigo Valor a Cada Entrega Excedente", "DescricaoAntigoValorEntregaExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                    grid.AdicionarCabecalho("% Dif. Vl. Entrega Excedente", "PercentualDiferencaAtualizacaoValorEntregaExcedente", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
                }
            }

            if (tabelaFrete.TipoEmbalagens.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem))
            {
                grid.AdicionarCabecalho("Tipo de Embalagem", "TipoEmbalagem", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo de Embalagem", "DescricaoValorTipoEmbalagem", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Tipo de Embalagem", "DescricaoAntigoValorTipoEmbalagem", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Tipo de Embalagem", "PercentualDiferencaAtualizacaoValorTipoEmbalagem", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            if (tabelaFrete.PesosTransportados.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso))
            {
                grid.AdicionarCabecalho("Peso", "DescricaoPeso", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Peso", "DescricaoValorPeso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Peso", "DescricaoAntigoValorPeso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Peso", "PercentualDiferencaAtualizacaoValorPeso", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();

                if (tabelaFrete.PermiteValorAdicionalPorPesoExcedente)
                {
                    string unidadeMedida = tabelaFrete.PesosTransportados.Select(o => o.UnidadeMedida.Sigla).FirstOrDefault();
                    string pesoExcedente = tabelaFrete.PesoExcecente.ToString("n3");

                    grid.AdicionarCabecalho($"Valor a Cada {pesoExcedente}{unidadeMedida} Excedente", "DescricaoValorPesoExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho($"Antigo Valor a Cada {pesoExcedente}{unidadeMedida} Excedente", "DescricaoAntigoValorPesoExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                    grid.AdicionarCabecalho($"% Dif. Vl. a Cada {pesoExcedente}{unidadeMedida} Excedente", "PercentualDiferencaAtualizacaoValorPesoExcedente", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
                }
            }

            if (tabelaFrete.Distancias.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia))
            {
                grid.AdicionarCabecalho("Distância", "DescricaoDistancia", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Distância", "DescricaoValorDistancia", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Distância", "DescricaoAntigoValorDistancia", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Distância", "PercentualDiferencaAtualizacaoValorDistancia", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();

                if (tabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente)
                {
                    string quilometragemExcedente = tabelaFrete.QuilometragemExcedente.ToString("n2");

                    grid.AdicionarCabecalho($"Valor a Cada {quilometragemExcedente}km Excedente", "DescricaoValorDistanciaExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho($"Antigo Valor a Cada {quilometragemExcedente}km Excedente", "DescricaoAntigoValorDistanciaExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                    grid.AdicionarCabecalho($"% Dif. Vl. a Cada {quilometragemExcedente}", "PercentualDiferencaAtualizacaoValorDistanciaExcedente", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
                }
            }

            if (!tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga && tabelaFrete.TiposCarga.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga))
            {
                grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo Carga", "DescricaoValorTipoCarga", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Tipo Carga", "DescricaoAntigoValorTipoCarga", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Tipo Carga", "PercentualDiferencaAtualizacaoValorTipoCarga", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            if (tabelaFrete.ModelosReboque.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque))
            {
                grid.AdicionarCabecalho("Reboque", "ModeloReboque", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Reboque", "DescricaoValorModeloReboque", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Reboque", "DescricaoAntigoValorModeloReboque", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Reboque", "PercentualDiferencaAtualizacaoValorModeloReboque", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            if (tabelaFrete.ModelosTracao.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao))
            {
                grid.AdicionarCabecalho("Tração", "ModeloTracao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tração", "DescricaoValorModeloTracao", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Tração", "DescricaoAntigoValorModeloTracao", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Tração", "PercentualDiferencaAtualizacaoValorModeloTracao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            if (tabelaFrete.Ajudantes.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante))
            {
                grid.AdicionarCabecalho("Ajudante", "NumeroAjudante", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Ajudante", "DescricaoValorAjudante", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Ajudante", "DescricaoAntigoValorAjudante", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Ajudante", "PercentualDiferencaAtualizacaoValorAjudante", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();

                if (tabelaFrete.PermiteValorAdicionalPorAjudanteExcedente)
                {
                    grid.AdicionarCabecalho("Valor a Cada Ajudante Excedente", "DescricaoValorAjudanteExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho("Antigo Valor a Cada Ajudante Excedente", "DescricaoAntigoValorAjudanteExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                    grid.AdicionarCabecalho("% Dif. Vl. a Cada Ajudante Excedente", "PercentualDiferencaAtualizacaoValorAjudanteExcedente", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
                }
            }

            if (tabelaFrete.Pallets.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets))
            {
                grid.AdicionarCabecalho("Pallet", "NumeroPallets", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Pallet", "DescricaoValorPallets", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Pallet", "DescricaoAntigoValorPallets", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Pallet", "PercentualDiferencaAtualizacaoValorPallets", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();

                if (tabelaFrete.PermiteValorAdicionalPorPalletExcedente)
                {
                    grid.AdicionarCabecalho("Valor a Cada Pallet Excedente", "DescricaoValorPalletsExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho("Antigo Valor a Cada Pallet Excedente", "DescricaoAntigoValorPalletsExcedente", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                    grid.AdicionarCabecalho("% Dif. Vl. a Cada Pallet Excedente", "PercentualDiferencaAtualizacaoValorPalletsExcedente", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
                }
            }

            if (tabelaFrete.Tempos.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo))
            {
                grid.AdicionarCabecalho("Tempo", "HoraTempo", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tempo", "DescricaoValorTempo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Tempo", "DescricaoAntigoValorTempo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Tempo", "PercentualDiferencaAtualizacaoValorTempo", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            for (int i = 0; i < tabelaFrete.Componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(tabelaFrete.Componentes[i].ComponenteFrete.Descricao, $"DescricaoValorComponente{UltimaColunaDinamica}", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo);
                    grid.AdicionarCabecalho("Antigo Valor " + tabelaFrete.Componentes[i].ComponenteFrete.Descricao, $"DescricaoAntigoValorComponente{UltimaColunaDinamica}", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo).Uneditable();
                    grid.AdicionarCabecalho("% Dif. Vl. " + tabelaFrete.Componentes[i].ComponenteFrete.Descricao, $"PercentualDiferencaAtualizacaoValorComponente{UltimaColunaDinamica}", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo).Uneditable();
                    UltimaColunaDinamica++;
                }
                else
                {
                    break;
                }
            }

            grid.AdicionarCabecalho("Valor Total", "ValorTotal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true).Uneditable();
            grid.AdicionarCabecalho("Antigo Valor Total", "AntigoValorTotal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true).Uneditable();
            grid.AdicionarCabecalho("% Dif. Vl. Total", "PercentualDiferencaAtualizacaoValorTotal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();

            if (tabelaFrete.PossuiMinimoGarantido)
            {
                grid.AdicionarCabecalho("Valor Mínimo", "ValorMinimo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Mínimo", "AntigoValorMinimo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Mínimo", "PercentualDiferencaAtualizacaoValorMinimo", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            if (tabelaFrete.PossuiValorMaximo)
            {
                grid.AdicionarCabecalho("Valor Máximo", "ValorMaximo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Máximo", "AntigoValorMaximo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Máximo", "PercentualDiferencaAtualizacaoValorMaximo", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            if (tabelaFrete.PossuiValorBase)
            {
                grid.AdicionarCabecalho("Valor Base", "DescricaoValorBase", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Base", "AntigoValorBase", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true).Uneditable();
                grid.AdicionarCabecalho("% Dif. Vl. Base", "PercentualDiferencaAtualizacaoValorBase", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false).Uneditable();
            }

            grid.AdicionarCabecalho("Código Item", "ItemCodigoFormatado", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true).Uneditable();
            grid.AdicionarCabecalho("Código Item Retorno Integração", "ItemCodigoRetornoIntegracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true).Uneditable();
            grid.AdicionarCabecalho("Status Aprovação", "StatusAprovacao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false).Uneditable();
            grid.AdicionarCabecalho("Responsável", "UsuarioAlteracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Alteração", "DataAlteracao", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tabela", "TabelaFrete", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Criação", "DataCriacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Ajuste", "DataAjuste", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, true);

            return grid;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracoesTabelaFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste)
        {

            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();


            int ultimoFixo = 10;//caso adicionar outros fixos incremetar o ultimo fixo para o maior valor
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ Transportadora", Propriedade = "CNPJTransportadora", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Modelo Veicular", Propriedade = "ModeloVeicular", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Rota de Origem", Propriedade = "RotaFreteOrigem", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Rota de Destino", Propriedade = "RotaFreteDestino", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Origem", Propriedade = "Origem", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Destino", Propriedade = "Destino", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Tipo Operacao", Propriedade = "TipoOperacao", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Inicio Vigência (dd/MM/aaaa)", Propriedade = "InicioVigencia", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Termino Vigência (dd/MM/aaaa)", Propriedade = "TerminoVigencia", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = ultimoFixo, Descricao = "CNPJ Cliente Origem", Propriedade = "CNPJClienteOrigem", Tamanho = 150, Obrigatorio = false, CampoInformacao = true, Regras = new List<string> { } });

            var items = ObterItensAjusteTabelaFrete(ajuste);
            for (int i = 0; i < items.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao configuracao = new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao();
                configuracao.Descricao = (string)items[i].Descricao;
                configuracao.Id = (ultimoFixo + 1) + i;
                if ((int)items[i].Codigo > 0)
                    configuracao.Propriedade = items[i].Tipo.ToString() + "_" + (string)items[i].Codigo.ToString();
                else
                    configuracao.Propriedade = items[i].Tipo.ToString();

                configuracao.Obrigatorio = false;
                configuracao.CampoInformacao = true;
                configuracao.Regras = new List<string> { };
                configuracao.Tamanho = 150;
                configuracoes.Add(configuracao);
            }

            return configuracoes;

        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private void GerarRelatorioAjusteTabelaFrete(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

            try
            {
                var result = ReportRequest.WithType(ReportType.AjusteTabelaFrete)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("FiltrosPesquisa", filtrosPesquisa.ToJson())
                    .AddExtraData("Propriedades", propriedades.ToJson())
                    .AddExtraData("ParametrosConsulta", parametrosConsulta.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("RelatorioTemporario", servicoRelatorio.ObterConfiguracaoRelatorio(relatorioTemporario).ToJson())
                    .AddExtraData("Parametros", ObterParametrosConsulta(filtrosPesquisa, unitOfWork).ToJson())
                    .CallReport();

                if(!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);
            int codigoEmpresaExclusiva = Request.GetIntParam("EmpresaExclusiva");

            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
            {
                ParametroBase = tabelaFrete.ParametroBase,
                CodigoTabelaFrete = tabelaFrete.Codigo,
                CodigoAjusteTabelaFrete = Request.GetIntParam("Codigo"),
                CodigoContratoTransporteFrete = Request.GetIntParam("ContratoTransporteFrete"),
                CodigosEmpresa = Request.GetListParam<int>("Empresa"),
                CodigosEstadoDestino = Request.GetListParam<string>("EstadoDestino"),
                CodigosEstadoOrigem = Request.GetListParam<string>("EstadoOrigem"),
                CodigosLocalidadeDestino = Request.GetListParam<int>("LocalidadeDestino"),
                CodigosLocalidadeOrigem = Request.GetListParam<int>("LocalidadeOrigem"),
                CodigosModeloTracao = Request.GetListParam<int>("ModeloTracao"),
                CodigosModeloReboque = Request.GetListParam<int>("ModeloReboque"),
                CodigosRegiaoDestino = Request.GetListParam<int>("RegiaoDestino"),
                CodigosRotaFreteDestino = Request.GetListParam<int>("RotaFreteDestino"),
                CodigosRotaFreteOrigem = Request.GetListParam<int>("RotaFreteOrigem"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigoVigencia = Request.GetIntParam("DataVigencia"),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                CpfCnpjRemetentes = Request.GetListParam<double>("Remetente"),
                CpfCnpjTomadores = Request.GetListParam<double>("Tomador"),
                TabelaComCargaRealizada = Request.GetBoolParam("TabelaComCargaRealizada"),
                TipoPagamentoEmissao = Request.GetNullableEnumParam<TipoPagamentoEmissao>("TipoPagamento"),
                TipoRegistro = Request.GetNullableEnumParam<TipoRegistroAjusteTabelaFrete>("TipoRegistro"),
                UtilizarBuscaNasLocalidadesPorEstadoDestino = Request.GetBoolParam("UtilizarBuscaNasLocalidadesPorEstadoDestino"),
                UtilizarBuscaNasLocalidadesPorEstadoOrigem = Request.GetBoolParam("UtilizarBuscaNasLocalidadesPorEstadoOrigem"),
                AjustarPedagiosComSemParar = Request.GetBoolParam("AjustarPedagiosComSemParar"),
                SomenteRegistrosComValores = Request.GetBoolParam("SomenteRegistrosComValores"),
                CodigosRegiaoOrigem = Request.GetListParam<int>("RegiaoOrigem"),
                CodigosCanaisEntrega = Request.GetListParam<int>("CanalEntrega"),
                CodigosCanaisVenda = Request.GetListParam<int>("CanalVenda"),
            };

            if (codigoEmpresaExclusiva > 0)
                filtrosPesquisa.CodigosEmpresa = new List<int>() { codigoEmpresaExclusiva };

            return filtrosPesquisa;
        }

        private List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametrosConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            string descricaoTabelaFrete = repositorioTabelaFrete.BuscarDescricaoPorCodigo(filtrosPesquisa.CodigoTabelaFrete);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TabelaFrete", descricaoTabelaFrete, true));

            if (filtrosPesquisa.CodigoVigencia > 0)
            {
                Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = repositorioVigencia.BuscarPorCodigo(filtrosPesquisa.CodigoVigencia);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vigencia", vigencia.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vigencia", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", false));

            if (filtrosPesquisa.CodigosLocalidadeOrigem.Count > 0)
            {
                List<Dominio.Entidades.Localidade> localidades = repositorioLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosLocalidadeOrigem);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", string.Join(", ", localidades.Select(obj => obj.DescricaoCidadeEstado).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", false));

            if (filtrosPesquisa.CodigosLocalidadeDestino.Count > 0)
            {
                List<Dominio.Entidades.Localidade> localidades = repositorioLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosLocalidadeDestino);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", string.Join(", ", localidades.Select(obj => obj.DescricaoCidadeEstado).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", false));

            if (filtrosPesquisa.CodigosRegiaoDestino.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Localidades.Regiao> regiao = repositorioRegiao.BuscarPorCodigos(filtrosPesquisa.CodigosRegiaoDestino);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RegiaoDestino", string.Join(", ", regiao.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RegiaoDestino", false));

            if (filtrosPesquisa.CodigosEstadoOrigem.Count > 0)
            {
                List<Dominio.Entidades.Estado> estado = repositorioEstado.BuscarPorSiglas(filtrosPesquisa.CodigosEstadoOrigem);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", string.Join(", ", estado.Select(obj => obj.Sigla).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", false));

            if (filtrosPesquisa.CodigosEstadoDestino.Count > 0)
            {
                List<Dominio.Entidades.Estado> estado = repositorioEstado.BuscarPorSiglas(filtrosPesquisa.CodigosEstadoDestino);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", string.Join(", ", estado.Select(obj => obj.Sigla).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", false));

            if (filtrosPesquisa.CodigosTipoCarga.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tipoCarga = repositorioTipoCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTipoCarga);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", string.Join(", ", tipoCarga.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", false));

            if (filtrosPesquisa.CodigosModeloReboque.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelo = repositorioModeloVeicular.BuscarPorCodigos(filtrosPesquisa.CodigosModeloReboque);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloReboque", string.Join(", ", modelo.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloReboque", false));

            if (filtrosPesquisa.CodigosModeloTracao.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelo = repositorioModeloVeicular.BuscarPorCodigos(filtrosPesquisa.CodigosModeloTracao);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloTracao", string.Join(", ", modelo.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloTracao", false));

            if (filtrosPesquisa.CpfCnpjRemetentes.Count > 0)
            {
                List<Dominio.Entidades.Cliente> cliente = repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjRemetentes);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", string.Join(", ", cliente.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", false));

            if (filtrosPesquisa.CpfCnpjDestinatarios.Count > 0)
            {
                List<Dominio.Entidades.Cliente> cliente = repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjDestinatarios);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", string.Join(", ", cliente.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

            if (filtrosPesquisa.CpfCnpjTomadores.Count > 0)
            {
                List<Dominio.Entidades.Cliente> cliente = repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjTomadores);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", string.Join(", ", cliente.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

            if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tipoOperacao = repositorioTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacao);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", string.Join(", ", tipoOperacao.Select(obj => obj.Descricao).ToList()), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPagamento", filtrosPesquisa.TipoPagamentoEmissao.HasValue ? filtrosPesquisa.TipoPagamentoEmissao.Value == TipoPagamentoEmissao.A_Pagar ? "A pagar" : filtrosPesquisa.TipoPagamentoEmissao.Value == TipoPagamentoEmissao.Pago ? "Pago" : "Outros" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ComponenteFrete", false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TabelaComCargaRealizada", filtrosPesquisa.TabelaComCargaRealizada ? "Sim" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TransportadorTerceiro", false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContratoFrete", false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteEmVigencia", filtrosPesquisa.SomenteEmVigencia ? "Sim" : null));

            //Atualizar a informação também no ConsultaTabelaFreteController

            return parametros;
        }

        private dynamic ObterItensAjusteTabelaFrete(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete> itensAjuste = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete>();

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia) && ajusteTabelaFrete.TabelaFrete.Distancias.Count() > 0)
            {
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor da Distância", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.Distancia));

                if (ajusteTabelaFrete.TabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente)
                    itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete($"Valor a cada {ajusteTabelaFrete.TabelaFrete.QuilometragemExcedente.ToString("n2")}km excedente", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.DistanciaExcedente));
            }

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque) && ajusteTabelaFrete.TabelaFrete.ModelosReboque.Count() > 0)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Modelo de Reboque", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.ModeloReboque));

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao) && ajusteTabelaFrete.TabelaFrete.ModelosTracao.Count() > 0)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Modelo de Tração", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.ModeloTracao));

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega) && ajusteTabelaFrete.TabelaFrete.NumeroEntregas.Count() > 0)
            {
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Número de Entrega", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.NumeroEntrega));

                if (ajusteTabelaFrete.TabelaFrete.PermiteValorAdicionalPorEntregaExcedente)
                    itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor a cada entrega excedente", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.NumeroEntregaExcedente));
            }

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem) && ajusteTabelaFrete.TabelaFrete.TipoEmbalagens.Count() > 0)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Tipo de Embalagem", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.TipoEmbalagem));

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso) && ajusteTabelaFrete.TabelaFrete.PesosTransportados.Count() > 0)
            {
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Peso", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.Peso));

                if (ajusteTabelaFrete.TabelaFrete.PermiteValorAdicionalPorPesoExcedente)
                    itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete($"Valor a cada {ajusteTabelaFrete.TabelaFrete.PesoExcecente.ToString("n3") + ajusteTabelaFrete.TabelaFrete.PesosTransportados.Select(o => o.UnidadeMedida.Sigla).FirstOrDefault()} excedente", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.PesoExcedente));
            }

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Rota) && ajusteTabelaFrete.TabelaFrete.RotasFreteEmbarcador.Count() > 0)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor da Rota", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.Rota));

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga) && ajusteTabelaFrete.TabelaFrete.TiposCarga.Count() > 0)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Tipo de Carga", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.TipoCarga));

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets) && ajusteTabelaFrete.TabelaFrete.Pallets.Count() > 0)
            {
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Pallet", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.Pallets));

                if (ajusteTabelaFrete.TabelaFrete.PermiteValorAdicionalPorPalletExcedente)
                    itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete($"Valor a cada pallet excedente", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.PalletExcedente));
            }

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante) && ajusteTabelaFrete.TabelaFrete.Ajudantes.Count() > 0)
            {
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Ajudante", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.Ajudante));

                if (ajusteTabelaFrete.TabelaFrete.PermiteValorAdicionalPorAjudanteExcedente)
                    itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete($"Valor a cada ajudante excedente", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.AjudanteExcedente));
            }

            if ((!ajusteTabelaFrete.TabelaFrete.ParametroBase.HasValue || ajusteTabelaFrete.TabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo) && ajusteTabelaFrete.TabelaFrete.Tempos.Count() > 0)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor do Tempo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.Tempo));

            foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componente in ajusteTabelaFrete.TabelaFrete.Componentes)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete(componente.Codigo, "Valor do(a) " + componente.ComponenteFrete.Descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.ComponenteFrete, componente.ComponenteFrete.TipoComponenteFrete));

            if (ajusteTabelaFrete.TabelaFrete.PossuiMinimoGarantido)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor Mínimo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.ValorMinimo));

            if (ajusteTabelaFrete.TabelaFrete.PossuiValorMaximo)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor Máximo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.ValorMaximo));

            if (ajusteTabelaFrete.TabelaFrete.PossuiValorBase)
                itensAjuste.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ItemAjusteTabelaFrete("Valor Base", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroAjusteTabelaFrete.ValorBase));

            return itensAjuste;
        }

        private dynamic DetalhasAjuste(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            // Verifica situacao
            if (
                ajusteTabelaFrete.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Pendente ||
                ajusteTabelaFrete.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.SemRegraAprovacao ||
                ajusteTabelaFrete.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Cancelado
                )
                return null;

            // Respositorios
            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

            return new
            {
                ajusteTabelaFrete.Numero,
                Situacao = ajusteTabelaFrete.DescricaoSituacao,
                Criador = ajusteTabelaFrete.Criador?.Nome ?? string.Empty,
                DataCriacao = ajusteTabelaFrete.DataCriacao.ToString("dd/MM/yyyy"),
                NumeroAprovadores = repAjusteTabelaFreteAutorizacao.ContarAutorizacaoPorAjuste(ajusteTabelaFrete.Codigo),
                Aprovacoes = repAjusteTabelaFreteAutorizacao.ContarAprovacoesPorAjuste(ajusteTabelaFrete.Codigo),
                Reprovacoes = repAjusteTabelaFreteAutorizacao.ContarReprovacoesPorAjuste(ajusteTabelaFrete.Codigo)
            };
        }

        private string CorAprovacao(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao ajuste)
        {
            if (ajuste.Bloqueada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Grey;

            if (ajuste.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (ajuste.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (ajuste.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private bool VerificarRegrasAutorizacaoAjuste(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaFiltrada = Servicos.Embarcador.Frete.AutorizacaoAjusteTabelaFrete.VerificarRegrasAutorizacaoAjuste(ajuste, etapa, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.Frete.AutorizacaoAjusteTabelaFrete.CriarRegrasAutorizacao(listaFiltrada, ajuste, ajuste.Criador, tipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
                return true;
            }

            return false;
        }

        private void EfetuarIntegracaoAvaria(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete repTempoEtapaAjusteTabelaFrete = new Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete(unitOfWork);

            // Busca Tempo ajuste e fecha o mesmo
            Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete tempoEtapa = repTempoEtapaAjusteTabelaFrete.BuscarUltimaEtapa(ajuste.Codigo);
            tempoEtapa.Saida = DateTime.Now;

            ajuste.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Finalizado;
            ajuste.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAjusteTabelaFrete.Finalizada;

            // Atualizar
            repTempoEtapaAjusteTabelaFrete.Atualizar(tempoEtapa);
            repAjusteTabelaFrete.Atualizar(ajuste);
        }

        private void ValidarInformacoesObrigatorias(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            if (configuracaoTabelaFrete.ObrigatorioInformarTransportadorAjusteTabelaFrete)
            {
                if (ajuste.Empresas.Count == 0)
                    throw new ControllerException("O transportador é obrigatório para o ajuste da tabela de frete");

                if (ajuste.Empresas.Count > 1)
                    throw new ControllerException("Não é possível informar mais de um transportador para o ajuste da tabela de frete");
            }

            if (configuracaoTabelaFrete.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete)
            {
                if (ajuste.ContratoTransporteFrete == null)
                    throw new ControllerException("O contrato do transportador é obrigatório para o ajuste da tabela de frete");

                if (ajuste.Empresas.Count == 0)
                    throw new ControllerException("O contrato do transportador não possui um transportador informado");
            }
        }

        private void ValidarVigencia(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            if (ajusteTabelaFrete.NovaVigenciaIndefinida.HasValue)
            {
                if (ajusteTabelaFrete.NovaVigenciaIndefinida.Value.AddDays(-1) < ajusteTabelaFrete.Vigencia.DataInicial)
                    throw new ControllerException($"A nova vingência não pode ser anterior ou igual a data inicial da vigência de origem ({ajusteTabelaFrete.Vigencia.DataInicial.ToString("dd/MM/yyyy")}).");
            }

            if ((ajusteTabelaFrete.NovaVigencia != null) && (ajusteTabelaFrete.ContratoTransporteFrete != null))
            {
                if ((ajusteTabelaFrete.ContratoTransporteFrete.DataInicio != DateTime.MinValue) && (ajusteTabelaFrete.NovaVigencia.DataInicial < ajusteTabelaFrete.ContratoTransporteFrete.DataInicio))
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFrete.DataInicialVigenciaNaoPodeSerMenorDataInicialContrato);

                if ((ajusteTabelaFrete.ContratoTransporteFrete.DataFim != DateTime.MinValue) && (ajusteTabelaFrete.NovaVigencia.DataFinal > ajusteTabelaFrete.ContratoTransporteFrete.DataFim))
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFrete.DataFinalVigenciaNaoPodeSerMaiorDataFinalContrato);
            }
        }

        #endregion
    }
}
