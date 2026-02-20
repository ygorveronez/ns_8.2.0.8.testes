/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumEstado.js" />
/// <reference path="Localidade.js" />
/// <reference path="Cliente.js" />
/// <reference path="Regiao.js" />

var BuscarTabelasDeFreteCliente = function (knout, callbackRetorno, knoutTabelaFrete, basicGrid) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Tabelas de Frete Cliente", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Tabelas de Frete Cliente", type: types.local });

        this.LocalidadeOrigem = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
        this.LocalidadeDestino = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
        this.RegiaoDestino = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Região de Destino:", idBtnSearch: guid() });
        this.Tomador = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
        this.Remetente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
        this.Destinatario = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
        this.EstadoOrigem = PropertyEntity({ col: 3, options: EnumEstado.obterOpcoesPesquisa(), text: "Estado de Origem:", idBtnSearch: guid() });
        this.EstadoDestino = PropertyEntity({ col: 3, options: EnumEstado.obterOpcoesPesquisa(), text: "Estado de Destino:", idBtnSearch: guid() });
        this.CEPOrigem = PropertyEntity({ col: 3, text: "CEP de Origem:", getType: typesKnockout.cep });
        this.CEPDestino = PropertyEntity({ col: 3, text: "CEP de Destino:", getType: typesKnockout.cep });

        this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.SomenteEmVigencia = PropertyEntity({ val: ko.observable(true), def: true, visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutTabelaFrete != null) {
        funcaoParametroDinamico = function () {
            if (knoutTabelaFrete != null) {
                knoutOpcoes.TabelaFrete.codEntity(knoutTabelaFrete.codEntity());
                knoutOpcoes.TabelaFrete.val(knoutTabelaFrete.val());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {
        new BuscarLocalidades(knoutOpcoes.LocalidadeOrigem);
        new BuscarLocalidades(knoutOpcoes.LocalidadeDestino);
        new BuscarClientes(knoutOpcoes.Remetente);
        new BuscarClientes(knoutOpcoes.Destinatario);
        new BuscarClientes(knoutOpcoes.Tomador);
        new BuscarRegioes(knoutOpcoes.RegiaoDestino);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "TabelaFreteCliente/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "TabelaFreteCliente/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};