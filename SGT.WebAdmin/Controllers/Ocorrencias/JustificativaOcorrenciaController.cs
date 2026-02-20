using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/JustificativaOcorrencia")]
    public class JustificativaOcorrenciaController : BaseController
    {
		#region Construtores

		public JustificativaOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaJustificativaOcorrencia filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia repositorioJustificativaOcorrencia = new Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia> justificativasOcorrencia = repositorioJustificativaOcorrencia.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioJustificativaOcorrencia.ContarConsulta(filtrosPesquisa));

                var lista = (from p in justificativasOcorrencia
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
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

                Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia repositorioJustificativaOcorrencia = new Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia justificativaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia();

                PreencherJustificativaOcorrencia(justificativaOcorrencia, unitOfWork);

                repositorioJustificativaOcorrencia.Inserir(justificativaOcorrencia, Auditado);

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

                Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia repositorioJustificativaOcorrencia = new Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia justificativaOcorrencia = repositorioJustificativaOcorrencia.BuscarPorCodigo(codigo, true);

                if (justificativaOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherJustificativaOcorrencia(justificativaOcorrencia, unitOfWork);

                repositorioJustificativaOcorrencia.Atualizar(justificativaOcorrencia, Auditado);

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

                Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia repositorioJustificativaOcorrencia = new Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia justificativaOcorrencia = repositorioJustificativaOcorrencia.BuscarPorCodigo(codigo, false);

                if (justificativaOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynJustificativaOcorrencia = new
                {
                    justificativaOcorrencia.Codigo,
                    justificativaOcorrencia.Descricao,
                    justificativaOcorrencia.Ativo
                };

                return new JsonpResult(dynJustificativaOcorrencia);
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

                Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia repositorioJustificativaOcorrencia = new Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia justificativaOcorrencia = repositorioJustificativaOcorrencia.BuscarPorCodigo(codigo, true);

                if (justificativaOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioJustificativaOcorrencia.Deletar(justificativaOcorrencia, Auditado);

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

        private void PreencherJustificativaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.JustificativaOcorrencia justificativaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            justificativaOcorrencia.Descricao = Request.GetStringParam("Descricao");
            justificativaOcorrencia.Ativo = Request.GetBoolParam("Ativo");
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaJustificativaOcorrencia ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaJustificativaOcorrencia()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        #endregion
    }
}
