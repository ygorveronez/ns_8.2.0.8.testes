/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _AreaPedido;
var _filtroDetalhePedido;
var _detalhePedido;
var _detalhePedidoExportacao;
var _detalhePedidoTransporteMaritimo;
var _gridPedidosSessao;
var tiposPaletesClienteDetalhes = [];

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

var fimListagemPedidos = 0;
var quantidadeLoadPedidos = 36;
var _knoutsPedidos = [];

var AreaPedido = function () {
    this.Pedidos = PropertyEntity({ val: PEDIDOS, def: [] });
    this.CarregandoPedidos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Fim = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: PedidosPesquisaScroll });
    this.TotalPedidos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.TotalDePedidos.getFieldDescription() });

    this.PesoTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.MontagemCargaMapa.PesoTotalBarraSaldo.getFieldDescription(), visible: true });
    this.PesoLiquidoTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.MontagemCargaMapa.PesoLiquidoTotal.getFieldDescription(), visible: true });
    this.PesoSaldoRestante = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.VolumeTotal = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.MontagemCargaMapa.TotalDeVolumesBarraSaldo.getFieldDescription(), visible: true });
    this.SaldoVolumesRestante = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TotalPedidosSelecionados = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.PedidosSelecionados.getFieldDescription() });
    this.PesoTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.MontagemCargaMapa.PesoTotalBarraSaldoSelecionados.getFieldDescription(), visible: true });
    this.PesoLiquidoTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: Localization.Resources.Cargas.MontagemCargaMapa.PesoLiquidoTotalSelecionados.getFieldDescription(), visible: true });
    this.PesoSaldoRestanteSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.VolumeTotalSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.MontagemCargaMapa.TotalDeVolumesSelecionados.getFieldDescription(), visible: true });

    this.ValorTotalDosPedidos = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.ValorTotalDosPedidos.getFieldDescription()), visible: true });
    this.ValorPedidosSelecionados = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.decimal });

    //SIMONETTI
    this.TotalEntregas = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.TotalDeEntregas.getFieldDescription()) });
    this.TotalEntregasSelecionados = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.TotalDeEntregasSelecionados.getFieldDescription()) });
    this.TotalCubagem = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.TotalDeCubagem.getFieldDescription() });
    this.TotalCubagemSelecionados = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCargaMapa.TotalDeCubagemSelecionados.getFieldDescription() });

    this.PesquisarPedidos = PropertyEntity({ type: types.event, eventClick: pesquisarPedidosClick, text: Localization.Resources.Cargas.MontagemCargaMapa.OpcoesDeSessaoDeRoteirizacao, val: false, visible: true });
    this.RemoverCancelarPedidosSessao = PropertyEntity({ type: types.event, eventClick: removerCancelarPedidosSessaoClick, text: Localization.Resources.Cargas.MontagemCargaMapa.RemoverCancelarPedidos, val: false, visible: ko.observable(false) });
    this.RemoverPedidosSessao = PropertyEntity({ type: types.event, eventClick: removerPedidosSessaoClick, text: Localization.Resources.Cargas.MontagemCargaMapa.RemoverPedidos, val: false, visible: ko.observable(false) });
    this.CancelarReserva = PropertyEntity({ type: types.event, eventClick: cancelarReservaClick, text: Localization.Resources.Cargas.MontagemCargaMapa.CancelarReserva, val: false, visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, eventClick: selecionarTodosPedidosClick, text: Localization.Resources.Cargas.MontagemCargaMapa.MarcarTodos, val: false, visible: true });
    this.CanalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.CanalEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ChaveNotaFiscalEletronicaAreaPedido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ChaveNotaFiscalEletronica.getRequiredFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.MontagemCarga.AtivarMontagemCargaPorNFe), required: _CONFIGURACAO_TMS.MontagemCarga.AtivarMontagemCargaPorNFe, maxlength: 44 });
    this.SelecionarTodosMesmoAgrupamento = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.SelecionarTodosMesmoAgrupamento, def: false, enable: ko.observable(true) });

    this.ExibirPercentualSeparacaoPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TabelaPedidosVisivel = PropertyEntity({ visible: ko.observable(false) });

    this.GridPedidos = PropertyEntity({ type: types.local });
};

var FiltroDetalhePedido = function () {
    this.Pedidos = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoPedidoEmbarcador.getFieldDescription(), val: ko.observable(0), options: ko.observableArray([]), def: 0, visible: ko.observable(false) });

    this.Pedidos.val.subscribe(function (newValue) {
        var codigoPedido = parseInt(newValue);
        if (!isNaN(codigoPedido) && codigoPedido > 0)
            ObterDetalhesPedido(codigoPedido);
    });
};

