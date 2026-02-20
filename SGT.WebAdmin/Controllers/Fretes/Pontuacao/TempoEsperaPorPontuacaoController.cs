using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/TempoEsperaPorPontuacao")]
    public class TempoEsperaPorPontuacaoController : BaseController
    {
		#region Construtores

		public TempoEsperaPorPontuacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao repositorioTempoEsperaPorPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao tempoEsperaPorPontuacao = new Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao();

                unitOfWork.Start();

                PreencherTempoEsperaPorPontuacao(tempoEsperaPorPontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao tempoEsperaPorPontuacaoDuplicado = repositorioTempoEsperaPorPontuacao.BuscarPorIntervaloPontuacaoDuplicado(tempoEsperaPorPontuacao.PontuacaoInicial, tempoEsperaPorPontuacao.PontuacaoFinal);

                if (tempoEsperaPorPontuacaoDuplicado != null)
                    throw new ControllerException("Já existe um tempo de espera cadastrado contendo o intervalo de pontuação informado.");

                repositorioTempoEsperaPorPontuacao.Inserir(tempoEsperaPorPontuacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
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
                Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao repositorioTempoEsperaPorPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao tempoEsperaPorPontuacao = repositorioTempoEsperaPorPontuacao.BuscarPorCodigo(codigo, auditavel: true);

                if (tempoEsperaPorPontuacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                PreencherTempoEsperaPorPontuacao(tempoEsperaPorPontuacao);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao tempoEsperaPorPontuacaoDuplicado = repositorioTempoEsperaPorPontuacao.BuscarPorIntervaloPontuacaoDuplicado(tempoEsperaPorPontuacao.PontuacaoInicial, tempoEsperaPorPontuacao.PontuacaoFinal);

                if (tempoEsperaPorPontuacaoDuplicado?.Codigo != tempoEsperaPorPontuacao.Codigo)
                    throw new ControllerException("Já existe um tempo de espera cadastrado contendo o intervalo de pontuação informado.");

                repositorioTempoEsperaPorPontuacao.Atualizar(tempoEsperaPorPontuacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
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
                Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao repositorioTempoEsperaPorPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao tempoEsperaPorPontuacao = repositorioTempoEsperaPorPontuacao.BuscarPorCodigo(codigo, true);

                if (tempoEsperaPorPontuacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tempoEsperaPorPontuacao.Codigo,
                    tempoEsperaPorPontuacao.PontuacaoInicial,
                    tempoEsperaPorPontuacao.PontuacaoFinal,
                    tempoEsperaPorPontuacao.TempoEsperaEmMinutos
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao repositorioTempoEsperaPorPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao tempoEsperaPorPontuacao = repositorioTempoEsperaPorPontuacao.BuscarPorCodigo(codigo, auditavel: true);

                if (tempoEsperaPorPontuacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioTempoEsperaPorPontuacao.Deletar(tempoEsperaPorPontuacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
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
                grid.Prop("PontuacaoInicial").Nome("Pontuação Inicial").Tamanho(30);
                grid.Prop("PontuacaoFinal").Nome("Pontuação Final").Tamanho(30);
                grid.Prop("TempoEsperaEmMinutos").Nome("Tempo de Espera").Tamanho(30);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta((propriedadeOrdenar) => { return "PontuacaoInicial"; });
                Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao repositorioTempoEsperaPorPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao(unitOfWork);
                int totalRegistros = repositorioTempoEsperaPorPontuacao.ContarConsulta();
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> listaTempoEsperaPorPontuacao = (totalRegistros > 0) ? repositorioTempoEsperaPorPontuacao.Consultar(parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao>();

                var listaTempoEsperaPorPontuacaoRetornar = (from obj in listaTempoEsperaPorPontuacao
                                                            select new
                                                            {
                                                                obj.Codigo,
                                                                obj.PontuacaoInicial,
                                                                obj.PontuacaoFinal,
                                                                obj.TempoEsperaEmMinutos
                                                            }
                ).ToList();

                grid.AdicionaRows(listaTempoEsperaPorPontuacaoRetornar);
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

        private void PreencherTempoEsperaPorPontuacao(Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao tempoEsperaPorPontuacao)
        {
            tempoEsperaPorPontuacao.PontuacaoFinal = Request.GetIntParam("PontuacaoFinal");
            tempoEsperaPorPontuacao.PontuacaoInicial = Request.GetIntParam("PontuacaoInicial");
            tempoEsperaPorPontuacao.TempoEsperaEmMinutos = Request.GetIntParam("TempoEsperaEmMinutos");

            if (tempoEsperaPorPontuacao.PontuacaoFinal < tempoEsperaPorPontuacao.PontuacaoInicial)
                throw new ControllerException("A pontuação final deve ser maior que a inicial.");
        }

        #endregion
    }
}
