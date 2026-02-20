/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarNotaFiscal = function (knout, tipoEmissao, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.BuscarNotasFiscaisProprias, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.NotasFiscaisProprias, type: types.local });

        this.NumeroInicial = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.NumeroInicial.getFieldDescription() });
        this.NumeroFinal = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.NumeroFinal.getFieldDescription() });
        this.Serie = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.Serie.getFieldDescription() });
        this.TipoEmissao = PropertyEntity({
            val: ko.observable(tipoEmissao), def: tipoEmissao, text: Localization.Resources.Consultas.NotaFiscalEletronica.TipoEmissao.getFieldDescription(), visible: false
        });
        this.Chave = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.NotaFiscalEletronica.Chave.getFieldDescription() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NotaFiscalEletronica/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Chave.val(knout.val());
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


var BuscarXMLNotaFiscal = function (knout, callbackRetorno, knoutCarga, basicGrid, montagemContainer, knoutCargaEntrega, knoutCliente) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var urlPesquisa = "XMLNotaFiscal/Pesquisa";

    if (montagemContainer)
        urlPesquisa = "XMLNotaFiscal/PesquisaMontagemContainer";

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.BuscarNotasFiscais, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.NotasFiscais, type: types.local });

        this.Numero = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.Numero.getFieldDescription() });
        this.Serie = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.Serie.getFieldDescription() });
        this.Emitente = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.NotaFiscalEletronica.Emitente.getFieldDescription(), codEntity: ko.observable(0) });
        this.DataEmissao = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao.getFieldDescription() });
        this.Chave = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.NotaFiscalEletronica.Chave.getFieldDescription() });
        this.Carga = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), visible: false, text: Localization.Resources.Consultas.NotaFiscalEletronica.Carga.getFieldDescription(), idBtnSearch: guid() });
        this.CargaEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;
    if (knoutCarga != null || knoutCargaEntrega != null || knoutCliente != null) {
        knoutOpcoes.Carga.visible = false;
        funcaoParamentroDinamico = function () {
            if (knoutCarga != null) {
                knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
                knoutOpcoes.Carga.val(knoutCarga.val());
            }

            if (knoutCargaEntrega != null) {
                knoutOpcoes.CargaEntrega.codEntity(knoutCargaEntrega.codEntity());
                knoutOpcoes.CargaEntrega.val(knoutCargaEntrega.val());
            }

            if (knoutCliente != null) {
                knoutOpcoes.Cliente.codEntity(knoutCliente.codEntity());
                knoutOpcoes.Cliente.val(knoutCliente.val());
            }
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarClientes(knoutOpcoes.Emitente);
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

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", urlPesquisa, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", urlPesquisa, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {

            if (knout.val().length < 44)
                knoutOpcoes.Numero.val(knout.val());
            else
                knoutOpcoes.Chave.val(knout.val());
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

var BuscarNFesPermiteMDFeManual = function (knout, knoutEmpresa, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.BuscarNotasFiscais, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.NotaFiscalEletronica.NotasFiscais, type: types.local });

        this.NumeroInicial = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.NumeroInicial.getFieldDescription() });
        this.NumeroFinal = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.NumeroFinal.getFieldDescription() });
        this.Serie = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.NotaFiscalEletronica.Serie.getFieldDescription() });
        this.Chave = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.NotaFiscalEletronica.Chave.getFieldDescription() });
        this.Status = PropertyEntity({ text: "Situacao: ", col: 12, visible: false, val: ko.observable(4) });
        this.TipoEmissao = PropertyEntity({ text: "TipoEmissao: ", col: 12, visible: false, val: ko.observable(1) });


        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {

    }, null, true);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.OrigemDestino);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NotaFiscalEletronica/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NotaFiscalEletronica/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        GridConsulta.CarregarGrid(function (lista) {

            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}