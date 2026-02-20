using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/MotivoFalhaNotaFiscal")]
    public class MotivoFalhaNotaFiscalController : BaseController
    {
		#region Construtores

		public MotivoFalhaNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal repMotivoFalhaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal motivoFalhaNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal();

                PreencherEntidade(motivoFalhaNotaFiscal, unitOfWork);

                unitOfWork.Start();

                repMotivoFalhaNotaFiscal.Inserir(motivoFalhaNotaFiscal, Auditado);

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

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal repMotivoFalhaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal motivoFalhaNotaFiscal = repMotivoFalhaNotaFiscal.BuscarPorCodigo(codigo, true);

                if (motivoFalhaNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(motivoFalhaNotaFiscal, unitOfWork);

                unitOfWork.Start();

                repMotivoFalhaNotaFiscal.Atualizar(motivoFalhaNotaFiscal, Auditado);

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

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal repMotivoFalhaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal motivoFalhaNotaFiscal = repMotivoFalhaNotaFiscal.BuscarPorCodigo(codigo, false);

                if (motivoFalhaNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoFalhaNotaFiscal.Codigo,
                    motivoFalhaNotaFiscal.Descricao,
                    motivoFalhaNotaFiscal.Observacao,
                    Situacao = motivoFalhaNotaFiscal.Ativo,
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

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal repMotivoFalhaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal motivoFalhaNotaFiscal = repMotivoFalhaNotaFiscal.BuscarPorCodigo(codigo, true);

                if (motivoFalhaNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repMotivoFalhaNotaFiscal.Deletar(motivoFalhaNotaFiscal, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal motivoFalhaNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            motivoFalhaNotaFiscal.Ativo = Request.GetBoolParam("Situacao");
            motivoFalhaNotaFiscal.Descricao = Request.GetStringParam("Descricao");
            motivoFalhaNotaFiscal.Observacao = Request.GetStringParam("Observacao");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string observacao = Request.Params("Observacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 60, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal repMotivoFalhaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal> listaMotivoFalhaNotaFiscal = repMotivoFalhaNotaFiscal.Consultar(descricao, observacao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repMotivoFalhaNotaFiscal.ContarConsulta(descricao, observacao, status);

                var retorno = listaMotivoFalhaNotaFiscal.Select(motivo => new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    motivo.Observacao,
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
