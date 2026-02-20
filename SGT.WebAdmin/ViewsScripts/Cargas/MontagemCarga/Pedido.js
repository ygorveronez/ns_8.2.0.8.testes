/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="DetalhePedidoNotaFiscal.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _AreaPedido;
var _detalhePedido;
var _detalhePedidoProduto;
var _detalhePedidoExportacao;
var _detalhePedidoTransporteMaritimo;
var _gridPedidosMontagemCarga;

/**
 * Contem todos os pedidos resultantes da busca, incluíndo os selecionados
 */
var PEDIDOS = ko.observableArray([]);

/**
 * Contem apenas os pedidos selecionados
 */
var PEDIDOS_SELECIONADOS = ko.observableArray([]);

/**
 * Contem apenas os pedidos selecionados do carregamento não resultantes da busca
 */
var PEDIDOS_NAO_LISTADOS = ko.observableArray([]);

/**
 * Contem apenas as notas fiscais não selecionadas
 * */
var NOTAS_FISCAIS_SELECIONADAS = ko.observableArray([]);

var fimListagemPedidos = 0;
var quantidadeLoadPedidos = 36;
var _knoutsPedidos = [];
var _gridProdutos;

var AreaPedido = function () {
    this.Pedidos = PropertyEntity({ val: PEDIDOS, def: [] });
    this.CarregandoPedidos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Fim = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: PedidosPesquisaScroll });
    this.TotalPedidos = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TotalPedidos.getFieldDescription()) });

    this.PesoTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoTotal.getFieldDescription()), visible: true });
    this.PesoLiquidoTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotal.getFieldDescription()), visible: true });
    this.PesoSaldoRestante = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.PalletSaldoRestante = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.VolumeTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.VolumeTotal.getFieldDescription()), visible: true });
    this.SaldoVolumesRestante = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorTotalDosPedidos = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ValorTotalDosPedidos.getFieldDescription()), visible: true });
    this.ValorPedidosSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });

    this.TotalPedidosSelecionados = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TotalPedidosSelecionados.getFieldDescription()) });
    this.PesoTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoTotalSelecionados.getFieldDescription()), visible: true });
    this.PesoLiquidoTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotalSelecionados.getFieldDescription()), visible: true });
    this.PesoSaldoRestanteSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.PalletSaldoRestanteSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.VolumeTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.VolumeTotalSelecionados.getFieldDescription()), visible: true });

    this.SelecionarTodos = PropertyEntity({ type: types.event, eventClick: selecionarTodosPedidosClick, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.SelecionarTodos.getFieldDescription()), val: false, visible: true });
    this.ExibirPercentualSeparacaoPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    //Visualização Pedidos Grid
    this.TabelaPedidosVisivel = PropertyEntity({ visible: ko.observable(false) });
    this.GridPedidos = PropertyEntity({ type: types.local });
};

