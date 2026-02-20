using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PedidoCancelamento")]
    public class PedidoCancelamentoController : BaseController
    {
		#region Construtores

		public PedidoCancelamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string numeroPedidoEmbarcador = Request.Params("NumeroPedidoEmbarcador");

                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("NumeroPedido"), out int numeroPedido);

                double.TryParse(Request.Params("Remetente"), out double cpfCnpjRemetente);
                double.TryParse(Request.Params("Destinatario"), out double cpfCnpjDestinatario);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pedido", "Pedido", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data", "DataCancelamento", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 22, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 22, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Solicitante", "Solicitante", 20, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Pedido")
                    propOrdenar = "Pedido.Numero";

                Repositorio.Embarcador.Pedidos.PedidoCancelamento repPedidoCancelamento = new Repositorio.Embarcador.Pedidos.PedidoCancelamento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento> pedidosCancelamento = repPedidoCancelamento.Consultar(numeroPedido, numeroPedidoEmbarcador, codigoGrupoPessoas, cpfCnpjRemetente, cpfCnpjDestinatario, dataInicial, dataFinal, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoCancelamento.ContarConsulta(numeroPedido, numeroPedidoEmbarcador, codigoGrupoPessoas, cpfCnpjRemetente, cpfCnpjDestinatario, dataInicial, dataFinal));

                var retorno = (from obj in pedidosCancelamento
                               select new
                               {
                                   obj.Codigo,
                                   Pedido = obj.Pedido.Numero,
                                   DataCancelamento = obj.DataCancelamento.ToString("dd/MM/yyyy HH:mm:ss"),
                                   Remetente = obj.Pedido.Remetente?.Descricao ?? string.Empty,
                                   Destinatario = obj.Pedido.Destinatario?.Descricao ?? string.Empty,
                                   Tipo = obj.DescricaoTipo,
                                   Situacao = obj.DescricaoSituacao,
                                   Solicitante = obj.Usuario?.Nome ?? string.Empty
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var retorno = new
                {
                    Usuario = Usuario.Nome
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados gerais para cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarPedido()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPedido;
                int.TryParse(Request.Params("Pedido"), out codigoPedido);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                bool permiteCancelamento = true;
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento> tipoCancelamentosPermitidos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento>();

                string mensagem = string.Empty;

                bool cancelamentoValido = Servicos.Embarcador.Pedido.Pedido.ValidarCancelamentoPedido(out mensagem, pedido, unidadeTrabalho);

                if (cancelamentoValido)
                {
                    if (pedido.TipoOperacao != null)
                    {
                        if (pedido.TipoOperacao.PermiteDesistenciaCarga && pedido.TipoOperacao.PercentualCobrarDesistenciaCarga > 0m)
                        {
                            if (pedido.DataInicialColeta.HasValue && pedido.TipoOperacao.CobrarDesistenciaCargaAposHorario && pedido.TipoOperacao.HoraCobrarDesistenciaCarga.HasValue)
                            {
                                DateTime dataLimite = new DateTime(pedido.DataInicialColeta.Value.Year, pedido.DataInicialColeta.Value.Month, pedido.DataInicialColeta.Value.Day, pedido.TipoOperacao.HoraCobrarDesistenciaCarga.Value.Hours, pedido.TipoOperacao.HoraCobrarDesistenciaCarga.Value.Minutes, pedido.TipoOperacao.HoraCobrarDesistenciaCarga.Value.Seconds);

                                if (dataLimite < DateTime.Now)
                                    permiteCancelamento = false;
                            }

                            tipoCancelamentosPermitidos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento.DesistenciaCarga);
                        }

                        if (pedido.TipoOperacao.PermiteDesistenciaCarregamento && pedido.TipoOperacao.PercentualCobrarDesistenciaCarga > 0m)
                            tipoCancelamentosPermitidos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento.DesistenciaCarregamento);

                        if (permiteCancelamento && pedido.CargasPedido.Count <= 0)
                            tipoCancelamentosPermitidos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento.Cancelamento);
                    }
                    else
                    {
                        if (pedido.CargasPedido.Count <= 0)
                            tipoCancelamentosPermitidos.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento.Cancelamento);
                    }
                }

                return new JsonpResult(new
                {
                    TiposCancelamentosPermitidos = tipoCancelamentosPermitidos,
                    PermiteCancelamento = cancelamentoValido,
                    Mensagem = mensagem
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao validar os dados da carga para cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Pedido"), out int codigoPedido);

                Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoCancelamento tipoCancelamento);

                string motivo = Request.Params("MotivoCancelamento");

                unidadeTrabalho.Start();

                if (!Servicos.Embarcador.Pedido.Pedido.CancelarPedido(out string erro, codigoPedido, tipoCancelamento, Usuario, motivo, unidadeTrabalho, TipoServicoMultisoftware, Auditado, Cliente, 0))
                {
                    unidadeTrabalho.Rollback();

                    return new JsonpResult(false, true, erro);
                }
                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoPedidoCancelamento);

                Repositorio.Embarcador.Pedidos.PedidoCancelamento repPedidoCancelamento = new Repositorio.Embarcador.Pedidos.PedidoCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento pedidoCancelamento = repPedidoCancelamento.BuscarPorCodigo(codigoPedidoCancelamento, false);

                return new JsonpResult(new
                {
                    pedidoCancelamento.Codigo,
                    DataCancelamento = pedidoCancelamento.DataCancelamento.ToString("dd/MM/yyyy HH:mm:ss"),
                    pedidoCancelamento.MotivoCancelamento,
                    pedidoCancelamento.Situacao,
                    pedidoCancelamento.Tipo,
                    Usuario = pedidoCancelamento.Usuario?.Nome ?? string.Empty,
                    Pedido = new
                    {
                        pedidoCancelamento.Pedido.Codigo,
                        Descricao = pedidoCancelamento.Pedido.Numero.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
