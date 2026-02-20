using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositorio;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    /// <summary>
    /// Essa tela foi criada para a Decatlhon fazer uma conexão entre os lotes de pedido (pedido.NumeroLote) e o número de carregamento (pedido.NumeroRomaneio)
    /// </summary>
    [CustomAuthorize("Pedidos/NumeroCarregamentoPorLote", "Pedidos/Pedido", "Cargas/Carga")]
    public class NumeroCarregamentoPorLoteController : BaseController
    {
		#region Construtores

		public NumeroCarregamentoPorLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> CriarConexao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var lista = JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Lista"));

                foreach (dynamic item in lista)
                {
                    ConectarLoteECarregamento(item, unitOfWork);
                }

                unitOfWork.Start();

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarCarregamentoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            var codigoPedido = Request.GetIntParam("CodigoPedido");
            var numeroCarregamento = Request.GetStringParam("NumeroCarregamento");

            var pedido = repPedido.BuscarPorCodigo(codigoPedido);

            if (pedido == null)
                throw new ControllerException($"Pedido não encontrado");

            if (string.IsNullOrEmpty(numeroCarregamento))
                throw new ControllerException($"Número carregamento é obrigatório");

            var carga = repCarga.BuscarPorPedido(pedido.Codigo);

            if (carga != null)
                throw new ControllerException($"O pedido {pedido.NumeroPedidoEmbarcador} com lote {pedido.NumeroLote } já tem uma carga, não é possivel alterar o carregamento");

            pedido.NumeroCarregamento = numeroCarregamento;
            repPedido.Atualizar(pedido);
            return new JsonpResult(true);
        }

        public async Task<IActionResult> ValidarCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                string numeroTransportePedido = Request.GetStringParam("NumeroTransportePedido");

                if (string.IsNullOrWhiteSpace(numeroTransportePedido))
                    return new JsonpResult(false, true, "Número do Carregamento não informado, favor verificar!");

                Dominio.Entidades.Cliente destinatario = repositorioXMLNotaFiscal.BuscarFilialPorNumeroTransporteDoPedido(numeroTransportePedido);

                if (destinatario == null)
                    return new JsonpResult(false, true, "Destinatário não encontrado para este Número de Carregamento, favor verificar!");

                var retorno = new
                {
                    Carregamento = numeroTransportePedido + " - " + destinatario.Descricao
                };

                return new JsonpResult(retorno);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private void ConectarLoteECarregamento(dynamic item, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            string numeroLote = (string)item.Lote;
            string numeroCarregamento = (string)item.Carregamento;

            var pedidosDoLote = repPedido.BuscarPorLote(numeroLote);

            if (pedidosDoLote.Count == 0)
            {
                throw new ControllerException($"O lote {numeroLote} não está ligado a nenhum pedido");
            }

            foreach (var pedido in pedidosDoLote)
            {
                var carga = repCarga.BuscarPorPedido(pedido.Codigo);

                if (carga != null)
                {
                    throw new ControllerException($"O pedido {pedido.NumeroPedidoEmbarcador} com lote {numeroLote} já tem uma carga");
                }

                pedido.NumeroCarregamento = numeroCarregamento;
                repPedido.Atualizar(pedido);
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                var NumeroLote = Request.GetStringParam("Lote");
                var NumeroCarregamento = Request.GetStringParam("Carregamento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pedido", "NumeroPedidoEmbarcador", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Lote", "Lote", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carregamento", "Carregamento", 50, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ListaPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                if (!string.IsNullOrEmpty(NumeroLote) || !string.IsNullOrEmpty(NumeroCarregamento))
                    ListaPedidos = repPedido.BuscarPorCarregamentoLote(NumeroLote, NumeroCarregamento);

                var lista = (
                    from pedido in ListaPedidos
                    select new
                    {
                        pedido.Codigo,
                        pedido.NumeroPedidoEmbarcador,
                        Cliente = pedido.Destinatario?.NomeCNPJ ?? "",
                        Lote = pedido.NumeroLote,
                        Carregamento = pedido.NumeroCarregamento,
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(lista.Count);

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

    }
}
