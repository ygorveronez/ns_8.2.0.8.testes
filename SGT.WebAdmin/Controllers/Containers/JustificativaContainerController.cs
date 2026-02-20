using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Containers
{
    [CustomAuthorize("Containers/JustificativaContainer")]
    public class JustificativaContainerController : BaseController
    {
		#region Construtores

		public JustificativaContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaJustificativa filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status Container", "StatusContainer", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Pedidos.JustificativaContainer repositorioJustificativaContainer = new Repositorio.Embarcador.Pedidos.JustificativaContainer(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer> justificativaResultados = repositorioJustificativaContainer.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioJustificativaContainer.ContarConsulta(filtrosPesquisa));

                var lista = (from p in justificativaResultados
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo,
                                 StatusContainer = p.StatusContainer.ObterDescricao()

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

                Repositorio.Embarcador.Pedidos.JustificativaContainer repositorioJustificativaContainer = new Repositorio.Embarcador.Pedidos.JustificativaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer justificativaContainer = new Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer();

                PreencherJustificativaContainer(justificativaContainer, unitOfWork);
                repositorioJustificativaContainer.Inserir(justificativaContainer, Auditado);
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

                Repositorio.Embarcador.Pedidos.JustificativaContainer repositorioJustificativaContainer = new Repositorio.Embarcador.Pedidos.JustificativaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer justificativaAlterar = repositorioJustificativaContainer.BuscarPorCodigo(codigo, true);

                if (justificativaAlterar == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherJustificativaContainer(justificativaAlterar, unitOfWork);

                repositorioJustificativaContainer.Atualizar(justificativaAlterar, Auditado);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            int codigo = Request.GetIntParam("Codigo");
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.JustificativaContainer repositorioJustificativaContainer = new Repositorio.Embarcador.Pedidos.JustificativaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer justificativaExcluir = repositorioJustificativaContainer.BuscarPorCodigo(codigo, true);

                if (justificativaExcluir == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioJustificativaContainer.Deletar(justificativaExcluir, Auditado);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.JustificativaContainer repositorioJustificativaContainer = new Repositorio.Embarcador.Pedidos.JustificativaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer justificativaPesquisa = repositorioJustificativaContainer.BuscarPorCodigo(codigo, false);

                if (justificativaPesquisa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynMarcaEPI = new
                {
                    justificativaPesquisa.Codigo,
                    justificativaPesquisa.Descricao,
                    justificativaPesquisa.Ativo,
                    justificativaPesquisa.StatusContainer,
                    justificativaPesquisa.Observacao
                };

                return new JsonpResult(dynMarcaEPI);
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

        private void PreencherJustificativaContainer(Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer justificativaContainer, Repositorio.UnitOfWork unitOfWork)
        {
            justificativaContainer.Descricao = Request.GetStringParam("Descricao");
            justificativaContainer.Ativo = Request.GetBoolParam("Ativo");
            justificativaContainer.StatusContainer = Request.GetEnumParam<StatusColetaContainer>("StatusContainer");
            justificativaContainer.Observacao = Request.GetStringParam("Observacao");
        }

        #region private 

        private Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaJustificativa ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaJustificativa()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                StatusContainer = Request.GetNullableEnumParam<StatusColetaContainer>("StatusContainer"),
            };
        }

        #endregion
    }
}
