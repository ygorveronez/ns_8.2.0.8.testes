using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/JanelaDescargaSituacao")]
    public class JanelaDescargaSituacaoController : BaseController
    {
		#region Construtores

		public JanelaDescargaSituacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao janelaDescarregamentoSituacao = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao();

                PreencherJanelaDescarregamentoSituacao(janelaDescarregamentoSituacao);
                ValidarJanelaDescarregamentoSituacaoDuplicada(janelaDescarregamentoSituacao, unitOfWork);

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);

                repositorio.Inserir(janelaDescarregamentoSituacao, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao janelaDescarregamentoSituacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (janelaDescarregamentoSituacao == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                PreencherJanelaDescarregamentoSituacao(janelaDescarregamentoSituacao);
                ValidarJanelaDescarregamentoSituacaoDuplicada(janelaDescarregamentoSituacao, unitOfWork);

                repositorio.Atualizar(janelaDescarregamentoSituacao, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
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
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao janelaDescarregamentoSituacao = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (janelaDescarregamentoSituacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    janelaDescarregamentoSituacao.Codigo,
                    janelaDescarregamentoSituacao.Cor,
                    janelaDescarregamentoSituacao.Descricao,
                    janelaDescarregamentoSituacao.Situacao
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao janelaDescarregamentoSituacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (janelaDescarregamentoSituacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(janelaDescarregamentoSituacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherJanelaDescarregamentoSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao janelaDescarregamentoSituacao)
        {
            janelaDescarregamentoSituacao.Cor = Request.GetStringParam("Cor");
            janelaDescarregamentoSituacao.Descricao = Request.GetStringParam("Descricao");
            janelaDescarregamentoSituacao.Situacao = Request.GetEnumParam<SituacaoCargaJanelaDescarregamentoAdicional>("Situacao");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.center, true);

                string descricao = Request.GetStringParam("Descricao");
                SituacaoCargaJanelaDescarregamentoAdicional? situacao = Request.GetNullableEnumParam<SituacaoCargaJanelaDescarregamentoAdicional>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, situacao);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> listaJanelaDescarregamentoSituacao = (totalRegistros > 0) ? repositorio.Consultar(descricao, situacao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao>();

                var listaJanelaDescarregamentoSituacaoRetornar = (
                    from janelaDescarregamentoSituacao in listaJanelaDescarregamentoSituacao
                    select new
                    {
                        janelaDescarregamentoSituacao.Codigo,
                        janelaDescarregamentoSituacao.Descricao,
                        Situacao = janelaDescarregamentoSituacao.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaJanelaDescarregamentoSituacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private void ValidarJanelaDescarregamentoSituacaoDuplicada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao janelaDescarregamentoSituacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);

            if (repositorio.ExistePorSituacao(janelaDescarregamentoSituacao.Situacao, janelaDescarregamentoSituacao.Codigo))
                throw new ControllerException("Já existe um cadastro para a situação informada.");
        }

        #endregion
    }
}