var DetalhePedido = function (pedido) {
    this.DT_RowColor = PropertyEntity();
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataCarregamentoPedido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Data.getFieldDescription(), val: ko.observable("") });
    this.Filial = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Filial.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedido.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoPedidoEmbarcador.getFieldDescription(), val: ko.observable("") });
    this.Origem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Origem.getFieldDescription(), val: ko.observable("") });
    this.ExibirOrigem = PropertyEntity({ val: ko.observable(false) });
    this.Destino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Destino.getFieldDescription(), val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Remetente.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Expedidor.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Recebedor = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Recebedor.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DestinoRecebedor = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Destino.getFieldDescription(), val: ko.observable("") });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Peso.getFieldDescription(), val: ko.observable("") });
    this.PesoLiquido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PesoLiquido.getFieldDescription(), val: ko.observable("") });
    this.ExibirPesoSaldoRestante = PropertyEntity({ val: ko.observable(false) });
    this.PesoSaldoRestante = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Saldo.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.QuantidadeVolumes = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeVolumes.getFieldDescription(), val: ko.observable("") });
    this.Empresa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Transportador.getFieldDescription(), val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacao.getFieldDescription(), val: ko.observable("") });
    this.CanalEntrega = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CanalDeEntrega.getFieldDescription(), val: ko.observable("") });
    this.CanalVenda = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CanalVenda.getFieldDescription(), val: ko.observable("") });
    this.TotalPallets = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false, precision: 3 }, maxlength: 15, text: Localization.Resources.Cargas.MontagemCargaMapa.Pallets.getFieldDescription(), required: true, val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CubagemMetrosCubicos.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Restricao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.RestricaoDeEntrega.getFieldDescription(), val: ko.observable("") });
    this.ObservacaoRestricao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ObservacaoDaRestricao.getFieldDescription(), val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ObservacaoDoPedido.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.ObservacaoCliente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ObservacaoDoCliente.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCarga.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DescricaoTipoCondicaoPagamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDePagamento.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DescricaoTipoTomador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoTomador.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.SenhaAgendamentoCliente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.SenhaAgendamento.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DataInicialColeta = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataDaColeta.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataPrevistaDeEntregaBarraRetorno.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.ExigirPreCargaMontagemCarga = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false) });
    this.Temperatura = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Temperatura.getFieldDescription(), val: ko.observable("") });
    this.Vendedor = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Vendedor.getFieldDescription(), val: ko.observable("") });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Ordem.getFieldDescription(), val: ko.observable("") });
    this.PortoSaida = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDeSaida.getFieldDescription(), val: ko.observable("") });
    this.PortoChegada = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDeChegada.getFieldDescription(), val: ko.observable("") });
    this.Companhia = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Companhia.getFieldDescription(), val: ko.observable("") });
    this.NumeroNavio = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoNavio.getFieldDescription(), val: ko.observable("") });
    this.Reserva = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Reserva.getFieldDescription(), val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.Resumo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Resumo.getFieldDescription(), val: ko.observable("") });
    this.TipoEmbarque = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeEmbarque.getFieldDescription(), val: ko.observable("") });
    this.DeliveryTerm = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TermoDeEntrega.getFieldDescription(), val: ko.observable("") });
    this.IdAutorizacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.IDDeAutorizacao.getFieldDescription(), val: ko.observable("") });
    this.DataETA = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataETA.getFieldDescription(), val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.DataInclusaoBooking = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.InclusaoDoBooking.getFieldDescription(), val: ko.observable("") });
    this.DataInclusaoPCP = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.InclusaoDoPCP.getFieldDescription(), val: ko.observable("") });
    this.Selecionado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.SemLatLng = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PedidoPrioritario = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiAjudante = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PossuiAjudante, val: ko.observable("") });
    this.PossuiCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PossuiCarga, val: ko.observable("") });
    this.PossuiDescarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PossuiDescarga, val: ko.observable("") });
    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NotasFiscais.getFieldDescription(), val: ko.observable("") });
    this.CEPDestinatario = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CEP.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DataAgendamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataAgendamento.getFieldDescription(), val: ko.observable("") });
    this.GrupoPessoa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.GrupoPessoa.getFieldDescription(), val: ko.observable("") });
    this.CategoriaCliente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CategoriaCliente.getFieldDescription(), val: ko.observable("") });
    this.SituacaoComercial = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.SituacaoComercial.getFieldDescription(), val: ko.observable("") });

    this.PercentualSeparacaoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PedidoBloqueado = PropertyEntity({ val: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Bloqueado), cssClass: ko.observable("ribbon-tms ribbon-tms-red"), def: false, getType: typesKnockout.bool });
    this.PedidoRestricaoData = PropertyEntity({ val: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.RestricaoData), cssClass: ko.observable("ribbon-tms ribbon-tms-red"), def: false, getType: typesKnockout.bool });
    this.ExisteAlgumPercentualSeparacaoPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.InfoPedido = PropertyEntity({ eventClick: selecionarPedidoClick, eventClickDetalhe: eventClickDetalheClick, type: types.event, cssClass: ko.observable("card card-carga no-padding padding-5"), visibleSemLatLng: ko.observable(false) });

    this.Email = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Email.getRequiredFieldDescription()), issue: 30, required: true, getType: typesKnockout.email, maxlength: 1000 });
    this.EnviarEmail = PropertyEntity({ eventClick: enviarEmailClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.EnviarEmail });

    this.PedidoSelecionadoCompleto = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PedidoIntegradoCarregamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.QuantidadeBipada = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.QuantidadeBipagemTotal = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.VolumesBipagem = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.text });
    this.Saldo = PropertyEntity({ text: "Saldo", val: ko.observable("") });

    this.CodigoPedidoCliente = PropertyEntity({ text: "N° Pedido Cliente:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataAgendamento = PropertyEntity({ text: "Data Agendamento:", val: ko.observable(""), visible: ko.observable(true) });
    this.SenhaAgendamento = PropertyEntity({ text: "Senha Agendamento:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ text: "Vendedor:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Gerente = PropertyEntity({ text: "Gerente:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Supervisor = PropertyEntity({ text: "Supervisor:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.ValorTotalPedido = PropertyEntity({ text: "Valor Total Mercadorias:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.ObservacaoPedido = PropertyEntity({ text: "Observações Pedido:", val: ko.observable(""), def: "", visible: ko.observable(true), enable: ko.observable(false) });
    this.ObservacaoInterna = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ObservacaoInterna.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(false) });


    PreencherObjetoKnout(this, { Data: pedido });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        this.PesoSaldoRestante.visible(false);
        this.Peso.val(pedido.PesoSaldoRestante);
        this.NumeroPedido.visible(true);
        this.InfoPedido.val(pedido.PedidoPrioritario);
    }

    if (pedido.PedidoBloqueado && !pedido.LiberadoMontagemCarga)
        this.PedidoBloqueado.text(Localization.Resources.Cargas.MontagemCargaMapa.PedidoBloqueadoNaoLiberado);//        this.PedidoBloqueado.text('Bloq - N. LIB.');
    else if (!pedido.LiberadoMontagemCarga) {
        this.PedidoBloqueado.text('N. LIBERADO');
        this.PedidoBloqueado.val(true);
    }
    else if (pedido.PedidoBloqueado)
        this.PedidoBloqueado.text('Bloqueado');

    this.ExisteAlgumPercentualSeparacaoPedido.val(_AreaPedido.ExibirPercentualSeparacaoPedido.val());
    this.HabilitaEdicaoQuantidades = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.MontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa) });

    this.NumeroPaletesFracionado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false, precision: 3 }, maxlength: 15, text: Localization.Resources.Cargas.MontagemCargaMapa.Pallets.getFieldDescription(), required: true, val: ko.observable("") });
    this.TipoPaleteCliente = PropertyEntity({ text: "Tipo Pallet:", val: ko.observable(EnumTipoPalletCliente.NaoDefinido), options: EnumTipoPalletCliente.obterOpcoes(), def: EnumTipoPalletCliente.NaoDefinido, visible: ko.observable(true) });
    this.PesoTotalPaletes = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Peso Palete:", required: true });
};

var DetalhePedidoExportacao = function () {
    this.AcondicionamentoCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.AcondicionamentoDaCarga.getFieldDescription() });
    this.CargaPaletizada = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CargaPaletizada.getFieldDescription() });
    this.ClienteAdicional = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ClienteAdicional.getFieldDescription() });
    this.ClienteDonoContainer = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ClienteDonoDoContainer.getFieldDescription() });
    this.DataDeadLCargaNavioViagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DeadLineDaCarga.getFieldDescription() });
    this.DataDeadLineNavioViagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Deadline.getFieldDescription() });
    this.DataEstufagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataDaEstufagem.getFieldDescription() });
    this.Despachante = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Despachante.getFieldDescription() });
    this.ETA = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETA.getFieldDescription() });
    this.ETS = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETS.getFieldDescription() });
    this.InLand = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.InLand.getFieldDescription() });
    this.NavioViagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NavioDaViagem.getFieldDescription() });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoBooking.getFieldDescription() });
    this.NumeroEXP = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroEXP.getFieldDescription() });
    this.NumeroPedidoProvisorio = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoPedidoProvisorio.getFieldDescription() });
    this.NumeroReserva = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDaReserva.getFieldDescription() });
    this.PortoViagemDestino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDoDestino.getFieldDescription() });
    this.PortoViagemOrigem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDoOrigem.getFieldDescription() });
    this.PossuiGenset = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PossuiGenset.getFieldDescription() });
    this.RefEXPTransferencia = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.RefExpTransferencia.getFieldDescription() });
    this.TipoContainer = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDoContainer.getFieldDescription() });
    this.TipoProbe = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoProbe.getFieldDescription() });
    this.ViaTransporte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ViaTransporte.getFieldDescription() });

    //Tipo Container
}

