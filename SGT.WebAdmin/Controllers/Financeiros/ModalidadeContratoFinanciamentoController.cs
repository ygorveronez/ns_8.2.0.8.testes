using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/ModalidadeContratoFinanciamento")]
    public class ModalidadeContratoFinanciamentoController : BaseController
    {
		#region Construtores

		public ModalidadeContratoFinanciamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento repModalidadeContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento modalidadeContrato = new Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento();

                PreencherEntidade(modalidadeContrato, unitOfWork);

                unitOfWork.Start();

                repModalidadeContratoFinanciamento.Inserir(modalidadeContrato, Auditado);

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

                Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento repModalidadeContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento modalidadeContrato = repModalidadeContratoFinanciamento.BuscarPorCodigo(codigo, true);

                if (modalidadeContrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(modalidadeContrato, unitOfWork);

                unitOfWork.Start();

                repModalidadeContratoFinanciamento.Atualizar(modalidadeContrato, Auditado);

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento repModalidadeContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento modalidadeContrato = repModalidadeContratoFinanciamento.BuscarPorCodigo(codigo, false);

                if (modalidadeContrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    modalidadeContrato.Codigo,
                    modalidadeContrato.Descricao,
                    Situacao = modalidadeContrato.Ativo,
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

                Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento repModalidadeContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento modalidadeContrato = repModalidadeContratoFinanciamento.BuscarPorCodigo(codigo, true);

                if (modalidadeContrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repModalidadeContratoFinanciamento.Deletar(modalidadeContrato, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento modalidadeContrato, Repositorio.UnitOfWork unitOfWork)
        {
            

            bool ativo = Request.GetBoolParam("Situacao");
            string descricao = Request.Params("Descricao");

            
            
            modalidadeContrato.Ativo = ativo;
            modalidadeContrato.Descricao = descricao;
            
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                
                
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);
               
                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento repModalidadeContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ModalidadeContratoFinanciamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento> listaGrupoDespesa = repModalidadeContratoFinanciamento.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repModalidadeContratoFinanciamento.ContarConsulta(descricao, status);

                var retorno = (from motivo in listaGrupoDespesa
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.Descricao,
                                   motivo.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);
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

        #endregion
    }
}