var DetalhePedido = function (pedido) {
    this.DT_RowColor = PropertyEntity();
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataCriacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataCriacao.getFieldDescription()), val: ko.observable("") });
    this.DataCarregamentoPedido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataCarregamentoPedido.getFieldDescription()), val: ko.observable("") });
    this.Filial = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Filial.getFieldDescription()), val: ko.observable(""), visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroPedido.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroPedidoEmbarcador.getFieldDescription()), val: ko.observable("") });
    this.Origem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Origem.getFieldDescription()), val: ko.observable("") });
    this.ExibirOrigem = PropertyEntity({ val: ko.observable(false) });
    this.Destino = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Destino.getFieldDescription()), val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Remetente.getFieldDescription()), val: ko.observable(""), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Destinatario.getFieldDescription()), val: ko.observable(""), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Expedidor.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.Recebedor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Recebedor.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.CodRecebedor = PropertyEntity({ val: ko.observable(0), visible: ko.observable(false) });
    this.DestinoRecebedor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Destino.getFieldDescription()), val: ko.observable("") });
    this.Peso = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoTotal.getFieldDescription()), val: ko.observable("") });
    this.PesoLiquido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotal.getFieldDescription()), val: ko.observable("") });
    this.PesoSaldoRestante = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoSaldoRestante.getFieldDescription()), val: ko.observable("") });
    this.Volumes = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Volumes.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.ValorTotalPedido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ValorTotalPedido.getFieldDescription()), val: ko.observable(""), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Empresa.getFieldDescription()), val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoOperacao.getFieldDescription()), val: ko.observable("") });
    this.CanalEntrega = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CanalEntrega.getFieldDescription()), val: ko.observable("") });
    this.TotalPallets = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TotalPallets.getFieldDescription()), val: ko.observable("") });
    this.PalletSaldoRestante = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PalletSaldoRestante.getFieldDescription()), val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Cubagem.getFieldDescription()), val: ko.observable("") });
    this.Restricao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Restricao.getFieldDescription()), val: ko.observable("") });
    this.ObservacaoRestricao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ObservacaoRestricao.getFieldDescription()), val: ko.observable("") });
    this.ObservacaoPedido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ObservacaoPedido.getFieldDescription()), val: ko.observable(""), visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Observacao.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.ObservacaoInterna = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ObservacaoInterna.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.DataInicialColeta = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInicialColeta.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.DataPrevisaoSaida = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataPrevisaoSaida.getFieldDescription()), val: ko.observable("") });
    this.DataPrevisaoEntrega = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataPrevisaoEntrega.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.ExigirPreCargaMontagemCarga = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false) });
    this.Temperatura = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Temperatura.getFieldDescription()), val: ko.observable("") });
    this.Vendedor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Vendedor.getFieldDescription()), val: ko.observable("") });
    this.Ordem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Ordem.getFieldDescription()), val: ko.observable("") });
    this.PortoSaida = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PortoSaida.getFieldDescription()), val: ko.observable("") });
    this.PortoChegada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PortoChegada.getFieldDescription()), val: ko.observable("") });
    this.Companhia = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Companhia.getFieldDescription()), val: ko.observable("") });
    this.NumeroNavio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroNavio.getFieldDescription()), val: ko.observable("") });
    this.Reserva = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Reserva.getFieldDescription()), val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.Resumo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Resumo.getFieldDescription()), val: ko.observable("") });
    this.TipoEmbarque = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoEmbarque.getFieldDescription()), val: ko.observable("") });
    this.DeliveryTerm = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DeliveryTerm.getFieldDescription()), val: ko.observable("") });
    this.IdAutorizacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.IdAutorizacao.getFieldDescription()), val: ko.observable("") });
    this.DataETA = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETA.getFieldDescription()), val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.DataInclusaoBooking = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInclusaoBooking.getFieldDescription()), val: ko.observable("") });
    this.DataInclusaoPCP = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataInclusaoPCP.getFieldDescription()), val: ko.observable("") });
    this.Selecionado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.SemLatLng = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PedidoPrioritario = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiAjudante = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PossuiAjudante), val: ko.observable("") });
    this.Distancia = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Distancia), val: ko.observable("") });
    this.TipoPagamentoValePedagio = PropertyEntity({ text: ko.observable("Tipo de Pagamento do Vale Pedágio"), val: ko.observable("asdasdasdasd"), visible: ko.observable(true) });
    this.PossuiCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PossuiCarga), val: ko.observable("") });
    this.PossuiDescarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PossuiDescarga), val: ko.observable("") });
    this.ModeloVeicularCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ModeloVeicularCarga.getFieldDescription()), val: ko.observable("") });
    this.FormaPagamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.FormaPagamento.getFieldDescription()), val: ko.observable("") });
    this.DiasDePrazoFatura = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DiasDePrazoFatura.getFieldDescription()), val: ko.observable("") });
    this.CargaPerigosa = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CargaPerigosa.getFieldDescription()), val: ko.observable("") });
    this.PermiteInformarPesoCubadoNaMontagemDaCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PedidoColetaEntrega = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Produto = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Produto.getFieldDescription()), val: ko.observable(""), visible: ko.observable(false) });
    this.QuantidadeVolumes = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.QuantidadeVolumes.getFieldDescription()), val: ko.observable("") });
    this.PercentualSeparacaoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PedidoBloqueado = PropertyEntity({ val: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PedidoBloqueado.getFieldDescription()), cssClass: ko.observable("ribbon-tms ribbon-tms-red"), def: false, getType: typesKnockout.bool });
    this.ValorMercadoInferior = PropertyEntity({ val: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ValorMercadoInferior.getFieldDescription()), cssClass: ko.observable("ribbon-tms ribbon-tms-red"), def: false, getType: typesKnockout.bool });
    this.PedidoRestricaoData = PropertyEntity({ val: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PedidoRestricaoData.getFieldDescription()), cssClass: ko.observable("ribbon-tms ribbon-tms-red"), def: false, getType: typesKnockout.bool });
    this.LiberadoMontagemCarga = PropertyEntity({ val: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.LiberadoMontagemCarga.getFieldDescription()), cssClass: ko.observable(""), def: false, getType: typesKnockout.bool });
    this.ExisteAlgumPercentualSeparacaoPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoTomador = PropertyEntity({});
    this.DescricaoTipoTomador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DescricaoTipoTomador.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ValorMercadoria.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.RemetenteCNPJ = PropertyEntity({ val: ko.observable(""), visible: ko.observable(false) });
    this.TransportadorLocalCarregamentoRestringido = PropertyEntity({ val: ko.observable(""), visible: ko.observable(false) });
    this.NumeroPedidoSequencial = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroPedido.getFieldDescription()), val: ko.observable("") });
    this.DataAgendamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataAgendamento.getFieldDescription()), val: ko.observable("") });
    this.NumeroOrdem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroOrdem.getFieldDescription()), val: ko.observable("") });

    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.NotasFiscais.getFieldDescription(), val: ko.observable("") });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.TipoDeCarga.getFieldDescription(), val: ko.observable("") });

    this.InfoPedido = PropertyEntity({ eventClick: selecionarPedidoClick, eventClickDetalhe: eventClickDetalheClick, type: types.event, cssClass: ko.observable("card card-carga no-padding padding-5"), visibleSemLatLng: ko.observable(false) });

    this.Anexos = PropertyEntity({ eventClick: anexosAgendamentoColetaPedidoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Anexos.getFieldDescription()), visible: ko.observable(true) });

    this.Email = PropertyEntity({ text: ko.observable("*E-mail: "), issue: 30, required: true, getType: typesKnockout.email, maxlength: 1000 });
    this.EnviarEmail = PropertyEntity({ eventClick: enviarEmailClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.EnviarEmail.getFieldDescription()) });

    PreencherObjetoKnout(this, { Data: pedido });

    this.Peso.val(pedido.PesoSaldoRestante);
    //this.TotalPallets.val(pedido.TotalPallets);
    this.TotalPallets.val(pedido.PalletSaldoRestante);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        this.NumeroPedido.visible(true);
        this.InfoPedido.val(pedido.PedidoPrioritario);
        this.Produto.visible(true);
    }

    if (pedido.PedidoBloqueado && !pedido.LiberadoMontagemCarga)
        this.PedidoBloqueado.text('Bloq - N. LIB.');
    else if (!pedido.LiberadoMontagemCarga) {
        this.PedidoBloqueado.text(Localization.Resources.Cargas.MontagemCarga.PedidoLiberado);
        this.PedidoBloqueado.val(true);
    }
    else if (pedido.PedidoBloqueado)
        this.PedidoBloqueado.text(Localization.Resources.Cargas.MontagemCarga.PedidoBloqueado);

    //this.ExisteAlgumPercentualSeparacaoPedido.val(AlgumPercentualSeparacaoPedido());
    this.ExisteAlgumPercentualSeparacaoPedido.val(_AreaPedido.ExibirPercentualSeparacaoPedido.val());
};

var DetalhePedidoProduto = function () {
    this.GridProdutos = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Produtos = PropertyEntity({});
}

var DetalhePedidoExportacao = function () {
    this.AcondicionamentoCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.AcondicionamentoCarga.getFieldDescription()) });
    this.CargaPaletizada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CargaPaletizada.getFieldDescription()) });
    this.ClienteAdicional = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ClienteAdicional.getFieldDescription()) });
    this.ClienteDonoContainer = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ClienteDonoContainer.getFieldDescription()) });
    this.DataDeadLCargaNavioViagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataDeadLCargaNavioViagem.getFieldDescription()) });
    this.DataDeadLineNavioViagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataDeadLineNavioViagem.getFieldDescription()) });
    this.DataEstufagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataEstufagem.getFieldDescription()) });
    this.Despachante = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Despachante.getFieldDescription()) });
    this.ETA = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ETA.getFieldDescription()) });
    this.ETS = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ETS.getFieldDescription()) });
    this.InLand = PropertyEntity({ text: "In Land: " });
    this.NavioViagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NavioViagem.getFieldDescription()) });
    this.NumeroBooking = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroBooking.getFieldDescription()) });
    this.NumeroEXP = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroEXP.getFieldDescription()) });
    this.NumeroPedidoProvisorio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroPedidoProvisorio.getFieldDescription()) });
    this.NumeroReserva = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroReserva.getFieldDescription()) });
    this.PortoViagemDestino = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PortoViagemDestino.getFieldDescription()) });
    this.PortoViagemOrigem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PortoViagemOrigem.getFieldDescription()) });
    this.PossuiGenset = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PossuiGenset.getFieldDescription()) });
    this.RefEXPTransferencia = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.RefEXPTransferencia.getFieldDescription()) });
    this.TipoContainer = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoContainer.getFieldDescription()) });
    this.TipoProbe = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoProbe.getFieldDescription()) });
    this.ViaTransporte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ViaTransporte.getFieldDescription()) });
};

