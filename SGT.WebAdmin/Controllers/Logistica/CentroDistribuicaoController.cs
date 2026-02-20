using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CentroDistribuicao")]
    public class CentroDistribuicaoController : BaseController
    {
		#region Construtores

		public CentroDistribuicaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.CentroDistribuicao repositorioCentroDistribuicao = new Repositorio.Embarcador.Logistica.CentroDistribuicao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroDistribuicao filtrosPesquisa = ObterFiltrosPesquisaCentroDistribuicao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioCentroDistribuicao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao> listaCentroDistribuicao = totalRegistros > 0 ? repositorioCentroDistribuicao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao>();

                grid.AdicionaRows((
                    from o in listaCentroDistribuicao
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.CodigoIntegracao,
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
                Repositorio.Embarcador.Logistica.CentroDistribuicao repositorioCentroDistribuicao = new Repositorio.Embarcador.Logistica.CentroDistribuicao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao centroDistribuicao = repositorioCentroDistribuicao.BuscarPorCodigo(codigo, true);

                if (centroDistribuicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(centroDistribuicao, unitOfWork);

                // Persiste dados
                unitOfWork.Start();
                repositorioCentroDistribuicao.Atualizar(centroDistribuicao, Auditado);
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
                Repositorio.Embarcador.Logistica.CentroDistribuicao repCentroDistribuicao = new Repositorio.Embarcador.Logistica.CentroDistribuicao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao centroDistribuicao = new Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao();

                PreencherEntidade(centroDistribuicao, unitOfWork);

                unitOfWork.Start();
                repCentroDistribuicao.Inserir(centroDistribuicao, Auditado);
                unitOfWork.CommitChanges();

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
                Repositorio.Embarcador.Logistica.CentroDistribuicao repCentroDistribuicao = new Repositorio.Embarcador.Logistica.CentroDistribuicao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao centroDistribuicao = repCentroDistribuicao.BuscarPorCodigo(codigo);

                if (centroDistribuicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repCentroDistribuicao.Deletar(centroDistribuicao, Auditado);
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

                Repositorio.Embarcador.Logistica.CentroDistribuicao repCentroDistribuicao = new Repositorio.Embarcador.Logistica.CentroDistribuicao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao dadosCentroDistribuicao = repCentroDistribuicao.BuscarPorCodigo(codigo, true);

                if (dadosCentroDistribuicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                dynamic dynPedido = new
                {
                    Descricao = dadosCentroDistribuicao.Descricao,
                    CodigoIntegracao = dadosCentroDistribuicao.CodigoIntegracao,
                    Situacao = dadosCentroDistribuicao.Situacao
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroDistribuicao ObterFiltrosPesquisaCentroDistribuicao()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCentroDistribuicao()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetBoolParam("Situacao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao centroDistribuicao, Repositorio.UnitOfWork unitOfWork)
        {
            centroDistribuicao.Situacao = Request.GetBoolParam("Situacao");
            centroDistribuicao.Descricao = Request.GetStringParam("Descricao");
            centroDistribuicao.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
        }

        #endregion
    }

}

