using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Patrimonio
{
    [CustomAuthorize("Patrimonio/MotivoDefeito")]
    public class MotivoDefeitoController : BaseController
    {
		#region Construtores

		public MotivoDefeitoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Dominio.ObjetosDeValor.Embarcador.Patrimonio.FiltroPesquisaMotivoDefeito filtrosPesquisa = ObterFiltrosPesquisa();
                

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Patrimonio.MotivoDefeito repoStatusLancamento = new Repositorio.Embarcador.Patrimonio.MotivoDefeito(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito> statusLancamento = repoStatusLancamento.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repoStatusLancamento.ContarConsulta(filtrosPesquisa));

                var lista = (from p in statusLancamento
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

                Repositorio.Embarcador.Patrimonio.MotivoDefeito repoMotivoDefeito = new Repositorio.Embarcador.Patrimonio.MotivoDefeito(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito MotivoDefeito = new Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito();

                PreencherMotivoDefeito(MotivoDefeito, unitOfWork);

                repoMotivoDefeito.Inserir(MotivoDefeito, Auditado);

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

                Repositorio.Embarcador.Patrimonio.MotivoDefeito repoMotivoDefeito = new Repositorio.Embarcador.Patrimonio.MotivoDefeito(unitOfWork);
               
                Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito MotivoDefeito = repoMotivoDefeito.BuscarPorCodigo(codigo, true);

                if (MotivoDefeito == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMotivoDefeito(MotivoDefeito, unitOfWork);

                repoMotivoDefeito.Atualizar(MotivoDefeito, Auditado);

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

                Repositorio.Embarcador.Patrimonio.MotivoDefeito repoMotivoDefeito = new Repositorio.Embarcador.Patrimonio.MotivoDefeito(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito MotivoDefeito = repoMotivoDefeito.BuscarPorCodigo(codigo, true);

                if (MotivoDefeito == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynMarcaEPI = new
                {
                    MotivoDefeito.Codigo,
                    MotivoDefeito.Descricao,
                    MotivoDefeito.Ativo
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Patrimonio.MotivoDefeito repoMotivoDefeito = new Repositorio.Embarcador.Patrimonio.MotivoDefeito(unitOfWork);
                Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito MotivoDefeito = repoMotivoDefeito.BuscarPorCodigo(codigo, true);

                if (MotivoDefeito == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repoMotivoDefeito.Deletar(MotivoDefeito, Auditado);

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
                    //Servicos.Log.TratarErro(ex);
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

        private void PreencherMotivoDefeito(Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito MotivoDefeito, Repositorio.UnitOfWork unitOfWork)
        {
            MotivoDefeito.Descricao = Request.GetStringParam("Descricao");
            MotivoDefeito.Ativo = Request.GetBoolParam("Ativo");
        }

        private Dominio.ObjetosDeValor.Embarcador.Patrimonio.FiltroPesquisaMotivoDefeito ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Patrimonio.FiltroPesquisaMotivoDefeito()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        #endregion

    }
}

