/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SessaoRoteirizador.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />
/// <reference path="../../Consultas/FiltroPesquisa.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="Locais.js" />
/// <reference path="../../Enumeradores/EnumTipoMontagemCarregamentoVrp.js" />
/// <reference path="../../Enumeradores/EnumTipoMontagemCarregamentoPedidoProduto.js" />

var _sessaoRoteirizador;
var _sessaoRoteirizadorParametros;

var _sessaoRoteirizadorPedidoInconsistente;
var _gridPedidosInconsistentes;

var _gridDestinatarios;
var _gridTiposDeCarga;
var _gridDisponibilidadeFrotaUtilizarSessaoRoteirizador;

var _pedidosInconsistentesMotivos;
var _gridDisponibilidadesFrota;
var _gridTemposCarregamento;
var _preFiltroParametrosSessaoRoteirizador;

var PEDIDOS_INCONSISTENTES = ko.observableArray([]);

var PreFiltroParametrosSessaoRoteirizador = function () {

    this.CodigoFiltro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NomeFiltro = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), val: ko.observable(""), def: "", visible: true, required: true });

    this.Cancelar = PropertyEntity({ eventClick: limparPreFiltroParametrosSessaoRoteirizadorClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Novo), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirPreFiltroParametrosSessaoRoteirizadorClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false) });
    this.Alterar = PropertyEntity({ eventClick: salvarParametrosSessaoPreFiltroClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.SalvarFiltros) });

    this.CodigoFiltro.val.subscribe(function (novoValor) {
        var text = Localization.Resources.Gerais.Geral.SalvarFiltros;
        var visible = false;
        if (novoValor > 0) {
            text = Localization.Resources.Gerais.Geral.AtualizarFiltros;
            visible = true;
        }
        _sessaoRoteirizadorParametros.SalvarPreFiltros.text(text);
        _preFiltroParametrosSessaoRoteirizador.Alterar.text(text);
        _preFiltroParametrosSessaoRoteirizador.Cancelar.visible(visible);
        _preFiltroParametrosSessaoRoteirizador.Excluir.visible(visible);
    });
}

var SessaoRoterizador = function () {
    var dataInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Sessao.getFieldDescription() });
    this.Filial = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCargaMapa.Filial.getFieldDescription() });
    this.Expedidor = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoExpedidor = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCargaMapa.Expedidor.getFieldDescription() });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(dataInicial) });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(dataInicial) });
    this.Destinatarios = PropertyEntity({ val: ko.observable(new Array()) });
    this.TiposDeCarga = PropertyEntity({ val: ko.observable(new Array()) });
    this.MontagemCarregamentoPedidoProduto = PropertyEntity({ val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false });
    this.TipoRoteirizacaoColetaEntrega = PropertyEntity({ val: ko.observable(EnumTipoRoteirizacaoColetaEntrega.Entrega), options: EnumTipoRoteirizacaoColetaEntrega.obterOpcoes(), def: EnumTipoRoteirizacaoColetaEntrega.Entrega });
    this.TipoMontagemCarregamentoPedidoProduto = PropertyEntity({ val: ko.observable(EnumTipoMontagemCarregamentoPedidoProduto.Ambos), options: EnumTipoMontagemCarregamentoPedidoProduto.obterOpcoes(), def: EnumTipoMontagemCarregamentoPedidoProduto.Ambos });
    this.TipoMontagemCarregamentoVRP = PropertyEntity({ val: ko.observable(0), options: EnumTipoMontagemCarregamentoVrp.obterOpcoes(), def: EnumTipoMontagemCarregamentoVrp.Nenhum });
    this.SituacaoSessaoRoteirizador = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Parametros = PropertyEntity({});
    this.InfoSessao = PropertyEntity({ val: ko.observable(0), def: 0, eventClick: infoClick });
    this.GridDestinatarios = PropertyEntity({ type: types.local });
    this.GridTiposDeCarga = PropertyEntity({ type: types.local });
    this.GridDisponibilidadeFrotaUtilizarSessaoRoteirizador = PropertyEntity({ type: types.local });

    this.SimuladorFreteCriterioSelecaoTransportador = PropertyEntity({ val: ko.observable(EnumSimuladorFreteCriterioSelecaoTransportador.Nenhum), options: EnumSimuladorFreteCriterioSelecaoTransportador.obterOpcoes() });
    this.TipoPedidoMontagemCarregamento = PropertyEntity({ val: ko.observable(EnumTipoPedidoMontagemCarregamento.Card), options: EnumTipoPedidoMontagemCarregamento.obterOpcoes() });
    this.TipoEdicaoPalletProdutoMontagemCarregamento = PropertyEntity({ val: ko.observable(EnumTipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado), options: EnumTipoEdicaoPalletProdutoMontagemCarregamento.obterOpcoes() });

    this.ConsiderarPesoPalletPesoTotalCarga = PropertyEntity({ val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false });

    this.TipoPedidoMontagemCarregamento.val.subscribe(function (novoValor) {
        if (_AreaPedido)
            _AreaPedido.TabelaPedidosVisivel.visible(novoValor == 1);
        controleApresentacaoPedidos();
    });

    this.PreencherAutomaticamenteDadosCentroTelaMontagemCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.EmpresaPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: false });
    this.TipoOperacaoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: false });
    this.VeiculoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, idBtnSearch: guid(), issue: 143, enable: ko.observable(true), visible: ko.observable(true) });
    this.VeiculoModeloVeicularPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, idBtnSearch: guid(), issue: 143, enable: ko.observable(true), visible: ko.observable(true) });
    this.MotoristaPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotoristaPadraoCPF = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
};

