/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarLayoutsEDI = function (knout, tituloOpcional, tituloGridOpcional, callbackRetorno, basicGrid, tiposLayoutsEDI, knoutCodigoGruposPessoa) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;

    if (tiposLayoutsEDI == null)
        tiposLayoutsEDI = [];

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.LayoutEDI.ConsultaDeLayoutsDeEDI, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.LayoutEDI.LayoutsDeEDI, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.TipoLayoutEDI = PropertyEntity({ visible: false, val: ko.observable(JSON.stringify(tiposLayoutsEDI)), def: JSON.stringify(tiposLayoutsEDI) });
        this.GrupoPessoa = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.LayoutEDI.GrupoPessoa, idBtnSearch: guid(), visible: false });;
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
        if (knoutCodigoGruposPessoa != null) {
            knoutOpcoes.GrupoPessoa.visible = false;
            knoutOpcoes.GrupoPessoa.codEntity(knoutCodigoGruposPessoa.codEntity());
            knoutOpcoes.GrupoPessoa.val(knoutCodigoGruposPessoa.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    var url = "LayoutEDI/Pesquisar";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, { column: 1, dir: orderDir.asc }, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }

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