var DetalhePedidoTransporteMaritimo = function () {
    this.CodigoArmador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoArmador.getFieldDescription()) });
    this.CodigoNCM = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoNCM.getFieldDescription()) });
    this.CodigoRota = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoRota.getFieldDescription()) });
    this.CodigoIdentificacaoCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoIdentificacaoCarga.getFieldDescription()) });
    this.DescricaoIdentificacaoCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DescricaoIdentificacaoCarga.getFieldDescription()) });
    this.Incoterm = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Incoterm.getFieldDescription()) })
    this.MensagemTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.MensagemTransbordo.getFieldDescription()) });
    this.MetragemCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.MetragemCarga.getFieldDescription()) });
    this.ModoTransporte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ModoTransporte.getFieldDescription()) });
    this.NomeNavio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NomeNavio.getFieldDescription()) });
    this.NomeNavioTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NomeNavioTransbordo.getFieldDescription()) });
    this.NumeroBL = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroBL.getFieldDescription()) });
    this.NumeroContainer = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroContainer.getFieldDescription()) });
    this.NumeroLacre = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroLacre.getFieldDescription()) });
    this.NumeroViagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroViagem.getFieldDescription()) });
    this.NumeroViagemTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroViagemTransbordo.getFieldDescription()) });
    this.DataBooking = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataBooking.getFieldDescription()) });
    this.DataETAOrigem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETAOrigem.getFieldDescription()) });
    this.DataETASegundaOrigem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETASegundaOrigem.getFieldDescription()) });
    this.DataETAOrigemFinal = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETAOrigemFinal.getFieldDescription()) });
    this.DataETADestino = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETADestino.getFieldDescription()) });
    this.DataETASegundoDestino = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETASegundoDestino.getFieldDescription()) });
    this.DataETADestinoFinal = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETADestinoFinal.getFieldDescription()) });
    this.DataETATransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETATransbordo.getFieldDescription()) });
    this.DataETS = PropertyEntity({ text: "ETS: " });
    this.DataETSTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataETSTransbordo.getFieldDescription()) });
    this.DataDepositoContainer = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataDepositoContainer.getFieldDescription()) });
    this.DataRetiradaContainerDestino = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataRetiradaContainerDestino.getFieldDescription()) });
    this.DataRetiradaVazio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataRetiradaVazio.getFieldDescription()) });
    this.DataRetornoVazio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataRetornoVazio.getFieldDescription()) });
    this.DataDeadLineCarga = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataDeadLCargaNavioViagem.getFieldDescription()) });
    this.DataDeadLineDraf = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataDeadLineDraf.getFieldDescription()) });
    this.CodigoPortoCarregamentoTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoPortoCarregamentoTransbordo.getFieldDescription()) });
    this.DescricaoPortoCarregamentoTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DescricaoPortoCarregamentoTransbordo.getFieldDescription()) });
    this.CodigoPortoDestinoTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CodigoPortoDestinoTransbordo.getFieldDescription()) });
    this.DescricaoPortoDestinoTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DescricaoPortoDestinoTransbordo.getFieldDescription()) });
    this.TerminalOrigem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TerminalOrigem.getFieldDescription()) });
    this.TipoEnvio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoEnvio.getFieldDescription()) });
    this.TipoTransporte = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoTransporte.getFieldDescription()) });
    this.Transbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Transbordo.getFieldDescription()) });
}

//*******EVENTOS*******
function loadGridProduto() {

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [], tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProdutoEmbarcadorIntegracao", title: Localization.Resources.Cargas.MontagemCarga.CodigoIntegracao, width: "40%" },
        { data: "Descricao", title: Localization.Resources.Cargas.MontagemCarga.Descricao, width: "40%" },
        { data: "Quantidade", title: Localization.Resources.Cargas.MontagemCarga.Quantidade, width: "15%" },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCarga.Peso, width: "15%" },
        { data: "PesoTotal", title: Localization.Resources.Cargas.MontagemCarga.PesoTotal, width: "15%" },
        { data: "QuantidadePalets", title: Localization.Resources.Cargas.MontagemCarga.QuantidadePalets, width: "15%" },
        { data: "PalletFechado", title: Localization.Resources.Cargas.MontagemCarga.PalletFechado, width: "15%" },
        { data: "MetrosCubico", title: "M³", width: "15%" },
        { data: "LinhaSeparacao", title: Localization.Resources.Cargas.MontagemCarga.LinhaSeparacao, width: "15%" },
        { data: "Observacao", title: Localization.Resources.Cargas.MontagemCarga.Observacao, width: "15%" },
    ];
    _gridProdutos = new BasicDataTable(_detalhePedidoProduto.GridProdutos.idGrid, header, null, { column: 1, dir: orderDir.asc });

    _gridProdutos.CarregarGrid(_detalhePedidoProduto.Produtos.val());

}

function loadDetalhesPedido() {
    _AreaPedido = new AreaPedido();
    KoBindings(_AreaPedido, "knoutAreaPedido");

    _detalhePedido = new DetalhePedido({});
    KoBindings(_detalhePedido, "knoutDetalhePedido");

    _detalhePedidoProduto = new DetalhePedidoProduto({});
    KoBindings(_detalhePedidoProduto, "knoutDetalhePedidoProdutos");

    _detalhePedidoExportacao = new DetalhePedidoExportacao();
    KoBindings(_detalhePedidoExportacao, "knoutDetalhePedidoExportacao");

    _detalhePedidoTransporteMaritimo = new DetalhePedidoTransporteMaritimo();
    KoBindings(_detalhePedidoTransporteMaritimo, "knoutDetalhePedidoTransporteMaritimo");

    if (_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga)
        $("#liDetalhePedidoExportacao").show();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _detalhePedido.DataPrevisaoEntrega.visible(true);
        _detalhePedido.ObservacaoInterna.visible(true);
        _detalhePedido.DataInicialColeta.visible(true);
        _detalhePedido.Observacao.visible(true);
        _detalhePedido.Filial.visible(false);
        $("#lcPedidosPrioritarios").show();
    }
    else {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
            _detalhePedido.DataPrevisaoEntrega.visible(true);

        $("#lcPedidosPrioritarios").hide();
    }

    RegistraComponente();

    _AreaPedido.TabelaPedidosVisivel.visible(_CONFIGURACAO_TMS.ExibirPedidosFormatoGrid);
    controleApresentacaoGridPedidos();
}

function RegistraComponente() {
    if (ko.components.isRegistered('montagem-carga-pedido'))
        return;

    ko.components.register('montagem-carga-pedido', {
        viewModel: DetalhePedido,
        template: {
            element: 'template-pedido'
        }
    });
}

