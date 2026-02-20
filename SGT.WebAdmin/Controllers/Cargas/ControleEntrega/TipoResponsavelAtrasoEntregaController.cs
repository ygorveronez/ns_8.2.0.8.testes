using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/TipoResponsavelAtrasoEntrega")]
    public class TipoResponsavelAtrasoEntregaController : BaseController
    {
		#region Construtores

		public TipoResponsavelAtrasoEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaTipoResponsavelAtrasoEntrega filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repTipoResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega> tipoResponsavelAtrasoEntregas = repTipoResponsavelAtrasoEntrega.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repTipoResponsavelAtrasoEntrega.ContarConsulta(filtrosPesquisa));

                var lista = (from p in tipoResponsavelAtrasoEntregas
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repTipoResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega tipoResponsavelAtrasoEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega();

                PreencherTipoResponsavelAtrasoEntrega(tipoResponsavelAtrasoEntrega, unitOfWork);

                repTipoResponsavelAtrasoEntrega.Inserir(tipoResponsavelAtrasoEntrega, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repTipoResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega tipoResponsavelAtrasoEntrega = repTipoResponsavelAtrasoEntrega.BuscarPorCodigo(codigo, true);

                if (tipoResponsavelAtrasoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTipoResponsavelAtrasoEntrega(tipoResponsavelAtrasoEntrega, unitOfWork);

                repTipoResponsavelAtrasoEntrega.Atualizar(tipoResponsavelAtrasoEntrega, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repTipoResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega tipoResponsavelAtrasoEntrega = repTipoResponsavelAtrasoEntrega.BuscarPorCodigo(codigo, false);

                if (tipoResponsavelAtrasoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynTipoResponsavelAtrasoEntrega = new
                {
                    tipoResponsavelAtrasoEntrega.Codigo,
                    tipoResponsavelAtrasoEntrega.Descricao,
                    tipoResponsavelAtrasoEntrega.Observacao,
                    tipoResponsavelAtrasoEntrega.Ativo
                };

                return new JsonpResult(dynTipoResponsavelAtrasoEntrega);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repTipoResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega tipoResponsavelAtrasoEntrega = repTipoResponsavelAtrasoEntrega.BuscarPorCodigo(codigo, true);

                if (tipoResponsavelAtrasoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTipoResponsavelAtrasoEntrega.Deletar(tipoResponsavelAtrasoEntrega, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTiposResponsavelAtrasoAtivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaTipoResponsavelAtrasoEntrega filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaTipoResponsavelAtrasoEntrega();
                filtrosPesquisa.Ativo = SituacaoAtivoPesquisa.Ativo;

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repTipoResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega> tipoResponsavelAtrasoEntregas = repTipoResponsavelAtrasoEntrega.Consultar(filtrosPesquisa, null);

                return new JsonpResult(new
                {
                    TipoResponsavel = from tipo in tipoResponsavelAtrasoEntregas
                                      select new
                                      {
                                          text = tipo.Descricao,
                                          value = tipo.Codigo
                                      }
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

        #endregion

        #region Métodos Privados

        private void PreencherTipoResponsavelAtrasoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega tipoResponsavelAtrasoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            tipoResponsavelAtrasoEntrega.Descricao = Request.GetStringParam("Descricao");
            tipoResponsavelAtrasoEntrega.Ativo = Request.GetBoolParam("Ativo");
            tipoResponsavelAtrasoEntrega.Observacao = Request.GetStringParam("Observacao");
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaTipoResponsavelAtrasoEntrega ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaTipoResponsavelAtrasoEntrega()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        #endregion
    }
}

