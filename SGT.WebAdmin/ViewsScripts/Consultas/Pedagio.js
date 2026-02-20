/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="Veiculo.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarPedagiosSemAcertoDeViagem = function (knout, callbackRetorno, basicGrid, knoutCodigoAcertoViagem, knoutTipoPedagio) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Pedágios", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedágios", type: types.local });
        this.Praca = PropertyEntity({ text: "Praça ", col: 6 });
        this.Data = PropertyEntity({ text: "Data Inicial: ", col: 3, getType: typesKnockout.date, visible: true });
        this.DataFinal = PropertyEntity({ text: "Data Final: ", col: 3, getType: typesKnockout.date, visible: true });
        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });
        this.TipoPedagio = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Tipo:", idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoAcertoViagem != null && knoutTipoPedagio == null) {
        knoutOpcoes.AcertoViagem.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutCodigoAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutCodigoAcertoViagem.val());
        }
    } else if (knoutCodigoAcertoViagem != null && knoutTipoPedagio != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutCodigoAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutCodigoAcertoViagem.val());
            knoutOpcoes.TipoPedagio.codEntity(knoutTipoPedagio);
            knoutOpcoes.TipoPedagio.val(knoutTipoPedagio);
        }
    } else if (knoutCodigoAcertoViagem == null && knoutTipoPedagio != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.TipoPedagio.codEntity(knoutTipoPedagio);
            knoutOpcoes.TipoPedagio.val(knoutTipoPedagio);
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);
    

    var callback = function (e) {
        preecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedagio/PesquisaPedagiosSemAcertoDeViagem", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedagio/PesquisaPedagiosSemAcertoDeViagem", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}