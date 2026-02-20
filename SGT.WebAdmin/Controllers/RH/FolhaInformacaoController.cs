using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/FolhaInformacao")]
    public class FolhaInformacaoController : BaseController
    {
		#region Construtores

		public FolhaInformacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.RH.FolhaInformacao> informacoesFolha = repFolhaInformacao.Consultar(descricao, codigoIntegracao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFolhaInformacao.ContarConsulta(descricao, codigoIntegracao));

                var lista = (from p in informacoesFolha
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao,
                                 Justificativa = p.Justificativa != null ? p.Justificativa.Descricao : string.Empty
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

                Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaInformacao folhaInformacao = new Dominio.Entidades.Embarcador.RH.FolhaInformacao();

                PreencherFolhaInformacao(folhaInformacao, unitOfWork);
                repFolhaInformacao.Inserir(folhaInformacao, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaInformacao folhaInformacao = repFolhaInformacao.BuscarPorCodigo(codigo, true);

                PreencherFolhaInformacao(folhaInformacao, unitOfWork);
                repFolhaInformacao.Atualizar(folhaInformacao, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaInformacao folhaInformacao = repFolhaInformacao.BuscarPorCodigo(codigo, false);

                var dynFolhaInformacao = new
                {
                    folhaInformacao.Codigo,
                    folhaInformacao.Descricao,
                    folhaInformacao.CodigoIntegracao,
                    Justificativa = folhaInformacao.Justificativa != null ? new { folhaInformacao.Justificativa.Codigo, folhaInformacao.Justificativa.Descricao } : null,
                };

                return new JsonpResult(dynFolhaInformacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaInformacao folhaInformacao = repFolhaInformacao.BuscarPorCodigo(codigo, true);

                if (folhaInformacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repFolhaInformacao.Deletar(folhaInformacao, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherFolhaInformacao(Dominio.Entidades.Embarcador.RH.FolhaInformacao folhaInformacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            int.TryParse(Request.Params("Justificativa"), out int justificativa);

            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");

            folhaInformacao.Descricao = descricao;
            folhaInformacao.CodigoIntegracao = codigoIntegracao;
            folhaInformacao.Justificativa = justificativa > 0 ? repJustificativa.BuscarPorCodigo(justificativa) : null;
        }

        #endregion
    }
}
