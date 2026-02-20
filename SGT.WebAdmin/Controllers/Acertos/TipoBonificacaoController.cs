using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize("Acertos/TipoBonificacao")]
    public class TipoBonificacaoController : BaseController
    {
		#region Construtores

		public TipoBonificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Acerto.TipoBonificacao repTipoBonificacao = new Repositorio.Embarcador.Acerto.TipoBonificacao(unitOfWork);

                string descricao = Request.Params("Descricao");
                
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Acerto.TipoBonificacao> listaTipoBonificacao = repTipoBonificacao.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoBonificacao.ContarConsulta(descricao, ativo));
                var lista = from p in listaTipoBonificacao
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                p.DescricaoAtivo
                            };
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

                Repositorio.Embarcador.Acerto.TipoBonificacao repTipoBonificacao = new Repositorio.Embarcador.Acerto.TipoBonificacao(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.TipoBonificacao tipoBonificacao = new Dominio.Entidades.Embarcador.Acerto.TipoBonificacao();                
                bool.TryParse(Request.Params("Ativo"), out bool ativo);

                tipoBonificacao.Descricao = Request.Params("Descricao");
                tipoBonificacao.Observacao = Request.Params("Observacao");
                tipoBonificacao.Ativo = ativo;

                repTipoBonificacao.Inserir(tipoBonificacao, Auditado);
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
                Repositorio.Embarcador.Acerto.TipoBonificacao repTipoBonificacao = new Repositorio.Embarcador.Acerto.TipoBonificacao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Acerto.TipoBonificacao tipoBonificacao = repTipoBonificacao.BuscarPorCodigo(codigo, true);
                
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                tipoBonificacao.Descricao = Request.Params("Descricao");
                tipoBonificacao.Observacao = Request.Params("Observacao");
                tipoBonificacao.Ativo = ativo;
                repTipoBonificacao.Atualizar(tipoBonificacao, Auditado);
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
                Repositorio.Embarcador.Acerto.TipoBonificacao repTipoBonificacao = new Repositorio.Embarcador.Acerto.TipoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TipoBonificacao tipoBonificacao = repTipoBonificacao.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    tipoBonificacao.Codigo,
                    tipoBonificacao.Descricao,
                    tipoBonificacao.Observacao,
                    tipoBonificacao.Ativo
                };
                return new JsonpResult(dynProcessoMovimento);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Acerto.TipoBonificacao reTipoBonificacao = new Repositorio.Embarcador.Acerto.TipoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TipoBonificacao tipoBonificacao = reTipoBonificacao.BuscarPorCodigo(codigo);
                reTipoBonificacao.Deletar(tipoBonificacao, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
    }
}


