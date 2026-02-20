using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TabelaPontuacao")]
    public class PontuacaoPorQuantidadeCargaGanhaCotacaoController : BaseController
    {
		#region Construtores

		public PontuacaoPorQuantidadeCargaGanhaCotacaoController(Conexao conexao) : base(conexao) { }

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
                grid.Prop("QuantidadeInicio").Nome("Quantidade Inicio de Cargas Ganhas em Cotação").Tamanho(30);
                grid.Prop("QuantidadeFim").Nome("Quantidade Final de Cargas Ganhas em Cotação").Tamanho(30);
                grid.Prop("Pontuacao").Nome("Pontuação").Tamanho(30);

                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao repPontuacaoPorQuantidadeCargaGanhaCotacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao> pontuacaoPorQuantidadeCargaGanhaCotacaos = repPontuacaoPorQuantidadeCargaGanhaCotacao.BuscarTodas(grid.inicio, grid.limite);
                int totalRegistros = repPontuacaoPorQuantidadeCargaGanhaCotacao.ContarTodos();

                var lista = (from obj in pontuacaoPorQuantidadeCargaGanhaCotacaos
                             select new
                             {
                                 obj.Codigo,
                                 QuantidadeInicio = obj.QuantidadeInicio.ToString(),
                                 QuantidadeFim = obj.QuantidadeFim.ToString(),
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao repPontuacaoPorQuantidadeCargaGanhaCotacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao pontuacaoPorQuantidadeCargaGanhaCotacao = repPontuacaoPorQuantidadeCargaGanhaCotacao.BuscarPorCodigo(codigo, true);

                var dynPontuacao = new
                {
                    pontuacaoPorQuantidadeCargaGanhaCotacao.Codigo,
                    pontuacaoPorQuantidadeCargaGanhaCotacao.QuantidadeInicio,
                    pontuacaoPorQuantidadeCargaGanhaCotacao.QuantidadeFim,
                    pontuacaoPorQuantidadeCargaGanhaCotacao.Pontuacao
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao repPontuacaoPorQuantidadeCargaGanhaCotacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao(unitOfWork);

                int.TryParse(Request.Params("QuantidadeInicio"), out int quantidadeInicio);
                int.TryParse(Request.Params("QuantidadeFim"), out int quantidadeFim);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao pontuacaoPorQuantidadeCargaGanhaCotacao = new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao();
                pontuacaoPorQuantidadeCargaGanhaCotacao.QuantidadeInicio = quantidadeInicio;
                pontuacaoPorQuantidadeCargaGanhaCotacao.QuantidadeFim = quantidadeFim;
                pontuacaoPorQuantidadeCargaGanhaCotacao.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao pontuacaoPorQuantidadeCargaGanhaCotacaoExiste = repPontuacaoPorQuantidadeCargaGanhaCotacao.VerificarExisteEntrePeriodo(quantidadeInicio, quantidadeFim);

                if (quantidadeInicio > quantidadeFim)
                    return new JsonpResult(false, true, "A quantidade inicial não deve ser maior que o final.");

                if (pontuacaoPorQuantidadeCargaGanhaCotacaoExiste == null)
                {
                    repPontuacaoPorQuantidadeCargaGanhaCotacao.Inserir(pontuacaoPorQuantidadeCargaGanhaCotacao, Auditado);
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao repPontuacaoPorQuantidadeCargaGanhaCotacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao(unitOfWork);

                int.TryParse(Request.Params("QuantidadeInicio"), out int quantidadeInicio);
                int.TryParse(Request.Params("QuantidadeFim"), out int quantidadeFim);
                int.TryParse(Request.Params("Pontuacao"), out int pontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao pontuacaoPorQuantidadeCargaGanhaCotacao = repPontuacaoPorQuantidadeCargaGanhaCotacao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                pontuacaoPorQuantidadeCargaGanhaCotacao.QuantidadeInicio = quantidadeInicio;
                pontuacaoPorQuantidadeCargaGanhaCotacao.QuantidadeFim = quantidadeFim;
                pontuacaoPorQuantidadeCargaGanhaCotacao.Pontuacao = pontuacao;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao pontuacaoPorQuantidadeCargaGanhaCotacaoExiste = repPontuacaoPorQuantidadeCargaGanhaCotacao.VerificarExisteEntrePeriodo(quantidadeInicio, quantidadeFim);

                if (quantidadeInicio > quantidadeFim)
                    return new JsonpResult(false, true, "A quantidade inicial não deve ser maior que o final.");

                if (pontuacaoPorQuantidadeCargaGanhaCotacaoExiste == null || pontuacaoPorQuantidadeCargaGanhaCotacao.Codigo == pontuacaoPorQuantidadeCargaGanhaCotacaoExiste.Codigo)
                {
                    repPontuacaoPorQuantidadeCargaGanhaCotacao.Atualizar(pontuacaoPorQuantidadeCargaGanhaCotacao, Auditado);
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao repPontuacaoPorQuantidadeCargaGanhaCotacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao(unitOfWork);

                int codigo = int.Parse(Request.Params("codigo"));

                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao pontuacaoPorQuantidadeCargaGanhaCotacao = repPontuacaoPorQuantidadeCargaGanhaCotacao.BuscarPorCodigo(codigo);

                unitOfWork.Start();

                repPontuacaoPorQuantidadeCargaGanhaCotacao.Deletar(pontuacaoPorQuantidadeCargaGanhaCotacao, Auditado);

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
