using Dominio.Entidades.Embarcador.Logistica;
using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ProcedimentoEmbarque")]
    public class ProcedimentoEmbarqueController : BaseController
    {
		#region Construtores

		public ProcedimentoEmbarqueController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);

                ProcedimentoEmbarque procedimentoEmbarque = new ProcedimentoEmbarque();

                PreencherEntidade(procedimentoEmbarque, unitOfWork);

                unitOfWork.Start();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, procedimentoEmbarque, null, "Adicionou o procedimento embarque " + procedimentoEmbarque.Descricao, unitOfWork);

                repProcedimentoEmbarque.Inserir(procedimentoEmbarque, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
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

                Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);

                ProcedimentoEmbarque procedimentoEmbarque = repProcedimentoEmbarque.BuscarPorCodigo(codigo, true);

                if (procedimentoEmbarque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(procedimentoEmbarque, unitOfWork);

                unitOfWork.Start();

                repProcedimentoEmbarque.Atualizar(procedimentoEmbarque, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
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
        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque procedimentoEmbarque = repProcedimentoEmbarque.BuscarPorCodigo(codigo);

                if (procedimentoEmbarque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    procedimentoEmbarque.Codigo,
                    procedimentoEmbarque.Descricao,
                    CodigoTipoOperacao = procedimentoEmbarque.TipoOperacao?.Codigo ?? 0,
                    CodigoFilial = procedimentoEmbarque.Filial?.Codigo ?? 0,
                    Filial = procedimentoEmbarque.Filial?.Descricao ?? "",
                    TipoOperacao = procedimentoEmbarque.TipoOperacao?.Descricao ?? "",
                    ModeloContratacao = procedimentoEmbarque.CodigoModeloContratacao,
                    ProcedimentoEmbarque = procedimentoEmbarque.IntegracaoProcedimentoEmbarque,
                    procedimentoEmbarque.Ativo,
                    TempoEntrega = procedimentoEmbarque.TempoEntregaAngelLira,
                    NaoEnviarDataInicioTermino = procedimentoEmbarque.NaoEnviarDataInicioETerminoViagemAngelLira
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

                Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);

                ProcedimentoEmbarque procedimentoEmbarque = repProcedimentoEmbarque.BuscarPorCodigo(codigo, true);

                if (procedimentoEmbarque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repProcedimentoEmbarque.Deletar(procedimentoEmbarque, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, procedimentoEmbarque, null, "Excluiu o procedimento embarque " + procedimentoEmbarque.Descricao, unitOfWork);

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
        #endregion

        #region Métodos Privados
        private void PreencherEntidade(ProcedimentoEmbarque procedimentoEmbarque, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            int codigoFilial = Request.GetIntParam("Filial");
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

            int codProcedimentoEmbarque = Request.GetIntParam("ProcedimentoEmbarque");
            int modeloContratacao = Request.GetIntParam("ModeloContratacao");
            int tempoEntrega = Request.GetIntParam("TempoEntrega");
            bool naoEnviarDataInicioTerminoViagem = Request.GetBoolParam("NaoEnviarDataInicioTermino");
            bool ativo = Request.GetBoolParam("Ativo");

            procedimentoEmbarque.Ativo = ativo;
            procedimentoEmbarque.Filial = filial;
            procedimentoEmbarque.TipoOperacao = tipoOperacao;
            procedimentoEmbarque.IntegracaoProcedimentoEmbarque = codProcedimentoEmbarque;
            procedimentoEmbarque.CodigoModeloContratacao = modeloContratacao;
            procedimentoEmbarque.TempoEntregaAngelLira = tempoEntrega;
            procedimentoEmbarque.NaoEnviarDataInicioETerminoViagemAngelLira = naoEnviarDataInicioTerminoViagem;

            VerificarExistencia(procedimentoEmbarque, unitOfWork);
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                int codigoFilial = Request.GetIntParam("Filial");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                if (!string.IsNullOrWhiteSpace(Request.Params("Ativo")))
                    Enum.TryParse(Request.Params("Ativo"), out status);

                //Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);
                //Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Filial", "Filial", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operacao", "TipoOperacao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Procedimento de Embarque", "ProcedimentoEmbarque", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);


                Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque> listaProcedimentoEmbarque = repProcedimentoEmbarque.Consultar(codigoFilial, codigoTipoOperacao, status, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repProcedimentoEmbarque.ContarConsulta(codigoFilial, codigoTipoOperacao, status);

                var retorno = (from procedimentoEmbarque in listaProcedimentoEmbarque
                               select new
                               {
                                   procedimentoEmbarque.Codigo,
                                   Descricao = procedimentoEmbarque.Descricao,
                                   Filial = procedimentoEmbarque.Filial?.Descricao ?? "", 
                                   TipoOperacao = procedimentoEmbarque.TipoOperacao?.Descricao ?? "",
                                   ProcedimentoEmbarque = procedimentoEmbarque.IntegracaoProcedimentoEmbarque,
                                   procedimentoEmbarque.DescricaoAtivo
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
        private void VerificarExistencia(ProcedimentoEmbarque procedimentoEmbarque, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);
            if (repProcedimentoEmbarque.VerificarExistencia(procedimentoEmbarque))
            {
                throw new ControllerException("Já existe um registro com a mesma filial e o mesmo tipo de operação.");
            }
        }
        #endregion
    }
}
