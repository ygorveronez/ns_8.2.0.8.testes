/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarMotivosIrregularidade = function (knout, callbackRetorno, basicGrid, knoutIrregularidade) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) 
        multiplaEscolha = true;
    
    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Motivos de Irregularidade", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivo Irregularidade", type: types.local });

        this.Descricao = PropertyEntity({ col: 10, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), val: ko.observable(), getType: typesKnockout.string });
        this.Situacao = PropertyEntity({ col: 2, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(0), options: _statusFemPesquisa, def: 0 });
        this.Irregularidade = PropertyEntity({ text: 'Irregularidade:', val: ko.observable(), codEntity: ko.observable(0), type: types.entity });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;
    if (knoutIrregularidade != null) {
        knoutOpcoes.Irregularidade.visible = false;
        funcaoParamentroDinamico = function () {
            if (knoutIrregularidade != null) {
                knoutOpcoes.Irregularidade.codEntity(knoutIrregularidade.codEntity());
                knoutOpcoes.Irregularidade.val(knoutIrregularidade.val());
            }
        }
    }

        var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoIrregularidade/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoIrregularidade/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
    })
}