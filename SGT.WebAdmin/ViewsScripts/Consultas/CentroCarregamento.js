/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Filial.js" />


var BuscarCentrosCarregamento = function (knout, callbackRetorno, basicGrid, knoutFilial, somenteCentrosOperadorLogistica, somenteCentrosManobra) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    if (somenteCentrosOperadorLogistica == null)
        somenteCentrosOperadorLogistica = false;

    if (somenteCentrosManobra == null)
        somenteCentrosManobra = false;
    
    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.CentroCarregamento.BuscarCentrosDeCarregamento, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.CentroCarregamento.CentrosDeCarregamento, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Filial = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.CentroCarregamento.Filial.getFieldDescription(), idBtnSearch: guid() });
        this.Ativo = PropertyEntity({ visible: false, def: 1, val: ko.observable(1) });
        this.SomenteCentrosOperadorLogistica = PropertyEntity({ visible: false, def: somenteCentrosOperadorLogistica, val: ko.observable(somenteCentrosOperadorLogistica) });
        this.SomenteCentrosManobra = PropertyEntity({ visible: false, def: somenteCentrosManobra, val: ko.observable(somenteCentrosManobra) });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.CentroCarregamento.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutFilial != null) {
        knoutOpcoes.Filial.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroCarregamento/Pesquisa", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroCarregamento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}