function controleApresentacaoGridPedidos() {
    if (_AreaPedido.TabelaPedidosVisivel.visible())
        inicializarTabelaPedidosMontagemCarga();
}

function inicializarTabelaPedidosMontagemCarga() {

    const header = [
        { data: "Codigo", visible: false },
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Cargas.MontagemCarga.NumeroPedidoEmbarcador, width: "5%", widthDefault: "5%", visible: true },
        { data: "DataCarregamentoPedido", title: Localization.Resources.Cargas.MontagemCarga.Data, width: "5%", widthDefault: "5%", visible: true },
        { data: "Filial", title: Localization.Resources.Cargas.MontagemCarga.Filial, width: "5%", widthDefault: "5%", visible: true },
        { data: "Remetente", title: Localization.Resources.Cargas.MontagemCarga.Remetente, width: "5%", widthDefault: "5%", visible: true },
        { data: "Expedidor", title: Localization.Resources.Cargas.MontagemCarga.Expedidor, width: "5%", widthDefault: "5%", visible: true },
        { data: "Destinatario", title: Localization.Resources.Cargas.MontagemCarga.Destinatario, width: "5%", widthDefault: "5%", visible: true },
        { data: "Recebedor", title: Localization.Resources.Cargas.MontagemCarga.Recebedor, width: "5%", widthDefault: "5%", visible: true },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCarga.Destino, width: "5%", widthDefault: "5%", visible: true },
        { data: "DestinoRecebedor", title: Localization.Resources.Cargas.MontagemCarga.Destino, width: "5%", widthDefault: "5%", visible: true },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCarga.Peso, width: "5%", widthDefault: "5%", visible: true },
        { data: "PesoLiquido", title: Localization.Resources.Cargas.MontagemCarga.PesoLiquidoTotal, width: "5%", widthDefault: "5%", visible: true },
        { data: "PesoSaldoRestante", title: Localization.Resources.Cargas.MontagemCarga.PesoSaldoRestante, width: "5%", widthDefault: "5%", visible: true },
        { data: "QuantidadeVolumes", title: Localization.Resources.Cargas.MontagemCarga.QuantidadeVolumes, width: "5%", widthDefault: "5%", visible: true },
        { data: "Ordem", title: Localization.Resources.Cargas.MontagemCarga.Ordem, width: "5%", widthDefault: "5%", visible: true },
        { data: "NumeroPedido", title: Localization.Resources.Cargas.MontagemCarga.NumeroPedido, width: "5%", widthDefault: "5%", visible: true },
        { data: "DataPrevisaoEntrega", title: Localization.Resources.Cargas.MontagemCarga.DataPrevisaoEntrega, width: "5%", widthDefault: "5%", visible: true },
        { data: "Cubagem", title: Localization.Resources.Cargas.MontagemCarga.Cubagem, width: "5%", widthDefault: "5%", visible: true },
        { data: "PercentualSeparacaoPedido", title: Localization.Resources.Cargas.MontagemCarga.Percentual, width: "5%", widthDefault: "5%", visible: true },
        { data: "DataAgendamento", title: Localization.Resources.Cargas.MontagemCarga.DataAgendamento, width: "5%", widthDefault: "5%", visible: true },
        { data: "RotaFreteDescricao", title: Localization.Resources.Cargas.MontagemCarga.Rotas, width: "5%", widthDefault: "5%", visible: true },
        { data: "Carregamentos", title: Localization.Resources.Cargas.MontagemCarga.Carregamentos, width: "5%", widthDefault: "5%", visible: true },
        { data: "CanalEntrega", title: Localization.Resources.Cargas.MontagemCarga.CanalEntrega, width: "5%", widthDefault: "5%", visible: true },
        { data: "TotalPallets", title: Localization.Resources.Cargas.MontagemCarga.TotalPallets, width: "5%", widthDefault: "5%", visible: true },
        { data: "ObservacaoDestinatario", title: Localization.Resources.Cargas.MontagemCarga.ObservacaoCliente, width: "5%", widthDefault: "5%", visible: true },
        { data: "GrupoPessoa", title: Localization.Resources.Cargas.MontagemCarga.GrupoPessoa, width: "5%", widthDefault: "5%", visible: true },
        { data: "PrazoEntrega", title: Localization.Resources.Cargas.MontagemCarga.PrazoEntrega, width: "5%", widthDefault: "5%", visible: true },
        { data: "ValorFrete", title: Localization.Resources.Cargas.MontagemCarga.ValorFrete, width: "5%", widthDefault: "5%", visible: true },
        { data: "CanalVenda", title: Localization.Resources.Cargas.MontagemCarga.CanalVenda, width: "5%", widthDefault: "5%", visible: true },
        { data: "PedidoRestricaoData", title: Localization.Resources.Cargas.MontagemCarga.PedidoRestricaoData, width: "5%", widthDefault: "5%", visible: true },
        { data: "PedidoBloqueadoNaoLiberado", title: Localization.Resources.Cargas.MontagemCarga.PedidoBloqueado, width: "5%", widthDefault: "5%", visible: true },
        { data: "CodigoAgrupamentoCarregamento", title: Localization.Resources.Cargas.MontagemCarga.CodigoAgrupamentoCarregamento, width: "5%", widthDefault: "5%", visible: true },
        { data: "DestinatarioNomeFantasia", title: Localization.Resources.Cargas.MontagemCarga.NomeFantasia, width: "5%", widthDefault: "5%", visible: true },
        { data: "SituacaoComercial", title: Localization.Resources.Cargas.MontagemCarga.SituacaoComercial, width: "5%", widthDefault: "5%", visible: true },
        { data: "SituacaoEstoque", title: Localization.Resources.Cargas.MontagemCarga.SituacaoEstoque, width: "5%", widthDefault: "5%", visible: true },
        { data: "TendenciaEntrega", title: Localization.Resources.Cargas.MontagemCarga.TendenciaEntrega, width: "5%", widthDefault: "5%", visible: true },
        { data: "PrimeiroCodigoCargaEmbarcador", title: Localization.Resources.Cargas.MontagemCarga.NumeroPrimeiraCarga, width: "5%", widthDefault: "5%", visible: true },
        { data: "NotasFiscais", title: Localization.Resources.Cargas.MontagemCarga.NotasFiscais, width: "5%", widthDefault: "5%", visible: true },
        { data: "DataDigitalizacaoCanhotoAvulso", title: Localization.Resources.Cargas.MontagemCarga.DataDigitalizacaoCanhotoAvulso, width: "5%", widthDefault: "5%", visible: true },
        { data: "DataEntregaNotaCanhoto", title: Localization.Resources.Cargas.MontagemCarga.DataEntregaNotaCanhotoCliente, width: "5%", widthDefault: "5%", visible: true },
        { data: "Regiao", title: Localization.Resources.Cargas.MontagemCarga.Regiao, width: "5%", widthDefault: "5%", visible: true },
        { data: "MesoRegiao", title: Localization.Resources.Cargas.MontagemCarga.Mesorregiao, width: "5%", widthDefault: "5%", visible: true },
        { data: "NumeroOrdem", title: Localization.Resources.Cargas.MontagemCarga.NumeroOrdem, width: "5%", widthDefault: "5%", visible: true },
    ];

    const configExportacao = {
        url: "MontagemCargaPedido/ExportarPesquisa",
        btnText: "Exportar Excel",
        funcaoObterParametros: function () {
            return RetornarObjetoPesquisa(_pesquisaMontegemCarga);
        }
    }

    const opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesPedidoGridMontagemCargaClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: false };

    _gridPedidosMontagemCarga = new BasicDataTable(_AreaPedido.GridPedidos.id, header, menuOpcoes, { column: 1, dir: "asc" }, configRowsSelect, 50, null, null, null, null, null, null, configExportacao, null, callbackBoolToString, true, tablePedidosMontagemCargaRegistroSelecionadoChange, selecionarTodosPedidosClick, "Cargas/MontagemCarga", "grid-montagem-carga-pedido");
    _gridPedidosMontagemCarga.SetPermitirEdicaoColunas(true);
    _gridPedidosMontagemCarga.SetSalvarPreferenciasGrid(true);
    _gridPedidosMontagemCarga.SetScrollHorizontal(true);
    _gridPedidosMontagemCarga.SetHabilitarScrollHorizontal(true, 150);
}

