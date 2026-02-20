using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/NotaFiscalObservacaoCartaCorrecao")]
    public class NotaFiscalObservacaoCartaCorrecaoController : BaseController
    {
		#region Construtores

		public NotaFiscalObservacaoCartaCorrecaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao repNotaFiscalObservacaoCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao(unitOfWork);

                string especificacao = Request.Params("Especificacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Especificação", "Especificacao", 90, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao> listaNotaFiscalObservacaoCartaCorrecao = repNotaFiscalObservacaoCartaCorrecao.Consultar(especificacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscalObservacaoCartaCorrecao.ContarConsulta(especificacao));
                var lista = (from p in listaNotaFiscalObservacaoCartaCorrecao
                            select new
                            {
                                p.Codigo,
                                p.Especificacao
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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao repNotaFiscalObservacaoCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao notaFiscalObservacaoCartaCorrecao = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao();

                bool status;

                string especificacao = Request.Params("Especificacao");
                string mensagem = Request.Params("Mensagem");

                bool.TryParse(Request.Params("Status"), out status);

                notaFiscalObservacaoCartaCorrecao.Especificacao = especificacao;
                notaFiscalObservacaoCartaCorrecao.Mensagem = mensagem;
                notaFiscalObservacaoCartaCorrecao.Status = status;

                repNotaFiscalObservacaoCartaCorrecao.Inserir(notaFiscalObservacaoCartaCorrecao, Auditado);
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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao repNotaFiscalObservacaoCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao notaFiscalObservacaoCartaCorrecao = repNotaFiscalObservacaoCartaCorrecao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                bool status;

                string especificacao = Request.Params("Especificacao");
                string mensagem = Request.Params("Mensagem");

                bool.TryParse(Request.Params("Status"), out status);

                notaFiscalObservacaoCartaCorrecao.Especificacao = especificacao;
                notaFiscalObservacaoCartaCorrecao.Mensagem = mensagem;
                notaFiscalObservacaoCartaCorrecao.Status = status;

                repNotaFiscalObservacaoCartaCorrecao.Atualizar(notaFiscalObservacaoCartaCorrecao, Auditado);
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
                Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao repNotaFiscalObservacaoCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao notaFiscalObservacaoCartaCorrecao = repNotaFiscalObservacaoCartaCorrecao.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    notaFiscalObservacaoCartaCorrecao.Codigo,
                    notaFiscalObservacaoCartaCorrecao.Especificacao,
                    notaFiscalObservacaoCartaCorrecao.Mensagem,
                    notaFiscalObservacaoCartaCorrecao.Status
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
                Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao repNotaFiscalObservacaoCartaCorrecao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao notaFiscalObservacaoCartaCorrecao = repNotaFiscalObservacaoCartaCorrecao.BuscarPorCodigo(codigo);
                repNotaFiscalObservacaoCartaCorrecao.Deletar(notaFiscalObservacaoCartaCorrecao, Auditado);
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