var DetalhePedidoTransporteMaritimo = function () {
    this.CodigoArmador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CodigoDoArmador.getFieldDescription() });
    this.CodigoNCM = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CodigoNCM.getFieldDescription() });
    this.CodigoRota = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CodigoDaRota.getFieldDescription() });
    this.CodigoIdentificacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DescricaoDaCargHifenID.getFieldDescription() });
    this.DescricaoIdentificacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DescricaoDacargaHifenDescricao.getFieldDescription() });
    this.Incoterm = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Incoterm.getFieldDescription() })
    this.MensagemTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.MensagemTransbBarraArm.getFieldDescription() });
    this.MetragemCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.MetragemDaCarga.getFieldDescription() });
    this.ModoTransporte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ModoDeTransporte.getFieldDescription() });
    this.NomeNavio = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NomeDoNavio.getFieldDescription() });
    this.NomeNavioTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NomeDoNavioDeTrabsbordo.getFieldDescription() });
    this.NumeroBL = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroBL.getFieldDescription() });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoContainer.getFieldDescription() });
    this.NumeroLacre = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoLacre.getFieldDescription() });
    this.NumeroViagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDaViagem.getFieldDescription() });
    this.NumeroViagemTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDaViagemDeTransbordo.getFieldDescription() });
    this.DataBooking = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataDoBookingBarraDataDaReserva.getFieldDescription() });
    this.DataETAOrigem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETAOrigem.getFieldDescription() });
    this.DataETASegundaOrigem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.EtaOrigemDois.getFieldDescription() });
    this.DataETAOrigemFinal = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETAOrigemFinal.getFieldDescription() });
    this.DataETADestino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETADestino.getFieldDescription() });
    this.DataETASegundoDestino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETADestinoDois.getFieldDescription() });
    this.DataETADestinoFinal = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETADestinoFinal.getFieldDescription() });
    this.DataETATransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETADoTransbordo.getFieldDescription() });
    this.DataETS = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETS.getFieldDescription() });
    this.DataETSTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ETSDoTransbordo.getFieldDescription() });
    this.DataDepositoContainer = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DepositoDoContainer.getFieldDescription() });
    this.DataRetiradaContainerDestino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.RetiradaDoContainerNoDestino.getFieldDescription() });
    this.DataRetiradaVazio = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.RetiradaDoVazio.getFieldDescription() });
    this.DataRetornoVazio = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.RetornoDoVazio.getFieldDescription() });
    this.DataDeadLineCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DeadLineDaCarga.getFieldDescription() });
    this.DataDeadLineDraf = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DeadLineDraf.getFieldDescription() });
    this.CodigoPortoCarregamentoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDeCarregamentoDeTransbordoHifenCodigo.getFieldDescription() });
    this.DescricaoPortoCarregamentoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDeCarregamentoDeTransbordoHifenDescricao.getFieldDescription() });
    this.CodigoPortoDestinoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDeDestinoDeTransbordoHifenCodigo.getFieldDescription() });
    this.DescricaoPortoDestinoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDeDestinoDeTransbordoHifenDescricao.getFieldDescription() });
    this.TerminalOrigem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TerminalPortuarioDeSaida.getFieldDescription() });
    this.TipoEnvio = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDoEnvio.getFieldDescription() });
    this.TipoTransporte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeTransporte.getFieldDescription() });
    this.Transbordo = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Transbordo.getFieldDescription() });
}

var Botoes = function () {
    this.HabilitaEdicaoQuantidades = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.MontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa) });
    this.AtualizarQuantidadesPedido = PropertyEntity({ type: types.event, eventClick: atualizarQuantidadesPedidoClick, text: Localization.Resources.Gerais.Geral.Salvar, val: false, visible: true });
}

//*******EVENTOS*******

function loadDetalhesPedido() {
    _AreaPedido = new AreaPedido();
    KoBindings(_AreaPedido, "knoutAreaPedido");

    new BuscarCanaisEntrega(_AreaPedido.CanalEntrega, null, null, callbackRetornoMultiplaEscolha);

    _detalhePedido = new DetalhePedido({});
    KoBindings(_detalhePedido, "knoutDetalhePedido");

    _filtroDetalhePedido = new FiltroDetalhePedido();
    KoBindings(_filtroDetalhePedido, "knoutFiltroDetalhePedido");

    _detalhePedidoExportacao = new DetalhePedidoExportacao();
    KoBindings(_detalhePedidoExportacao, "knoutDetalhePedidoExportacao");

    _detalhePedidoTransporteMaritimo = new DetalhePedidoTransporteMaritimo();
    KoBindings(_detalhePedidoTransporteMaritimo, "knoutDetalhePedidoTransporteMaritimo");

    _botoes = new Botoes();
    KoBindings(_botoes, "knoutBotoes");

    if (_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga)
        $("#liDetalhePedidoExportacao").show();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _detalhePedido.DataPrevisaoEntrega.visible(true);
        _detalhePedido.DataInicialColeta.visible(true);
        _detalhePedido.Observacao.visible(true);
        _detalhePedido.Filial.visible(false);
        $("#lcPedidosPrioritarios").show();
    }
    else {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _detalhePedido.DataPrevisaoEntrega.visible(true);
            _detalhePedido.Cubagem.visible(true);
        }

        $("#lcPedidosPrioritarios").hide();
    }

    BiparNotas();

    RegistraComponente();

    controleApresentacaoPedidos();

    buscarTiposPaletesDetalhes();
}

function controleApresentacaoPedidos() {
    //if (_sessaoRoteirizador.Codigo.val() == 0)
    //    return;

    if (_AreaPedido.TabelaPedidosVisivel.visible()) {
        inicializarTabelaPedidosSessao();
        //} else {
        //    RegistraComponente();
    }
}

