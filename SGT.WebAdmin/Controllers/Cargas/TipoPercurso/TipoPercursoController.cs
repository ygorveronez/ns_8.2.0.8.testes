using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.TipoPercurso
{
    public class TipoPercursoController : BaseController
    {
		#region Construtores

		public TipoPercursoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.TipoPercurso repTipoPercurso = new Repositorio.Embarcador.Cargas.TipoPercurso(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoPercurso tipoPercurso = new Dominio.Entidades.Embarcador.Cargas.TipoPercurso();

                PreencherTipoPercurso(tipoPercurso);

                unitOfWork.Start();
                repTipoPercurso.Inserir(tipoPercurso, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Tipo de Percurso Adicionado Com Sucesso");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Não Foi Possivel Adicionar Tipo De Percurso");
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
                Repositorio.Embarcador.Cargas.TipoPercurso repTipoPercurso = new Repositorio.Embarcador.Cargas.TipoPercurso(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoPercurso tipoPercurso = repTipoPercurso.BuscarPorCodigo(codigo);

                if (tipoPercurso == null)
                    return new JsonpResult(false, true, "Tipo de Percurso Não Encontrado");

                PreencherTipoPercurso(tipoPercurso);

                unitOfWork.Start();
                repTipoPercurso.Atualizar(tipoPercurso);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Cargas.TipoPercurso repTipoPercurso = new Repositorio.Embarcador.Cargas.TipoPercurso(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoPercurso tipoPercurso = repTipoPercurso.BuscarPorCodigo(codigo);

                if (tipoPercurso == null)
                    return new JsonpResult(false, true, "Tipo de Percurso Não Encontrado");

                return new JsonpResult(new
                {
                    tipoPercurso.Codigo,
                    tipoPercurso.Descricao,
                    tipoPercurso.Vazio,
                    tipoPercurso.CodigoIntegracao,
                    TipoPercurso = tipoPercurso.TipoPercursoValor
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
                Repositorio.Embarcador.Cargas.TipoPercurso repTipoPercurso = new Repositorio.Embarcador.Cargas.TipoPercurso(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoPercurso tipoPercurso = repTipoPercurso.BuscarPorCodigo(codigo);
               
                if (tipoPercurso == null)
                    return new JsonpResult(false, true, "Tipo de Percurso Não Encontrado");

                unitOfWork.Start();
                repTipoPercurso.Deletar(tipoPercurso, Auditado);

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

        #region Metodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoPercurso repTipoPercurso = new Repositorio.Embarcador.Cargas.TipoPercurso(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoPercurso filtroPesquisa = ObterFiltrosPesquisa();

                var grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Codigo Integração", "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vazio", "Vazio", 10, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta();
                parametroConsulta.DirecaoOrdenar = "acs";
                parametroConsulta.PropriedadeOrdenar = "Codigo";

                var listaTipoPercurso = repTipoPercurso.Consultar(filtroPesquisa, parametroConsulta);
                var totalRegistros = repTipoPercurso.ContarConsulta(filtroPesquisa);

                var listaTipoPercursoRetornar = (
                    from tipoPercurso in listaTipoPercurso
                    select new
                    {
                        tipoPercurso.Codigo,
                        tipoPercurso.Descricao,
                        tipoPercurso.CodigoIntegracao,
                        Vazio = tipoPercurso.Vazio.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaTipoPercursoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void PreencherTipoPercurso(Dominio.Entidades.Embarcador.Cargas.TipoPercurso tipoPercurso)
        {
            tipoPercurso.Descricao = Request.GetStringParam("Descricao");
            tipoPercurso.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoPercurso.TipoPercursoValor = Request.GetStringParam("TipoPercurso");
            tipoPercurso.Vazio = Request.GetEnumParam<Vazio>("Vazio");
        }

        private FiltroPesquisaTipoPercurso ObterFiltrosPesquisa()
        {
            return new FiltroPesquisaTipoPercurso()
            {
                Vazio = Request.GetNullableEnumParam<Vazio>("Vazio"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Descricao = Request.GetStringParam("Descricao")
            };
        }

        #endregion
    }
}
