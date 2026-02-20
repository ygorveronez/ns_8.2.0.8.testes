using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Isca
{
    [CustomAuthorize("Cargas/Isca")]
    public class IscaController : BaseController
    {
		#region Construtores

		public IscaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaIsca filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = ObterGridPesquisa();

                Repositorio.Embarcador.Cargas.Isca repositorioIsca = new Repositorio.Embarcador.Cargas.Isca(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Isca> iscas = repositorioIsca.Consultar(filtrosPesquisa, grid.ObterParametrosConsulta());
                grid.setarQuantidadeTotal(repositorioIsca.ContarConsulta(filtrosPesquisa));
                
                var lista = (from p in iscas
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoIntegracao,
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

                Repositorio.Embarcador.Cargas.Isca repositorioIsca = new Repositorio.Embarcador.Cargas.Isca(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Isca isca = new Dominio.Entidades.Embarcador.Cargas.Isca();

                PreencherIsca(isca);

                repositorioIsca.Inserir(isca, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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

                Repositorio.Embarcador.Cargas.Isca repositorioIsca = new Repositorio.Embarcador.Cargas.Isca(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Isca isca = repositorioIsca.BuscarPorCodigo(codigo, true);

                PreencherIsca(isca);

                repositorioIsca.Atualizar(isca, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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
                Repositorio.Embarcador.Cargas.Isca repositorioIsca = new Repositorio.Embarcador.Cargas.Isca(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Isca isca = repositorioIsca.BuscarPorCodigo(codigo, false);

                var dynIsca = new
                {
                    isca.Codigo,
                    isca.CodigoIntegracao,
                    isca.Descricao,
                    isca.Ativo,
                    isca.CodigoEmpresaIsca,
                    isca.Site,
                    isca.Login,
                    isca.Senha

                };

                return new JsonpResult(dynIsca);
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
                Repositorio.Embarcador.Cargas.Isca repositorioIsca = new Repositorio.Embarcador.Cargas.Isca(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Isca isca = repositorioIsca.BuscarPorCodigo(codigo, true);
                
                if (isca == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioIsca.Deletar(isca, Auditado);
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
        
        private void PreencherIsca(Dominio.Entidades.Embarcador.Cargas.Isca isca)
        {
            isca.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            isca.Descricao = Request.GetStringParam("Descricao");
            isca.Ativo = Request.GetBoolParam("Ativo");
            isca.Site = Request.GetStringParam("Site");
            isca.Login = Request.GetStringParam("Login");
            isca.Senha = Request.GetStringParam("Senha");
            isca.CodigoEmpresaIsca = Request.GetStringParam("CodigoEmpresaIsca");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Isca repositorioIsca = new Repositorio.Embarcador.Cargas.Isca(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Status", "DescricaoAtivo", 20, Models.Grid.Align.center, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaIsca ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaIsca()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
            };
        }


        #endregion
    }
}
