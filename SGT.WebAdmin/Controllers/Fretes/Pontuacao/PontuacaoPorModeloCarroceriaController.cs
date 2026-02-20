using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TabelaPontuacao")]
    public class PontuacaoPorModeloCarroceriaController : BaseController
    {
		#region Construtores

		public PontuacaoPorModeloCarroceriaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria repositorioPontuacaoPorModeloCarroceria = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria pontuacaoPorModeloCarroceria = new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria();

                PreencherPontuacaoPorModeloCarroceria(pontuacaoPorModeloCarroceria, unitOfWork);

                if (repositorioPontuacaoPorModeloCarroceria.VerificarExistePorModeloCarroceria(pontuacaoPorModeloCarroceria.ModeloCarroceria.Codigo))
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para esse modelo de carroceria.");

                repositorioPontuacaoPorModeloCarroceria.Inserir(pontuacaoPorModeloCarroceria, Auditado);

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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria repositorioPontuacaoPorModeloCarroceria = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria pontuacaoPorModeloCarroceria = repositorioPontuacaoPorModeloCarroceria.BuscarPorCodigo(codigo, true);

                PreencherPontuacaoPorModeloCarroceria(pontuacaoPorModeloCarroceria, unitOfWork);

                if (repositorioPontuacaoPorModeloCarroceria.VerificarExistePorModeloCarroceria(pontuacaoPorModeloCarroceria.ModeloCarroceria.Codigo, codigo))
                    return new JsonpResult(false, true, "Já existe uma pontuação cadastrada para esse modelo de carroceria.");

                repositorioPontuacaoPorModeloCarroceria.Atualizar(pontuacaoPorModeloCarroceria, Auditado);

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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria repositorioPontuacaoPorModeloCarroceria = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria pontuacaoPorModeloCarroceria = repositorioPontuacaoPorModeloCarroceria.BuscarPorCodigo(codigo, true);

                if (pontuacaoPorModeloCarroceria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pontuacaoPorModeloCarroceria.Codigo,
                    ModeloCarroceria = new { pontuacaoPorModeloCarroceria.ModeloCarroceria.Codigo, pontuacaoPorModeloCarroceria.ModeloCarroceria.Descricao },
                    pontuacaoPorModeloCarroceria.Pontuacao
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
                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria repositorioPontuacaoPorModeloCarroceria = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria pontuacaoPorModeloCarroceria = repositorioPontuacaoPorModeloCarroceria.BuscarPorCodigo(codigo, auditavel: true);

                if (pontuacaoPorModeloCarroceria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioPontuacaoPorModeloCarroceria.Deletar(pontuacaoPorModeloCarroceria, Auditado);

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
                grid.Prop("ModeloCarroceria").Nome("Modelo de Carroceria").Tamanho(60);
                grid.Prop("Pontuacao").Nome("Pontuação").Tamanho(30);

                Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria repositorioPontuacaoPorModeloCarroceria = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria(unitOfWork);
                int totalRegistros = repositorioPontuacaoPorModeloCarroceria.ContarTodos();
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria> listaPontuacaoPorModeloCarroceria = (totalRegistros > 0) ? repositorioPontuacaoPorModeloCarroceria.BuscarTodas(grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria>();

                var listaPontuacaoPorModeloCarroceriaRetornar = (
                    from o in listaPontuacaoPorModeloCarroceria
                    select new
                    {
                        o.Codigo,
                        ModeloCarroceria = o.ModeloCarroceria.Descricao,
                        o.Pontuacao
                    }
                ).ToList();

                grid.AdicionaRows(listaPontuacaoPorModeloCarroceriaRetornar);
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

        private void PreencherPontuacaoPorModeloCarroceria(Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria pontuacaoPorModeloCarroceria, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoModeloCarroceria = Request.GetIntParam("ModeloCarroceria");
            Repositorio.Embarcador.Veiculos.ModeloCarroceria repositorioModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unitOfWork);

            pontuacaoPorModeloCarroceria.ModeloCarroceria = repositorioModeloCarroceria.BuscarPorCodigo(codigoModeloCarroceria) ?? throw new ControllerException("O modelo de carroceria deve ser informado.");
            pontuacaoPorModeloCarroceria.Pontuacao = Request.GetIntParam("Pontuacao");
        }

        #endregion
    }
}