function carregarTabelaPedidosMontagemCarga() {
    if (_gridPedidosMontagemCarga)
        _gridPedidosMontagemCarga.CarregarGrid(PEDIDOS());
}

function setarPedidosSelecionadosMontagemCarga() {
    if (_gridPedidosMontagemCarga)
        _gridPedidosMontagemCarga.SetarSelecionados(PEDIDOS_SELECIONADOS());
}

function tablePedidosMontagemCargaRegistroSelecionadoChange(registro, selecionado) {
    _precisarSetarPedidosSelecionadosTabelaMontagemCarga = false;
    SelecionarPedido(registro, !selecionado);
    PedidosSelecionadosChange();

    selecionarNotaFiscalPorPedido(registro, selecionado);
}

function detalhesPedidoGridMontagemCargaClick(pedidoSelecionado) {
    ObterDetalhesPedido(pedidoSelecionado.Codigo);
}

function callbackBoolToString(head, data, row) {
    if (head.name == 'PedidoRestricaoData' || head.name == 'PedidoBloqueadoNaoLiberado') {
        if (data === true)
            return Localization.Resources.Enumeradores.SimNao.Sim;
        else
            return Localization.Resources.Enumeradores.SimNao.Nao;
    } else if (head.name == 'SituacaoComercial') {
        return '<span style="background-color: ' + row.SituacaoComercialCor + '; display: block; text-align: center;">' + data + '</span>';;
    } else if (head.name == 'SituacaoEstoque') {
        return '<span style="background-color: ' + row.SituacaoEstoqueCor + '; display: block; text-align: center;">' + data + '</span>';;
    }
    return data;
}

function selecionarPedidoClick(e, event) {
    var pedido = RetornarObjetoPesquisa(e);
    if (!pedido.Selecionado && _CONFIGURACAO_TMS.NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento && PEDIDOS_SELECIONADOS().length > 0 && PEDIDOS_SELECIONADOS().find(function (pedidoSelecionado) { return pedidoSelecionado.TipoTomador != pedido.TipoTomador })) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, Localization.Resources.Cargas.MontagemCarga.NaoPermitidoGerarCarregamentoParaPedidosDeTomadoresDiferentes);
        event.stopPropagation();
        return;
    }

    //AUREA
    ValidarRecebedorRegiaoDestinoSelecionada(pedido);
    if (pedido.PedidoBloqueado && !pedido.Selecionado && !_CONFIGURACAO_TMS.MontagemCarga.PermitirGerarCarregamentoPedidoBloqueado) return;
    if (!pedido.LiberadoMontagemCarga && !_CONFIGURACAO_TMS.MontagemCarga.PermitirGerarCarregamentoPedidoBloqueado) return;

    var itemPedido = PEDIDOS.where(function (ped) { return ped.Codigo == pedido.Codigo; });

    //#49608-SIMONETTI
    if (itemPedido.CodigoAgrupamentoCarregamento != '' && itemPedido.CodigoAgrupamentoCarregamento != null) {

        var pedidos = ko.utils.arrayFilter(PEDIDOS(), function (item) {
            if (item.CodigoAgrupamentoCarregamento == itemPedido.CodigoAgrupamentoCarregamento) {
                return item;
            }
        });

        var selecionar = true;

        for (var i = 0; i < pedidos.length; i++) {
            var ped = pedidos[i];
            var pedidoAtualizar = $.extend({}, ped);

            if (i == 0) {
                selecionar = !pedidoAtualizar.Selecionado;
            }

            pedidoAtualizar.Selecionado = selecionar;
            pedidoAtualizar.PedidoSelecionadoCompleto = selecionar;

            PEDIDOS.replace(ped, pedidoAtualizar);

            if (selecionar) {
                if (isNaN(parseFloat(pedidoAtualizar.PalletPedidoCarregamento)) || parseFloat(pedidoAtualizar.PalletPedidoCarregamento) == 0)
                    pedidoAtualizar.PalletPedidoCarregamento = parseFloat(pedidoAtualizar.PalletSaldoRestante);

                PEDIDOS_SELECIONADOS.push(pedidoAtualizar);
            } else {
                PEDIDOS_SELECIONADOS.remove(function (p) { return p.Codigo == ped.Codigo });
            }
        }

    } else {
        SelecionarPedido(pedido, e);
    }
}

function anexosAgendamentoColetaPedidoClick() {
    _agendamentoColetaListaAnexos.Adicionar.visible(false);

    Global.abrirModal('divModalAnexoAgendamentoColeta');
    $("#divModalAnexoAgendamentoColeta").one("hidden.bs.modal", function () {
        Global.fecharModal('divModalAnexoAgendamentoColeta');
    });
}

function enviarEmailClick() {
    var data = { Codigo: _detalhePedido.Codigo.val(), Email: _detalhePedido.Email.val() };
    if (ValidarCamposObrigatorios(_detalhePedido)) {
        executarReST("MontagemCargaPedido/EnviarEmail", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.EmailEnviadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.CamposObrigatorios, Localization.Resources.Cargas.MontagemCarga.PorFavorPreenchaOsCamposObrigatorios);
    }
}