var SessaoRoterizadorParametros = function () {

    this.TipoMontagemCarregamentoVRP = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.MontagemCarregamentoOpcao.getFieldDescription(), val: ko.observable(EnumTipoMontagemCarregamentoVrp.Nenhum), options: EnumTipoMontagemCarregamentoVrp.obterOpcoes(), visible: ko.observable(true) });
    this.TipoOcupacaoMontagemCarregamentoVRP = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoMontagemCarregamento.getFieldDescription(), val: ko.observable(EnumTipoOcupacaoMontagemCarregamentoVrp.Peso), options: EnumTipoOcupacaoMontagemCarregamentoVrp.obterOpcoes(), visible: ko.observable(false) });
    this.ConsiderarTempoDeslocamentoCD = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ConsiderarTempoDeslocamentoCD.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NivelQuebraProdutoRoteirizar = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NivelQuebraProdutoRoteirizar.getFieldDescription(), val: ko.observable(EnumNivelQuebraProdutoRoteirizar.Item), options: EnumNivelQuebraProdutoRoteirizar.obterOpcoes(), visible: ko.observable(true) });
    this.GerarCarregamentoDoisDias = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.GerarCarregamentosParaDoisDias.getFieldDescription(), visible: ko.observable(false), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false });
    this.GerarCarregamentosAlemDaDispFrota = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.GerarCarregamentosAlemDaDisponibilidadeDeFrota.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false, visible: ko.observable(true) });
    this.UtilizarDispFrotaCentroDescCliente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.UtilizarDisponibilidadeFrotaCentroDescargaCliente.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false, visible: ko.observable(true) });
    this.QuantidadeMaximaEntregasRoteirizar = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "", maxlength: 2 }, text: Localization.Resources.Cargas.MontagemCargaMapa.MaximoEntregasMontagemCarregamento.getFieldDescription(), visible: ko.observable(true) });
    this.MontagemCarregamentoPedidoProduto = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.MontagemCarregamentoPorPedidoProduto.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false, visible: ko.observable(true) });
    this.CarregamentoTempoMaximoRota = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Cargas.MontagemCargaMapa.TempoMaximoEntreAsEntregasDeUmCarregamentoMinutos.getFieldDescription(), visible: ko.observable(false), maxlength: 5, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.AgruparPedidosMesmoDestinatario = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.AgruparPedidosMesmoDestinatario.getFieldDescription(), visible: ko.observable(false), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false });
    this.OcultarDetalhesDoPontoNoMapa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Cargas.MontagemCargaMapa.OcultarDetalhesNoPontoMapa });

    //this.DisponibilidadesFrota = PropertyEntity({});
    this.DisponibilidadesFrota = PropertyEntity({ type: types.local });
    this.DisponibilidadesFrotaVisible = PropertyEntity({ visible: ko.observable(true) });

    this.TemposCarregamento = PropertyEntity({ type: types.local });
    this.TemposCarregamentoVisible = PropertyEntity({ visible: ko.observable(true) });

    this.Reset = PropertyEntity({
        eventClick: function (e) {
            resetParametrosSessao();
        }, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.RestaurarConfiguracoes), idGrid: guid(), visible: ko.observable(true)
    });

    this.Salvar = PropertyEntity({
        eventClick: function (e) {
            salvarParametrosSessao();
        }, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.SalvarParametros), idGrid: guid(), visible: ko.observable(true)
    });


    this.PreFiltros = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.PreFiltros, idBtnSearch: guid(), visible: ko.observable(true) });

    this.SalvarPreFiltros = PropertyEntity({
        eventClick: function (e) {
            SalvarPreFiltroParametrosSessaoRoteirizador();
        }, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.SalvarFiltros), idGrid: guid(), visible: ko.observable(true)
    });

    this.TipoMontagemCarregamentoVRP.val.subscribe(tipoMontagemCarregamentoVrpChange);
    this.MontagemCarregamentoPedidoProduto.val.subscribe(montagemCarregamentoPedidoProdutoChange);
    this.UtilizarDispFrotaCentroDescCliente.val.subscribe(utilizaDisponibilidadeFrotaCentroDescargaClienteChange);
    this.OcultarDetalhesDoPontoNoMapa.val.subscribe(ocultarDetalhesDoPontoMapaChange);
};

function tipoMontagemCarregamentoVrpChange() {

    var vrp = (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.Nenhum &&
        _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.SimuladorFrete &&
        _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.Prioridades);

    if (vrp) {
        _sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.val(false);
        _sessaoRoteirizadorParametros.UtilizarDispFrotaCentroDescCliente.val(false);
    }

    _sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.visible(_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Nenhum);
    _sessaoRoteirizadorParametros.NivelQuebraProdutoRoteirizar.visible(_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Nenhum);

    _sessaoRoteirizadorParametros.CarregamentoTempoMaximoRota.visible(_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpTimeWindows);
    _sessaoRoteirizadorParametros.ConsiderarTempoDeslocamentoCD.visible(_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpTimeWindows);
    _sessaoRoteirizadorParametros.GerarCarregamentoDoisDias.visible(_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpCapacity);

    _sessaoRoteirizadorParametros.AgruparPedidosMesmoDestinatario.visible(vrp);
    _sessaoRoteirizadorParametros.GerarCarregamentoDoisDias.visible(vrp);
    _sessaoRoteirizadorParametros.TipoOcupacaoMontagemCarregamentoVRP.visible(vrp || (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Prioridades));
    _sessaoRoteirizadorParametros.TemposCarregamentoVisible.visible((vrp || (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Prioridades)) && !_sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.val());
    _sessaoRoteirizadorParametros.QuantidadeMaximaEntregasRoteirizar.visible(vrp || (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Prioridades));

    var prioridades = (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Prioridades);
    _sessaoRoteirizadorParametros.GerarCarregamentosAlemDaDispFrota.visible(!prioridades);
    _sessaoRoteirizadorParametros.UtilizarDispFrotaCentroDescCliente.visible(!prioridades);
}

