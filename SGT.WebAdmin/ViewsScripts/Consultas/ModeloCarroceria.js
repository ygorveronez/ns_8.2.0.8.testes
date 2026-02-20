/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="GrupoPessoa.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarModelosCarroceria = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) 
        multiplaEscolha = true;
    
    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ModeloCarroceria.BuscarModelosDeCarroceria, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ModeloCarroceria.ModelosDeCarroceria, type: types.local });

        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModeloCarroceria/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModeloCarroceria/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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