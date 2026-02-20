using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Canhotos.ConfiguracaoReconhecimentoCanhoto
{
    [CustomAuthorize("Canhotos/ConfiguracaoReconhecimentoCanhoto")]
    public class ConfiguracaoReconhecimentoCanhotoController : BaseController
    {
		#region Construtores

		public ConfiguracaoReconhecimentoCanhotoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto repConfiguracaoReconhecimentoCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto configuracao = new Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto();
                PreencherEntidade(configuracao, unitOfWork);

                unitOfWork.Start();

                repConfiguracaoReconhecimentoCanhoto.Inserir(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto repConfiguracaoReconhecimentoCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto configuracao = repConfiguracaoReconhecimentoCanhoto.BuscarPorCodigo(codigo, true);
                PreencherEntidade(configuracao, unitOfWork);

                unitOfWork.Start();

                repConfiguracaoReconhecimentoCanhoto.Atualizar(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto repConfiguracaoReconhecimentoCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto configuracao = repConfiguracaoReconhecimentoCanhoto.BuscarPorCodigo(codigo, true);

                repConfiguracaoReconhecimentoCanhoto.Deletar(configuracao, Auditado);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto repConfiguracaoReconhecimentoCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto configuracao = repConfiguracaoReconhecimentoCanhoto.BuscarPorCodigo(codigo, false);

                if (configuracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    configuracao.Codigo,
                    configuracao.Descricao,
                    configuracao.Ativo,
                    configuracao.PalavrasChaves,
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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            configuracao.Descricao = Request.GetStringParam("Descricao");
            configuracao.PalavrasChaves = Request.GetStringParam("PalavrasChaves");
            configuracao.Ativo = Request.GetBoolParam("Ativo");
        }

        private (string Descricao, SituacaoAtivoPesquisa Situacao) ObterFiltrosPesquisa()
        {
            string descricao = Request.GetStringParam("Descricao");
            SituacaoAtivoPesquisa situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo");

            return ValueTuple.Create(descricao, situacao);
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
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Ativo", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                var filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto repConfiguracaoReconhecimentoCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto(unitOfWork);
                int totalRegistros = repConfiguracaoReconhecimentoCanhoto.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto> listaConfiguracaoReconhecimentoCanhoto = (totalRegistros > 0) ? repConfiguracaoReconhecimentoCanhoto.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto>();

                var retorno = listaConfiguracaoReconhecimentoCanhoto.Select(configuracao => new
                {
                    configuracao.Codigo,
                    configuracao.Descricao,
                    Ativo = configuracao.DescricaoAtivo,
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
