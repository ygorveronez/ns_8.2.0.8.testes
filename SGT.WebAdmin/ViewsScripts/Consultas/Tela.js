/// <reference path="Sistema.js" />
/// <reference path="Modulo.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTelas = function (knout, callbackRetorno, menuOpcoes, knoutSistema, knoutModulo) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Telas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Telas", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });
        this.Status = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, visible: false });
        this.Sistema = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Sistema:", idBtnSearch: guid(), visible: true });
        this.Modulo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Módulo:", idBtnSearch: guid(), visible: true });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutSistema != null) {
        if (knoutSistema.codEntity() > 0) {
            knoutOpcoes.Sistema.visible = false;
            funcaoParamentroDinamico = function () {
                knoutOpcoes.Sistema.codEntity(knoutSistema.codEntity());
                knoutOpcoes.Sistema.val(knoutSistema.val());
            }
        }
    } else
        knoutOpcoes.Sistema.visible = true;

    if (knoutModulo != null) {
        if (knoutModulo.codEntity() > 0) {
            knoutOpcoes.Modulo.visible = false;
            funcaoParamentroDinamico = function () {
                knoutOpcoes.Modulo.codEntity(knoutModulo.codEntity());
                knoutOpcoes.Modulo.val(knoutModulo.val());
            }
        }
    } else
        knoutOpcoes.Modulo.visible = true;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarSistemas(knoutOpcoes.Sistema);
        new BuscarModulos(knoutOpcoes.Modulo);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    }

    var opcoes = divBusca.OpcaoPadrao(callback)
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Tela/Pesquisa", knoutOpcoes, opcoes, null);
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
    })
}