function callbackRetornoMultiplaEscolha(data) {

    var codigos = data.map(a => parseInt(a.Codigo));

    var pedidos = PEDIDOS();
    var selecionar = true;

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;

    PEDIDOS_SELECIONADOS.removeAll();
    for (var i = 0; i < pedidos.length; i++) {
        var pedido = pedidos[i];
        if (codigos.includes(pedido.CodigoCanalEntrega)) {
            var pedidoAtualizar = $.extend({}, pedido);

            pedidoAtualizar.Selecionado = selecionar;

            PEDIDOS.replace(pedido, pedidoAtualizar);

            if (selecionar) {
                PEDIDOS_SELECIONADOS.push(pedidoAtualizar);
            }
        }
    }

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
    _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;
    PedidosSelecionadosChange();
    PEDIDOS(pedidos);
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

function inicializarTabelaPedidosSessao() {

    const header = [
        { data: "Codigo", visible: false },
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoPedidoEmbarcador, width: "5%", widthDefault: "5%", visible: true },
        { data: "DataCarregamentoPedido", title: Localization.Resources.Cargas.MontagemCargaMapa.Data, width: "5%", widthDefault: "5%", visible: true },
        { data: "Filial", title: Localization.Resources.Cargas.MontagemCargaMapa.Filial, width: "5%", widthDefault: "5%", visible: true },
        { data: "Remetente", title: Localization.Resources.Cargas.MontagemCargaMapa.Remetente, width: "5%", widthDefault: "5%", visible: true },
        { data: "Expedidor", title: Localization.Resources.Cargas.MontagemCargaMapa.Expedidor, width: "5%", widthDefault: "5%", visible: true },
        { data: "Destinatario", title: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario, width: "5%", widthDefault: "5%", visible: true },
        { data: "Recebedor", title: Localization.Resources.Cargas.MontagemCargaMapa.Recebedor, width: "5%", widthDefault: "5%", visible: true },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCargaMapa.Destino, width: "5%", widthDefault: "5%", visible: true },
        { data: "DestinoRecebedor", title: Localization.Resources.Cargas.MontagemCargaMapa.Destino, width: "5%", widthDefault: "5%", visible: true },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCargaMapa.Peso, width: "5%", widthDefault: "5%", visible: true },
        { data: "PesoLiquido", title: Localization.Resources.Cargas.MontagemCargaMapa.PesoLiquidoTotal, width: "5%", widthDefault: "5%", visible: true },
        { data: "PesoSaldoRestante", title: Localization.Resources.Cargas.MontagemCargaMapa.Saldo, width: "5%", widthDefault: "5%", visible: true },
        { data: "QuantidadeVolumes", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeVolumes, width: "5%", widthDefault: "5%", visible: true },
        { data: "Ordem", title: Localization.Resources.Cargas.MontagemCargaMapa.Ordem, width: "5%", widthDefault: "5%", visible: true },
        { data: "NumeroPedido", title: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedido, width: "5%", widthDefault: "5%", visible: true },
        { data: "DataPrevisaoEntrega", title: Localization.Resources.Cargas.MontagemCargaMapa.DataPrevistaDeEntregaBarraRetorno, width: "5%", widthDefault: "5%", visible: true },
        { data: "Cubagem", title: Localization.Resources.Cargas.MontagemCargaMapa.CubagemMetrosCubicos, width: "5%", widthDefault: "5%", visible: true },
        { data: "PercentualSeparacaoPedido", title: "Percentual", width: "5%", widthDefault: "5%", visible: true },
        { data: "DataAgendamento", title: Localization.Resources.Cargas.MontagemCargaMapa.DataAgendamento, width: "5%", widthDefault: "5%", visible: true },
        { data: "RotaFrete.Descricao", title: Localization.Resources.Cargas.MontagemCargaMapa.Rotas, width: "5%", widthDefault: "5%", visible: true },
        { data: "Carregamentos", title: Localization.Resources.Cargas.MontagemCargaMapa.Carregamentos, width: "5%", widthDefault: "5%", visible: true },
        { data: "CanalEntrega", title: Localization.Resources.Cargas.MontagemCargaMapa.CanalEntrega, width: "5%", widthDefault: "5%", visible: true },
        { data: "TotalPallets", title: Localization.Resources.Cargas.MontagemCargaMapa.Pallet, width: "5%", widthDefault: "5%", visible: true },
        { data: "ObservacaoDestinatario", title: Localization.Resources.Cargas.MontagemCargaMapa.ObservacaoCliente, width: "5%", widthDefault: "5%", visible: true },
        { data: "GrupoPessoa", title: Localization.Resources.Cargas.MontagemCargaMapa.GrupoPessoa, width: "5%", widthDefault: "5%", visible: true },
        { data: "PrazoEntrega", title: Localization.Resources.Cargas.MontagemCargaMapa.PrazoEntrega, width: "5%", widthDefault: "5%", visible: true },
        { data: "ValorFrete", title: Localization.Resources.Cargas.MontagemCargaMapa.ValorFrete, width: "5%", widthDefault: "5%", visible: true },
        { data: "CanalVenda", title: Localization.Resources.Cargas.MontagemCargaMapa.CanalVenda, width: "5%", widthDefault: "5%", visible: true },
        { data: "PedidoRestricaoData", title: Localization.Resources.Cargas.MontagemCargaMapa.RestricaoData, width: "5%", widthDefault: "5%", visible: true },
        { data: "PedidoBloqueadoNaoLiberado", title: Localization.Resources.Cargas.MontagemCargaMapa.PedidoBloqueadoNaoLiberado, width: "5%", widthDefault: "5%", visible: true },
        { data: "CodigoAgrupamentoCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.CodigoAgrupamentoCarregamento, width: "5%", widthDefault: "5%", visible: true },
        { data: "DestinatarioNomeFantasia", title: Localization.Resources.Cargas.MontagemCargaMapa.DestinatarioNomeFantasia, width: "5%", widthDefault: "5%", visible: true },
        { data: "SituacaoComercial", title: Localization.Resources.Cargas.MontagemCargaMapa.SituacaoComercial, width: "5%", widthDefault: "5%", visible: true },
        { data: "SituacaoEstoque", title: Localization.Resources.Cargas.MontagemCargaMapa.SituacaoEstoque, width: "5%", widthDefault: "5%", visible: true },
        { data: "TipoOperacao", title: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacao, width: "5%", widthDefault: "5%", visible: true },
        { data: "ValorTotalNotasFiscais", title: Localization.Resources.Cargas.MontagemCargaMapa.ValorMercadoria, width: "5%", widthDefault: "5%", visible: true },
        { data: "NumeroOrdem", title: Localization.Resources.Cargas.MontagemCargaMapa.NumeroOrdem, width: "5%", widthDefault: "5%", visible: true },
        { data: "Reentrega", title: Localization.Resources.Cargas.MontagemCargaMapa.Reentrega, width: "5%", widthDefault: "5%", visible: true },
    ];

    const opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesPedidoTabelaPedidosSessaoClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };

    var configExportacao = {
        url: "MontagemCargaPedido/ExportarPesquisa",
        btnText: "Exportar excel (Dados Salvos)",
        funcaoObterParametros: function () {
            return RetornarObjetoPesquisa(_pesquisaMontegemCarga);
        }
    }

    _gridPedidosSessao = new BasicDataTable(_AreaPedido.GridPedidos.id, header, menuOpcoes, { column: 1, dir: "asc" }, configRowsSelect, 100, null, null, null, null, null, null, configExportacao, null, callbackBoolToString, true, tablePedidosSessaoRegistroSelecionadoChange, selecionarTodosPedidosClick, "Cargas/MontagemCargaMapa", "grid-montagem-carga-pedido");
    _gridPedidosSessao.SetPermitirEdicaoColunas(true);
    _gridPedidosSessao.SetSalvarPreferenciasGrid(true);
    _gridPedidosSessao.SetHabilitarScrollHorizontal(true, 150);
}