function montagemCarregamentoPedidoProdutoChange() {
    _sessaoRoteirizadorParametros.NivelQuebraProdutoRoteirizar.visible(_sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.val());
}

function utilizaDisponibilidadeFrotaCentroDescargaClienteChange() {
    _sessaoRoteirizadorParametros.DisponibilidadesFrotaVisible.visible(!_sessaoRoteirizadorParametros.UtilizarDispFrotaCentroDescCliente.val());
}

function ocultarDetalhesDoPontoMapaChange(valor) {
    if (valor) {
        for (objMarker of _arrayMarker) {
            google.maps.event.clearListeners(objMarker.marker, 'mouseover');
        }

        if (_polylineCargas) {
            var codigosCarregamentoMarker = _arrayMarker.filter(function (elemento) { return parseInt(elemento.codigo_carregamento) > 0; }).map(function (elemento) {
                return elemento.codigo_carregamento;
            });

            for (codigoCarregamentoMarker of codigosCarregamentoMarker)
                google.maps.event.clearListeners(_polylineCargas[codigoCarregamentoMarker], 'mouseover');
        }
    }
    else {
        for (objMarker of _arrayMarker) {
            var latLng = { lat: objMarker.pedidos[0].EnderecoDestino.Latitude, lng: objMarker.pedidos[0].EnderecoDestino.Longitude };

            attachEventMarker(objMarker, latLng);

            var codigosCarregamentoMarker = _arrayMarker.filter(function (elemento) { return parseInt(elemento.codigo_carregamento) > 0; }).map(function (elemento) {
                return elemento.codigo_carregamento;
            });

            for (codigoCarregamentoMarker of codigosCarregamentoMarker)
                attachMouseOverEventPolyLine(_polylineCargas[codigoCarregamentoMarker]);
        }
    }
}

function infoClick() {
    $("#divDropDownInfoSessaoRoteirizador").css('display', 'block');
    _gridDestinatarios.CarregarGrid(_sessaoRoteirizador.Destinatarios.val());
    _gridTiposDeCarga.CarregarGrid(_sessaoRoteirizador.TiposDeCarga.val());
    var dados = _gridDisponibilidadesFrota.BuscarRegistros().filter(function (item) { return item.QuantidadeUtilizar > 0 });
    _gridDisponibilidadeFrotaUtilizarSessaoRoteirizador.CarregarGrid(dados);
}

function modalParametrosSessao() {
    if (_sessaoRoteirizador.Codigo.val() > 0) {
        //if (!_sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.val()) {
        Global.abrirModal('divModalParametrosSessao');
        //}
    }
}

var SessaoRoteirizadorPedidoInconsistente = function () {
    this.Grid = PropertyEntity({ type: types.local });
}

function loadGridDisponibilidadeFrotaSessao() {

    var editable = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        numberMask: ConfigInt()
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.ModeloVeicular, width: "30%" },
        { data: "CodigoTransportador", visible: false },
        { data: "DescricaoTransportador", title: Localization.Resources.Cargas.MontagemCargaMapa.Transportador, width: "30%" },
        { data: "Quantidade", title: Localization.Resources.Cargas.MontagemCargaMapa.Disponibilidade, width: "20%" },
        { data: "QuantidadeUtilizar", title: Localization.Resources.Cargas.MontagemCargaMapa.Utilizar, width: "20%", editableCell: editable }
    ];

    var editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarDisponibilidadesFrotaSessao
    };

    var configRowsSelect = { permiteSelecao: false, marcarTodos: false, permiteSelecionarTodos: false };

    _gridDisponibilidadesFrota = new BasicDataTable(_sessaoRoteirizadorParametros.DisponibilidadesFrota.id, header, null, null, configRowsSelect, null, null, null, editarColuna);
}

function loadGridTemposCarregamentoSessao() {

    var editable = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        numberMask: ConfigInt()
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoCarga", visible: false },
        { data: "DescricaoTipoCarga", title: Localization.Resources.Cargas.MontagemCargaMapa.TipoDaCarga, width: "30%" },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.ModeloVeicular, width: "30%" },
        { data: "Quantidade", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeMaximoEntregas, width: "20%" },
        { data: "QuantidadeUtilizar", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeMaximoEntregasUtilizar, width: "20%", editableCell: editable },
        { data: "QuantidadeMinima", title: "Quantidade Minima", width: "20%" },
        { data: "QuantidadeMinimaUtilizar", title: "Quantidade Min. Utilizar", width: "20%", editableCell: editable }
    ];

    var editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarDisponibilidadesFrotaSessao
    };

    var configRowsSelect = { permiteSelecao: false, marcarTodos: false, permiteSelecionarTodos: false };

    _gridTemposCarregamento = new BasicDataTable(_sessaoRoteirizadorParametros.TemposCarregamento.id, header, null, null, configRowsSelect, null, null, null, editarColuna);
}

function AtualizarDisponibilidadesFrotaSessao(dataRow, row, head) {

}

