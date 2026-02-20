/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarSerieEmpresa = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, knoutEmpresa, tipoSerie) {

    var idDiv = guid();
    var GridConsulta;

    var tipo = 3;
    if (tipoSerie > 0) {
        tipo = tipoSerie;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisa de Série", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Séries", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Número: ", getType: typesKnockout.int });
        this.Empresa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: false });
        this.TipoSerie = PropertyEntity({ val: ko.observable(tipo), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutEmpresa != null) {
        knoutOpcoes.Empresa.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Empresa.codEntity(knoutEmpresa.codEntity());
            knoutOpcoes.Empresa.val(knoutEmpresa.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SerieEmpresa/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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