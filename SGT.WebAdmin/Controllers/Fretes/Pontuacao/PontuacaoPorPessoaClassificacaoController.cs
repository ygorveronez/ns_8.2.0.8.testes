using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TabelaPontuacao")]
    public class PontuacaoPorPessoaClassificacaoController : BaseController
    {
		#region Construtores

		public PontuacaoPorPessoaClassificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao repositorioPontuacaoPorPessoaClassificacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao pontuacaoPorPessoaClassificacao = new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao();

                PreencherPontuacaoPorPessoaClassificacao(pontuacaoPorPessoaClassificacao, unitOfWork);

                if (repositorioPontuacaoPorPessoaClassificacao.VerificarExistePorPessoaClassificacao(pontuacaoPorPessoaClassificacao.PessoaClassificacao.Codigo))
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para essa classificação de cliente.");

                repositorioPontuacaoPorPessoaClassificacao.Inserir(pontuacaoPorPessoaClassificacao, Auditado);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao repositorioPontuacaoPorPessoaClassificacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao pontuacaoPorPessoaClassificacao = repositorioPontuacaoPorPessoaClassificacao.BuscarPorCodigo(codigo, true);

                PreencherPontuacaoPorPessoaClassificacao(pontuacaoPorPessoaClassificacao, unitOfWork);

                if (repositorioPontuacaoPorPessoaClassificacao.VerificarExistePorPessoaClassificacao(pontuacaoPorPessoaClassificacao.PessoaClassificacao.Codigo, codigo))
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para essa classificação de cliente.");

                repositorioPontuacaoPorPessoaClassificacao.Atualizar(pontuacaoPorPessoaClassificacao, Auditado);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao repositorioPontuacaoPorPessoaClassificacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao pontuacaoPorPessoaClassificacao = repositorioPontuacaoPorPessoaClassificacao.BuscarPorCodigo(codigo, true);

                if (pontuacaoPorPessoaClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pontuacaoPorPessoaClassificacao.Codigo,
                    PessoaClassificacao = new { pontuacaoPorPessoaClassificacao.PessoaClassificacao.Codigo, pontuacaoPorPessoaClassificacao.PessoaClassificacao.Descricao },
                    pontuacaoPorPessoaClassificacao.Pontuacao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                int codigo = Request.GetIntParam("codigo");
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao repositorioPontuacaoPorPessoaClassificacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao pontuacaoPorPessoaClassificacao = repositorioPontuacaoPorPessoaClassificacao.BuscarPorCodigo(codigo, auditavel: true);

                if (pontuacaoPorPessoaClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioPontuacaoPorPessoaClassificacao.Deletar(pontuacaoPorPessoaClassificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("PessoaClassificacao").Nome("Classificação de Cliente").Tamanho(60);
                grid.Prop("Pontuacao").Nome("Pontuação").Tamanho(30);

                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao repositorioPontuacaoPorPessoaClassificacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao(unitOfWork);
                int totalRegistros = repositorioPontuacaoPorPessoaClassificacao.ContarTodos();
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao> listaPontuacaoPorPessoaClassificacao = (totalRegistros > 0) ? repositorioPontuacaoPorPessoaClassificacao.BuscarTodas(grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao>();

                var listaPontuacaoPorPessoaClassificacaoRetornar = (
                    from o in listaPontuacaoPorPessoaClassificacao
                    select new
                    {
                        o.Codigo,
                        PessoaClassificacao = o.PessoaClassificacao.Descricao,
                        o.Pontuacao
                    }
                ).ToList();

                grid.AdicionaRows(listaPontuacaoPorPessoaClassificacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherPontuacaoPorPessoaClassificacao(Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao pontuacaoPorPessoaClassificacao, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoPessoaClassificacao = Request.GetIntParam("PessoaClassificacao");
            Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorioPessoaClassificacao = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(unitOfWork);

            pontuacaoPorPessoaClassificacao.PessoaClassificacao = repositorioPessoaClassificacao.BuscarPorCodigo(codigoPessoaClassificacao, auditavel: false) ?? throw new ControllerException("A classificação de cliente deve ser informada.");
            pontuacaoPorPessoaClassificacao.Pontuacao = Request.GetIntParam("Pontuacao");
        }

        #endregion
    }
}
