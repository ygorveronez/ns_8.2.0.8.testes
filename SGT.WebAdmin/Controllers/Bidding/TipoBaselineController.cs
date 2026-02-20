using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/TipoBaseline")]
    public class TipoBaselineController : BaseController
    {
		#region Construtores

		public TipoBaselineController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBaseline filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 10, Models.Grid.Align.center, true);

                if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline = new Repositorio.Embarcador.Bidding.TipoBaseline(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> tipoBaselines = repositorioTipoBaseline.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioTipoBaseline.ContarConsulta(filtrosPesquisa));

                var lista = (from p in tipoBaselines
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoIntegracao,
                                 p.Descricao,
                                 p.DescricaoStatus
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

                Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline = new Repositorio.Embarcador.Bidding.TipoBaseline(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline = new Dominio.Entidades.Embarcador.Bidding.TipoBaseline();

                PreencherTipoBaseline(tipoBaseline);

                if (!VerificarDuplicidade(tipoBaseline, repositorioTipoBaseline))
                    repositorioTipoBaseline.Inserir(tipoBaseline, Auditado);
                else
                    throw new ControllerException("Já existe um Tipo de Baseline com esses dados.");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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

                Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline = new Repositorio.Embarcador.Bidding.TipoBaseline(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline = repositorioTipoBaseline.BuscarPorCodigo(codigo, true);

                if (tipoBaseline == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTipoBaseline(tipoBaseline);

                if (!VerificarDuplicidade(tipoBaseline, repositorioTipoBaseline))
                    repositorioTipoBaseline.Atualizar(tipoBaseline, Auditado);
                else
                    throw new ControllerException("Impossível atualizar: já existe um Tipo de Baseline com esses dados.");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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

                Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline = new Repositorio.Embarcador.Bidding.TipoBaseline(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline = repositorioTipoBaseline.BuscarPorCodigo(codigo, false);

                if (tipoBaseline == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynTipoBaseline = new
                {
                    tipoBaseline.Codigo,
                    tipoBaseline.Descricao,
                    tipoBaseline.CodigoIntegracao,
                    tipoBaseline.Status,
                };

                return new JsonpResult(dynTipoBaseline);
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

                Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline = new Repositorio.Embarcador.Bidding.TipoBaseline(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline = repositorioTipoBaseline.BuscarPorCodigo(codigo, true);

                if (tipoBaseline == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioTipoBaseline.Deletar(tipoBaseline, Auditado);

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

        private void PreencherTipoBaseline(Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline)
        {
            tipoBaseline.Descricao = Request.GetStringParam("Descricao");
            tipoBaseline.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoBaseline.Status = Request.GetBoolParam("Status");
        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBaseline ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBaseline()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
            };
        }

        private bool VerificarDuplicidade(Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline, Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline)
        {
            return repositorioTipoBaseline.ExisteDuplicado(tipoBaseline);
        }

        #endregion
    }
}