function eventClickDetalheClick(e) {
    ObterDetalhesPedido(e.Codigo.val());
}

function PedidosPesquisaScroll(e, sender) {
    if (_AreaPedido.TabelaPedidosVisivel.visible())
        return;

    let elem = null;
    if (sender && sender.currentTarget && sender.currentTarget.scrollHeight != null && sender.currentTarget.scrollTop != null && sender.currentTarget.offsetHeight != null)
        elem = sender.currentTarget;
    else if (sender && sender.target && sender.target.scrollHeight != null && sender.target.scrollTop != null && sender.target.offsetHeight != null)
        elem = sender.target;
    else {
        console.warn("Nenhum elemento de scroll encontrado.");
        return;
    }

    if (_AreaPedido.Fim.val() < _AreaPedido.Total.val() &&
        elem.scrollTop + 100 >= (elem.scrollHeight - elem.offsetHeight)) {
        PesquisarPedidos();
    }
}

//*******METODOS*******

function atualizarDadosPedidosSelecionados() {
    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i in pedidos) {
        var pedidoSelecionado = pedidos[i];

        PEDIDOS.update(function (pedido) { return pedido.Codigo == pedidoSelecionado.Codigo }, function (pedido) {
            pedido.DataPrevisaoEntrega = pedidoSelecionado.DataPrevisaoEntrega;
            pedido.NumeroReboque = pedidoSelecionado.NumeroReboque;
            pedido.NumeroReboqueDescricao = pedidoSelecionado.NumeroReboqueDescricao;
            pedido.TipoCarregamentoPedido = pedidoSelecionado.TipoCarregamentoPedido;
            pedido.TipoCarregamentoPedidoDescricao = pedidoSelecionado.TipoCarregamentoPedidoDescricao;

            return pedido;
        });
    }
}

function selecionarTodosPedidosClick() {
    var pedidos = PEDIDOS();
    var selecionar = !_AreaPedido.SelecionarTodos.val;

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;

    var pedidosSelecionados = (PEDIDOS_SELECIONADOS().length > 0);

    PEDIDOS_SELECIONADOS.removeAll();

    for (var i = 0; i < pedidos.length; i++) {
        var pedido = pedidos[i];
        var pedidoAtualizar = $.extend({}, pedido);

        if ((selecionar && pedido.PedidoBloqueado !== true) || !selecionar) {
            pedidoAtualizar.Selecionado = selecionar;

            PEDIDOS.replace(pedido, pedidoAtualizar);

            if (selecionar) {
                if (isNaN(parseFloat(pedidoAtualizar.PalletPedidoCarregamento)) || parseFloat(pedidoAtualizar.PalletPedidoCarregamento) == 0)
                    pedidoAtualizar.PalletPedidoCarregamento = parseFloat(pedidoAtualizar.PalletSaldoRestante);
                PEDIDOS_SELECIONADOS.push(pedidoAtualizar);
            }
        }
    }

    _AreaPedido.SelecionarTodos.val = selecionar;
    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
    _precisarSetarPedidosSelecionadosTabelaMontagemCarga = true;

    PedidosSelecionadosChange();
    PEDIDOS(pedidos);

    if (pedidosSelecionados || (selecionar && pedidos.length > 0))
        roteirizarAutomaticamenteAoAdicionarRemoverPedido();

    selecionarTodasNotasFiscais(selecionar);
}

function SelecionarPedido(_pedido, removerSelecao) {
    var pedidos = PEDIDOS();
    var pedido = PEDIDOS.where(function (ped) { return _pedido.Codigo == ped.Codigo; });
    var refPedido;
    var objPedido;

    for (var i in pedidos) {
        if (pedidos[i].Codigo == pedido.Codigo) {
            var refPedido = pedidos[i];
            var objPedido = $.extend({}, refPedido);
            break;
        }
    }

    if (pedido.Selecionado || removerSelecao === true) {
        objPedido.Selecionado = false;
        removerNotasFiscaisDoPedido(objPedido.Codigo);
    } else {
        objPedido.Selecionado = true;

        if (_carregamento.Carregamento.codEntity() <= 0 && PEDIDOS_SELECIONADOS().length == 0 && EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS === false) {
            _carregamentoTransporte.CodigoPedidoBase.val(_pedido.Codigo);
            BuscarCarregamentoPorPedido(pedido, function (encontrou, carregamento) {
                if (encontrou)
                    PreencherCarregamento(carregamento);
            });
        }
    }

    PEDIDOS.replace(refPedido, objPedido);
    if (pedido.Selecionado || removerSelecao === true) {
        PEDIDOS_SELECIONADOS.remove(function (ped) { return ped.Codigo == pedido.Codigo });
    } else {
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal == true && objPedido.CodigoPedidoViagemNavio > 0) {
            if (_carregamento.PedidoViagemNavio.codEntity() !== null && _carregamento.PedidoViagemNavio.codEntity() !== undefined && _carregamento.PedidoViagemNavio.codEntity() !== "" && _carregamento.PedidoViagemNavio.codEntity() !== "0" && _carregamento.PedidoViagemNavio.codEntity() > 0) {
                if (_carregamento.PedidoViagemNavio.codEntity() !== objPedido.CodigoPedidoViagemNavio) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, Localization.Resources.Cargas.MontagemCarga.PedidoSelecionadoNaoPossuiMesmaQuantidadeDoPedidoSelecionadoAnteriormente);
                    SelecionarPedido(objPedido, true);
                    return;
                } else {
                    _carregamento.PedidoViagemNavio.enable(false);
                }
            } else {
                _carregamento.PedidoViagemNavio.codEntity(objPedido.CodigoPedidoViagemNavio);
                _carregamento.PedidoViagemNavio.val(objPedido.PedidoViagemNavio);
                _carregamento.PedidoViagemNavio.enable(false);
            }
        }

        if (isNaN(parseFloat(objPedido.PalletPedidoCarregamento)) || parseFloat(objPedido.PalletPedidoCarregamento) == 0)
            objPedido.PalletPedidoCarregamento = parseFloat(objPedido.PalletSaldoRestante);

        PEDIDOS_SELECIONADOS.remove(function (ped) { return ped.Codigo == pedido.Codigo });
        PEDIDOS_SELECIONADOS.push(objPedido);
    }

    ValidarFronteira();
    setarRecebedorCarregamento(pedido);
    setarTipoOperacaoColetaTransporte(pedido);
    setarDataCarregamento(pedido);
    setarTransportadorCarregamentoTransporte(pedido);
    setarModeloVeicularCarga(pedido);
    setarTipoCondicaoCarregamento(pedido);
    roteirizarAutomaticamenteAoAdicionarRemoverPedido();

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && PEDIDOS_SELECIONADOS().length === 0) {
        LimparCampoEntity(_carregamento.PedidoViagemNavio);
        _carregamento.PedidoViagemNavio.enable(true);
    }
}

