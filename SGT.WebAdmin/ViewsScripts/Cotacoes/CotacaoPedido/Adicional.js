/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoClienteCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumStatusCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="CotacaoPedido.js" />

var _cotacaoPedidoAdicional;
var _gridCubagens;

var CubagemMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Altura = PropertyEntity({ type: types.map, val: "" });
    this.Comprimento = PropertyEntity({ type: types.map, val: "" });
    this.Largura = PropertyEntity({ type: types.map, val: "" });
    this.QtdVolume = PropertyEntity({ type: types.map, val: "" });
    this.MetroCubico = PropertyEntity({ type: types.map, val: "" });
    this.FatorCubico = PropertyEntity({ type: types.map, val: "" });
    this.PesoCubado = PropertyEntity({ type: types.map, val: "" });
}

var CotacaoPedidoAdicional = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicialColeta = PropertyEntity({ text: ko.observable("Previsão de Saída:"), getType: typesKnockout.dateTime, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataFinalColeta = PropertyEntity({ text: ko.observable("Previsão de Retorno:"), getType: typesKnockout.dateTime, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoModal = PropertyEntity({ val: ko.observable(EnumTipoModal.Rodoviario), options: EnumTipoModal.obterOpcoes(), def: EnumTipoModal.Rodoviario, text: "*Tipo Modal: ", required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroPaletes = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: ko.observable("Nº Pallets:"), configInt: { precision: 0, allowZero: false, thousands: "" }, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.PesoTotal = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: ko.observable("Peso Bruto:"), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorTotalNotasFiscais = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: ko.observable("Valor Mercadoria:"), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.QuantidadeNotas = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: ko.observable("Qtd. Notas:"), configInt: { precision: 0, allowZero: false, thousands: "" }, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.QtdEntregas = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: ko.observable("Qtd. Entregas:"), configInt: { precision: 0, allowZero: false, thousands: "" }, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Temperatura = PropertyEntity({ text: "Temperatura: ", maxlength: 50, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.KMTotal = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: "KM Total:", configInt: { precision: 0, allowZero: false, thousands: "" }, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorPorKM = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Valor por KM:", required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoPagamento = PropertyEntity({ val: ko.observable(EnumTipoPagamento.A_Pagar), options: EnumTipoPagamento.obterOpcoes(), def: EnumTipoPagamento.A_Pagar, text: "Tipo do Pagamento:", issue: 120, required: false, eventChange: tipoPagamentoChange });
    this.ObservacaoInterna = PropertyEntity({ text: "Observação Interna: ", maxlength: 2000, required: false, visible: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: ko.observable("Observação Pedido: "), maxlength: 2000, visible: ko.observable(true), enable: ko.observable(true), required: false });

    this.Rastreado = PropertyEntity({ getType: typesKnockout.bool, text: "Será rastreado?", val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.GerenciamentoRisco = PropertyEntity({ getType: typesKnockout.bool, text: "Terá Gerenciamento de Risco?", val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.EscoltaArmada = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable("Necessário Escolta?"), val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.QtdEscoltas = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: "Escolta? Informe a quantidade:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Ajudante = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable("Ajudante?"), val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.QtdAjudantes = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, text: ko.observable("Ajudantes? Informe a quantidade:"), required: false, visible: ko.observable(true), enable: ko.observable(true) });
        
    this.Altura = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Altura:", required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.Comprimento = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Comprimento:", required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.Largura = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Largura:", required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.QtdVolume = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: "*Qtd. Volume:", configInt: { precision: 0, allowZero: false, thousands: "" }, required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });    
    this.MetroCubico = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, maxlength: 15, text: "*M. Cúbico:", required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });    
    this.FatorCubico = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Fator Cúbico:", required: false, visible: ko.observable(true), enable: ko.observable(true), val: ko.observable("") });
    this.PesoCubado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, maxlength: 15, text: ko.observable("*Peso Cubado:"), required: false, visible: ko.observable(true), enable: ko.observable(false), val: ko.observable("") });

    this.AdicionarCubagem = PropertyEntity({ type: types.event, eventClick: AdicionarCubagemClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.Cubagens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.ListaCubagem = PropertyEntity({ val: ko.observable(""), def: "" });

    this.CubagemTotal = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, maxlength: 15, text: "Metro Cúbico:", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.TotalPesoCubado = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, maxlength: 15, text: ko.observable("*Peso Cubado:"), required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.QtVolumes = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10, text: "Total Volumes:", required: false, visible: ko.observable(true), enable: ko.observable(false) });

    this.Altura.val.subscribe(function () {
        AjustarMetrosCubicosCotacaoPedidoAdicional();
    });
    this.Comprimento.val.subscribe(function () {
        AjustarMetrosCubicosCotacaoPedidoAdicional();
    });
    this.Largura.val.subscribe(function () {
        AjustarMetrosCubicosCotacaoPedidoAdicional();
    });
    this.QtdVolume.val.subscribe(function () {
        AjustarMetrosCubicosCotacaoPedidoAdicional();
    });
    this.FatorCubico.val.subscribe(function () {
        AjustarMetrosCubicosCotacaoPedidoAdicional();
    });
};

//*******EVENTOS*******

function loadCotacaoPedidoAdicional() {
    _cotacaoPedidoAdicional = new CotacaoPedidoAdicional();
    KoBindings(_cotacaoPedidoAdicional, "knockoutAdicionais");

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirCubagem(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [{ data: "Codigo", visible: false },
    { data: "Altura", title: "Altura", width: "10%" },
    { data: "Comprimento", title: "Comprimento", width: "10%" },
    { data: "Largura", title: "Largura", width: "10%" },
    { data: "QtdVolume", title: "Qtd. Volume", width: "10%" },
    { data: "MetroCubico", title: "M³", width: "10%" },
    { data: "FatorCubico", title: "Fator", width: "10%" },
    { data: "PesoCubado", title: "Peso", width: "10%" }];

    _gridCubagens = new BasicDataTable(_cotacaoPedidoAdicional.Cubagens.idGrid, header, menuOpcoes);
    recarregarGridCubagens();
}

function ControlarCamposObrigatoriosAdicional() {
    if (_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosPedido === true && _cotacaoPedido.StatusCotacaoPedido.val() === EnumStatusCotacaoPedido.Fechada) {
        //_cotacaoPedidoAdicional.Observacao.text("*Observação:");
        //_cotacaoPedidoAdicional.Observacao.required = true;
        //_cotacaoPedidoAdicional.Observacao.visible(true);

        //_cotacaoPedidoAdicional.DataInicialColeta.text("*Previsão de Saída:");
        //_cotacaoPedidoAdicional.DataInicialColeta.required = true;

        //_cotacaoPedidoAdicional.DataFinalColeta.text("*Previsão de Retorno:");
        //_cotacaoPedidoAdicional.DataFinalColeta.required = true;

        //_cotacaoPedidoAdicional.NumeroPaletes.text("*Nº Pallets:");
        //_cotacaoPedidoAdicional.NumeroPaletes.required = true;

        //_cotacaoPedidoAdicional.PesoTotal.text("*Peso Bruto:");
        //_cotacaoPedidoAdicional.PesoTotal.required = true;

        //_cotacaoPedidoAdicional.QtdEntregas.text("*Qtd. Entregas:");
        //_cotacaoPedidoAdicional.QtdEntregas.required = true;

        //_cotacaoPedidoAdicional.TotalPesoCubado.text("*Peso Cubado:");
        //_cotacaoPedidoAdicional.TotalPesoCubado.required = true;        

        //_cotacaoPedidoAdicional.QuantidadeNotas.text("*Qtd. Notas:");
        //_cotacaoPedidoAdicional.QuantidadeNotas.visible(true);
        //_cotacaoPedidoAdicional.QuantidadeNotas.required = true;
    }
}

function AdicionarCubagemClick(e, sender) {
    var tudoCerto = true;
    if (_cotacaoPedidoAdicional.Altura.val() === "")
        tudoCerto = false;
    if (_cotacaoPedidoAdicional.Comprimento.val() === "")
        tudoCerto = false;
    if (_cotacaoPedidoAdicional.Largura.val() === "")
        tudoCerto = false;
    if (_cotacaoPedidoAdicional.QtdVolume.val() === "")
        tudoCerto = false;
    if (_cotacaoPedidoAdicional.MetroCubico.val() === "")
        tudoCerto = false;
    if (_cotacaoPedidoAdicional.PesoCubado.val() === "")
        tudoCerto = false;

    if (tudoCerto) {
        var map = new Object();

        map.Codigo = guid();
        map.Altura = _cotacaoPedidoAdicional.Altura.val();
        map.Comprimento = _cotacaoPedidoAdicional.Comprimento.val();
        map.Largura = _cotacaoPedidoAdicional.Largura.val();
        map.QtdVolume = _cotacaoPedidoAdicional.QtdVolume.val();
        map.MetroCubico = _cotacaoPedidoAdicional.MetroCubico.val();
        map.FatorCubico = _cotacaoPedidoAdicional.FatorCubico.val();
        map.PesoCubado = _cotacaoPedidoAdicional.PesoCubado.val();

        _cotacaoPedidoAdicional.Cubagens.list.push(map);

        recarregarGridCubagens();
        limparDadosCubagem();
        $("#" + _cotacaoPedidoAdicional.Altura.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento da cubagem!");
    }
}

function excluirCubagem(e) {
    if (_cotacaoPedido.SituacaoPedido.val() !== EnumSituacaoPedido.Aberto) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O status da cotação não permite remover a cubagem!");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja a cubagem selecionada?", function () {
        $.each(_cotacaoPedidoAdicional.Cubagens.list, function (i, cubagem) {
            if (cubagem !== null && cubagem.Codigo !== null && e !== null && e.Codigo !== null && e.Codigo === cubagem.Codigo) {
                _cotacaoPedidoAdicional.Cubagens.list.splice(i, 1);
            }
        });
        recarregarGridCubagens();
    });
}

//********METODOS**********

function AjustarMetrosCubicosCotacaoPedidoAdicional() {
    var altura = Globalize.parseFloat(_cotacaoPedidoAdicional.Altura.val());
    var largura = Globalize.parseFloat(_cotacaoPedidoAdicional.Largura.val());
    var comprimento = Globalize.parseFloat(_cotacaoPedidoAdicional.Comprimento.val());
    var volumes = Globalize.parseFloat(_cotacaoPedidoAdicional.QtdVolume.val());
    var fatorCubico = Globalize.parseFloat(_cotacaoPedidoAdicional.FatorCubico.val());

    if (isNaN(volumes))
        volumes = 0;
    if (isNaN(altura))
        altura = 0;
    if (isNaN(largura))
        largura = 0;
    if (isNaN(comprimento))
        comprimento = 0;
    if (isNaN(fatorCubico))
        fatorCubico = 0;

    var totalMetroCubico = (altura * largura * comprimento * volumes);

    _cotacaoPedidoAdicional.MetroCubico.val(Globalize.format(totalMetroCubico, "n3"));
    if (fatorCubico > 0)
        _cotacaoPedidoAdicional.PesoCubado.val(Globalize.format((totalMetroCubico * fatorCubico), "n3"));
    else
        _cotacaoPedidoAdicional.PesoCubado.val(Globalize.format(totalMetroCubico, "n3"));
}

function recarregarGridCubagens() {
    var data = new Array();

    var totalMetroCubico = 0;
    var totalPesoCupado = 0;
    var totalQuantidadeVolume = 0;

    $.each(_cotacaoPedidoAdicional.Cubagens.list, function (i, cubagem) {
        var obj = new Object();

        obj.Codigo = cubagem.Codigo;
        obj.Altura = cubagem.Altura;
        obj.Comprimento = cubagem.Comprimento;
        obj.Largura = cubagem.Largura;
        obj.QtdVolume = cubagem.QtdVolume;
        obj.MetroCubico = cubagem.MetroCubico;
        obj.FatorCubico = cubagem.FatorCubico;
        obj.PesoCubado = cubagem.PesoCubado;

        var metroCubico = Globalize.parseFloat(cubagem.MetroCubico);
        var pesoCupado = Globalize.parseFloat(cubagem.PesoCubado);
        var quantidadeVolume = Globalize.parseFloat(cubagem.QtdVolume);

        if (isNaN(metroCubico))
            metroCubico = 0;
        if (isNaN(pesoCupado))
            pesoCupado = 0;
        if (isNaN(quantidadeVolume))
            quantidadeVolume = 0;        

        totalMetroCubico += metroCubico;
        totalPesoCupado += pesoCupado;
        totalQuantidadeVolume += quantidadeVolume;        

        data.push(obj);
    });

    _cotacaoPedidoAdicional.CubagemTotal.val(Globalize.format(totalMetroCubico, "n3"));
    _cotacaoPedidoAdicional.TotalPesoCubado.val(Globalize.format(totalPesoCupado, "n3"));
    _cotacaoPedidoAdicional.QtVolumes.val(totalQuantidadeVolume);
    _gridCubagens.CarregarGrid(data);
}

function limparDadosCubagem() {
    _cotacaoPedidoAdicional.Altura.val("");
    _cotacaoPedidoAdicional.Comprimento.val("");
    _cotacaoPedidoAdicional.Largura.val("");
    _cotacaoPedidoAdicional.QtdVolume.val("");
    _cotacaoPedidoAdicional.MetroCubico.val("");
    _cotacaoPedidoAdicional.FatorCubico.val("");
    _cotacaoPedidoAdicional.PesoCubado.val("");
}

function limparCotacaoPedidoAdicional() {
    LimparCampos(_cotacaoPedidoAdicional);
    recarregarGridCubagens();    

    _cotacaoPedidoAdicional.Observacao.text("Observação Pedido:");
    _cotacaoPedidoAdicional.Observacao.required = false;

    _cotacaoPedidoAdicional.DataInicialColeta.text("Previsão de Saída:");
    _cotacaoPedidoAdicional.DataInicialColeta.required = false;

    _cotacaoPedidoAdicional.DataFinalColeta.text("Previsão de Retorno:");
    _cotacaoPedidoAdicional.DataFinalColeta.required = false;

    _cotacaoPedidoAdicional.NumeroPaletes.text("Nº Pallets:");
    _cotacaoPedidoAdicional.NumeroPaletes.required = false;

    _cotacaoPedidoAdicional.PesoTotal.text("Peso Bruto:");
    _cotacaoPedidoAdicional.PesoTotal.required = false;

    _cotacaoPedidoAdicional.QtdEntregas.text("Qtd. Entregas:");
    _cotacaoPedidoAdicional.QtdEntregas.required = false;

    _cotacaoPedidoAdicional.TotalPesoCubado.text("Peso Cubado:");
    _cotacaoPedidoAdicional.TotalPesoCubado.required = false;
    _cotacaoPedidoAdicional.TotalPesoCubado.val("");

    _cotacaoPedidoAdicional.QuantidadeNotas.text("Qtd. Notas:");    
    _cotacaoPedidoAdicional.QuantidadeNotas.required = false;
}


function VerificarDadosAdicionais() {
    if (!ValidarCamposObrigatorios(_cotacaoPedidoAdicional)) {
        $("#myTab a:eq(3)").tab("show");
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
        return false;
    } else {       
        return true;
    }
}

function tipoPagamentoChange() {
    if (_adicional.TipoPagamento.val() == EnumTipoPagamento.Outros) {
        _adicional.Tomador.required = true;
        _adicional.Tomador.text(Localization.Resources.Cotacoes.CotacaoPedido.Tomador.getRequiredFieldDescription());
    }
    else {
        _adicional.Tomador.required = false;
        _adicional.Tomador.text(Localization.Resources.Cotacoes.CotacaoPedido.Tomador.getRequiredFieldDescription());
        _adicional.Tomador.text(Localization.Resources.Pedido)

    }

}
