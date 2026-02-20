using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/JanelaCarregamentoSituacao")]
    public class JanelaCarregamentoSituacaoController : BaseController
    {
		#region Construtores

		public JanelaCarregamentoSituacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao janelaCarregamentoSituacao = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao();

                PreencherJanelaCarregamentoSituacao(janelaCarregamentoSituacao);
                ValidarJanelaCarregamentoSituacaoDuplicada(janelaCarregamentoSituacao, unitOfWork);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unitOfWork);

                repositorio.Inserir(janelaCarregamentoSituacao, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao janelaCarregamentoSituacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (janelaCarregamentoSituacao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherJanelaCarregamentoSituacao(janelaCarregamentoSituacao);
                ValidarJanelaCarregamentoSituacaoDuplicada(janelaCarregamentoSituacao, unitOfWork);

                repositorio.Atualizar(janelaCarregamentoSituacao, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao janelaCarregamentoSituacao = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (janelaCarregamentoSituacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    janelaCarregamentoSituacao.Codigo,
                    janelaCarregamentoSituacao.Cor,
                    janelaCarregamentoSituacao.Descricao,
                    janelaCarregamentoSituacao.Situacao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao janelaCarregamentoSituacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (janelaCarregamentoSituacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                repositorio.Deletar(janelaCarregamentoSituacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRemover);
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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherJanelaCarregamentoSituacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao janelaCarregamentoSituacao)
        {
            janelaCarregamentoSituacao.Cor = Request.GetStringParam("Cor");
            janelaCarregamentoSituacao.Descricao = Request.GetStringParam("Descricao");
            janelaCarregamentoSituacao.Situacao = Request.GetEnumParam<SituacaoCargaJanelaCarregamentoAdicional>("Situacao");
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
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 30, Models.Grid.Align.center, true);

                string descricao = Request.GetStringParam("Descricao");
                SituacaoCargaJanelaCarregamentoAdicional? situacao = Request.GetNullableEnumParam<SituacaoCargaJanelaCarregamentoAdicional>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, situacao);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> listaJanelaCarregamentoSituacao = (totalRegistros > 0) ? repositorio.Consultar(descricao, situacao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao>();

                var listaJanelaCarregamentoSituacaoRetornar = (
                    from janelaCarregamentoSituacao in listaJanelaCarregamentoSituacao
                    select new
                    {
                        janelaCarregamentoSituacao.Codigo,
                        janelaCarregamentoSituacao.Descricao,
                        Situacao = janelaCarregamentoSituacao.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaJanelaCarregamentoSituacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private void ValidarJanelaCarregamentoSituacaoDuplicada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao janelaCarregamentoSituacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unitOfWork);

            if (repositorio.ExistePorSituacao(janelaCarregamentoSituacao.Situacao, janelaCarregamentoSituacao.Codigo))
                throw new ControllerException(Localization.Resources.Gerais.Geral.JaExisteCadastroSituacaoInformada);
        }

        #endregion
    }
}
