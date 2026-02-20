/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarDeposito = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.BuscarDeposito, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.Depositos, type: types.local });
        this.DescricaoBusca = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
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
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Deposito/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.DescricaoBusca.val(knout.val());
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

var BuscarDepositoRua = function (knout, callbackRetorno, knoutDeposito) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.BuscarRua, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.Ruas, type: types.local });
        this.DescricaoBusca = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });

        this.Deposito = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutDeposito != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Deposito.codEntity(knoutDeposito.codEntity());
            knoutOpcoes.Deposito.val(knoutDeposito.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Rua/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.DescricaoBusca.val(knout.val());
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

var BuscarDepositoBloco = function (knout, callbackRetorno, knoutDeposito, knoutRua) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.BuscarBloco, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.Blocos, type: types.local });
        this.DescricaoBusca = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });

        this.Deposito = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Rua = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutDeposito != null && knoutRua != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Deposito.codEntity(knoutDeposito.codEntity());
            knoutOpcoes.Deposito.val(knoutDeposito.val());

            knoutOpcoes.Rua.codEntity(knoutRua.codEntity());
            knoutOpcoes.Rua.val(knoutRua.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Bloco/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.DescricaoBusca.val(knout.val());
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

var BuscarDepositoPosicao = function (knout, callbackRetorno, knoutDeposito, knoutRua, knoutBloco) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.BuscarPosicao, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Deposito.Posicoes, type: types.local });
        this.Abreviacao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Deposito.Abreviacao.getFieldDescription(), maxlength: 250 });
        this.DescricaoBusca = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 250 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });

        this.Deposito = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Deposito.DescricaoDeposito.getFieldDescription(), idBtnSearch: guid(), visible: true, required: false });
        this.Rua = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Deposito.Rua.getFieldDescription(), idBtnSearch: guid(), visible: true, required: false });
        this.Bloco = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Deposito.Bloco.getFieldDescription(), idBtnSearch: guid(), visible: true, required: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutDeposito != null && knoutRua != null && knoutBloco) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Deposito.codEntity(knoutDeposito.codEntity());
            knoutOpcoes.Deposito.val(knoutDeposito.val());

            knoutOpcoes.Rua.codEntity(knoutRua.codEntity());
            knoutOpcoes.Rua.val(knoutRua.val());

            knoutOpcoes.Bloco.codEntity(knoutBloco.codEntity());
            knoutOpcoes.Bloco.val(knoutBloco.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarDeposito(knoutOpcoes.Deposito, null);
        new BuscarDepositoRua(knoutOpcoes.Rua, null);
        new BuscarDepositoBloco(knoutOpcoes.Bloco, null);
    });


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Posicao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Abreviacao.val(knout.val());
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