function callbackRowDisponibilidadesFrotaSessao(nRow, aData) {
    //$(_carregamentoProdutos.Grid.id).addClass("tableCursorMove");
    //$(nRow).draggable(obterObjetoDragglable("remocao-item-produto-carregamento"));
}

function loadGridSessaoRoteirizadorPedidoInconsistente() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "DataPrevisaoEntrega", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "NumeroReboque", visible: false },
        { data: "TipoCarregamentoPedido", visible: false },
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Cargas.MontagemCargaMapa.Numero, width: "10%" },
        { data: "Destinatario", title: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario, width: "20%" },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCargaMapa.Destino, width: "15%", visible: false },
        { data: "Remetente", title: Localization.Resources.Cargas.MontagemCargaMapa.Remetente, width: "20%", visible: false },
        { data: "Origem", title: Localization.Resources.Cargas.MontagemCargaMapa.Origem, width: "15%", visible: false },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCargaMapa.Peso, width: "10%" },
        { data: "Cubagem", title: Localization.Resources.Cargas.MontagemCargaMapa.Cubagem, width: "10%" },
        { data: "DataPrevisaoEntrega", title: Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisao, width: "10%" },
        { data: "CanalEntrega", title: Localization.Resources.Cargas.MontagemCargaMapa.Canal, width: "10%" },
        { data: "Pallet", title: Localization.Resources.Cargas.MontagemCargaMapa.Pallet, width: "10%" },
        { data: "MotivoInconsistencia", title: Localization.Resources.Cargas.MontagemCargaMapa.Motivo, width: "25%" }
    ];

    _gridPedidosInconsistentes = new BasicDataTable(_sessaoRoteirizadorPedidoInconsistente.Grid.id, header, null, { column: 2, dir: "asc" }, null, 25);
    RenderizarGridPedidosInconsistentes();
}

function validaInformacoesVisiveisSessaoRoteirizador() {

    _AreaCarregamento.OtimizarTodos.visible(!_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val());

    // Se for montagem por pedido produto ou diferente de Simulação de frete.
    if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val() || _sessaoRoteirizador.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.SimuladorFrete) {
        $("#liListaResumoCarregamentos").css('display', 'block');
        if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val()) {
            //$("#divOpcoesParametrosSessao").css('display', 'none');
            _resumoCarregamentos.Grid.visible(true);
            _resumoCarregamentos.GridFrota.visible(false);
            _resumoCarregamentos.GridResumo.visible(false);
        } else {
            _resumoCarregamentos.Grid.visible(false);
            _resumoCarregamentos.GridFrota.visible(true);
            _resumoCarregamentos.GridResumo.visible(true);
        }
    } else {
        $("#liListaResumoCarregamentos").css('display', 'none');
        //$("#divOpcoesParametrosSessao").css('display', 'block');
    }

    simuladorFreteHabilitaBotoesSomente();

    if (_sessaoRoteirizador.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.SimuladorFrete) {
        $("#liAreaSimulacaoFretesBlocos").css('display', 'block');
        $("#divOpcoesParametrosSessao").css('display', 'none');
    } else {
        $("#divOpcoesParametrosSessao").css('display', 'block');
        if (_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val() == true || _sessaoRoteirizador.SimuladorFreteCriterioSelecaoTransportador.val() == EnumSimuladorFreteCriterioSelecaoTransportador.Nenhum) {
            $("#liAreaSimulacaoFretesBlocos").css('display', 'none');
        } else {
            $("#liAreaSimulacaoFretesBlocos").css('display', 'block');
        }
    }

    opcoesConfiguracaoParametrosSessaoRoteirizador();

    if (_sessaoRoteirizador.Codigo.val() > 0) {
        $("#knoutInfoSessaoRoteirizador").css('display', 'block');
    } else {
        $("#knoutInfoSessaoRoteirizador").css('display', 'none');
    }
}

function opcoesConfiguracaoParametrosSessaoRoteirizador() {
    var vrp = (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.Nenhum &&
        _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.SimuladorFrete &&
        _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() != EnumTipoMontagemCarregamentoVrp.Prioridades);

    _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.visible(!_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val());
    _sessaoRoteirizadorParametros.ConsiderarTempoDeslocamentoCD.visible(!_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val() && _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.VrpTimeWindows);
    _sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.visible(!_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val() && _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Nenhum);
    _sessaoRoteirizadorParametros.TemposCarregamentoVisible.visible((vrp || (_sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val() == EnumTipoMontagemCarregamentoVrp.Prioridades)) && !_sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.val());
}

function RenderizarGridPedidosInconsistentes() {

    _gridPedidosInconsistentes.CarregarGrid(PEDIDOS_INCONSISTENTES());

    if (_sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val() == EnumTipoRoteirizacaoColetaEntrega.Coleta) {
        //_gridPedidosInconsistentes.ControlarExibicaoColuna('Destino', false);
        _gridPedidosInconsistentes.ControlarExibicaoColuna('Destinatario', false);
        //_gridPedidosInconsistentes.ControlarExibicaoColuna('Origem', true);
        _gridPedidosInconsistentes.ControlarExibicaoColuna('Remetente', true);
    } else {
        //_gridPedidosInconsistentes.ControlarExibicaoColuna('Destino', true);
        _gridPedidosInconsistentes.ControlarExibicaoColuna('Destinatario', true);
        //_gridPedidosInconsistentes.ControlarExibicaoColuna('Origem', false);
        _gridPedidosInconsistentes.ControlarExibicaoColuna('Remetente', false);
    }

    if (PEDIDOS_INCONSISTENTES().length > 0) {
        $("#liAreaPedidosInconsistentes").css('display', 'block');
    } else {
        $("#liAreaPedidosInconsistentes").css('display', 'none');
    }
}

