using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PedidoTipoPagamento")]
    public class PedidoTipoPagamentoController : BaseController
    {
		#region Construtores

		public PedidoTipoPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento tipoPagamento = new Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento();

                PreencherEntidade(tipoPagamento, unitOfWork);

                unitOfWork.Start();

                repPedidoTipoPagamento.Inserir(tipoPagamento, Auditado);

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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento tipoPagamento = repPedidoTipoPagamento.BuscarPorCodigo(codigo, true);

                if (tipoPagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoPagamento, unitOfWork);

                unitOfWork.Start();

                repPedidoTipoPagamento.Atualizar(tipoPagamento, Auditado);

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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento tipoPagamento = repPedidoTipoPagamento.BuscarPorCodigo(codigo, false);

                if (tipoPagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoPagamento.Codigo,
                    tipoPagamento.Descricao,
                    Situacao = tipoPagamento.Ativo,
                    tipoPagamento.Observacao,
                    tipoPagamento.ObservacaoPedido,
                    tipoPagamento.FormaPagamento,
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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento tipoPagamento = repPedidoTipoPagamento.BuscarPorCodigo(codigo, true);

                if (tipoPagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repPedidoTipoPagamento.Deletar(tipoPagamento, Auditado);

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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento tipoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            bool ativo = Request.GetBoolParam("Situacao");
            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");
            string observacaoPedido = Request.GetStringParam("ObservacaoPedido");
            

            tipoPagamento.Ativo = ativo;
            tipoPagamento.Descricao = descricao;
            tipoPagamento.Observacao = observacao;
            tipoPagamento.ObservacaoPedido = observacaoPedido;
            tipoPagamento.FormaPagamento = Request.GetNullableEnumParam<FormaPagamento>("FormaPagamento");
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
                grid.AdicionarCabecalho("ObservacaoPedido", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento> listaPedidoTipoPagamento = repPedidoTipoPagamento.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPedidoTipoPagamento.ContarConsulta(descricao, status);

                var retorno = (from motivo in listaPedidoTipoPagamento
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.ObservacaoPedido,
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
