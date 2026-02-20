using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/SituacaoLancamentoDocumentoEntrada")]
    public class SituacaoLancamentoDocumentoEntradaController : BaseController
    {
		#region Construtores

		public SituacaoLancamentoDocumentoEntradaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
              
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaSituacaoLancamentoDocumentoEntrada filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repoStatusLancamento = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada> statusLancamento = repoStatusLancamento.Consultar(filtrosPesquisa, parametrosConsulta);
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

                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repoSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada situacaoLancamentoDocumentoEntrada = new Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada();

                PreencherSituacaoLancamentoDocumentoEntrada(situacaoLancamentoDocumentoEntrada, unitOfWork);

                repoSituacaoLancamentoDocumentoEntrada.Inserir(situacaoLancamentoDocumentoEntrada,Auditado);

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

                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repoSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada situacaoLancamentoDocumentoEntrada = repoSituacaoLancamentoDocumentoEntrada.BuscarPorCodigo(codigo, true);

                if (situacaoLancamentoDocumentoEntrada == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherSituacaoLancamentoDocumentoEntrada(situacaoLancamentoDocumentoEntrada, unitOfWork);

                repoSituacaoLancamentoDocumentoEntrada.Atualizar(situacaoLancamentoDocumentoEntrada,Auditado);

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

                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repoSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada situacaoLancamentoDocumentoEntrada = repoSituacaoLancamentoDocumentoEntrada.BuscarPorCodigo(codigo, true);

                if (situacaoLancamentoDocumentoEntrada == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynMarcaEPI = new
                {
                    situacaoLancamentoDocumentoEntrada.Codigo,
                    situacaoLancamentoDocumentoEntrada.Descricao,
                    situacaoLancamentoDocumentoEntrada.Ativo
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

                Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada repoSituacaoLancamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada situacaoLancamentoDocumentoEntrada = repoSituacaoLancamentoDocumentoEntrada.BuscarPorCodigo(codigo, true);

                if (situacaoLancamentoDocumentoEntrada == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repoSituacaoLancamentoDocumentoEntrada.Deletar(situacaoLancamentoDocumentoEntrada,Auditado);

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

        private void PreencherSituacaoLancamentoDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada situacaoLancamentoDocumentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            situacaoLancamentoDocumentoEntrada.Descricao = Request.GetStringParam("Descricao");
            situacaoLancamentoDocumentoEntrada.Ativo = Request.GetBoolParam("Ativo");
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaSituacaoLancamentoDocumentoEntrada ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaSituacaoLancamentoDocumentoEntrada()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        #endregion

    }
}