function loadSessaoRoteirizador() {
    _sessaoRoteirizador = new SessaoRoterizador()
    KoBindings(_sessaoRoteirizador, "knoutInfoSessaoRoteirizador");

    _sessaoRoteirizadorParametros = new SessaoRoterizadorParametros();
    KoBindings(_sessaoRoteirizadorParametros, "knoutParametrosSessao");

    new BuscarFiltroPesquisa(_sessaoRoteirizadorParametros.PreFiltros, abrirPreFiltroParametrosSessaoRoteirizador, 2, null);

    _preFiltroParametrosSessaoRoteirizador = new PreFiltroParametrosSessaoRoteirizador();
    KoBindings(_preFiltroParametrosSessaoRoteirizador, "knockoutSalvarPreFiltroParametrosSessaoRoteirizador");


    var header = [
        { data: "CodigoIntegracao", title: Localization.Resources.Cargas.MontagemCargaMapa.Codigo, width: "25%" },
        { data: "CPF_CNPJ", title: Localization.Resources.Cargas.MontagemCargaMapa.CPFCNPJ, width: "25%" },
        { data: "Nome", title: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario, width: "50%" }
    ];

    _gridDestinatarios = new BasicDataTable(_sessaoRoteirizador.GridDestinatarios.id, header, null, { column: 2, dir: "asc" }, null, 25);

    header = [
        { data: "Codigo", title: Localization.Resources.Cargas.MontagemCargaMapa.Codigo, width: "25%" },
        { data: "Descricao", title: Localization.Resources.Cargas.MontagemCargaMapa.CPFCNPJ, width: "75%" }
    ];

    _gridTiposDeCarga = new BasicDataTable(_sessaoRoteirizador.GridTiposDeCarga.id, header, null, { column: 1, dir: "asc" }, null, 25);

    header = [
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Cargas.MontagemCargaMapa.ModeloVeicular, width: "50%" },
        { data: "Quantidade", title: Localization.Resources.Cargas.MontagemCargaMapa.Disponibilidade, width: "25%" },
        { data: "QuantidadeUtilizar", title: Localization.Resources.Cargas.MontagemCargaMapa.Utilizar, width: "25%" }
    ];
    _gridDisponibilidadeFrotaUtilizarSessaoRoteirizador = new BasicDataTable(_sessaoRoteirizador.GridDisponibilidadeFrotaUtilizarSessaoRoteirizador.id, header, null, { column: 1, dir: "asc" }, null, 25);

    HeaderAuditoria("SessaoRoteirizador", _sessaoRoteirizador);

    _sessaoRoteirizadorPedidoInconsistente = new SessaoRoteirizadorPedidoInconsistente();
    KoBindings(_sessaoRoteirizadorPedidoInconsistente, "knoutPedidosInconsistente");

    loadGridSessaoRoteirizadorPedidoInconsistente();

    loadGridDisponibilidadeFrotaSessao();

    loadGridTemposCarregamentoSessao();
}

function sessaoRoteirizadorFinalizada() {
    if (_sessaoRoteirizador == null)
        return false;

    if (_sessaoRoteirizador.SituacaoSessaoRoteirizador.val() == 2)
        return true;

    return false;
}

function preencherDadosPadrao(sessao) {

    _sessaoRoteirizador.PreencherAutomaticamenteDadosCentroTelaMontagemCarga.val(sessao.PreencherAutomaticamenteDadosCentroTelaMontagemCarga);

    if (_sessaoRoteirizador.PreencherAutomaticamenteDadosCentroTelaMontagemCarga.val()) {

        if (sessao.EmpresaPadrao != null &&
            sessao.EmpresaPadrao.Codigo > 0 &&
            sessao.EmpresaPadrao.Descricao != "") {

            _sessaoRoteirizador.EmpresaPadrao.codEntity(sessao.EmpresaPadrao.Codigo);
            _sessaoRoteirizador.EmpresaPadrao.val(sessao.EmpresaPadrao.Descricao);
        }

        if (sessao.VeiculoPadrao != null &&
            sessao.VeiculoPadrao.Codigo > 0 &&
            sessao.VeiculoPadrao.Descricao != "") {

            _sessaoRoteirizador.VeiculoPadrao.codEntity(sessao.VeiculoPadrao.Codigo);
            _sessaoRoteirizador.VeiculoPadrao.val(sessao.VeiculoPadrao.Descricao);
        }

        if (sessao.ModeloVeicularPadrao != null &&
            sessao.ModeloVeicularPadrao.Codigo > 0 &&
            sessao.ModeloVeicularPadrao.Descricao != "") {
            _sessaoRoteirizador.VeiculoModeloVeicularPadrao.codEntity(sessao.ModeloVeicularPadrao.Codigo);
            _sessaoRoteirizador.VeiculoModeloVeicularPadrao.val(sessao.ModeloVeicularPadrao.Descricao);
        }

        if (sessao.TipoOperacaoPadrao != null &&
            sessao.TipoOperacaoPadrao.Codigo > 0 &&
            sessao.TipoOperacaoPadrao.Descricao != "") {

            _sessaoRoteirizador.TipoOperacaoPadrao.codEntity(sessao.TipoOperacaoPadrao.Codigo);
            _sessaoRoteirizador.TipoOperacaoPadrao.val(sessao.TipoOperacaoPadrao.Descricao);
        }

        if (sessao.MotoristaPadrao != null &&
            sessao.MotoristaPadrao.Codigo > 0 &&
            sessao.MotoristaPadrao.Descricao != "" &&
            sessao.MotoristaPadrao.CPF != "") {

            _sessaoRoteirizador.MotoristaPadrao.codEntity(sessao.MotoristaPadrao.Codigo);
            _sessaoRoteirizador.MotoristaPadrao.val(sessao.MotoristaPadrao.Descricao);
            _sessaoRoteirizador.MotoristaPadraoCPF.val(sessao.MotoristaPadrao.CPF);
        }

        obterDadosPadrao();
    }
}

