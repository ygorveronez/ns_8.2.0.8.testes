using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize("Canhotos/MotivoRejeicaoAuditoria")]
    public class MotivoRejeicaoAuditoriaController : BaseController
    {
		#region Construtores

		public MotivoRejeicaoAuditoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaMotivoRejeicaoAuditoria filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descripção ", "Descricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 20, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria repositorioMotivoRejeicaoAuditoria = new Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria> motivoRejeicaoAuditoria = repositorioMotivoRejeicaoAuditoria.Consultar(filtrosPesquisa, parametrosConsulta);

                var lista = (from m in motivoRejeicaoAuditoria
                             select new
                             {
                                 m.Codigo,
                                 m.Descricao,
                                 m.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(repositorioMotivoRejeicaoAuditoria.ContarConsulta(filtrosPesquisa));
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria repositorioMotivoRejeicaoAuditoria = new Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria motivoRejeicaoAuditoria = repositorioMotivoRejeicaoAuditoria.BuscarPorCodigo(codigo);

                if (motivoRejeicaoAuditoria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o motivo.");

                var retornoMotivoRejeicao = new
                {
                    motivoRejeicaoAuditoria.Codigo,
                    motivoRejeicaoAuditoria.Descricao,
                    Status = motivoRejeicaoAuditoria.DescricaoAtivo,
                    Observacao = motivoRejeicaoAuditoria.Observacao ?? string.Empty,
                };

                return new JsonpResult(retornoMotivoRejeicao);
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

                Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria repositorioMotivoRejeicaoAuditoria = new Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria(unitOfWork);

                Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria motivoRejeicaoAuditoria = new Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria();

                PreencheMotivoRejeicaoAuditoria(motivoRejeicaoAuditoria);

                repositorioMotivoRejeicaoAuditoria.Inserir(motivoRejeicaoAuditoria, Auditado);

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

                Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria repositorioMotivoRejeicaoAuditoria = new Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria motivoRejeicaoAuditoria = repositorioMotivoRejeicaoAuditoria.BuscarPorCodigo(codigo);

                if (motivoRejeicaoAuditoria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o motivo.");

                PreencheMotivoRejeicaoAuditoria(motivoRejeicaoAuditoria);

                repositorioMotivoRejeicaoAuditoria.Atualizar(motivoRejeicaoAuditoria, Auditado);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria repositorioMotivoRejeicaoAuditoria = new Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria motivoRejeicaoAuditoria = repositorioMotivoRejeicaoAuditoria.BuscarPorCodigo(codigo);

                if (motivoRejeicaoAuditoria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioMotivoRejeicaoAuditoria.Deletar(motivoRejeicaoAuditoria, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheMotivoRejeicaoAuditoria(Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria motivoRejeicaoAuditoria)
        {
            motivoRejeicaoAuditoria.Descricao = Request.GetStringParam("Descricao");
            motivoRejeicaoAuditoria.Ativo = Request.GetBoolParam("Status");
            motivoRejeicaoAuditoria.Observacao = Request.GetStringParam("Observacao");
        }

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaMotivoRejeicaoAuditoria ObterFiltrosPesquisa()
        {

            return new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaMotivoRejeicaoAuditoria()
            {
              FiltroDescricao = Request.GetStringParam("Descricao"),
              FiltroSituacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status")
            };
        }

        #endregion
    }
}
