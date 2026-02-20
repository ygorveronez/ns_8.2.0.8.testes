using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/TiposCausadoresOcorrencia")]
    public class TiposCausadoresOcorrenciaController : BaseController
    {
		#region Construtores

		public TiposCausadoresOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTiposCausadoresOcorrencia filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 20, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia> tiposCausadoresOcorrencia = repositorioTiposCausadoresOcorrencia.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioTiposCausadoresOcorrencia.ContarConsulta(filtrosPesquisa));

                var lista = (from p in tiposCausadoresOcorrencia
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao,
                                 p.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia();

                PreencherTiposCausadoresOcorrencia(tiposCausadoresOcorrencia, unitOfWork);

                repositorioTiposCausadoresOcorrencia.Inserir(tiposCausadoresOcorrencia, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia = repositorioTiposCausadoresOcorrencia.BuscarPorCodigo(codigo, true);

                if (tiposCausadoresOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTiposCausadoresOcorrencia(tiposCausadoresOcorrencia, unitOfWork);

                repositorioTiposCausadoresOcorrencia.Atualizar(tiposCausadoresOcorrencia, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia = repositorioTiposCausadoresOcorrencia.BuscarPorCodigo(codigo, false);

                if (tiposCausadoresOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynTiposCausadoresOcorrencia = new
                {
                    tiposCausadoresOcorrencia.Codigo,
                    tiposCausadoresOcorrencia.Descricao,
                    tiposCausadoresOcorrencia.CodigoIntegracao,
                    tiposCausadoresOcorrencia.Ativo
                };

                return new JsonpResult(dynTiposCausadoresOcorrencia);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia = repositorioTiposCausadoresOcorrencia.BuscarPorCodigo(codigo, true);

                if (tiposCausadoresOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioTiposCausadoresOcorrencia.Deletar(tiposCausadoresOcorrencia, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherTiposCausadoresOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            tiposCausadoresOcorrencia.Descricao = Request.GetStringParam("Descricao");
            tiposCausadoresOcorrencia.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tiposCausadoresOcorrencia.Ativo = Request.GetBoolParam("Ativo");
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTiposCausadoresOcorrencia ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTiposCausadoresOcorrencia()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        #endregion
    }
}
