using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/Script")]
    public class ScriptController : BaseController
    {
		#region Construtores

		public ScriptController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.Script repositorioScript = new Repositorio.Embarcador.Configuracoes.Script(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaScript filtrosPesquisa = ObterFiltrosPesquisaScript();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Observação", "Observacao", 60, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioScript.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Configuracoes.Script> listaScript = totalRegistros > 0 ? repositorioScript.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Configuracoes.Script>();

                grid.AdicionaRows((
                    from o in listaScript
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.Observacao,
                    }).ToList()
                );

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
                Repositorio.Embarcador.Configuracoes.Script repScript = new Repositorio.Embarcador.Configuracoes.Script(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Configuracoes.Script script = repScript.BuscarPorCodigo(codigo, true);

                if (script == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(script);

                unitOfWork.Start();
                repScript.Atualizar(script, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.Script repScript = new Repositorio.Embarcador.Configuracoes.Script(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Script script = new Dominio.Entidades.Embarcador.Configuracoes.Script();

                PreencherEntidade(script);

                unitOfWork.Start();
                repScript.Inserir(script, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Configuracoes.Script repScript = new Repositorio.Embarcador.Configuracoes.Script(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Script script = repScript.BuscarPorCodigo(codigo);

                if (script == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repScript.Deletar(script, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.Script repScript = new Repositorio.Embarcador.Configuracoes.Script(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Script script = repScript.BuscarPorCodigo(codigo, true);

                if (script == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                dynamic dynPedido = new
                {
                    script.Codigo,
                    script.Descricao,
                    script.Observacao,
                    script.ScriptSQL
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metódos Privados

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaScript ObterFiltrosPesquisaScript()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaScript()
            {
                Descricao = Request.GetStringParam("Descricao"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Configuracoes.Script script)
        {
            script.Descricao = Request.GetStringParam("Descricao");
            script.Observacao = Request.GetStringParam("Observacao");
            script.ScriptSQL = Request.GetStringParam("ScriptSQL");
        }

        #endregion
    }
}
