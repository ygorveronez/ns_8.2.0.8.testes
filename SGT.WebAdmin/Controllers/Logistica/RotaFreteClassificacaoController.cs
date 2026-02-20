using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/RotaFreteClassificacao")]
    public class RotaFreteClassificacaoController : BaseController
    {
		#region Construtores

		public RotaFreteClassificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao rotaFreteClassificacao = new Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao();

                PreencherRotaFreteClassificacao(rotaFreteClassificacao);

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.RotaFreteClassificacao repositorio = new Repositorio.Embarcador.Logistica.RotaFreteClassificacao(unitOfWork);

                repositorio.Inserir(rotaFreteClassificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.Logistica.RotaFreteClassificacao repositorio = new Repositorio.Embarcador.Logistica.RotaFreteClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao rotaFreteClassificacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (rotaFreteClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRotaFreteClassificacao(rotaFreteClassificacao);

                unitOfWork.Start();

                repositorio.Atualizar(rotaFreteClassificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Logistica.RotaFreteClassificacao repositorio = new Repositorio.Embarcador.Logistica.RotaFreteClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao rotaFreteClassificacao = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (rotaFreteClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    rotaFreteClassificacao.Codigo,
                    rotaFreteClassificacao.Descricao,
                    rotaFreteClassificacao.Classe
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
                Repositorio.Embarcador.Logistica.RotaFreteClassificacao repositorio = new Repositorio.Embarcador.Logistica.RotaFreteClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao rotaFreteClassificacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (rotaFreteClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(rotaFreteClassificacao, Auditado);

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

        private void PreencherRotaFreteClassificacao(Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao rotaFreteClassificacao)
        {
            rotaFreteClassificacao.Classe = Request.GetEnumParam<RotaFreteClasse>("Classe");
            rotaFreteClassificacao.Descricao = Request.GetStringParam("Descricao");
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
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Classe", "Classe", 15, Models.Grid.Align.left, true);

                string descricao = Request.GetStringParam("Descricao");
                RotaFreteClasse? classe = Request.GetNullableEnumParam<RotaFreteClasse>("Classe");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.RotaFreteClassificacao repositorio = new Repositorio.Embarcador.Logistica.RotaFreteClassificacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, classe);
                List<Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao> listaRotaFreteClassificacao = (totalRegistros > 0) ? repositorio.Consultar(descricao, classe, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao>();

                var listaRotaFreteClassificacaoRetornar = (
                    from classificacao in listaRotaFreteClassificacao
                    select new
                    {
                        classificacao.Codigo,
                        classificacao.Descricao,
                        Classe = classificacao.Classe.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaRotaFreteClassificacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        #endregion
    }
}