function callbackBoolToString(head, data, row) {
    if (head.name == 'PedidoRestricaoData' || head.name == 'PedidoBloqueadoNaoLiberado' || head.name == 'Reentrega') {
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

function carregarTabelaPedidosSessao() {
    if (_gridPedidosSessao)
        _gridPedidosSessao.CarregarGrid(PEDIDOS());
}

function setarPedidosSelecionadosTabelaPedidosSessao() {
    if (_sessaoRoteirizador.Codigo.val() > 0 && _sessaoRoteirizador.TipoPedidoMontagemCarregamento.val() == EnumTipoPedidoMontagemCarregamento.Tabela)
        _gridPedidosSessao.SetarSelecionados(PEDIDOS_SELECIONADOS());
}

function detalhesPedidoTabelaPedidosSessaoClick(pedidoSelecionado) {
    if (_filtroDetalhePedido)
        _filtroDetalhePedido.Pedidos.visible(false);
    ObterDetalhesPedido(pedidoSelecionado.Codigo);
}

function tablePedidosSessaoRegistroSelecionadoChange(registro, selecionado) {
    _precisarSetarPedidosSelecionadosTabelaPedidosSessao = false;
    selecionaPedido(registro, !selecionado);
}

function selecionarPedidoClick(e) {
    const pedido = RetornarObjetoPesquisa(e);

    selecionaPedido(pedido, e.Selecionado.val())
}

function selecionaPedido(pedido, removerSelecao) {
    if (pedido.PedidoBloqueado && !_CONFIGURACAO_TMS.MontagemCarga.PermitirGerarCarregamentoPedidoBloqueado) return;

    //#49608-SIMONETTI
    if (!pedido.CodigoAgrupamentoCarregamento || !_AreaPedido.SelecionarTodosMesmoAgrupamento.val()) {
        SelecionarPedido(pedido, removerSelecao);
        const index = obterIndiceMakerPedido(pedido);
        if (index >= 0) {
            if (parseInt(_arrayMarker[index].codigo_carregamento) > 0) {
                removerCarregamentoPedidos(parseInt(_arrayMarker[index].codigo_carregamento), [pedido.Codigo]);
            }
            const new_nro_carregamento = _carregamento.Carregamento.codEntity();
            // Se está adicionando em novo carregamento e o nro for diferente do carregametno anterior...
            if (!removerSelecao && new_nro_carregamento > 0) {
                _arrayMarker[index].codigo_carregamento = new_nro_carregamento;
            } else {
                _arrayMarker[index].codigo_carregamento = 0;
            }

        }

        PedidosSelecionadosChange();
        return;
    }

    const pedidos = ko.utils.arrayFilter(PEDIDOS(), function (item) {
        if (item.CodigoAgrupamentoCarregamento == pedido.CodigoAgrupamentoCarregamento) {
            return item;
        }
    });

    let selecionar = true;

    for (let i = 0; i < pedidos.length; i++) {
        const ped = pedidos[i];
        let pedidoAtualizar = $.extend({}, ped);

        if (i == 0) {
            selecionar = !pedidoAtualizar.Selecionado;
        }

        pedidoAtualizar.Selecionado = selecionar;
        pedidoAtualizar.PedidoSelecionadoCompleto = selecionar;

        PEDIDOS.replace(ped, pedidoAtualizar);
        if (selecionar) {
            const exists = PEDIDOS_SELECIONADOS().find(p => p.Codigo === pedidoAtualizar.Codigo);
            if (!exists)
                PEDIDOS_SELECIONADOS.push(pedidoAtualizar);
        } else {
            PEDIDOS_SELECIONADOS.remove(function (p) { return p.Codigo == ped.Codigo });
        }
    }

    if (_AreaPedido.TabelaPedidosVisivel.visible() && pedidos.length > 1) {
        setarPedidosSelecionadosTabelaPedidosSessao();
    }
}

function fecharModalPedidoClick(e, sender) {
    Global.fecharModal('modalDetalhePedido');
}

function atualizouQuantidadesPedido(dados) {
    // Atualizar informações de TipoPaleteCliente, NumeroPaletesFracionado, PesoTotalPaletes do PEDIDO e PEDIDO_SELECIONADOS    
    let pedido = PEDIDOS().find(p => p.Codigo === dados.CodigoPedido);
    if (pedido) {
        pedido.TipoPaleteCliente = dados.TipoPaleteCliente;
        pedido.NumeroPaletesFracionado = dados.NumeroPaletesFracionado;
        pedido.PesoTotalPaletes = dados.PesoTotalPaletes;
        pedido.Peso = dados.Peso;
        pedido.PesoSaldoRestante = dados.PesoSaldoRestante;
        if (_AreaPedido.TabelaPedidosVisivel.visible()) {
            carregarTabelaPedidosSessao();
            setarPedidosSelecionadosTabelaPedidosSessao();
        }
    }

    let pedidoSelecionado = PEDIDOS_SELECIONADOS().find(p => p.Codigo === dados.CodigoPedido);
    if (pedidoSelecionado) {
        pedidoSelecionado.TipoPaleteCliente = dados.TipoPaleteCliente;
        pedidoSelecionado.NumeroPaletesFracionado = dados.NumeroPaletesFracionado;
        pedidoSelecionado.PesoTotalPaletes = dados.PesoTotalPaletes;
        pedidoSelecionado.Peso = dados.Peso;
        pedidoSelecionado.PesoSaldoRestante = dados.PesoSaldoRestante;
    }
}

function atualizarQuantidadesPedidoClick() {
    if (!_CONFIGURACAO_TMS.MontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa) return;

    var produtos = _gridDetalhesPedidoProdutos.BuscarRegistros();

    var dados = {
        ProdutosPedido: JSON.stringify(produtos),
        CodigoPedido: _detalhePedido.Codigo.val(),
        PesoTotalPaletes: _detalhePedido.PesoTotalPaletes.val(),
        NumeroPaletesFracionado: _detalhePedido.NumeroPaletesFracionado.val(),
        TipoPaleteCliente: _detalhePedido.TipoPaleteCliente.val(),
        SessaoRoteirizador: _sessaoRoteirizador.Codigo.val()
    }

    executarReST("GestaoPedido/AtualizarPedido", dados, function (retorno) {
        if (retorno.Success) {

            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
            fecharModalPedidoClick();

            atualizouQuantidadesPedido(retorno.Data);

            // Caso algum Carregamento envolvido
            if (retorno.Data.Carregamentos.length > 0) {
                buscarCarregamentos();
                if (_carregamento.Carregamento.codEntity() != 0) {
                    if (retorno.Data.Carregamentos.includes(_carregamento.Carregamento.codEntity())) {
                        retornoCarregamento({ Codigo: _carregamento.Carregamento.codEntity() });
                    }
                }
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
    });
}

function enviarEmailClick() {
    var data = { Codigo: _detalhePedido.Codigo.val(), Email: _detalhePedido.Email.val() };
    if (ValidarCamposObrigatorios(_detalhePedido)) {
        executarReST("MontagemCargaPedido/EnviarEmail", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.EmailEnviadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
}

function eventClickDetalheClick(e) {
    if (_filtroDetalhePedido)
        _filtroDetalhePedido.Pedidos.visible(false);
    ObterDetalhesPedido(e.Codigo.val());
}

function PedidosPesquisaScroll(e, sender) {
    if (_AreaPedido.TabelaPedidosVisivel.visible())
        return;
    var elem = sender.target;
    if (_AreaPedido.Fim.val() < _AreaPedido.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight - 5)) {
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

function pesquisarPedidosClick() {
    modalPesquisar();
}

function removerCancelarPedidosSessaoClick() {
    var pedidos = PEDIDOS_SELECIONADOS();
    removerPedidoSessao(pedidos, false, true);
}

function removerPedidosSessaoClick() {
    var pedidos = PEDIDOS_SELECIONADOS();
    removerPedidoSessao(pedidos, false, false);
}

function cancelarReservaClick() {
    var pedidos = PEDIDOS_SELECIONADOS();
    removerPedidoSessao(pedidos, true, false);
}

function opcoesPedidosSelecionados() {
    if (PEDIDOS_SELECIONADOS().length == 0) {
        _AreaPedido.RemoverCancelarPedidosSessao.visible(false);
        _AreaPedido.RemoverPedidosSessao.visible(false);
        _AreaPedido.CancelarReserva.visible(false);
    } else {
        _AreaPedido.RemoverCancelarPedidosSessao.visible(_CONFIGURACAO_TMS.MontagemCarga.ApresentaOpcaoRemoverCancelarPedidos);
        _AreaPedido.RemoverPedidosSessao.visible(true);
        _AreaPedido.CancelarReserva.visible(_CONFIGURACAO_TMS.MontagemCarga.ApresentaOpcaoCancelarReserva);
    }
}

function selecionarTodosPedidosClick() {
    const pedidos = PEDIDOS();
    const selecionar = !_AreaPedido.SelecionarTodos.val;

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;

    PEDIDOS_SELECIONADOS.removeAll();
    for (let i = 0; i < pedidos.length; i++) {
        const pedido = pedidos[i];
        let pedidoAtualizar = $.extend({}, pedido);

        pedidoAtualizar.Selecionado = selecionar;
        pedidoAtualizar.PedidoSelecionadoCompleto = selecionar;

        PEDIDOS.replace(pedido, pedidoAtualizar);

        if (selecionar) {
            PEDIDOS_SELECIONADOS.push(pedidoAtualizar);
        } else {
            PEDIDOS_SELECIONADOS.remove(function (p) { return p.Codigo == pedido.Codigo });
        }
    }

    _AreaPedido.SelecionarTodos.val = selecionar;
    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
    _precisarSetarPedidosSelecionadosTabelaPedidosSessao = true;

    PedidosSelecionadosChange();
    PEDIDOS(pedidos);
}

function SelecionarPedido(_pedido, removerSelecao) {
    let pedido = PEDIDOS.where(function (ped) { return _pedido.Codigo == ped.Codigo; });
    let refPedido = pedido;
    let objPedido = $.extend({}, refPedido);

    if ((pedido.Selecionado && pedido.QuantidadeBipada <= 0) || removerSelecao === true) {
        objPedido.Selecionado = false;
        objPedido.PedidoSelecionadoCompleto = false;
        objPedido.QuantidadeBipada = 0;
        objPedido.VolumesBipagem = "0";
        pedido.VolumesBipagem = "0";
    } else {
        objPedido.Selecionado = true;
        objPedido.PedidoSelecionadoCompleto = true;
        if (_carregamento.Carregamento.codEntity() <= 0 && PEDIDOS_SELECIONADOS().length == 0 && EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS === false) {
            BuscarCarregamentoPorPedido(pedido, function (retorno) {
                if (retorno.Carregamento != null)
                    PreencherCarregamento(retorno.Carregamento);

                if (retorno.CentroCarregamento != null)
                    ConfigurarCentroCarregamento(retorno.CentroCarregamento);
            });
        }
    }
    if (pedido.QuantidadeBipada > 0) {
        objPedido.QuantidadeBipada = pedido.QuantidadeBipada;
        if ((pedido.QuantidadeBipagemTotal - pedido.QuantidadeBipada) <= 0) {
            //selecionou todos os volumes do pedido, deve ficar azul
            objPedido.PedidoSelecionadoCompleto = true;
        }
        else {
            //ainda nao selecionou todos os volumes do pedido.. tem q ficar amarelo..
            objPedido.PedidoSelecionadoCompleto = false;
        }
        objPedido.VolumesBipagem = pedido.QuantidadeBipada + "/" + pedido.QuantidadeBipagemTotal;
        pedido.VolumesBipagem = pedido.QuantidadeBipada + "/" + pedido.QuantidadeBipagemTotal;
    }

    objPedido.PedidoIntegradoCarregamento = pedido.PedidoIntegradoCarregamento;

    PEDIDOS.replace(refPedido, objPedido);
    if ((pedido.Selecionado && pedido.QuantidadeBipada <= 0) || removerSelecao === true) {
        objPedido.QuantidadeBipada = 0;
        objPedido.VolumesBipagem = "0";
        pedido.VolumesBipagem = "0";
        if (objPedido.PedidoIntegradoCarregamento) {
            let codigos = [];
            codigos.push(objPedido.Codigo);
            removerPedidosCarregamento(_carregamento.Carregamento.codEntity(), codigos, function () { });
        }
        PEDIDOS_SELECIONADOS.remove(function (ped) { return ped.Codigo == pedido.Codigo });
        if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val() == true && !objPedido.PedidoIntegradoCarregamento) {
            try {
                const demaisProdutos = _gridProdutosCarregamento.BuscarRegistros().filter(function (ped) { return pedido.Codigo != ped.CodigoPedido; });
                _gridProdutosCarregamento.CarregarGrid(demaisProdutos);
            } catch { }
            let codigos = [];
            codigos.push(objPedido.Codigo);
            removerPedidosCarregamento(_carregamento.Carregamento.codEntity(), codigos, function () {
                retornoCarregamento({ Codigo: _carregamento.Carregamento.codEntity() });
            });
        }
    } else {
        if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal == true && objPedido.CodigoPedidoViagemNavio > 0) {
            if (_carregamento.PedidoViagemNavio.codEntity() !== null && _carregamento.PedidoViagemNavio.codEntity() !== undefined && _carregamento.PedidoViagemNavio.codEntity() !== "" && _carregamento.PedidoViagemNavio.codEntity() !== "0" && _carregamento.PedidoViagemNavio.codEntity() > 0) {
                if (_carregamento.PedidoViagemNavio.codEntity() !== objPedido.CodigoPedidoViagemNavio) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.MontagemCargaMapa.PedidoSelecionadoNaoPossuiMesmaViagemDoPedidoSelecionadoanteriormente);
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

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && PEDIDOS_SELECIONADOS().length === 0) {
        LimparCampoEntity(_carregamento.PedidoViagemNavio);
        _carregamento.PedidoViagemNavio.enable(true);
    }
    opcoesPedidosSelecionados();
}

function ValidarFronteira() {
    var possuiFronteira = false;
    var fronteira = null;

    for (var i = 0; i < PEDIDOS_SELECIONADOS().length; i++) {
        if (PEDIDOS_SELECIONADOS()[i].Importacao || (PEDIDOS_SELECIONADOS()[i].Exportacao && PEDIDOS_SELECIONADOS()[i].CodigoRecebedor == 0)) {
            possuiFronteira = true;
            fronteira = PEDIDOS_SELECIONADOS()[i].Fronteira;
            break;
        }
    }

    if (possuiFronteira) {
        _carregamentoTransporte.Fronteira.visible(true);
        _carregamentoTransporte.Fronteira.required = _CONFIGURACAO_TMS.FronteiraObrigatoriaMontagemCarga;
        _carregamentoTransporte.Fronteira.text(_carregamentoTransporte.Fronteira.required ? Localization.Resources.Cargas.MontagemCargaMapa.Fronteira.getRequiredFieldDescription() : Localization.Resources.Cargas.MontagemCargaMapa.Fronteira.getFieldDescription());

        if (_carregamentoTransporte.Fronteira.codEntity() == 0 && fronteira.Codigo > 0) {
            _carregamentoTransporte.Fronteira.codEntity(fronteira.Codigo);
            _carregamentoTransporte.Fronteira.val(fronteira.Descricao);
        }
    }
    else {
        _carregamentoTransporte.Fronteira.visible(false);
        _carregamentoTransporte.Fronteira.required = false;
        _carregamentoTransporte.Fronteira.codEntity(0);
        _carregamentoTransporte.Fronteira.val("");
        _carregamentoTransporte.Fronteira.text(Localization.Resources.Cargas.MontagemCargaMapa.Fronteira.getFieldDescription());
    }
}

function VerificarVisibilidadeBuscaSugestaoPedido() {
    if (PEDIDOS_SELECIONADOS().length == 1 && _carregamento.Carregamento.codEntity() == 0 && _carregamento.ModeloVeicularCarga.codEntity() > 0) {
        _carregamentoPedido.BuscarSugestaoPedidos.visible(true);
    }
    else {
        _carregamentoPedido.BuscarSugestaoPedidos.visible(false);
    }

    if (PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _carregamento.PesoCarregamento.visible(true);
    } else {
        _carregamento.PesoCarregamento.visible(false);
    }
}

function BuscarCarregamentoPorPedido(pedido, callback) {
    const dados = { Codigo: pedido.Codigo, CarregamentoRedespacho: _objPesquisaMontagem.GerarCargasDeRedespacho };
    executarReST("MontagemCarga/BuscarPorPedido", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                callback({
                    Carregamento: arg.Data.carregamento,
                    CentroCarregamento: arg.Data.CentroCarregamento,
                });
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                callback({});
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            callback({});
        }
    });
}

function ObterCodigoPedidosSelecionados() {
    return PEDIDOS_SELECIONADOS().map(function (pedido) {
        return pedido.Codigo;
    });
}

function ObterPedidosSelecionados() {
    return PEDIDOS_SELECIONADOS().map(function (pedido) {
        var pesoCarregado = pedido.PesoPedidoCarregamento;

        if (pesoCarregado == undefined)
            pesoCarregado = pedido.Peso;

        return {
            Codigo: pedido.Codigo,
            NumeroReboque: pedido.NumeroReboque,
            DataCarregamento: pedido.DataCarregamento,
            DataDescarregamento: pedido.DataDescarregamento,
            DataPrevisaoEntrega: pedido.DataPrevisaoEntrega,
            PesoPedidoCarregamento: pesoCarregado,
            PesoTotal: pedido.Peso,
            PesoLiquido: pedido.PesoLiquido,
            CodRecebedor: pedido.CodRecebedor,
            TipoCarregamentoPedido: pedido.TipoCarregamentoPedido,
            Ordem: 0,
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
}

function BuscarPedidos(callback, continuar) {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {

        var pedidosSelecionados = ObterCodigoPedidosSelecionados();
        if (_sessaoRoteirizador != null) {
            _pesquisaMontegemCarga.SessaoRoteirizador.val(_sessaoRoteirizador.Codigo.val());
        }

        var data = RetornarObjetoPesquisa(_pesquisaMontegemCarga);

        data.Selecionados = JSON.stringify(pedidosSelecionados);

        if (DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS != null) {
            if (DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Pedidos != null)
                data.Pedidos = JSON.stringify(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Pedidos);

            if (DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.CodigosAgrupadores != null)
                data.CodigosAgrupadores = JSON.stringify(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.CodigosAgrupadores);

            if (DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.PedidosSemSessao != null)
                data.PedidosSemSessao = JSON.stringify(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.PedidosSemSessao);
        }

        if (_sessaoRoteirizador != null) {
            data.SituacaoSessaoRoteirizador = _sessaoRoteirizador.SituacaoSessaoRoteirizador.val();
            data.Continuar = continuar == undefined ? false : continuar;
        }

        _AreaPedido.CarregandoPedidos.val(true);
        _pedidosSemPontos = new Array();

        executarReST("MontagemCargaPedido/ObterPedidos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {

                    var retorno = arg.Data;

                    //Se adicionando pedidos e adicionou algumm... vamos reconsultar a sessão.
                    if (_pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val() == 3 && retorno.QtdeAddSessaoRoteirizador > 0) {

                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.ForamAdicionadosPedidosSessaoDoRoteirizador.format(retorno.QtdeAddSessaoRoteirizador));
                        consultarSessaoRoteirizador(_sessaoRoteirizador.Codigo.val(), false);
                        if (callback) callback();
                    }
                    else {

                        PEDIDOS([]);

                        // Totalizadores.. rodapé;;
                        _AreaPedido.Total.val(retorno.Quantidade);
                        _AreaPedido.TotalPedidos.val(retorno.Quantidade);
                        _AreaPedido.PesoTotal.val(retorno.PesoTotal);
                        _AreaPedido.PesoLiquidoTotal.val(retorno.PesoLiquidoTotal);
                        _AreaPedido.PesoSaldoRestante.val(retorno.PesoSaldoRestante);
                        _AreaPedido.VolumeTotal.val(retorno.VolumeTotal);
                        _AreaPedido.SaldoVolumesRestante.val(retorno.SaldoVolumesRestante);
                        _AreaPedido.ExibirPercentualSeparacaoPedido.val(retorno.ExibirPercentualSeparacaoPedido);
                        _AreaPedido.ValorTotalDosPedidos.val(Globalize.format(retorno.ValorTotalDosPedidos, "n2"));

                        _AreaPedido.TotalEntregas.val(retorno.TotalEntregas);
                        _AreaPedido.TotalCubagem.val(retorno.TotalCubagem);

                        if (_sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val() == EnumTipoRoteirizacaoColetaEntrega.Coleta) {
                            _AreaPedido.TotalEntregas.text(Localization.Resources.Cargas.MontagemCargaMapa.TotalDeColetas.getFieldDescription());
                            _AreaPedido.TotalEntregasSelecionados.text(Localization.Resources.Cargas.MontagemCargaMapa.TotalDeColetasSelecionados.getFieldDescription());
                        } else {
                            _AreaPedido.TotalEntregas.text(Localization.Resources.Cargas.MontagemCargaMapa.TotalDeEntregas.getFieldDescription());
                            _AreaPedido.TotalEntregasSelecionados.text(Localization.Resources.Cargas.MontagemCargaMapa.TotalDeEntregasSelecionados.getFieldDescription());
                        }

                        _pesquisaMontegemCarga.SessaoRoteirizador.val(retorno.SessaoRoteirizador);
                        _pesquisaMontegemCarga.SessaoRoteirizador.codEntity(retorno.SessaoRoteirizador);
                        _pesquisaProtudosNaoAtendido.SessaoRoteirizador.val(retorno.SessaoRoteirizador);
                        _sessaoRoteirizador.Codigo.val(retorno.SessaoRoteirizador);
                        _sessaoRoteirizador.TipoPedidoMontagemCarregamento.val(retorno.TipoPedidoMontagemCarregamento);
                        _sessaoRoteirizador.TipoEdicaoPalletProdutoMontagemCarregamento.val(retorno.TipoEdicaoPalletProdutoMontagemCarregamento);

                        if (_pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val() == 3) { // ADD_PEDIDOS_SESSAO
                            //Quantidade de pedidos adicionados a sessão existente...
                            if (retorno.QtdeAddSessaoRoteirizador > 0) {
                                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.ForamAdicionadosPedidosSessaoDoRoteirizador.format(retorno.QtdeAddSessaoRoteirizador));
                            } else {
                                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.NenhumPedidoAdicionadoSessaoDoRoteirizadorComOsFiltrosInformados);
                            }
                        } else if (_pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val() == 2 && retorno.Registros.length == 0) { // ABRIR SESSAO
                            //Se não retornou nenhum registro.. e o ID da sessão é > 0, finalizou as cargas.. vamos abrir a janela para ele finalizar a sessão.
                            modalPesquisar();
                        }
                        if (retorno.SessaoRoteirizador > 0 && _pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val() == 1 && retorno.Registros.length > 0) {
                            consultarSessaoRoteirizador(retorno.SessaoRoteirizador, true);
                        }

                        if (DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS != null && _pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val() == 1 && retorno.Registros.length < DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Pedidos.length) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.MontagemCargaMapa.RestricoesPedidosAdicionadosGestaoPedidos.format(retorno.Registros.length, DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Pedidos.length));
                        }

                        for (var i = 0; i < retorno.Registros.length; i++) {
                            var pedido = retorno.Registros[i];
                            pedido.Selecionado = ($.inArray(pedido.Codigo, pedidosSelecionados) >= 0);
                            pedido.ExibirOrigem = retorno.ExibirCampoOrdem;
                            pedido.ExibirPesoSaldoRestante = _sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val();

                            PEDIDOS.push(pedido);
                            VerificarPontosFaltantes(pedido);
                        }

                        if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente)
                            AtualizarPontosFaltantes();

                        fimListagemPedidos = 0;

                        if (_AreaPedido.TabelaPedidosVisivel.visible())
                            carregarTabelaPedidosSessao();

                        if (callback) callback();
                    }

                } else {
                    if (arg.Msg.indexOf(Localization.Resources.Cargas.MontagemCargaMapa.LimiteDe + ' ') >= 0) {
                        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, arg.Msg, function () {
                            BuscarPedidos(callback, true);
                        });
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
            _AreaPedido.CarregandoPedidos.val(false);
        });
    } else if (callback) {
        callback();
    }
}

