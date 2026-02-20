using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/BiddingChecklistQuestionarioPadrao")]
    public class BiddingChecklistQuestionarioPadraoController : BaseController
    {
        #region Construtores

        public BiddingChecklistQuestionarioPadraoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingChecklistQuestionarioPadrao filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Bidding", "TipoBidding", 50, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao repBiddingChecklistQuestionarioPadrao = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao> biddingChecklistQuestionarioPadraos = repBiddingChecklistQuestionarioPadrao.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repBiddingChecklistQuestionarioPadrao.ContarConsulta(filtrosPesquisa));

                var lista = (from p in biddingChecklistQuestionarioPadraos
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 TipoBidding = p.TipoBidding.Descricao
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

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao repBiddingChecklistQuestionarioPadrao = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao biddingChecklistQuestionarioPadrao = new Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao();

                PreencherBiddingChecklistQuestionarioPadrao(biddingChecklistQuestionarioPadrao, unitOfWork);

                repBiddingChecklistQuestionarioPadrao.Inserir(biddingChecklistQuestionarioPadrao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { biddingChecklistQuestionarioPadrao.Codigo });
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

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao repBiddingChecklistQuestionarioPadrao = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao biddingChecklistQuestionarioPadrao = repBiddingChecklistQuestionarioPadrao.BuscarPorCodigo(codigo, true);

                if (biddingChecklistQuestionarioPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherBiddingChecklistQuestionarioPadrao(biddingChecklistQuestionarioPadrao, unitOfWork);

                repBiddingChecklistQuestionarioPadrao.Atualizar(biddingChecklistQuestionarioPadrao, Auditado);

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

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao repBiddingChecklistQuestionarioPadrao = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao(unitOfWork);

                Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao biddingChecklistQuestionarioPadrao = repBiddingChecklistQuestionarioPadrao.BuscarPorCodigo(codigo);

                if (biddingChecklistQuestionarioPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynBiddingChecklistQuestionarioPadrao = new
                {
                    biddingChecklistQuestionarioPadrao.Codigo,
                    biddingChecklistQuestionarioPadrao.Descricao,
                    biddingChecklistQuestionarioPadrao.Requisito,
                    TipoBidding = new
                    {
                        Codigo = biddingChecklistQuestionarioPadrao.TipoBidding?.Codigo ?? 0,
                        Descricao = biddingChecklistQuestionarioPadrao.TipoBidding?.Descricao ?? string.Empty
                    },
                    Anexos = from Anexos in biddingChecklistQuestionarioPadrao.Anexos
                             select new
                             {
                                 Anexos.Codigo,
                                 Anexos.Descricao,
                                 Anexos.NomeArquivo
                             }
                };

                return new JsonpResult(dynBiddingChecklistQuestionarioPadrao);
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

                Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao repBiddingChecklistQuestionarioPadrao = new Repositorio.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao biddingChecklistQuestionarioPadrao = repBiddingChecklistQuestionarioPadrao.BuscarPorCodigo(codigo, true);

                if (biddingChecklistQuestionarioPadrao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repBiddingChecklistQuestionarioPadrao.Deletar(biddingChecklistQuestionarioPadrao, Auditado);

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

        private void PreencherBiddingChecklistQuestionarioPadrao(Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao biddingChecklistQuestionarioPadrao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.TipoBidding repTipoDeBidding = new Repositorio.Embarcador.Bidding.TipoBidding(unitOfWork);

            biddingChecklistQuestionarioPadrao.Descricao = Request.GetStringParam("Descricao");
            biddingChecklistQuestionarioPadrao.Requisito = Request.GetEnumParam<TipoRequisitoBiddingChecklist>("Requisito");
            int codigoTipoBidding = Request.GetIntParam("TipoBidding");
            biddingChecklistQuestionarioPadrao.TipoBidding = codigoTipoBidding > 0 ? repTipoDeBidding.BuscarPorCodigo(codigoTipoBidding, false) : null;

        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingChecklistQuestionarioPadrao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingChecklistQuestionarioPadrao()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoTipoBidding = Request.GetIntParam("TipoBidding")
            };
        }

        #endregion
    }
}