function obterDadosPadrao() {
    
    if (_sessaoRoteirizador.PreencherAutomaticamenteDadosCentroTelaMontagemCarga.val()) {

        if (_carregamentoTransporte.Empresa.codEntity() == 0) {
            _carregamentoTransporte.Empresa.codEntity(_sessaoRoteirizador.EmpresaPadrao.codEntity());
            _carregamentoTransporte.Empresa.val(_sessaoRoteirizador.EmpresaPadrao.val());
        }

        if (_carregamentoTransporte.Veiculo.codEntity() == 0) {
            _carregamentoTransporte.Veiculo.codEntity(_sessaoRoteirizador.VeiculoPadrao.codEntity());
            _carregamentoTransporte.Veiculo.val(_sessaoRoteirizador.VeiculoPadrao.val());
        }

        if (_carregamento.ModeloVeicularCarga.codEntity() == 0) {
            _carregamento.ModeloVeicularCarga.codEntity(_sessaoRoteirizador.VeiculoModeloVeicularPadrao.codEntity());
            _carregamento.ModeloVeicularCarga.val(_sessaoRoteirizador.VeiculoModeloVeicularPadrao.val());
        }

        if (_carregamentoTransporte.TipoOperacao.codEntity() == 0) {
            _carregamentoTransporte.TipoOperacao.codEntity(_sessaoRoteirizador.TipoOperacaoPadrao.codEntity());
            _carregamentoTransporte.TipoOperacao.val(_sessaoRoteirizador.TipoOperacaoPadrao.val());
        }

        if (_carregamentoTransporte.Motoristas.basicTable.BuscarRegistros().length == 0) {

            var codigoMotorista = _sessaoRoteirizador.MotoristaPadrao.codEntity();
            var nomeMotorista = _sessaoRoteirizador.MotoristaPadrao.val();
            var cpfMotorista = _sessaoRoteirizador.MotoristaPadraoCPF.val();

            var dataGrid = _gridMotoristas.BuscarRegistros();

            var obj = new Object();
            obj.Codigo = codigoMotorista;
            obj.CPF = cpfMotorista;
            obj.Nome = nomeMotorista;

            dataGrid.push(obj);

            _gridMotoristas.CarregarGrid(dataGrid);
        }
    }
}