function ValidarFronteira() {
    var possuiFronteira = false;

    for (var i = 0; i < PEDIDOS_SELECIONADOS().length; i++) {
        if (((PEDIDOS_SELECIONADOS()[i].Importacao && PEDIDOS_SELECIONADOS()[i].CodigoExpedidor == 0) || PEDIDOS_SELECIONADOS()[i].ExpedidorExterior) || (PEDIDOS_SELECIONADOS()[i].Exportacao && PEDIDOS_SELECIONADOS()[i].CodigoRecebedor == 0)) {
            possuiFronteira = true;
            break;
        }
    }

    if (possuiFronteira) {
        _carregamentoTransporte.Fronteira.visible(true);
        _carregamentoTransporte.Fronteira.required(_CONFIGURACAO_TMS.FronteiraObrigatoriaMontagemCarga);
        _carregamentoTransporte.Fronteira.text(_carregamentoTransporte.Fronteira.required() ? "*Fronteiras:" : "Fronteiras:");
    }
    else {
        _carregamentoTransporte.Fronteira.visible(false);
        _carregamentoTransporte.Fronteira.required(false);
        _carregamentoTransporte.Fronteira.codEntity(0);
        _carregamentoTransporte.Fronteira.val("");
        _carregamentoTransporte.Fronteira.text("Fronteira:");
    }
}

function VerificarVisibilidadeBuscaSugestaoPedido() {
    if (PEDIDOS_SELECIONADOS().length == 1 && _carregamento.Carregamento.codEntity() == 0 && _carregamento.ModeloVeicularCarga.codEntity() > 0) {
        _carregamentoPedido.BuscarSugestaoPedidos.visible(true);
    }
    else {
        _carregamentoPedido.BuscarSugestaoPedidos.visible(false);
    }

    if (PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Peso) {
        _carregamento.PesoCarregamento.visible(true);
    } else {
        _carregamento.PesoCarregamento.visible(false);
    }

    if (PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Pallet) {
        _carregamento.PalletCarregamento.visible(true);
    } else {
        _carregamento.PalletCarregamento.visible(false);
    }
}

function BuscarCarregamentoPorPedido(pedido, callback) {
    var dados = { Codigo: pedido.Codigo, CarregamentoRedespacho: _objPesquisaMontagem.GerarCargasDeRedespacho };
    executarReST("MontagemCarga/BuscarPorPedido", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                //if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && arg.Data.CodigoPedidoViagemNavio > 0) {
                //    _carregamento.PedidoViagemNavio.codEntity(arg.Data.CodigoPedidoViagemNavio);
                //    _carregamento.PedidoViagemNavio.val(arg.Data.DescricaoPedidoViagemNavio);
                //    _carregamento.PedidoViagemNavio.enable(false);
                //}
                callback(arg.Data.encontrou, arg.Data.carregamento);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                callback(false);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            callback(false);
        }
    });
}

function ObterCodigoPedidosSelecionados() {
    return PEDIDOS_SELECIONADOS().map(function (pedido) {
        return pedido.Codigo;
    });
}

function ObterCodigoPedidos() {
    return PEDIDOS().map(function (pedido) {
        return pedido.Codigo;
    });
}

function ObterPedidosSelecionados() {
    return PEDIDOS_SELECIONADOS().map(function (pedido) {
        return {
            Codigo: pedido.Codigo,
            NumeroReboque: pedido.NumeroReboque,
            DataCarregamento: pedido.DataCarregamento,
            DataDescarregamento: pedido.DataDescarregamento,
            DataPrevisaoEntrega: pedido.DataPrevisaoEntrega,
            PesoPedidoCarregamento: pedido.PesoPedidoCarregamento,
            PalletPedidoCarregamento: pedido.PalletPedidoCarregamento,
            PesoTotal: pedido.Peso,
            PesoLiquido: pedido.PesoLiquido,
            CodRecebedor: pedido.CodRecebedor,
            TipoCarregamentoPedido: pedido.TipoCarregamentoPedido,
            Ordem: pedido.Ordem,
            TipoPaleteCliente: pedido.TipoPaleteCliente
        };
    });
}

var ObterPedidoPorCodigo = function (cod) {
    return PEDIDOS.where(function (ped) { return ped.Codigo == cod });
}

function ObterLocalidadePedido(pedido, distribuidor) {
    if (distribuidor)
        return pedido.CodigoRecebedor;

    return pedido.CodigoDestino;
}

function PontoTransbordoValidos(endereco) {
    var lat = endereco.LatitudeTransbordo || "";
    var lng = endereco.LongitudeTransbordo || "";

    if (lat == "" || lng == "")
        return false;

    return true;
}

function PontoValido(endereco) {

    if (endereco == null)
        return false;

    var lat = endereco.Latitude || "";
    var lng = endereco.Longitude || "";

    if (lat == "" || lng == "")
        return false;

    return true;
}

function VerificarPontosFaltantes(pedido) {
    if (!_objPesquisaMontagem.GerarCargasDeRedespacho && pedido.EnderecoRecebedor != null) {
        if (!PontoValido(pedido.EnderecoRecebedor))
            _pedidosSemPontos.push(pedido);
    } else if (!PontoValido(pedido.EnderecoDestino)) {
        _pedidosSemPontos.push(pedido);
    }
    //if ((!_objPesquisaMontagem.GerarCargasDeRedespacho && pedido.EnderecoRecebedor != null && !PontoValido(pedido.EnderecoRecebedor)) || !PontoValido(pedido.EnderecoDestino))
    //    _pedidosSemPontos.push(pedido);
}

function BuscarPedidos(callback) {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {
        var pedidosSelecionados = ObterCodigoPedidosSelecionados();
        var data = RetornarObjetoPesquisa(_pesquisaMontegemCarga);
        data.Selecionados = JSON.stringify(pedidosSelecionados);

        _AreaPedido.CarregandoPedidos.val(true);
        _pedidosSemPontos = new Array();

        executarReST("MontagemCargaPedido/ObterPedidos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var retorno = arg.Data;
                    PEDIDOS([]);

                    // Totalizadores.. rodapé
                    _AreaPedido.Total.val(retorno.Quantidade);
                    _AreaPedido.TotalPedidos.val(retorno.Quantidade);
                    _AreaPedido.PesoTotal.val(retorno.PesoTotal);
                    _AreaPedido.PesoLiquidoTotal.val(retorno.PesoLiquidoTotal);
                    _AreaPedido.PesoSaldoRestante.val(retorno.PesoSaldoRestante);
                    _AreaPedido.VolumeTotal.val(retorno.VolumeTotal);
                    _AreaPedido.SaldoVolumesRestante.val(retorno.SaldoVolumesRestante);
                    _AreaPedido.ExibirPercentualSeparacaoPedido.val(retorno.ExibirPercentualSeparacaoPedido);
                    _AreaPedido.ValorTotalDosPedidos.val(retorno.ValorTotalDosPedidos);


                    if (retorno.Quantidade == 0) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Atencao, Localization.Resources.Cargas.MontagemCarga.NaoExistemPedidosComOsFiltrosInformados);
                    }
                    for (var i = 0; i < retorno.Registros.length; i++) {

                        var pedido = retorno.Registros[i];

                        pedido.Selecionado = ($.inArray(pedido.Codigo, pedidosSelecionados) >= 0);
                        pedido.ExibirOrigem = retorno.ExibirCampoOrdem;

                        PEDIDOS.push(pedido);
                        VerificarPontosFaltantes(pedido);
                    }

                    if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente)
                        AtualizarPontosFaltantes();

                    fimListagemPedidos = 0;

                    if (_AreaPedido.TabelaPedidosVisivel.visible())
                        carregarTabelaPedidosMontagemCarga();

                    if (callback) callback();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, arg.Msg);
            }

            _AreaPedido.CarregandoPedidos.val(false);
        });
    } else if (callback) {
        callback();
    }
}

