using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/OcorrenciaPatioTipo")]
    public class OcorrenciaPatioTipoController : BaseController
    {
		#region Construtores

		public OcorrenciaPatioTipoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ocorrenciaPatioTipo = new Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo();

                PreencherOcorrenciaPatioTipo(ocorrenciaPatioTipo);

                unitOfWork.Start();

                Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(unitOfWork);

                repositorio.Inserir(ocorrenciaPatioTipo, Auditado);

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
                Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ocorrenciaPatioTipo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (ocorrenciaPatioTipo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherOcorrenciaPatioTipo(ocorrenciaPatioTipo);

                unitOfWork.Start();

                repositorio.Atualizar(ocorrenciaPatioTipo, Auditado);

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
                Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ocorrenciaPatioTipo = repositorio.BuscarPorCodigo(codigo);

                if (ocorrenciaPatioTipo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    ocorrenciaPatioTipo.Codigo,
                    ocorrenciaPatioTipo.Descricao,
                    Status = ocorrenciaPatioTipo.Ativo,
                    ocorrenciaPatioTipo.Tipo
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
                Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ocorrenciaPatioTipo = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (ocorrenciaPatioTipo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(ocorrenciaPatioTipo, Auditado);

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
                var grid = ObterGridPesquisa();
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

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatioTipo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatioTipo()
            {
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                Tipo = Request.GetNullableEnumParam<TipoOcorrenciaPatio>("Tipo")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatioTipo filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.center, true);

                if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo> listaOcorrenciaPatioTipo = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo>();

                var listaOcorrenciaPatioTipoRetornar = (
                    from ocorrenciaPatioTipo in listaOcorrenciaPatioTipo
                    select new
                    {
                        ocorrenciaPatioTipo.Codigo,
                        ocorrenciaPatioTipo.Descricao,
                        ocorrenciaPatioTipo.DescricaoAtivo,
                        Tipo = ocorrenciaPatioTipo.Tipo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaOcorrenciaPatioTipoRetornar);
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

        private void PreencherOcorrenciaPatioTipo(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ocorrenciaPatioTipo)
        {
            ocorrenciaPatioTipo.Ativo = Request.GetBoolParam("Status");
            ocorrenciaPatioTipo.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException("Descrição é obrigatória.");
            ocorrenciaPatioTipo.Tipo = Request.GetEnumParam<TipoOcorrenciaPatio>("Tipo");
        }

        #endregion
    }
}
