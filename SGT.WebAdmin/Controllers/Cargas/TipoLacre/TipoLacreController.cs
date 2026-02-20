using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.TipoLacre
{
    [CustomAuthorize("Cargas/TipoLacre")]
    public class TipoLacreController : BaseController
    {
		#region Construtores

		public TipoLacreController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.GetStringParam("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 20, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 20, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Cargas.TipoLacre repMarcaProduto = new Repositorio.Embarcador.Cargas.TipoLacre(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.TipoLacre> tipoLacres = repMarcaProduto.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMarcaProduto.ContarConsulta(descricao, ativo));

                var lista = (from p in tipoLacres
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

                Repositorio.Embarcador.Cargas.TipoLacre repMarcaProduto = new Repositorio.Embarcador.Cargas.TipoLacre(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoLacre tipoLacre = new Dominio.Entidades.Embarcador.Cargas.TipoLacre();

                PreencherTipoLacre(tipoLacre);

                repMarcaProduto.Inserir(tipoLacre, Auditado);

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

                Repositorio.Embarcador.Cargas.TipoLacre repMarcaProduto = new Repositorio.Embarcador.Cargas.TipoLacre(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoLacre tipoLacre = repMarcaProduto.BuscarPorCodigo(codigo, true);

                PreencherTipoLacre(tipoLacre);

                repMarcaProduto.Atualizar(tipoLacre, Auditado);

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
                Repositorio.Embarcador.Cargas.TipoLacre repMarcaProduto = new Repositorio.Embarcador.Cargas.TipoLacre(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoLacre tipoLacre = repMarcaProduto.BuscarPorCodigo(codigo, false);

                var dynMarcaProduto = new
                {
                    tipoLacre.Codigo,
                    tipoLacre.CodigoIntegracao,
                    tipoLacre.Descricao,
                    tipoLacre.Ativo,
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
                Repositorio.Embarcador.Cargas.TipoLacre repMarcaProduto = new Repositorio.Embarcador.Cargas.TipoLacre(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoLacre tipoLacre = repMarcaProduto.BuscarPorCodigo(codigo, true);

                if (tipoLacre == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMarcaProduto.Deletar(tipoLacre, Auditado);
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

        private void PreencherTipoLacre(Dominio.Entidades.Embarcador.Cargas.TipoLacre tipoLacre)
        {
            tipoLacre.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoLacre.Descricao = Request.GetStringParam("Descricao");
            tipoLacre.Ativo = Request.GetBoolParam("Ativo");
        }

        #endregion
    }
}
