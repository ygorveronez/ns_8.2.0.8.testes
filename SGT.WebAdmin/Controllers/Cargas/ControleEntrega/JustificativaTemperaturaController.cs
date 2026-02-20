using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/JustificativaTemperatura")]
    public class JustificativaTemperaturaController : BaseController
    {
		#region Construtores

		public JustificativaTemperaturaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura repJustificativaTemperatura = new Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura motivoRetificacaoColeta = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura();

                PreencherEntidade(motivoRetificacaoColeta, unitOfWork);

                unitOfWork.Start();

                repJustificativaTemperatura.Inserir(motivoRetificacaoColeta, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura repJustificativaTemperatura = new Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura motivoRetificacaoColeta = repJustificativaTemperatura.BuscarPorCodigo(codigo, true);

                if (motivoRetificacaoColeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(motivoRetificacaoColeta, unitOfWork);

                unitOfWork.Start();

                repJustificativaTemperatura.Atualizar(motivoRetificacaoColeta, Auditado);

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

                Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura repJustificativaTemperatura = new Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura motivoRetificacaoColeta = repJustificativaTemperatura.BuscarPorCodigo(codigo, false);

                if (motivoRetificacaoColeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoRetificacaoColeta.Codigo,
                    motivoRetificacaoColeta.Descricao,
                    Status = motivoRetificacaoColeta.Ativo
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

                Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura repJustificativaTemperatura = new Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura motivoRetificacaoColeta = repJustificativaTemperatura.BuscarPorCodigo(codigo, true);

                if (motivoRetificacaoColeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repJustificativaTemperatura.Deletar(motivoRetificacaoColeta, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura motivoRetificacaoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            motivoRetificacaoColeta.Ativo = Request.GetBoolParam("Status");
            motivoRetificacaoColeta.Descricao = Request.GetStringParam("Descricao");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura repJustificativaTemperatura = new Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura> listaJustificativaTemperatura = repJustificativaTemperatura.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repJustificativaTemperatura.ContarConsulta(descricao, status);

                var retorno = listaJustificativaTemperatura.Select(motivo => new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    motivo.DescricaoAtivo
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch
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

        #endregion
    }
}
