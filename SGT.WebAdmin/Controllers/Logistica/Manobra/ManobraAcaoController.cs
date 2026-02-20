using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ManobraAcao")]
    public class ManobraAcaoController : BaseController
    {
		#region Construtores

		public ManobraAcaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.ManobraAcao manobraAcao = new Dominio.Entidades.Embarcador.Logistica.ManobraAcao();

                PreencherManobraAcao(manobraAcao, unitOfWork);

                unitOfWork.Start();

                new Repositorio.Embarcador.Logistica.ManobraAcao(unitOfWork).Inserir(manobraAcao, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ManobraAcao repositorio = new Repositorio.Embarcador.Logistica.ManobraAcao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ManobraAcao manobraAcao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (manobraAcao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherManobraAcao(manobraAcao, unitOfWork);

                unitOfWork.Start();

                repositorio.Atualizar(manobraAcao, Auditado);

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
                Repositorio.Embarcador.Logistica.ManobraAcao repositorio = new Repositorio.Embarcador.Logistica.ManobraAcao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ManobraAcao manobraAcao = repositorio.BuscarPorCodigo(codigo);

                if (manobraAcao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    manobraAcao.Codigo,
                    manobraAcao.Descricao,
                    manobraAcao.LocalDestinoObrigatorio,
                    manobraAcao.Tipo
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
                Repositorio.Embarcador.Logistica.ManobraAcao repositorio = new Repositorio.Embarcador.Logistica.ManobraAcao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ManobraAcao manobraAcao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (manobraAcao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(manobraAcao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover os dados.");
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraAcao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraAcao()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                Descricao = Request.GetStringParam("Descricao"),
                Tipo = Request.GetNullableEnumParam<TipoManobraAcao>("Tipo")
            };
        }

        private void PreencherManobraAcao(Dominio.Entidades.Embarcador.Logistica.ManobraAcao manobraAcao, Repositorio.UnitOfWork unitOfWork)
        {
            manobraAcao.Descricao = Request.GetStringParam("Descricao");
            manobraAcao.LocalDestinoObrigatorio = Request.GetBoolParam("LocalDestinoObrigatorio");
            manobraAcao.Tipo = Request.GetEnumParam<TipoManobraAcao>("Tipo");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("LocalDestinoObrigatorio", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Local de Destino Obrigatório", "DescricaoLocalDestinoObrigatorio", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraAcao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.ManobraAcao repositorio = new Repositorio.Embarcador.Logistica.ManobraAcao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.ManobraAcao> listaManobraAcao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ManobraAcao>();

                var listaMotivoRetornar = (
                    from manobraAcao in listaManobraAcao
                    select new
                    {
                        manobraAcao.Codigo,
                        manobraAcao.Descricao,
                        manobraAcao.DescricaoLocalDestinoObrigatorio,
                        manobraAcao.LocalDestinoObrigatorio,
                        Tipo = manobraAcao.Tipo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoRetornar);
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

        #endregion
    }
}
