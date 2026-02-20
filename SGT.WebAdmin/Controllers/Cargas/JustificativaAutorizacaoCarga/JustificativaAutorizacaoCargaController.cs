using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.JustificativaAutorizacaoCarga
{
    [CustomAuthorize("Cargas/JustificativaAutorizacaoCarga")]
    public class JustificativaAutorizacaoCargaController : BaseController
    {
		#region Construtores

		public JustificativaAutorizacaoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga repositorioScript = new Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaJustificativaAutorizacaoCarga filtrosPesquisa = ObterFiltrosPesquisaScript();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Observação", "Observacao", 60, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioScript.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga> listaScript = totalRegistros > 0 ? repositorioScript.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga>();

                grid.AdicionaRows((
                    from o in listaScript
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.Observacao,
                        Situacao = o.Situacao.ObterDescricaoAtivo(),
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

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga repJustificativa = new Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga justificativa = repJustificativa.BuscarPorCodigo(codigo, true);

                if (justificativa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(justificativa);

                unitOfWork.Start();
                repJustificativa.Atualizar(justificativa, Auditado);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga repJustificativa = new Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga justificativa = new Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga();

                PreencherEntidade(justificativa);

                unitOfWork.Start();
                repJustificativa.Inserir(justificativa, Auditado);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga repJustificativa = new Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga justificativa = repJustificativa.BuscarPorCodigo(codigo);

                if (justificativa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repJustificativa.Deletar(justificativa, Auditado);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga repJustificativa = new Repositorio.Embarcador.Cargas.JustificativaAutorizacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga justificativa = repJustificativa.BuscarPorCodigo(codigo, true);

                if (justificativa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                dynamic dynPedido = new
                {
                    justificativa.Codigo,
                    justificativa.Descricao,
                    justificativa.Situacao,
                    justificativa.Observacao,
                    justificativa.CodigoIntegracao,
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

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaJustificativaAutorizacaoCarga ObterFiltrosPesquisaScript()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaJustificativaAutorizacaoCarga()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetNullableBoolParam("Situacao"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.JustificativaAutorizacaoCarga justificativa)
        {
            justificativa.Descricao = Request.GetStringParam("Descricao");
            justificativa.Situacao = Request.GetBoolParam("Situacao");
            justificativa.Observacao = Request.GetStringParam("Observacao");
            justificativa.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
        }

        #endregion
    }
}