function ObterDetalhesPedido(codigo) {
    carregarPedidoProdutosCarregamentos(codigo,
        carregarPedidoProduto(codigo, function () {
            executarReST("MontagemCargaPedido/BuscarPorCodigoPedido", { Codigo: codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        PreencherObjetoKnout(_detalhePedido, { Data: retorno.Data.DetalhesPedido });
                        PreencherObjetoKnout(_detalhePedidoExportacao, { Data: retorno.Data.DetalhesPedidoExportacao });

                        if (_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga && Boolean(retorno.Data.DetalhesPedidoTransporteMaritimo)) {
                            PreencherObjetoKnout(_detalhePedidoTransporteMaritimo, { Data: retorno.Data.DetalhesPedidoTransporteMaritimo });
                            $("#liDetalhePedidoTransporteMaritimo").show();
                        }
                        else
                            $("#liDetalhePedidoTransporteMaritimo").hide();

                        Global.abrirModal('modalDetalhePedido');

                        var subscribes = [];

                        if (_CONFIGURACAO_TMS.MontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa) {
                            subscribes.push(_detalhePedido.TipoPaleteCliente.val.subscribe(tipoPaleteClienteChange));
                            subscribes.push(_detalhePedido.NumeroPaletesFracionado.val.subscribe(tipoPaleteClienteChange));
                        }

                        $("#modalDetalhePedido").one('hidden.bs.modal', function () {

                            if (subscribes.length > 0)
                                subscribes.forEach(sub => sub.dispose());

                            LimparCampos(_detalhePedido);
                            LimparCampos(_detalhePedidoExportacao);
                            LimparCampos(_detalhePedidoTransporteMaritimo);

                            $("a[href='#knoutDetalhePedido']").click();
                        });

                        carregarDetalhesPedidoProdutos(codigo, null);
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        })
    );
}

function ObterPedidoProdutosCarregamento(codigo) {
    var data = { Codigo: codigo };
    carregarPedidoProduto(codigo, null);
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

    opcoesPedidosSelecionados();

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

function CodigoFiliaisDistintasPedidos() {
    var filiais = [];
    var pedidos = PEDIDOS();
    for (var i = 0; i < pedidos.length; i++) {
        if ($.inArray(pedidos[i].CodigoFilial, filiais) < 0) {
            filiais.push(pedidos[i].CodigoFilial);
        }
    }
    return filiais;
}

function BiparNotas() {
    _AreaPedido.ChaveNotaFiscalEletronicaAreaPedido.val.subscribe(function (chaveNFe) {
        if (!chaveNFe || chaveNFe.trim() === "" || chaveNFe.length < 44) return;

        if (!ValidarChaveAcesso(chaveNFe))
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.MontagemCargaMapa.ChaveNotaFiscalEletronicaInvalida);

        BuscarPedidoPorChaveNotaFiscalEletronicaAreaPedido(chaveNFe);
    });
}

function BuscarPedidoPorChaveNotaFiscalEletronicaAreaPedido(chaveNFe) {
    executarReST("MontagemCarga/ObterPedidoAtravesChaveNFe", { ChaveNotaFiscalEletronica: chaveNFe }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                let result = arg.Data;
                if (result !== null) {
                    _pesquisaMontegemCarga.Pedido.codEntity(result.CodigoPedido);
                    _pesquisaMontegemCarga.Pedido.val(result.CodigoPedido);
                    _pesquisaMontegemCarga.Filial.codEntity(result.CodigoFilial);
                    _pesquisaMontegemCarga.Filial.val(result.DescricaoFilial);
                    _pesquisaMontegemCarga.RequisicaoEnviadaMontagemCargaMapa.val(true);
                    _pesquisaMontegemCarga.ChaveNotaFiscalEletronica.val(chaveNFe);
                    LimparCampo(_AreaPedido.ChaveNotaFiscalEletronicaAreaPedido);
                    BuscarDadosMontagemCarga(3, function () {
                        LimparCampo(_pesquisaMontegemCarga.ChaveNotaFiscalEletronica);
                        BuscarDadosMontagemCarga(2);//Recarrega a Grid dos pedidos sem a nota;
                    });//Adc. Pedido  
                    _pesquisaMontegemCarga.RequisicaoEnviadaMontagemCargaMapa.val(false);
                }
            }
            else { exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg); }
        }
        else { exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg); }
    });
}

function buscarTiposPaletesDetalhes() {
    executarReST("GestaoPedido/BuscarTiposPaletesDetalhes", null, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                tiposPaletesClienteDetalhes = retorno.Data;
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function tipoPaleteClienteChange(tipoPaleteClienteSelecionado) {
    _detalhePedido.PesoTotalPaletes.val(Globalize.format(0, "n2"));

    if (tipoPaleteClienteSelecionado === EnumTipoPalletCliente.NaoDefinido || _detalhePedido.NumeroPaletesFracionado.val() <= 0) return;

    calcularPesoTotalPaletes();
}

function calcularPesoTotalPaletes() {
    var valor = tiposPaletesClienteDetalhes.find(tipo => tipo.Codigo === _detalhePedido.TipoPaleteCliente.val()).Valor;

    if (valor == 0) return;

    var numeroPaletesFracionado = Globalize.parseFloat(_detalhePedido.NumeroPaletesFracionado.val());

    _detalhePedido.PesoTotalPaletes.val(Globalize.format(numeroPaletesFracionado * valor, "n2"));
}