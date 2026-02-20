using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/FormasTitulo")]
    public class FormasTituloController : BaseController
    {
		#region Construtores

		public FormasTituloController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.FormasTitulo repFormasTitulo = new Repositorio.Embarcador.Financeiro.FormasTitulo(unitOfWork);                

                string descricao = Request.GetStringParam("Descricao");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cód. Integração", "CodigoIntegracao", 20, Models.Grid.Align.center, true);

                List<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo> listaFormasTitulo = repFormasTitulo.Consultar(codigoIntegracao, descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFormasTitulo.ContarConsulta(codigoIntegracao, descricao));
                var lista = (from formaTitulo in listaFormasTitulo
                            select new
                            {
                                formaTitulo.Codigo,
                                formaTitulo.Descricao,
                                formaTitulo.CodigoIntegracao
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.FormasTitulo repFormasTitulo = new Repositorio.Embarcador.Financeiro.FormasTitulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.FormasTitulo formasTitulo = repFormasTitulo.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");

                formasTitulo.Descricao = descricao;
                formasTitulo.CodigoIntegracao = codigoIntegracao;

                if (repFormasTitulo.ContemFormaTituloCadastrado(codigoIntegracao, formasTitulo.Codigo))
                    return new JsonpResult(false, true, "Já existe um cadastro com o mesmo código de integração.");

                repFormasTitulo.Atualizar(formasTitulo, Auditado);

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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.FormasTitulo repFormasTitulo = new Repositorio.Embarcador.Financeiro.FormasTitulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.FormasTitulo formasTitulo = repFormasTitulo.BuscarPorCodigo(codigo);
                var dynFormaTitulo = new
                {
                    formasTitulo.Codigo,
                    formasTitulo.Descricao,
                    formasTitulo.CodigoIntegracao
                };
                return new JsonpResult(dynFormaTitulo);
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
        #endregion
    }
}
