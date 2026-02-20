/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarGuiasRecolhimento = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Guia", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Guias", type: types.local });
        this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(), cssClass: ko.observable("col col-xs-6 col-lg-3"), col: 2 });
        this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final:", getType: typesKnockout.date, col: 2 });
        this.Status = PropertyEntity({ text: "Status:", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoGuia.obterOpcoes(), visible: ko.observable(true), col: 4 });
        this.Carga = PropertyEntity({ text: "Número Carga:", col: 2 });
        this.SerieCte = PropertyEntity({ text: "Série CT-e:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 10, col: 2 });
        this.NroCte = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 10, col: 2 });
        this.ChaveCte = PropertyEntity({ text: "Chave CT-e:", getType: typesKnockout.text, maxlength: 44, col: 6 });
        this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true), col: 4 });
        this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), col: 6 });
        this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true), text: "Veículo:", idBtnSearch: guid(), col: 6 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), visible: false });
        this.OcultarColunaToken = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false, function () {

        new BuscarTransportadores(knoutOpcoes.Transportador);
        new BuscarVeiculos(knoutOpcoes.Veiculo);
        new BuscarMotorista(knoutOpcoes.Motorista);
    });



    new BuscarTransportadores(knoutOpcoes.Transportador);
    new BuscarVeiculos(knoutOpcoes.Veiculo);
    new BuscarMotorista(knoutOpcoes.Motorista);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GuiaNacionalRecolhimentoTributoEstual/ConsultarGuias", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}