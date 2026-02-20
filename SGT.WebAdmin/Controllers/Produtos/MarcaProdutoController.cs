using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/MarcaProduto")]
    public class MarcaProdutoController : BaseController
    {
		#region Construtores

		public MarcaProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaMarcaProduto filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código", "CodigoIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcaProdutos = repMarcaProduto.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repMarcaProduto.ContarConsulta(filtrosPesquisa));

                var lista = (from p in marcaProdutos
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

                Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = new Dominio.Entidades.Embarcador.Produtos.MarcaProduto();

                PreencherMarcaProduto(marcaProduto, unitOfWork);

                repMarcaProduto.Inserir(marcaProduto, Auditado);

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

                Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = repMarcaProduto.BuscarPorCodigo(codigo, true);

                if (marcaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMarcaProduto(marcaProduto, unitOfWork);

                repMarcaProduto.Atualizar(marcaProduto, Auditado);

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

                Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = repMarcaProduto.BuscarPorCodigo(codigo, false);

                if (marcaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynMarcaProduto = new
                {
                    marcaProduto.Codigo,
                    marcaProduto.Descricao,
                    marcaProduto.CodigoIntegracao,
                    marcaProduto.Status,
                };

                return new JsonpResult(dynMarcaProduto);
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

                Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = repMarcaProduto.BuscarPorCodigo(codigo, true);

                if (marcaProduto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMarcaProduto.Deletar(marcaProduto, Auditado);

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

        private void PreencherMarcaProduto(Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto, Repositorio.UnitOfWork unitOfWork)
        {
            marcaProduto.Descricao = Request.GetStringParam("Descricao");
            marcaProduto.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            marcaProduto.Status = Request.GetBoolParam("Status");

            if (marcaProduto.Codigo == 0)
                marcaProduto.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaMarcaProduto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaMarcaProduto()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0
            };
        }

        #endregion
    }
}
