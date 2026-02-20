/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumRegraOfertaCarga.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeOfertaCarga.js" />

var _ofertaCargaKnockout;
var _gridOfertaCarga;

var OfertaCarga = function () {
    this.AtivarRegraParaOfertarCarga = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.AtivarRegraParaOfertarCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.PeriodoDiferenciadoShare = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PreencherPeriodoDiferenciadoShare, getType: typesKnockout.bool, val: ko.observable(false) });
    this.DataInicialPeriodoDiferenciadoShare = PropertyEntity({ text: Localization.Resources.Gerais.Geral.De.getRequiredFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), required: ko.observable(false) });
    this.DataFinalPeriodoDiferenciadoShare = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Ate.getRequiredFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), required: ko.observable(false) });
    
    this.Regra = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.Regra, val: ko.observable(), required: true, options: EnumRegraOfertaCarga.obterOpcoes()  });
    this.Prioridade = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.Prioridade, val: ko.observable(), required: true, options: EnumPrioridadeOfertaCarga.obterOpcoes() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarConfiguracaoCargaOferta, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.ListaOfertaCarga = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });

    this.ListaOfertaCarga.val.subscribe(
        function () {
            recarregarGridOfertaCarga();
        }
    );

    this.PeriodoDiferenciadoShare.val.subscribe((value) => {
        this.DataInicialPeriodoDiferenciadoShare.required(value);
        this.DataFinalPeriodoDiferenciadoShare.required(value);
    })
}

function loadOfertaCarga() {
    _ofertaCargaKnockout = new OfertaCarga();
    KoBindings(_ofertaCargaKnockout, "knockoutOfertaCarga");

    loadGridOfertaCarga();
}

function loadGridOfertaCarga() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirConfiguracaoOfertaCarga };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "RegraValue", visible: false },
        { data: "RegraDescricao", title: Localization.Resources.Logistica.CentroCarregamento.Regra, width: "50%" },
        { data: "Prioridade", title: Localization.Resources.Logistica.CentroCarregamento.Prioridade, width: "50%" }
    ];

    _gridOfertaCarga = new BasicDataTable(_ofertaCargaKnockout.ListaOfertaCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridOfertaCarga();
}

function recarregarGridOfertaCarga() {
    var data = new Array();

    $.each(_ofertaCargaKnockout.ListaOfertaCarga.val(), function (i, ofertaCarga) {
        var configOfertaCarga = new Object();

        configOfertaCarga.Codigo = ofertaCarga.Codigo;
        configOfertaCarga.RegraDescricao = ofertaCarga.RegraDescricao;
        configOfertaCarga.RegraValue = ofertaCarga.RegraValue;
        configOfertaCarga.Prioridade = ofertaCarga.Prioridade;

        data.push(configOfertaCarga);
    });

    _gridOfertaCarga.CarregarGrid(data);
}

function preencherOfertaCargaSalvar(centroCarregamento) {
    centroCarregamento["OfertaCarga"] = JSON.stringify(_ofertaCargaKnockout.ListaOfertaCarga.val());
    centroCarregamento["AtivarRegraParaOfertarCarga"] = _ofertaCargaKnockout.AtivarRegraParaOfertarCarga.val();
    centroCarregamento["PeriodoDiferenciadoShare"] = _ofertaCargaKnockout.PeriodoDiferenciadoShare.val();
    centroCarregamento["DataInicialPeriodoDiferenciadoShare"] = _ofertaCargaKnockout.DataInicialPeriodoDiferenciadoShare.val();
    centroCarregamento["DataFinalPeriodoDiferenciadoShare"] = _ofertaCargaKnockout.DataFinalPeriodoDiferenciadoShare.val();
}

function preencherOfertaCarga(data) {
    _ofertaCargaKnockout.ListaOfertaCarga.val(data.OfertasCarga);
    _ofertaCargaKnockout.AtivarRegraParaOfertarCarga.val(data.AtivarRegraParaOfertarCarga);
    _ofertaCargaKnockout.PeriodoDiferenciadoShare.val(data.PeriodoDiferenciadoShare);
    _ofertaCargaKnockout.DataInicialPeriodoDiferenciadoShare.val(data.DataInicialPeriodoDiferenciadoShare);
    _ofertaCargaKnockout.DataFinalPeriodoDiferenciadoShare.val(data.DataFinalPeriodoDiferenciadoShare);
}

function adicionarConfiguracaoCargaOferta() {
    if (!ValidarCamposObrigatorios(_ofertaCargaKnockout)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }


    var data = [];
    var ofertaCarga = new Object();

    ofertaCarga.Codigo = guid();
    ofertaCarga.RegraDescricao = EnumRegraOfertaCarga.obterTexto(_ofertaCargaKnockout.Regra.val());
    ofertaCarga.RegraValue = _ofertaCargaKnockout.Regra.val();
    ofertaCarga.Prioridade = _ofertaCargaKnockout.Prioridade.val();

    var listaOfertaCarga = _ofertaCargaKnockout.ListaOfertaCarga.val();

    for (var i = 0; i < listaOfertaCarga.length; i++) {
        if (listaOfertaCarga[i].RegraValue == _ofertaCargaKnockout.Regra.val()) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Logistica.CentroCarregamento.JaExisteUmaConfiguracaoDeLanceContendoOsNumerosEscolhidos);
            return;
        }      
    }

    data = _ofertaCargaKnockout.ListaOfertaCarga.val();
    data.push(ofertaCarga);

    _ofertaCargaKnockout.ListaOfertaCarga.val(data);

    
}

function excluirConfiguracaoOfertaCarga(registroSelecionado) {
    var listaOfertaCarga = _ofertaCargaKnockout.ListaOfertaCarga.val();

    for (var i = 0; i < listaOfertaCarga.length; i++) {
        if (listaOfertaCarga[i].Codigo == registroSelecionado.Codigo)
            listaOfertaCarga.splice(i, 1);
    }

    _ofertaCargaKnockout.ListaOfertaCarga.val(listaOfertaCarga);
}

function limparCamposOfertaCarga() {
    _ofertaCargaKnockout.ListaOfertaCarga.val([]);
    LimparCampos(_ofertaCargaKnockout);
}