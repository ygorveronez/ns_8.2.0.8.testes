using Dominio.ObjetosDeValor.Enumerador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/ControleIntegracao")]
    public class ControleIntegracaoController : BaseController
    {
		#region Construtores

		public ControleIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> DownloadXmlsTesteDisponibilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                MemoryStream xmlsTesteDisponibilidade = new Servicos.Embarcador.Integracao.Sistema.IntegracaoSistema(unitOfWork).DownloadXmlsTesteDisponibilidade();

                return Arquivo(xmlsTesteDisponibilidade, "application/zip", "TesteDisponibilidade.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Integracoes.ControleIntegracao.OcorreuUmaFalhaAoRealizarODownloadDosXmlsDeTesteDeDisponibilidade);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        public async Task<IActionResult> ExportarPesquisaRetorno()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaWebServiceRetorno();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        public async Task<IActionResult> PerquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Propriedade, "Propriedade", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.De, "De", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Para, "Para", 35, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> propriedades;

                if (codigo > 0)
                {
                    Repositorio.Auditoria.HistoricoObjeto repositorio = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);
                    Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorio.BuscarPorCodigo(codigo);
                    propriedades = historico.Propriedades.OrderBy(o => o.Propriedade).ToList();
                }
                else
                    propriedades = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                int totalRegistros = propriedades.Count;

                var listaDetalhesRetornar = (
                    from p in propriedades
                    select new
                    {
                        p.Codigo,
                        p.Propriedade,
                        p.De,
                        p.Para
                    }
                );

                grid.AdicionaRows(listaDetalhesRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> PesquisaRetorno()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaWebServiceRetorno());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> TestarDisponibilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(new Servicos.Embarcador.Integracao.Sistema.IntegracaoSistema(unitOfWork).TestarDisponibilidade());
            }
            catch (Exception)
            {
                return new JsonpResult(false, Localization.Resources.Integracoes.ControleIntegracao.OcorreuUmaFalhaAoTestarADisponibilidade);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao()
            {
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                CodigoIntegradora = Request.GetIntParam("Integradora"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroCte = Request.GetIntParam("NumeroCte"),
                OrigemAuditado = Request.GetEnumParam<OrigemAuditado>("Origem")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();

            if (filtrosPesquisa.OrigemAuditado == OrigemAuditado.Sistema)
                return ObterGridPesquisaSistema(filtrosPesquisa);
            else if (filtrosPesquisa.OrigemAuditado == OrigemAuditado.WebServiceCargas)
                return ObterGridPesquisaWebServiceCarga(filtrosPesquisa);

            return ObterGridPesquisaWebService(filtrosPesquisa);
        }

        private Models.Grid.Grid ObterGridPesquisaSistema(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Data, "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Auditado, "Auditado", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Objeto, "Objeto", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Descricao, "Descricao", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Acao, "Acao", 28, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Auditoria.HistoricoObjeto repositorio = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaIntegracao(filtrosPesquisa);
                List<Dominio.Entidades.Auditoria.HistoricoObjeto> listaHistoricoObjeto = (totalRegistros > 0) ? repositorio.ConsultarIntegracao(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Auditoria.HistoricoObjeto>();

                var listaHistoricoObjetoRetornar = (
                    from historico in listaHistoricoObjeto
                    select new
                    {
                        historico.Codigo,
                        Acao = historico.DescricaoAcao,
                        historico.Auditado,
                        Auditado2 = (this.Usuario.UsuarioDaMultisoftware && !string.IsNullOrWhiteSpace(historico.UsuarioMultisoftware)) ? historico.UsuarioMultisoftware : historico.Auditado,
                        Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                        historico.Descricao,
                        historico.Objeto
                    }
                ).ToList();

                grid.AdicionaRows(listaHistoricoObjetoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaWebService(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Data, "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Acao, "Acao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("IP", "IP", 15, Models.Grid.Align.left, false);

                if ((filtrosPesquisa.OrigemAuditado == OrigemAuditado.WebServiceEmpresa) || (filtrosPesquisa.OrigemAuditado == OrigemAuditado.WebServiceNFS))
                    grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Integradora, "Integradora", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Auditoria.HistoricoObjeto repositorio = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaIntegracao(filtrosPesquisa);
                List<Dominio.Entidades.Auditoria.HistoricoObjeto> listaHistoricoObjeto = (totalRegistros > 0) ? repositorio.ConsultarIntegracao(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Auditoria.HistoricoObjeto>();

                var listaHistoricoObjetoRetornar = (
                    from historico in listaHistoricoObjeto
                    select new
                    {
                        historico.Codigo,
                        Integradora = historico.Integradora?.Descricao ?? "",
                        Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                        Acao = historico.DescricaoAcao,
                        IP = historico.IP ?? ""
                    }
                ).ToList();

                grid.AdicionaRows(listaHistoricoObjetoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaWebServiceCarga(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Data, "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Carga, "Carga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Acao, "Acao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("IP", "IP", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Auditoria.HistoricoObjeto repositorio = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaIntegracaoCarga(filtrosPesquisa);
                IList<Dominio.ObjetosDeValor.Embarcador.Auditoria.HistoricoObjetoCarga> listaHistoricoObjeto = (totalRegistros > 0) ? repositorio.ConsultarIntegracaoCarga(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Auditoria.HistoricoObjetoCarga>();

                var listaHistoricoObjetoRetornar = (
                    from historico in listaHistoricoObjeto
                    select new
                    {
                        historico.Codigo,
                        historico.Carga,
                        historico.Integradora,
                        historico.Data,
                        historico.Acao,
                        historico.IP
                    }
                ).ToList();

                grid.AdicionaRows(listaHistoricoObjetoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaWebServiceRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Data, "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.CodigoDeIntegracao, "CodigoIntegracao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Metodo, "NomeMetodo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Retorno, "Retorno", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.ControleIntegracao.Status, "StatusRetorno", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Auditoria.HistoricoIntegracao repositorio = new Repositorio.Auditoria.HistoricoIntegracao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Auditoria.HistoricoIntegracao> listaHistoricoObjeto = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Auditoria.HistoricoIntegracao>();

                var listaHistoricoObjetoRetornar = (
                    from historico in listaHistoricoObjeto
                    select new
                    {
                        historico.Codigo,
                        historico.CodigoIntegracao,
                        Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                        historico.NomeMetodo,
                        historico.Retorno,
                        StatusRetorno = (int)historico.StatusRetorno
                    }
                ).ToList();

                grid.AdicionaRows(listaHistoricoObjetoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
