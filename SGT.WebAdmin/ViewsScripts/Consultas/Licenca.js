/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoLicenca.js" />

var BuscarLicenca = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Licenca.BuscarLicenca, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Licenca.Licencas, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
        this.Tipo = PropertyEntity({ visible: true, text: Localization.Resources.Consultas.Licenca.Tipo.getFieldDescription(), options: EnumTipoLicenca.obterOpcoesPesquisa() });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Licenca/Pesquisa", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Licenca/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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