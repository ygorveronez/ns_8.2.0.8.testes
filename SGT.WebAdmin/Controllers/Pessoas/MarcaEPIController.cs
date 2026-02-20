using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/MarcaEPI")]
    public class MarcaEPIController : BaseController
    {
		#region Construtores

		public MarcaEPIController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaMarcaEPI filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Pessoas.MarcaEPI repMarcaEPI = new Repositorio.Embarcador.Pessoas.MarcaEPI(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Pessoas.MarcaEPI> marcasEPI = repMarcaEPI.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repMarcaEPI.ContarConsulta(filtrosPesquisa));

                var lista = (from p in marcasEPI
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

                Repositorio.Embarcador.Pessoas.MarcaEPI repMarcaEPI = new Repositorio.Embarcador.Pessoas.MarcaEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.MarcaEPI marcaEPI = new Dominio.Entidades.Embarcador.Pessoas.MarcaEPI();

                PreencherMarcaEPI(marcaEPI, unitOfWork);

                repMarcaEPI.Inserir(marcaEPI, Auditado);

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

                Repositorio.Embarcador.Pessoas.MarcaEPI repMarcaEPI = new Repositorio.Embarcador.Pessoas.MarcaEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.MarcaEPI marcaEPI = repMarcaEPI.BuscarPorCodigo(codigo, true);

                if (marcaEPI == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMarcaEPI(marcaEPI, unitOfWork);

                repMarcaEPI.Atualizar(marcaEPI, Auditado);

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

                Repositorio.Embarcador.Pessoas.MarcaEPI repMarcaEPI = new Repositorio.Embarcador.Pessoas.MarcaEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.MarcaEPI marcaEPI = repMarcaEPI.BuscarPorCodigo(codigo, false);

                if (marcaEPI == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynMarcaEPI = new
                {
                    marcaEPI.Codigo,
                    marcaEPI.Descricao,
                    marcaEPI.Ativo
                };

                return new JsonpResult(dynMarcaEPI);
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

                Repositorio.Embarcador.Pessoas.MarcaEPI repMarcaEPI = new Repositorio.Embarcador.Pessoas.MarcaEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.MarcaEPI marcaEPI = repMarcaEPI.BuscarPorCodigo(codigo, true);

                if (marcaEPI == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMarcaEPI.Deletar(marcaEPI, Auditado);

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

        #endregion

        #region Métodos Privados

        private void PreencherMarcaEPI(Dominio.Entidades.Embarcador.Pessoas.MarcaEPI marcaEPI, Repositorio.UnitOfWork unitOfWork)
        {
            marcaEPI.Descricao = Request.GetStringParam("Descricao");
            marcaEPI.Ativo = Request.GetBoolParam("Ativo");
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaMarcaEPI ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaMarcaEPI()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        #endregion
    }
}
