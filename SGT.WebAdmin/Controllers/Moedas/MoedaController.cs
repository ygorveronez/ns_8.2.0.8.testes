using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Moeda
{
    [CustomAuthorize("Moedas/Moeda", "Pedidos/Booking")]
    public class MoedaController : BaseController
    {
		#region Construtores

		public MoedaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Moedas.Moeda repositorioMoeda = new Repositorio.Embarcador.Moedas.Moeda(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Moedas.FiltroPesquisaMoeda filtrosPesquisa = ObterFiltrosPesquisaMoeda();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Moedas.Moeda.DescricaoMoeda, "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Moedas.Moeda.Sigla, "Sigla", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Moedas.Moeda.CodigoMoeda, "CodigoMoeda", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho(Localization.Resources.Moedas.Moeda.Situacao, "Situacao", 15, Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioMoeda.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Moedas.Moeda> listaMoeda = totalRegistros > 0 ? repositorioMoeda.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Moedas.Moeda>();

                grid.AdicionaRows((
                    from o in listaMoeda
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        Sigla = o.Simbolo,
                        o.CodigoMoeda,
                        Situacao = o.Situacao.ObterDescricaoAtivo()
                    }).ToList()
                );

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

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Moedas.Moeda repMoeda = new Repositorio.Embarcador.Moedas.Moeda(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Moedas.Moeda moeda = repMoeda.BuscarPorCodigo(codigo, true);

                // Valida
                if (moeda == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencherEntidade(moeda, unitOfWork);

                // Persiste dados
                unitOfWork.Start();
                repMoeda.Atualizar(moeda, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Moedas.Moeda repMoeda = new Repositorio.Embarcador.Moedas.Moeda(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Moedas.Moeda moeda = new Dominio.Entidades.Embarcador.Moedas.Moeda();

                // Preenche entidade com dados
                PreencherEntidade(moeda, unitOfWork);

                // Persiste dados
                unitOfWork.Start();
                repMoeda.Inserir(moeda, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Moedas.Moeda repMoeda = new Repositorio.Embarcador.Moedas.Moeda(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Moedas.Moeda moeda = repMoeda.BuscarPorCodigo(codigo);

                if (moeda == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repMoeda.Deletar(moeda, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Moedas.Moeda repMoeda = new Repositorio.Embarcador.Moedas.Moeda(unitOfWork);

                Dominio.Entidades.Embarcador.Moedas.Moeda dadosMoeda = repMoeda.BuscarPorCodigo(codigo, true);

                if (dadosMoeda == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                dynamic dynPedido = new
                {
                    Descricao = dadosMoeda.Descricao,
                    Sigla = dadosMoeda.Simbolo,
                    CodigoMoeda = dadosMoeda.CodigoMoeda,
                    Situacao = dadosMoeda.Situacao
                };

                return new JsonpResult(dynPedido);
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
        #endregion

        #region Metódos Privados

        private Dominio.ObjetosDeValor.Embarcador.Moedas.FiltroPesquisaMoeda ObterFiltrosPesquisaMoeda()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Moedas.FiltroPesquisaMoeda()
            {
                DescricaoMoeda = Request.GetStringParam("Descricao"),
                Situacao = Request.GetBoolParam("Situacao"),
                CodigoMoeda = Request.GetIntParam("CodigoMoeda"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Moedas.Moeda moeda, Repositorio.UnitOfWork unitOfWork)
        {
            moeda.Situacao = Request.GetBoolParam("Situacao");
            moeda.Descricao = Request.GetStringParam("Descricao");
            moeda.Simbolo = Request.GetStringParam("Sigla");
            moeda.CodigoMoeda = Request.GetIntParam("CodigoMoeda");
        }

        #endregion
    }

}

