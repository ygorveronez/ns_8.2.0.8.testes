using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TabelaPontuacao")]
    public class PontuacaoPorTipoOperacaoController : BaseController
    {
		#region Construtores

		public PontuacaoPorTipoOperacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("TipoOperacao").Nome("Tipo de Operação").Tamanho(60);
                grid.Prop("Pontuacao").Nome("Pontuação").Tamanho(30);

                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao repPontuacaoPorTipoOperacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao> pontuacaoPorTipoOperacaos = repPontuacaoPorTipoOperacao.BuscarTodas(grid.inicio, grid.limite);
                int totalRegistros = repPontuacaoPorTipoOperacao.ContarTodos();

                var lista = (from obj in pontuacaoPorTipoOperacaos
                             select new
                             {
                                 obj.Codigo,
                                 TipoOperacao = obj.TipoOperacao.Descricao,
                                 obj.Pontuacao
                             }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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



        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao repPontuacaoPorTipoOperacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao pontuacaoPorTipoOperacao = repPontuacaoPorTipoOperacao.BuscarPorCodigo(codigo, true);

                var dynPontuacao = new
                {
                    pontuacaoPorTipoOperacao.Codigo,
                    TipoOperacao = new { pontuacaoPorTipoOperacao.TipoOperacao.Codigo, pontuacaoPorTipoOperacao.TipoOperacao.Descricao },
                    pontuacaoPorTipoOperacao.Pontuacao
                };

                return new JsonpResult(dynPontuacao);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao repPontuacaoPorTipoOperacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao pontuacaoPorTipoOperacao = new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao();
                
                pontuacaoPorTipoOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
                pontuacaoPorTipoOperacao.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao pontuacaoPorTipoOperacaoExiste = repPontuacaoPorTipoOperacao.VerificarExistePorTipoOperacao(tipoOperacao);

                if (pontuacaoPorTipoOperacaoExiste == null)
                {
                    repPontuacaoPorTipoOperacao.Inserir(pontuacaoPorTipoOperacao, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para essa faixa.");
                }
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao repPontuacaoPorTipoOperacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao pontuacaoPorTipoOperacao = repPontuacaoPorTipoOperacao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                pontuacaoPorTipoOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
                pontuacaoPorTipoOperacao.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao pontuacaoPorTipoOperacaoExiste = repPontuacaoPorTipoOperacao.VerificarExistePorTipoOperacao(tipoOperacao);

                if (pontuacaoPorTipoOperacaoExiste == null || pontuacaoPorTipoOperacao.Codigo == pontuacaoPorTipoOperacaoExiste.Codigo)
                {
                    repPontuacaoPorTipoOperacao.Atualizar(pontuacaoPorTipoOperacao, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para essa faixa.");
                }
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
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
            try
            {
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao repPontuacaoPorTipoOperacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao(unitOfWork);

                int codigo = int.Parse(Request.Params("codigo"));

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao pontuacaoPorTipoOperacao = repPontuacaoPorTipoOperacao.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                repPontuacaoPorTipoOperacao.Deletar(pontuacaoPorTipoOperacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");
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

    }
}
