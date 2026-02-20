/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoTabelaFrete.js" />

var BuscarTabelasDeFrete = function (knout, callbackRetorno, tipoTabelaFrete, basicGrid, limiteRegistros) {

    var idDiv = guid();
    var GridConsulta;

    var tipoTabela = 0;
    if (tipoTabelaFrete != null) {
        tipoTabela = tipoTabelaFrete;
    }

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TabelaFrete.BuscarTabelasDeFrete, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TabelaFrete.TabelasDeFrete, type: types.local });
        this.Descricao = PropertyEntity({ col: 9, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Ativo = PropertyEntity({ col: 3, val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
        this.TipoTabelaFrete = PropertyEntity({ val: ko.observable(tipoTabela), def: tipoTabela, visible: false });
        this.CalcularFreteDestinoPrioritario = PropertyEntity({ val: ko.observable(false), def: false, visible: false });
        
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, null, null, null, null, null, limiteRegistros);
    

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }


    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TabelaFrete/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TabelaFrete/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

    this.Opcoes = knoutOpcoes;
}