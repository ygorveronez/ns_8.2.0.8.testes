using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ControleThread")]

    public class ControleThreadController : BaseController
    {
        private const string ControleThreadKey = "ControleThread";

        #region Construtores

        public ControleThreadController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos 

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaControleThread filtrosPesquisa = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (!filtrosPesquisa.Ativo.HasValue)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta();
                Repositorio.ControleThread repositorioControleThread = new Repositorio.ControleThread(unitOfWork);
                int totalRegistros = repositorioControleThread.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.ControleThread> listaControleThread = (totalRegistros > 0) ? repositorioControleThread.Consultar(filtrosPesquisa, parametroConsulta) : new List<Dominio.Entidades.ControleThread>();

                var listalistaControleThreadRetornar = (
                    from controleThread in listaControleThread
                    select new
                    {
                        controleThread.Codigo,
                        controleThread.Descricao,
                        controleThread.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listalistaControleThreadRetornar);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.ControleThread repositorioControleThread = new Repositorio.ControleThread(unitOfWork);
                Dominio.Entidades.ControleThread controleThread = repositorioControleThread.BuscarPorCodigo(codigo, auditavel: true);

                if (controleThread == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                controleThread.Thread = Request.GetStringParam("Descricao");
                controleThread.Ativo = Request.GetBoolParam("Ativo");
                controleThread.DataCadastro = Request.GetDateTimeParam("DataCadastro");
                controleThread.DataInicio = Request.GetNullableDateTimeParam("DataInicio");
                controleThread.DataFim = Request.GetNullableDateTimeParam("DataFim");
                controleThread.Tempo = Request.GetIntParam("Tempo");

                repositorioControleThread.Atualizar(controleThread, Auditado);

                unitOfWork.CommitChanges();

                CacheProvider.Instance.Remove(ControleThreadKey + controleThread.Thread);

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("codigo");

                Repositorio.ControleThread repControleThread = new Repositorio.ControleThread(unitOfWork);
                Dominio.Entidades.ControleThread controleThread = repControleThread.BuscarPorCodigo(codigo, false);

                if (controleThread == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    controleThread.Codigo,
                    Descricao = controleThread.Thread,
                    DataCadastro = controleThread.DataCadastro.ToDateString(),
                    DataInicio = controleThread.DataInicio?.ToDateString() ?? string.Empty,
                    DataFim = controleThread.DataFim?.ToDateString() ?? string.Empty,
                    controleThread.Tempo,
                    controleThread.Ativo
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaControleThread ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaControleThread()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetNullableBoolParam("Ativo")
            };
        }

        #endregion
    }
}