function CarregarListaPedidos(callback) {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {
        var pedidosSelecionados = ObterCodigoPedidosSelecionados();
        var data = RetornarObjetoPesquisa(_pesquisaMontegemCarga);
        data.Selecionados = JSON.stringify(pedidosSelecionados);

        _AreaPedido.CarregandoPedidos.val(false);
        _pedidosSemPontos = new Array();

        if (callback) callback();

    } else if (callback) {
        callback();
    }
}

function ObterDetalhesPedido(codigo) {
    carregarPedidoProduto(codigo, function () {
        executarReST("MontagemCargaPedido/BuscarPorCodigoPedido", { Codigo: codigo, Carregamento: _carregamento.Carregamento.codEntity() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    PreencherObjetoKnout(_detalhePedido, { Data: retorno.Data.DetalhesPedido });
                    PreencherObjetoKnout(_detalhePedidoExportacao, { Data: retorno.Data.DetalhesPedidoExportacao });
                    PreencherObjetoKnout(_detalhePedidoNotaFiscal, { Data: retorno.Data.DetalheNotaFiscalPedido });
                    PreencherObjetoKnout(_detalhePedidoProduto, { Data: retorno.Data });
                    preencherNotasFiscaisPedido(retorno.Data.NotasFiscais, retorno.Data.NotasFiscaisEnviar, codigo);
                    preencherAnexo(retorno.Data.ListaAnexosAgendamentoColeta);
                    $("#liDetalhePedidoProdutos").show();
                    loadGridProduto();
                    _detalhePedido.Anexos.visible(retorno.Data.ListaAnexosAgendamentoColeta.length > 0);

                    _detalhePedido.ModeloVeicularCarga.val(retorno.Data.DetalhesPedido.ModeloVeicularCarga.Descricao);

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
                        _detalhePedido.DataPrevisaoEntrega.visible(_detalhePedido.DataPrevisaoEntrega.val() != "");
                        _detalhePedido.DataPrevisaoEntrega.text(Localization.Resources.Cargas.MontagemCarga.DataEntrega.getFieldDescription());
                        _detalhePedido.DataInicialColeta.visible(_detalhePedido.DataInicialColeta.val() != "");
                    }

                    if (_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga && Boolean(retorno.Data.DetalhesPedidoTransporteMaritimo)) {
                        PreencherObjetoKnout(_detalhePedidoTransporteMaritimo, { Data: retorno.Data.DetalhesPedidoTransporteMaritimo });
                        $("#liDetalhePedidoTransporteMaritimo").show();
                    }
                    else
                        $("#liDetalhePedidoTransporteMaritimo").hide();

                    Global.abrirModal('modalDetalhePedido');
                    $("#modalDetalhePedido").one('hidden.bs.modal', function () {
                        salvarNotasSelecionadas();

                        LimparCampos(_detalhePedido);
                        LimparCampos(_detalhePedidoExportacao);
                        LimparCampos(_detalhePedidoTransporteMaritimo);
                        $("#liDetalhePedido a").tab("show");
                    });
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, retorno.Msg);
        });
    });
}

function LimparPedidosSelecionados() {
    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i in pedidos) {
        PEDIDOS.update(function (pedido) { return pedido.Codigo == pedidos[i].Codigo }, function (pedido) {
            pedido.Selecionado = false;
            return pedido;
        });
    }

    PEDIDOS_SELECIONADOS.removeAll();

    var pedidosNaoListados = PEDIDOS_NAO_LISTADOS();

    for (var j in pedidosNaoListados)
        PEDIDOS.remove(function (pedido) { return pedido.Codigo == pedidosNaoListados[j].Codigo });

    PEDIDOS_NAO_LISTADOS.removeAll();

    //#24725
    _AreaPedido.SelecionarTodos.val = false;

    $("#divAlertaPedidosCarregamento").hide();
}

function PesquisarPedidos() {
    if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.Todos || _CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.NovaCarga) {
        fimListagemPedidos += quantidadeLoadPedidos;
        _AreaPedido.Fim.val(fimListagemPedidos);
    }
}

function ObterPrimeiroRemetenteLista() {
    var _pedidosSelecionados = PEDIDOS_SELECIONADOS();
    var _cnpjRemetente = 0;

    for (var i in _pedidosSelecionados) {
        var pedidoSelecionado = _pedidosSelecionados[i];

        _cnpjRemetente = pedidoSelecionado.RemetenteCNPJ;

        break;
    }

    if (_carregamentoTransporte.TransportadorLocalCarregamentoRestringido.val())
        return _cnpjRemetente;
    else
        return 0;
}

function ValidarRecebedorRegiaoDestinoSelecionada(knout) {
    executarReST("MontagemCargaPedido/ObterRecebedorRegiaoDestino", { CodigoPedido: knout.Codigo }, function (retorno) {
        if (retorno.Data.Status) {
            var mapeamentoRecebedor = {
                Codigo: knout.Codigo,
                CodigoRecebedor: retorno.Data.CodRecebedor,
                Recebedor: retorno.Data.Recebedor
            };
            salvarDefinicaoRecebedorPedidoRegiaoDestino(mapeamentoRecebedor);
        }
        else if (retorno.Data.Erro)
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, retorno.Msg);
    });
}

function salvarDefinicaoRecebedorPedidoRegiaoDestino(knout) {
    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i = 0; i < pedidos.length; i++) {
        if (pedidos[i].Codigo == knout.Codigo) {
            pedidos[i].Recebedor = knout.Recebedor;
            pedidos[i].CodRecebedor = knout.CodigoRecebedor;
            break;
        }
    }

    RenderizarGridMotagemPedidos();
}