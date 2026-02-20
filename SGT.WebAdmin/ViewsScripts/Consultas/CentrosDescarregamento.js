/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Cliente.js" />

const BuscarCentrosDescarregamento = function (knout, callbackRetorno, basicGrid, somenteCentrosOperadorLogistica) {

    const idDiv = guid();
    let GridConsulta;

    let multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    if (somenteCentrosOperadorLogistica == null)
        somenteCentrosOperadorLogistica = false;

    const OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.CentrosDescarregamento.BuscarCentrosDescarregamento, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.CentrosDescarregamento.CentroDescarregamento, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.CentrosDescarregamento.Descricao.getFieldDescription() });
        this.Destinatario = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.CentrosDescarregamento.Destinatario.getFieldDescription(), idBtnSearch: guid() });
        this.Ativo = PropertyEntity({ visible: false, def: 1, val: ko.observable(1) });
        this.SomenteCentrosOperadorLogistica = PropertyEntity({ visible: false, def: somenteCentrosOperadorLogistica, val: ko.observable(somenteCentrosOperadorLogistica) });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.CentrosDescarregamento.Pesquisar, visible: true });
    }

    let knoutOpcoes = new OpcoesKnout();
    let funcaoParamentroDinamico = null;

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        BuscarClientes(knoutOpcoes.Destinatario);
    });

    let callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroDescarregamento/Pesquisa", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CentroDescarregamento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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