function consultarSessaoRoteirizador(codigo, somenteSessao) {
    var data = { Codigo: codigo };
    //Pegar o ID da sessão selecionada...
    executarReST("SessaoRoteirizador/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {

            Global.fecharModal('divModalOpcoesSessao');
            var sessao = arg.Data;
            PreencherObjetoKnout(_sessaoRoteirizador, { Data: sessao });

            _pesquisaMontegemCarga.NumeroCarregamentoPedido.val(""); //precisa ser limpado senao busca só os pedidos com esse numero carregamento.
            _pesquisaMontegemCarga.SessaoRoteirizador.val(data.Codigo);
            _pesquisaMontegemCarga.SessaoRoteirizador.codEntity(data.Codigo);
            _pesquisaProtudosNaoAtendido.SessaoRoteirizador.val(data.Codigo);

            //Atualizado o SessaoRoteirizador
            _sessaoRoteirizador.Codigo.val(sessao.Codigo);
            _sessaoRoteirizador.Filial.val(sessao.Filial);
            _sessaoRoteirizador.Expedidor.val(sessao.Expedidor.Codigo);
            _sessaoRoteirizador.DataInicial.val(sessao.DataInicial);
            _sessaoRoteirizador.DataFinal.val(sessao.DataFinal);
            _sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val(sessao.MontagemCarregamentoPedidoProduto);
            _sessaoRoteirizador.ConsiderarPesoPalletPesoTotalCarga.val(sessao.ConsiderarPesoPalletPesoTotalCarga);
            _sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val(sessao.TipoRoteirizacaoColetaEntrega);
            _sessaoRoteirizador.TipoMontagemCarregamentoPedidoProduto.val(sessao.TipoMontagemCarregamentoPedidoProduto);
            _sessaoRoteirizador.SituacaoSessaoRoteirizador.val(sessao.SituacaoSessaoRoteirizador);
            _sessaoRoteirizador.TipoPedidoMontagemCarregamento.val(sessao.TipoPedidoMontagemCarregamento);

            preencherDadosPadrao(sessao);

            validaInformacoesVisiveisSessaoRoteirizador();

            _pesquisaMontegemCarga.Filial.codEntity(sessao.Filial);
            _pesquisaMontegemCarga.Filial.val(sessao.Descricao);
            _pesquisaMontegemCarga.DataInicio.val(sessao.DataInicial);
            _pesquisaMontegemCarga.DataFim.val(sessao.DataFinal);
            _pesquisaMontegemCarga.Expedidor.codEntity(sessao.Expedidor.Codigo);
            _pesquisaMontegemCarga.Expedidor.val(sessao.Expedidor.Descricao);
            _pesquisaMontegemCarga.PedidosOrigemRecebedor.val(sessao.RoteirizacaoRedespacho);

            _carregamento.DataCarregamento.required(sessao.DataCarregamentoObrigatoriaMontagemCarga);
            _carregamento.EscolherHorarioCarregamentoPorLista.val(sessao.EscolherHorarioCarregamentoPorLista);
            _carregamento.EscolherHorarioCarregamentoPorLista.def = sessao.EscolherHorarioCarregamentoPorLista;

            if (_pesquisaResumoCarregamentos)
                _pesquisaResumoCarregamentos.Reload.val(true);

            PreencherObjetoKnoutParametros(sessao.Parametros);

            if (!somenteSessao) {
                BuscarDadosMontagemCarga(2); // Abrir Sessão...
            }

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function PreencherObjetoKnoutParametros(json) {
    //Parametros
    var parametros = JSON.parse(json);
    PreencherObjetoKnout(_sessaoRoteirizadorParametros, { Data: parametros });

    var frota = parametros.DisponibilidadesFrota;
    for (var i in frota) {
        frota[i].DT_Enable = true;
    }
    _gridDisponibilidadesFrota.CarregarGrid(frota);

    var tempos = parametros.TemposCarregamento;
    for (var i in tempos) {
        tempos[i].DT_Enable = true;
    }
    _gridTemposCarregamento.CarregarGrid(tempos);
}

function BuscarPedidosInconsistentes(callback) {
    var codigo = parseInt(_pesquisaMontegemCarga.SessaoRoteirizador.codEntity());
    if (codigo > 0) {
        var data = { Codigo: codigo };
        executarReST("SessaoRoteirizador/ObterPedidosInconsistentes", data, function (arg) {
            if (arg.Success) {
                _pedidosInconsistentesMotivos = arg.Data.Registros;
                refreshPedidosInconsistentes();
                if (callback instanceof Function) {
                    callback();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function refreshPedidosInconsistentes() {
    if (_pedidosInconsistentesMotivos != undefined && _pedidosInconsistentesMotivos != null && PEDIDOS != null && PEDIDOS != undefined) {
        //Achar na lista de pedidos, os pedidos e motivos...
        PEDIDOS_INCONSISTENTES.removeAll();
        for (var i in _pedidosInconsistentesMotivos) {
            var pedido = ObterPedidoPorCodigo(_pedidosInconsistentesMotivos[i].Codigo);
            if (pedido != null) {

                PEDIDOS_INCONSISTENTES.push(
                    {
                        Codigo: pedido.Codigo,
                        DataPrevisaoEntrega: pedido.DataPrevisaoEntrega,
                        DT_RowColor: pedido.DT_RowColor,
                        NumeroReboque: pedido.NumeroReboque,
                        NumeroPedidoEmbarcador: pedido.NumeroPedidoEmbarcador,
                        Peso: pedido.Peso,
                        Cubagem: pedido.Cubagem,
                        DataPrevisaoEntrega: pedido.DataPrevisaoEntrega,
                        CanalEntrega: pedido.CanalEntrega,
                        Pallet: pedido.TotalPallets,
                        Destinatario: pedido.Destinatario,
                        Destino: pedido.Destino,
                        Remetente: pedido.Remetente,
                        Origem: pedido.Origem,
                        MotivoInconsistencia: _pedidosInconsistentesMotivos[i].DescricaoSituacao,
                        TipoCarregamentoPedido: pedido.TipoCarregamentoPedido
                    });
            }
        }
        RenderizarGridPedidosInconsistentes();
    }
}

function removerPedidoSessao(pedidos, cancelarReserva, cancelarPedidos) {
    var codigos = [];
    for (var i = 0; i < pedidos.length; i++)
        codigos.push(pedidos[i].Codigo);

    var msg = Localization.Resources.Cargas.MontagemCargaMapa.TemCertezaQueDesejaRemoverOsPedidosSelecionadosDaSessaoDoRoteirizador;
    if (cancelarReserva)
        msg = Localization.Resources.Cargas.MontagemCargaMapa.TemCertezaQueDesejaCancelarSaldoDosPedidosSelecionadosDaSessaoDoRoteirizador;
    else if (cancelarPedidos)
        msg = Localization.Resources.Cargas.MontagemCargaMapa.TemCertezaQueDesejaRemoverECancelarOsPedidosSelecionadosDaSessaoDoRoteirizador;

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, msg, function () {

        var data = { pedidos: JSON.stringify(codigos) };
        data.Codigo = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
        data.CancelarReserva = cancelarReserva;
        data.CancelarPedidos = cancelarPedidos;

        //data.CanalEntrega = canais;
        executarReST("SessaoRoteirizador/RemoverPedidos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                    LimparPedidosSelecionados();
                    //Reconsultar...
                    BuscarDadosMontagemCarga(2); // ABRIR SESSÃO.
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                //Se deu erro ou Data for numerico, conseguiu cancelar algusn e vamos reconsultar...
                if (!isNaN(arg.Data)) {
                    LimparPedidosSelecionados();
                    //Reconsultar...
                    BuscarDadosMontagemCarga(2); // ABRIR SESSÃO.
                }
            }
        });
    });
}

function validaConfiguracaoVisible() {
    BuscarPedidosInconsistentes();
    var visible = $("#modal-carregamentos").is(':visible');
    if (visible) {
        exibirPedidosCarregamentos();
    }
}

/// PRE-FILTROS

function abrirPreFiltroParametrosSessaoRoteirizador(data) {
    if (data) {
        _preFiltroParametrosSessaoRoteirizador.CodigoFiltro.val(data.Codigo);
        _preFiltroParametrosSessaoRoteirizador.NomeFiltro.val(data.NomeFiltro);
        PreencherObjetoKnoutParametros(data.Dados);
    }
}

function SalvarPreFiltroParametrosSessaoRoteirizador() {
    modalPreFiltroParametrosSessaoRoteirizador(true);
}

function modalPreFiltroParametrosSessaoRoteirizador(show) {
    if (show) {
        Global.abrirModal('modalSalvarPreFiltroParametrosSessaoRoteirizador');
    } else {
        Global.fecharModal('modalSalvarPreFiltroParametrosSessaoRoteirizador');
    }
}

function limparPreFiltroParametrosSessaoRoteirizadorClick() {
    _preFiltroParametrosSessaoRoteirizador.CodigoFiltro.val(0);
    _preFiltroParametrosSessaoRoteirizador.NomeFiltro.val('');
}

function excluirPreFiltroParametrosSessaoRoteirizadorClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaExcluirFiltroDeParametro, function () {
        var data = { CodigoFiltro: _preFiltroParametrosSessaoRoteirizador.CodigoFiltro.val() };
        executarReST("SessaoRoteirizador/ExcluirPreFiltrosMontagemCarregamento", data, function (arg) {
            if (arg.Success) {
                modalPreFiltroParametrosSessaoRoteirizador(false);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.FiltroDeParametrosExlucidosComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        });
    });
}

// FIM PRÉ-FILTROS

function resetParametrosSessao() {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.TemCertezaQueDesejaRestaurarAsConfiguracoesDoCentroDeCarregamento, function () {
        var dados = {
            Codigo: _sessaoRoteirizador.Codigo.val()
        };

        executarReST("SessaoRoteirizador/ObterParametrosSessaoRoteirizador", dados, function (arg) {
            if (arg.Success) {
                PreencherObjetoKnoutParametros(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        });
    });
}

function obterParametros() {
    var parametros = {
        TipoMontagemCarregamentoVRP: _sessaoRoteirizadorParametros.TipoMontagemCarregamentoVRP.val(),
        TipoOcupacaoMontagemCarregamentoVRP: _sessaoRoteirizadorParametros.TipoOcupacaoMontagemCarregamentoVRP.val(),
        ConsiderarTempoDeslocamentoCD: _sessaoRoteirizadorParametros.ConsiderarTempoDeslocamentoCD.val(),
        GerarCarregamentoDoisDias: _sessaoRoteirizadorParametros.GerarCarregamentoDoisDias.val(),
        GerarCarregamentosAlemDaDispFrota: _sessaoRoteirizadorParametros.GerarCarregamentosAlemDaDispFrota.val(),
        UtilizarDispFrotaCentroDescCliente: _sessaoRoteirizadorParametros.UtilizarDispFrotaCentroDescCliente.val(),
        QuantidadeMaximaEntregasRoteirizar: _sessaoRoteirizadorParametros.QuantidadeMaximaEntregasRoteirizar.val(),
        MontagemCarregamentoPedidoProduto: _sessaoRoteirizadorParametros.MontagemCarregamentoPedidoProduto.val(),
        CarregamentoTempoMaximoRota: _sessaoRoteirizadorParametros.CarregamentoTempoMaximoRota.val(),
        NivelQuebraProdutoRoteirizar: _sessaoRoteirizadorParametros.NivelQuebraProdutoRoteirizar.val(),
        AgruparPedidosMesmoDestinatario: _sessaoRoteirizadorParametros.AgruparPedidosMesmoDestinatario.val(),
        OcultarDetalhesDoPontoNoMapa: _sessaoRoteirizadorParametros.OcultarDetalhesDoPontoNoMapa.val(),
        DisponibilidadesFrota: _gridDisponibilidadesFrota.BuscarRegistros(),
        TemposCarregamento: _gridTemposCarregamento.BuscarRegistros()
    };

    return parametros;
}

function salvarParametrosSessao() {

    var parametros = obterParametros();

    var dados = {
        Codigo: _sessaoRoteirizador.Codigo.val(),
        Parametros: JSON.stringify(parametros)
    };

    executarReST("SessaoRoteirizador/SalvarParametrosSessaoRoteirizador", dados, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ParametrosSalvoComSucesso);
            Global.fecharModal('divModalParametrosSessao');
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
    });

}

function salvarParametrosSessaoPreFiltroClick() {

    if (!ValidarCamposObrigatorios(_preFiltroParametrosSessaoRoteirizador)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    var parametros = obterParametros();

    var dados = {
        CodigoFiltro: _preFiltroParametrosSessaoRoteirizador.CodigoFiltro.val(),
        NomeFiltro: _preFiltroParametrosSessaoRoteirizador.NomeFiltro.val(),
        FiltroPesquisa: JSON.stringify(parametros),
        TipoFiltro: 2 // ParametrosCentroCarregamentoSessaoRoteirizacao
    };

    executarReST("SessaoRoteirizador/SalvarPreFiltrosMontagemCarregamento", dados, function (arg) {
        if (arg.Success) {
            modalPreFiltroParametrosSessaoRoteirizador(false);
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.FiltrosSalvosComSucesso);
            limparPreFiltroParametrosSessaoRoteirizadorClick();
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
    });
}