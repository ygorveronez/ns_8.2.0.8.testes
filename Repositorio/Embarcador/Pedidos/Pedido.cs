using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using LinqKit;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Util;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class Pedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.Pedido>
    {
        #region Construtores

        public Pedido(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Pedido(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa)
        {
            var filtroPedidos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            if (filtrosPesquisa.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !filtrosPesquisa.ProgramaComSessaoRoteirizador)
                consultaPedido = consultaPedido.Where(obj => obj.Protocolo != 0);

            if (filtrosPesquisa.CodigoPedidoMinimo > 0)
                consultaPedido = consultaPedido.Where(obj => obj.Codigo > filtrosPesquisa.CodigoPedidoMinimo);

            if (filtrosPesquisa.CodigosEmpresa != null && filtrosPesquisa.CodigosEmpresa.Count > 0)
                filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosEmpresa.Contains(obj.Empresa.Codigo));

            if (filtrosPesquisa.PedidoParaReentrega)
            {
                consultaPedido = consultaPedido.Where(o => o.ReentregaSolicitada == true);
            }
            else if (filtrosPesquisa.PedidoSemCargaPedido)
            {
                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

                filtroPedidos = filtroPedidos.And(obj => !consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo));
            }
            else if (filtrosPesquisa.PedidoSemCarga)
            {
                if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                    if (filtrosPesquisa.TipoControleSaldoPedido.HasValue && filtrosPesquisa.TipoControleSaldoPedido.Value == TipoControleSaldoPedido.Pallet)
                        filtroPedidos = filtroPedidos.And(obj => !consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo) || (obj.PalletSaldoRestante > 0m && !obj.PedidoTotalmenteCarregado));
                    else if (filtrosPesquisa.TipoControleSaldoPedido.HasValue && filtrosPesquisa.TipoControleSaldoPedido.Value == TipoControleSaldoPedido.CarregamentoUnico)
                        filtroPedidos = filtroPedidos.And(obj => !consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo));
                    else
                        filtroPedidos = filtroPedidos.And(obj => !consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo) || (obj.PesoSaldoRestante > 0m && !obj.PedidoTotalmenteCarregado));
                }
                else
                {
                    if (!filtrosPesquisa.DisponivelColeta)
                        filtroPedidos = filtroPedidos.And(obj => obj.GerarAutomaticamenteCargaDoPedido == false && (obj.TipoOperacao == null || obj.TipoOperacao.OperacaoRecolhimentoTroca == false));

                    if (filtrosPesquisa.PedidosRedespacho)
                        filtroPedidos = filtroPedidos.And(obj => obj.Recebedor != null && obj.PedidoRedespachoTotalmenteCarregado == false);
                    else
                    {
                        if (filtrosPesquisa.SituacaoSessaoRoteirizador != SituacaoSessaoRoteirizador.Finalizada)
                        {
                            // este if não é executável atualmente, pois este bloco inteiro está contido dentro de um `else if (filtrosPesquisa.PedidoSemCarga)`, atualmente na linha 78.
                            if (!filtrosPesquisa.ExibirPedidosExpedidor && !filtrosPesquisa.PedidoSemCarga)
                            {
                                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                                    .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

                                filtroPedidos = filtroPedidos.And(obj =>
                                    (obj.Expedidor != null && consultaCargaPedido.Any(o => o.Expedidor.CPF_CNPJ == obj.Expedidor.CPF_CNPJ && o.Pedido.Codigo == obj.Codigo)) ||
                                    (obj.Expedidor == null)
                                );
                            }
                            else if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) // Boticário, portal transportador...
                            {
                                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                                    .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

                                var consultaXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

                                //Se não existe carga pedido para todas as notas ou se o tipo de operação está marcado para colete entrega
                                filtroPedidos = filtroPedidos.And(
                                    obj => obj.TipoOperacao.PedidoColetaEntrega ||
                                    consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo && o.Carga.TipoOperacao.PedidoColetaEntrega && (o.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos || o.Carga.SituacaoCarga == SituacaoCarga.ProntoTransporte || o.Carga.SituacaoCarga == SituacaoCarga.EmTransporte || o.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento || o.Carga.SituacaoCarga == SituacaoCarga.Encerrada)) ||
                                    !consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo) ||
                                    obj.NotasFiscais.Any(n => !consultaXMLNotaFiscal.Any(x => x.XMLNotaFiscal.Codigo == n.Codigo && x.CargaPedido.Pedido.Codigo == obj.Codigo && x.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada)) ||
                                    obj.TipoOperacao.ConfiguracaoTransportador.PermitirTransportadorAjusteCargaSegundoTrecho || obj.TipoOperacao.ConfiguracaoCarga.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga);
                            }
                            else if (filtrosPesquisa.FiltrarPedidosVinculadoOutrasCarga)
                            {
                                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().Where(o =>
                                    (
                                        o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.PermitidoSelecionarTipoDeOperacaoNaMontagemDaCarga
                                    )
                                    &&
                                    (
                                        o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                                        o.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                                        o.Carga.SituacaoCarga != SituacaoCarga.Encerrada
                                    )
                                );

                                filtroPedidos = filtroPedidos.And(obj =>
                                    !this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().Any(o => o.Pedido.Codigo == obj.Codigo)
                                    || consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo)
                                    || obj.ReentregaSolicitada
                                );
                            }
                            else
                            {
                                //moreto seu bostinha; se vc colocar essa flag tem q validar pois alguns clientes precisam que os pedidos voltem pra montagem mesmo tendo carga.
                                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                                    .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

                                if ((filtrosPesquisa.TipoControleSaldoPedido.HasValue ? filtrosPesquisa.TipoControleSaldoPedido.Value : TipoControleSaldoPedido.Peso) != TipoControleSaldoPedido.CarregamentoUnico)
                                    consultaCargaPedido = consultaCargaPedido.Where(o => o.Carga.TipoOperacao == null || o.Carga.TipoOperacao.ConfiguracaoMontagemCarga == null && (o.Carga.TipoOperacao.ConfiguracaoMontagemCarga.ExibirPedidosMontagemIntegracao == false && o.Carga.TipoOperacao.ConfiguracaoMontagemCarga.DisponibilizarPedidosMontagemAoFinalizarTransporte == false && o.Carga.TipoOperacao.ConfiguracaoMontagemCarga.DisponibilizarPedidosMontagemDeterminadosTransportadores == false));

                                // Segunda carga.. origem Expedidor..
                                if (filtrosPesquisa.PedidosOrigemRecebedor && filtrosPesquisa.Expedidor > 0)
                                    consultaCargaPedido = consultaCargaPedido.Where(x => x.Expedidor.CPF_CNPJ == filtrosPesquisa.Expedidor);

                                filtroPedidos = filtroPedidos.And(obj => !consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo) || obj.ReentregaSolicitada);//Exibir pedidos de reentrega mesmo que estão em outra carga
                            }

                            //PedidosOrigemRecebedor, é utilizado para roteirizar um "segundo trecho" com o mesmo pedido...
                            if (!filtrosPesquisa.DisponivelColeta && !filtrosPesquisa.PedidosOrigemRecebedor)
                                filtroPedidos = filtroPedidos.And(obj => obj.PedidoTotalmenteCarregado == false);
                            else if (!filtrosPesquisa.DisponivelColeta && filtrosPesquisa.PedidosOrigemRecebedor)
                                filtroPedidos = filtroPedidos.And(obj => obj.PedidoRedespachoTotalmenteCarregado == false);
                        }
                    }
                }
            }
            else if (filtrosPesquisa.VinculoCarga?.Count > 0)
            {
                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

                if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoComCarga))
                    filtroPedidos = filtroPedidos.And(obj => consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo));
                else if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoSemCarga))
                    filtroPedidos = filtroPedidos.And(obj => !consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo));
            }

            if (filtrosPesquisa.OcultarPedidosRetiradaProdutos)
                consultaPedido = consultaPedido.Where(o => o.TipoOperacao == null || o.TipoOperacao.SelecionarRetiradaProduto == false);

            if (filtrosPesquisa.NumeroControlesPedido != null)
                consultaPedido = consultaPedido.Where(o => filtrosPesquisa.NumeroControlesPedido.Contains(o.NumeroControle));

            if (filtrosPesquisa.PedidoSemCarregamento)
            {
                var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                    .Where(o =>
                        o.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado &&
                        o.Carregamento.CarregamentoRedespacho == filtrosPesquisa.PedidosRedespacho &&
                        o.Carregamento.CarregamentoColeta == filtrosPesquisa.DisponivelColeta
                    );

                filtroPedidos = filtroPedidos.And(o => !consultaCarregamentoPedido.Any(c => c.Pedido.Codigo == o.Codigo));
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamento))
            {
                var consultaCarregamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                    .Where(o => o.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado);

                filtroPedidos = filtroPedidos.And(o => consultaCarregamentoPedido.Any(c => c.Pedido.Codigo == o.Codigo && c.Carregamento.NumeroCarregamento == filtrosPesquisa.NumeroCarregamento));
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamentoPedido))
            {
                filtroPedidos = filtroPedidos.And(o => o.NumeroCarregamento == filtrosPesquisa.NumeroCarregamentoPedido);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoAgrupamentoCarregamento))
                filtroPedidos = filtroPedidos.And(o => o.CodigoAgrupamentoCarregamento == filtrosPesquisa.CodigoAgrupamentoCarregamento);

            if (filtrosPesquisa.NumeroCotacao > 0)
                filtroPedidos = filtroPedidos.And(o => o.Cotacoes.Any(c => c.Codigo == filtrosPesquisa.NumeroCotacao));

            if (filtrosPesquisa.NaoExibirPedidosDoDia)
                filtroPedidos = filtroPedidos.And(o => o.DataCriacao < DateTime.Now.Date || o.PedidoLiberadoPortalRetira);

            if (filtrosPesquisa.NaoMostrarCargasDeslocamentoVazio)
                filtroPedidos = filtroPedidos.And(o => o.TipoOperacao.DeslocamentoVazio != true || o.TipoOperacao == null);

            if (!filtrosPesquisa.ProgramaComSessaoRoteirizador || filtrosPesquisa.OpcaoSessaoRoteirizador != OpcaoSessaoRoteirizador.ABRIR_SESSAO)
            {
                if (filtrosPesquisa.Transbordo.HasValue)
                    consultaPedido = consultaPedido.Where(o => o.PedidoTransbordo == filtrosPesquisa.Transbordo.Value);

                if (filtrosPesquisa.DisponivelColeta)
                    filtroPedidos = filtroPedidos.And(obj => obj.DisponibilizarPedidoParaColeta == true);

                if (filtrosPesquisa.CodigoPedidoViagemNavio > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.PedidoViagemNavio.Codigo == filtrosPesquisa.CodigoPedidoViagemNavio);

                if (filtrosPesquisa.CodigoFuncionarioVendedor > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.FuncionarioVendedor.Codigo == filtrosPesquisa.CodigoFuncionarioVendedor);

                if (filtrosPesquisa.CodigoTransportador > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

                if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosTransportador.Contains(obj.Empresa.Codigo));

                if (filtrosPesquisa.ListaExpedidor?.Count > 0)
                {
                    if (!filtrosPesquisa.CodigosFilial.Any(o => o == -1))
                    {
                        var consultaStagesPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();
                        filtroPedidos = filtroPedidos.And(o => filtrosPesquisa.ListaExpedidor.Contains(o.Expedidor.CPF_CNPJ) || consultaStagesPedido.Any(c => c.Pedido.Codigo == o.Codigo && filtrosPesquisa.ListaExpedidor.Contains(c.Stage.Expedidor.CPF_CNPJ)));
                    }
                }
                else if (filtrosPesquisa.Expedidor > 0d)
                    filtroPedidos = filtroPedidos.And(obj => obj.Expedidor.CPF_CNPJ == filtrosPesquisa.Expedidor);

                if (filtrosPesquisa.ListaRecebedor?.Count > 0)
                {
                    if (!filtrosPesquisa.CodigosFilial.Any(o => o == -1))
                    {
                        var consultaStagesPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

                        filtroPedidos = filtroPedidos.And(o => filtrosPesquisa.ListaRecebedor.Contains(o.Recebedor.CPF_CNPJ) || consultaStagesPedido.Any(c => c.Pedido.Codigo == o.Codigo && filtrosPesquisa.ListaRecebedor.Contains(c.Stage.Recebedor.CPF_CNPJ)));

                        ;// filtroPedidos = filtroPedidos.And();

                    }
                }
                else if (filtrosPesquisa.Recebedor > 0d)
                {
                    IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consultaCargaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                        .Where(o => o.Recebedor != null && o.Recebedor.CPF_CNPJ == filtrosPesquisa.Recebedor);
                    filtroPedidos = filtroPedidos.And(obj => obj.Recebedor.CPF_CNPJ == filtrosPesquisa.Recebedor || consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo));
                }

                if (filtrosPesquisa.Rota > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.RotaFrete.Codigo == filtrosPesquisa.Rota);

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                    filtroPedidos = filtroPedidos.And(obj => obj.NumeroBooking.Equals(filtrosPesquisa.NumeroBooking));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Ordem))
                    filtroPedidos = filtroPedidos.And(obj => obj.Ordem.Equals(filtrosPesquisa.Ordem));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PortoSaida))
                    filtroPedidos = filtroPedidos.And(obj => obj.PortoSaida.Equals(filtrosPesquisa.PortoSaida));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Reserva))
                    filtroPedidos = filtroPedidos.And(obj => obj.Reserva.Equals(filtrosPesquisa.Reserva));

                if (filtrosPesquisa.SomenteComReserva)
                    filtroPedidos = filtroPedidos.And(obj => obj.Reserva != null && obj.Reserva != "");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoEmbarque))
                    filtroPedidos = filtroPedidos.And(obj => obj.TipoEmbarque.Equals(filtrosPesquisa.TipoEmbarque));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                    filtroPedidos = filtroPedidos.And(obj => obj.NumeroOS.Equals(filtrosPesquisa.NumeroOS));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                {
                    if (filtrosPesquisa.FiltrarPorMultiplosRegistros)
                    {
                        List<string> pedidosFiltro = filtrosPesquisa.NumeroPedidoEmbarcador.Split(',').Select(s => s.Trim()).Distinct().ToList();
                        filtroPedidos = filtroPedidos.And(obj => pedidosFiltro.Contains(obj.NumeroPedidoEmbarcador));
                    }
                    else if (filtrosPesquisa.FiltrarPorParteDoNumero)
                        filtroPedidos = filtroPedidos.And(obj => obj.NumeroPedidoEmbarcador.Contains(filtrosPesquisa.NumeroPedidoEmbarcador));
                    else
                        filtroPedidos = filtroPedidos.And(obj => obj.NumeroPedidoEmbarcador.Equals(filtrosPesquisa.NumeroPedidoEmbarcador));
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcadorDe) && !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcadorAte))
                {
                    filtroPedidos = filtroPedidos.And(obj => obj.NumeroPedidoEmbarcador != null);
                    string de = Regex.Replace(filtrosPesquisa.NumeroPedidoEmbarcadorDe, @"[^\d]", "");
                    string ate = Regex.Replace(filtrosPesquisa.NumeroPedidoEmbarcadorAte, @"[^\d]", "");
                    long nroDe = long.Parse((Regex.Match(de, @"\d+").Value == null ? "0" : Regex.Match(de, @"\d+").Value));
                    long nroAte = long.Parse((Regex.Match(ate, @"\d+").Value == null ? "9999999999" : Regex.Match(ate, @"\d+").Value));

                    filtroPedidos = filtroPedidos.And(obj =>
                        obj.NumeroPedidoEmbarcador.TryParseLong() >= nroDe &&
                        obj.NumeroPedidoEmbarcador.TryParseLong() <= nroAte
                    );
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                {
                    if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                        filtroPedidos = filtroPedidos.And(obj => obj.CargasPedido.Any(o => o.CodigoCargaEmbarcador.Contains(filtrosPesquisa.NumeroCarga)) || obj.CargasPedido.Any(o => o.CodigosAgrupados.Contains(filtrosPesquisa.NumeroCarga)));
                    else
                        filtroPedidos = filtroPedidos.And(obj => obj.CargasPedido.Any(o => o.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga) || obj.CargasPedido.Any(o => o.CodigosAgrupados.Contains(filtrosPesquisa.NumeroCarga)));
                }

                if (filtrosPesquisa.CodigoCarga > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.CargasPedido.Any(o => o.Codigo == filtrosPesquisa.CodigoCarga));

                if (filtrosPesquisa.OcultarPedidosProvisorios)
                    filtroPedidos = filtroPedidos.And(obj => obj.Filial == null || obj.Destinatario == null || double.Parse(obj.Filial.CNPJ) != obj.Destinatario.CPF_CNPJ);

                if (filtrosPesquisa.Remetentes?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.Remetentes.Contains(obj.Remetente.CPF_CNPJ));

                if (filtrosPesquisa.NumeroPedido > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.Numero == filtrosPesquisa.NumeroPedido);

                if (filtrosPesquisa.Destinatarios?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.Destinatarios.Contains(obj.Destinatario.CPF_CNPJ));

                if (filtrosPesquisa.ClientePortal > 0)
                {
                    if ((filtrosPesquisa.Remetentes?.Count ?? 0) == 0 && (filtrosPesquisa.Destinatarios?.Count ?? 0) == 0)
                        filtroPedidos = filtroPedidos.And(obj => (obj.Destinatario.CPF_CNPJ == filtrosPesquisa.ClientePortal || obj.Remetente.CPF_CNPJ == filtrosPesquisa.ClientePortal));
                }

                if (filtrosPesquisa.CodigosRemetenteDestinatarioRetirada?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.TipoOperacao.ConfiguracaoPedido.FiltrarPedidosPorRemetenteRetiradaProduto
                        ? filtrosPesquisa.CodigosRemetenteDestinatarioRetirada.Contains(obj.Remetente.CPF_CNPJ)
                        : filtrosPesquisa.CodigosRemetenteDestinatarioRetirada.Contains(obj.Destinatario.CPF_CNPJ));

                if (filtrosPesquisa.CodigosFilial?.Count > 0)
                {
                    if (filtrosPesquisa.CodigosFilial.Any(o => o == -1))
                        filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosFilial.Contains(obj.Filial.Codigo) || filtrosPesquisa.ListaRecebedor.Contains(obj.Recebedor.CPF_CNPJ));
                    else
                        filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosFilial.Contains(obj.Filial.Codigo));
                }

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosFilialVenda.Contains(obj.FilialVenda.Codigo));

                if ((filtrosPesquisa.CodigosAgrupadores?.Count > 0) && (filtrosPesquisa.CodigosPedido?.Count > 0))
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosAgrupadores.Contains(obj.CodigoAgrupamentoCarregamento) || filtrosPesquisa.CodigosPedido.Contains(obj.Codigo));
                else if (filtrosPesquisa.CodigosPedido?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosPedido.Contains(obj.Codigo));

                if (filtrosPesquisa?.CodigosTipoCarga?.Count > 0)
                    filtroPedidos = filtroPedidos.And(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.TipoDeCarga.Codigo) || (filtrosPesquisa.CodigosTipoCarga.Contains(-1) && o.TipoDeCarga == null));

                if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosTipoOperacao.Contains(obj.TipoOperacao.Codigo) || (filtrosPesquisa.CodigosTipoOperacao.Contains(-1) && obj.TipoOperacao == null));

                if (filtrosPesquisa.CodigosTipoOperacaoDiferenteDe?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => !filtrosPesquisa.CodigosTipoOperacaoDiferenteDe.Contains(obj.TipoOperacao.Codigo));

                if (filtrosPesquisa.CodigosCarga?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.CargasPedido.Any(o => filtrosPesquisa.CodigosCarga.Contains(o.Codigo)));

                if (filtrosPesquisa.DataInicial.HasValue)
                {
                    if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.CARREGAMENTO_PEDIDO || !filtrosPesquisa.TipoFiltroData.HasValue)
                        filtroPedidos = filtroPedidos.And(o => o.DataCarregamentoPedido >= filtrosPesquisa.DataInicial.Value.Date);
                    else if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.PREVISAO_SAIDA)
                        filtroPedidos = filtroPedidos.And(o => o.DataPrevisaoSaida >= filtrosPesquisa.DataInicial.Value.Date);
                    else
                        filtroPedidos = filtroPedidos.And(o => o.PrevisaoEntrega >= filtrosPesquisa.DataInicial.Value.Date);
                }

                if (filtrosPesquisa.DataLimite.HasValue)
                {
                    if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.CARREGAMENTO_PEDIDO || !filtrosPesquisa.TipoFiltroData.HasValue)
                        filtroPedidos = filtroPedidos.And(o => o.DataCarregamentoPedido < filtrosPesquisa.DataLimite.Value.AddDays(1).Date);
                    else if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.PREVISAO_SAIDA)
                        filtroPedidos = filtroPedidos.And(o => o.DataPrevisaoSaida.Value.Date < filtrosPesquisa.DataLimite.Value.AddDays(1).Date);
                    else
                        filtroPedidos = filtroPedidos.And(o => o.PrevisaoEntrega.Value.Date < filtrosPesquisa.DataLimite.Value.AddDays(1).Date);
                }

                if (filtrosPesquisa.NumeroNotasFiscais?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.PedidoNotasParciais.Any(nf => filtrosPesquisa.NumeroNotasFiscais.Contains(nf.Numero)) || obj.PedidosCarga.Any(cp => cp.NotasFiscais.Any(nf => filtrosPesquisa.NumeroNotasFiscais.Contains(nf.XMLNotaFiscal.Numero))) || obj.NotasFiscais.Any(nf => filtrosPesquisa.NumeroNotasFiscais.Contains(nf.Numero)));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroTransporte))
                    filtroPedidos = filtroPedidos.And(p => p.NotasFiscais.Any(n => n.NumeroTransporte == filtrosPesquisa.NumeroTransporte));

                if (filtrosPesquisa.CodigosPaisOrigem?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosPaisOrigem.Contains(obj.Origem.Pais.Codigo));

                if (filtrosPesquisa.CodigosPaisDestino?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosPaisDestino.Contains(obj.Destino.Pais.Codigo));

                if (filtrosPesquisa.CodigosCidadePoloOrigem?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosCidadePoloOrigem.Contains(obj.Origem.LocalidadePolo.Codigo));

                if (filtrosPesquisa.CodigosCidadePoloDestino?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosCidadePoloDestino.Contains(obj.Destino.LocalidadePolo.Codigo));

                if (filtrosPesquisa.CodigosOrigem?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosOrigem.Contains(obj.Origem.Codigo));

                if (filtrosPesquisa.CodigosDestino?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosDestino.Contains(obj.Destino.Codigo));

                if (filtrosPesquisa.EstadosOrigem?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.EstadosOrigem.Contains(obj.Origem.Estado.Sigla));

                if (filtrosPesquisa.EstadosDestino?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.EstadosDestino.Contains(obj.Destino.Estado.Sigla));

                if (filtrosPesquisa.CodigosVeiculo?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosVeiculo.Contains(obj.VeiculoTracao.Codigo) || obj.Veiculos.Any(v => filtrosPesquisa.CodigosVeiculo.Contains(v.Codigo)));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                    filtroPedidos = filtroPedidos.And(obj => obj.VeiculoTracao.Placa.Equals(filtrosPesquisa.Placa) || obj.Veiculos.Any(v => v.Placa.Equals(filtrosPesquisa.Placa)));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPropriedadeVeiculo) && !filtrosPesquisa.TipoPropriedadeVeiculo.Equals("A"))
                    filtroPedidos = filtroPedidos.And(obj => obj.VeiculoTracao.Tipo.Equals(filtrosPesquisa.TipoPropriedadeVeiculo) || obj.Veiculos.Any(v => v.Tipo.Equals(filtrosPesquisa.TipoPropriedadeVeiculo)));

                if (filtrosPesquisa.CodigosMotorista?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.Motoristas.Any(m => filtrosPesquisa.CodigosMotorista.Contains(m.Codigo)));

                if (filtrosPesquisa.CodigosGestores?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.Motoristas.Any(m => filtrosPesquisa.CodigosGestores.Contains(m.Gestor.Codigo)));

                if (filtrosPesquisa.DataColeta.HasValue)
                    filtroPedidos = filtroPedidos.And(o => o.DataCarregamentoPedido >= filtrosPesquisa.DataColeta.Value.Date && o.DataCarregamentoPedido < filtrosPesquisa.DataColeta.Value.AddDays(1).Date);

                if (filtrosPesquisa.SituacaoAcompanhamentoPedido.HasValue && filtrosPesquisa.SituacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.Todos)
                    filtroPedidos = filtroPedidos.And(obj => obj.SituacaoAcompanhamentoPedido == filtrosPesquisa.SituacaoAcompanhamentoPedido.Value);

                if (filtrosPesquisa.Situacao.HasValue && filtrosPesquisa.Situacao != SituacaoPedido.Todos)
                    filtroPedidos = filtroPedidos.And(obj => obj.SituacaoPedido == filtrosPesquisa.Situacao.Value);

                if (filtrosPesquisa.SituacaoPlanejamentoPedido.HasValue)
                    filtroPedidos = filtroPedidos.And(obj => obj.SituacaoPlanejamentoPedido == filtrosPesquisa.SituacaoPlanejamentoPedido.Value);

                if (filtrosPesquisa.SituacaoPlanejamentoPedidoTMS?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.SituacaoPlanejamentoPedidoTMS.Contains(obj.SituacaoPlanejamentoPedidoTMS));

                if (filtrosPesquisa.PedidosSelecionados != null && filtrosPesquisa.PedidosSelecionados.Count > 0)
                    filtroPedidos = filtroPedidos.Or(p => filtrosPesquisa.PedidosSelecionados.Contains(p.Codigo));

                if (filtrosPesquisa.TipoOperacao > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.TipoOperacao);

                if (filtrosPesquisa.CodigosCanalEntrega?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosCanalEntrega.Contains(obj.CanalEntrega.Codigo));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                    filtroPedidos = filtroPedidos.And(obj => obj.NumeroBooking == filtrosPesquisa.NumeroBooking);

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                    filtroPedidos = filtroPedidos.And(obj => obj.NumeroOS == filtrosPesquisa.NumeroOS);

                if (filtrosPesquisa.PedidoEmpresaResponsavel > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.PedidoEmpresaResponsavel.Codigo == filtrosPesquisa.PedidoEmpresaResponsavel);

                if (filtrosPesquisa.PedidoCentroCusto > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.PedidoCentroCusto.Codigo == filtrosPesquisa.PedidoCentroCusto);

                if (filtrosPesquisa.Container > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.Container.Codigo == filtrosPesquisa.Container);

                if (filtrosPesquisa.FiltrarPedidosCargaFechada)
                    filtroPedidos = filtroPedidos.And(obj => obj.PedidosCarga.All(cp => cp.Carga.CargaFechada));

                if (filtrosPesquisa.FiltarRementeDestinatarioPorTransportador)
                    filtroPedidos = filtroPedidos.And(obj => obj.Remetente.CPF_CNPJ == filtrosPesquisa.CodigoTransportador || obj.Destinatario.CPF_CNPJ == filtrosPesquisa.CodigoTransportador || obj.Recebedor.CPF_CNPJ == filtrosPesquisa.CodigoTransportador || obj.Expedidor.CPF_CNPJ == filtrosPesquisa.CodigoTransportador || obj.Remetente.CPF_CNPJ == filtrosPesquisa.CodigoTransportador);

                if (filtrosPesquisa.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas && filtrosPesquisa.CodigosGrupoPessoa?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.GrupoPessoas.Codigo) || filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Remetente.GrupoPessoas.Codigo));
                else if (filtrosPesquisa.CompartilharAcessoEntreGrupoPessoas && filtrosPesquisa.VisualizarApenasParaPedidoDesteTomador)
                {
                    if (filtrosPesquisa.CodigosGrupoPessoa?.Count > 0)
                        filtroPedidos = filtroPedidos.And(obj => (filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Tomador.GrupoPessoas.Codigo) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros) ||
                        (filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Remetente.GrupoPessoas.Codigo) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente) ||
                        (filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Destinatario.GrupoPessoas.Codigo) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario) ||
                        (filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Expedidor.GrupoPessoas.Codigo) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor) ||
                        (filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Recebedor.GrupoPessoas.Codigo) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor) ||
                        (filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Tomador.GrupoPessoas.Codigo) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Tomador));
                }
                else if (filtrosPesquisa.CompartilharAcessoEntreGrupoPessoas)
                {
                    if (filtrosPesquisa.CodigosGrupoPessoa?.Count > 0)
                        filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.GrupoPessoas.Codigo) || filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Destinatario.GrupoPessoas.Codigo) || filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Remetente.GrupoPessoas.Codigo) || filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Tomador.GrupoPessoas.Codigo));
                }
                else
                {
                    if (filtrosPesquisa.CodigosGrupoPessoa?.Count > 0)
                        filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.GrupoPessoas.Codigo) || filtrosPesquisa.CodigosGrupoPessoa.Contains(obj.Remetente.GrupoPessoas.Codigo));

                    if (filtrosPesquisa.GrupoPessoaDestinatario > 0)
                        filtroPedidos = filtroPedidos.And(obj => obj.Destinatario.GrupoPessoas.Codigo == filtrosPesquisa.GrupoPessoaDestinatario);
                }

                if (filtrosPesquisa.CodigosGrupoPessoaRetirada?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.TipoOperacao.ConfiguracaoPedido.FiltrarPedidosPorRemetenteRetiradaProduto
                        ? (filtrosPesquisa.CodigosGrupoPessoaRetirada.Contains(obj.GrupoPessoas.Codigo) || filtrosPesquisa.CodigosGrupoPessoaRetirada.Contains(obj.Remetente.GrupoPessoas.Codigo))
                        : filtrosPesquisa.CodigosGrupoPessoaRetirada.Contains(obj.Destinatario.GrupoPessoas.Codigo));

                if (filtrosPesquisa.CategoriaPessoa > 0)
                    filtroPedidos = filtroPedidos.And(obj => obj.Destinatario.Categoria.Codigo == filtrosPesquisa.CategoriaPessoa);

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DeliveryTerm))
                    filtroPedidos = filtroPedidos.And(obj => obj.DeliveryTerm == filtrosPesquisa.DeliveryTerm);

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.IdAutorizacao))
                    filtroPedidos = filtroPedidos.And(obj => obj.IdAutorizacao == filtrosPesquisa.IdAutorizacao);

                if (filtrosPesquisa.DataInclusaoBookingInicial.HasValue)
                    filtroPedidos = filtroPedidos.And(obj => obj.DataInclusaoBooking >= filtrosPesquisa.DataInclusaoBookingInicial.Value.Date);

                if (filtrosPesquisa.DataInclusaoBookingLimite.HasValue)
                    filtroPedidos = filtroPedidos.And(obj => obj.DataInclusaoBooking <= filtrosPesquisa.DataInclusaoBookingLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

                if (filtrosPesquisa.DataInclusaoPCPInicial.HasValue)
                    filtroPedidos = filtroPedidos.And(obj => obj.DataInclusaoPCP >= filtrosPesquisa.DataInclusaoPCPInicial.Value.Date);

                if (filtrosPesquisa.DataInclusaoPCPLimite.HasValue)
                    filtroPedidos = filtroPedidos.And(obj => obj.DataInclusaoPCP <= filtrosPesquisa.DataInclusaoPCPLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

                if (filtrosPesquisa.PesoDe > 0)
                    filtroPedidos = filtroPedidos.And(x => x.PesoTotal >= filtrosPesquisa.PesoDe);

                if (filtrosPesquisa.PesoAte > 0)
                    filtroPedidos = filtroPedidos.And(x => x.PesoTotal <= filtrosPesquisa.PesoAte);

                if (filtrosPesquisa.PalletDe > 0)
                    filtroPedidos = filtroPedidos.And(x => x.NumeroPaletes + x.NumeroPaletesFracionado >= filtrosPesquisa.PalletDe);

                if (filtrosPesquisa.PalletAte > 0)
                    filtroPedidos = filtroPedidos.And(x => x.NumeroPaletes + x.NumeroPaletesFracionado <= filtrosPesquisa.PalletAte);

                if (filtrosPesquisa.VolumeDe > 0)
                    filtroPedidos = filtroPedidos.And(x => x.CubagemTotal >= filtrosPesquisa.VolumeDe);

                if (filtrosPesquisa.VolumeAte > 0)
                    filtroPedidos = filtroPedidos.And(x => x.CubagemTotal <= filtrosPesquisa.VolumeAte);

                if (filtrosPesquisa.ValorDe > 0)
                    filtroPedidos = filtroPedidos.And(x => x.ValorTotalNotasFiscais >= filtrosPesquisa.ValorDe);

                if (filtrosPesquisa.ValorAte > 0)
                    filtroPedidos = filtroPedidos.And(x => x.ValorTotalNotasFiscais <= filtrosPesquisa.ValorAte);

                if (filtrosPesquisa.TipoCarga > 0)
                    filtroPedidos = filtroPedidos.And(x => x.TipoCarga.Codigo == filtrosPesquisa.TipoCarga);

                if (filtrosPesquisa.TipoDeCarga > 0)
                    filtroPedidos = filtroPedidos.And(x => x.TipoDeCarga.Codigo == filtrosPesquisa.TipoDeCarga);

                if (filtrosPesquisa.TiposDeCargas?.Count > 0)
                    filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.TiposDeCargas.Contains(obj.TipoDeCarga.Codigo));

                if (filtrosPesquisa.NaoRecebeCargaCompartilhada)
                {
                    var consultaClienteDescarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();

                    filtroPedidos = filtroPedidos.And(x => consultaClienteDescarga.Any(s => s.Cliente.CPF_CNPJ == x.Destinatario.CPF_CNPJ && s.NaoRecebeCargaCompartilhada));
                }

                if (filtrosPesquisa.TransportadoraMatriz > 0)
                    filtroPedidos = filtroPedidos.And(p => p.Empresa.Codigo == filtrosPesquisa.TransportadoraMatriz || p.Empresa.Matriz.Any(m => m.Codigo == filtrosPesquisa.TransportadoraMatriz));

                if (filtrosPesquisa.Transportador > 0 && !filtrosPesquisa.FiltroRetirada)
                    filtroPedidos = filtroPedidos.And(p => p.Empresa == null || p.Empresa.Codigo == filtrosPesquisa.Transportador);

                if (filtrosPesquisa.Transportador > 0 && filtrosPesquisa.FiltroRetirada)
                    filtroPedidos = filtroPedidos.And(p => p.Empresa.Codigo == filtrosPesquisa.Transportador);

                if (filtrosPesquisa.SomentePedidosComNota)
                    filtroPedidos = filtroPedidos.And(p => p.NotasFiscais.Count > 0);

                if (filtrosPesquisa.SomentePedidosSemNota)
                    filtroPedidos = filtroPedidos.And(p => p.NotasFiscais.Count == 0);

                if (filtrosPesquisa.PedidoParaReentrega)
                    filtroPedidos = filtroPedidos.And(p => p.ReentregaSolicitada);

                //Se o pedido for 'Regular' ou 'Reentrega'
                if (filtrosPesquisa.PedidosSessao == 2 || filtrosPesquisa.PedidosSessao == 3)
                {
                    if (filtrosPesquisa.PedidosSessao == 2) //Todos os pedidos que não possuem reentrega
                        filtroPedidos = filtroPedidos.And(p => !p.ReentregaSolicitada);
                    else if (filtrosPesquisa.PedidosSessao == 3) //Apenas os pedidos que possuem reentrega
                        filtroPedidos = filtroPedidos.And(p => p.ReentregaSolicitada);
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                    filtroPedidos = filtroPedidos.And(obj => obj.NumeroEXP.Contains(filtrosPesquisa.NumeroEXP));

                if (filtrosPesquisa.DataCriacaoPedidoInicio.HasValue)
                    filtroPedidos = filtroPedidos.And(o => o.DataCriacao >= filtrosPesquisa.DataCriacaoPedidoInicio.Value.Date);

                if (filtrosPesquisa.DataCriacaoPedidoLimite.HasValue)
                    filtroPedidos = filtroPedidos.And(o => o.DataCriacao < filtrosPesquisa.DataCriacaoPedidoLimite.Value.Date.AddDays(1));

                if (filtrosPesquisa.Deposito > 0)
                    filtroPedidos = filtroPedidos.And(x => x.Deposito.Codigo == filtrosPesquisa.Deposito);

                if (filtrosPesquisa.CodigosCategoriaClientes?.Count > 0)
                {
                    if ((filtrosPesquisa?.TipoRoteirizacaoColetaEntrega ?? TipoRoteirizacaoColetaEntrega.Entrega) == TipoRoteirizacaoColetaEntrega.Coleta)
                        filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosCategoriaClientes.Contains(obj.Remetente.Categoria.Codigo));
                    else
                        filtroPedidos = filtroPedidos.And(obj => filtrosPesquisa.CodigosCategoriaClientes.Contains(obj.Destinatario.Categoria.Codigo));
                }

                if (filtrosPesquisa.CodigosProdutos?.Count > 0)
                {
                    var consultaProdutos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                    filtroPedidos = filtroPedidos.And(x => consultaProdutos.Any(s => s.Pedido.Codigo == x.Codigo && filtrosPesquisa.CodigosProdutos.Contains(s.Produto.Codigo)));
                }

                if (filtrosPesquisa.CodigosGrupoProdutos?.Count > 0)
                {
                    var consultaProdutos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                    filtroPedidos = filtroPedidos.And(x => consultaProdutos.Any(s => s.Pedido.Codigo == x.Codigo && filtrosPesquisa.CodigosGrupoProdutos.Contains(s.Produto.GrupoProduto.Codigo)));
                }

                if (filtrosPesquisa.CodigosLinhaSeparacao?.Count > 0)
                {
                    var consultaProdutos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
                    filtroPedidos = filtroPedidos.And(x => consultaProdutos.Any(s => s.Pedido.Codigo == x.Codigo && filtrosPesquisa.CodigosLinhaSeparacao.Contains(s.LinhaSeparacao.Codigo)));
                }

                if (filtrosPesquisa.CodigoAutor > 0)
                    filtroPedidos = filtroPedidos.And(x => x.Autor.Codigo == filtrosPesquisa.CodigoAutor);

                if (filtrosPesquisa.UsarTipoTomadorPedido)
                {
                    filtroPedidos = filtroPedidos.And(x => x.TipoTomador == filtrosPesquisa.TipoTomador);
                    if ((filtrosPesquisa.Tomadores?.Count ?? 0) > 0)
                    {
                        switch (filtrosPesquisa.TipoTomador)
                        {
                            case Dominio.Enumeradores.TipoTomador.Remetente:
                                filtroPedidos = filtroPedidos.And(x => filtrosPesquisa.Tomadores.Contains(x.Remetente.Codigo));
                                break;
                            case Dominio.Enumeradores.TipoTomador.Destinatario:
                                filtroPedidos = filtroPedidos.And(x => filtrosPesquisa.Tomadores.Contains(x.Destinatario.Codigo));
                                break;
                            case Dominio.Enumeradores.TipoTomador.Expedidor:
                                filtroPedidos = filtroPedidos.And(x => filtrosPesquisa.Tomadores.Contains(x.Expedidor.Codigo));
                                break;
                            case Dominio.Enumeradores.TipoTomador.Recebedor:
                                filtroPedidos = filtroPedidos.And(x => filtrosPesquisa.Tomadores.Contains(x.Recebedor.Codigo));
                                break;
                            case Dominio.Enumeradores.TipoTomador.Tomador:
                                filtroPedidos = filtroPedidos.And(x => filtrosPesquisa.Tomadores.Contains(x.Tomador.Codigo));
                                break;
                        }
                    }
                }
                else if (filtrosPesquisa.TiposTomador?.Count > 0)
                    filtroPedidos = filtroPedidos.And(x => filtrosPesquisa.TiposTomador.Contains(x.TipoTomador));
                else
                {
                    if (filtrosPesquisa.Tomadores?.Count > 0)
                        filtroPedidos = filtroPedidos.And(obj => (filtrosPesquisa.Tomadores.Contains(obj.Tomador.CPF_CNPJ) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros) ||
                        (filtrosPesquisa.Tomadores.Contains(obj.Remetente.CPF_CNPJ) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente) ||
                        (filtrosPesquisa.Tomadores.Contains(obj.Destinatario.CPF_CNPJ) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario) ||
                        (filtrosPesquisa.Tomadores.Contains(obj.Expedidor.CPF_CNPJ) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor) ||
                        (filtrosPesquisa.Tomadores.Contains(obj.Recebedor.CPF_CNPJ) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor) ||
                        (filtrosPesquisa.Tomadores.Contains(obj.Tomador.CPF_CNPJ) && obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Tomador));
                }

                if (filtrosPesquisa?.CodigosRotaFrete?.Count > 0)
                    filtroPedidos = filtroPedidos.And(o => filtrosPesquisa.CodigosRotaFrete.Contains(o.RotaFrete.Codigo));

                if (filtrosPesquisa?.CodigosUsuario?.Count > 0)
                    filtroPedidos = filtroPedidos.And(o => filtrosPesquisa.CodigosUsuario.Contains(o.Usuario.Codigo));

                if (!string.IsNullOrEmpty(filtrosPesquisa?.UsuarioRemessa))
                    filtroPedidos = filtroPedidos.And(o => o.UsuarioCriacaoRemessa.Contains(filtrosPesquisa.UsuarioRemessa));

                if (filtrosPesquisa.PrevisaoDataInicial.HasValue)
                    filtroPedidos = filtroPedidos.And(o => o.PrevisaoEntrega >= filtrosPesquisa.PrevisaoDataInicial.Value);

                if (filtrosPesquisa.PrevisaoDataFinal.HasValue)
                    filtroPedidos = filtroPedidos.And(o => o.PrevisaoEntrega <= filtrosPesquisa.PrevisaoDataFinal.Value);

                if (filtrosPesquisa.CodigoProcessamentoEspecial + filtrosPesquisa.CodigoHorarioEntrega + filtrosPesquisa.CodigoZonaTransporte + filtrosPesquisa.CodigoDetalheEntrega > 0)
                {
                    var consultaAdicionais = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>();
                    if (filtrosPesquisa.CodigoProcessamentoEspecial > 0)
                        filtroPedidos = filtroPedidos.And(x => consultaAdicionais.Any(s => s.Pedido.Codigo == x.Codigo && s.ProcessamentoEspecial.Codigo == filtrosPesquisa.CodigoProcessamentoEspecial));

                    if (filtrosPesquisa.CodigoHorarioEntrega > 0)
                        filtroPedidos = filtroPedidos.And(x => consultaAdicionais.Any(s => s.Pedido.Codigo == x.Codigo && s.HorarioEntrega.Codigo == filtrosPesquisa.CodigoHorarioEntrega));

                    if (filtrosPesquisa.CodigoZonaTransporte > 0)
                        filtroPedidos = filtroPedidos.And(x => consultaAdicionais.Any(s => s.Pedido.Codigo == x.Codigo && s.ZonaTransporte.Codigo == filtrosPesquisa.CodigoZonaTransporte));

                    if (filtrosPesquisa.CodigoDetalheEntrega > 0)
                        filtroPedidos = filtroPedidos.And(x => consultaAdicionais.Any(s => s.Pedido.Codigo == x.Codigo && s.DetalheEntrega.Codigo == filtrosPesquisa.CodigoDetalheEntrega));
                }

                if (filtrosPesquisa.RestricaoDiasEntrega?.Count > 0)
                {
                    var consultaRestricaoDiasEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega>();
                    filtroPedidos = filtroPedidos.And(x => consultaRestricaoDiasEntrega.Any(s => s.Pedido.Codigo == x.Codigo && filtrosPesquisa.RestricaoDiasEntrega.Contains(s.Dia)));
                }
                if (filtrosPesquisa.CodigosRestricoesEntrega?.Count > 0)
                {
                    var consultaRestricoesDescarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();
                    filtroPedidos = filtroPedidos.And(x => consultaRestricoesDescarga.Any(s => (s.Cliente.CPF_CNPJ == x.Destinatario.CPF_CNPJ || s.Cliente.CPF_CNPJ == x.Recebedor.CPF_CNPJ) && s.RestricoesDescarga.Any(r => filtrosPesquisa.CodigosRestricoesEntrega.Contains(r.Codigo))));
                }
            }

            if (filtrosPesquisa.ProgramaComSessaoRoteirizador)
            {
                filtroPedidos = filtroPedidos.And(x => x.PedidoDeDevolucao == false);

                if (filtrosPesquisa.CodigoSessaoRoteirizador > 0 && filtrosPesquisa.OpcaoSessaoRoteirizador == OpcaoSessaoRoteirizador.ABRIR_SESSAO)
                {
                    var consultaSessaoRoteirizadorPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();

                    filtroPedidos = filtroPedidos.And(x => consultaSessaoRoteirizadorPedido.Any(s => s.Pedido.Codigo == x.Codigo &&
                                                                                                     s.Situacao != SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao &&
                                                                                                     s.SessaoRoteirizador.Codigo == filtrosPesquisa.CodigoSessaoRoteirizador));
                }
                else if (filtrosPesquisa.CodigoSessaoRoteirizador > 0 && filtrosPesquisa.OpcaoSessaoRoteirizador == OpcaoSessaoRoteirizador.ADD_PEDIDOS_SESSAO)
                {
                    if (filtrosPesquisa.CodigosLinhaSeparacao?.Count > 0)
                    {
                        var consultaProdutos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>().Where(s => filtrosPesquisa.CodigosLinhaSeparacao.Contains(s.LinhaSeparacao.Codigo));
                        IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> consultaSessaoRoteirizadorPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();

                        filtroPedidos = filtroPedidos.And(x => !consultaProdutos.Any(s => s.Pedido.Codigo == x.Codigo &&
                                                                                            consultaSessaoRoteirizadorPedidoProduto.Any(p => p.PedidoProduto.Codigo == s.Codigo &&
                                                                                                                                                p.SessaoRoteirizadorPedido.Codigo != filtrosPesquisa.CodigoSessaoRoteirizador &&
                                                                                                                                                p.SessaoRoteirizadorPedido.Situacao != SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao &&
                                                                                                                                                p.SessaoRoteirizadorPedido.SessaoRoteirizador.SituacaoSessaoRoteirizador == SituacaoSessaoRoteirizador.Iniciada)));
                    }
                    else
                    {
                        // Pesquisa todos os pedidos da sessão atual e não podem estar em outra sessão em andamento...
                        IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();
                        IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> subQueryOuther = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();

                        filtroPedidos = filtroPedidos.And(x => subQuery.Any(s => s.Pedido.Codigo == x.Codigo &&
                                                                                 s.SessaoRoteirizador.Codigo == filtrosPesquisa.CodigoSessaoRoteirizador &&
                                                                                 s.Situacao != SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao) ||
                                                               !subQueryOuther.Any(o => o.Pedido.Codigo == x.Codigo &&
                                                                                        o.Situacao != SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao &&
                                                                                        o.SessaoRoteirizador.Codigo != filtrosPesquisa.CodigoSessaoRoteirizador &&
                                                                                        o.SessaoRoteirizador.SituacaoSessaoRoteirizador == SituacaoSessaoRoteirizador.Iniciada)
                        );
                    }
                }
                else if (filtrosPesquisa.OpcaoSessaoRoteirizador == OpcaoSessaoRoteirizador.CRIAR_NOVA)
                {
                    if (filtrosPesquisa.HabilitarCadastroArmazem)
                    {
                        var filialArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialArmazem>().Where(fa => filtrosPesquisa.CodigosFilial.Contains(fa.Filial.Codigo)).FirstOrDefault();

                        if (filialArmazem != null)
                        {
                            var consultaProdutoEstoqueArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem>().Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Filial.Codigo) && o.EstoqueDisponivel > 0);
                            var consultaProdutosPedidos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>().Where(o => filtrosPesquisa.CodigosFilial.Contains(o.FilialArmazem.Filial.Codigo));

                            filtroPedidos = filtroPedidos.And(p => consultaProdutosPedidos.Any(cpp => consultaProdutoEstoqueArmazem.Where(cpe => cpe.Produto.Codigo == cpp.Produto.Codigo &&
                                                                                                                                                   cpe.Armazem.Codigo == cpp.FilialArmazem.Codigo).Count() >= 1 &&
                                                                                                                                                   p.Codigo == cpp.Pedido.Codigo));
                        }
                    }

                    if (filtrosPesquisa.CodigosLinhaSeparacao?.Count > 0)
                    {
                        var consultaProdutos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>().Where(s => filtrosPesquisa.CodigosLinhaSeparacao.Contains(s.LinhaSeparacao.Codigo));
                        var consultaSessaoRoteirizadorPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();

                        filtroPedidos = filtroPedidos.And(x => !consultaProdutos.Any(s => s.Pedido.Codigo == x.Codigo &&
                                                                                            consultaSessaoRoteirizadorPedidoProduto.Any(p => p.PedidoProduto.Codigo == s.Codigo &&
                                                                                                                                                p.SessaoRoteirizadorPedido.Situacao != SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao &&
                                                                                                                                                p.SessaoRoteirizadorPedido.SessaoRoteirizador.SituacaoSessaoRoteirizador == SituacaoSessaoRoteirizador.Iniciada)));
                    }
                    else
                    {
                        // Os pedidos não podem conter em outra sessão de roteirização ou os mesmos foram removidos...
                        var consultaSessaoRoteirizadorPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();

                        filtroPedidos = filtroPedidos.And(x => !consultaSessaoRoteirizadorPedido.Any(s => s.Pedido.Codigo == x.Codigo &&
                                                                                                          s.Situacao != SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao &&
                                                                                                          s.SessaoRoteirizador.SituacaoSessaoRoteirizador == SituacaoSessaoRoteirizador.Iniciada));
                    }
                }

                if (filtrosPesquisa.SituacaoSessaoRoteirizador != SituacaoSessaoRoteirizador.Finalizada)
                {
                    // Agora precisamos ver o saldo do pedido
                    // Vamos ver todos os pedidos que tem saldo em relação as cargas fechadas.
                    // Adicionado ajuste técnino de primeira para somar 0.05 ao peso dos pedidos em carga para tratar problemas de arredondamento
                    var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.CargaPreCarga == null && o.Carga.TipoOperacao.ConfiguracaoCarga.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga == false);
                    var consultaOcorrenciaEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>().Where(o => o.TipoDeOcorrencia.DisponibilizarPedidoParaNovaIntegracao);
                    if (!filtrosPesquisa.PedidosOrigemRecebedor)
                    {
                        filtroPedidos = filtroPedidos.And(x => x.PesoTotal > Decimal.Round(consultaCargaPedido.Where(p => p.Pedido.Codigo == x.Codigo && !consultaOcorrenciaEntrega.Any(o => o.Carga.Codigo == p.Carga.Codigo && o.Pedido.Codigo == x.Codigo)).Sum(c => c.Peso) + 0.05m, 3) ||
                                                               !consultaCargaPedido.Any(p => p.Pedido.Codigo == x.Codigo && !consultaOcorrenciaEntrega.Any(o => o.Carga.Codigo == p.Carga.Codigo && o.Pedido.Codigo == x.Codigo)));
                    }
                    else
                    {
                        filtroPedidos = filtroPedidos.And(x => x.PesoTotal > Decimal.Round(consultaCargaPedido.Where(p => p.Pedido.Codigo == x.Codigo && !consultaOcorrenciaEntrega.Any(o => o.Carga.Codigo == p.Carga.Codigo && o.Pedido.Codigo == x.Codigo)).Sum(c => c.Peso) + 0.05m, 3) ||
                                                               !consultaCargaPedido.Any(p => p.Pedido.Codigo == x.Codigo && p.Expedidor != null && !consultaOcorrenciaEntrega.Any(o => o.Carga.Codigo == p.Carga.Codigo && o.Pedido.Codigo == x.Codigo)));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ProcImportacao))
                consultaPedido = consultaPedido.Where(obj => obj.Adicional1 == filtrosPesquisa.ProcImportacao);

            if (filtrosPesquisa.CodigosRegiaoDestino?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosRegiaoDestino.Contains(obj.Destino.Regiao.Codigo));


            if (filtrosPesquisa.CodigosVendedor != null && filtrosPesquisa.CodigosVendedor.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosVendedor.Contains(obj.FuncionarioVendedor.Codigo));

            if (filtrosPesquisa.CodigosSupervisor != null && filtrosPesquisa.CodigosSupervisor.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosSupervisor.Contains(obj.FuncionarioSupervisor.Codigo));

            if (filtrosPesquisa.CodigosGerente != null && filtrosPesquisa.CodigosGerente.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosGerente.Contains(obj.FuncionarioGerente.Codigo));

            if (filtrosPesquisa.CargaPerigosa.HasValue)
            {
                if (filtrosPesquisa.CargaPerigosa == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    filtroPedidos = filtroPedidos.And(o => o.TipoDeCarga.PossuiCargaPerigosa == true);
                else if (filtrosPesquisa.CargaPerigosa == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                    filtroPedidos = filtroPedidos.And(o => o.TipoDeCarga.PossuiCargaPerigosa == false || o.TipoDeCarga.PossuiCargaPerigosa == null);
            }

            if (filtrosPesquisa.NumeroProtocoloIntegracaoPedido > 0)
                filtroPedidos = filtroPedidos.And(obj => obj.Protocolo == filtrosPesquisa.NumeroProtocoloIntegracaoPedido);

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosCentroResultado.Contains(obj.CentroResultado.Codigo));

            if (filtrosPesquisa.CodigosFuncionarioResponsavel?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosFuncionarioResponsavel.Contains(obj.VeiculoTracao.FuncionarioResponsavel.Codigo) || obj.Veiculos.Any(v => filtrosPesquisa.CodigosFuncionarioResponsavel.Contains(v.FuncionarioResponsavel.Codigo)));

            if (filtrosPesquisa.CodigosFronteiras?.Count > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira> consultaPedidoFronteira = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>();
                filtroPedidos = filtroPedidos
                    .And(x => consultaPedidoFronteira
                    .Any(s => s.Pedido.Codigo == x.Codigo && (filtrosPesquisa.CodigosFronteiras.Contains(s.Fronteira.CPF_CNPJ) || (x.Fronteira != null && filtrosPesquisa.CodigosFronteiras.Contains(x.Fronteira.CPF_CNPJ)))));
            }

            if (filtrosPesquisa.CodigosModelosVeicularesCarga?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosModelosVeicularesCarga.Contains(obj.ModeloVeicularCarga.Codigo));

            if (filtrosPesquisa.CodigosSegmentosVeiculos?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosSegmentosVeiculos.Contains(obj.VeiculoTracao.SegmentoVeiculo.Codigo) || obj.Veiculos.Any(o => filtrosPesquisa.CodigosSegmentosVeiculos.Contains(o.SegmentoVeiculo.Codigo)));

            if (filtrosPesquisa.AceiteMotorista.HasValue)
                consultaPedido = consultaPedido.Where(obj => obj.AceiteMotorista == filtrosPesquisa.AceiteMotorista);

            if (filtrosPesquisa.PedidosBloqueados.HasValue)
            {
                if (filtrosPesquisa.PedidosBloqueados.Value == SimNao.Sim)
                    consultaPedido = consultaPedido.Where(obj => obj.PedidoBloqueado);
                else if (filtrosPesquisa.PedidosBloqueados.Value == SimNao.Nao)
                    consultaPedido = consultaPedido.Where(obj => !obj.PedidoBloqueado);
            }

            if (filtrosPesquisa.PedidosRestricaoData.HasValue)
            {
                if (filtrosPesquisa.PedidosRestricaoData.Value == SimNao.Sim)
                    consultaPedido = consultaPedido.Where(obj => obj.PedidoRestricaoData);
                else if (filtrosPesquisa.PedidosRestricaoData.Value == SimNao.Nao)
                    consultaPedido = consultaPedido.Where(obj => !obj.PedidoRestricaoData);
            }

            if (filtrosPesquisa.PedidosRestricaoPercentual.HasValue)
            {
                if (filtrosPesquisa.PedidosRestricaoPercentual.Value == SimNao.Sim)
                    consultaPedido = consultaPedido.Where(obj => obj.PercentualSeparacaoPedido != 100);
                else if (filtrosPesquisa.PedidosRestricaoPercentual.Value == SimNao.Nao)
                    consultaPedido = consultaPedido.Where(obj => obj.PercentualSeparacaoPedido == 100);
            }

            if (filtrosPesquisa.DataAgendamentoInicial.HasValue)
                consultaPedido = consultaPedido.Where(obj => obj.DataAgendamento.Value.Date >= filtrosPesquisa.DataAgendamentoInicial.Value);

            if (filtrosPesquisa.DataAgendamentoFinal.HasValue)
                consultaPedido = consultaPedido.Where(obj => obj.DataAgendamento.Value.Date <= filtrosPesquisa.DataAgendamentoFinal.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ChaveNotaFiscalEletronica))
                consultaPedido = consultaPedido.Where(obj => obj.NotasFiscais.Any(x => x.Chave == filtrosPesquisa.ChaveNotaFiscalEletronica));

            if (filtrosPesquisa.NotaFiscal?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => obj.NotasFiscais.Any(o => filtrosPesquisa.NotaFiscal.Contains(o.Codigo)));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOrdem))
                filtroPedidos = filtroPedidos.And(obj => obj.NumeroOrdem.Contains(filtrosPesquisa.NumeroOrdem));

            if (filtrosPesquisa.SituacaoComercialPedido > 0 && !filtrosPesquisa.BloquearSituacaoComercialPedido)
                consultaPedido = consultaPedido.Where(obj => obj.SituacaoComercialPedido.Codigo == filtrosPesquisa.SituacaoComercialPedido);

            if (filtrosPesquisa.BloquearSituacaoComercialPedido)
                consultaPedido = consultaPedido.Where(obj => obj.SituacaoComercialPedido == null || obj.SituacaoComercialPedido.BloqueiaPedido == false);

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && filtrosPesquisa.FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador && filtrosPesquisa.Recebedor > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consultaCargaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(o => o.Recebedor != null && o.Recebedor.CPF_CNPJ == filtrosPesquisa.Recebedor);
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.Recebedor == obj.Recebedor.CPF_CNPJ || consultaCargaPedido.Any(o => o.Pedido.Codigo == obj.Codigo));
            }

            if (filtrosPesquisa.ProvedorOS > 0)
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.ProvedorOS == obj.ProvedorOS.CPF_CNPJ);

            if (filtrosPesquisa.CategoriaOS?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => obj.CategoriaOS != null && filtrosPesquisa.CategoriaOS.Contains(obj.CategoriaOS ?? default));

            if (filtrosPesquisa.TipoOSConvertido?.Count > 0)
                consultaPedido = consultaPedido.Where(obj => obj.TipoOSConvertido != null && filtrosPesquisa.TipoOSConvertido.Contains(obj.TipoOSConvertido ?? default));

            if (filtrosPesquisa.UsuarioUtilizaSegregacaoPorProvedor && filtrosPesquisa.CodigosProvedores.Count > 0)
                consultaPedido = consultaPedido.Where(obj => obj.ProvedorOS != null && filtrosPesquisa.CodigosProvedores.Contains(obj.ProvedorOS.CPF_CNPJ));

            if (filtrosPesquisa.CodigosRegiao != null && filtrosPesquisa.CodigosRegiao.Count > 0 && filtrosPesquisa.CodigosMesorregiao != null && filtrosPesquisa.CodigosMesorregiao.Count > 0)
            {
                consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosRegiao.Contains(obj.Remetente.Regiao.Codigo)
                                                          || filtrosPesquisa.CodigosMesorregiao.Contains(obj.Remetente.MesoRegiao.Codigo));
            }
            else
            {
                if (filtrosPesquisa.CodigosRegiao != null && filtrosPesquisa.CodigosRegiao.Count > 0)
                    consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosRegiao.Contains(obj.Remetente.Regiao.Codigo));

                if (filtrosPesquisa.CodigosMesorregiao != null && filtrosPesquisa.CodigosMesorregiao.Count > 0)
                    consultaPedido = consultaPedido.Where(obj => filtrosPesquisa.CodigosMesorregiao.Contains(obj.Remetente.MesoRegiao.Codigo));
            }

            if (filtrosPesquisa.TendenciaEntrega != TendenciaEntrega.Nenhum)
            {
                var configuracaoTempoTendendicas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao>().FirstOrDefault();

                if (configuracaoTempoTendendicas != null)
                {
                    DateTime dataReferenciaBase = DateTime.Now;

                    if (filtrosPesquisa.TendenciaEntrega == TendenciaEntrega.Adiantado)
                    {
                        DateTime limiteDestinoEmTempo = dataReferenciaBase.AddMinutes(-configuracaoTempoTendendicas.DestinoEmTempo.TotalMinutes);

                        consultaPedido = consultaPedido.Where(pedido => pedido.PrevisaoEntrega.HasValue && pedido.PrevisaoEntrega.Value >= limiteDestinoEmTempo);
                    }

                    if (filtrosPesquisa.TendenciaEntrega == TendenciaEntrega.Nohorario)
                    {
                        DateTime limiteDestinoAtraso = dataReferenciaBase.AddMinutes(-configuracaoTempoTendendicas.DestinoAtraso1.TotalMinutes);
                        DateTime limiteDestinoEmTempo = dataReferenciaBase.AddMinutes(-configuracaoTempoTendendicas.DestinoEmTempo.TotalMinutes);

                        consultaPedido = consultaPedido.Where(pedido => pedido.PrevisaoEntrega.HasValue
                                                                     && pedido.PrevisaoEntrega.Value < limiteDestinoEmTempo
                                                                     && pedido.PrevisaoEntrega.Value >= limiteDestinoAtraso);
                    }

                    if (filtrosPesquisa.TendenciaEntrega == TendenciaEntrega.Poucoatrasado)
                    {
                        DateTime limiteDestinoAtraso2 = dataReferenciaBase.AddMinutes(-configuracaoTempoTendendicas.DestinoAtraso2.TotalMinutes);
                        DateTime limiteDestinoAtraso1 = dataReferenciaBase.AddMinutes(-configuracaoTempoTendendicas.DestinoAtraso1.TotalMinutes);


                        consultaPedido = consultaPedido.Where(pedido => pedido.PrevisaoEntrega.HasValue
                                                                     && pedido.PrevisaoEntrega.Value < limiteDestinoAtraso1
                                                                     && pedido.PrevisaoEntrega.Value >= limiteDestinoAtraso2);
                    }

                    if (filtrosPesquisa.TendenciaEntrega == TendenciaEntrega.Atrasado)
                    {
                        DateTime limiteDestinoAtraso = dataReferenciaBase.AddMinutes(-configuracaoTempoTendendicas.DestinoAtraso3.TotalMinutes);

                        consultaPedido = consultaPedido.Where(pedido => pedido.PrevisaoEntrega.HasValue && pedido.PrevisaoEntrega.Value < limiteDestinoAtraso);
                    }
                }
            }

            consultaPedido = consultaPedido.Where(filtroPedidos);

            return consultaPedido;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultaPendentesPorRemetente(double cnpjRemetete, double cnpjDestinatario, int grupoPessoa, List<string> pedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query
                         where
                            obj.PedidoTotalmenteCarregado == false
                            && obj.DataValidade >= DateTime.Now
                            && obj.SituacaoPedido != SituacaoPedido.Cancelado
                            && obj.TipoOperacao.TipoOperacaoAgendamento
                         select obj;

            if (cnpjRemetete > 0)
                result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjRemetete);

            if (cnpjDestinatario > 0)
                result = result.Where(o => o.Destinatario.CPF_CNPJ == cnpjDestinatario);

            if (grupoPessoa > 0)
                result = result.Where(o => o.Remetente.GrupoPessoas.Codigo == grupoPessoa);

            if (pedidos.Count() > 0)
                result = result.Where(o => pedidos.Contains(o.NumeroPedidoEmbarcador));

            return result;
        }

        private void ExecuteDeletaPedidoEProdutosPorCodigoPedido(int codigoPedido)
        {
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO_PRODUTO WHERE PED_CODIGO = :codigoPedido;").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_PEDIDO WHERE PED_CODIGO = :codigoPedido;").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
        }

        private NHibernate.ISQLQuery ObterSQLCTesPorPedido(int codigoPedido)
        {
            string sqlQuery = $@" 
                SELECT DISTINCT CTe.CON_CODIGO
                FROM VIEW_PEDIDO_CTE PedidoCTe
                LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = PedidoCTe.CON_CODIGO
                WHERE PedidoCTe.PED_CODIGO = {codigoPedido} 
            ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            return query;
        }

        private NHibernate.ISQLQuery ObterSQLPedidosClientes(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sqlQuery = "SELECT ";
            string sqlWhere = "";

            if (parametrosConsulta == null)
                sqlQuery += "COUNT(*)";
            else
            {
                sqlQuery += $@"
                    Pedido.PED_CODIGO Codigo,
                    Pedido.PED_SITUACAO_ACOMPANHAMENTO_PEDIDO CodigoStatus,
                    Pedido.PED_PESO_TOTAL_CARGA Peso,
                    Pedido.PED_DATA_ENTREGA Entrega,
                    Pedido.PED_PREVISAO_ENTREGA PrevisaoEntrega,
                    Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido,
                    (
                        SELECT TOP 1 _cte.CON_DATAHORAEMISSAO
                        FROM VIEW_PEDIDO_CTE _pedido_cte
                        JOIN T_CTE _cte on _cte.CON_CODIGO = _pedido_cte.CON_CODIGO
                        JOIN T_CTE_PARTICIPANTE _tomadorCTe on _tomadorCTe.PCT_CODIGO = _cte.CON_TOMADOR_PAGADOR_CTE
                        WHERE _pedido_cte.PED_CODIGO = Pedido.PED_CODIGO
                        AND _tomadorCTe.PCT_CPF_CNPJ = Remetente.CLI_CGCCPF
                    ) Emissao,
                    Destinatario.CLI_NOME Cliente,
                    CONCAT(LocalidadeDestino.LOC_DESCRICAO, ' - ', LocalidadeDestino.UF_SIGLA) Destino,
                    SUBSTRING((
                        SELECT ', ' + CONVERT(NVARCHAR(50), _cte.CON_NUM) 
                        FROM VIEW_PEDIDO_CTE _pedido_cte
                        LEFT JOIN T_CTE _cte on _cte.CON_CODIGO = _pedido_cte.CON_CODIGO
                        WHERE _pedido_cte.PED_CODIGO = Pedido.PED_CODIGO
                        GROUP BY _cte.CON_NUM
                    FOR XML PATH('')), 3, 1000) CTes,
                    (
                        SELECT COUNT(_pedido_xml.NFX_CODIGO)
                        FROM VIEW_PEDIDO_XML _pedido_xml
                        LEFT JOIN T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedido_xml.NFX_CODIGO
                        WHERE _pedido_xml.PED_CODIGO = Pedido.PED_CODIGO
                    ) QuantidadeNotas,
                    (
                        SELECT COUNT(_pedido_xml.NFX_CODIGO)
                        FROM VIEW_PEDIDO_XML _pedido_xml
                        LEFT JOIN T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedido_xml.NFX_CODIGO
                        WHERE _pedido_xml.PED_CODIGO = Pedido.PED_CODIGO AND _xmlnotafiscal.NF_SITUACAO_ENTREGA in ({(int)SituacaoNotaFiscal.Entregue}, {(int)SituacaoNotaFiscal.DevolvidaParcial})
                    ) QuantidadeNotasEntregues,
                    SUBSTRING((
                        SELECT ', ' + CONVERT(NVARCHAR(50), NotaFiscal.NF_NUMERO) 
		                    FROM T_PEDIDO_NOTAS_FISCAIS PedidoNotasFiscais
			                LEFT JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNotasFiscais.NFX_CODIGO
		                    WHERE PedidoNotasFiscais.PED_CODIGO = Pedido.PED_CODIGO
		                    FOR XML PATH('')
	                ), 3, 1000) NotasFiscais
                ";
            }

            sqlQuery += @"
                FROM T_PEDIDO Pedido
                LEFT JOIN T_LOCALIDADES LocalidadeDestino ON LocalidadeDestino.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO
                LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO 
                LEFT JOIN T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE 
            ";

            List<SituacaoAcompanhamentoPedido> situacoesAcompanhamentoPedidoPendente = SituacaoAcompanhamentoPedidoHelper.ObterSituacoesAcompanhamentoPedidoPendente();
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataInicial = dataAtual.FirstDayOfMonth();
            DateTime dataLimite = dataAtual.LastDayOfMonth().Add(DateTime.MaxValue.TimeOfDay);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoFiltro))
                situacoesAcompanhamentoPedidoPendente.Add(SituacaoAcompanhamentoPedido.Entregue);

            sqlWhere += $" AND Pedido.PED_SITUACAO <> {(int)SituacaoPedido.Cancelado} ";

            if (!filtrosPesquisa.DataInicial.HasValue && (!filtrosPesquisa.DataLimite.HasValue || filtrosPesquisa.DataLimite.Value >= dataInicial))
                sqlWhere += $" AND Pedido.PED_DATA_CRIACAO >= '{dataInicial.ToString("yyyy-MM-dd HH:mm:ss")}' ";
            else if (filtrosPesquisa.DataInicial.HasValue)
                sqlWhere += $" AND Pedido.PED_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.Value.Date.ToString("yyyyMMdd HH:mm:ss")}' ";

            if (!filtrosPesquisa.DataLimite.HasValue && (!filtrosPesquisa.DataInicial.HasValue || filtrosPesquisa.DataInicial.Value <= dataLimite))
                sqlWhere += $" AND Pedido.PED_DATA_CRIACAO <= '{dataLimite.ToString("yyyy-MM-dd HH:mm:ss")}' ";
            else if (filtrosPesquisa.DataLimite.HasValue)
                sqlWhere += $" AND Pedido.PED_DATA_CRIACAO <= '{filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ";

            if (!filtrosPesquisa.SituacaoAcompanhamentoPedido.HasValue || filtrosPesquisa.SituacaoAcompanhamentoPedido == SituacaoAcompanhamentoPedido.Todos)
                sqlWhere += $" AND Pedido.PED_SITUACAO_ACOMPANHAMENTO_PEDIDO IN ({string.Join(", ", situacoesAcompanhamentoPedidoPendente.Select(x => (int)x).ToList())}) ";
            else
            {
                if (filtrosPesquisa.SituacaoAcompanhamentoPedido.Value == SituacaoAcompanhamentoPedido.AgColeta)
                    sqlWhere += $" AND Pedido.PED_SITUACAO_ACOMPANHAMENTO_PEDIDO IN ({string.Join(", ", new List<int> { (int)SituacaoAcompanhamentoPedido.AgColeta, (int)SituacaoAcompanhamentoPedido.ColetaAgendada })}) ";
                else
                    sqlWhere += $" AND Pedido.PED_SITUACAO_ACOMPANHAMENTO_PEDIDO = {(int)filtrosPesquisa.SituacaoAcompanhamentoPedido.Value} ";
            }

            if (filtrosPesquisa.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                sqlWhere += " AND Pedido.PED_PROTOCOLO <> 0 ";

            if (filtrosPesquisa.CodigoGrupoPessoaClienteFornecedor > 0)
                sqlWhere += $" AND (Remetente.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoaClienteFornecedor} OR Destinatario.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoaClienteFornecedor}) ";

            if (filtrosPesquisa.ListaCodigosGruposPessoaPortalAcesso != null && filtrosPesquisa.ListaCodigosGruposPessoaPortalAcesso.Count > 0)
                sqlWhere += $" AND (Remetente.GRP_CODIGO IN ({string.Join(", ", filtrosPesquisa.ListaCodigosGruposPessoaPortalAcesso)}) OR Destinatario.GRP_CODIGO IN ({string.Join(", ", filtrosPesquisa.ListaCodigosGruposPessoaPortalAcesso)})) ";

            if (filtrosPesquisa.CpfCnpjClienteFornecedor > 0d)
                sqlWhere += $" AND (Remetente.CLI_CGCCPF= {filtrosPesquisa.CpfCnpjClienteFornecedor} OR Destinatario.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjClienteFornecedor}) ";

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoVendedor))
                {
                    sqlWhere += $" AND Pedido.PED_VENDEDOR = '{filtrosPesquisa.CodigoVendedor}' ";
                }
            }

            int filtroNumeros = filtrosPesquisa.DescricaoFiltro.ToInt();
            bool isFiltroNumeral = filtroNumeros > 0;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoFiltro))
            {
                sqlWhere += " AND (";

                if (isFiltroNumeral)
                {
                    sqlWhere += $" Destinatario.CLI_CGCCPF = {filtroNumeros} OR ";
                    sqlWhere += $" Remetente.CLI_CGCCPF = {filtroNumeros} OR ";
                    sqlWhere += $@"
                        Pedido.PED_CODIGO IN (
                            SELECT _cargapedido.PED_CODIGO
                              FROM T_CARGA_PEDIDO _cargapedido
                              LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal on _cargapedidoxmlnotafiscal.CPE_CODIGO = _cargapedido.CPE_CODIGO
                              LEFT JOIN T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO
                             WHERE _xmlnotafiscal.NF_NUMERO = {filtroNumeros}
                        ) ";
                }
                else
                {
                    sqlWhere += $" Destinatario.CLI_NOME LIKE :DescricaoFiltro OR ";
                    sqlWhere += $" Remetente.CLI_NOME LIKE :DescricaoFiltro OR ";
                    sqlWhere += $" LocalidadeDestino.LOC_DESCRICAO LIKE :DescricaoFiltro ";
                }

                sqlWhere += " )";
            }

            if (filtrosPesquisa.CodigosTiposOperacao.Count > 0)
            {
                sqlWhere += $@"
                        AND EXISTS(
                            SELECT _cargapedido.PED_CODIGO
                              FROM T_CARGA_PEDIDO _cargapedido
                              INNER JOIN T_CARGA _carga ON _carga.CAR_CODIGO = _cargaPedido.CAR_CODIGO
                             WHERE _cargaPedido.PED_CODIGO = Pedido.PED_CODIGO AND _carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTiposOperacao)})
                        ) ";
            }

            if (parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    sqlWhere += $" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar} ";

                if (parametrosConsulta.InicioRegistros >= 0)
                    sqlWhere += $" OFFSET {parametrosConsulta.InicioRegistros} ROWS ";

                if (parametrosConsulta.LimiteRegistros > 0)
                    sqlWhere += $" FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY";
            }

            sqlQuery += " WHERE 1 = 1 " + sqlWhere;

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoFiltro) && !isFiltroNumeral)
                query.SetString("DescricaoFiltro", $"%{filtrosPesquisa.DescricaoFiltro}%");

            return query;
        }

        private NHibernate.ISQLQuery ObterSQLPedidoNotasClientes(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sqlQuery = "SELECT ";

            if (parametrosConsulta == null)
                sqlQuery += "COUNT(*)";
            else
            {
                sqlQuery += @"
                    XMLNotaFiscal.NFX_CODIGO Codigo,
	                Pedido.PED_SITUACAO CodigoStatus,
                    XMLNotaFiscal.NF_NUMERO Nota,
                    CanhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO CodigoStatusCanhoto
                ";
            }

            sqlQuery += $@"
                FROM VIEW_PEDIDO_XML PedidoXml
                LEFT JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = PedidoXml.PED_CODIGO
                LEFT JOIN T_XML_NOTA_FISCAL XMLNotaFiscal on XMLNotaFiscal.NFX_CODIGO = PedidoXml.NFX_CODIGO
                LEFT JOIN T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal ON CanhotoNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO

                WHERE Pedido.PED_CODIGO = {codigoPedido} ";

            if (parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    sqlQuery += $" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar} ";

                if (parametrosConsulta.InicioRegistros >= 0)
                    sqlQuery += $" OFFSET {parametrosConsulta.InicioRegistros} ROWS ";

                if (parametrosConsulta.LimiteRegistros > 0)
                    sqlQuery += $" FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY";
            }

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query;
        }

        private string QueryPedidosGestaoPedidos(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoPedido.FiltroPesquisaGestaoPedido filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool selecionarTodos = false, List<int> codigosPedidos = null)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append($@"SELECT
                                pedido.PED_CODIGO AS Codigo,
								pedido.PED_PROTOCOLO AS Protocolo,
                                pedido.PED_NUMERO_PEDIDO_EMBARCADOR AS NumeroPedido,
                                pedido.PED_NUMERO_CARREGAMENTO AS NumeroCarregamento,
                                pedido.PED_CODIGO_AGRUPAMENTO_CARREGAMENTO AS CodigoAgrupamentoCarregamento,
                                pedido.PED_SITUACAO_ROTEIRIZADOR_INTEGRACAO AS SituacaoRoteirizadorIntegracao,
                                pedido.PED_SITUACAO AS SituacaoPedido,
                                pedido.PED_REENTREGA_SOLICITADA AS ReentregaSolicitada,
                                (SELECT
                                    STRING_AGG(SRO_CODIGO, ', ')
                                FROM
                                    T_SESSAO_ROTEIRIZADOR_PEDIDO SRP   
                                WHERE
                                    SRP.PED_CODIGO = pedido.PED_CODIGO     
                                    AND (
                                        SRP.SRP_SITUACAO <> {(int)SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao}         
                                        OR EXISTS (
                                            SELECT
                                                1                 
                                            FROM
                                                T_CARREGAMENTO_PEDIDO CRP 
                                            INNER JOIN
                                                T_CARREGAMENTO CRG 
                                                    ON CRG.CRG_CODIGO = CRP.CRG_CODIGO       
                                            WHERE
                                                CRP.PED_CODIGO = SRP.PED_CODIGO         
                                                AND CRG.SRO_CODIGO = SRP.SRO_CODIGO         
                                                AND CRG.CRG_SITUACAO <> {(int)SituacaoCarregamento.Cancelado}
                                        )
                                    )) AS SessaoRoteirizador,
                                (SELECT
		                            COALESCE(SUM(CASE WHEN subconsulta.PRP_QUANTIDADE > subconsulta.PEA_ESTOQUE_DISPONIVEL THEN 1 ELSE 0 END), 1)
	                            FROM
		                            (
			                            SELECT
				                            pedidoproduto.PRP_QUANTIDADE,
				                            produtoembarcadorestoque.PEA_ESTOQUE_DISPONIVEL
			                            FROM
				                            T_PEDIDO_PRODUTO pedidoproduto
			                            INNER JOIN
				                            T_PRODUTO_EMBARCADOR_ESTOQUE_ARMAZEM produtoembarcadorestoque 
					                            ON produtoembarcadorestoque.PRO_CODIGO = pedidoproduto.PRO_CODIGO                                      
			                            INNER JOIN
				                            T_FILIAL filialprodutoembarcador 
					                            ON produtoembarcadorestoque.FIL_CODIGO=filialprodutoembarcador.FIL_CODIGO                                      
			                            INNER JOIN
				                            T_PRODUTO_EMBARCADOR produtoembarcador 
					                            ON produtoembarcadorestoque.PRO_CODIGO=produtoembarcador.PRO_CODIGO                                      
			                            LEFT JOIN
				                            T_FILIAL_ARMAZEM filialarmazen 
					                            ON produtoembarcadorestoque.FIA_CODIGO=filialarmazen.FIA_CODIGO                                 
			                            WHERE
				                            pedidoproduto.PED_CODIGO = pedido.PED_CODIGO                                   
				                            AND filialprodutoembarcador.FIL_CODIGO = filialarmazen.FIL_CODIGO                                        
				                            AND produtoembarcador.PRO_CODIGO = pedidoproduto.PRO_CODIGO                                        
				                            AND filialarmazen.FIA_CODIGO = pedidoproduto.FIA_CODIGO
		                            ) AS subconsulta) AS QuantidadeMaiorQueEstoque,
                                situacaocomercial.SCP_DESCRICAO AS SituacaoComercialPedido,
                          		pedido.CAR_DATA_CARREGAMENTO_PEDIDO AS DataCarregamentoPedido,
								filial.FIL_DESCRICAO AS Filial,
		                        pais.PAI_ABREVIACAO AS AbreviacaoPais,
								pais.PAI_NOME	AS NomePais,
								estado.UF_SIGLA AS SiglaEstado,
								cliente.CLI_NOME	AS NomeCliente,
								cliente.CLI_PONTO_TRANSBORDO	AS PontoTransbordoCliente,
								cliente.CLI_NOMEFANTASIA	AS NomeFantasiaCliente,
								cliente.CLI_CODIGO_INTEGRACAO	AS CodigoIntegracaoCliente,
								cliente.CLI_FISJUR AS TipoCliente,
								cliente.CLI_CGCCPF AS CPF_CNPJ_Cliente,
								localidadeDestino.LOC_DESCRICAO AS Destino,
								destino.PEN_CEP AS DestinoCep,
								pedido.PED_NUMERO_PALETES AS NumeroPaletes,
								pedido.PED_NUMERO_PALETES_FRACIONADO AS NumeroPaletesFracionado,
	                            pedido.PED_PESO_TOTAL_PALETES AS PesoTotalPaletes,
								pedido.PED_PESO_TOTAL_CARGA AS PesoTotalPedido,
                                (SELECT SUM(_produtoPedido.PRP_VALOR_PRODUTO * _produtoPedido.PRP_QUANTIDADE)
								FROM T_PEDIDO_PRODUTO _produtoPedido
								    WHERE _produtoPedido.PED_CODIGO = pedido.PED_CODIGO) AS ValorTotalPedido,
                                situacaoEstoque.SEP_DESCRICAO AS SituacaoEstoque,
                                canalEntrega.CNE_DESCRICAO AS CanalEntrega,
                                pedido.PED_PREVISAO_ENTREGA AS DataPrevisaoEntrega,
                                tipooperacao.TOP_DESCRICAO As TipoOperacao,
                                (SELECT
                                    STRING_AGG(carga.CAR_CODIGO_CARGA_EMBARCADOR, ', ')
                                FROM
                                    T_CARGA_PEDIDO cargaPedido
                                    JOIN T_CARGA carga ON carga.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                WHERE
                                    cargaPedido.PED_CODIGO = pedido.PED_CODIGO) AS NumeroCargas,
                                configuracaoGeral.CGE_HABILITAR_CADASTRO_ARMAZEM HabilitarCadastroArmazem,
								(select CIR_POSSUI_INTEGRACAO from T_CONFIGURACAO_INTEGRACAO_ROUTEASY) ExisteIntegracaoRouteasy
                ");

            sql.Append(" from T_PEDIDO pedido ");
            sql.Append(@"
                left outer join T_PEDIDO_ENDERECO destino ON destino.PEN_CODIGO = pedido.PEN_CODIGO_DESTINO
				left outer join T_LOCALIDADES localidadeDestino on pedido.LOC_CODIGO_DESTINO=localidadeDestino.LOC_CODIGO 
                left outer join T_REGIAO regiao on localidadeDestino.REG_CODIGO=regiao.REG_CODIGO 
	            left outer join T_PAIS pais on pais.PAI_CODIGO = localidadeDestino.PAI_CODIGO
			    left outer join T_UF estado on estado.UF_SIGLA = localidadeDestino.UF_SIGLA
                left outer join T_LOCALIDADES localidadeOrigem on pedido.LOC_CODIGO_ORIGEM=localidadeOrigem.LOC_CODIGO
				left outer join T_UF estadoOrigem on estadoOrigem.UF_SIGLA = localidadeOrigem.UF_SIGLA
                left outer join T_FUNCIONARIO funcionarioVendedor on pedido.FUN_CODIGO_VENDEDOR = funcionarioVendedor.FUN_CODIGO 
                left outer join T_FUNCIONARIO funcionarioSupervisor on pedido.FUN_CODIGO_SUPERVISOR = funcionarioSupervisor.FUN_CODIGO 
                left outer join T_FUNCIONARIO funcionarioGerente on pedido.FUN_CODIGO_GERENTE = funcionarioGerente.FUN_CODIGO 
                left outer join T_FILIAL filial on pedido.FIL_CODIGO=filial.FIL_CODIGO 
                left outer join T_CANAL_ENTREGA canalEntrega on pedido.CNE_CODIGO=canalEntrega.CNE_CODIGO
                left join T_TIPO_OPERACAO tipooperacao on pedido.TOP_CODIGO=tipooperacao.TOP_CODIGO 
                inner join T_CLIENTE cliente on pedido.CLI_CODIGO=cliente.CLI_CGCCPF 
                left join T_GRUPO_PESSOAS grupopessoa on cliente.GRP_CODIGO=grupopessoa.GRP_CODIGO
                left outer join T_SITUACAO_COMERCIAL_PEDIDO situacaocomercial on situacaocomercial.SCP_CODIGO=pedido.SCP_CODIGO
                left outer join T_PEDIDO_ADICIONAL pedidoAdicional on pedidoAdicional.PED_CODIGO = pedido.PED_CODIGO
					left join T_SITUACAO_ESTOQUE_PEDIDO situacaoEstoque ON situacaoEstoque.SEP_CODIGO = pedidoAdicional.SEP_CODIGO
                left outer join T_CLIENTE destinatario ON destinatario.CLI_CGCCPF = pedido.CLI_CODIGO,
				T_CONFIGURACAO_GERAL configuracaoGeral
            ");

            sql.Append("WHERE 1 = 1 ");

            if (filtrosPesquisa.DataInicial.HasValue)
                sql.Append($" AND pedido.PED_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataInicial.Value.Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.DataLimite.HasValue)
                sql.Append($" AND pedido.PED_PREVISAO_ENTREGA < '{filtrosPesquisa.DataLimite.Value.AddDays(1).Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.Situacao.HasValue && filtrosPesquisa.Situacao != SituacaoPedido.Todos)
                sql.Append($" AND pedido.PED_SITUACAO = '{(int)filtrosPesquisa.Situacao.Value}'");

            if (filtrosPesquisa.CodigoSessaoRoteirizador > 0)
            {
                sql.Append($@" AND EXISTS (
                    SELECT 1 FROM T_SESSAO_ROTEIRIZADOR_PEDIDO SRP WHERE SRP.PED_CODIGO = pedido.PED_CODIGO AND (
                    SRP.SRP_SITUACAO <> {(int)SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao} OR EXISTS (
                    SELECT 1 FROM T_CARREGAMENTO_PEDIDO CRP INNER JOIN T_CARREGAMENTO CRG ON CRG.CRG_CODIGO = CRP.CRG_CODIGO
                        WHERE CRP.PED_CODIGO = SRP.PED_CODIGO AND CRG.SRO_CODIGO = SRP.SRO_CODIGO AND CRG.CRG_SITUACAO <> {(int)SituacaoCarregamento.Cancelado})
                    )
                    AND SRP.SRO_CODIGO = {filtrosPesquisa.CodigoSessaoRoteirizador}
                )");
            }

            if (filtrosPesquisa.ListaNumeroPedido?.Count > 0)
                sql.Append($" AND (pedido.PED_CODIGO IN ({string.Join(", ", filtrosPesquisa.ListaNumeroPedido)}))");

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                sql.Append($" AND (filial.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))");

            if (filtrosPesquisa.TipoOperacao > 0)
                sql.Append($" AND tipooperacao.TOP_CODIGO = {filtrosPesquisa.TipoOperacao}");

            if (filtrosPesquisa.GrupoPessoaDestinatario > 0)
                sql.Append($" AND grupopessoa.GRP_CODIGO = {filtrosPesquisa.GrupoPessoaDestinatario}");

            if (filtrosPesquisa.CodigosRegiaoDestino?.Count > 0)
                sql.Append($" AND (regiao.REG_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosRegiaoDestino)}))");

            if (filtrosPesquisa.CodigosVendedor?.Count > 0)
                sql.Append($" AND (funcionarioVendedor.FUN_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosVendedor)}))");

            if (filtrosPesquisa.CodigosSupervisor?.Count > 0)
                sql.Append($" AND (funcionarioSupervisor.FUN_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosSupervisor)}))");

            if (filtrosPesquisa.CodigosGerente?.Count > 0)
                sql.Append($" AND (funcionarioGerente.FUN_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosGerente)}))");

            if (filtrosPesquisa.SituacaoComercialPedido > 0)
                sql.Append($" AND pedido.SCP_CODIGO = {filtrosPesquisa.SituacaoComercialPedido}");

            if (filtrosPesquisa.Reentrega.HasValue)
                sql.Append($" AND pedido.PED_REENTREGA_SOLICITADA = {(filtrosPesquisa.Reentrega.Value ? 1 : 0)}");

            if (filtrosPesquisa.SituacaoRoteirizadorIntegracao != null)
                sql.Append($" AND pedido.PED_SITUACAO_ROTEIRIZADOR_INTEGRACAO = {(int)filtrosPesquisa.SituacaoRoteirizadorIntegracao}");

            if (filtrosPesquisa.SituacaoPedido != SituacaoPedidoGestaoPedido.Todos)
            {
                sql.Append($" AND pedido.PED_SITUACAO  = '{(int)SituacaoPedido.Aberto}'");

                switch (filtrosPesquisa.SituacaoPedido)
                {
                    case SituacaoPedidoGestaoPedido.PedidoSemCarga:
                        sql.Append($" AND pedido.PED_CODIGO_CARGA_EMBARCADOR IS NULL");
                        break;
                    case SituacaoPedidoGestaoPedido.PedidoEmSessao:
                        sql.Append($@" AND pedido.PED_CODIGO_CARGA_EMBARCADOR IS NULL   
                          AND(
                            SELECT STRING_AGG(SRO_CODIGO, ', ') FROM T_SESSAO_ROTEIRIZADOR_PEDIDO SRP 
                            WHERE SRP.PED_CODIGO = pedido.PED_CODIGO AND(SRP.SRP_SITUACAO<> {(int)SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao}
                            OR EXISTS(
                                SELECT 1 FROM T_CARREGAMENTO_PEDIDO CRP INNER JOIN T_CARREGAMENTO CRG ON CRG.CRG_CODIGO = CRP.CRG_CODIGO WHERE CRP.PED_CODIGO = SRP.PED_CODIGO AND CRG.SRO_CODIGO = SRP.SRO_CODIGO AND CRG.CRG_SITUACAO<> {(int)SituacaoCarregamento.Cancelado}
                            )
                          )
                        ) IS NOT NULL ");
                        break;
                    case SituacaoPedidoGestaoPedido.PedidoComSaldoDisponivel:
                        sql.Append($" AND pedido.PED_CODIGO_CARGA_EMBARCADOR IS NOT NULL AND PED_SALDO_VOLUMES_RESTANTE > 0");
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamento))
                sql.Append($@" AND (
                        EXISTS (
                            SELECT
                                carregamento_pedido.CRP_CODIGO 
                            from
                                T_CARREGAMENTO_PEDIDO carregamento_pedido
                            inner join
                                T_CARREGAMENTO carregamento
                                    on carregamento_pedido.CRG_CODIGO=carregamento.CRG_CODIGO 
                            inner join
                                T_PEDIDO pedido12_ 
                                    on carregamento_pedido.PED_CODIGO=pedido12_.PED_CODIGO 
                            where
                                (
                                    carregamento.CRG_SITUACAO <> {(int)SituacaoCarregamento.Cancelado} 
                                    or carregamento.CRG_SITUACAO is null
                                ) 
                                and (
                                    pedido12_.PED_CODIGO=pedido.PED_CODIGO 
                                    or (
                                        pedido12_.PED_CODIGO is null
                                    ) 
                                    and (
                                        pedido.PED_CODIGO is null
                                    )
                                ) 
                                and carregamento.CRG_NUMERO_CARREGAMENTO = '{filtrosPesquisa.NumeroCarregamento}'
                        ))
                ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoAgrupamentoCarregamento))
                sql.Append($" AND pedido.PED_CODIGO_AGRUPAMENTO_CARREGAMENTO LIKE '%{filtrosPesquisa.CodigoAgrupamentoCarregamento}%'");

            if (filtrosPesquisa.CodigosDestino?.Count > 0)
                sql.Append($" AND (destino.LOC_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosDestino)}))");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                sql.Append($" AND (pedido.TCG_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}))");

            if (filtrosPesquisa.CodigosDestinatario?.Count > 0)
                sql.Append($" AND (destinatario.CLI_CGCCPF IN ({string.Join(", ", filtrosPesquisa.CodigosDestinatario)}))");

            if (filtrosPesquisa.CodigosRemetente?.Count > 0)
                sql.Append($" AND (pedido.CLI_CODIGO_REMETENTE IN ({string.Join(", ", filtrosPesquisa.CodigosRemetente)}))");

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                sql.Append($" AND (pedido.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportador)}))");

            if (codigosPedidos?.Count > 0)
                if (!selecionarTodos)
                    sql.Append($" AND pedido.PED_CODIGO IN ({string.Join(", ", codigosPedidos)})");
                else
                    sql.Append($" AND pedido.PED_CODIGO NOT IN ({string.Join(", ", codigosPedidos)})");

            if (filtrosPesquisa.CodigoCanalEntrega > 0)
                sql.Append($" AND pedido.CNE_CODIGO = {filtrosPesquisa.CodigoCanalEntrega}");

            if (filtrosPesquisa.EstadosOrigem?.Count > 0)
            {
                List<string> estadosOrigem = filtrosPesquisa.EstadosOrigem.Select(estadoOrigem => $"'{estadoOrigem}'").ToList();
                sql.Append($" AND estadoOrigem.UF_SIGLA IN ({string.Join(", ", estadosOrigem)})");
            }

            if (filtrosPesquisa.EstadosDestino?.Count > 0)
            {
                List<string> estadosDestino = filtrosPesquisa.EstadosDestino.Select(estadoDestino => $"'{estadoDestino}'").ToList();
                sql.Append($" AND estado.UF_SIGLA IN ({string.Join(", ", estadosDestino)})");
            }

            if (filtrosPesquisa.SituacaoEstoqueProdutoArmazem != SituacaoEstoqueProdutoArmazem.Todos)
            {
                if (filtrosPesquisa.SituacaoEstoqueProdutoArmazem == SituacaoEstoqueProdutoArmazem.SemEstoque)
                {
                    sql.Append(@"And Not Exists (
				            Select 1
			                From T_PEDIDO_PRODUTO pedidoproduto
					            Inner Join T_PRODUTO_EMBARCADOR_ESTOQUE_ARMAZEM produtoembarcadorestoque On produtoembarcadorestoque.PRO_CODIGO = pedidoproduto.PRO_CODIGO
					            Inner Join T_FILIAL filialprodutoembarcador On produtoembarcadorestoque.FIL_CODIGO = filialprodutoembarcador.FIL_CODIGO
					            Inner Join T_PRODUTO_EMBARCADOR produtoembarcador On produtoembarcadorestoque.PRO_CODIGO = produtoembarcador.PRO_CODIGO                                      
					            Left Join T_FILIAL_ARMAZEM filialarmazen On produtoembarcadorestoque.FIA_CODIGO = filialarmazen.FIA_CODIGO
			                Where pedidoproduto.PED_CODIGO = pedido.PED_CODIGO                                   
				                And filialprodutoembarcador.FIL_CODIGO = filialarmazen.FIL_CODIGO                                        
				                And produtoembarcador.PRO_CODIGO = pedidoproduto.PRO_CODIGO                                        
				                And filialarmazen.FIA_CODIGO = pedidoproduto.FIA_CODIGO
					            And produtoembarcadorestoque.PEA_ESTOQUE_DISPONIVEL <> 0		                             
			            )
                    ");
                }
                else if (filtrosPesquisa.SituacaoEstoqueProdutoArmazem == SituacaoEstoqueProdutoArmazem.EstoqueParcial)
                {
                    sql.Append(@"And Exists (
				            Select 1
			                From T_PEDIDO_PRODUTO pedidoproduto
					            Inner Join T_PRODUTO_EMBARCADOR_ESTOQUE_ARMAZEM produtoembarcadorestoque On produtoembarcadorestoque.PRO_CODIGO = pedidoproduto.PRO_CODIGO
					            Inner Join T_FILIAL filialprodutoembarcador On produtoembarcadorestoque.FIL_CODIGO = filialprodutoembarcador.FIL_CODIGO
					            Inner Join T_PRODUTO_EMBARCADOR produtoembarcador On produtoembarcadorestoque.PRO_CODIGO = produtoembarcador.PRO_CODIGO                                      
					            Left Join T_FILIAL_ARMAZEM filialarmazen On produtoembarcadorestoque.FIA_CODIGO = filialarmazen.FIA_CODIGO
			                Where pedidoproduto.PED_CODIGO = pedido.PED_CODIGO                                   
				                And filialprodutoembarcador.FIL_CODIGO = filialarmazen.FIL_CODIGO                                        
				                And produtoembarcador.PRO_CODIGO = pedidoproduto.PRO_CODIGO                                        
				                And filialarmazen.FIA_CODIGO = pedidoproduto.FIA_CODIGO
					            And (produtoembarcadorestoque.PEA_ESTOQUE_DISPONIVEL != 0 and produtoembarcadorestoque.PEA_ESTOQUE_DISPONIVEL < pedidoproduto.PRP_QUANTIDADE) 		                             
			            ) 
                    ");
                }
                else if (filtrosPesquisa.SituacaoEstoqueProdutoArmazem == SituacaoEstoqueProdutoArmazem.EstoqueTotal)
                {
                    sql.Append(@"AND ( SELECT 
                                          COALESCE(
                                            SUM(
                                              CASE WHEN subconsulta.PRP_QUANTIDADE > subconsulta.PEA_ESTOQUE_DISPONIVEL THEN 1 ELSE 0 END
                                            ), 
                                            1
                                          ) 
                                        FROM 
                                          (
                                            SELECT 
                                              pedidoproduto.PRP_QUANTIDADE, 
                                              produtoembarcadorestoque.PEA_ESTOQUE_DISPONIVEL 
                                            FROM 
                                              T_PEDIDO_PRODUTO pedidoproduto 
                                              INNER JOIN T_PRODUTO_EMBARCADOR_ESTOQUE_ARMAZEM produtoembarcadorestoque 
                                                ON produtoembarcadorestoque.PRO_CODIGO = pedidoproduto.PRO_CODIGO 
                                              INNER JOIN T_FILIAL filialprodutoembarcador 
                                                ON produtoembarcadorestoque.FIL_CODIGO = filialprodutoembarcador.FIL_CODIGO 
                                              INNER JOIN T_PRODUTO_EMBARCADOR produtoembarcador 
                                                ON produtoembarcadorestoque.PRO_CODIGO = produtoembarcador.PRO_CODIGO 
                                              LEFT JOIN T_FILIAL_ARMAZEM filialarmazen 
                                                ON produtoembarcadorestoque.FIA_CODIGO = filialarmazen.FIA_CODIGO 
                                            WHERE 
                                              pedidoproduto.PED_CODIGO = pedido.PED_CODIGO 
                                              AND filialprodutoembarcador.FIL_CODIGO = filialarmazen.FIL_CODIGO 
                                              AND produtoembarcador.PRO_CODIGO = pedidoproduto.PRO_CODIGO 
                                              AND filialarmazen.FIA_CODIGO = pedidoproduto.FIA_CODIGO
                                          ) AS subconsulta
                                      ) = 0");
                }
            }

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeOrdenar))
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterQueryVerificarExistenciaPedidoPorNumeroPedidoEmbarcador(string numeroPedidoEmbarcador)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(obj => obj.NumeroPedidoEmbarcador == numeroPedidoEmbarcador);
        }

        private async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>> ObterDadosPedidosAsync(IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> consultaPedido)
        {
            int limiteRegistros = 1000;

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidos = await consultaPedido
                .Select(pedido => new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido()
                {
                    Codigo = pedido.Codigo,
                    Ajudante = pedido.Ajudante,
                    CodigoAgrupamentoCarregamento = pedido.CodigoAgrupamentoCarregamento,
                    CodigoPedidoCliente = pedido.CodigoPedidoCliente,
                    Companhia = pedido.Companhia,
                    CubagemTotal = pedido.CubagemTotal,
                    DataAgendamento = pedido.DataAgendamento,
                    DataCarregamentoCarga = pedido.DataCarregamentoCarga,
                    DataCarregamentoPedido = pedido.DataCarregamentoPedido,
                    DataCriacao = pedido.DataCriacao,
                    DataETA = pedido.DataETA,
                    DataInclusaoBooking = pedido.DataInclusaoBooking,
                    DataInclusaoPCP = pedido.DataInclusaoPCP,
                    DataInicialColeta = pedido.DataInicialColeta,
                    DataPrevisaoSaida = pedido.DataPrevisaoSaida,
                    DataTerminoCarregamento = pedido.DataTerminoCarregamento,
                    DeliveryTerm = pedido.DeliveryTerm,
                    DiasUteisPrazoTransportador = (int?)pedido.DiasUteisPrazoTransportador ?? 0,
                    DisponibilizarPedidoParaColeta = (bool?)pedido.DisponibilizarPedidoParaColeta ?? false,
                    Distancia = (decimal?)pedido.Distancia ?? 0m,
                    IdAutorizacao = pedido.IdAutorizacao,
                    Numero = pedido.Numero,
                    NumeroBooking = pedido.NumeroBooking,
                    NumeroContainer = pedido.NumeroContainer,
                    NumeroNavio = pedido.NumeroNavio,
                    NumeroOrdem = pedido.NumeroOrdem,
                    NumeroPaletes = pedido.NumeroPaletes,
                    NumeroPaletesFracionado = pedido.NumeroPaletesFracionado,
                    NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                    Observacao = pedido.Observacao,
                    ObservacaoCTe = pedido.ObservacaoCTe,
                    ObservacaoInterna = pedido.ObservacaoInterna,
                    Ordem = pedido.Ordem,
                    PalletSaldoRestante = (decimal?)pedido.PalletSaldoRestante ?? 0m,
                    PedidoBloqueado = (bool?)pedido.PedidoBloqueado ?? false,
                    PedidoLiberadoMontagemCarga = (bool?)pedido.PedidoLiberadoMontagemCarga ?? false,
                    PedidoRestricaoData = (bool?)pedido.PedidoRestricaoData ?? false,
                    PedidoTotalmenteCarregado = pedido.PedidoTotalmenteCarregado,
                    PercentualSeparacaoPedido = (decimal?)pedido.PercentualSeparacaoPedido ?? 0m,
                    PesoLiquidoTotal = pedido.PesoLiquidoTotal,
                    PesoSaldoRestante = pedido.PesoSaldoRestante,
                    PesoTotal = pedido.PesoTotal,
                    PesoTotalPaletes = pedido.PesoTotalPaletes,
                    PortoChegada = pedido.PortoChegada,
                    PortoSaida = pedido.PortoSaida,
                    PossuiCarga = pedido.PossuiCarga,
                    PossuiDescarga = pedido.PossuiDescarga,
                    PrevisaoEntrega = pedido.PrevisaoEntrega,
                    ProdutoPredominante = pedido.ProdutoPredominante,
                    QtdAjudantes = pedido.QtdAjudantes,
                    QtVolumes = pedido.QtVolumes,
                    ReentregaSolicitada = pedido.ReentregaSolicitada,
                    Reserva = pedido.Reserva,
                    Resumo = pedido.Resumo,
                    SaldoVolumesRestante = pedido.SaldoVolumesRestante,
                    SenhaAgendamento = pedido.SenhaAgendamento,
                    SenhaAgendamentoCliente = pedido.SenhaAgendamentoCliente,
                    Situacao = pedido.SituacaoPedido,
                    Temperatura = pedido.Temperatura,
                    TipoEmbarque = pedido.TipoEmbarque,
                    TipoPaleteCliente = pedido.TipoPaleteCliente,
                    TipoPagamento = pedido.TipoPagamento,
                    TipoTomador = pedido.TipoTomador,
                    UsarOutroEnderecoDestino = pedido.UsarOutroEnderecoDestino,
                    ValorCarga = pedido.ValorCarga,
                    ValorCobrancaFreteCombinado = pedido.ValorCobrancaFreteCombinado,
                    ValorDescarga = pedido.ValorDescarga,
                    ValorFreteNegociado = pedido.ValorFreteNegociado,
                    ValorTotalNotasFiscais = pedido.ValorTotalNotasFiscais,
                    Vendedor = pedido.Vendedor,
                    Filial = (pedido.Filial == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Filial()
                    {
                        Codigo = pedido.Filial.Codigo,
                        Cnpj = pedido.Filial.CNPJ,
                        Descricao = pedido.Filial.Descricao,
                        ExigirPreCargaMontagemCarga = pedido.Filial.ExigirPreCargaMontagemCarga
                    },
                    Empresa = (pedido.Empresa == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Empresa()
                    {
                        Codigo = pedido.Empresa.Codigo,
                        Cnpj = pedido.Empresa.CNPJ,
                        RazaoSocial = pedido.Empresa.RazaoSocial,
                        RestringirLocaisCarregamentoAutorizadosMotoristas = (bool?)pedido.Empresa.RestringirLocaisCarregamentoAutorizadosMotoristas ?? false,
                        Localidade = (pedido.Empresa.Localidade == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                        {
                            Codigo = pedido.Empresa.Localidade.Codigo,
                            CodigoIbge = pedido.Empresa.Localidade.CodigoIBGE,
                            Descricao = pedido.Empresa.Localidade.Descricao,
                            Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                            {
                                Descricao = pedido.Empresa.Localidade.Estado.Nome,
                                Sigla = pedido.Empresa.Localidade.Estado.Sigla,
                            },
                            Pais = (pedido.Empresa.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                            {
                                Codigo = pedido.Empresa.Localidade.Pais.Codigo,
                                Nome = pedido.Empresa.Localidade.Pais.Nome,
                                Abreviacao = pedido.Empresa.Localidade.Pais.Abreviacao
                            }
                        }
                    },
                    TipoCarga = (pedido.TipoDeCarga == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoCarga()
                    {
                        Codigo = pedido.TipoDeCarga.Codigo,
                        Descricao = pedido.TipoDeCarga.Descricao
                    },
                    TipoOperacao = (pedido.TipoOperacao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoOperacao()
                    {
                        Codigo = pedido.TipoOperacao.Codigo,
                        Descricao = pedido.TipoOperacao.Descricao,
                        NaoExigeRoteirizacaoMontagemCarga = pedido.TipoOperacao.NaoExigeRoteirizacaoMontagemCarga,
                        PermitirInformarRecebedorMontagemCarga = pedido.TipoOperacao.PermitirInformarRecebedorMontagemCarga,
                        Observacao = pedido.TipoOperacao.Observacao,
                        ConfiguracaoCarga = (pedido.TipoOperacao.ConfiguracaoCarga == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.ConfiguracaoTipoOperacaoCarga()
                        {
                            ValidarValorMinimoCarga = (bool?)pedido.TipoOperacao.ConfiguracaoCarga.ValidarValorMinimoCarga ?? false
                        },
                        ConfiguracaoMobile = (pedido.TipoOperacao.ConfiguracaoMobile == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.ConfiguracaoTipoOperacaoMobile()
                        {
                            NecessarioConfirmacaoMotorista = (bool?)pedido.TipoOperacao.ConfiguracaoMobile.NecessarioConfirmacaoMotorista ?? false,
                            TempoLimiteConfirmacaoMotorista = (TimeSpan?)pedido.TipoOperacao.ConfiguracaoMobile.TempoLimiteConfirmacaoMotorista ?? TimeSpan.Zero
                        },
                        ConfiguracaoMontagemCarga = (pedido.TipoOperacao.ConfiguracaoMontagemCarga == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.ConfiguracaoTipoOperacaoMontagemCarga()
                        {
                            ControlarCapacidadePorUnidade = (bool?)pedido.TipoOperacao.ConfiguracaoMontagemCarga.ControlarCapacidadePorUnidade ?? false,
                            ExigirInformarDataPrevisaoInicioViagem = (bool?)pedido.TipoOperacao.ConfiguracaoMontagemCarga.ExigirInformarDataPrevisaoInicioViagem ?? false,
                            OcultarTipoDeOperacaoNaMontagemDaCarga = (bool?)pedido.TipoOperacao.ConfiguracaoMontagemCarga.OcultarTipoDeOperacaoNaMontagemDaCarga ?? false
                        }
                    },
                    Origem = (pedido.Origem == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                    {
                        Codigo = pedido.Origem.Codigo,
                        CodigoIbge = pedido.Origem.CodigoIBGE,
                        Descricao = pedido.Origem.Descricao,
                        Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                        {
                            Descricao = pedido.Origem.Estado.Nome,
                            Sigla = pedido.Origem.Estado.Sigla,
                        },
                        Pais = (pedido.Origem.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                        {
                            Codigo = pedido.Origem.Pais.Codigo,
                            Nome = pedido.Origem.Pais.Nome,
                            Abreviacao = pedido.Origem.Pais.Abreviacao
                        }
                    },
                    Destino = (pedido.Destino == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                    {
                        Codigo = pedido.Destino.Codigo,
                        CodigoIbge = pedido.Destino.CodigoIBGE,
                        Descricao = pedido.Destino.Descricao,
                        Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                        {
                            Descricao = pedido.Destino.Estado.Nome,
                            Sigla = pedido.Destino.Estado.Sigla,
                        },
                        Pais = (pedido.Destino.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                        {
                            Codigo = pedido.Destino.Pais.Codigo,
                            Nome = pedido.Destino.Pais.Nome,
                            Abreviacao = pedido.Destino.Pais.Abreviacao
                        }
                    },
                    EnderecoOrigem = (pedido.EnderecoOrigem == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                    {
                        Logradouro = pedido.EnderecoOrigem.Endereco,
                        Bairro = pedido.EnderecoOrigem.Bairro,
                        Numero = pedido.EnderecoOrigem.Numero,
                        Cep = pedido.EnderecoOrigem.CEP,
                        Latitude = (pedido.EnderecoOrigem.ClienteOutroEndereco != null) ? pedido.EnderecoOrigem.ClienteOutroEndereco.Latitude : string.Empty,
                        Longitude = (pedido.EnderecoOrigem.ClienteOutroEndereco != null) ? pedido.EnderecoOrigem.ClienteOutroEndereco.Longitude : string.Empty,
                        Localidade = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                        {
                            Codigo = pedido.EnderecoOrigem.Localidade.Codigo,
                            CodigoIbge = pedido.EnderecoOrigem.Localidade.CodigoIBGE,
                            Descricao = pedido.EnderecoOrigem.Localidade.Descricao,
                            Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                            {
                                Descricao = pedido.EnderecoOrigem.Localidade.Estado.Nome,
                                Sigla = pedido.EnderecoOrigem.Localidade.Estado.Sigla,
                            },
                            Pais = (pedido.EnderecoOrigem.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                            {
                                Codigo = pedido.EnderecoOrigem.Localidade.Pais.Codigo,
                                Nome = pedido.EnderecoOrigem.Localidade.Pais.Nome,
                                Abreviacao = pedido.EnderecoOrigem.Localidade.Pais.Abreviacao
                            }
                        }
                    },
                    EnderecoDestino = (pedido.EnderecoDestino == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                    {
                        Logradouro = pedido.EnderecoDestino.Endereco,
                        Bairro = pedido.EnderecoDestino.Bairro,
                        Numero = pedido.EnderecoDestino.Numero,
                        Cep = pedido.EnderecoDestino.CEP,
                        Latitude = (pedido.EnderecoDestino.ClienteOutroEndereco != null) ? pedido.EnderecoDestino.ClienteOutroEndereco.Latitude : string.Empty,
                        Longitude = (pedido.EnderecoDestino.ClienteOutroEndereco != null) ? pedido.EnderecoDestino.ClienteOutroEndereco.Longitude : string.Empty,
                        Localidade = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                        {
                            Codigo = pedido.EnderecoDestino.Localidade.Codigo,
                            CodigoIbge = pedido.EnderecoDestino.Localidade.CodigoIBGE,
                            Descricao = pedido.EnderecoDestino.Localidade.Descricao,
                            Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                            {
                                Descricao = pedido.EnderecoDestino.Localidade.Estado.Nome,
                                Sigla = pedido.EnderecoDestino.Localidade.Estado.Sigla,
                            },
                            Pais = (pedido.EnderecoDestino.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                            {
                                Codigo = pedido.EnderecoDestino.Localidade.Pais.Codigo,
                                Nome = pedido.EnderecoDestino.Localidade.Pais.Nome,
                                Abreviacao = pedido.EnderecoDestino.Localidade.Pais.Abreviacao
                            }
                        }
                    },
                    Remetente = (pedido.Remetente == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.Remetente.CPF_CNPJ,
                        CodigoIntegracao = pedido.Remetente.CodigoIntegracao,
                        CpfCnpj = pedido.Remetente.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.Remetente.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.Remetente.CPF_CNPJ),
                        DiasDePrazoFatura = pedido.Remetente.DiasDePrazoFatura,
                        Nome = pedido.Remetente.Nome,
                        NomeFantasia = pedido.Remetente.NomeFantasia,
                        Observacao = pedido.Remetente.Observacao,
                        PontoTransbordo = (bool?)pedido.Remetente.PontoTransbordo ?? false,
                        Tipo = pedido.Remetente.Tipo,
                        ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = (bool?)pedido.Remetente.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento ?? false,
                        ValorMinimoCarga = pedido.Remetente.ValorMinimoCarga,
                        Categoria = (pedido.Remetente.Categoria == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.CategoriaPessoa()
                        {
                            Codigo = pedido.Remetente.Categoria.Codigo,
                            Descricao = pedido.Remetente.Categoria.Descricao
                        },
                        Regiao = (pedido.Remetente.Regiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Regiao()
                        {
                            Codigo = pedido.Remetente.Regiao.Codigo,
                            Descricao = pedido.Remetente.Regiao.Descricao
                        },
                        MesoRegiao = (pedido.Remetente.MesoRegiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.MesoRegiao()
                        {
                            Codigo = pedido.Remetente.MesoRegiao.Codigo,
                            Descricao = pedido.Remetente.MesoRegiao.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                        {
                            Logradouro = pedido.Remetente.Endereco,
                            Bairro = pedido.Remetente.Bairro,
                            Numero = pedido.Remetente.Numero,
                            Cep = pedido.Remetente.CEP,
                            Latitude = pedido.Remetente.Latitude,
                            Longitude = pedido.Remetente.Longitude,
                            LatitudeTransbordo = pedido.Remetente.LatitudeTransbordo,
                            LongitudeTransbordo = pedido.Remetente.LongitudeTransbordo,
                            Localidade = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                            {
                                Codigo = pedido.Remetente.Localidade.Codigo,
                                CodigoIbge = pedido.Remetente.Localidade.CodigoIBGE,
                                Descricao = pedido.Remetente.Localidade.Descricao,
                                Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                                {
                                    Descricao = pedido.Remetente.Localidade.Estado.Nome,
                                    Sigla = pedido.Remetente.Localidade.Estado.Sigla,
                                },
                                Pais = (pedido.Remetente.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                                {
                                    Codigo = pedido.Remetente.Localidade.Pais.Codigo,
                                    Nome = pedido.Remetente.Localidade.Pais.Nome,
                                    Abreviacao = pedido.Remetente.Localidade.Pais.Abreviacao
                                }
                            }
                        },
                        GrupoPessoas = (pedido.Remetente.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.GrupoPessoas()
                        {
                            Codigo = pedido.Remetente.GrupoPessoas.Codigo,
                            Descricao = pedido.Remetente.GrupoPessoas.Descricao,
                            TornarPedidosPrioritarios = pedido.Remetente.GrupoPessoas.TornarPedidosPrioritarios,
                            DiasDePrazoFatura = pedido.Remetente.GrupoPessoas.DiasDePrazoFatura,
                            TipoOperacaoColeta = (pedido.Remetente.GrupoPessoas.TipoOperacaoColeta == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoOperacao()
                            {
                                Codigo = pedido.Remetente.GrupoPessoas.TipoOperacaoColeta.Codigo,
                                Descricao = pedido.Remetente.GrupoPessoas.TipoOperacaoColeta.Descricao
                            },
                            FormaPagamento = (pedido.Remetente.GrupoPessoas.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                            {
                                Codigo = pedido.Remetente.GrupoPessoas.FormaPagamento.Codigo,
                                Descricao = pedido.Remetente.GrupoPessoas.FormaPagamento.Descricao
                            }
                        },
                        FormaPagamento = (pedido.Remetente.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                        {
                            Codigo = pedido.Remetente.FormaPagamento.Codigo,
                            Descricao = pedido.Remetente.FormaPagamento.Descricao
                        }
                    },
                    Destinatario = (pedido.Destinatario == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.Destinatario.CPF_CNPJ,
                        CodigoIntegracao = pedido.Destinatario.CodigoIntegracao,
                        CpfCnpj = pedido.Destinatario.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.Destinatario.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.Destinatario.CPF_CNPJ),
                        DiasDePrazoFatura = pedido.Destinatario.DiasDePrazoFatura,
                        Nome = pedido.Destinatario.Nome,
                        NomeFantasia = pedido.Destinatario.NomeFantasia,
                        Observacao = pedido.Destinatario.Observacao,
                        PontoTransbordo = (bool?)pedido.Destinatario.PontoTransbordo ?? false,
                        Tipo = pedido.Destinatario.Tipo,
                        ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = (bool?)pedido.Destinatario.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento ?? false,
                        ValorMinimoCarga = pedido.Destinatario.ValorMinimoCarga,
                        Categoria = (pedido.Destinatario.Categoria == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.CategoriaPessoa()
                        {
                            Codigo = pedido.Destinatario.Categoria.Codigo,
                            Descricao = pedido.Destinatario.Categoria.Descricao
                        },
                        Regiao = (pedido.Destinatario.Regiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Regiao()
                        {
                            Codigo = pedido.Destinatario.Regiao.Codigo,
                            Descricao = pedido.Destinatario.Regiao.Descricao
                        },
                        MesoRegiao = (pedido.Destinatario.MesoRegiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.MesoRegiao()
                        {
                            Codigo = pedido.Destinatario.MesoRegiao.Codigo,
                            Descricao = pedido.Destinatario.MesoRegiao.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                        {
                            Logradouro = pedido.Destinatario.Endereco,
                            Bairro = pedido.Destinatario.Bairro,
                            Numero = pedido.Destinatario.Numero,
                            Cep = pedido.Destinatario.CEP,
                            Latitude = pedido.Destinatario.Latitude,
                            Longitude = pedido.Destinatario.Longitude,
                            LatitudeTransbordo = pedido.Destinatario.LatitudeTransbordo,
                            LongitudeTransbordo = pedido.Destinatario.LongitudeTransbordo,
                            Localidade = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                            {
                                Codigo = pedido.Destinatario.Localidade.Codigo,
                                CodigoIbge = pedido.Destinatario.Localidade.CodigoIBGE,
                                Descricao = pedido.Destinatario.Localidade.Descricao,
                                Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                                {
                                    Descricao = pedido.Destinatario.Localidade.Estado.Nome,
                                    Sigla = pedido.Destinatario.Localidade.Estado.Sigla,
                                },
                                Pais = (pedido.Destinatario.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                                {
                                    Codigo = pedido.Destinatario.Localidade.Pais.Codigo,
                                    Nome = pedido.Destinatario.Localidade.Pais.Nome,
                                    Abreviacao = pedido.Destinatario.Localidade.Pais.Abreviacao
                                }
                            }
                        },
                        GrupoPessoas = (pedido.Destinatario.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.GrupoPessoas()
                        {
                            Codigo = pedido.Destinatario.GrupoPessoas.Codigo,
                            Descricao = pedido.Destinatario.GrupoPessoas.Descricao,
                            TornarPedidosPrioritarios = pedido.Destinatario.GrupoPessoas.TornarPedidosPrioritarios,
                            DiasDePrazoFatura = pedido.Destinatario.GrupoPessoas.DiasDePrazoFatura,
                            FormaPagamento = (pedido.Destinatario.GrupoPessoas.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                            {
                                Codigo = pedido.Destinatario.GrupoPessoas.FormaPagamento.Codigo,
                                Descricao = pedido.Destinatario.GrupoPessoas.FormaPagamento.Descricao
                            }
                        },
                        FormaPagamento = (pedido.Destinatario.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                        {
                            Codigo = pedido.Destinatario.FormaPagamento.Codigo,
                            Descricao = pedido.Destinatario.FormaPagamento.Descricao
                        }
                    },
                    Expedidor = (pedido.Expedidor == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.Expedidor.CPF_CNPJ,
                        CodigoIntegracao = pedido.Expedidor.CodigoIntegracao,
                        CpfCnpj = pedido.Expedidor.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.Expedidor.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.Expedidor.CPF_CNPJ),
                        DiasDePrazoFatura = pedido.Expedidor.DiasDePrazoFatura,
                        Nome = pedido.Expedidor.Nome,
                        NomeFantasia = pedido.Expedidor.NomeFantasia,
                        Observacao = pedido.Expedidor.Observacao,
                        PontoTransbordo = (bool?)pedido.Expedidor.PontoTransbordo ?? false,
                        Tipo = pedido.Expedidor.Tipo,
                        ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = (bool?)pedido.Expedidor.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento ?? false,
                        ValorMinimoCarga = pedido.Expedidor.ValorMinimoCarga,
                        Categoria = (pedido.Expedidor.Categoria == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.CategoriaPessoa()
                        {
                            Codigo = pedido.Expedidor.Categoria.Codigo,
                            Descricao = pedido.Expedidor.Categoria.Descricao
                        },
                        Regiao = (pedido.Expedidor.Regiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Regiao()
                        {
                            Codigo = pedido.Expedidor.Regiao.Codigo,
                            Descricao = pedido.Expedidor.Regiao.Descricao
                        },
                        MesoRegiao = (pedido.Expedidor.MesoRegiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.MesoRegiao()
                        {
                            Codigo = pedido.Expedidor.MesoRegiao.Codigo,
                            Descricao = pedido.Expedidor.MesoRegiao.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                        {
                            Logradouro = pedido.Expedidor.Endereco,
                            Bairro = pedido.Expedidor.Bairro,
                            Numero = pedido.Expedidor.Numero,
                            Cep = pedido.Expedidor.CEP,
                            Latitude = pedido.Expedidor.Latitude,
                            Longitude = pedido.Expedidor.Longitude,
                            LatitudeTransbordo = pedido.Expedidor.LatitudeTransbordo,
                            LongitudeTransbordo = pedido.Expedidor.LongitudeTransbordo,
                            Localidade = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                            {
                                Codigo = pedido.Expedidor.Localidade.Codigo,
                                CodigoIbge = pedido.Expedidor.Localidade.CodigoIBGE,
                                Descricao = pedido.Expedidor.Localidade.Descricao,
                                Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                                {
                                    Descricao = pedido.Expedidor.Localidade.Estado.Nome,
                                    Sigla = pedido.Expedidor.Localidade.Estado.Sigla,
                                },
                                Pais = (pedido.Expedidor.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                                {
                                    Codigo = pedido.Expedidor.Localidade.Pais.Codigo,
                                    Nome = pedido.Expedidor.Localidade.Pais.Nome,
                                    Abreviacao = pedido.Expedidor.Localidade.Pais.Abreviacao
                                }
                            }
                        },
                        GrupoPessoas = (pedido.Expedidor.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.GrupoPessoas()
                        {
                            Codigo = pedido.Expedidor.GrupoPessoas.Codigo,
                            Descricao = pedido.Expedidor.GrupoPessoas.Descricao,
                            TornarPedidosPrioritarios = pedido.Expedidor.GrupoPessoas.TornarPedidosPrioritarios,
                            DiasDePrazoFatura = pedido.Expedidor.GrupoPessoas.DiasDePrazoFatura,
                            FormaPagamento = (pedido.Expedidor.GrupoPessoas.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                            {
                                Codigo = pedido.Expedidor.GrupoPessoas.FormaPagamento.Codigo,
                                Descricao = pedido.Expedidor.GrupoPessoas.FormaPagamento.Descricao
                            }
                        },
                        FormaPagamento = (pedido.Expedidor.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                        {
                            Codigo = pedido.Expedidor.FormaPagamento.Codigo,
                            Descricao = pedido.Expedidor.FormaPagamento.Descricao
                        }
                    },
                    Recebedor = (pedido.Recebedor == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.Recebedor.CPF_CNPJ,
                        CodigoIntegracao = pedido.Recebedor.CodigoIntegracao,
                        CpfCnpj = pedido.Recebedor.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.Recebedor.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.Recebedor.CPF_CNPJ),
                        DiasDePrazoFatura = pedido.Recebedor.DiasDePrazoFatura,
                        Nome = pedido.Recebedor.Nome,
                        NomeFantasia = pedido.Recebedor.NomeFantasia,
                        Observacao = pedido.Recebedor.Observacao,
                        PontoTransbordo = (bool?)pedido.Recebedor.PontoTransbordo ?? false,
                        Tipo = pedido.Recebedor.Tipo,
                        ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = (bool?)pedido.Recebedor.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento ?? false,
                        ValorMinimoCarga = pedido.Recebedor.ValorMinimoCarga,
                        Categoria = (pedido.Recebedor.Categoria == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.CategoriaPessoa()
                        {
                            Codigo = pedido.Recebedor.Categoria.Codigo,
                            Descricao = pedido.Recebedor.Categoria.Descricao
                        },
                        Regiao = (pedido.Recebedor.Regiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Regiao()
                        {
                            Codigo = pedido.Recebedor.Regiao.Codigo,
                            Descricao = pedido.Recebedor.Regiao.Descricao
                        },
                        MesoRegiao = (pedido.Recebedor.MesoRegiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.MesoRegiao()
                        {
                            Codigo = pedido.Recebedor.MesoRegiao.Codigo,
                            Descricao = pedido.Recebedor.MesoRegiao.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                        {
                            Logradouro = pedido.Recebedor.Endereco,
                            Bairro = pedido.Recebedor.Bairro,
                            Numero = pedido.Recebedor.Numero,
                            Cep = pedido.Recebedor.CEP,
                            Latitude = pedido.Recebedor.Latitude,
                            Longitude = pedido.Recebedor.Longitude,
                            LatitudeTransbordo = pedido.Recebedor.LatitudeTransbordo,
                            LongitudeTransbordo = pedido.Recebedor.LongitudeTransbordo,
                            Localidade = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                            {
                                Codigo = pedido.Recebedor.Localidade.Codigo,
                                CodigoIbge = pedido.Recebedor.Localidade.CodigoIBGE,
                                Descricao = pedido.Recebedor.Localidade.Descricao,
                                Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                                {
                                    Descricao = pedido.Recebedor.Localidade.Estado.Nome,
                                    Sigla = pedido.Recebedor.Localidade.Estado.Sigla,
                                },
                                Pais = (pedido.Recebedor.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                                {
                                    Codigo = pedido.Recebedor.Localidade.Pais.Codigo,
                                    Nome = pedido.Recebedor.Localidade.Pais.Nome,
                                    Abreviacao = pedido.Recebedor.Localidade.Pais.Abreviacao
                                }
                            }
                        },
                        GrupoPessoas = (pedido.Recebedor.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.GrupoPessoas()
                        {
                            Codigo = pedido.Recebedor.GrupoPessoas.Codigo,
                            Descricao = pedido.Recebedor.GrupoPessoas.Descricao,
                            TornarPedidosPrioritarios = pedido.Recebedor.GrupoPessoas.TornarPedidosPrioritarios,
                            DiasDePrazoFatura = pedido.Recebedor.GrupoPessoas.DiasDePrazoFatura,
                            FormaPagamento = (pedido.Recebedor.GrupoPessoas.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                            {
                                Codigo = pedido.Recebedor.GrupoPessoas.FormaPagamento.Codigo,
                                Descricao = pedido.Recebedor.GrupoPessoas.FormaPagamento.Descricao
                            }
                        },
                        FormaPagamento = (pedido.Recebedor.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                        {
                            Codigo = pedido.Recebedor.FormaPagamento.Codigo,
                            Descricao = pedido.Recebedor.FormaPagamento.Descricao
                        }
                    },
                    Tomador = (pedido.Tomador == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.Tomador.CPF_CNPJ,
                        CodigoIntegracao = pedido.Tomador.CodigoIntegracao,
                        CpfCnpj = pedido.Tomador.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.Tomador.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.Tomador.CPF_CNPJ),
                        DiasDePrazoFatura = pedido.Tomador.DiasDePrazoFatura,
                        Nome = pedido.Tomador.Nome,
                        NomeFantasia = pedido.Tomador.NomeFantasia,
                        Observacao = pedido.Tomador.Observacao,
                        PontoTransbordo = (bool?)pedido.Tomador.PontoTransbordo ?? false,
                        Tipo = pedido.Tomador.Tipo,
                        ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = (bool?)pedido.Tomador.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento ?? false,
                        ValorMinimoCarga = pedido.Tomador.ValorMinimoCarga,
                        Categoria = (pedido.Tomador.Categoria == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.CategoriaPessoa()
                        {
                            Codigo = pedido.Tomador.Categoria.Codigo,
                            Descricao = pedido.Tomador.Categoria.Descricao
                        },
                        Regiao = (pedido.Tomador.Regiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Regiao()
                        {
                            Codigo = pedido.Tomador.Regiao.Codigo,
                            Descricao = pedido.Tomador.Regiao.Descricao
                        },
                        MesoRegiao = (pedido.Tomador.MesoRegiao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.MesoRegiao()
                        {
                            Codigo = pedido.Tomador.MesoRegiao.Codigo,
                            Descricao = pedido.Tomador.MesoRegiao.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                        {
                            Logradouro = pedido.Tomador.Endereco,
                            Bairro = pedido.Tomador.Bairro,
                            Numero = pedido.Tomador.Numero,
                            Cep = pedido.Tomador.CEP,
                            Latitude = pedido.Tomador.Latitude,
                            Longitude = pedido.Tomador.Longitude,
                            LatitudeTransbordo = pedido.Tomador.LatitudeTransbordo,
                            LongitudeTransbordo = pedido.Tomador.LongitudeTransbordo,
                            Localidade = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Localidade()
                            {
                                Codigo = pedido.Tomador.Localidade.Codigo,
                                CodigoIbge = pedido.Tomador.Localidade.CodigoIBGE,
                                Descricao = pedido.Tomador.Localidade.Descricao,
                                Estado = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Estado()
                                {
                                    Descricao = pedido.Tomador.Localidade.Estado.Nome,
                                    Sigla = pedido.Tomador.Localidade.Estado.Sigla,
                                },
                                Pais = (pedido.Tomador.Localidade.Pais == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pais()
                                {
                                    Codigo = pedido.Tomador.Localidade.Pais.Codigo,
                                    Nome = pedido.Tomador.Localidade.Pais.Nome,
                                    Abreviacao = pedido.Tomador.Localidade.Pais.Abreviacao
                                }
                            }
                        },
                        GrupoPessoas = (pedido.Tomador.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.GrupoPessoas()
                        {
                            Codigo = pedido.Tomador.GrupoPessoas.Codigo,
                            Descricao = pedido.Tomador.GrupoPessoas.Descricao,
                            TornarPedidosPrioritarios = pedido.Tomador.GrupoPessoas.TornarPedidosPrioritarios,
                            DiasDePrazoFatura = pedido.Tomador.GrupoPessoas.DiasDePrazoFatura,
                            FormaPagamento = (pedido.Tomador.GrupoPessoas.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                            {
                                Codigo = pedido.Tomador.GrupoPessoas.FormaPagamento.Codigo,
                                Descricao = pedido.Tomador.GrupoPessoas.FormaPagamento.Descricao
                            }
                        },
                        FormaPagamento = (pedido.Tomador.FormaPagamento == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento()
                        {
                            Codigo = pedido.Tomador.FormaPagamento.Codigo,
                            Descricao = pedido.Tomador.FormaPagamento.Descricao
                        }
                    },
                    RecebedorColeta = (pedido.RecebedorColeta == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.RecebedorColeta.CPF_CNPJ,
                        CodigoIntegracao = pedido.RecebedorColeta.CodigoIntegracao,
                        CpfCnpj = pedido.RecebedorColeta.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.RecebedorColeta.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.RecebedorColeta.CPF_CNPJ),
                        Nome = pedido.RecebedorColeta.Nome,
                        NomeFantasia = pedido.RecebedorColeta.NomeFantasia,
                        PontoTransbordo = (bool?)pedido.RecebedorColeta.PontoTransbordo ?? false,
                        Tipo = pedido.RecebedorColeta.Tipo
                    },
                    LocalPaletizacao = (pedido.LocalPaletizacao == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.LocalPaletizacao.CPF_CNPJ,
                        CodigoIntegracao = pedido.LocalPaletizacao.CodigoIntegracao,
                        CpfCnpj = pedido.LocalPaletizacao.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.LocalPaletizacao.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.LocalPaletizacao.CPF_CNPJ),
                        Nome = pedido.LocalPaletizacao.Nome,
                        NomeFantasia = pedido.LocalPaletizacao.NomeFantasia,
                        PontoTransbordo = (bool?)pedido.LocalPaletizacao.PontoTransbordo ?? false,
                        Tipo = pedido.LocalPaletizacao.Tipo
                    },
                    Fronteira = (pedido.Fronteira == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                    {
                        Codigo = pedido.Fronteira.CPF_CNPJ,
                        CodigoIntegracao = pedido.Fronteira.CodigoIntegracao,
                        CpfCnpj = pedido.Fronteira.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.Fronteira.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.Fronteira.CPF_CNPJ),
                        Nome = pedido.Fronteira.Nome,
                        NomeFantasia = pedido.Fronteira.NomeFantasia,
                        PontoTransbordo = (bool?)pedido.Fronteira.PontoTransbordo ?? false,
                        Tipo = pedido.Fronteira.Tipo,
                        Endereco = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                        {
                            Latitude = pedido.Fronteira.Latitude,
                            Longitude = pedido.Fronteira.Longitude
                        }
                    },
                    GrupoPessoas = (pedido.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.GrupoPessoas()
                    {
                        Codigo = pedido.GrupoPessoas.Codigo,
                        Descricao = pedido.GrupoPessoas.Descricao,
                        TornarPedidosPrioritarios = pedido.GrupoPessoas.TornarPedidosPrioritarios
                    },
                    ModeloVeicularCarga = (pedido.ModeloVeicularCarga == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.ModeloVeicularCarga()
                    {
                        Codigo = pedido.ModeloVeicularCarga.Codigo,
                        Descricao = pedido.ModeloVeicularCarga.Descricao,
                        CapacidadePesoTransporte = pedido.ModeloVeicularCarga.CapacidadePesoTransporte,
                        Cubagem = pedido.ModeloVeicularCarga.Cubagem,
                        ExigirDefinicaoReboquePedido = pedido.ModeloVeicularCarga.ExigirDefinicaoReboquePedido,
                        ModeloControlaCubagem = pedido.ModeloVeicularCarga.ModeloControlaCubagem,
                        NumeroPaletes = pedido.ModeloVeicularCarga.NumeroPaletes,
                        NumeroReboques = pedido.ModeloVeicularCarga.NumeroReboques,
                        OcupacaoCubicaPaletes = (decimal?)pedido.ModeloVeicularCarga.OcupacaoCubicaPaletes ?? 0m,
                        ToleranciaMinimaCubagem = pedido.ModeloVeicularCarga.ToleranciaMinimaCubagem,
                        ToleranciaMinimaPaletes = pedido.ModeloVeicularCarga.ToleranciaMinimaPaletes,
                        ToleranciaPesoMenor = pedido.ModeloVeicularCarga.ToleranciaPesoMenor,
                        VeiculoPaletizado = pedido.ModeloVeicularCarga.VeiculoPaletizado
                    },
                    CanalEntrega = (pedido.CanalEntrega == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.CanalEntrega()
                    {
                        Codigo = pedido.CanalEntrega.Codigo,
                        Descricao = pedido.CanalEntrega.Descricao
                    },
                    CanalVenda = (pedido.CanalVenda == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.CanalVenda()
                    {
                        Codigo = pedido.CanalVenda.Codigo,
                        Descricao = pedido.CanalVenda.Descricao
                    },
                    RotaFrete = (pedido.RotaFrete == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.RotaFrete()
                    {
                        Codigo = pedido.RotaFrete.Codigo,
                        Descricao = pedido.RotaFrete.Descricao
                    },
                    FuncionarioGerente = (pedido.FuncionarioGerente == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Usuario()
                    {
                        Codigo = pedido.FuncionarioGerente.Codigo,
                        Nome = pedido.FuncionarioGerente.Nome,
                        Cpf = pedido.FuncionarioGerente.CPF,
                        Email = pedido.FuncionarioGerente.Email,
                        MotoristaEstrangeiro = (bool?)pedido.FuncionarioGerente.MotoristaEstrangeiro ?? false,
                        Telefone = pedido.FuncionarioGerente.Telefone,
                        TipoAcesso = pedido.FuncionarioGerente.TipoAcesso
                    },
                    FuncionarioSupervisor = (pedido.FuncionarioSupervisor == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Usuario()
                    {
                        Codigo = pedido.FuncionarioSupervisor.Codigo,
                        Nome = pedido.FuncionarioSupervisor.Nome,
                        Cpf = pedido.FuncionarioSupervisor.CPF,
                        Email = pedido.FuncionarioSupervisor.Email,
                        MotoristaEstrangeiro = (bool?)pedido.FuncionarioSupervisor.MotoristaEstrangeiro ?? false,
                        Telefone = pedido.FuncionarioSupervisor.Telefone,
                        TipoAcesso = pedido.FuncionarioSupervisor.TipoAcesso
                    },
                    FuncionarioVendedor = (pedido.FuncionarioVendedor == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Usuario()
                    {
                        Codigo = pedido.FuncionarioVendedor.Codigo,
                        Nome = pedido.FuncionarioVendedor.Nome,
                        Cpf = pedido.FuncionarioVendedor.CPF,
                        Email = pedido.FuncionarioVendedor.Email,
                        MotoristaEstrangeiro = (bool?)pedido.FuncionarioVendedor.MotoristaEstrangeiro ?? false,
                        Telefone = pedido.FuncionarioVendedor.Telefone,
                        TipoAcesso = pedido.FuncionarioVendedor.TipoAcesso
                    },
                    Usuario = (pedido.Usuario == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Usuario()
                    {
                        Codigo = pedido.Usuario.Codigo,
                        Nome = pedido.Usuario.Nome,
                        Cpf = pedido.Usuario.CPF,
                        Email = pedido.Usuario.Email,
                        MotoristaEstrangeiro = (bool?)pedido.Usuario.MotoristaEstrangeiro ?? false,
                        Telefone = pedido.Usuario.Telefone,
                        TipoAcesso = pedido.Usuario.TipoAcesso
                    },
                    Autor = (pedido.Autor == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Usuario()
                    {
                        Codigo = pedido.Autor.Codigo,
                        Nome = pedido.Autor.Nome,
                        Cpf = pedido.Autor.CPF,
                        Email = pedido.Autor.Email,
                        MotoristaEstrangeiro = (bool?)pedido.Autor.MotoristaEstrangeiro ?? false,
                        Telefone = pedido.Autor.Telefone,
                        TipoAcesso = pedido.Autor.TipoAcesso
                    },
                    SituacaoComercialPedido = (pedido.SituacaoComercialPedido == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.SituacaoComercialPedido()
                    {
                        Codigo = pedido.SituacaoComercialPedido.Codigo,
                        Descricao = pedido.SituacaoComercialPedido.Descricao,
                        Cor = pedido.SituacaoComercialPedido.Cor,
                    },
                    Container = (pedido.Container == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Container()
                    {
                        Codigo = pedido.Container.Codigo,
                        Numero = pedido.Container.Numero
                    },
                    PedidoViagemNavio = (pedido.PedidoViagemNavio == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoViagemNavio()
                    {
                        Codigo = pedido.PedidoViagemNavio.Codigo,
                        Descricao = pedido.PedidoViagemNavio.Descricao
                    },
                    Porto = (pedido.Porto == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Porto()
                    {
                        Codigo = pedido.Porto.Codigo,
                        Descricao = pedido.Porto.Descricao
                    },
                    PortoDestino = (pedido.PortoDestino == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Porto()
                    {
                        Codigo = pedido.PortoDestino.Codigo,
                        Descricao = pedido.PortoDestino.Descricao
                    }
                })
                .WithOptions(o => { o.SetTimeout(300); })
                .ToListAsync(CancellationToken);

            if (pedidos.Count > 0)
            {
                List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoAdicional PedidoAdicional)> pedidoAdicionais = new List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoAdicional PedidoAdicional)>();
                List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoProduto PedidoProduto)> pedidoProdutos = new List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoProduto PedidoProduto)>();
                List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoAnexo Anexo)> pedidoAnexos = new List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoAnexo Anexo)>();
                List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente Fronteira)> pedidoFronteiras = new List<(int CodigoPedido, Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente Fronteira)>();

                for (int registroInicial = 0; registroInicial < pedidos.Count; registroInicial += limiteRegistros)
                {
                    List<int> codigosPedidos = pedidos.Select(pedido => pedido.Codigo).Skip(registroInicial).Take(limiteRegistros).ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> consultaPedidoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                        .Where(pedidoAdicional => codigosPedidos.Contains(pedidoAdicional.Pedido.Codigo));

                    pedidoAdicionais.AddRange(await consultaPedidoAdicional
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(pedidoAdicional => ValueTuple.Create(
                            pedidoAdicional.Pedido.Codigo,
                            new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoAdicional()
                            {
                                AjudanteCarga = (bool?)pedidoAdicional.AjudanteCarga ?? false,
                                AjudanteDescarga = (bool?)pedidoAdicional.AjudanteDescarga ?? false,
                                QtdAjudantesCarga = (int?)pedidoAdicional.QtdAjudantesCarga ?? 0,
                                QtdAjudantesDescarga = (int?)pedidoAdicional.QtdAjudantesDescarga ?? 0,
                                SituacaoEstoquePedido = (pedidoAdicional.SituacaoEstoquePedido == null) ? null : new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.SituacaoEstoquePedido()
                                {
                                    Codigo = pedidoAdicional.SituacaoEstoquePedido.Codigo,
                                    Descricao = pedidoAdicional.SituacaoEstoquePedido.Descricao,
                                    Cor = pedidoAdicional.SituacaoEstoquePedido.Cor,
                                },
                            }
                        ))
                        .ToListAsync(CancellationToken)
                    );

                    IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> consultaPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>()
                        .Where(pedidoProduto => codigosPedidos.Contains(pedidoProduto.Pedido.Codigo));

                    pedidoProdutos.AddRange(await consultaPedidoProduto
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(pedidoProduto => ValueTuple.Create(
                            pedidoProduto.Pedido.Codigo,
                            new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoProduto()
                            {
                                Codigo = pedidoProduto.Codigo,
                                Quantidade = pedidoProduto.Quantidade,
                                ValorProduto = pedidoProduto.ValorProduto
                            }
                        ))
                        .ToListAsync(CancellationToken)
                    );

                    IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> consultaPedidoAnexo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>()
                        .Where(pedidoAnexo => codigosPedidos.Contains(pedidoAnexo.EntidadeAnexo.Codigo));

                    pedidoAnexos.AddRange(await consultaPedidoAnexo
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(pedidoAnexo => ValueTuple.Create(
                            pedidoAnexo.EntidadeAnexo.Codigo,
                            new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.PedidoAnexo()
                            {
                                Codigo = pedidoAnexo.Codigo,
                                Descricao = pedidoAnexo.Descricao,
                                NomeArquivo = pedidoAnexo.NomeArquivo
                            }
                        ))
                        .ToListAsync(CancellationToken)
                    );

                    IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira> consultaPedidoFronteira = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>()
                        .Where(pedidoFronteira => codigosPedidos.Contains(pedidoFronteira.Pedido.Codigo));

                    pedidoFronteiras.AddRange(await consultaPedidoFronteira
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(pedidoFronteira => ValueTuple.Create(
                            pedidoFronteira.Pedido.Codigo,
                            new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente()
                            {
                                Codigo = pedidoFronteira.Fronteira.CPF_CNPJ,
                                CodigoIntegracao = pedidoFronteira.Fronteira.CodigoIntegracao,
                                CpfCnpj = pedidoFronteira.Fronteira.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedidoFronteira.Fronteira.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedidoFronteira.Fronteira.CPF_CNPJ),
                                Nome = pedidoFronteira.Fronteira.Nome,
                                NomeFantasia = pedidoFronteira.Fronteira.NomeFantasia,
                                PontoTransbordo = (bool?)pedidoFronteira.Fronteira.PontoTransbordo ?? false,
                                Tipo = pedidoFronteira.Fronteira.Tipo,
                                Endereco = new Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Endereco()
                                {
                                    Latitude = pedidoFronteira.Fronteira.Latitude,
                                    Longitude = pedidoFronteira.Fronteira.Longitude
                                }
                            }
                        ))
                        .ToListAsync(CancellationToken)
                    );
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido pedido in pedidos)
                {
                    pedido.PedidoAdicional = pedidoAdicionais
                        .Where(pedidoAdicional => pedidoAdicional.CodigoPedido == pedido.Codigo)
                        .Select(pedidoAdicional => pedidoAdicional.PedidoAdicional)
                        .FirstOrDefault();

                    pedido.Produtos = pedidoProdutos
                        .Where(produto => produto.CodigoPedido == pedido.Codigo)
                        .Select(produto => produto.PedidoProduto)
                        .ToList();

                    pedido.Anexos = pedidoAnexos
                        .Where(anexo => anexo.CodigoPedido == pedido.Codigo)
                        .Select(anexo => anexo.Anexo)
                        .ToList();

                    if (pedido.Fronteira != null)
                        pedido.Fronteiras = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente>() { pedido.Fronteira };
                    else
                        pedido.Fronteiras = pedidoFronteiras
                            .Where(fronteira => fronteira.CodigoPedido == pedido.Codigo)
                            .Select(fronteira => fronteira.Fronteira)
                            .ToList();
                }
            }

            return pedidos;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> BuscarNumeroNotasFiscaisPorPedidos(List<int> pedidos, bool considerarApenasVinculoPedido = false)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> result = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                string sqlQuery = @"
	                             select ped_codigo as CodigoPedido, xnf.NF_NUMERO as NumeroNota 
                                    from T_PEDIDO_NOTAS_FISCAIS pnf inner join t_xml_nota_fiscal xnf on xnf.NFX_CODIGO = pnf.NFX_CODIGO and xnf.NF_ATIVA = 1
                                 where PED_CODIGO in ( :codigos ) 
                                 ";

                if (!considerarApenasVinculoPedido)
                    sqlQuery += @"UNION 
                                select DISTINCT t_pedido.ped_codigo as CodigoPedido, xnf.NF_NUMERO as numeroNota
                                    from T_CARGA_PEDIDO inner join t_pedido on t_carga_pedido.ped_codigo = t_pedido.ped_codigo
                                    left join T_PEDIDO_XML_NOTA_FISCAL on T_PEDIDO_XML_NOTA_FISCAL.cpe_codigo = t_carga_pedido.cpe_codigo
                                    inner join t_xml_nota_fiscal xnf on xnf.nfx_codigo = T_PEDIDO_XML_NOTA_FISCAL.nfx_codigo
                                where t_pedido.PED_CODIGO in ( :codigos )
                                UNION
                                select DISTINCT ped_codigo as CodigoPedido,  CNP_NUMERO as NumeroNota
                                    from T_PEDIDO_NOTA_FISCAL_PARCIAL where PED_CODIGO in  ( :codigos ) ";

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameterList("codigos", tmp);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal)));

                result.AddRange(query.List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal>());

                start += take;
            }

            return result;
        }

        public List<(int CodigoPedido, DateTime? DataDigitalizacaoCanhoto, DateTime? DataEntregaNotaCanhoto)> BuscarDataDigitalizacaoCanhotoAvulsoPorPedidos(List<int> pedidos)
        {
            List<(int CodigoPedido, DateTime? DataDigitalizacaoCanhoto, DateTime? DataEntregaNotaCanhoto)> result = new List<(int CodigoPedido, DateTime? DataDigitalizacaoCanhoto, DateTime? DataEntregaNotaCanhoto)>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                string sqlQuery = @"SELECT 
	                                    cargaPedido.PED_CODIGO AS CodigoPedido, 
	                                    MAX(canhoto.CNF_DATA_DIGITALIZACAO) AS DataDigitalizacaoCanhoto,
                                        MAX(canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE) AS DataEntregaNotaCanhoto
                                    FROM 
	                                    T_CANHOTO_NOTA_FISCAL canhoto
	                                    JOIN T_CANHOTO_AVULSO canhotoAvulso ON canhotoAvulso.CAV_CODIGO = canhoto.CAV_CODIGO
	                                    JOIN T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL canhotoAvulsoNotaFiscal ON canhotoAvulsoNotaFiscal.CAV_CODIGO = canhotoAvulso.CAV_CODIGO
	                                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXMLNotaFiscal ON pedidoXMLNotaFiscal.PNF_CODIGO = canhotoAvulsoNotaFiscal.PNF_CODIGO
	                                    JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.CPE_CODIGO = pedidoXMLNotaFiscal.CPE_CODIGO
                                    WHERE 
	                                    cargaPedido.PED_CODIGO in ( :codigos )
                                    GROUP BY cargaPedido.PED_CODIGO
                                    ORDER BY DataDigitalizacaoCanhoto DESC";

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameterList("codigos", tmp);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoPedido, DateTime? DataDigitalizacaoCanhoto, DateTime? DataEntregaNotaCanhoto)).GetConstructors().FirstOrDefault()));

                result.AddRange(query.List<(int CodigoPedido, DateTime? DataDigitalizacaoCanhoto, DateTime? DataEntregaNotaCanhoto)>());

                start += take;
            }

            return result;
        }

        public List<(int CodigoPedido, string CodigoCargaEmbarcador)> BuscarNumeroPrimieraCargaPorPedidos(List<int> pedidos)
        {
            List<(int CodigoPedido, string CodigoCargaEmbarcador)> result = new List<(int CodigoPedido, string CodigoCargaEmbarcador)>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                string sqlQuery = @"SELECT 
                                        cargaPedido.PED_CODIGO AS CodigoPedido, 
                                        carga.CAR_CODIGO_CARGA_EMBARCADOR AS CodigoCargaEmbarcador 
                                    FROM 
                                        T_CARGA_PEDIDO cargaPedido
                                        JOIN T_CARGA carga ON carga.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                    WHERE 
		                                carga.CAR_SITUACAO NOT IN ( :situacoes )
                                        AND cargaPedido.PED_CODIGO in ( :codigos )
	                                ORDER BY carga.CAR_CODIGO";

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameterList("codigos", tmp);
                query.SetParameterList("situacoes", SituacaoCargaHelper.ObterSituacoesCargaCancelada());
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoPedido, string CodigoCargaEmbarcador)).GetConstructors().FirstOrDefault()));

                result.AddRange(query.List<(int CodigoPedido, string CodigoCargaEmbarcador)>());

                start += take;
            }

            return result;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> BuscarNotasPorPedidos(List<int> pedidos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> result = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                //var sqlQuery = @"select ped_codigo as Codigo, nfx_codigo as Total from T_PEDIDO_NOTAS_FISCAIS where PED_CODIGO in ( :codigos )";
                string sqlQuery = @"
select ped_codigo as Codigo, pnf.nfx_codigo as Total 
  from T_PEDIDO_NOTAS_FISCAIS pnf inner join t_xml_nota_fiscal xnf on xnf.NFX_CODIGO = pnf.NFX_CODIGO and xnf.NF_ATIVA = 1
 where PED_CODIGO in ( :codigos ) ";

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameterList("codigos", tmp);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento)));

                result.AddRange(query.List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>());

                start += take;
            }

            return result;
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>> BuscarNotasPorPedidosAsync(List<int> pedidos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> result = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();

            int take = 1000;
            int start = 0;
            while (start < pedidos?.Count)
            {
                List<int> tmp = pedidos.Skip(start).Take(take).ToList();

                string sqlQuery = @"select ped_codigo as Codigo, pnf.nfx_codigo as Total 
                                     from T_PEDIDO_NOTAS_FISCAIS pnf inner join t_xml_nota_fiscal xnf on xnf.NFX_CODIGO = pnf.NFX_CODIGO and xnf.NF_ATIVA = 1
                                     where PED_CODIGO in ( :codigos ) ";

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameterList("codigos", tmp);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento)));

                result.AddRange(await query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>(CancellationToken));

                start += take;
            }

            return result;
        }

        public IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarAtendimentoPedidoCliente(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedidoCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            if (filtrosPesquisa.codigoPedido > 0)
                query = query.Where(obj => obj.Codigo == filtrosPesquisa.codigoPedido);

            if (filtrosPesquisa.DataInicial.HasValue && filtrosPesquisa.DataInicial.Value != DateTime.MinValue)
                query = query.Where(obj => obj.DataCriacao >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue && filtrosPesquisa.DataFinal.Value != DateTime.MinValue)
                query = query.Where(obj => obj.DataCriacao <= filtrosPesquisa.DataFinal.Value);

            if (filtrosPesquisa.CNPJDestinatario > 0)
                query = query.Where(obj => obj.Destinatario.CPF_CNPJ == filtrosPesquisa.CNPJDestinatario);

            if (filtrosPesquisa.NotaFiscal > 0)
            {
                List<int> codigosPedidosComNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                    .Where(obj => obj.XMLNotaFiscal.Numero == filtrosPesquisa.NotaFiscal)
                    .Select(obj => obj.CargaPedido.Pedido.Codigo)
                    .ToList();

                for (int i = 0; i < codigosPedidosComNotaFiscal.Count; i += 1500)
                {
                    List<int> codigosPedidosPesquisar = codigosPedidosComNotaFiscal.Skip(i).Take(1500).ToList();
                    query = query.Where(obj => codigosPedidosPesquisar.Contains(obj.Codigo));
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                query = query.Where(obj => obj.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador);

            if (filtrosPesquisa.CodigoCliente > 0d)
                query = query.Where(obj => obj.Remetente.CPF_CNPJ == filtrosPesquisa.CodigoCliente || obj.Destinatario.CPF_CNPJ == filtrosPesquisa.CodigoCliente);

            //lista apenas os pedidos que possuem chat #43825
            query = query.Where(obj => obj.PossuiChatAtivo == true && obj.SituacaoPedido != SituacaoPedido.Cancelado);

            return query;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.PedidoDestinatarioBloqueado> BuscarPedidosDestinatarioBloqueados(List<int> pedidos)
        {
            if (pedidos?.Count == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.Carga.PedidoDestinatarioBloqueado>();

            const int maxParameters = 2000;
            List<Dominio.ObjetosDeValor.Embarcador.Carga.PedidoDestinatarioBloqueado> resultado = new List<Dominio.ObjetosDeValor.Embarcador.Carga.PedidoDestinatarioBloqueado>();

            for (int i = 0; i < pedidos.Count; i += maxParameters)
            {
                var subListaPedidos = pedidos.Skip(i).Take(maxParameters).ToList();
                resultado.AddRange(ExecutarConsultaDestinatarioBloqueado(subListaPedidos));
            }

            return resultado;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.PedidoDestinatarioBloqueado> ExecutarConsultaDestinatarioBloqueado(List<int> subListaPedidos)
        {
            string sqlQuery = @"SELECT PED_CODIGO as Pedido, PED_CNPJ_CPF_DESTINATARIO_BLOQUEADO as CnpjCpfDestinatarioBloqueado 
                        FROM T_PEDIDO_DESTINATARIOS_BLOQUEADOS 
                        WHERE PED_CODIGO IN (:codigos)";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameterList("codigos", subListaPedidos);

            return query
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.PedidoDestinatarioBloqueado)))
                .List<Dominio.ObjetosDeValor.Embarcador.Carga.PedidoDestinatarioBloqueado>()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNotaFiscal(int codigoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NotasFiscais.Any(nota => nota.Codigo == codigoNotaFiscal) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorNumeroNotaFiscal(int numeroNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                .Where(obj => obj.XMLNotaFiscal.Numero == numeroNotaFiscal);

            return query
                .OrderByDescending(obj => obj.CargaPedido.Codigo).Select(obj => obj.CargaPedido.Pedido).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoDeDevolucaoPorNumeroNotaFiscal(int numeroNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(a => a.NotasFiscais.Any(nota => nota.Numero == numeroNotaFiscal) &&
                a.SituacaoPedido == SituacaoPedido.Aberto);

            return query.FirstOrDefault();
        }

        public async Task<bool> ExistePedidoPercentualSeparacaoInformadoAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>().Where(x => x.SituacaoPedido != SituacaoPedido.Cancelado && x.PercentualSeparacaoPedido > 0);
            return await query.CountAsync(CancellationToken) > 0;
        }

        public void SetarSituacaoAcompanhamentoPorPedidos(List<int> pedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido situacaoAcompanhamentoPedido)
        {
            string hql = "update Pedido pedido set pedido.SituacaoAcompanhamentoPedido = :SituacaoAcompanhamentoPedido where pedido.Codigo in (:Pedidos) ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetEnum("SituacaoAcompanhamentoPedido", situacaoAcompanhamentoPedido);
            query.SetParameterList("Pedidos", pedidos);
            query.ExecuteUpdate();
        }

        public void SetarControleNumeracaoPorPedido(int codigoPedido)
        {
            string hql = "update Pedido pedido set pedido.ControleNumeracao = :controleNumeracao where pedido.Codigo = :codigoPedido ";

            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("controleNumeracao", codigoPedido);
            query.SetInt32("codigoPedido", codigoPedido);

            query.ExecuteUpdate();
        }

        public void DefinirDataEntregaPorCargaEntrega(int codigoCargaEntrega, DateTime? dataEntrega)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("update Pedidos ");
            sql.Append($"  set DataEntregaPedido = :dataEntrega ");
            sql.Append("  from ( ");
            sql.Append("           select Pedido.PED_DATA_ENTREGA DataEntregaPedido ");
            sql.Append("             from T_CARGA_ENTREGA CargaEntrega ");
            sql.Append("             join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
            sql.Append("             join T_CARGA_PEDIDO CargaPedido on CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
            sql.Append("             join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
            sql.Append($"           where CargaEntrega.CEN_CODIGO = :codigoCargaEntrega ");
            sql.Append("              and CargaEntrega.CEN_COLETA = 0 ");
            sql.Append("              and CargaEntrega.CLI_CODIGO_ENTREGA = Pedido.CLI_CODIGO ");
            sql.Append("       ) as Pedidos ");

            UnitOfWork.Sessao
                .CreateSQLQuery(sql.ToString())
                .SetParameter("codigoCargaEntrega", codigoCargaEntrega)
                .SetParameter("dataEntrega", dataEntrega)
                .ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorChaveNFe(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NotasFiscais.Any(x => x.Chave == chaveNFe) select obj;
            return result.FirstOrDefault();
        }

        public async Task<(int CodigoPedido, int CodigoFilial, string DescricaoFilial)?> BuscarPorChaveNFeAsync(string chaveNFe)
        {
            var resultado = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(x => x.NotasFiscais.Any(o => o.Chave == chaveNFe) && x.Filial != null)
                .Select(x => new
                {
                    CodigoPedido = x.Codigo,
                    CodigoFilial = x.Filial.Codigo,
                    DescricaoFilial = x.Filial.Descricao
                })
                .FirstOrDefaultAsync(CancellationToken);

            if (resultado == null)
                return null;

            return (resultado.CodigoPedido, resultado.CodigoFilial, resultado.DescricaoFilial);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterAgendamentoEntregaPedidos(List<int> codigos)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(p => codigos.Contains(p.Codigo)).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosEmContacaoParaExcluir(int quantidadeDiasCriacao, int qtdeRegistros)
        {
            DateTime dataCriacao = DateTime.Now.AddDays(-quantidadeDiasCriacao).Date;
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.SituacaoPedido == SituacaoPedido.EmCotacao && obj.DataCriacao < dataCriacao && !queryCargaPedido.Any(c => c.Pedido.Codigo == obj.Codigo));
            query = query.OrderBy(x => x.Codigo);
            return query.Take(qtdeRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPendentesPorRemetente(double cnpjRemetete, double cnpjDestinatario, int grupoPessoa, List<string> pedidos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros)
        {
            var query = ConsultaPendentesPorRemetente(cnpjRemetete, cnpjDestinatario, grupoPessoa, pedidos);

            query = query.
                Fetch(o => o.TipoOperacao)
               .Fetch(o => o.Destinatario)
               .Fetch(o => o.TipoCarga)
               .Fetch(o => o.ProdutoPrincipal);

            return ObterLista(query, parametros);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorIdLoteTrizy(int idLoteTrizy)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.IDLoteTrizy == idLoteTrizy select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroOS(string numeroOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroOS == numeroOS && obj.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNumeroOSAsync(string numeroOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroOS == numeroOS && obj.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroPedidoEmbarcador(string numeroPedidoEmbarcador, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> lstlstPedidos = null)
        {
            if (lstlstPedidos != null && lstlstPedidos.Count > 0)
                return lstlstPedidos.Where(obj => obj.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && obj.SituacaoPedido != SituacaoPedido.Cancelado).FirstOrDefault();


            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && obj.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNumerosPedidoEmbarcador(List<string> numerosPedidoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where numerosPedidoEmbarcador.Contains(obj.NumeroPedidoEmbarcador) && obj.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroPedidoEmbarcadorOuNumeroPedidoCliente(string numeroPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query
                         where (obj.NumeroPedidoEmbarcador == numeroPedido || obj.CodigoPedidoCliente == numeroPedido)
                         select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroPedidoEmbarcadorECodigoPedidoCliente(string numeroPedidoEmbarcador, string codigoPedidoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && obj.CodigoPedidoCliente == codigoPedidoCliente select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroPedidoEmbarcadorENumeroOrdem(string numeroPedidoEmbarcador, string numeroOrdem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && obj.NumeroOrdem == numeroOrdem && obj.SituacaoPedido != SituacaoPedido.Cancelado && obj.SituacaoPedido != SituacaoPedido.Rejeitado select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroOrdem(string numeroOrdem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroOrdem == numeroOrdem && obj.SituacaoPedido != SituacaoPedido.Cancelado && obj.SituacaoPedido != SituacaoPedido.Rejeitado select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNumeroOrdens(List<string> numeroOrdens)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query
                         where numeroOrdens.Contains(obj.NumeroOrdem) &&
                             obj.SituacaoPedido != SituacaoPedido.Cancelado &&
                             obj.SituacaoPedido != SituacaoPedido.Rejeitado
                         select obj;
            return result.ToList();
        }

        public List<int> BuscarPorAgrupamento(List<string> agrupamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where agrupamentos.Contains(obj.PalletAgrupamento) && obj.DisponivelParaSeparacao == true select obj.Codigo;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorBookingContainer(string numeroBooking, string numeroContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.NumeroBooking == numeroBooking && obj.Container.Numero == numeroContainer && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto);

            query = query.Where(obj => !queryCargaPedido.Where(c => c.Carga.SituacaoCarga != SituacaoCarga.Cancelada && c.Carga.SituacaoCarga != SituacaoCarga.Anulada).Any(p => p.Pedido == obj));

            return query.FirstOrDefault();
        }

        public int BuscarQuantidadeBooking(string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.NumeroBooking == numeroBooking && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto);
            query = query.Where(obj => !queryCargaPedido.Where(c => c.Carga.SituacaoCarga != SituacaoCarga.Cancelada && c.Carga.SituacaoCarga != SituacaoCarga.Anulada).Any(p => p.Pedido == obj));

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorBookingSemContainer(string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(obj => obj.NumeroBooking == numeroBooking && obj.Container == null && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto);
            query = query.Where(obj => !queryCargaPedido.Where(c => c.Carga.SituacaoCarga != SituacaoCarga.Cancelada && c.Carga.SituacaoCarga != SituacaoCarga.Anulada).Any(p => p.Pedido == obj));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorBookingCodigos(string numeroBooking, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroBooking == numeroBooking && codigosPedidos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosSVM(int numero, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.PedidoSVM == true select obj;

            if (codigoPedidoViagemNavio > 0)
                result = result.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (codigoTerminalOrigem > 0)
                result = result.Where(obj => obj.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                result = result.Where(obj => obj.TerminalDestino.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosAgIntegracao(int empresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.SituacaoPedido != SituacaoPedido.Cancelado && obj.AguardandoIntegracao select obj;

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa || obj.Empresa.Matriz.Any(o => o.Codigo == empresa));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorCarga(int codigoCarga)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    (o.Carga.Codigo == codigoCarga) &&
                    (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada) &&
                    (o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                )
                .Select(o => o.Pedido);

            return consultaPedido
                .Distinct()
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorFrotaEDataCarregamentoMenor(string frota, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Veiculos.Any(o => o.NumeroFrota == frota) && obj.DataCarregamentoPedido < data select obj;
            return result.OrderByDescending(o => o.DataCarregamentoPedido).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorFrotaEData(string frota, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Veiculos.Any(o => o.NumeroFrota == frota) && obj.DataCarregamentoPedido.Value.Date == data select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPrimeiroPedidoPorVeiculoESituacao(string placa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Veiculos.Any(o => o.Placa == placa) && obj.SituacaoPedido == situacaoPedido select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorFrotaEntreData(string frota, DateTime dataIni, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Veiculos.Any(o => o.NumeroFrota == frota) && obj.DataCarregamentoPedido >= dataIni && obj.DataCarregamentoPedido < dataFim && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNomeMotoristaEDataCarregamentoMenor(string nomeMotorista, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Motoristas.Any(o => o.Nome == nomeMotorista || o.Apelido == nomeMotorista) && obj.DataCarregamentoPedido < data select obj;
            return result.OrderByDescending(o => o.DataCarregamentoPedido).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNomeMotoristaEData(string nomeMotorista, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Motoristas.Any(o => o.Nome == nomeMotorista || o.Apelido == nomeMotorista) && obj.DataCarregamentoPedido.Value.Date == data select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNomeMotoristaEntreData(string nomeMotorista, DateTime dataIni, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Motoristas.Any(o => o.Nome == nomeMotorista || o.Apelido == nomeMotorista) && obj.DataCarregamentoPedido >= dataIni && obj.DataCarregamentoPedido < dataFim && obj.SituacaoPedido == SituacaoPedido.Aberto select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorProtocolo(int protocoloPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(o => o.Protocolo == protocoloPedido);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorProtocolos(List<int> protocolos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(o => protocolos.Contains(o.Protocolo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCodigos(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> result = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                query = query.Where(o => tmp.Contains(o.Codigo));
                query = query.Fetch(x => x.Filial)
                             .Fetch(x => x.TipoDeCarga)
                             .ThenFetch(x => x.TipoCargaPrincipal)
                             .Fetch(x => x.TipoOperacao)
                             .Fetch(x => x.Recebedor)
                             .ThenFetch(x => x.Localidade)
                             .Fetch(o => o.Remetente)
                             .ThenFetch(o => o.Localidade)
                             .Fetch(x => x.Destinatario)
                             .ThenFetch(x => x.Localidade);
                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.GerarCarregamento.Pedido>> BuscarObjetoPorCodigosAsync(List<int> codigos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.GerarCarregamento.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.GerarCarregamento.Pedido>(); // revisar os fetch

            int take = 1000;
            int start = 0;

            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var result = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                    .Where(o => tmp.Contains(o.Codigo))
                    .Select(o => new Dominio.ObjetosDeValor.Embarcador.Pedido.GerarCarregamento.Pedido
                    {
                        CodigoPedido = o.Codigo,
                        NumeroPedidoEmbarcador = o.NumeroPedidoEmbarcador,
                        CodigoCargaEmbarcador = o.CodigoCargaEmbarcador,
                        DataCarregamentoPedido = o.DataCarregamentoPedido,
                        NumeroPaletes = o.NumeroPaletes,
                        TotalPallets = o.NumeroPaletes + o.NumeroPaletesFracionado,
                        SaldoVolumesRestante = o.SaldoVolumesRestante,
                        ValorTotalPaletes = o.ValorTotalPaletes,
                        Numero = o.Numero,
                        PesoTotal = o.PesoTotal,
                        PesoLiquidoTotal = o.PesoLiquidoTotal,
                        ValorTotalNotasFiscais = o.ValorTotalNotasFiscais,
                        ValorFreteNegociado = o.ValorFreteNegociado,
                        ValorFreteAReceber = o.ValorFreteAReceber,
                        PercentualAliquota = o.PercentualAliquota,
                        BaseCalculoICMS = o.BaseCalculoICMS,
                        ValorICMS = o.ValorICMS,
                        ValorFreteFilialEmissora = o.ValorFreteFilialEmissora,
                        DataInicialColeta = o.DataInicialColeta,
                        DataFinalColeta = o.DataFinalColeta,
                        PedidoDeDevolucao = o.PedidoDeDevolucao,
                        ImpostoNegociado = o.ImpostoNegociado,
                        UsarOutroEnderecoOrigem = o.UsarOutroEnderecoOrigem,
                        IncluirBaseCalculoICMS = o.IncluirBaseCalculoICMS,
                        PedidoTakeOrPay = o.PedidoTakeOrPay,
                        PedidoDemurrage = o.PedidoDemurrage,
                        PedidoDetention = o.PedidoDetention,
                        PedidoDeSVMTerceiro = o.PedidoDeSVMTerceiro,
                        PedidoSVM = o.PedidoSVM,
                        CanalEntrega = o.CanalEntrega != null ? new Dominio.ObjetosDeValor.Embarcador.Pedido.CanalEntrega() { Codigo = o.CanalEntrega.Codigo, LiberarPedidoSemNFeAutomaticamente = o.CanalEntrega.LiberarPedidoSemNFeAutomaticamente } : null,
                        CodigoCanalVenda = o.CanalVenda != null ? o.CanalVenda.Codigo : 0,
                        CodigoCentroResultadoEmbarcador = o.CentroResultadoEmbarcador != null ? o.CentroResultadoEmbarcador.Codigo : 0,
                        CodigoContaContabil = o.ContaContabil != null ? o.ContaContabil.Codigo : 0,
                        QuantidadeNotasFiscais = o.NotasFiscais.Count,
                        Origem = new Dominio.ObjetosDeValor.Localidade
                        {
                            Codigo = o.Origem.Codigo,
                            Descricao = o.Origem.Descricao,
                            SiglaUF = o.Origem.Estado.Sigla,
                        },
                        ColetaEmProdutorRural = o.ColetaEmProdutorRural,
                        PedidoDePreCarga = o.PedidoDePreCarga,
                        Observacao = o.Observacao,
                        OrdemColetaProgramada = o.OrdemColetaProgramada,
                        OrdemEntregaProgramada = o.OrdemEntregaProgramada,
                        PalletSaldoRestante = o.PalletSaldoRestante,
                        PedidoRedespachoTotalmenteCarregado = o.PedidoRedespachoTotalmenteCarregado,
                        PedidoIntegradoEmbarcador = o.PedidoIntegradoEmbarcador,
                        TipoTomador = o.TipoTomador,
                        PesoSaldoRestante = o.PesoSaldoRestante,
                        PercentualInclusaoBC = o.PercentualInclusaoBC,
                        PedidoTotalmenteCarregado = o.PedidoTotalmenteCarregado,
                        TipoPagamento = o.TipoPagamento,
                        QtVolumes = o.QtVolumes,
                        PrevisaoEntrega = o.PrevisaoEntrega,
                        ReentregaSolicitada = o.ReentregaSolicitada,
                        UsarTipoPagamentoNF = o.UsarTipoPagamentoNF,
                        UsarTipoTomadorPedido = o.UsarTipoTomadorPedido,
                        PontoPartida = o.PontoPartida != null ? new Dominio.ObjetosDeValor.Cliente()
                        {
                            CPFCNPJ = o.PontoPartida.CPF_CNPJ.ToString(),
                            Codigo = o.PontoPartida.CPF_CNPJ,
                        } : null,
                        Destino = o.Destino != null ? new Dominio.ObjetosDeValor.Localidade
                        {
                            Codigo = o.Destino.Codigo,
                            Descricao = o.Destino.Descricao,
                            SiglaUF = o.Destino.Estado.Sigla,
                        } : null,
                        ElementoPEP = o.ElementoPEP,
                        Expedidor = o.Expedidor != null ? new Dominio.ObjetosDeValor.Cliente()
                        {
                            CPFCNPJ = o.Expedidor.CPF_CNPJ.ToString(),
                            Codigo = o.Expedidor.CPF_CNPJ,
                        } : null,
                        Recebedor = o.Recebedor != null ? new Dominio.ObjetosDeValor.Cliente()
                        {
                            CPFCNPJ = o.Recebedor.CPF_CNPJ.ToString(),
                            Codigo = o.Recebedor.CPF_CNPJ,
                            Localidade = o.Recebedor.Localidade.Codigo,
                        } : null,
                        Remetente = o.Remetente != null ? new Dominio.ObjetosDeValor.Cliente()
                        {
                            CPFCNPJ = o.Remetente.CPF_CNPJ.ToString(),
                            Codigo = o.Remetente.CPF_CNPJ,
                            Tipo = o.Remetente.Tipo,
                        } : null,
                        Destinatario = o.Destinatario != null ? new Dominio.ObjetosDeValor.Cliente()
                        {
                            CPFCNPJ = o.Destinatario.CPF_CNPJ.ToString(),
                            Codigo = o.Destinatario.CPF_CNPJ,
                            CodigoRegiao = o.Destinatario.Localidade.Regiao != null ? o.Destinatario.Localidade.Regiao.Codigo : 0,
                            Tipo = o.Destinatario.Tipo,
                        } : null,
                        Tomador = o.Tomador != null ? new Dominio.ObjetosDeValor.Cliente()
                        {
                            CPFCNPJ = o.Tomador.CPF_CNPJ.ToString(),
                            Codigo = o.Tomador.CPF_CNPJ,
                        } : null,
                        TipoCarga = o.TipoCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga()
                        {
                            Codigo = o.TipoCarga.Codigo,
                            Descricao = o.TipoCarga.Descricao,
                        } : null,
                        TipoDeCarga = o.TipoDeCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoDeCarga()
                        {
                            Codigo = o.TipoCarga.Codigo,
                            ModalProposta = o.TipoDeCarga.ModalProposta,
                        } : null,
                        TipoOperacao = o.TipoOperacao != null ? new Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacao()
                        {
                            Codigo = o.TipoOperacao.Codigo,
                            Descricao = o.TipoOperacao.Descricao,
                            NaoExigeVeiculoParaEmissao = o.TipoOperacao.NaoExigeVeiculoParaEmissao,
                            OperacaoDestinadaCTeComplementar = o.TipoOperacao.OperacaoDestinadaCTeComplementar,
                            IgnorarRateioConfiguradoPorto = o.TipoOperacao.ConfiguracaoCarga.IgnorarRateioConfiguradoPorto,
                        } : null,
                        PortoDestinoFormaEmissaoSVM = o.PortoDestino != null ? o.PortoDestino.FormaEmissaoSVM : FormaEmissaoSVM.Nenhum,
                        PesoTotalPaletes = o.PesoTotalPaletes,
                        CubagemTotal = o.CubagemTotal,
                        CodigoFilial = o.Filial != null ? o.Filial.Codigo : 0,
                        CodigoEmpresa = o.Empresa != null ? o.Empresa.Codigo : 0,
                        CodigoRotaFrete = o.RotaFrete != null ? o.RotaFrete.Codigo : 0,
                    })
            .ToListAsync(CancellationToken);

                pedidos.AddRange(result);
                start += take;
            }
            return pedidos;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>> BuscarPorCodigosAsync(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> result = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>(); // revisar os fetch
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                query = query.Where(o => tmp.Contains(o.Codigo));
                query = query.Fetch(x => x.Filial)
                             .Fetch(x => x.TipoDeCarga)
                             .ThenFetch(x => x.TipoCargaPrincipal)
                             .Fetch(x => x.TipoOperacao)
                             .Fetch(x => x.Recebedor)
                             .ThenFetch(x => x.Localidade)
                             .Fetch(o => o.Remetente)
                             .ThenFetch(o => o.Localidade)
                             .Fetch(x => x.Destinatario)
                             .ThenFetch(x => x.Localidade);

                result.AddRange(await query.ToListAsync(CancellationToken));
                start += take;
            }
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCodigosComOutroEndereco(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> result = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                query = query.Where(o => tmp.Contains(o.Codigo));
                query = query.Fetch(x => x.Filial)
                             .Fetch(x => x.TipoDeCarga)
                             .ThenFetch(x => x.TipoCargaPrincipal)
                             .Fetch(x => x.TipoOperacao)
                             .Fetch(x => x.Expedidor)
                             .Fetch(x => x.Recebedor)
                             .ThenFetch(x => x.Localidade)
                             .Fetch(o => o.Remetente)
                             .ThenFetch(o => o.Localidade)
                             .Fetch(x => x.Destinatario)
                             .ThenFetch(x => x.Localidade)
                             .Fetch(x => x.EnderecoDestino)
                             .ThenFetch(x => x.ClienteOutroEndereco);
                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>> BuscarPorCodigosParaMontagemCargaAsync(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(o => codigos.Contains(o.Codigo));
            query = query.Fetch(x => x.ModeloVeicularCarga)
                         .Fetch(x => x.TipoDeCarga)
                         .Fetch(x => x.TipoOperacao)
                         .Fetch(x => x.Remetente)
                         .Fetch(x => x.Destinatario)
                         .Fetch(x => x.Tomador);

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCodigosSemCarregamento(List<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> result = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                var queryCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>().Where(c => c.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado);
                query = query.Where(o => tmp.Contains(o.Codigo) &&
                                         (o.PesoTotal > queryCarregamento.Where(c => c.Pedido.Codigo == o.Codigo).Sum(c => c.Peso) ||
                                         !queryCarregamento.Any(p => p.Pedido.Codigo == o.Codigo)));

                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public List<int> BuscarCodigosPorCodigosSemCarregamento(List<int> codigos, int codigoSessaoRoteirizador)
        {
            string sqlPermitePedidoBloqueado = "SELECT CMC_PERMITIR_GERAR_CARREGAMENTO_PEDIDO_BLOQUEADO FROM T_CONFIGURACAO_MONTAGEM_CARGA";

            var query1 = this.SessionNHiBernate.CreateSQLQuery(sqlPermitePedidoBloqueado);
            bool? pode = query1.UniqueResult<bool?>();

            bool permiteGerarPedidosBloqueados = (pode.HasValue && pode.Value);

            var sqlQuery = @"
SELECT DISTINCT PED.PED_CODIGO
  FROM T_PEDIDO PED
 WHERE PED.PED_CODIGO in ( :codigos ) " + (permiteGerarPedidosBloqueados ? " AND 0 = :bloqueado " : "AND COALESCE(PED.PED_BLOQUEADO,0) = :bloqueado ") + @"
   AND PED.PED_SITUACAO <> :cancelado
   AND (
        PED.PED_AG_INTEGRACAO = 1
        OR PED.PED_PESO_TOTAL_CARGA > COALESCE((SELECT SUM(CPE.CRP_PESO)
								 FROM T_CARREGAMENTO CAR 
									, T_CARREGAMENTO_PEDIDO CPE
								WHERE CAR.CRG_CODIGO = CPE.CRG_CODIGO
								  AND CPE.PED_CODIGO = PED.PED_CODIGO
								  AND CAR.CRG_SITUACAO <> :situacao ), 0) + 0.05)
    AND PED.PED_CODIGO not in (select PED_CODIGO from T_CARREGAMENTO carregamento
inner join T_CARREGAMENTO_PEDIDO carregamentoPedido on carregamentoPedido.CRG_CODIGO = carregamento.CRG_CODIGO
where carregamento.SRO_CODIGO = :codigoSessaoRoteirizador);";

            List<int> result = new List<int>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameterList("codigos", tmp);
                query.SetParameter("bloqueado", false);
                query.SetParameter("cancelado", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);
                query.SetParameter("situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);
                query.SetParameter("codigoSessaoRoteirizador", codigoSessaoRoteirizador);
                result.AddRange(query.List<int>());

                start += take;
            }
            return result;
        }

        public async Task<List<int>> BuscarCodigosPorCodigosSemCarregamentoAsync(List<int> codigos, int codigoSessaoRoteirizador)
        {
            string sqlPermitePedidoBloqueado = "SELECT CMC_PERMITIR_GERAR_CARREGAMENTO_PEDIDO_BLOQUEADO FROM T_CONFIGURACAO_MONTAGEM_CARGA";

            var query1 = this.SessionNHiBernate.CreateSQLQuery(sqlPermitePedidoBloqueado);
            bool? pode = await query1.UniqueResultAsync<bool?>(CancellationToken);

            bool permiteGerarPedidosBloqueados = (pode.HasValue && pode.Value);

            var sqlQuery = @"
SELECT DISTINCT PED.PED_CODIGO
  FROM T_PEDIDO PED
 WHERE PED.PED_CODIGO in ( :codigos ) " + (permiteGerarPedidosBloqueados ? " AND 0 = :bloqueado " : "AND COALESCE(PED.PED_BLOQUEADO,0) = :bloqueado ") + @"
   AND PED.PED_SITUACAO <> :cancelado
   AND (
        PED.PED_AG_INTEGRACAO = 1
        OR PED.PED_PESO_TOTAL_CARGA > COALESCE((SELECT SUM(CPE.CRP_PESO)
								 FROM T_CARREGAMENTO CAR 
									, T_CARREGAMENTO_PEDIDO CPE
								WHERE CAR.CRG_CODIGO = CPE.CRG_CODIGO
								  AND CPE.PED_CODIGO = PED.PED_CODIGO
								  AND CAR.CRG_SITUACAO <> :situacao ), 0) + 0.05)
    AND PED.PED_CODIGO not in (select PED_CODIGO from T_CARREGAMENTO carregamento
inner join T_CARREGAMENTO_PEDIDO carregamentoPedido on carregamentoPedido.CRG_CODIGO = carregamento.CRG_CODIGO
where carregamento.SRO_CODIGO = :codigoSessaoRoteirizador);";

            List<int> result = new List<int>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameterList("codigos", tmp);
                query.SetParameter("bloqueado", false);
                query.SetParameter("cancelado", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);
                query.SetParameter("situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);
                query.SetParameter("codigoSessaoRoteirizador", codigoSessaoRoteirizador);
                result.AddRange(await query.ListAsync<int>(CancellationToken));

                start += take;
            }
            return result.ToList();
        }

        public List<int> BuscarPorCodigosComCarregamento(List<int> codigos)
        {
            var sqlQuery = @"
SELECT DISTINCT CPE.PED_CODIGO
  FROM T_CARREGAMENTO_PEDIDO CPE
	 , T_CARREGAMENTO CAR
 WHERE CPE.CRG_CODIGO = CAR.CRG_CODIGO
   AND CAR.CRG_SITUACAO <> :situacao 
   AND CPE.PED_CODIGO in ( :codigos ); ";

            List<int> result = new List<int>();
            int take = 1000;
            int start = 0;
            while (start < codigos?.Count)
            {
                List<int> tmp = codigos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameter("situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado);
                query.SetParameterList("codigos", tmp);

                result.AddRange(query.List<int>());

                start += take;
            }
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCodigosSemCarregamentoCompleto(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(o => codigos.Contains(o.Codigo) && o.PesoSaldoRestante > 0);
            return query.ToList();
        }

        public bool ContemPedidoMesmoNumero(int numero)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(o => o.Numero == numero);

            return query.Any();
        }

        public bool ContemPedidoMesmoNumeroCarga(string numeroCarga, DateTime dataCadastro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(o => o.CodigoCargaEmbarcador == numeroCarga && o.SituacaoPedido != SituacaoPedido.Cancelado && o.DataCadastro.Value.Date == dataCadastro.Date);

            return query.Any();
        }

        public bool ContemPedidoMesmaOrigemDestinoVeiculoData(int cidadeOrigem, int cidadeDestino, int codigoVeiculo, DateTime? dataColeta, int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(o => o.Origem.Codigo == cidadeOrigem && o.Destino.Codigo == cidadeDestino && o.Codigo != codigoPedido);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculos.Any(v => v.Codigo == codigoVeiculo) || o.VeiculoTracao.Codigo == codigoVeiculo);

            if (dataColeta.HasValue)
                query = query.Where(o => o.DataCarregamentoPedido.Value.Date == dataColeta.Value.Date);

            return query.Any();
        }

        public int BuscarProximoNumero()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            return (query.Max(o => (int?)o.Numero) ?? 0) + 1;
        }

        public List<string> BuscarPlacas(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(o => o.Codigo == codigoPedido);

            return query.Select(o => o.Veiculos.Select(v => v.Placa)).SelectMany(o => o).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoVeiculo> BuscarPlacasPorPedidos(List<int> codigosPedidos)
        {
            string sql = @"select 
                             PED_CODIGO as CodigoPedido,
                             Veiculo.VEI_PLACA as Placa
                             FROM T_PEDIDO_VEICULO PedidoVeiculo
                             JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = PedidoVeiculo.VEI_CODIGO
                             WHERE PED_CODIGO IN (:codigosPedidos)";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameterList("codigosPedidos", codigosPedidos);

            return query
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoVeiculo)))
                .List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoVeiculo>()
                .ToList();
        }

        public List<string> BuscarMotoristas(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(o => o.Codigo == codigoPedido);

            return query.Select(o => o.Motoristas.Select(v => v.Nome)).SelectMany(o => o).ToList();
        }

        public List<string> BuscarGestoresMotoristas(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(o => o.Codigo == codigoPedido);

            return query.Select(o => o.Motoristas.Select(v => v.Gestor.Nome)).SelectMany(o => o).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoMotorista> BuscarMotoristasPorPedidos(List<int> codigosPedidos)
        {
            string sql = @"select 
                             PED_CODIGO as CodigoPedido,
                             Motorista.FUN_NOME as Motorista
                             FROM T_PEDIDO_MOTORISTA PedidoMotorista
                             JOIN T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = PedidoMotorista.FUN_CODIGO
                             WHERE PED_CODIGO IN (:codigosPedidos)";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameterList("codigosPedidos", codigosPedidos);

            return query
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoMotorista)))
                .List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoMotorista>()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNumero(int codigoEmpresa, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNumeroControleSemCodigoXMLNotaFiscal(string numeroControle, int codigoXMLNotaFiscal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from obj in query
                         where
                             obj.NumeroControle == numeroControle
                             && obj.SituacaoPedido != SituacaoPedido.Cancelado
                             && obj.SituacaoPedido != SituacaoPedido.Finalizado
                             && !obj.NotasFiscais.Any(x => x.Codigo == codigoXMLNotaFiscal)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorCaraSemNotasFiscais(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Pedido.NotasFiscais.Count <= 0 select obj.Pedido;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoSemXMLNotasFiscaisPorCarga(int codigoCarga)
        {
            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var subResult = from obj in subQuery where obj.CargaPedido.Carga.Codigo == codigoCarga select obj.CargaPedido.Codigo;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && !subResult.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosNaoIntegrados(int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where !obj.PedidoIntegradoEmbarcador && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto select obj;
            return result.OrderByDescending(obj => obj.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ObterProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            int? retorno = query.Max(o => (int?)o.NumeroSequenciaPedido);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        public int ObterProximoCodigo(Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            if (filial != null)
                query = query.Where(obj => obj.Filial.Codigo == filial.Codigo);
            else
                query = query.Where(obj => obj.Filial == null);

            int? retorno = query.Max(o => (int?)o.NumeroSequenciaPedido);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        public int ObterUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNumeroBooking(string numeroBooking)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                query = query.Where(obj => obj.NumeroBooking == numeroBooking);

            return query.ToList();
        }

        public string BuscarPrimeiroPorNumeroBooking(int codigoPedido)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.Codigo == codigoPedido);

            return consultaPedido.Select(c => c.NumeroBooking).FirstOrDefault();
        }

        public string BuscarPorNumeroControle(int codigoPedido)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.Codigo == codigoPedido);

            return consultaPedido.Select(c => c.NumeroControle).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroOSMae(string numeroOSMae)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                .Where(o => o.NumeroOSMae == numeroOSMae && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return consultaPedido.Select(c => c.Pedido).FirstOrDefault();
        }

        public string BuscarNumeroOSMae(int codigoPedido)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return consultaPedido.Select(c => c.NumeroOSMae).FirstOrDefault();
        }

        public async Task<string> BuscarNumeroOSMaeAsync(int codigoPedido)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.Pedido.SituacaoPedido != SituacaoPedido.Cancelado);

            return await consultaPedido.Select(c => c.NumeroOSMae).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroEmbarcador(string numeroPedidoEmbarcador)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && o.SituacaoPedido != SituacaoPedido.Cancelado);

            return consultaPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroEmbarcador(string numeroPedidoEmbarcador, int codigoFilial, bool pedidoAdicinadoManualmente)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && o.SituacaoPedido != SituacaoPedido.Cancelado && o.Filial.Codigo == codigoFilial);

            if (pedidoAdicinadoManualmente)
                consultaPedido = consultaPedido.Where(obj => obj.AdicionadaManualmente);

            return consultaPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroEmbarcador(string numeroPedidoEmbarcador, int filial, string cnpjEmpresa)
        {
            return BuscarPorNumeroEmbarcador(numeroPedidoEmbarcador, filial, cnpjEmpresa, pedidoDePreCarga: null, ignorarPedidosInseridosManualmente: false);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorNumeroDevolucao(string numeroPedido, int filial)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
           .Where(o =>
               o.NumeroPedidoDevolucao == numeroPedido &&
               o.SituacaoPedido != SituacaoPedido.Cancelado
           );

            if (filial > 0)
                consultaPedido = consultaPedido.Where(obj => obj.Filial.Codigo == filial);

            return consultaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoDeDevolucao(string numeroPedidoDevolucao, int filial)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
           .Where(o =>
               o.NumeroPedidoEmbarcador == numeroPedidoDevolucao &&
               o.SituacaoPedido != SituacaoPedido.Cancelado
           );

            if (filial > 0)
                consultaPedido = consultaPedido.Where(obj => obj.Filial.Codigo == filial);

            return consultaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPrincipalPeloPedidoDevolucao(int codigoPedidoDevolucao)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
           .Where(o =>
               o.PedidoDevolucao.Codigo == codigoPedidoDevolucao &&
               o.SituacaoPedido != SituacaoPedido.Cancelado
           );

            return consultaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidoPrincipalPeloPedidoDevolucaoAsync(int codigoPedidoDevolucao)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
           .Where(o =>
               o.PedidoDevolucao.Codigo == codigoPedidoDevolucao &&
               o.SituacaoPedido != SituacaoPedido.Cancelado
           );

            return await consultaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroEmbarcador(string numeroPedidoEmbarcador, int filial, string cnpjEmpresa, bool? pedidoDePreCarga, bool ignorarPedidosInseridosManualmente = false)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o =>
                    o.NumeroPedidoEmbarcador == numeroPedidoEmbarcador &&
                    o.SituacaoPedido != SituacaoPedido.Cancelado &&
                    (
                        o.TipoOperacao == null ||
                        o.TipoOperacao.UtilizarExpedidorComoTransportador == false ||
                        (o.TipoOperacao.UtilizarExpedidorComoTransportador && o.TipoOperacao.OperacaoDeRedespacho)
                    )
                );

            if (pedidoDePreCarga.HasValue)
                consultaPedido = consultaPedido.Where(obj => obj.PedidoDePreCarga == pedidoDePreCarga.Value);

            if (filial > 0)
                consultaPedido = consultaPedido.Where(obj => obj.Filial.Codigo == filial);

            if (!string.IsNullOrWhiteSpace(cnpjEmpresa))
                consultaPedido = consultaPedido.Where(obj => obj.Empresa.CNPJ == cnpjEmpresa);

            if (ignorarPedidosInseridosManualmente)
                consultaPedido = consultaPedido.Where(obj => obj.AdicionadaManualmente == false);

            return consultaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorNumeroPedidoEmbarcadorSemRegra(string numeroPedidoEmbarcador, int filial, string cnpjEmpresa, bool? pedidoDePreCarga)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o =>
                    o.NumeroPedidoEmbarcadorSemRegra == numeroPedidoEmbarcador &&
                    o.Protocolo > 0 &&
                    o.SituacaoPedido != SituacaoPedido.Cancelado &&
                    (
                        o.TipoOperacao == null ||
                        o.TipoOperacao.UtilizarExpedidorComoTransportador == false ||
                        (o.TipoOperacao.UtilizarExpedidorComoTransportador && o.TipoOperacao.OperacaoDeRedespacho)
                    )
                );

            if (pedidoDePreCarga.HasValue)
                consultaPedido = consultaPedido.Where(obj => obj.PedidoDePreCarga == pedidoDePreCarga.Value);

            if (filial > 0)
                consultaPedido = consultaPedido.Where(obj => obj.Filial.Codigo == filial);

            if (!string.IsNullOrWhiteSpace(cnpjEmpresa))
                consultaPedido = consultaPedido.Where(obj => obj.Empresa.CNPJ == cnpjEmpresa);

            return consultaPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorNumeroOrdem(string NumeroOrdem)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.NumeroOrdem == NumeroOrdem && o.SituacaoPedido != SituacaoPedido.Cancelado);

            return consultaPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido buscarPorNumeroEmbarcadorExpedidor(string numeroPedidoEmbarcador, int codigoFilial, string cnpjExpedidor, bool? pedidoDePreCarga)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o =>
                    o.NumeroPedidoEmbarcador == numeroPedidoEmbarcador &&
                    o.SituacaoPedido != SituacaoPedido.Cancelado &&
                    o.TipoOperacao.UtilizarExpedidorComoTransportador &&
                    !o.TipoOperacao.OperacaoDeRedespacho
                );

            if (pedidoDePreCarga.HasValue)
                consultaPedido = consultaPedido.Where(obj => obj.PedidoDePreCarga == pedidoDePreCarga.Value);

            if (codigoFilial > 0)
                consultaPedido = consultaPedido.Where(o => o.Filial.Codigo == codigoFilial);

            if (!string.IsNullOrWhiteSpace(cnpjExpedidor))
                consultaPedido = consultaPedido.Where(o => o.Expedidor.CPF_CNPJ == double.Parse(cnpjExpedidor));

            return consultaPedido
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorPedido(string numeroPedidoEmbarcador, string codigoFilialEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from p in query
                         where
                          p.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador
                            && p.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
                            && (
                                p.Pedido.TipoOperacao == null
                                || p.Pedido.TipoOperacao.UtilizarExpedidorComoTransportador == false
                                || (
                                    p.Pedido.TipoOperacao.UtilizarExpedidorComoTransportador
                                    && p.Pedido.TipoOperacao.OperacaoDeRedespacho
                                )
                            )
                         select p;

            if (!string.IsNullOrWhiteSpace(codigoFilialEmbarcador))
                result = result.Where(obj => obj.Pedido.Filial.CodigoFilialEmbarcador == codigoFilialEmbarcador || obj.Pedido.Filial.OutrosCodigosIntegracao.Contains(codigoFilialEmbarcador));

            return result
                .Select(o => o.Carga)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorNumeroOrdemPedido(string numeroOrdemPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(p => p.Pedido.NumeroOrdem == numeroOrdemPedido
                         && p.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);

            return query.OrderByDescending(o => o.Codigo).Select(o => o.Carga).FirstOrDefault();

        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaDePreCargaPorPedido(string numeroPedidoEmbarcador, string codigoFilialEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            var result = from p in query
                         where
                          p.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador
                         select p;

            if (!string.IsNullOrWhiteSpace(codigoFilialEmbarcador))
                result = result.Where(obj => obj.Carga.CargaDePreCarga == true);

            return result
                .Select(o => o.Carga)
                .FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotasPortalCliente> ConsultarPedidoNotasCliente(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = ObterSQLPedidoNotasClientes(codigoPedido, parametrosConsulta);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotasPortalCliente)));
            return query.List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotasPortalCliente>();
        }

        public int ContarBuscarPedidosSVM(int numero, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.PedidoSVM == true select obj;

            if (codigoPedidoViagemNavio > 0)
                result = result.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (codigoTerminalOrigem > 0)
                result = result.Where(obj => obj.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                result = result.Where(obj => obj.TerminalDestino.Codigo == codigoTerminalDestino);

            return result.Count();
        }

        public int ContarBuscarPendentesPorRemetente(double cnpjRemetete, double cnpjDestinatario, int grupoPessoa, List<string> pedidos)
        {
            var query = ConsultaPendentesPorRemetente(cnpjRemetete, cnpjDestinatario, grupoPessoa, pedidos);

            return query.Count();
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa)
        {
            var consultaPlanejamentoPedido = Consultar(filtrosPesquisa);

            return consultaPlanejamentoPedido.WithOptions(o => { o.SetTimeout(300); }).CountAsync(CancellationToken);
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string nomeRemetente, double cnpjRemetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(nomeRemetente))
                result = result.Where(o => o.Remetente.Nome.Contains(nomeRemetente));

            if (cnpjRemetente > 0)
                result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjRemetente);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicialColeta.Value.Date >= dataInicial.Date && o.DataInicialColeta.Value.Date < dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFinalColeta.Value.Date >= dataFinal.Date && o.DataFinalColeta.Value.Date < dataFinal.Date);

            return result.Count();
        }

        public int ContarConsultaAtendimentoPedidoChat(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedido filtrosPesquisa)
        {
            var query = ConsultarAtendimentoPedidoComChat(filtrosPesquisa);

            return query.Count();
        }

        public int ContarConsultaAtendimentoPedidoCliente(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedidoCliente filtrosPesquisa)
        {
            var query = ConsultarAtendimentoPedidoCliente(filtrosPesquisa);

            return query.Count();
        }

        public int ContarConsultaPedidoNotasCliente(int codigoPedido)
        {
            var query = ObterSQLPedidoNotasClientes(codigoPedido, null);
            int total = query.UniqueResult<int>();

            return total;
        }

        public int ContarConsultaPedidosCliente(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente filtrosPesquisa)
        {
            var query = ObterSQLPedidosClientes(filtrosPesquisa, null);
            int total = query.UniqueResult<int>();
            return total;
        }

        public int ContarPedidosAgIntegracao(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.SituacaoPedido != SituacaoPedido.Cancelado && obj.AguardandoIntegracao select obj;

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa || obj.Empresa.Matriz.Any(o => o.Codigo == empresa));

            return result.Count();
        }

        public int ContarPedidosEmContacaoParaExcluir(int quantidadeDiasCriacao)
        {
            DateTime dataCriacao = DateTime.Now.AddDays(-quantidadeDiasCriacao).Date;
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query
                         where obj.SituacaoPedido == SituacaoPedido.EmCotacao && obj.DataCriacao < dataCriacao
                         select obj;
            return result.Count();
        }

        public int ContarPedidosEmOutrasCargas(int codigoCarga, List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo != codigoCarga && codigosPedidos.Contains(obj.Pedido.Codigo) select obj;
            return result.Count();
        }

        public int ContarPedidosNaoIntegrados()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where !obj.PedidoIntegradoEmbarcador && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto select obj;
            return result.Count();
        }

        public Task<int> ContarConsultaPedidosGestaoPedidoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoPedido.FiltroPesquisaGestaoPedido filtrosPesquisa)
        {
            var sql = QueryPedidosGestaoPedidos(filtrosPesquisa, true, null);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido>> ObterSelecionadosConsultaPedidosGestaoPedidoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoPedido.FiltroPesquisaGestaoPedido filtrosPesquisa, bool selecionarTodos = false, List<int> codigosPedidos = null)
        {
            var sql = QueryPedidosGestaoPedidos(filtrosPesquisa, false, null, selecionarTodos, codigosPedidos);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido)));

            return consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido>(CancellationToken);
        }

        public Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido>> ConsultaPedidosGestaoPedidoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoPedido.FiltroPesquisaGestaoPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sql = QueryPedidosGestaoPedidos(filtrosPesquisa, false, parametrosConsulta);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido)));

            return consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoPedido.PedidoGestaoPedido>(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarAtendimentoPedidoCliente(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedidoCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = ConsultarAtendimentoPedidoCliente(filtrosPesquisa);
            return ObterLista(query, parametrosConsulta);
        }

        public void DeletaPedidoEProdutosPorCodigoPedido(int codigoPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                    this.ExecuteDeletaPedidoEProdutosPorCodigoPedido(codigoPedido);
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        this.ExecuteDeletaPedidoEProdutosPorCodigoPedido(codigoPedido);

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorPedido(int codigoPedido)
        {
            var queryCtes = ObterSQLCTesPorPedido(codigoPedido);
            IList<int?> codigosCTes = queryCtes.List<int?>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query
                         where codigosCTes.Contains(obj.Codigo)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarPorPedidoViagemNavio(int codigoPedidoViagemNavio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> ConsultarPedidoTransbordoPorPedidoViagemNavio(int codigoPedidoViagemNavio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            var result = from obj in query where obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio select obj;

            return result.ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPortalCliente> ConsultarPedidosCliente(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = ObterSQLPedidosClientes(filtrosPesquisa, parametrosConsulta);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPortalCliente)));
            return query.List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPortalCliente>();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido ValidarViagemContainer(int codigoViagem, int codigoContainer, int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.PedidoViagemNavio.Codigo == codigoViagem && obj.Container.Codigo == codigoContainer && obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado select obj;
            if (codigoPedido > 0)
                result = result.Where(obj => obj.Codigo != codigoPedido);

            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>> ConsultarDadosPedidosPorCodigosAsync(List<int> codigosPedidos)
        {
            int limiteRegistros = 1000;
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>();

            for (int registroInicial = 0; registroInicial < codigosPedidos.Count; registroInicial += limiteRegistros)
            {
                List<int> codigosPedidosPaginados = codigosPedidos.Skip(registroInicial).Take(limiteRegistros).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                    .Where(pedido => codigosPedidosPaginados.Contains(pedido.Codigo));

                pedidos.AddRange(await ObterDadosPedidosAsync(consultaPedido));
            }

            return pedidos;
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>> ConsultarDadosPedidosAsync(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> consultaPedido = Consultar(filtrosPesquisa);

            if (filtrosPesquisa.OrdernarPorPrioridade)
            {
                consultaPedido = consultaPedido.OrderByDescending(pedido => pedido.Remetente.GrupoPessoas.TornarPedidosPrioritarios);
                consultaPedido = consultaPedido.OrderByDescending(pedido => pedido.GrupoPessoas.TornarPedidosPrioritarios);
            }
            else if (filtrosPesquisa.Ordenacao.HasValue && filtrosPesquisa.Ordenacao.Value != OrdenacaoFiltroPesquisaPedido.Padrao)
            {
                if (filtrosPesquisa.Ordenacao.Value == OrdenacaoFiltroPesquisaPedido.Remetente)
                    consultaPedido = consultaPedido.OrderByDescending(pedido => pedido.Remetente.Nome);
                else
                    consultaPedido = consultaPedido.OrderByDescending(pedido => pedido.Destinatario.Nome);
            }
            else
                consultaPedido = consultaPedido.OrderBy($"{parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "ascending" : "descending")}");

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaPedido = consultaPedido.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

            return await ObterDadosPedidosAsync(consultaPedido);
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>> BuscarSugestaoPedidoPorLocalidadeDestinoAsync(int filial, int tipoOperacao, int empresa, DateTime dataCarregamentoPedido, DateTime dataFinalCarregamento)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido =>
                    !pedido.GerarAutomaticamenteCargaDoPedido &&
                    !pedido.PedidoTotalmenteCarregado &&
                    (pedido.DataCarregamentoPedido >= dataCarregamentoPedido || pedido.DataCarregamentoPedido == null)
                );

            if (empresa > 0)
                consultaPedido = consultaPedido.Where(pedido => pedido.Empresa.Codigo == empresa);

            if (filial > 0)
                consultaPedido = consultaPedido.Where(pedido => pedido.Filial.Codigo == filial);

            if (tipoOperacao > 0)
                consultaPedido = consultaPedido.Where(pedido => pedido.TipoOperacao.Codigo == tipoOperacao);

            if (dataFinalCarregamento != DateTime.MinValue)
                consultaPedido = consultaPedido.Where(pedido => pedido.DataCarregamentoPedido.Value.Date < dataFinalCarregamento.Date);

            return await ObterDadosPedidosAsync(consultaPedido);
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>> BuscarDadosPedidosPorCargaAsync(int codigoCarga)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => cargaPedido.Carga.Codigo == codigoCarga);

            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido => consultaCargaPedido.Any(cargaPedido => cargaPedido.Pedido.Codigo == pedido.Codigo));

            return await ObterDadosPedidosAsync(consultaPedido);
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido>> BuscarDadosPedidosPorSeparacaoPedido(int codigoSeparacaoPedido)
        {
            var consultaSeperacaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido>()
                .Where(separacaoPedido => separacaoPedido.SeparacaoPedido.Codigo == codigoSeparacaoPedido);

            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido => consultaSeperacaoPedido.Any(separacaoPedido => separacaoPedido.Pedido.Codigo == pedido.Codigo));

            return await ObterDadosPedidosAsync(consultaPedido);
        }

        public async Task<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> BuscarDadosPedidoPorCodigoAsync(int codigo)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido => pedido.Codigo == codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> pedidos = await ObterDadosPedidosAsync(consultaPedido);

            return pedidos.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPlanejamentoPedido = Consultar(filtrosPesquisa);

            if (filtrosPesquisa.OrdernarPorPrioridade)
            {
                consultaPlanejamentoPedido = consultaPlanejamentoPedido.OrderByDescending(v => v.Remetente.GrupoPessoas.TornarPedidosPrioritarios);
                consultaPlanejamentoPedido = consultaPlanejamentoPedido.OrderByDescending(v => v.GrupoPessoas.TornarPedidosPrioritarios);
            }
            else if (filtrosPesquisa.Ordenacao.HasValue && filtrosPesquisa.Ordenacao.Value != OrdenacaoFiltroPesquisaPedido.Padrao)
            {
                if (filtrosPesquisa.Ordenacao.Value == OrdenacaoFiltroPesquisaPedido.Remetente)
                    consultaPlanejamentoPedido = consultaPlanejamentoPedido.OrderByDescending(v => v.Remetente.Nome);
                else
                    consultaPlanejamentoPedido = consultaPlanejamentoPedido.OrderByDescending(v => v.Destinatario.Nome);
            }
            else
                consultaPlanejamentoPedido = consultaPlanejamentoPedido.OrderBy($"{parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "ascending" : "descending")}");

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaPlanejamentoPedido = consultaPlanejamentoPedido.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

            var query1 = await consultaPlanejamentoPedido
                .Fetch(o => o.Remetente).ThenFetch(o => o.Localidade).ThenFetch(o => o.Pais)
                .Fetch(o => o.Destinatario).ThenFetch(o => o.Localidade).ThenFetch(o => o.Pais)
                .Fetch(o => o.GrupoPessoas)
                .Fetch(o => o.RotaFrete)
                .Fetch(o => o.Origem)
                .Fetch(o => o.Destino)
                .Fetch(o => o.Recebedor).ThenFetch(o => o.Localidade).ThenFetch(o => o.Pais)
                .Fetch(o => o.Expedidor).ThenFetch(o => o.Localidade).ThenFetch(o => o.Pais)
                .WithOptions(o => { o.SetTimeout(300); })
                .ToListAsync(CancellationToken);

            var query2 = await consultaPlanejamentoPedido
                .Fetch(o => o.Filial)
                .Fetch(o => o.TipoOperacao).ThenFetch(o => o.ConfiguracaoMontagemCarga)
                .Fetch(o => o.TipoOperacao).ThenFetch(o => o.ConfiguracaoCarga)
                .Fetch(o => o.TipoOperacao).ThenFetch(o => o.ConfiguracaoMobile)
                .Fetch(o => o.TipoCarga)
                .Fetch(o => o.TipoDeCarga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.EnderecoDestino).ThenFetch(o => o.Localidade).ThenFetch(o => o.Pais)
                .Fetch(o => o.EnderecoDestino).ThenFetch(o => o.ClienteOutroEndereco)
                .WithOptions(o => { o.SetTimeout(300); })
                .ToListAsync(CancellationToken);

            foreach (var q1 in query1)
            {
                var ped2 = query2.Find(x => x.Codigo == q1.Codigo);
                if (ped2 != null)
                {
                    q1.Filial = ped2.Filial;
                    q1.TipoOperacao = ped2.TipoOperacao;
                    q1.TipoCarga = ped2.TipoCarga;
                    q1.TipoDeCarga = ped2.TipoDeCarga;
                    q1.Empresa = ped2.Empresa;
                    q1.ModeloVeicularCarga = ped2.ModeloVeicularCarga;
                    q1.EnderecoDestino = ped2.EnderecoDestino;
                }
            }

            return query1;
        }

        public dynamic Consultar(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string nomeRemetente, double cnpjRemetente, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(nomeRemetente))
                result = result.Where(o => o.Remetente.Nome.Contains(nomeRemetente));

            if (cnpjRemetente > 0)
                result = result.Where(o => o.Remetente.CPF_CNPJ == cnpjRemetente);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicialColeta.Value.Date >= dataInicial.Date && o.DataInicialColeta.Value.Date < dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFinalColeta.Value.Date >= dataFinal.Date && o.DataFinalColeta.Value.Date < dataFinal.Date);

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).Select(o => new
            {
                o.Codigo,
                o.Numero,
                Remetente = o.Remetente != null ? o.Remetente.Nome + " (" + o.Remetente.CPF_CNPJ + ")" : string.Empty,
                Origem = o.Origem != null ? o.Origem.Estado.Sigla + " / " + o.Origem.Descricao : string.Empty,
                Destino = o.Destino != null ? o.Destino.Estado.Sigla + " / " + o.Destino.Descricao : string.Empty,
                ValorNFs = o.ValorTotalNotasFiscais.ToString("n2")
            });
        }

        public dynamic Consultar(int codigoEmpresa, double cpfCnpjRemetente, double cpfCnpjDestinatario, int codigoOrigem, int codigoDestino, int[] codigosVeiculos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa &&
                             obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto &&
                             ((obj.Remetente.CPF_CNPJ == cpfCnpjRemetente && obj.Destinatario.CPF_CNPJ == cpfCnpjDestinatario) ||
                              (obj.Origem.Codigo == codigoOrigem && obj.Destino.Codigo == codigoDestino && (from v in obj.Veiculos where codigosVeiculos.Contains(v.Codigo) select obj).Any()))
                         select obj;

            return result.Select(o => new
            {
                o.Codigo,
                o.Numero,
                Remetente = o.Remetente.Nome,
                Destinatario = o.Destinatario.Nome,
                Origem = o.Origem.Estado.Sigla + " / " + o.Origem.Descricao,
                Destino = o.Destino.Estado.Sigla + " / " + o.Destino.Descricao,
                ValorNFs = o.ValorTotalNotasFiscais.ToString("n2"),
                Peso = o.PesoTotal.ToString("n2")
            }).ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS> ConsultarPlanejamentoPedido(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sqlDinamico = ConsultarPlanejamentoPedidoTMS(filtrosPesquisa, somenteContarNumeroRegistros, parametrosConsulta);

            var consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS)));

            return consulta.SetTimeout(120).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS>();
        }

        public int ContarConsultaPlanejamentoPedido(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa)
        {
            var sqlDinamico = ConsultarPlanejamentoPedidoTMS(filtrosPesquisa, true, null);

            var consultaPlanejamentoPedido = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return consultaPlanejamentoPedido.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Pedido;
            return result.ToList();
        }
        private SQLDinamico ConsultarPlanejamentoPedidoTMS(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            string sql;
            string pattern = "yyyy-MM-dd";
            var parametros = new List<ParametroSQL>();

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0)) ";
            else
                sql = @"SELECT pedido.PED_CODIGO Codigo,
						pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador,

						  SUBSTRING((SELECT DISTINCT ', ' + carga.CAR_CODIGO_CARGA_EMBARCADOR
                                    FROM T_CARGA carga	
                                    join T_CARGA_PEDIDO cargaPedido on cargaPedido.CAR_CODIGO = carga.CAR_CODIGO
                                    WHERE cargaPedido.PED_CODIGO = pedido.PED_CODIGO FOR XML PATH('')), 3, 1000) CodigoCargaEmbarcador,

		                tomador.CLI_NOME Tomador,
		                tomador.CLI_CODIGO_INTEGRACAO TomadorCodigoIntegracao,
                        destino.LOC_DESCRICAO Destino,
						origem.LOC_DESCRICAO OrigemDescricao,
						origem.UF_SIGLA UF,
						estado.UF_NOME Estado,
						origem.LOC_IBGE CodigoIBGE,
						origem.PAI_CODIGO CodigoPais,
						pais.PAI_ABREVIACAO PaisAbreviacao,
						pais.PAI_NOME NomePais,
						estado.UF_NOME Estado,
						pedido.PED_SITUACAO_PLANEJAMENTO_TMS SituacaoPlanejamentoPedidoTMS,
						pedido.PED_DATA_AGENDAMENTO DataAgendamento,
						pedido.CAR_DATA_CARREGAMENTO_PEDIDO DataCarregamentoPedido,
                        SUBSTRING((SELECT DISTINCT ', ' + motorista.FUN_CELULAR
                                    FROM T_FUNCIONARIO motorista	
                                    join T_PEDIDO_MOTORISTA motoristaPedido on motoristaPedido.FUN_CODIGO = motorista.FUN_CODIGO
                                    WHERE motoristaPedido.PED_CODIGO = pedido.PED_CODIGO FOR XML PATH('')), 3, 1000) NumeroCelularMotorista,
						pedido.PED_MOTORISTA_CIENTE MotoristaCiente,
						pedido.PED_SITUACAO SituacaoPedido,
						pedido.PED_TIPO_TOMADOR TipoTomador,
                        expedidor.CLI_CGCCPF ExpedidorCpf,
                        expedidor.CLI_CODIGO_INTEGRACAO ExpedidorCodigoIntegracao,
                        recebedor.CLI_CGCCPF RecebedorNomeCPF,
                        recebedor.CLI_CODIGO_INTEGRACAO RecebedorCodigoIntegracao,
                        tomador.CLI_CGCCPF TomadorCPF,

		                localidadeRecebedor.LOC_DESCRICAO DestinoRecebedor,     
		                modeloVeicularCarga.MVC_DESCRICAO ModeloVeicularCarga,
		                pedido.PED_OBSERVACAO_INTERNA ObservacaoInterna,					
						
		                
						rotaFrete.ROF_CODIGO_INTEGRACAO NumeroRota,

		                SUBSTRING((SELECT DISTINCT ', ' + motorista.FUN_NOME
                                    FROM T_FUNCIONARIO motorista	
                                    join T_PEDIDO_MOTORISTA motoristaPedido on motoristaPedido.FUN_CODIGO = motorista.FUN_CODIGO
                                    WHERE motoristaPedido.PED_CODIGO = pedido.PED_CODIGO FOR XML PATH('')), 3, 1000) Motoristas,		               

						SUBSTRING((SELECT DISTINCT ', ' + gestor.FUN_NOME
                                    FROM T_FUNCIONARIO gestor	
                                    join T_FUNCIONARIO motorista on motorista.FUN_CODIGO_GESTOR = gestor.FUN_CODIGO
									join T_PEDIDO_MOTORISTA motoristaPedido on motoristaPedido.FUN_CODIGO = motorista.FUN_CODIGO
                                    WHERE motoristaPedido.PED_CODIGO = pedido.PED_CODIGO FOR XML PATH('')), 3, 1000) Gestor,
						
		                    pedido.PED_COM_TRATATIVA Tratativa,
		                    pedido.PED_DATA_PREVISAO_SAIDA DataPrevisaoSaida,
		                    pedido.PED_PREVISAO_ENTREGA PrevisaoEntrega,
		                    centroResultado.CRE_DESCRICAO CentroResultado,
		                    tipoCarga.TCG_DESCRICAO TipoDeCarga,
							SUBSTRING((SELECT DISTINCT ', ' + dadosSumarizados.CDS_DESTINATARIOS
                                    FROM T_CARGA_DADOS_SUMARIZADOS dadosSumarizados	
                                    join T_CARGA carga on carga.CDS_CODIGO = dadosSumarizados.CDS_CODIGO
									join T_CARGA_PEDIDO cargaPedido on cargaPedido.CAR_CODIGO  = carga.CAR_CODIGO
                                    WHERE cargaPedido.PED_CODIGO = pedido.PED_CODIGO FOR XML PATH('')), 3, 1000) Destinatarios,

							SUBSTRING((SELECT DISTINCT ', ' + dadosSumarizados.CDS_RECEBEDORES
                                    FROM T_CARGA_DADOS_SUMARIZADOS dadosSumarizados	
                                    join T_CARGA carga on carga.CDS_CODIGO = dadosSumarizados.CDS_CODIGO
									join T_CARGA_PEDIDO cargaPedido on cargaPedido.CAR_CODIGO  = carga.CAR_CODIGO
                                    WHERE cargaPedido.PED_CODIGO = pedido.PED_CODIGO FOR XML PATH('')), 3, 1000) Recebedores,

									
							SUBSTRING((SELECT DISTINCT ', ' + proprietario.CLI_NOME
                                    FROM T_VEICULO veiculo     
									join T_CLIENTE proprietario on proprietario.CLI_CGCCPF  = veiculo.VEI_PROPRIETARIO
                                    WHERE veiculo.VEI_CODIGO = pedido.VEI_CODIGO FOR XML PATH('')), 3, 1000) Proprietario,

							SUBSTRING((SELECT DISTINCT ', ' + proprietario.CLI_NOME
                                    FROM T_PEDIDO_VEICULO pedidoVeiculo
									join T_VEICULO veiculo on veiculo.VEI_CODIGO = pedidoVeiculo.VEI_CODIGO
									join T_CLIENTE proprietario on proprietario.CLI_CGCCPF  = veiculo.VEI_PROPRIETARIO
                                    WHERE pedidoVeiculo.VEI_CODIGO = pedido.VEI_CODIGO and veiculo.VEI_TIPOVEICULO = 0 FOR XML PATH('')), 3, 1000) ProprietarioVeiculoTracao,


							 (select sum(dadosSumarizados.CDS_PESO_TOTAL) FROM T_CARGA_DADOS_SUMARIZADOS dadosSumarizados 
							 join T_CARGA carga on carga.CDS_CODIGO = dadosSumarizados.CDS_CODIGO
							join T_CARGA_PEDIDO cargaPedido on cargaPedido.CAR_CODIGO  = carga.CAR_CODIGO
							 WHERE cargaPedido.PED_CODIGO = pedido.PED_CODIGO) Peso,

		                    tipoOperacao.TOP_DESCRICAO TipoOperacao,
		                    tipoOperacao.TOP_CODIGO TipoOperacaoCodigo,
		                    tipoOperacao.TOP_OBSERVACAO TipoOperacaoObservacao,
		                    fronteira.CLI_CODIGO_INTEGRACAO CodigoIntegracaoFronteira,
							fronteira.CLI_NOME FronteiraNome,
							fronteira.CLI_NOMEFANTASIA  FronteiraNomeFantasia,
							fronteira.CLI_CGCCPF CPFCNPJFronteira,
							fronteira.CLI_FISJUR TipoFronteira,
							fronteira.CLI_PONTO_TRANSBORDO PontoTransbordo,
		                    remetente.CLI_NOME Remetente, 
		                    destinatario.CLI_NOME Destinatario,
		                    pedido.PED_NUMERO_PALETES_FRACIONADO NumeroPaletesFracionado,
		                    pedido.PED_PALLET_SALDO_RESTANTE PalletSaldoRestante,
                            (SELECT TOP(1) integracao.PEI_SITUACAO FROM T_PEDIDO_INTEGRACAO integracao WHERE integracao.PED_CODIGO = pedido.PED_CODIGO) Integracao,                          
							expedidor.CLI_NOME Expedidor,
							recebedor.CLI_NOME RecebedorNome,
                            destino.UF_SIGLA EstadoDestinoSigla,
                            remetente.CLI_CGCCPF RemetenteCPF,
                            remetente.CLI_CODIGO_INTEGRACAO RemetenteCodigoIntegracao,
                            destinatario.CLI_CGCCPF DestinatarioCPF,
                            destinatario.CLI_CODIGO_INTEGRACAO DestinatarioCodigoIntegracao,
                            pedidoAdicional.PAD_INDICATIVO_COLETA_ENTREGA IndicativoColetaEntrega,
                            pedido.PED_TARA_CONTAINER TaraContainer,
							pedido.PED_LACRE_CONTAINER_UM LacreContainerUm,
							pedido.PED_LACRE_CONTAINER_DOIS LacreContainerDois,
							pedido.PED_LACRE_CONTAINER_TRES LacreContainerTres,
							pedido.PED_NUMERO_BOOKING NumeroBooking,
							pedido.PED_NUMERO_NAVIO NumeroNavio,
							pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS ValorTotalNotas,
							pedido.PED_CATEGORIA_OS CategoriaOS,
							pedido.PED_TIPO_OS_CONVERTIDO TipoOSConvertido,
                            tipoContainer.CTI_DESCRICAO TipoContainerDescricao,
                            container.CTR_DESCRICAO Container,
                            veiculo.VEI_TIPO TipoPropriedadeVeiculo,
   
		                    (select sum(carregamentoPedido.CRP_PALLET) FROM T_CARREGAMENTO_PEDIDO carregamentoPedido WHERE carregamentoPedido.PED_CODIGO = pedido.PED_CODIGO) SomaPallet 
";

            sql += @"FROM  T_PEDIDO pedido

                    left join T_CLIENTE recebedor ON recebedor.CLI_CGCCPF = pedido.CLI_CODIGO_RECEBEDOR
                    left join T_LOCALIDADES localidadeRecebedor ON localidadeRecebedor.LOC_CODIGO = recebedor.LOC_CODIGO
                    left join T_LOCALIDADES destino ON destino.LOC_CODIGO = pedido.LOC_CODIGO_DESTINO
                    left join T_LOCALIDADES origem ON origem.LOC_CODIGO = pedido.LOC_CODIGO_ORIGEM
                    left join T_MODELO_VEICULAR_CARGA modeloVeicularCarga ON modeloVeicularCarga.MVC_CODIGO = pedido.MVC_CODIGO
                    left join T_ROTA_FRETE rotaFrete on rotaFrete.ROF_CODIGO = pedido.ROF_CODIGO   
					                
                    left join T_CENTRO_RESULTADO centroResultado on centroResultado.CRE_CODIGO = pedido.CRE_CODIGO
                    left join T_TIPO_DE_CARGA tipoCarga on tipoCarga.TCG_CODIGO = pedido.TCG_CODIGO 
                    left join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.TOP_CODIGO = pedido.TOP_CODIGO
                    left join T_CLIENTE fronteira ON fronteira.CLI_CGCCPF = pedido.CLI_CGCCPF_FRONTEIRA
                    left join T_CLIENTE remetente on remetente.CLI_CGCCPF = pedido.CLI_CODIGO_REMETENTE
					left join T_CLIENTE tomador on tomador.CLI_CGCCPF = pedido.CLI_CODIGO_TOMADOR
                    left join T_CLIENTE destinatario on destinatario.CLI_CGCCPF = pedido.CLI_CODIGO
					left join T_CLIENTE expedidor on expedidor.CLI_CGCCPF = pedido.CLI_CODIGO_EXPEDIDOR
                    
					left join T_PAIS pais on pais.PAI_CODIGO = origem.PAI_CODIGO
					left join T_UF estado on estado.UF_SIGLA = origem.UF_SIGLA
					  
                    left join T_VEICULO veiculo on veiculo.VEI_CODIGO = pedido.VEI_CODIGO
                    
                    left join T_PEDIDO_ADICIONAL pedidoAdicional on pedidoAdicional.PED_CODIGO = pedido.PED_CODIGO
                    left join T_CONTAINER_TIPO tipoContainer on tipoContainer.CTI_CODIGO = pedido.CTI_CODIGO_RESERVA
                    left join T_CONTAINER container on container.CTR_CODIGO = pedido.CTR_CODIGO
                   
 ";
            //ATENÇÃO SÓ DEVE FAZER JOIN COM A TABELA PEDIDO, OU SEJA, DEVE TER UMA LINHA POR PEDIDO, AS TABELAS A BAIXO OCORRERAM ERRO AO CLIENTE
            //--left join T_VEICULO_CONJUNTO veiculoConjunto on veiculoConjunto.VEC_CODIGO_PAI = veiculo.VEI_CODIGO
            //--left join T_VEICULO veiculoReboque on veiculoReboque.VEI_CODIGO = veiculoConjunto.VEC_CODIGO_FILHO
            //--left join T_PEDIDO_INTEGRACAO integracao on integracao.PED_CODIGO = pedido.PED_CODIGO
            //--left join T_PEDIDO_MOTORISTA motoristaPedido on motoristaPedido.PED_CODIGO = pedido.PED_CODIGO
            //--left join T_FUNCIONARIO motorista on motorista.FUN_CODIGO = motoristaPedido.FUN_CODIGO

            sql += @" where 1=1 ";

            if (filtrosPesquisa.FiltrarPedidosPorSegragacaoEmpresasFiliaisVinculadasAoUsuario && filtrosPesquisa.CodigosEmpresa.Count > 0)
                sql += $"and pedido.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosEmpresa)})";
            else if (filtrosPesquisa.FiltrarPedidosPorSegragacaoEmpresasFiliaisVinculadasAoUsuario && filtrosPesquisa.CodigosEmpresa.Count == 0)
                sql += $"and pedido.EMP_CODIGO = 0";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                sql += $"and pedido.PED_CODIGO_CARGA_EMBARCADOR = :PEDIDO_PED_CODIGO_CARGA_EMBARCADOR";
                parametros.Add(new ParametroSQL("PEDIDO_PED_CODIGO_CARGA_EMBARCADOR", filtrosPesquisa.NumeroCarga));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
            {
                sql += $"and pedido.PED_NUMERO_PEDIDO_EMBARCADOR = :PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR";
                parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR", filtrosPesquisa.NumeroPedidoEmbarcador));
            }

            if (filtrosPesquisa.NumeroPedido > 0)
                sql += $"and pedido.PED_NUMERO = '{filtrosPesquisa.NumeroPedido}'";

            if (filtrosPesquisa.CodigoPedido > 0)
                sql += $"and pedido.PED_CODIGO = '{filtrosPesquisa.CodigoPedido}'";

            if (filtrosPesquisa.Situacao.HasValue && filtrosPesquisa.Situacao != SituacaoPedido.Todos)
                sql += $"and pedido.PED_SITUACAO = '{(int)filtrosPesquisa.Situacao.Value}'";

            if (filtrosPesquisa.SituacaoPlanejamentoPedidoTMS?.Count > 0)
                sql += $"and pedido.PED_SITUACAO_PLANEJAMENTO_TMS in ({string.Join(", ", filtrosPesquisa.SituacaoPlanejamentoPedidoTMS.Select(o => (int)o))})";

            if (filtrosPesquisa.Remetentes?.Count > 0)
                sql += $"and remetente.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.Remetentes)}) ";

            if (filtrosPesquisa.Destinatarios?.Count > 0)
                sql += $"and pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.Destinatarios)}) ";

            if (filtrosPesquisa.CodigosOrigem?.Count > 0)
                sql += $"and pedido.LOC_CODIGO_ORIGEM  in ({string.Join(", ", filtrosPesquisa.CodigosOrigem)}) ";

            if (filtrosPesquisa.CodigosDestino?.Count > 0)
                sql += $"and pedido.LOC_CODIGO_DESTINO  in ({string.Join(", ", filtrosPesquisa.CodigosDestino)}) ";

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                sql += $"and pedido.TCG_CODIGO  in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ";

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                sql += $"and pedido.TOP_CODIGO  in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ";

            if (filtrosPesquisa.Tomadores?.Count > 0)
            {
                if (filtrosPesquisa.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    sql += $"and tomador.CLI_CGCCPF  in ({string.Join(", ", filtrosPesquisa.Tomadores)}) ";

                if (filtrosPesquisa.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                    sql += $"and remetente.CLI_CGCCPF  in ({string.Join(", ", filtrosPesquisa.Tomadores)}) ";

                if (filtrosPesquisa.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                    sql += $"and destinatario.CLI_CGCCPF  in ({string.Join(", ", filtrosPesquisa.Tomadores)}) ";

                if (filtrosPesquisa.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                    sql += $"and recebedor.CLI_CGCCPF  in ({string.Join(", ", filtrosPesquisa.Tomadores)}) ";

                if (filtrosPesquisa.TipoTomador == Dominio.Enumeradores.TipoTomador.Tomador)
                    sql += $"and tomador.CLI_CGCCPF  in ({string.Join(", ", filtrosPesquisa.Tomadores)}) ";
            }

            if (filtrosPesquisa.CodigosVeiculo?.Count > 0)
                sql += $@" AND (pedido.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)})
                           OR EXISTS
                             (SELECT pedidoVeiculo.PED_CODIGO
                              FROM T_PEDIDO_VEICULO pedidoVeiculo
                              WHERE pedidoVeiculo.PED_CODIGO = pedido.PED_CODIGO
                                AND pedidoVeiculo.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)})
                           )
                           OR EXISTS
                             (SELECT veiculoReboque.VEI_CODIGO
                              FROM T_CARGA_VEICULOS_VINCULADOS veiculoReboque
                              JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = veiculoReboque.CAR_CODIGO               
                              WHERE CargaPedido.PED_CODIGO = pedido.PED_CODIGO
                                AND veiculoReboque.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)}))
                           ) ";


            if (filtrosPesquisa.CodigosMotorista?.Count > 0)
                sql += $" and exists (select motoristaPedido.PED_CODIGO FROM T_PEDIDO_MOTORISTA motoristaPedido where motoristaPedido.PED_CODIGO = pedido.PED_CODIGO and  motoristaPedido.FUN_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosMotorista)})) "; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosCidadePoloOrigem?.Count > 0)
                sql += $"and localidadeRecebedor.LOC_CODIGO_POLO  in ({string.Join(", ", filtrosPesquisa.CodigosCidadePoloOrigem)}) ";

            if (filtrosPesquisa.CodigosCidadePoloDestino?.Count > 0)
                sql += $"and localidadeRecebedor.LOC_CODIGO_POLO  in ({string.Join(", ", filtrosPesquisa.CodigosCidadePoloDestino)}) ";

            if (filtrosPesquisa.CodigosPaisOrigem?.Count > 0)
                sql += $"and origem.PAI_CODIGO  in ({string.Join(", ", filtrosPesquisa.CodigosPaisOrigem)}) ";

            if (filtrosPesquisa.CodigosPaisDestino?.Count > 0)
                sql += $"and destino.PAI_CODIGO  in ({string.Join(", ", filtrosPesquisa.CodigosPaisDestino)}) ";

            if (filtrosPesquisa.EstadosOrigem?.Count > 0)
                sql += $"and origem.UF_SIGLA  in ('{string.Join("', '", filtrosPesquisa.EstadosOrigem)}') ";

            if (filtrosPesquisa.EstadosDestino?.Count > 0)
                sql += $"and destino.UF_SIGLA  in ('{string.Join("', '", filtrosPesquisa.EstadosDestino)}') ";

            if (filtrosPesquisa.CodigosFuncionarioResponsavel?.Count > 0)
                sql += $"and veiculo.FUN_CODIGO_RESPONSAVEL  in ({string.Join(", ", filtrosPesquisa.CodigosFuncionarioResponsavel)}) ";

            if (filtrosPesquisa.CodigosTipoOperacaoDiferenteDe?.Count > 0)
                sql += $"and pedido.TOP_CODIGO  not in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacaoDiferenteDe)}) ";

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
                sql += $"and pedido.CRE_CODIGO  in ({string.Join(", ", filtrosPesquisa.CodigosCentroResultado)}) ";

            if (filtrosPesquisa.CodigosModelosVeicularesCarga?.Count > 0)
                sql += $"and modeloVeicularCarga.MVC_CODIGO  in ({string.Join(", ", filtrosPesquisa.CodigosModelosVeicularesCarga)}) ";

            if (filtrosPesquisa.CodigosSegmentosVeiculos?.Count > 0)
                sql += $"and (veiculo.VSE_CODIGO  in ({string.Join(", ", filtrosPesquisa.CodigosSegmentosVeiculos)}))  ";

            if (filtrosPesquisa.CodigosFronteiras?.Count > 0)
                sql += $"and fronteira.CLI_CGCCPF  in ({string.Join(", ", filtrosPesquisa.CodigosFronteiras)}) ";

            if (filtrosPesquisa.CodigosGestores?.Count > 0)
                sql += $"and pedido.PED_CODIGO in (SELECT motoristaPedido.PED_CODIGO" +
                    $" FROM T_FUNCIONARIO gestor" +
                    $" join T_FUNCIONARIO motorista on motorista.FUN_CODIGO_GESTOR = gestor.FUN_CODIGO" +
                    $" join T_PEDIDO_MOTORISTA motoristaPedido on motoristaPedido.FUN_CODIGO = motorista.FUN_CODIGO  " +
                    $" WHERE motorista.FUN_CODIGO_GESTOR in ({string.Join(", ", filtrosPesquisa.CodigosGestores)})) ";

            if (filtrosPesquisa.CodigoOperador > 0)
                sql += $"and pedido.FUN_CODIGO_AUTOR in ({string.Join(", ", filtrosPesquisa.CodigoOperador)}) ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPropriedadeVeiculo) && !filtrosPesquisa.TipoPropriedadeVeiculo.Equals("A"))
                sql += $"and veiculo.VEI_TIPO = '{(string)filtrosPesquisa.TipoPropriedadeVeiculo}' ";

            if (filtrosPesquisa.VinculoCarga?.Count > 0)
            {
                if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoComCarga) ^ filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoSemCarga))
                {
                    if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoComCarga))
                        sql += $"and exists (select cargaPedido.PED_CODIGO from T_CARGA_PEDIDO cargaPedido where cargaPedido.PED_CODIGO = pedido.PED_CODIGO) ";

                    if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoSemCarga))
                        sql += $"and not exists (select cargaPedido.PED_CODIGO from T_CARGA_PEDIDO cargaPedido where cargaPedido.PED_CODIGO = pedido.PED_CODIGO) ";
                }

                if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaComMotoristaVinculado) ^ filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaSemMotoristaVinculado))
                {
                    if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaComMotoristaVinculado))
                        sql += $"and exists (select 1 from T_PEDIDO_MOTORISTA motoristaPedido where motoristaPedido.PED_CODIGO = pedido.PED_CODIGO) ";

                    if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaSemMotoristaVinculado))
                        sql += $"and not exists (select 1 from T_PEDIDO_MOTORISTA motoristaPedido where motoristaPedido.PED_CODIGO = pedido.PED_CODIGO) ";
                }

                if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaComVeiculoVinculado) ^ filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaSemVeiculoVinculado))
                {
                    if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaComVeiculoVinculado))
                        sql += @$"and exists (select pedidoVeiculo.PED_CODIGO from T_PEDIDO_VEICULO pedidoVeiculo where pedidoVeiculo.PED_CODIGO = pedido.PED_CODIGO or pedido.VEI_CODIGO is not null) ";

                    if (filtrosPesquisa.VinculoCarga.Contains(PedidosVinculadosCarga.PedidoCargaSemVeiculoVinculado))
                        sql += $"and not exists (select pedidoVeiculo.PED_CODIGO from T_PEDIDO_VEICULO pedidoVeiculo where pedidoVeiculo.PED_CODIGO = pedido.PED_CODIGO or pedido.VEI_CODIGO is not null) ";
                }
            }

            if (filtrosPesquisa.DataInicial.HasValue)
            {
                if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.CARREGAMENTO_PEDIDO || !filtrosPesquisa.TipoFiltroData.HasValue)
                    sql += $"and pedido.CAR_DATA_CARREGAMENTO_PEDIDO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' ";
                else if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.PREVISAO_SAIDA)
                    sql += $"and pedido.PED_DATA_PREVISAO_SAIDA >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' ";
                else
                    sql += $"and pedido.pedido.PED_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' ";
            }

            if (filtrosPesquisa.DataLimite.HasValue)
            {
                if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.CARREGAMENTO_PEDIDO || !filtrosPesquisa.TipoFiltroData.HasValue)
                    sql += $" and pedido.CAR_DATA_CARREGAMENTO_PEDIDO < '{filtrosPesquisa.DataLimite.Value.AddDays(1).ToString(pattern)}' ";
                else if (filtrosPesquisa.TipoFiltroData == TipoFiltroDataMontagemCarga.PREVISAO_SAIDA)
                    sql += $" and pedido.PED_DATA_PREVISAO_SAIDA < '{filtrosPesquisa.DataLimite.Value.AddDays(1).ToString(pattern)}' ";
                else
                    sql += $" and pedido.PED_PREVISAO_ENTREGA < '{filtrosPesquisa.DataLimite.Value.AddDays(1).ToString(pattern)}' ";
            }

            if (filtrosPesquisa.CargaPerigosa.HasValue && filtrosPesquisa.CargaPerigosa != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                if (filtrosPesquisa.CargaPerigosa == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    sql += $"and tipoCarga.TCG_POSSUI_CARGA_PERIGOSA = '{(int)filtrosPesquisa.CargaPerigosa}' ";

                if (filtrosPesquisa.CargaPerigosa == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                    sql += $"and tipoCarga.TCG_POSSUI_CARGA_PERIGOSA = '{(int)filtrosPesquisa.CargaPerigosa}' ";
            }

            if (filtrosPesquisa.AceiteMotorista.HasValue)
                sql += $"and pedido.PED_ACEITE_MOTORISTA_ESCALA = {(int)filtrosPesquisa.AceiteMotorista} ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPropriedadeVeiculo) && !filtrosPesquisa.TipoPropriedadeVeiculo.Equals("A"))
                sql += $"and veiculo.VEI_TIPO = '{filtrosPesquisa.TipoPropriedadeVeiculo}' ";

            if (filtrosPesquisa.CodigosProvedores?.Count > 0)
                sql += $"and pedido.CLI_CODIGO_PROVEDOR_OS in ({string.Join(", ", filtrosPesquisa.CodigosProvedores)}) ";

            if (filtrosPesquisa.CategoriaOS?.Count > 0)
                sql += $"and pedido.PED_CATEGORIA_OS in ({string.Join(", ", filtrosPesquisa.CategoriaOS.Select(o => (int)o))}) ";

            if (filtrosPesquisa.TipoOSConvertido?.Count > 0)
                sql += $"and pedido.PED_TIPO_OS_CONVERTIDO in ({string.Join(", ", filtrosPesquisa.TipoOSConvertido.Select(o => (int)o))}) ";

            if (filtrosPesquisa.AceitaContratarTerceiros.HasValue)
            {
                if (filtrosPesquisa.AceitaContratarTerceiros.Value == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                {
                    sql += @$"and exists (select tipoPropriedadeVeiculoConfiguracaoTipoOperacao.CTV_CODIGO 
                                          from T_TIPO_OPERACAO tipoOperacao
                                          join T_CONFIGURACAO_TIPO_OPERACAO_TIPO_PROPRIEDADE_VEICULO tipoPropriedadeVeiculoConfiguracaoTipoOperacao on tipoPropriedadeVeiculoConfiguracaoTipoOperacao.CTV_CODIGO = tipoOperacao.CTV_CODIGO
                                          where tipoOperacao.TOP_CODIGO = pedido.TOP_CODIGO and tipoPropriedadeVeiculoConfiguracaoTipoOperacao.CTV_TIPO_PROPRIEDADE_VEICULO in (2, 3)) ";
                }
                else if (filtrosPesquisa.AceitaContratarTerceiros.Value == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                {
                    sql += @$"and exists (select tipoPropriedadeVeiculoConfiguracaoTipoOperacao.CTV_CODIGO 
                                          from T_TIPO_OPERACAO tipoOperacao
                                          join T_CONFIGURACAO_TIPO_OPERACAO_TIPO_PROPRIEDADE_VEICULO tipoPropriedadeVeiculoConfiguracaoTipoOperacao on tipoPropriedadeVeiculoConfiguracaoTipoOperacao.CTV_CODIGO = tipoOperacao.CTV_CODIGO
                                          where tipoOperacao.TOP_CODIGO = pedido.TOP_CODIGO and tipoPropriedadeVeiculoConfiguracaoTipoOperacao.CTV_TIPO_PROPRIEDADE_VEICULO = 1) ";
                }
            }

            if (parametrosConsulta != null && !somenteContarNumeroRegistros)
            {

                if (filtrosPesquisa.OrdernarPorPrioridade)
                {
                    sql += $" order by grupoPessoas.GRP_TORNAR_PEDIDOS_PRIORITARIOS DESC ";
                    sql += $" order by grupoPessoa.GRP_TORNAR_PEDIDOS_PRIORITARIOS DESC ";

                }

                else
                    sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "ASC" : "DESC ")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return new SQLDinamico(sql, parametros);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorUnicaCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Pedido;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorCargaEProtocoloPedido(int codigoCarga, int protocoloPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Pedido.Protocolo == protocoloPedido select obj.Pedido;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPrimeiroPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Pedido;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPrimeiroPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Pedido;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<int> BuscarCodigosPedidosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Pedido;
            return result.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPedidosPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Codigo == cargaPedido select obj.Pedido;
            return result.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarPedidosParaCancelamento(List<int> codigosPedidosExistentes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(p => p.DataCarregamentoPedido.HasValue && p.DataCarregamentoPedido.Value.Date >= DateTime.Now.Date.AddDays(-10));
            if (codigosPedidosExistentes != null && codigosPedidosExistentes.Count > 0)
                query = query.Where(p => !codigosPedidosExistentes.Contains(p.Codigo));

            query = query.Where(p => p.SituacaoPedido == SituacaoPedido.Aberto);
            query = query.Where(p => p.FileIdMichelin != "" && p.MessageIdentifierCodeMichelin != "");

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Ocorrencias.Any(o => o.Codigo == codigoOcorrencia) select obj.Pedido;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorPreCarga(int preCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.PreCarga.Codigo == preCarga && obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorCodigoRastreamento(string codigoRastreamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.CodigoRastreamento == codigoRastreamento && obj.SituacaoPedido != SituacaoPedido.Cancelado select obj;
            return result.FirstOrDefault();
        }

        public bool CodigoRastreamentoJaExistente(string codigoRastreamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.CodigoRastreamento == codigoRastreamento select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorPreCargaECodigosDivergentes(int preCarga, List<int> pedidosDiferenteDe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query
                         where
                         obj.PreCarga.Codigo == preCarga
                         && !pedidosDiferenteDe.Contains(obj.Codigo)
                         select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPorPreCargaRemetenteDestinatario(int preCarga, double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.PreCarga.Codigo == preCarga && obj.Remetente.CPF_CNPJ == remetente && obj.Destinatario.CPF_CNPJ == destinatario select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorLote(string lote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.NumeroLote == lote select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCarregamentoLote(string lote, string carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            if (!string.IsNullOrEmpty(lote))
                query = query.Where(p => p.NumeroLote == lote);
            if (!string.IsNullOrEmpty(carregamento))
                query = query.Where(p => p.NumeroCarregamento == carregamento);

            return query.Select(o => o).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorNumeroNotaFiscalParcial(int numeroNf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(p => p.PedidoNotasParciais.Any(nf => nf.Numero == numeroNf) && p.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.FirstOrDefault();
        }

        public decimal BuscarPesoTotalPorPreCargaEDestinatario(int codigoPreCarga, double cpfCnpjDestinatario)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.PreCarga.Codigo == codigoPreCarga && o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            return consultaPedido.Sum(o => (decimal?)o.PesoTotal) ?? 0m;
        }

        public int BuscarQuantidadeProdutosDiferentePorPreCargaEDestinatario(int codigoPreCarga, double cpfCnpjDestinatario)
        {
            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.PreCarga.Codigo == codigoPreCarga && o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario)
                .Select(o => o.Produtos.Count()).ToList();

            return consultaPedido.Sum();
        }

        /* BuscarEspelho
         * Método usado para gerar relatório de coleta
         * Não remova ele, mesmo não tendo referências
         */
        public List<Dominio.ObjetosDeValor.Relatorios.EspelhoColeta> BuscarEspelho(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from pedido in query
                         where pedido.Codigo == codigo && pedido.Empresa.Codigo == codigoEmpresa
                         select new Dominio.ObjetosDeValor.Relatorios.EspelhoColeta()
                         {
                             Codigo = pedido.Codigo,
                             CPFCNPJDestinatario = pedido.Destinatario != null ? pedido.Destinatario.CPF_CNPJ_Formatado : string.Empty,
                             NomeDestinatario = pedido.Destinatario != null ? pedido.Destinatario.Nome : string.Empty,
                             CPFCNPJRemetente = pedido.Remetente != null ? pedido.Remetente.CPF_CNPJ_Formatado : string.Empty,
                             NomeRemetente = pedido.Remetente != null ? pedido.Remetente.Nome : string.Empty,
                             CPFCNPJTomador = pedido.Tomador != null ? pedido.Tomador.CPF_CNPJ_Formatado : string.Empty,
                             NomeTomador = pedido.Tomador != null ? pedido.Tomador.Nome : string.Empty,
                             Origem = pedido.Origem != null ? pedido.Origem.Descricao + " - " + pedido.Origem.Estado.Sigla : string.Empty,
                             Destino = pedido.Destino != null ? pedido.Destino.Descricao + " - " + pedido.Destino.Estado.Sigla : string.Empty,
                             Numero = pedido.Numero,
                             DataInicial = pedido.DataInicialColeta.HasValue ? pedido.DataInicialColeta : null,
                             DataFinal = pedido.DataFinalColeta.HasValue ? pedido.DataFinalColeta : null,
                             DataEntrega = pedido.PrevisaoEntrega.HasValue ? pedido.PrevisaoEntrega : null,
                             Situacao = pedido.SituacaoPedido,
                             TipoCarga = pedido.TipoCarga != null ? pedido.TipoCarga.Descricao : string.Empty,
                             TipoColeta = pedido.TipoColeta != null ? pedido.TipoColeta.Descricao : string.Empty,
                             ValorNFs = pedido.ValorTotalNotasFiscais,
                             Peso = pedido.PesoTotal,
                             TipoPagamento = pedido.TipoPagamento,
                             Observacao = pedido.Observacao,
                             ObservacaoCTe = pedido.ObservacaoCTe,
                             CodigoPedidoCliente = pedido.CodigoPedidoCliente,
                             Requisitante = pedido.Requisitante
                         };

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioColetas> RelatorioColetas(
            int codigoEmpresa,
            DateTime dataInicial, DateTime dataFinal, DateTime dataEntrega,
            double destinatario, double tomador, double remetente,
            int origem, int destino,
            int motorista, int veiculo,
            int tipoColeta, int tipoCarga,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta? requisitante,
            Dominio.Enumeradores.TipoPagamento? tipoPagamento,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido? situacao
        )
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            // Filtros usando datas
            if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataInicialColeta.Value.Date >= dataInicial.Date && o.DataFinalColeta.Value.Date <= dataFinal.Date);
            else if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicialColeta.Value.Date >= dataInicial.Date && o.DataInicialColeta.Value.Date <= dataInicial.Date);
            else if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFinalColeta.Value.Date >= dataFinal.Date && o.DataFinalColeta.Value.Date <= dataFinal.Date);

            if (dataEntrega != DateTime.MinValue)
                result = result.Where(o => o.PrevisaoEntrega.Value.Date >= dataEntrega.Date && o.PrevisaoEntrega.Value.Date <= dataEntrega.Date);


            // Filtros usando clientes
            if (destinatario > 0)
                result = result.Where(o => o.Destinatario.CPF_CNPJ == destinatario);
            if (tomador > 0)
                result = result.Where(o => o.Tomador.CPF_CNPJ == tomador);
            if (remetente > 0)
                result = result.Where(o => o.Remetente.CPF_CNPJ == remetente);


            // Filtros usando localidade
            if (origem > 0)
                result = result.Where(o => o.Origem.Codigo == origem);
            if (destino > 0)
                result = result.Where(o => o.Destino.Codigo == destino);


            // Filtros usando veiculo e motorista
            if (motorista > 0)
                // result = result.Where(o => o.Motoristas.Contains(motorista));
                result = result.Where(o => (from m in o.Motoristas where motorista == m.Codigo select o).Any());
            if (veiculo > 0)
                // result = result.Where(o => o.Veiculos.Contains(veiculo));
                //(from v in o.Veiculos where codigosVeiculos.Contains(v.Codigo) select obj).Any())
                result = result.Where(o => (from v in o.Veiculos where veiculo == v.Codigo select o).Any());
            // result = result.Where(o => (from v in o.Veiculos where v.Codigo == veiculo select o).Any());


            // Filtros por tipo de carga e coleta
            if (tipoColeta > 0)
                result = result.Where(o => o.TipoColeta.Codigo == tipoColeta);
            if (tipoCarga > 0)
                result = result.Where(o => o.TipoCarga.Codigo == tipoCarga);


            // Filtros com enumeradores
            if (requisitante.HasValue)
                result = result.Where(o => o.Requisitante == requisitante);
            if (tipoPagamento.HasValue)
                result = result.Where(o => o.TipoPagamento == tipoPagamento);
            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoPedido == situacao);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioColetas()
            {
                CodigoPedido = o.Codigo,
                DataInicial = o.DataInicialColeta,
                Numero = o.Numero,
                // CPFCNPJ = o.Remetente != null ? o.Remetente.CPF_CNPJ.ToString() : string.Empty,
                Nome = o.Remetente != null && o.Remetente.CPF_CNPJ > 0 ? o.Remetente.Nome : string.Empty,
                ValorNFs = o.ValorTotalNotasFiscais,
                ValorFrete = o.ValorFreteNegociado,
                PesoTotal = o.PesoTotal
            }).ToList();
        }

        //public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidoParaRecolhimento(double destinatario)
        //{
        //    var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca>();
        //    var subResult = from obj in subQuery
        //                    where
        //                        obj.RecolhimentoTroca != null
        //                        && obj.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
        //                    select obj.RecolhimentoTroca;

        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
        //    var result = from obj in query
        //                 where
        //                    obj.Destinatario.CPF_CNPJ == destinatario
        //                    && obj.TipoOperacao.OperacaoRecolhimentoTroca == true
        //                    && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto
        //                    && !subResult.Contains(obj)
        //                 select obj;


        //    return result.ToList();
        //}

        //public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidoParaRecolhimento(List<double> destinatarios)
        //{
        //    var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca>();
        //    var subResult = from obj in subQuery
        //                    where
        //                        obj.RecolhimentoTroca != null
        //                        && obj.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
        //                    select obj.RecolhimentoTroca;

        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
        //    var result = from obj in query
        //                 where
        //                    destinatarios.Contains(obj.Destinatario.CPF_CNPJ)
        //                    && obj.TipoOperacao.OperacaoRecolhimentoTroca == true
        //                    && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto
        //                    && !subResult.Contains(obj)
        //                 select obj;


        //    return result.Fetch(x => x.Destinatario).ToList();
        //}

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoRecolhimentoDestinatario> BuscarPedidoParaRecolhimento(List<double> destinatarios)
        {
            var sqlQuery = @"

SELECT PED.PED_CODIGO as Codigo
     , PED.CLI_CODIGO as CPF_CNPJ
  FROM T_PEDIDO PED
     , T_TIPO_OPERACAO	TIP
 WHERE PED.TOP_CODIGO	= TIP.TOP_CODIGO
   AND TIP.TOP_OPERACAO_RECOLHIMENTO_TROCA = 1
   AND PED.PED_SITUACAO = :situacaoPedido 
   AND NOT EXISTS ( select 1
					  from T_PEDIDO_RECOLHIMENTO_TROCA PRT
						 , T_PEDIDO PEI
					 where PRT.PED_CODIGO = PEI.PED_CODIGO
					   and PEI.PED_SITUACAO <> :situacaoPedidoCanc  
					   AND PEI.PED_CODIGO = PED.PED_CODIGO )
   AND PED.CLI_CODIGO IN ( :cnpjs ) ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("situacaoPedido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto);
            query.SetParameter("situacaoPedidoCanc", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);
            query.SetParameterList("cnpjs", destinatarios);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoRecolhimentoDestinatario>();

        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoRecolhimentoDestinatario>> BuscarPedidoParaRecolhimentoAsync(List<double> destinatarios)
        {
            var sqlQuery = @"SELECT PED.PED_CODIGO as Codigo, PED.CLI_CODIGO as CPF_CNPJ
                                    FROM T_PEDIDO PED, T_TIPO_OPERACAO	TIP
                                     WHERE PED.TOP_CODIGO	= TIP.TOP_CODIGO
                                       AND TIP.TOP_OPERACAO_RECOLHIMENTO_TROCA = 1
                                       AND PED.PED_SITUACAO = :situacaoPedido 
                                       AND NOT EXISTS ( select 1
					                                      from T_PEDIDO_RECOLHIMENTO_TROCA PRT
						                                     , T_PEDIDO PEI
					                                     where PRT.PED_CODIGO = PEI.PED_CODIGO
					                                       and PEI.PED_SITUACAO <> :situacaoPedidoCanc  
					                                       AND PEI.PED_CODIGO = PED.PED_CODIGO )
                                       AND PED.CLI_CODIGO IN ( :cnpjs ) ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("situacaoPedido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto);
            query.SetParameter("situacaoPedidoCanc", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado);
            query.SetParameterList("cnpjs", destinatarios);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento)));

            return (await query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoRecolhimentoDestinatario>(CancellationToken)).ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedido> ConsultarRelatorioPedido(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPedido = new Repositorio.Embarcador.Cargas.ConsultaPedido().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPedido.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedido)));

            return consultaPedido.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedido>();
        }

        public int ContarConsultaRelatorioPedido(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPedido = new Repositorio.Embarcador.Cargas.ConsultaPedido().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPedido.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedidoProduto> ConsultarRelatorioPedidoProdutos(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            var consultaPedidoProdutos = this.SessionNHiBernate.CreateSQLQuery(new Repositorio.Embarcador.Cargas.ConsultaPedido().ObterSqlPesquisaProdutos(filtrosPesquisa));

            consultaPedidoProdutos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedidoProduto)));

            return consultaPedidoProdutos.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedidoProduto>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico> RelatorioPedido(int codigoPedido, string codigosPedidos, bool imprimirCarga, string viaImpressao)
        {
            string queryEmpresa = "", queryContemMotorista = "", queryModeloVeiculo = "", queryVeiculo = "", queryTipoCargaTipoOperacao = "", queryConsultaMotorista = "", queryMotorista = "", queryNumeroDataCarga = "";
            if (imprimirCarga)
            {
                queryEmpresa = @" LEFT OUTER JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CP.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO ";
                queryContemMotorista = @" ISNULL((SELECT COUNT(1)
                                FROM T_CARGA_MOTORISTA M
                                JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = M.CAR_MOTORISTA
                                JOIN T_CARGA_PEDIDO PP ON PP.CAR_CODIGO = M.CAR_CODIGO WHERE PP.PED_CODIGO = P.PED_CODIGO), 0) ContemMotorista, ";
                queryModeloVeiculo = " LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = CA.MVC_CODIGO ";
                queryVeiculo = @" CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(VV.VEI_PLACA AS NVARCHAR(2000)) +
                                            ISNULL((
                                                SELECT ', ' + VeiculoVinculado.VEI_PLACA
                                                    FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
                                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
                                                WHERE VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO  order by VeiculoVinculado.VEI_POSICAO_REBOQUE ASC
                                                    FOR XML PATH('')
		                                    ), '')
	                            FROM T_CARGA PV		
		                        JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
                                WHERE CP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) VeiculosCarga, '' VeiculosPedidos,";
                queryVeiculo += @" CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(VV.VEI_NUMERO_FROTA AS NVARCHAR(2000)) +
                                            ISNULL((
                                                SELECT ', ' + VeiculoVinculado.VEI_NUMERO_FROTA
                                                    FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
                                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
                                                WHERE VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO  order by VeiculoVinculado.VEI_POSICAO_REBOQUE ASC
                                                    FOR XML PATH('')
		                                    ), '')
	                            FROM T_CARGA PV		
		                        JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
                                WHERE CP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) NumerosFrotaVeiculosCarga, '' NumerosFrotaVeiculosPedido,";
                queryTipoCargaTipoOperacao = @"LEFT OUTER JOIN T_TIPO_DE_CARGA T ON T.TCG_CODIGO = CA.TCG_CODIGO
                                LEFT OUTER JOIN T_TIPO_OPERACAO TP ON TP.TOP_CODIGO = CA.TOP_CODIGO";
                queryConsultaMotorista = "";
                queryMotorista = @"CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(FF.FUN_NOME AS NVARCHAR(2000))
	                                    FROM T_CARGA_MOTORISTA PP
                                        JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
                                        WHERE PP.CAR_CODIGO = CA.CAR_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Motoristas,";
                queryNumeroDataCarga = @" CA.CAR_DATA_CRIACAO DataCarga, ISNULL(CA.CAR_CODIGO_CARGA_EMBARCADOR, '') + '" + viaImpressao + @"' NumeroCarga, ";

            }
            else if (!string.IsNullOrWhiteSpace(codigosPedidos))
            {
                queryEmpresa = @" LEFT OUTER JOIN T_CARREGAMENTO_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO CA ON CA.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO_CARGA CAR ON CAR.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARGA CARGA ON CARGA.CAR_CODIGO = CAR.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO ";
                queryContemMotorista = @" ISNULL((SELECT COUNT(1)
                                FROM T_CARREGAMENTO_MOTORISTAS M
                                JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = M.FUN_CODIGO
                                JOIN T_CARREGAMENTO_PEDIDO PP ON PP.CRG_CODIGO = M.CRG_CODIGO WHERE PP.PED_CODIGO = P.PED_CODIGO), 0) ContemMotorista, ";
                queryModeloVeiculo = " LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = CA.MVC_CODIGO ";
                queryVeiculo = @" CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(VV.VEI_PLACA AS NVARCHAR(2000)) +
	                                 ISNULL((SELECT DISTINCT ', ' + VeiculoVinculado.VEI_PLACA
                                                FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
				                                JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO AND VeiculoConjunto.CAR_CODIGO = CP.CAR_CODIGO
                                                JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO            
                                                FOR XML PATH('')),
                                     ISNULL((SELECT ', ' + VeiculoVinculado.VEI_PLACA
                                                    FROM T_VEICULO_CONJUNTO VeiculoConjunto
                                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEC_CODIGO_FILHO
                                                WHERE VeiculoConjunto.VEC_CODIGO_PAI = VV.VEI_CODIGO  order by VeiculoVinculado.VEI_POSICAO_REBOQUE ASC
                                                    FOR XML PATH('')
		                                    ), ''))
                                FROM T_CARREGAMENTO PV
                                JOIN T_CARREGAMENTO_PEDIDO CP ON CP.CRG_CODIGO = PV.CRG_CODIGO										
                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.VEI_CODIGO
                                WHERE CP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) VeiculosPedidos, '' VeiculosCarga, ";
                queryVeiculo += @" CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(VV.VEI_NUMERO_FROTA AS NVARCHAR(2000)) +
	                                 ISNULL((SELECT DISTINCT ', ' + VeiculoVinculado.VEI_NUMERO_FROTA
                                                FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
				                                JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO AND VeiculoConjunto.CAR_CODIGO = CP.CAR_CODIGO
                                                JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO            
                                                FOR XML PATH('')),
                                     ISNULL((SELECT ', ' + VeiculoVinculado.VEI_NUMERO_FROTA
                                                    FROM T_VEICULO_CONJUNTO VeiculoConjunto
                                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEC_CODIGO_FILHO
                                                WHERE VeiculoConjunto.VEC_CODIGO_PAI = VV.VEI_CODIGO  order by VeiculoVinculado.VEI_POSICAO_REBOQUE ASC
                                                    FOR XML PATH('')
		                                    ), ''))
                                FROM T_CARREGAMENTO PV
                                JOIN T_CARREGAMENTO_PEDIDO CP ON CP.CRG_CODIGO = PV.CRG_CODIGO										
                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.VEI_CODIGO
                                WHERE CP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) NumerosFrotaVeiculosPedido, '' NumerosFrotaVeiculosCarga, ";
                queryTipoCargaTipoOperacao = @"LEFT OUTER JOIN T_TIPO_DE_CARGA T ON T.TCG_CODIGO = P.TCG_CODIGO
                                LEFT OUTER JOIN T_TIPO_OPERACAO TP ON TP.TOP_CODIGO = P.TOP_CODIGO";
                queryConsultaMotorista = @"LEFT OUTER JOIN T_PEDIDO_MOTORISTA PM ON PM.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = PM.FUN_CODIGO";
                queryMotorista = @"CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(FF.FUN_NOME AS NVARCHAR(2000))
	                                    FROM T_PEDIDO_MOTORISTA PP
                                        JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
                                        WHERE PP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Motoristas,";
                queryNumeroDataCarga = @" CARGA.CAR_DATA_CRIACAO DataCarga, ISNULL(CARGA.CAR_CODIGO_CARGA_EMBARCADOR, '') + '" + viaImpressao + @"' NumeroCarga, ";
            }
            else
            {
                queryEmpresa = " LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = ISNULL(P.EMP_CODIGO, FP.EMP_CODIGO) ";
                queryContemMotorista = @" ISNULL((SELECT COUNT(1)
								FROM T_PEDIDO_MOTORISTA PP WHERE PP.PED_CODIGO = P.PED_CODIGO), 0) ContemMotorista, ";
                queryModeloVeiculo = " LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = P.MVC_CODIGO ";
                queryVeiculo = @" ISNULL(SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA + ' - ' + Veiculo.UF_SIGLA +
	                            ISNULL((SELECT DISTINCT ', ' + VeiculoVinculado.VEI_PLACA + ' - ' + VeiculoVinculado.UF_SIGLA
                                        FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
				                        JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO AND VeiculoConjunto.CAR_CODIGO = CP.CAR_CODIGO
                                        JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO            
                                        FOR XML PATH('')),
		                        ISNULL((SELECT DISTINCT ', ' + VeiculoVinculado.VEI_PLACA
                                        FROM T_VEICULO_CONJUNTO VeiculoConjunto
                                        JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEC_CODIGO_FILHO
                                    WHERE VeiculoConjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO
                                        FOR XML PATH('')
		                        ), ''))
                            FROM T_PEDIDO _Pedido
                            JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = _Pedido.VEI_CODIGO
                            WHERE _Pedido.PED_CODIGO = P.PED_CODIGO
                            FOR XML PATH('')
                        ), 3, 2000), '') VeiculosPedidos, ";
                queryVeiculo += @" ISNULL(SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_NUMERO_FROTA +
	                            ISNULL((SELECT DISTINCT ', ' + VeiculoVinculado.VEI_NUMERO_FROTA
                                        FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
				                        JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO AND VeiculoConjunto.CAR_CODIGO = CP.CAR_CODIGO
                                        JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO            
                                        FOR XML PATH('')),
		                        ISNULL((SELECT DISTINCT ', ' + VeiculoVinculado.VEI_NUMERO_FROTA
                                        FROM T_VEICULO_CONJUNTO VeiculoConjunto
                                        JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEC_CODIGO_FILHO
                                    WHERE VeiculoConjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO
                                        FOR XML PATH('')
		                        ), ''))
                            FROM T_PEDIDO _Pedido
                            JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = _Pedido.VEI_CODIGO
                            WHERE _Pedido.PED_CODIGO = P.PED_CODIGO
                            FOR XML PATH('')
                        ), 3, 2000), '') NumerosFrotaVeiculosPedido, ";
                queryVeiculo += @" CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(VV.VEI_PLACA AS NVARCHAR(2000)) +
                                            ISNULL((
                                                SELECT  ', ' + VeiculoVinculado.VEI_PLACA
                                                    FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
                                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
                                                WHERE VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO  order by VeiculoVinculado.VEI_POSICAO_REBOQUE ASC
                                                    FOR XML PATH('')
		                                    ), '')
	                            FROM T_CARGA PV		
		                        JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
                                WHERE CP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) VeiculosCarga, ";
                queryVeiculo += @" CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(VV.VEI_NUMERO_FROTA AS NVARCHAR(2000)) +
                                            ISNULL((
                                                SELECT  ', ' + VeiculoVinculado.VEI_NUMERO_FROTA
                                                    FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
                                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
                                                WHERE VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO  order by VeiculoVinculado.VEI_POSICAO_REBOQUE ASC
                                                    FOR XML PATH('')
		                                    ), '')
	                            FROM T_CARGA PV		
		                        JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
                                WHERE CP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) NumerosFrotaVeiculosCarga, ";
                queryTipoCargaTipoOperacao = @"LEFT OUTER JOIN T_TIPO_DE_CARGA T ON T.TCG_CODIGO = P.TCG_CODIGO
                                LEFT OUTER JOIN T_TIPO_OPERACAO TP ON TP.TOP_CODIGO = P.TOP_CODIGO";
                queryConsultaMotorista = @"LEFT OUTER JOIN T_PEDIDO_MOTORISTA PM ON PM.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = PM.FUN_CODIGO";
                queryMotorista = @"CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(FF.FUN_NOME AS NVARCHAR(2000))
	                                    FROM T_PEDIDO_MOTORISTA PP
                                        JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
                                        WHERE PP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Motoristas,";
                queryNumeroDataCarga = @" (select TOP(1) CA.CAR_DATA_CRIACAO
                                            from T_CARGA_PEDIDO CP 
                                            JOIN T_CARGA CA ON CA.CAR_CODIGO = CP.CAR_CODIGO
                                            WHERE CP.PED_CODIGO = P.PED_CODIGO ORDER BY CA.CAR_CODIGO DESC) DataCarga, 
                                          isnull((select TOP(1) CA.CAR_CODIGO_CARGA_EMBARCADOR
                                            from T_CARGA_PEDIDO CP 
                                            JOIN T_CARGA CA ON CA.CAR_CODIGO = CP.CAR_CODIGO
                                            WHERE CP.PED_CODIGO = P.PED_CODIGO ORDER BY CA.CAR_CODIGO DESC), '') + '" + viaImpressao + @"' NumeroCarga , ";
            }

            string query = @"   SELECT E.EMP_CNPJ CNPJEmpresa, E.EMP_RAZAO RazaoEmpresa, E.EMP_CEP CEPEmpresa, E.EMP_ENDERECO RuaEmpresa, E.EMP_BAIRRO BairroEmpresa, E.EMP_NUMERO NumeroEmpresa,
                                 E.EMP_COMPLEMENTO ComplementoEmpresa, E.EMP_INSCRICAO IEEmpresa, E.EMP_FONE FoneEmpresa, LE.LOC_DESCRICAO CidadeEmpresa, LE.UF_SIGLA EstadoEmpresa,
                                " + queryMotorista + @"
                                " + queryVeiculo + @"
                                CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(PE.GRP_DESCRICAO AS NVARCHAR(2000))
	                                    FROM T_PEDIDO_PRODUTO PP
                                        JOIN T_PRODUTO_EMBARCADOR PE ON PE.PRO_CODIGO = PP.PRO_CODIGO
                                        WHERE PP.PED_CODIGO = P.PED_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Produtos,
                                MO.MVC_DESCRICAO Modelo,
                                MO.MVC_CAPACIDADE_PESO_TRANSPORTE LimitePesoModeloVeicular,
                                P.PED_QUANTIDADE_AJUDANTE Ajudantes,
                                P.PED_COTACAO Cotacao,
                                P.PED_VALOR_FRETE_COTADO ValorFreteCotado,
                                CASE                                    
                                    WHEN P.PED_TIPO_PESSOA = 1 THEN 
			                        CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_CGCCPF ELSE CR.CLI_CGCCPF END                                
                                    ELSE ''                                  
                                END CnpjCpfRemetente,
                                CASE	
	                                WHEN P.PED_TIPO_PESSOA = 1 THEN CR.CLI_FISJUR
	                                ELSE ''
                                END TipoRemetente,
                                CASE	
	                                WHEN P.PED_TIPO_PESSOA = 1 THEN 
			                        CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_NOME ELSE CR.CLI_NOME END        
                                    ELSE GR.GRP_DESCRICAO                                  
                                END NomeRemetente,
                                CASE	
	                                WHEN P.PED_TIPO_PESSOA = 1 THEN 
			                        CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_ENDERECO ELSE CR.CLI_ENDERECO END                                        
                                    ELSE ''                                  
                                END EnderecoRemetente,                                                            
                                CASE
                                    WHEN LocalidadeExpedidor.LOC_CODIGO > 0 THEN LocalidadeExpedidor.LOC_DESCRICAO
                                    ELSE LOREM.LOC_DESCRICAO
                                END CidadeRemetente,
                                CASE
                                    WHEN LocalidadeExpedidor.LOC_CODIGO > 0 THEN LocalidadeExpedidor.UF_SIGLA
                                    ELSE LOREM.UF_SIGLA
                                END EstadoRemetente,
                                CASE
                                    WHEN LocalidadeExpedidor.LOC_CODIGO > 0 THEN LocalidadeExpedidor.LOC_DESCRICAO
                                    ELSE LO.LOC_DESCRICAO
                                END CidadeOrigemOriginal,
								CASE
                                    WHEN LocalidadeExpedidor.LOC_CODIGO > 0 THEN LocalidadeExpedidor.UF_SIGLA
                                    ELSE LO.UF_SIGLA
                                END EstadoOrigemOriginal,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN 
									CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_ENDERECO ELSE CR.CLI_ENDERECO end						
	                                ELSE PO.PEN_ENDERECO
                                END EnderecoOrigem,					
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN
									CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_BAIRRO ELSE CR.CLI_BAIRRO end				
	                                ELSE PO.PEN_BAIRRO
                                END BairroOrigem,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN
									CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_CEP ELSE CR.CLI_CEP end									
	                                ELSE PO.PEN_CEP
                                END CEPOrigem,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN
									CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_COMPLEMENTO ELSE CR.CLI_COMPLEMENTO end								
	                                ELSE PO.PEN_COMPLEMENTO
                                END ComplementoOrigem,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN 
									CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_NUMERO ELSE CR.CLI_NUMERO end									
	                                ELSE PO.PEN_NUMERO
                                END NumeroOrigem,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN 
									CASE WHEN ClienteExpedidorPedido.CLI_CGCCPF > 0 THEN ClienteExpedidorPedido.CLI_FONE ELSE CR.CLI_FONE end											
	                                ELSE PO.PEN_FONE
                                END FoneOrigem,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN 
									CASE WHEN LocalidadeExpedidor.LOC_CODIGO > 0 THEN LocalidadeExpedidor.LOC_DESCRICAO ELSE LOREM.LOC_DESCRICAO end				
	                                ELSE LORIGEM.LOC_DESCRICAO
                                END CidadeOrigem, 
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_ORIGEM = 0 THEN 
									CASE WHEN LocalidadeRecebedor.LOC_CODIGO > 0 THEN LocalidadeRecebedor.UF_SIGLA ELSE LOREM.UF_SIGLA end			
	                                ELSE LORIGEM.UF_SIGLA
                                END EstadoOrigem,								
								CASE
                                    WHEN CienteRecebedorPedido.LOC_CODIGO > 0 THEN CienteRecebedorPedido.CLI_CGCCPF
                                    ELSE CD.CLI_CGCCPF
                                END CnpjCpfDestinatario,
								CASE
                                    WHEN CienteRecebedorPedido.LOC_CODIGO > 0 THEN CienteRecebedorPedido.CLI_FISJUR
                                    ELSE CD.CLI_FISJUR
                                END TipoDestinatario,
								CASE
                                    WHEN CienteRecebedorPedido.LOC_CODIGO > 0 THEN CienteRecebedorPedido.CLI_NOME
                                    ELSE CD.CLI_NOME
                                END NomeDestinatario,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 0 THEN 
									CASE WHEN CienteRecebedorPedido.CLI_CGCCPF > 0 THEN CienteRecebedorPedido.CLI_ENDERECO ELSE CD.CLI_ENDERECO end									
	                                ELSE PD.PEN_ENDERECO
                                END EnderecoDestino,
								CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 0 THEN 
									CASE WHEN CienteRecebedorPedido.CLI_CGCCPF > 0 THEN CienteRecebedorPedido.CLI_BAIRRO ELSE CD.CLI_BAIRRO end
	                                ELSE PD.PEN_BAIRRO
                                END BairroDestino,						
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 0 THEN
									CASE WHEN CienteRecebedorPedido.CLI_CGCCPF > 0 THEN CienteRecebedorPedido.CLI_CEP ELSE CD.CLI_CEP end								
	                                ELSE PD.PEN_CEP
                                END CEPDestino,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 0 THEN
									CASE WHEN CienteRecebedorPedido.CLI_CGCCPF > 0 THEN CienteRecebedorPedido.CLI_COMPLEMENTO ELSE CD.CLI_COMPLEMENTO end
	                                ELSE PD.PEN_COMPLEMENTO
                                END ComplementoDestino,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 0 THEN 
									CASE WHEN CienteRecebedorPedido.CLI_CGCCPF > 0 THEN CienteRecebedorPedido.CLI_NUMERO ELSE CD.CLI_NUMERO end								
	                                ELSE PD.PEN_NUMERO
                                END NumeroDestino,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 0 THEN 
									CASE WHEN CienteRecebedorPedido.CLI_CGCCPF > 0 THEN CienteRecebedorPedido.CLI_FONE ELSE CD.CLI_FONE end						
	                                ELSE PD.PEN_FONE
                                END FoneDestino,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 0 THEN 
									CASE WHEN LocalidadeRecebedor.LOC_CODIGO > 0 THEN LocalidadeRecebedor.LOC_DESCRICAO ELSE LOCDEST.LOC_DESCRICAO end								
	                                ELSE LODEST.LOC_DESCRICAO
                                END CidadeDestino,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 1 THEN LODEST.UF_SIGLA
	                                ELSE LOCDEST.UF_SIGLA
                                END EstadoDestino,
                                CASE
                                    WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalPaletizacao.CLI_NOME
                                    ELSE ''
                                END NomeLocalPaletizacao,
                                CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalPaletizacao.CLI_ENDERECO								
	                                ELSE ''
                                END EnderecoLocalPaletizacao,
								CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalPaletizacao.CLI_BAIRRO
	                                ELSE ''
                                END BairroLocalPaletizacao,						
                                CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalPaletizacao.CLI_CEP							
	                                ELSE ''
                                END CEPLocalPaletizacao,
                                CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalPaletizacao.CLI_COMPLEMENTO
	                                ELSE ''
                                END ComplementoLocalPaletizacao,
                                CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalPaletizacao.CLI_NUMERO							
	                                ELSE ''
                                END NumeroLocalPaletizacao,
                                CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalPaletizacao.CLI_FONE
	                                ELSE ''
                                END FoneLocalPaletizacao,
                                CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN 
									CASE WHEN LocalidadePaletizacao.LOC_CODIGO > 0 THEN LocalidadePaletizacao.LOC_DESCRICAO ELSE '' end								
	                                ELSE ''
                                END CidadeLocalPaletizacao,
                                CASE
	                                WHEN P.CLI_CODIGO_LOCAL_PALETIZACAO > 0 THEN LocalidadePaletizacao.UF_SIGLA
	                                ELSE ''
                                END EstadoLocalPaletizacao,
                                RemetenteParticipante.CLI_CGCCPF CnpjCpfRemetenteParticipante,
								RemetenteParticipante.CLI_FISJUR TipoRemetenteParticipante,
								RemetenteParticipante.CLI_NOME NomeRemetenteParticipante,
                                RemetenteParticipante.CLI_ENDERECO EnderecoRemetenteParticipante,								
                                RemetenteParticipante.CLI_BAIRRO BairroRemetenteParticipante,						
                                RemetenteParticipante.CLI_CEP CEPRemetenteParticipante,
                                RemetenteParticipante.CLI_COMPLEMENTO ComplementoRemetenteParticipante,
                                RemetenteParticipante.CLI_NUMERO NumeroRemetenteParticipante,                                
								RemetenteParticipante.CLI_FONE FoneRemetenteParticipante,
                                LocRemetenteParticipante.LOC_DESCRICAO CidadeRemetenteParticipante,
                                LocRemetenteParticipante.UF_SIGLA EstadoRemetenteParticipante,
                                DestinatarioParticipante.CLI_CGCCPF CnpjCpfDestinatarioParticipante,
								DestinatarioParticipante.CLI_FISJUR TipoDestinatarioParticipante,
								DestinatarioParticipante.CLI_NOME NomeDestinatarioParticipante,
                                DestinatarioParticipante.CLI_ENDERECO EnderecoDestinatarioParticipante,								
                                DestinatarioParticipante.CLI_BAIRRO BairroDestinatarioParticipante,						
                                DestinatarioParticipante.CLI_CEP CEPDestinatarioParticipante,
                                DestinatarioParticipante.CLI_COMPLEMENTO ComplementoDestinatarioParticipante,
                                DestinatarioParticipante.CLI_NUMERO NumeroDestinatarioParticipante,                                
								DestinatarioParticipante.CLI_FONE FoneDestinatarioParticipante,
                                LocDestinatarioParticipante.LOC_DESCRICAO CidadeDestinatarioParticipante,
                                LocDestinatarioParticipante.UF_SIGLA EstadoDestinatarioParticipante,
                                P.PED_ESCOLTA_ARMADA EscoltaArmada,
                                P.PED_ESCOLTA_MUNICIPAL EscoltaMunicipal,
                                P.PED_AJUDANTE Ajudante,
                                T.TCG_DESCRICAO TipoCarga,
                                TP.TOP_DESCRICAO TipoOperacao,
                                P.PED_OBSERVACAO Observacao,
                                P.PED_OBSERVACAO_CTE ObservacaoCTe,
                                P.PED_QUANTIDADE_ENTREGAS QtdEntregas,
                                P.PED_QUANTIDADE_VOLUMES QtdVolumes,
                                P.PED_PESO_TOTAL_CARGA PesoCarga,
                                P.PED_CUBAGEM_TOTAL PesoPaletes,
                                P.PED_NUMERO_PALETES_FRACIONADO QtdPaletes,
                                FP.FUN_LOGIN Usuario,
                                FP.FUN_NOME Operador,
                                P.CAR_DATA_CARREGAMENTO_PEDIDO DataCarregamento,
                                P.PED_PREVISAO_ENTREGA DataPrevisaoFim,
                                P.PED_DATA_PREVISAO_SAIDA DataPrevisaoInicio,
                                P.PED_DATA_PREVISAO_SAIDA DataPrevisaoSaida,
                                p.PED_DATA_FINAL_COLETA as DataFinalColeta,
								p.PED_DATA_INICIAL_COLETA as DataColeta,
                                CAST(P.PED_NUMERO AS VARCHAR(20)) Numero,
                                P.PED_NUMERO_PEDIDO_EMBARCADOR NumeroCliente,
                                P.PED_CODIGO CodigoPedido,
                                ISNULL((SELECT COUNT(1)
								FROM T_PEDIDO_PRODUTO PP WHERE PP.PED_CODIGO = P.PED_CODIGO), 0) ContemProduto,
								ISNULL((SELECT COUNT(1)
								FROM T_PEDIDO_PRODUTO_ONU PP JOIN T_PEDIDO_PRODUTO PPP ON PPP.PRP_CODIGO = PP.PRP_CODIGO WHERE PPP.PED_CODIGO = P.PED_CODIGO), 0) ContemONUProduto,
								" + queryContemMotorista + @"
                                " + queryNumeroDataCarga + @"    
                                ISNULL((SELECT COUNT(1)
								FROM T_PEDIDO_IMPORTACAO PP WHERE PP.PED_CODIGO = P.PED_CODIGO), 0) ContemImportacao,
                                P.PED_TIPO_TOMADOR TipoTomador,
                                ISNULL(P.PED_TIPO_PAGAMENTO, 0) TipoPagamento,
                                TOM.CLI_NOME NomeOutroTomador,
								TOM.CLI_ENDERECO EnderedoOutroTomador,
								LTOM.LOC_DESCRICAO CidadeOutroTomador,
								LTOM.UF_SIGLA EstadoOutroTomador,
								TOM.CLI_BAIRRO BairroOutroTomador,
								TOM.CLI_CEP CEPOutroTomador,
								TOM.CLI_COMPLEMENTO ComplementoOutroTomador,
								TOM.CLI_NUMERO NumeroOutroTomador,
								TOM.CLI_FONE FoneOutroTomador,
                                (SELECT TOP 1 CEM_OBSERVACAO_GERAL_PEDIDO FROM T_CONFIGURACAO_EMBARCADOR) ObservacaoGeral,
                                P.PED_CODIGO_PEDIDO_CLIENTE CodigoPedidoCliente
                                FROM T_PEDIDO P
                                JOIN T_LOCALIDADES LO ON LO.LOC_CODIGO = P.LOC_CODIGO_ORIGEM
                                LEFT OUTER JOIN T_FUNCIONARIO FP ON FP.FUN_CODIGO = P.FUN_CODIGO 
                                " + queryEmpresa + @" 
                                LEFT OUTER JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                                " + queryConsultaMotorista + @"
                                LEFT OUTER JOIN T_CLIENTE CR ON CR.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_LOCALIDADES LOREM ON LOREM.LOC_CODIGO = CR.LOC_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GR ON GR.GRP_CODIGO = P.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CD ON CD.CLI_CGCCPF = P.CLI_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LOCDEST ON LOCDEST.LOC_CODIGO = CD.LOC_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = P.LOC_CODIGO_DESTINO
                                " + queryTipoCargaTipoOperacao + @"
                                " + queryModeloVeiculo + @"
                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PO ON PO.PEN_CODIGO = P.PEN_CODIGO_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LORIGEM ON LORIGEM.LOC_CODIGO = PO.LOC_CODIGO
                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PD ON PD.PEN_CODIGO = P.PEN_CODIGO_DESTINO
                                LEFT OUTER JOIN T_LOCALIDADES LODEST ON LODEST.LOC_CODIGO = PD.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE TOM ON TOM.CLI_CGCCPF = P.CLI_CODIGO_TOMADOR
								LEFT OUTER JOIN T_LOCALIDADES LTOM ON LTOM.LOC_CODIGO = TOM.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE ClienteExpedidorPedido ON ClienteExpedidorPedido.CLI_CGCCPF = P.CLI_CODIGO_EXPEDIDOR
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeExpedidor ON LocalidadeExpedidor.LOC_CODIGO = ClienteExpedidorPedido.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CienteRecebedorPedido on CienteRecebedorPedido.CLI_CGCCPF = P.CLI_CODIGO_RECEBEDOR
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeRecebedor ON LocalidadeRecebedor.LOC_CODIGO = CienteRecebedorPedido.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE RemetenteParticipante on RemetenteParticipante.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_LOCALIDADES LocRemetenteParticipante on LocRemetenteParticipante.LOC_CODIGO = RemetenteParticipante.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE DestinatarioParticipante on DestinatarioParticipante.CLI_CGCCPF = P.CLI_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocDestinatarioParticipante on LocDestinatarioParticipante.LOC_CODIGO = DestinatarioParticipante.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE LocalPaletizacao on LocalPaletizacao.CLI_CGCCPF = P.CLI_CODIGO_LOCAL_PALETIZACAO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadePaletizacao ON LocalidadePaletizacao.LOC_CODIGO = LocalPaletizacao.LOC_CODIGO
                                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigosPedidos))
                query += " AND P.PED_CODIGO IN (" + codigosPedidos + ")";
            else if (codigoPedido > 0)
                query += " AND P.PED_CODIGO = " + codigoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PrestacaoServico>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico> RelatorioMotoristaPedido(int codigoPedido, string codigosPedidos, bool impressaoCarga)
        {
            string query = "";

            if (impressaoCarga)
            {
                query = @"SELECT FF.FUN_CPF CPF, FF.FUN_NOME Nome, PP.PED_CODIGO CodigoPedido, FF.FUN_NUMHABILITACAO CNH, FF.FUN_FONE Telefone, FF.FUN_VECTOHABILITACAO ValidadeCNH
                                 ,FF.FUN_DATA_PRIMEIRA_HABILITACAO PrimeiraCNH, FF.FUN_NUMERO_REGISTRO_HABILITACAO RegistroCNH,  FF.FUN_CATEGORIA CategoriaCNH, FF.FUN_DATAHABILITACAO EmissaoCNH
                                , (select TOP 1 CEM_INFORMACAO_ADICIONAL_MOTORISTA_ORDEM_COLETA from T_CONFIGURACAO_EMBARCADOR) UtilizaInformacoesAdicionionais
					FROM T_CARGA_MOTORISTA M
					JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = M.CAR_MOTORISTA
					JOIN T_CARGA_PEDIDO PP ON PP.CAR_CODIGO = M.CAR_CODIGO
                    WHERE 1 = 1";
                query += " AND PP.PED_CODIGO IN (" + codigosPedidos + ")";
            }
            else if (!string.IsNullOrWhiteSpace(codigosPedidos))
            {
                query = @"SELECT FF.FUN_CPF CPF, FF.FUN_NOME Nome, PP.PED_CODIGO CodigoPedido, FF.FUN_NUMHABILITACAO CNH, FF.FUN_FONE Telefone, FF.FUN_VECTOHABILITACAO ValidadeCNH
                                 ,FF.FUN_DATA_PRIMEIRA_HABILITACAO PrimeiraCNH, FF.FUN_NUMERO_REGISTRO_HABILITACAO RegistroCNH,  FF.FUN_CATEGORIA CategoriaCNH, FF.FUN_DATAHABILITACAO EmissaoCNH
                                , (select TOP 1 CEM_INFORMACAO_ADICIONAL_MOTORISTA_ORDEM_COLETA from T_CONFIGURACAO_EMBARCADOR) UtilizaInformacoesAdicionionais
					FROM T_CARREGAMENTO_MOTORISTAS M
					JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = M.FUN_CODIGO
					JOIN T_CARREGAMENTO_PEDIDO PP ON PP.CRG_CODIGO = M.CRG_CODIGO
                    WHERE 1 = 1";
                query += " AND PP.PED_CODIGO IN (" + codigosPedidos + ")";
            }
            else if (codigoPedido > 0)
            {
                query = @"SELECT FF.FUN_CPF CPF, FF.FUN_NOME Nome, PP.PED_CODIGO CodigoPedido, FF.FUN_NUMHABILITACAO CNH, FF.FUN_FONE Telefone, FF.FUN_VECTOHABILITACAO ValidadeCNH
                                ,FF.FUN_DATA_PRIMEIRA_HABILITACAO PrimeiraCNH, FF.FUN_NUMERO_REGISTRO_HABILITACAO RegistroCNH,  FF.FUN_CATEGORIA CategoriaCNH, FF.FUN_DATAHABILITACAO EmissaoCNH
                                , (select TOP 1 CEM_INFORMACAO_ADICIONAL_MOTORISTA_ORDEM_COLETA from T_CONFIGURACAO_EMBARCADOR) UtilizaInformacoesAdicionionais
                FROM T_PEDIDO_MOTORISTA PP
                JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
                WHERE 1 = 1";
                query += " AND PP.PED_CODIGO = " + codigoPedido.ToString();
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.MotoristasPrestacaoServico>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico> RelatorioONUPedido(int codigoPedido, string codigosPedidos)
        {
            string query = @"SELECT PPO_OBSERVACAO Observacao, C.CRO_NUMERO_ONU NumeroONU, CRO_CLASSE_RISCO ClasseRisco, 
                CRO_RISCO_SUBSIDIARIO RiscoSubsiriario, CRO_NUMERO_RISCO NumeroRisco, CRO_GRUPO_EMBARCADO GrupoEmbarcado, CRO_PROVISOES_ESPECIAIS ProvisoesEspeciais, CRO_LIMITE_KG_VEICULO LimiteKG,
                CRO_LIMITE_LITRO_EMBALAGEM_INTERNA LimiteLT, CRO_EMBALAGEM_INSTRUCAO InstrucaoEmbalagem, CRO_EMBALAGEM_PROVISOES_ESPECIAIS ProvisoesEmbalagem, 
                CRO_TANQUE_INSTRUCAO InstrucaoTanque, CRO_TANQUE_PROVISOES_ESPECIAIS ProvisoesTanque, P.PED_CODIGO CodigoPedido
                FROM T_PEDIDO_PRODUTO_ONU PP
                JOIN T_CLASSIFICACAI_RISCO_ONU C ON C.CRO_CODIGO = PP.CRO_CODIGO
                JOIN T_PEDIDO_PRODUTO P ON P.PRP_CODIGO = PP.PRP_CODIGO
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigosPedidos))
                query += " AND P.PED_CODIGO IN (" + codigosPedidos + ")";
            else if (codigoPedido > 0)
                query += " AND P.PED_CODIGO = " + codigoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ONUPrestacaoServico>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico> RelatorioProdutoPedido(int codigoPedido, string codigosPedidos)
        {
            string query = @"SELECT P.GRP_DESCRICAO Descricao, PP.PRP_PESO_UNITARIO Peso, PRP_QUANTIDADE Quantidade, PRP_QUANTIDADE_PALET Palet,
                PRP_ALTURA_CM Altura, PRP_LARGURA_CM Largura, PRP_COMPRIMENTO_CM Comprimento, PRP_METRO_CUBICO MetroCubico, PRP_OBSERVACAO Observacao, PP.PED_CODIGO CodigoPedido
                FROM T_PEDIDO_PRODUTO PP
                JOIN T_PRODUTO_EMBARCADOR P ON P.PRO_CODIGO = PP.PRO_CODIGO
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigosPedidos))
                query += " AND PP.PED_CODIGO IN (" + codigosPedidos + ")";
            else if (codigoPedido > 0)
                query += " AND PP.PED_CODIGO = " + codigoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ProdutosPrestacaoServico>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico> RelatorioImportacaoPedido(int codigoPedido, string codigosPedidos)
        {
            string query = @"SELECT PP.PEI_NUMERO_DI NumeroDI, PP.PEI_CODIGO_IMPORTACAO CodigoImportacao, PP.PEI_CODIGO_REFERENCIA PontoReferencia,
                 PP.PEI_VALOR_CARGA ValorCarga, PP.PEI_VOLUME Volume, PP.PEI_PESO Peso, P.PED_CODIGO CodigoPedido,
                P.PED_NUMERO_CONTAINER NumeroContainer, P.PED_NUMERO_BL NumeroBL, P.PED_NUMERO_NAVIO NumeroNavio, P.POT_CODIGO_ORIGEM CNPJPorto, C.POT_DESCRICAO NomePorto, 
                P.PED_ENDERECO_ENTREGA_IMPORTACAO EnderecoEntrega, P.PED_BAIRRO_ENTREGA_IMPORTACAO BairroEntrega,
                P.PED_CEP_ENTREGA_IMPORTACAO CEPEntrega, P.PED_PONTO_REFERENCIA_ENTREGA_IMPORTACAO PontoReferenciaEntrega, P.LOC_CODIGO_ENTREGA_IMPORTACAO CodigoCidade, 
                L.LOC_DESCRICAO Cidade, L.UF_SIGLA Estado,
                P.PED_DATA_VENCIMENTO_ARMAZENAMENTO_IMPORTACAO DataVencimento, P.PED_ARMADOR_IMPORTACAO Armador, P.TTI_CODIGO CodigoTerminal, T.TTI_DESCRICAO Terminal
                FROM T_PEDIDO P
                LEFT OUTER JOIN T_PEDIDO_IMPORTACAO PP ON P.PED_CODIGO = PP.PED_CODIGO
                LEFT OUTER JOIN T_PORTO C ON C.POT_CODIGO = P.POT_CODIGO_ORIGEM
                LEFT OUTER JOIN T_LOCALIDADES L ON L.LOC_CODIGO = P.LOC_CODIGO_ENTREGA_IMPORTACAO
                LEFT OUTER JOIN T_TIPO_TERMINAL_IMPORTACAO T ON T.TTI_CODIGO = P.TTI_CODIGO
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigosPedidos))
                query += " AND PP.PED_CODIGO IN (" + codigosPedidos + ")";
            else if (codigoPedido > 0)
                query += " AND PP.PED_CODIGO = " + codigoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.ImportacaoPrestacaoServico>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusiva> RelatorioOrdemColetaExclusivaPedido(int codigoPedido, string codigosPedidos, bool imprimirCarga)
        {
            string queryJoin, queryVeiculo, queryMotorista;
            if (imprimirCarga)
            {
                queryJoin = @"  LEFT OUTER JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CP.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO ";
                queryVeiculo = @" (SELECT TOP 1 VV.VEI_PLACA 
	                                    FROM T_CARGA PV		
	                                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
	                                    JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) Veiculo,
	
                                    ISNULL((SELECT TOP 1 MO.MVC_NUMERO_EIXOS 
	                                    FROM T_CARGA PV		
	                                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
	                                    JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
	                                    LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = VV.MVC_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO), 0) QuantidadeEixoVeiculo,
	
                                    (SELECT TOP 1 VeiculoVinculado.VEI_PLACA 
	                                    FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
	                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
	                                    JOIN T_CARGA PV on VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO	
	                                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
	                                    JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) Reboque,
	
                                    ISNULL((SELECT TOP 1 MO.MVC_NUMERO_EIXOS 
	                                    FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
	                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
	                                    LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = VeiculoVinculado.MVC_CODIGO
	                                    JOIN T_CARGA PV on VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO	
	                                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
	                                    JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO), 0) QuantidadeEixoReboque,";
                queryMotorista = @"(SELECT TOP 1 FF.FUN_CODIGO
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) CodigoMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NOME
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) Motorista,
	
                                    (SELECT TOP 1 FF.FUN_FONE
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) TelefoneMotorista,
	
                                    (SELECT TOP 1 FF.FUN_RG
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) RGMotorista,
	
                                    (SELECT TOP 1 FF.FUN_CPF
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) CPFMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NUMHABILITACAO
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) CNHMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NUMERO_REGISTRO_HABILITACAO
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) NumeroRegistroCNHMotorista,";
            }
            else if (!string.IsNullOrWhiteSpace(codigosPedidos))
            {
                queryJoin = @"  LEFT OUTER JOIN T_CARREGAMENTO_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO CA ON CA.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO_CARGA CAR ON CAR.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARGA CARGA ON CARGA.CAR_CODIGO = CAR.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO ";
                queryVeiculo = @" (SELECT top 1 VV.VEI_PLACA 
	                                FROM T_CARREGAMENTO PV
                                    JOIN T_CARREGAMENTO_PEDIDO CP ON CP.CRG_CODIGO = PV.CRG_CODIGO						
	                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.VEI_CODIGO
	                                WHERE CP.PED_CODIGO = P.PED_CODIGO) Veiculo,
	
                                ISNULL((SELECT top 1 MO.MVC_NUMERO_EIXOS 
	                                FROM T_CARREGAMENTO PV
                                    JOIN T_CARREGAMENTO_PEDIDO CP ON CP.CRG_CODIGO = PV.CRG_CODIGO						
	                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.VEI_CODIGO
	                                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = VV.MVC_CODIGO
	                                WHERE CP.PED_CODIGO = P.PED_CODIGO), 0) QuantidadeEixoVeiculo,
	
                                '' Reboque,	
                                0 QuantidadeEixoReboque, ";
                queryMotorista = @"(SELECT TOP 1 FF.FUN_CODIGO
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) CodigoMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NOME
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) Motorista,
	
                                    (SELECT TOP 1 FF.FUN_FONE
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) TelefoneMotorista,
	
                                    (SELECT TOP 1 FF.FUN_RG
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) RGMotorista,
	
                                    (SELECT TOP 1 FF.FUN_CPF
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) CPFMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NUMHABILITACAO
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) CNHMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NUMERO_REGISTRO_HABILITACAO
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) NumeroRegistroCNHMotorista,";
            }
            else
            {
                queryJoin = @"  LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = ISNULL(P.EMP_CODIGO, FP.EMP_CODIGO)
                                LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = P.VEI_CODIGO
								LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO";
                queryVeiculo = @" CASE WHEN Veiculo.VEI_CODIGO IS NOT NULL THEN Veiculo.VEI_PLACA 
                                    ELSE (SELECT top 1 VV.VEI_PLACA 
	                                    FROM T_PEDIDO_VEICULO _Pedido
                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = _Pedido.VEI_CODIGO
	                                    WHERE VV.VEI_TIPOVEICULO = 0 AND _Pedido.PED_CODIGO = P.PED_CODIGO) END Veiculo,

                                  CASE WHEN Veiculo.VEI_CODIGO IS NOT NULL THEN ModeloVeicular.MVC_NUMERO_EIXOS
                                    ELSE ISNULL((SELECT top 1 MO.MVC_NUMERO_EIXOS 
	                                    FROM T_PEDIDO_VEICULO _Pedido
                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = _Pedido.VEI_CODIGO
	                                    LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = VV.MVC_CODIGO
	                                    WHERE VV.VEI_TIPOVEICULO = 0 AND _Pedido.PED_CODIGO = P.PED_CODIGO), 0) END QuantidadeEixoVeiculo,
	
                                  CASE WHEN Veiculo.VEI_CODIGO IS NOT NULL THEN Veiculo.VEI_PLACA 
                                    ELSE (SELECT top 1 VV.VEI_PLACA 
	                                    FROM T_PEDIDO_VEICULO _Pedido
                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = _Pedido.VEI_CODIGO
	                                    WHERE VV.VEI_TIPOVEICULO = 1 AND _Pedido.PED_CODIGO = P.PED_CODIGO) END Reboque,

                                  CASE WHEN Veiculo.VEI_CODIGO IS NOT NULL THEN ModeloVeicular.MVC_NUMERO_EIXOS
                                    ELSE ISNULL((SELECT top 1 MO.MVC_NUMERO_EIXOS 
	                                    FROM T_PEDIDO_VEICULO _Pedido
                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = _Pedido.VEI_CODIGO
	                                    LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MO ON MO.MVC_CODIGO = VV.MVC_CODIGO
	                                    WHERE VV.VEI_TIPOVEICULO = 1 AND _Pedido.PED_CODIGO = P.PED_CODIGO), 0) END QuantidadeEixoReboque, ";

                //'' Reboque,	
                //0 QuantidadeEixoReboque, ";
                queryMotorista = @"(SELECT TOP 1 FF.FUN_CODIGO
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) CodigoMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NOME
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) Motorista,
	
                                    (SELECT TOP 1 FF.FUN_FONE
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) TelefoneMotorista,
	
                                    (SELECT TOP 1 FF.FUN_RG
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) RGMotorista,
	
                                    (SELECT TOP 1 FF.FUN_CPF
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) CPFMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NUMHABILITACAO
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) CNHMotorista,
	
                                    (SELECT TOP 1 FF.FUN_NUMERO_REGISTRO_HABILITACAO
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) NumeroRegistroCNHMotorista,";
            }

            string query = @"   SELECT P.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador,
                                P.PED_DATA_AGENDAMENTO DataAgendamento,
                                P.CAR_DATA_CARREGAMENTO_PEDIDO DataColeta,

                                " + queryMotorista + @"
                                " + queryVeiculo + @"
                                
                                CASE	
	                                WHEN P.PED_TIPO_PESSOA = 1 THEN CR.CLI_NOME
	                                ELSE GR.GRP_DESCRICAO
                                END Remetente, 
                                (LOREM.LOC_DESCRICAO + ' - ' + LOREM.UF_SIGLA) CidadeRemetente,

                                CD.CLI_NOME Destinatario,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 1 THEN (LODEST.LOC_DESCRICAO + ' - ' + LODEST.UF_SIGLA)
	                                ELSE (LOCDEST.LOC_DESCRICAO + ' - ' + LODEST.UF_SIGLA)
                                END CidadeDestino
                                                                
                                FROM T_PEDIDO P
                                JOIN T_LOCALIDADES LO ON LO.LOC_CODIGO = P.LOC_CODIGO_ORIGEM
                                JOIN T_FUNCIONARIO FP ON FP.FUN_CODIGO = P.FUN_CODIGO 
                                " + queryJoin + @" 
                                LEFT OUTER JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CR ON CR.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_LOCALIDADES LOREM ON LOREM.LOC_CODIGO = CR.LOC_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GR ON GR.GRP_CODIGO = P.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CD ON CD.CLI_CGCCPF = P.CLI_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LOCDEST ON LOCDEST.LOC_CODIGO = CD.LOC_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = P.LOC_CODIGO_DESTINO

                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PO ON PO.PEN_CODIGO = P.PEN_CODIGO_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LORIGEM ON LORIGEM.LOC_CODIGO = PO.LOC_CODIGO
                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PD ON PD.PEN_CODIGO = P.PEN_CODIGO_DESTINO
                                LEFT OUTER JOIN T_LOCALIDADES LODEST ON LODEST.LOC_CODIGO = PD.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE TOM ON TOM.CLI_CGCCPF = P.CLI_CODIGO_TOMADOR
								LEFT OUTER JOIN T_LOCALIDADES LTOM ON LTOM.LOC_CODIGO = TOM.LOC_CODIGO
                                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigosPedidos))
                query += " AND P.PED_CODIGO IN (" + codigosPedidos + ")";
            else if (codigoPedido > 0)
                query += " AND P.PED_CODIGO = " + codigoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusiva)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusiva>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusivaPaisagem> RelatorioOrdemColetaExclusivaPaisagemPedido(int codigoPedido, string codigosPedidos, bool imprimirCarga)
        {
            string queryJoin, querySelect;
            if (imprimirCarga)
            {
                queryJoin = @"  LEFT OUTER JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CP.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO
                                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = CA.MVC_CODIGO";
                querySelect = @" (SELECT TOP 1 VV.VEI_PLACA 
	                                    FROM T_CARGA PV		
	                                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
	                                    JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) Veiculo,
	
                                    ISNULL((SELECT TOP 1 VV.UF_SIGLA 
	                                    FROM T_CARGA PV		
	                                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO	
	                                    JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO), 0) EstadoVeiculo,
	
                                    (SELECT TOP 1 VeiculoVinculado.VEI_PLACA 
	                                    FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
	                                    JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
	                                    JOIN T_CARGA PV on VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO	
	                                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
	                                    JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) Reboque,";
                querySelect += @"(SELECT TOP 1 FF.FUN_NOME
	                                    FROM T_CARGA_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.CAR_MOTORISTA
	                                    WHERE PP.CAR_CODIGO = CA.CAR_CODIGO) Motorista,";
                querySelect += "ModeloVeicular.MVC_DESCRICAO ModeloVeicularCarga,";
            }
            else if (!string.IsNullOrWhiteSpace(codigosPedidos))
            {
                queryJoin = @"  LEFT OUTER JOIN T_CARREGAMENTO_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO CA ON CA.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO_CARGA CAR ON CAR.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARGA CARGA ON CARGA.CAR_CODIGO = CAR.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO
                                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = CARGA.MVC_CODIGO";
                querySelect = @" (SELECT top 1 VV.VEI_PLACA 
	                                FROM T_CARREGAMENTO PV
                                    JOIN T_CARREGAMENTO_PEDIDO CP ON CP.CRG_CODIGO = PV.CRG_CODIGO						
	                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.VEI_CODIGO
	                                WHERE CP.PED_CODIGO = P.PED_CODIGO) Veiculo,
	
                                ISNULL((SELECT top 1 VV.UF_SIGLA 
	                                FROM T_CARREGAMENTO PV
                                    JOIN T_CARREGAMENTO_PEDIDO CP ON CP.CRG_CODIGO = PV.CRG_CODIGO
	                                JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.VEI_CODIGO
	                                WHERE CP.PED_CODIGO = P.PED_CODIGO), 0) EstadoVeiculo,
	
                                '' Reboque, ";
                querySelect += @"(SELECT TOP 1 FF.FUN_NOME
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = CA.PED_CODIGO) Motorista,";
                querySelect += "ModeloVeicular.MVC_DESCRICAO ModeloVeicularCarga,";
            }
            else
            {
                queryJoin = @"  LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = ISNULL(P.EMP_CODIGO, FP.EMP_CODIGO)
                                LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = P.VEI_CODIGO
								LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = P.MVC_CODIGO";
                querySelect = @" CASE WHEN Veiculo.VEI_CODIGO IS NOT NULL THEN Veiculo.VEI_PLACA 
                                    ELSE(SELECT top 1 VV.VEI_PLACA 
	                                    FROM T_PEDIDO_VEICULO _Pedido
                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = _Pedido.VEI_CODIGO
	                                    WHERE _Pedido.PED_CODIGO = P.PED_CODIGO) END Veiculo,

                                 CASE WHEN Veiculo.VEI_CODIGO IS NOT NULL THEN Veiculo.VEI_PLACA 
                                    ELSE ISNULL((SELECT top 1 VV.UF_SIGLA 
	                                    FROM T_PEDIDO_VEICULO _Pedido
                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = _Pedido.VEI_CODIGO
	                                    WHERE _Pedido.PED_CODIGO = P.PED_CODIGO), '') END EstadoVeiculo,
	
                                    '' Reboque, ";
                querySelect += @"(SELECT TOP 1 FF.FUN_NOME
	                                    FROM T_PEDIDO_MOTORISTA PP
	                                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
	                                    WHERE PP.PED_CODIGO = P.PED_CODIGO) Motorista, ";
                querySelect += "ModeloVeicular.MVC_DESCRICAO ModeloVeicularCarga,";
            }

            string query = @"   SELECT P.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador,
                                P.PED_QUANTIDADE_VOLUMES Volume,
                                P.CAR_DATA_CARREGAMENTO_PEDIDO DataColeta,
                                P.PED_PESO_TOTAL_CARGA PesoTotal,
                                P.PED_OBSERVACAO Observacao,

                                " + querySelect + @"
                                
                                CASE	
	                                WHEN P.PED_TIPO_PESSOA = 1 THEN CR.CLI_NOME
	                                ELSE GR.GRP_DESCRICAO
                                END Remetente, 
                                (LOREM.LOC_DESCRICAO + ' - ' + LOREM.UF_SIGLA) CidadeRemetente,
                                CR.CLI_BAIRRO BairroRemetente,
                                CR.CLI_ENDERECO EnderecoRemetente,

                                CD.CLI_NOME Destinatario,
                                Fronteira.CLI_NOME Fronteira
                                                                
                                FROM T_PEDIDO P
                                JOIN T_LOCALIDADES LO ON LO.LOC_CODIGO = P.LOC_CODIGO_ORIGEM
                                JOIN T_FUNCIONARIO FP ON FP.FUN_CODIGO = P.FUN_CODIGO 
                                " + queryJoin + @" 
                                LEFT OUTER JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CR ON CR.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_LOCALIDADES LOREM ON LOREM.LOC_CODIGO = CR.LOC_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GR ON GR.GRP_CODIGO = P.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CD ON CD.CLI_CGCCPF = P.CLI_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LOCDEST ON LOCDEST.LOC_CODIGO = CD.LOC_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = P.LOC_CODIGO_DESTINO

                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PO ON PO.PEN_CODIGO = P.PEN_CODIGO_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LORIGEM ON LORIGEM.LOC_CODIGO = PO.LOC_CODIGO
                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PD ON PD.PEN_CODIGO = P.PEN_CODIGO_DESTINO
                                LEFT OUTER JOIN T_LOCALIDADES LODEST ON LODEST.LOC_CODIGO = PD.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE TOM ON TOM.CLI_CGCCPF = P.CLI_CODIGO_TOMADOR
								LEFT OUTER JOIN T_LOCALIDADES LTOM ON LTOM.LOC_CODIGO = TOM.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE Fronteira ON Fronteira.CLI_CGCCPF = P.CLI_CGCCPF_FRONTEIRA
                                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigosPedidos))
                query += " AND P.PED_CODIGO IN (" + codigosPedidos + ")";
            else if (codigoPedido > 0)
                query += " AND P.PED_CODIGO = " + codigoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusivaPaisagem)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaExclusivaPaisagem>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificada> RelatorioOrdemColetaSimplificada(int codigoPedido, string codigosPedidos, bool imprimirCarga)
        {
            string queryJoin, querySelect;
            if (imprimirCarga)
            {
                string fromJoinVeiculo = @" FROM T_CARGA PV		
	                                        JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO						
	                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.CAR_VEICULO";
                string fromJoinReboque = @" FROM T_CARGA_VEICULOS_VINCULADOS VeiculoConjunto
	                                        JOIN T_VEICULO VeiculoVinculado ON VeiculoVinculado.VEI_CODIGO = VeiculoConjunto.VEI_CODIGO
	                                        JOIN T_CARGA PV on VeiculoConjunto.CAR_CODIGO = PV.CAR_CODIGO	
	                                        JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = PV.CAR_CODIGO	";

                queryJoin = @"  LEFT OUTER JOIN T_CARGA_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CP.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO ";

                querySelect = $@" (SELECT TOP 1 VV.VEI_PLACA 
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) Veiculo,

                                    (SELECT TOP 1 VV.VEI_RENAVAM 
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoRenavam,

                                    (SELECT TOP 1 Rastreador.TRA_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_RASTREADOR_TECNOLOGIA Rastreador ON Rastreador.TRA_CODIGO = VV.TRA_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoRastreador,

                                    (SELECT TOP 1 TipoComunicacaoRastreador.TCR_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_RASTREADOR_TIPO_COMUNICACAO TipoComunicacaoRastreador ON TipoComunicacaoRastreador.TCR_CODIGO = VV.TCR_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoTipoRastreador,

                                    (SELECT TOP 1 Marca.VMA_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = VV.VMA_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoMarca,

                                    (SELECT TOP 1 Modelo.VMO_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = VV.VMO_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoModelo,

                                    (SELECT TOP 1 CAST(VV.VEI_ANO AS NVARCHAR(20))
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoAnoFabricacao,

                                    (SELECT TOP 1 CAST(VV.VEI_ANOMODELO AS NVARCHAR(20))
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoAnoModelo,

                                    (SELECT TOP 1 VV.VEI_CHASSI 
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoChassi,

                                    (SELECT TOP 1 CAST(VV.VEI_TARA AS NVARCHAR(20))
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoTara,
	
                                    (SELECT TOP 1 VeiculoVinculado.VEI_PLACA 
                                        {fromJoinReboque}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) Reboque,

                                    (SELECT TOP 1 VeiculoVinculado.VEI_RENAVAM 
	                                    {fromJoinReboque}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) ReboqueRenavam,

                                    (SELECT TOP 1 Marca.VMA_DESCRICAO 
	                                    {fromJoinReboque}
                                        JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = VeiculoVinculado.VMA_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) ReboqueMarca,

                                    (SELECT TOP 1 Modelo.VMO_DESCRICAO 
	                                    {fromJoinReboque}
                                        JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = VeiculoVinculado.VMO_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) ReboqueModelo,

                                    (SELECT TOP 1 CAST(VeiculoVinculado.VEI_ANO AS NVARCHAR(20))
	                                    {fromJoinReboque}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) ReboqueAnoFabricacao,

                                    (SELECT TOP 1 CAST(VeiculoVinculado.VEI_ANOMODELO AS NVARCHAR(20))
	                                    {fromJoinReboque}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) ReboqueAnoModelo,

                                    (SELECT TOP 1 VeiculoVinculado.VEI_CHASSI 
	                                    {fromJoinReboque}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) ReboqueChassi,

                                    (SELECT TOP 1 CAST(VeiculoVinculado.VEI_TARA AS NVARCHAR(20))
	                                    {fromJoinReboque}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) ReboqueTara,";
            }
            else if (!string.IsNullOrWhiteSpace(codigosPedidos))
            {
                string fromJoinVeiculo = @" FROM T_CARREGAMENTO PV
                                            JOIN T_CARREGAMENTO_PEDIDO CP ON CP.CRG_CODIGO = PV.CRG_CODIGO						
	                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = PV.VEI_CODIGO";

                queryJoin = @" LEFT OUTER JOIN T_CARREGAMENTO_PEDIDO CP ON CP.PED_CODIGO = P.PED_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO CA ON CA.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARREGAMENTO_CARGA CAR ON CAR.CRG_CODIGO = CP.CRG_CODIGO
                                LEFT OUTER JOIN T_CARGA CARGA ON CARGA.CAR_CODIGO = CAR.CAR_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = CA.EMP_CODIGO ";
                querySelect = $@" (SELECT top 1 VV.VEI_PLACA 
	                                {fromJoinVeiculo}
	                                WHERE CP.PED_CODIGO = P.PED_CODIGO) Veiculo,

                                    (SELECT TOP 1 VV.VEI_RENAVAM 
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoRenavam,

                                    (SELECT TOP 1 Rastreador.TRA_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_RASTREADOR_TECNOLOGIA Rastreador ON Rastreador.TRA_CODIGO = VV.TRA_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoRastreador,

                                    (SELECT TOP 1 TipoComunicacaoRastreador.TCR_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_RASTREADOR_TIPO_COMUNICACAO TipoComunicacaoRastreador ON TipoComunicacaoRastreador.TCR_CODIGO = VV.TCR_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoTipoRastreador,

                                    (SELECT TOP 1 Marca.VMA_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = VV.VMA_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoMarca,

                                    (SELECT TOP 1 Modelo.VMO_DESCRICAO 
	                                    {fromJoinVeiculo}
                                        JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = VV.VMO_CODIGO
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoModelo,

                                    (SELECT TOP 1 CAST(VV.VEI_ANO AS NVARCHAR(20))
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoAnoFabricacao,

                                    (SELECT TOP 1 CAST(VV.VEI_ANOMODELO AS NVARCHAR(20))
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoAnoModelo,

                                    (SELECT TOP 1 VV.VEI_CHASSI 
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoChassi,

                                    (SELECT TOP 1 CAST(VV.VEI_TARA AS NVARCHAR(20))
	                                    {fromJoinVeiculo}
	                                    WHERE CP.PED_CODIGO = P.PED_CODIGO) VeiculoTara,";
            }
            else
            {
                queryJoin = @"  LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = ISNULL(P.EMP_CODIGO, FP.EMP_CODIGO)
                                LEFT OUTER JOIN T_VEICULO VV ON VV.VEI_CODIGO = P.VEI_CODIGO
                                LEFT OUTER JOIN T_RASTREADOR_TECNOLOGIA Rastreador ON Rastreador.TRA_CODIGO = VV.TRA_CODIGO
                                LEFT OUTER JOIN T_RASTREADOR_TIPO_COMUNICACAO TipoComunicacaoRastreador ON TipoComunicacaoRastreador.TCR_CODIGO = VV.TCR_CODIGO
                                LEFT OUTER JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = VV.VMA_CODIGO
                                LEFT OUTER JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = VV.VMO_CODIGO";

                querySelect = @"VV.VEI_PLACA Veiculo,
	                            VV.VEI_RENAVAM VeiculoRenavam,
                                Rastreador.TRA_DESCRICAO VeiculoRastreador,
                                TipoComunicacaoRastreador.TCR_DESCRICAO VeiculoTipoRastreador,
                                Marca.VMA_DESCRICAO VeiculoMarca,
                                Modelo.VMO_DESCRICAO VeiculoModelo,
                                CAST(VV.VEI_ANO AS NVARCHAR(20)) VeiculoAnoFabricacao,
                                CAST(VV.VEI_ANOMODELO AS NVARCHAR(20)) VeiculoAnoModelo,
                                VV.VEI_CHASSI VeiculoChassi,
                                CAST(VV.VEI_TARA AS NVARCHAR(20)) VeiculoTara,";
            }

            string query = @"   SELECT P.PED_CODIGO CodigoPedido,
                                P.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador,
                                P.CAR_DATA_CARREGAMENTO_PEDIDO DataColeta,
                                P.PED_OBSERVACAO Observacao,

                                " + querySelect + @"
                                
                                CASE	
	                                WHEN P.PED_TIPO_PESSOA = 1 THEN CR.CLI_NOME
	                                ELSE GR.GRP_DESCRICAO
                                END Remetente, 
                                (LOREM.LOC_DESCRICAO + ' - ' + LOREM.UF_SIGLA) RemetenteCidade,

                                CD.CLI_NOME Destinatario,
                                CASE
	                                WHEN P.PED_USAR_OUTRO_ENDERECO_DESTINO = 1 THEN (LODEST.LOC_DESCRICAO + ' - ' + LODEST.UF_SIGLA)
	                                ELSE (LOCDEST.LOC_DESCRICAO + ' - ' + LODEST.UF_SIGLA)
                                END DestinatarioCidade
                                                                
                                FROM T_PEDIDO P
                                JOIN T_LOCALIDADES LO ON LO.LOC_CODIGO = P.LOC_CODIGO_ORIGEM
                                JOIN T_FUNCIONARIO FP ON FP.FUN_CODIGO = P.FUN_CODIGO 
                                " + queryJoin + @" 
                                LEFT OUTER JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CR ON CR.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_LOCALIDADES LOREM ON LOREM.LOC_CODIGO = CR.LOC_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GR ON GR.GRP_CODIGO = P.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CD ON CD.CLI_CGCCPF = P.CLI_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LOCDEST ON LOCDEST.LOC_CODIGO = CD.LOC_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = P.LOC_CODIGO_DESTINO

                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PO ON PO.PEN_CODIGO = P.PEN_CODIGO_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LORIGEM ON LORIGEM.LOC_CODIGO = PO.LOC_CODIGO
                                LEFT OUTER JOIN T_PEDIDO_ENDERECO PD ON PD.PEN_CODIGO = P.PEN_CODIGO_DESTINO
                                LEFT OUTER JOIN T_LOCALIDADES LODEST ON LODEST.LOC_CODIGO = PD.LOC_CODIGO
                                LEFT OUTER JOIN T_CLIENTE TOM ON TOM.CLI_CGCCPF = P.CLI_CODIGO_TOMADOR
								LEFT OUTER JOIN T_LOCALIDADES LTOM ON LTOM.LOC_CODIGO = TOM.LOC_CODIGO                                
                                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigosPedidos))
                query += " AND P.PED_CODIGO IN (" + codigosPedidos + ")";
            else if (codigoPedido > 0)
                query += " AND P.PED_CODIGO = " + codigoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificada)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificada>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificadaMotorista> RelatorioOrdemColetaSimplificadaMotorista(int codigoPedido, string codigosPedidos, bool impressaoCarga)
        {
            string query = "";

            if (impressaoCarga)
            {
                query = @"SELECT FF.FUN_CPF CPF, FF.FUN_NOME Nome, FF.FUN_RG RG, FF.FUN_FONE Telefone, PP.PED_CODIGO CodigoPedido 
					FROM T_CARGA_MOTORISTA M
					JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = M.CAR_MOTORISTA
					JOIN T_CARGA_PEDIDO PP ON PP.CAR_CODIGO = M.CAR_CODIGO
                    WHERE 1 = 1";
                query += " AND PP.PED_CODIGO IN (" + codigosPedidos + ")";
            }
            else if (!string.IsNullOrWhiteSpace(codigosPedidos))
            {
                query = @"SELECT FF.FUN_CPF CPF, FF.FUN_NOME Nome, FF.FUN_RG RG, FF.FUN_FONE Telefone, PP.PED_CODIGO CodigoPedido 
					FROM T_CARREGAMENTO_MOTORISTAS M
					JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = M.FUN_CODIGO
					JOIN T_CARREGAMENTO_PEDIDO PP ON PP.CRG_CODIGO = M.CRG_CODIGO
                    WHERE 1 = 1";
                query += " AND PP.PED_CODIGO IN (" + codigosPedidos + ")";
            }
            else if (codigoPedido > 0)
            {
                query = @"SELECT FF.FUN_CPF CPF, FF.FUN_NOME Nome, FF.FUN_RG RG, FF.FUN_FONE Telefone, PP.PED_CODIGO CodigoPedido
                    FROM T_PEDIDO_MOTORISTA PP
                    JOIN T_FUNCIONARIO FF ON FF.FUN_CODIGO = PP.FUN_CODIGO
                    WHERE 1 = 1";
                query += " AND PP.PED_CODIGO = " + codigoPedido.ToString();
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificadaMotorista)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.OrdemColetaSimplificadaMotorista>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDevolucao> ConsultarRelatorioPedidoDevolucao(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDevolucao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPedido = new ConsultaPedidoDevolucao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPedido.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDevolucao)));

            return consultaPedido.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDevolucao>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoRetornoOcorrencia> ConsultarRelatorioPedidoRetornoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaPedidoRetornoOcorrencia().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoRetornoOcorrencia)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoRetornoOcorrencia>();
        }

        public int ContarConsultaRelatorioPedidoRetornoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaPedidoRetornoOcorrencia().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaRelatorioPedidoDevolucao(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDevolucao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPedido = new ConsultaPedidoDevolucao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPedido.SetTimeout(600).UniqueResult<int>();
        }

        public string BuscarTaraContinar(int codigoContainer, List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(obj => obj.Pedido.Container.Codigo == codigoContainer);

            var queryCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCte = queryCte.Where(obj => codigosCTes.Contains(obj.CTe.Codigo));
            query = query.Where(obj => queryCte.Any(c => c.Carga == obj.Carga));

            return query.Select(o => o.Pedido.TaraContainer).FirstOrDefault();
        }

        public int AdicionarInformacaoContatoCliente(List<int> codigosPedidos, string usuario, string informacaoContato)
        {
            List<string> valores = new List<string>();

            for (int i = 0; i < codigosPedidos.Count; i++)
                valores.Add($" ('{informacaoContato}', '{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}', '{usuario}', '', '', {codigosPedidos[i]})");

            string sql = $"INSERT INTO T_AGENDAMENTO_ENTREGA_PEDIDO_CLIENTE_ANEXOS (ANX_DESCRICAO, ANX_DATA_CADASTRO_ARQUIVO, ANX_USUARIO_CADASTRO_ARQUIVO, ANX_NOME_ARQUIVO, ANX_GUID_ARQUIVO, PED_CODIGO) VALUES {string.Join(", ", valores)}"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.ExecuteUpdate();
        }

        public string BuscarProcImportacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(obj => obj.CargasPedido.Any(o => o.Codigo == codigoCarga));

            return query.Select(o => o.Adicional1).FirstOrDefault();
        }

        public bool VerificarExistenciaPedido(string numeroPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = ObterQueryVerificarExistenciaPedidoPorNumeroPedidoEmbarcador(numeroPedido);

            query = query.Where(obj => obj.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(pedido => pedido.Codigo).Any();
        }

        public Task<bool> VerificarExistenciaPedidoAsync(string numeroPedidoEmbarcador, SituacaoPedido situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = ObterQueryVerificarExistenciaPedidoPorNumeroPedidoEmbarcador(numeroPedidoEmbarcador);

            query = query.Where(obj => obj.SituacaoPedido == situacao);

            return query.Select(pedido => pedido.Codigo).AnyAsync(CancellationToken);
        }

        public int BuscarProtocoloPorNumeroPedido(string numeroPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(obj => obj.NumeroPedidoEmbarcador == numeroPedido && obj.SituacaoPedido != SituacaoPedido.Cancelado);

            return query.Select(o => o.Protocolo).FirstOrDefault();
        }

        public bool VerificarPedidoDestinadoAFilial(int codigoPedido)
        {
            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(filial => filial.Ativo == true);

            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido => pedido.Codigo == codigoPedido && consultaFilial.Any(filial => double.Parse(filial.CNPJ) == pedido.Destinatario.CPF_CNPJ));

            return consultaPedido.Any();
        }

        public List<int> BuscarCodigosDosPedidosDestinadosAFiliais(List<int> codigosPedido)
        {
            var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                .Where(filial => filial.Ativo == true);

            var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido => codigosPedido.Contains(pedido.Codigo) && consultaFilial.Any(filial => double.Parse(filial.CNPJ) == pedido.Destinatario.CPF_CNPJ));

            return consultaPedido.Select(pedido => pedido.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarAtendimentoPedidoChat(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = ConsultarAtendimentoPedidoComChat(filtrosPesquisa);
            return ObterLista(query, parametrosConsulta);
        }

        public IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarAtendimentoPedidoComChat(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedido filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            query = query.Where(obj => obj.SituacaoPedido != SituacaoPedido.Cancelado && obj.SituacaoPedido != SituacaoPedido.Rejeitado);

            if (filtrosPesquisa.Remetentes?.Count > 0)
                query = query.Where(obj => filtrosPesquisa.Remetentes.Contains(obj.Remetente.CPF_CNPJ));

            if (filtrosPesquisa.NumeroNotasFiscais?.Count > 0)
                query = query.Where(obj => obj.PedidoNotasParciais.Any(nf => filtrosPesquisa.NumeroNotasFiscais.Contains(nf.Numero)) || obj.PedidosCarga.Any(cp => cp.NotasFiscais.Any(nf => filtrosPesquisa.NumeroNotasFiscais.Contains(nf.XMLNotaFiscal.Numero))) || obj.NotasFiscais.Any(nf => filtrosPesquisa.NumeroNotasFiscais.Contains(nf.Numero)));

            if (filtrosPesquisa.DataFinal.HasValue && filtrosPesquisa.DataFinal.Value != DateTime.MinValue)
                query = query.Where(obj => obj.DataCriacao <= filtrosPesquisa.DataFinal.Value);

            if (filtrosPesquisa.DataInicial.HasValue && filtrosPesquisa.DataInicial.Value != DateTime.MinValue)
                query = query.Where(obj => obj.DataCriacao > filtrosPesquisa.DataInicial.Value);

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroPedido))
                query = query.Where(obj => obj.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedido);

            if (filtrosPesquisa.ApenasPedidosComChat)
                query = query.Where(obj => obj.PossuiChatAtivo == true);

            return query;
        }

        public bool PossuiPedidosAtrasados(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            if (!(centroCarregamento.NaoPermitirGerarCarregamentosQuandoExistirPedidosAtrasadosAgendamentoPedidos && centroCarregamento.DiasAtrasoPermitidosPedidosAgendamentoPedidos > 0))
                return false;

            var consultaPedido = Consultar(filtrosPesquisa);

            consultaPedido = consultaPedido.Where(p => p.DataCriacao < DateTime.Now.Date.AddDays(-centroCarregamento.DiasAtrasoPermitidosPedidosAgendamentoPedidos));

            return consultaPedido.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorNumeroCarregamentoPedido(string numeroCarregamentoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(o => o.NumeroCarregamento == numeroCarregamentoPedido && !o.PedidoTotalmenteCarregado);

            return query.Take(50).ToList();
        }

        public bool ExistePedidoComNumeroCarregamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(obj => obj.NumeroCarregamento != null && obj.NumeroCarregamento != "").Select(obj => obj.Codigo);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosComEntregaPendente(DateTime dataDiaAnterior)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var result = from obj in query
                         where (obj.SituacaoAcompanhamentoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.Entregue ||
                                obj.SituacaoAcompanhamentoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.EntregaRejeitada ||
                                obj.SituacaoAcompanhamentoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.EntregaParcial) &&
                               (obj.PrevisaoEntrega.Value.Date == dataDiaAnterior)
                         select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorPedidoNumeroCarregamento(string pedidoNumeroCarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(o => o.NumeroCarregamento == pedidoNumeroCarregamento);

            return query.FirstOrDefault();
        }

        public List<int> BuscarCodigosRegiaoDestinoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(obj => obj.RegiaoDestino != null && obj.CargasPedido.Any(o => o.Codigo == codigoCarga));

            return query.Select(o => o.RegiaoDestino.Codigo).Distinct().ToList();
        }

        public Task<List<int>> BuscarCodigosRegiaoDestinoPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(obj => obj.RegiaoDestino != null && obj.CargasPedido.Any(o => o.Codigo == codigoCarga));

            return query.Select(o => o.RegiaoDestino.Codigo).Distinct().ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPrimeiroPorNumeroStage(string numeroStage)
        {
            var queryPedidoStage = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();

            return queryPedidoStage.Where(ps => ps.Stage.NumeroStage.Equals(numeroStage)).Select(ps => ps.Pedido).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosLiberacaoPortalAgendamentoRetira(List<int> codigosFiliais, DateTime dataCriacaoPedido, TimeSpan horarioMaximo, int situacao, bool naoExibirPedidosDoDiaAgendamentoPedidos)
        {
            DateTime dataHoraFiltro = dataCriacaoPedido.Date;
            dataHoraFiltro = dataHoraFiltro.AddHours(horarioMaximo.Hours).AddMinutes(horarioMaximo.Minutes);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            query = query.Where(o => codigosFiliais.Contains(o.Filial.Codigo) && o.DataCriacao.Value.Date == dataCriacaoPedido.Date && o.DataCriacao.Value <= dataHoraFiltro);

            if (situacao == 0)
                query = query.Where(x => !x.PedidoLiberadoPortalRetira);
            else if (situacao == 1)
                query = query.Where(x => x.PedidoLiberadoPortalRetira);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorNumeroPedidoCliente(string numeroPedidoCliente)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(x => x.CodigoPedidoCliente == numeroPedidoCliente && x.SituacaoPedido != SituacaoPedido.Cancelado).FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPendente> GerarRelatorioPedidosPendentes(string codigoPedido, double codigoDestinatario)
        {
            string sql = @"SELECT 
                              pedido.PED_CODIGO AS Codigo, 
                              pedido.PED_NUMERO_PEDIDO_EMBARCADOR AS NumeroPedidoEmbarcador, 
                              tipoOperacao.TOP_DESCRICAO AS TipoOperacao, 
                              ISNULL(
                                (
                                  SELECT 
                                    TOP 1 centroDescarregamento.CED_DESCRICAO 
                                  FROM 
                                    T_CENTRO_DESCARREGAMENTO AS centroDescarregamento 
                                    INNER JOIN T_CLIENTE AS destinatarioCentro ON centroDescarregamento.CLI_CGCCPF_DESTINATARIO = destinatarioCentro.CLI_CGCCPF 
                                    LEFT JOIN T_FILIAL AS filial ON centroDescarregamento.FIL_CODIGO = filial.FIL_CODIGO 
                                  WHERE 
                                    destinatarioCentro.CLI_CGCCPF = ISNULL(pedido.CLI_CODIGO, 0) 
                                    AND (
                                      filial.FIL_CODIGO = ISNULL(pedido.FIL_CODIGO, 0) 
                                      OR filial.FIL_CODIGO IS NULL
                                    ) 
                                  ORDER BY 
                                    CASE WHEN filial.FIL_CODIGO IS NOT NULL THEN 1 ELSE 0 END DESC),'') AS DescricaoFilial, 
                              pedido.PED_QUANTIDADE_VOLUMES AS QtVolumes, 
                              pedido.PED_SALDO_VOLUMES_RESTANTE AS Saldo, 
                              ISNULL(
                                grupoProdutoEmbarcador.GRP_DESCRICAO,'') AS GrupoProduto, 
                              ISNULL(
                                CONVERT(
                                  VARCHAR(10), 
                                  pedido.PED_DATA_VALIDADE, 
                                  103
                                ) + ' 23:59:59','') AS DataFimJanelaDescarga, 
                              CONVERT(
                                VARCHAR(16), 
                                pedido.PED_DATA_INICIO_JANELA_DESCARGA, 
                                103
                              ) + ' ' + CONVERT(
                                VARCHAR(5), 
                                pedido.PED_DATA_INICIO_JANELA_DESCARGA, 
                                108
                              ) AS DataInicioJanelaDescarga, 
                              ISNULL(
                                produtoEmbarcador.GRP_DESCRICAO,'') AS Categoria, 
                              (
                                SELECT 
                                  (
                                    pedido.PED_QUANTIDADE_VOLUMES - subquery.quantidade
                                  ) 
                                FROM 
                                  (
                                    SELECT 
                                      CASE WHEN fluxo.UsarLayoutAgendamentoPorCaixaItem = 1 THEN 0 ELSE ISNULL(
                                        (
                                          SELECT 
                                            SUM(APP_QUANTIDADE) 
                                          FROM 
                                            T_AGENDAMENTO_COLETA_PEDIDO as agendamentoPedido 
                                            LEFT JOIN T_AGENDAMENTO_COLETA_PEDIDO_PRODUTO AS agendamentoPedidoProduto ON agendamentoPedidoProduto.ACP_CODIGO = agendamentoPedido.ACP_CODIGO 
                                            LEFT JOIN T_AGENDAMENTO_COLETA AS agendamento ON agendamento.ACO_CODIGO = agendamentoPedido.ACO_CODIGO 
                                          WHERE 
                                            agendamentoPedido.PED_CODIGO = pedido.PED_CODIGO 
                                            AND agendamento.ACO_SITUACAO IN (2, 3)
                                        ), 
                                        0
                                      ) END AS quantidade
                                  ) AS subquery
                              ) AS QtProdutos

                            FROM 
                              T_PEDIDO AS pedido ";

            sql += @"LEFT JOIN T_TIPO_OPERACAO tipoOperacao ON pedido.TOP_CODIGO = tipoOperacao.TOP_CODIGO 
                      LEFT JOIN T_PRODUTO_EMBARCADOR produtoEmbarcador ON pedido.PRO_CODIGO = produtoEmbarcador.PRO_CODIGO 
                      LEFT JOIN T_GRUPO_PRODUTO grupoProdutoEmbarcador ON grupoProdutoEmbarcador.GPR_CODIGO = produtoEmbarcador.GRP_CODIGO 
                      LEFT JOIN (
                        SELECT 
                          pedidoFluxo.PED_CODIGO, 
                          ISNULL(
                            (
                              SELECT 
                                TOP 1 centroDescarregamento.CED_USAR_LAYOUT_AGENDAMENTO_POR_CAIXA_ITEM 
                              FROM 
                                T_CENTRO_DESCARREGAMENTO AS centroDescarregamento 
                                INNER JOIN T_CLIENTE AS destinatarioCentro ON centroDescarregamento.CLI_CGCCPF_DESTINATARIO = destinatarioCentro.CLI_CGCCPF 
                                LEFT JOIN T_FILIAL AS filial ON centroDescarregamento.FIL_CODIGO = filial.FIL_CODIGO 
                              WHERE 
                                destinatarioCentro.CLI_CGCCPF = ISNULL(pedidoFluxo.CLI_CODIGO, 0) 
                                AND (
                                  filial.FIL_CODIGO = ISNULL(pedidoFluxo.FIL_CODIGO, 0) 
                                  OR filial.FIL_CODIGO IS NULL
                                ) 
                              ORDER BY 
                                CASE WHEN filial.FIL_CODIGO IS NOT NULL THEN 1 ELSE 0 END DESC
                            ), 
                            0
                          ) AS UsarLayoutAgendamentoPorCaixaItem 
                        FROM 
                          T_PEDIDO AS pedidoFluxo 
                        GROUP BY 
                          pedidoFluxo.PED_CODIGO, 
                          pedidoFluxo.CLI_CODIGO, 
                          pedidoFluxo.FIL_CODIGO
                      ) AS fluxo ON pedido.PED_CODIGO = fluxo.PED_CODIGO ";

            sql += "WHERE PED_TOTALMENTE_CARREGADO = 0 AND PED_DATA_VALIDADE >= GETDATE() AND PED_SITUACAO != 2 AND tipoOperacao.TOP_PEDIDOS_AGENDAMENTO_COLETA = 1 ";

            if (!string.IsNullOrWhiteSpace(codigoPedido))
                sql += $"AND pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{codigoPedido}'";

            if (codigoDestinatario > 0)
                sql += $"AND pedido.CLI_CODIGO = {codigoDestinatario}";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPendente)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPendente>();
        }

        public List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido> ObterModeloDadosPedidos(DateTime dataInicioCriacao, DateTime dataFimCriacao, string numeroPedido)
        {
            int limiteRegistros = 1000;
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            if (dataInicioCriacao != DateTime.MinValue)
                consultaPedido = consultaPedido.Where(pedido => pedido.DataCriacao >= dataInicioCriacao);

            if (dataFimCriacao != DateTime.MinValue)
                consultaPedido = consultaPedido.Where(pedido => pedido.DataCriacao <= dataFimCriacao);

            if (!string.IsNullOrWhiteSpace(numeroPedido))
                consultaPedido = consultaPedido.Where(pedido => pedido.NumeroPedidoEmbarcador == numeroPedido);

            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido> pedidos = consultaPedido
                .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                .Select(pedido => new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido()
                {
                    Protocolo = pedido.Protocolo,
                    Numero = pedido.NumeroPedidoEmbarcador,
                    DataCriacao = pedido.DataCriacao,
                    PesoTotal = pedido.PesoTotal,
                    Filial = (pedido.Filial == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Filial()
                    {
                        CodigoIntegracao = pedido.Filial.CodigoFilialEmbarcador,
                        Cnpj = pedido.Filial.CNPJ,
                        Descricao = pedido.Filial.Descricao
                    },
                    FilialVenda = (pedido.FilialVenda == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Filial()
                    {
                        CodigoIntegracao = pedido.FilialVenda.CodigoFilialEmbarcador,
                        Cnpj = pedido.FilialVenda.CNPJ,
                        Descricao = pedido.FilialVenda.Descricao
                    },
                    FuncionarioVendedor = (pedido.FuncionarioVendedor == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Usuario()
                    {
                        CpfCnpj = pedido.FuncionarioVendedor.CPF,
                        Nome = pedido.FuncionarioVendedor.Nome,
                    },
                    Destinatario = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Cliente()
                    {
                        CpfCnpj = pedido.Destinatario.Tipo == "J" ? String.Format(@"{0:00000000000000}", pedido.Destinatario.CPF_CNPJ) : String.Format(@"{0:00000000000}", pedido.Destinatario.CPF_CNPJ),
                        Nome = pedido.Destinatario.Nome,
                        IE = pedido.Destinatario.IE_RG,
                        GrupoPessoas = (pedido.Destinatario.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.GrupoPessoas()
                        {
                            CodigoIntegracao = pedido.Destinatario.GrupoPessoas.CodigoIntegracao,
                            Descricao = pedido.Destinatario.GrupoPessoas.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Endereco()
                        {
                            Logradouro = pedido.Destinatario.Endereco,
                            Bairro = pedido.Destinatario.Bairro,
                            Numero = pedido.Destinatario.Numero,
                            Latitude = pedido.Destinatario.Latitude,
                            Longitude = pedido.Destinatario.Longitude,
                            Localidade = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Localidade()
                            {
                                Descricao = pedido.Destinatario.Localidade.Descricao,
                                CodigoIbge = pedido.Destinatario.Localidade.CodigoIBGE,
                                Regiao = (pedido.Destinatario.Localidade.Regiao == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Regiao()
                                {
                                    Descricao = pedido.Destinatario.Localidade.Regiao.Descricao,
                                    CodigoIntegracao = pedido.Destinatario.Localidade.Regiao.CodigoIntegracao
                                },
                                Estado = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Estado()
                                {
                                    Descricao = pedido.Destinatario.Localidade.Estado.Nome,
                                    Sigla = pedido.Destinatario.Localidade.Estado.Sigla,
                                }
                            }
                        }
                    },
                    OutroEnderecoDestino = (!pedido.UsarOutroEnderecoDestino || (pedido.EnderecoDestino == null) || (pedido.EnderecoDestino.ClienteOutroEndereco == null)) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Endereco()
                    {
                        Logradouro = pedido.EnderecoDestino.ClienteOutroEndereco.Endereco,
                        Bairro = pedido.EnderecoDestino.ClienteOutroEndereco.Bairro,
                        Numero = pedido.EnderecoDestino.ClienteOutroEndereco.Numero,
                        Latitude = pedido.EnderecoDestino.ClienteOutroEndereco.Latitude,
                        Longitude = pedido.EnderecoDestino.ClienteOutroEndereco.Longitude,
                        Localidade = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Localidade()
                        {
                            Descricao = pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.Descricao,
                            CodigoIbge = pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.CodigoIBGE,
                            Regiao = (pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.Regiao == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Regiao()
                            {
                                Descricao = pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.Regiao.Descricao,
                                CodigoIntegracao = pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.Regiao.CodigoIntegracao
                            },
                            Estado = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Estado()
                            {
                                Descricao = pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.Estado.Nome,
                                Sigla = pedido.EnderecoDestino.ClienteOutroEndereco.Localidade.Estado.Sigla,

                            }
                        }
                    }
                })
                .ToList();

            if (pedidos.Count > 0)
            {
                List<(int ProtocoloPedido, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.PedidoProduto PedidoProduto)> pedidoProdutos = new List<(int ProtocoloPedido, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.PedidoProduto PedidoProduto)>();
                List<(int ProtocoloPedido, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.NotaFiscal PedidoNotaFiscal)> pedidoNotasFiscais = new List<(int ProtocoloPedido, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.NotaFiscal PedidoNotaFiscal)>();

                for (int registroInicial = 0; registroInicial < pedidos.Count; registroInicial += limiteRegistros)
                {
                    List<int> protocolosPedidos = pedidos.Select(pedido => pedido.Protocolo).Skip(registroInicial).Take(limiteRegistros).ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> consultaPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>()
                        .Where(pedidoProduto => protocolosPedidos.Contains(pedidoProduto.Pedido.Protocolo));

                    pedidoProdutos.AddRange(consultaPedidoProduto
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(pedidoProduto => ValueTuple.Create(
                            pedidoProduto.Pedido.Protocolo,
                            new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.PedidoProduto()
                            {
                                CodigoIntegracao = pedidoProduto.Produto.CodigoProdutoEmbarcador,
                                Descricao = pedidoProduto.Produto.Descricao,
                                Quantidade = pedidoProduto.Quantidade,
                                PesoUnitario = pedidoProduto.PesoUnitario,
                                PesoTotal = (pedidoProduto.Quantidade * pedidoProduto.PesoUnitario) + pedidoProduto.PesoTotalEmbalagem,
                                ValorUnitario = (decimal?)pedidoProduto.PrecoUnitario ?? 0m,
                                ValorTotal = (pedidoProduto.Quantidade * ((decimal?)pedidoProduto.PrecoUnitario ?? 0m))
                            }
                        ))
                        .ToList()
                    );

                    IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                        .Where(pedidoNotaFiscal =>
                            pedidoNotaFiscal.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                            pedidoNotaFiscal.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                            protocolosPedidos.Contains(pedidoNotaFiscal.CargaPedido.Pedido.Protocolo)
                        );

                    pedidoNotasFiscais.AddRange(consultaPedidoXMLNotaFiscal
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(pedidoNotaFiscal => ValueTuple.Create(
                            pedidoNotaFiscal.CargaPedido.Pedido.Protocolo,
                            new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.NotaFiscal()
                            {
                                Chave = pedidoNotaFiscal.XMLNotaFiscal.Chave,
                                Valor = pedidoNotaFiscal.XMLNotaFiscal.Valor
                            }
                        ))
                        .ToList()
                    );
                }

                foreach (Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Pedido pedido in pedidos)
                {
                    pedido.Produtos = pedidoProdutos
                        .Where(produto => produto.ProtocoloPedido == pedido.Protocolo)
                        .Select(produto => produto.PedidoProduto)
                        .ToList();

                    pedido.NotasFiscais = pedidoNotasFiscais
                        .Where(notaFiscal => notaFiscal.ProtocoloPedido == pedido.Protocolo)
                        .Select(notaFiscal => notaFiscal.PedidoNotaFiscal)
                        .Distinct()
                        .ToList();
                }
            }

            return pedidos;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosSemSessao(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido>();

            query = query.Where(o => !codigosPedidos.Contains(o.Pedido.Codigo));

            return query.Select(o => o.Pedido).ToList();
        }

        public bool BuscarNumeroPedidoEmbarcadorDuplicado(string numeroPedidoEmbarcador, int codigoTipoOperacao)
        {

            var retorno = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(x => x.NumeroPedidoEmbarcador == numeroPedidoEmbarcador && x.TipoOperacao.Codigo == codigoTipoOperacao).Any();

            return retorno;
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido>> ObterDetalhesPedidosMonitoramentoAsync(int codigoCarga)
        {
            int limiteRegistros = 1000;
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consultaCargaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido => cargaPedido.Carga.Codigo == codigoCarga);

            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido> detalhesPedidos = await consultaCargaPedido.Select(cargaPedido => new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido()
            {
                Codigo = cargaPedido.Pedido.Codigo,
                CodigoCargaPedido = cargaPedido.Codigo,
                CodigoCarga = cargaPedido.Carga.Codigo,
                NumeroPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                ProtocoloIntegracao = cargaPedido.Pedido.Protocolo,
                DataEntrega = cargaPedido.Pedido.DataEntrega,
                Filial = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Filial()
                {
                    Codigo = cargaPedido.Carga.Filial.Codigo,
                    Descricao = cargaPedido.Carga.Filial.Descricao
                },
                TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.TipoOperacao()
                {
                    Codigo = cargaPedido.Carga.TipoOperacao.Codigo,
                    Descricao = cargaPedido.Carga.TipoOperacao.Descricao,
                    CodigoIntegracao = cargaPedido.Carga.TipoOperacao.CodigoIntegracao
                },
                Carga = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Carga()
                {
                    Codigo = cargaPedido.Carga.Codigo,
                    CodigoCargaEmbarcador = cargaPedido.Carga.CodigoCargaEmbarcador,
                    CargaCritica = cargaPedido.Carga.CargaCritica,
                },
                Destinatario = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Cliente()
                {
                    CPFCNPJ = cargaPedido.Pedido.Destinatario.CPF_CNPJ,
                    CodigoCliente = cargaPedido.Pedido.Destinatario.CodigoIntegracao,
                    Nome = cargaPedido.Pedido.Destinatario.Nome,
                    IE = cargaPedido.Pedido.Destinatario.IE_RG,
                    Endereco = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Endereco()
                    {
                        Logradouro = cargaPedido.Pedido.Destinatario.Endereco,
                        Bairro = cargaPedido.Pedido.Destinatario.Bairro,
                        Numero = cargaPedido.Pedido.Destinatario.Numero,
                        CEP = cargaPedido.Pedido.Destinatario.CEP,
                        Localidade = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Localidade()
                        {
                            CodigoIBGE = cargaPedido.Pedido.Destinatario.Localidade.CodigoIBGE,
                            Descricao = cargaPedido.Pedido.Destinatario.Localidade.Descricao,
                            Estado = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Estado()
                            {
                                Sigla = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla,
                                Nome = cargaPedido.Pedido.Destinatario.Localidade.Estado.Nome
                            }
                        }
                    }
                },
                Remetente = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Cliente()
                {
                    CPFCNPJ = cargaPedido.Pedido.Remetente.CPF_CNPJ,
                    CodigoCliente = cargaPedido.Pedido.Remetente.CodigoIntegracao,
                    Nome = cargaPedido.Pedido.Remetente.Nome,
                    IE = cargaPedido.Pedido.Remetente.IE_RG,
                    Endereco = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Endereco()
                    {
                        Logradouro = cargaPedido.Pedido.Remetente.Endereco,
                        Bairro = cargaPedido.Pedido.Remetente.Bairro,
                        Numero = cargaPedido.Pedido.Remetente.Numero,
                        CEP = cargaPedido.Pedido.Remetente.CEP,
                        Localidade = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Localidade()
                        {
                            CodigoIBGE = cargaPedido.Pedido.Remetente.Localidade.CodigoIBGE,
                            Descricao = cargaPedido.Pedido.Remetente.Localidade.Descricao,
                            Estado = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Estado()
                            {
                                Sigla = cargaPedido.Pedido.Remetente.Localidade.Estado.Sigla,
                                Nome = cargaPedido.Pedido.Remetente.Localidade.Estado.Nome
                            }
                        }
                    }
                },
                ModeloVeicular = cargaPedido.Carga.ModeloVeicularCarga != null ? new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.ModeloVeicular()
                {
                    Codigo = cargaPedido.Carga.ModeloVeicularCarga.Codigo,
                    Descricao = cargaPedido.Carga.ModeloVeicularCarga.Descricao
                } : null,
                CanalVenda = cargaPedido.Pedido.CanalVenda.Descricao,
                CanalEntrega = cargaPedido.Pedido.CanalEntrega.Descricao,
                EscritorioVenda = cargaPedido.Pedido.EscritorioVenda,
                TipoMercadoria = cargaPedido.Pedido.TipoMercadoria,
                EquipeVendas = cargaPedido.Pedido.EquipeVendas,
                Observacao = cargaPedido.Pedido.Observacao,
                Peso = cargaPedido.Peso,
                DataPrevisaoEntrega = cargaPedido.PrevisaoEntrega,
                Adicional7 = cargaPedido.Pedido.Adicional7 ?? string.Empty,
                PedidoCritico = cargaPedido.Pedido.PedidoCritico ?? false
            }).ToListAsync(CancellationToken);

            if (detalhesPedidos.Count > 0)
            {
                List<(int CodigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Produto Produto)> produtos = new List<(int CodigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Produto produto)>();
                List<(int CodigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.NotaFiscal NotaFiscal)> notasFiscais = new List<(int CodigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.NotaFiscal NotaFiscal)>();
                List<(int CodigosCargaPedido, Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.CargaEntrega CargaEntrega)> cargasEntrega = new List<(int CodigoCargaPedido, Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.CargaEntrega CargaEntrega)>();

                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                    .Where(cargaEntrega => cargaEntrega.Carga.Codigo == codigoCarga);

                var codigosPedidos = await consultaCargaEntrega
                    .SelectMany(cargaEntrega => cargaEntrega.Pedidos.Select(cargaEntregaPedido => cargaEntregaPedido.CargaPedido.Codigo))
                    .ToListAsync(CancellationToken);

                if (cargasEntrega.Any()) {
                    cargasEntrega.AddRange(await consultaCargaEntrega.Select(cargaEntrega => ValueTuple.Create(
                    cargaEntrega.Pedidos.Select(cargaEntregaPedido => cargaEntregaPedido.CargaPedido.Codigo).FirstOrDefault(),
                            new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.CargaEntrega()
                            {

                                Codigo = cargaEntrega.Codigo,
                                DataPrevisaoEntrega = cargaEntrega.DataPrevista,
                                DataEntrega = cargaEntrega.DataConfirmacao,
                                Situacao = cargaEntrega.Situacao,
                                DataReprogramada = cargaEntrega.DataReprogramada,
                                DataPrevisaoEntregaAjustada = cargaEntrega.DataPrevisaoEntregaAjustada,
                            }
                    )).ToListAsync(CancellationToken));
                }


                for (int registroInicial = 0; registroInicial < detalhesPedidos.Count; registroInicial += limiteRegistros)
                {
                    List<int> codigosCargaPedido = detalhesPedidos.Select(detalhePedido => detalhePedido.CodigoCargaPedido).Skip(registroInicial).Take(limiteRegistros).ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> consultaCargaPedidoProduto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                        .Where(cargaPedidoProduto => codigosCargaPedido.Contains(cargaPedidoProduto.CargaPedido.Codigo));

                    produtos.AddRange(await consultaCargaPedidoProduto.Select(cargaPedidoProduto => ValueTuple.Create(
                        cargaPedidoProduto.CargaPedido.Codigo,
                        new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Produto()
                        {
                            Codigo = cargaPedidoProduto.Produto.Codigo,
                            CodigoProdutoEmbarcador = cargaPedidoProduto.Produto.CodigoProdutoEmbarcador,
                            Descricao = cargaPedidoProduto.Produto.Descricao
                        }
                       )
                    ).ToListAsync(CancellationToken));

                    IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> consultaCargaPedidoXMLNotaFiscal = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>()
                        .Where(cargaPedidoXMLNotaFiscal => codigosCargaPedido.Contains(cargaPedidoXMLNotaFiscal.CargaPedido.Codigo));

                    notasFiscais.AddRange(await consultaCargaPedidoXMLNotaFiscal.Select(cargaPedidoXMLNotaFiscal => ValueTuple.Create(
                        cargaPedidoXMLNotaFiscal.CargaPedido.Codigo,
                        new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.NotaFiscal()
                        {
                            NumeroNota = cargaPedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                            DataEmissao = cargaPedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao
                        })
                    ).ToListAsync(CancellationToken));

                }

                foreach (Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido detalhePedido in detalhesPedidos)
                {
                    detalhePedido.Produtos = produtos
                        .Where(produto => produto.CodigoCargaPedido == detalhePedido.CodigoCargaPedido)
                        .Select(produto => produto.Produto)
                        .ToList();

                    detalhePedido.NotasFiscais = notasFiscais
                        .Where(notaFiscal => notaFiscal.CodigoCargaPedido == detalhePedido.CodigoCargaPedido)
                        .Select(notaFiscal => notaFiscal.NotaFiscal)
                        .ToList();

                    detalhePedido.CargaEntrega = cargasEntrega
                        .Where(cargaEntrega => cargaEntrega.CodigosCargaPedido == detalhePedido.CodigoCargaPedido)
                        .Select(cargaEntrega => cargaEntrega.CargaEntrega)
                        .FirstOrDefault();
                }
            }

            return detalhesPedidos;
        }

        public async Task AtualizarValorFreteNegociadoProdutorRuralAsync(int codigoPedido, decimal valorFreteNegociado)
        {
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido => pedido.Codigo == codigoPedido)
                .SelectMany(pedido => pedido.NotasFiscais)
                .FirstOrDefaultAsync(CancellationToken);

            if (xmlNotaFiscal != null)
            {
                xmlNotaFiscal.ValorFrete = valorFreteNegociado;

                XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(UnitOfWork);
                await repositorioXMLNotaFiscal.AtualizarAsync(xmlNotaFiscal);
            }
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCotacao(long codigoCotacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.CotacaoPedido.Codigo == codigoCotacaoPedido select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPorCotacaoQueNaoEstejaCanceladaOuRejeitada(long codigoCotacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.CotacaoPedido.Codigo == codigoCotacaoPedido && obj.SituacaoPedido != SituacaoPedido.Cancelado && obj.SituacaoPedido != SituacaoPedido.Rejeitado select obj;
            return result.ToList();

        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidoComGrupoPessoasMaiorPrioridadeAsync(List<int> codigosPedidos, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                .Where(pedido => codigosPedidos.Contains(pedido.Codigo))
                .OrderByDescending(o => o.Destinatario.GrupoPessoas.Prioridade);

            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public DateTime? BuscarMaiorDataPrevisaoEntregaEntrePedidosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga
                         select obj.Pedido.PrevisaoEntrega;
            return result.Max();
        }

        #endregion Métodos Públicos
    }
}
