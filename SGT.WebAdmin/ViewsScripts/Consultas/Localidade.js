/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Enumeradores/EnumEstado.js" />

var BuscarLocalidades = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, basicGrid, afterDefaultCallback, knockoutEstado) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.Localidade.PesquisaLocalidades, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.Localidade.Localidades, type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Localidade.Cidade.getFieldDescription() });
        this.CodigoIBGE = PropertyEntity({ col: 3, getType: typesKnockout.string, maxlength: 7, text: Localization.Resources.Consultas.Localidade.CodigoIBGE.getFieldDescription() });
        this.Estado = PropertyEntity({ col: 3, val: ko.observable(EnumEstado.Todos), def: EnumEstado.Todos, options: EnumEstado.obterOpcoesPesquisaSemExterior(), text: Localization.Resources.Consultas.Localidade.UF.getFieldDescription() });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    $("#" + knoutOpcoes.CodigoIBGE.id).mask("0000000", { selectOnFocus: true, clearIfNotMatch: true });

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knockoutEstado != null) {
        funcaoParamentroDinamico = function () {
            if (knockoutEstado.codEntity() == "")
                return;

            knoutOpcoes.Estado.val(knockoutEstado.codEntity());
        }
    }

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    } else if (afterDefaultCallback != null) {
        callback = function (e) {
            divBusca.DefCallback(e);
            afterDefaultCallback(e);
        }
    }

    var url = "Localidade/Pesquisa";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, afterDefaultCallback: afterDefaultCallback, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
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
                Global.setarFocoProximoCampo(knout.id);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.Destroy = function () {
        divBusca.Destroy();
    };
}


var BuscarLocalidadesPorEstados = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, basicGrid, afterDefaultCallback, knockoutEstado) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.Localidade.PesquisaLocalidades, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.Localidade.Localidades, type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Localidade.Cidade.getFieldDescription() });
        this.CodigoIBGE = PropertyEntity({ col: 3, getType: typesKnockout.string, maxlength: 7, text: Localization.Resources.Consultas.Localidade.CodigoIBGE.getFieldDescription() });
        this.Estado = PropertyEntity({ col: 3, visible: false ,val: ko.observable(EnumEstado.Todos), def: EnumEstado.Todos, options: EnumEstado.obterOpcoesPesquisaSemExterior(), text: Localization.Resources.Consultas.Localidade.UF.getFieldDescription() });
        this.Estados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knockoutEstado != null) {
        funcaoParamentroDinamico = function () {
            const codigosEstados = knockoutEstado.val().map(x => x.Codigo)
            knoutOpcoes.Estados.val(JSON.stringify(codigosEstados))
        }
    }
    
    $("#" + knoutOpcoes.CodigoIBGE.id).mask("0000000", { selectOnFocus: true, clearIfNotMatch: true });

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    } else if (afterDefaultCallback != null) {
        callback = function (e) {
            divBusca.DefCallback(e);
            afterDefaultCallback(e);
        }
    }

    var url = "Localidade/Pesquisa";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, afterDefaultCallback: afterDefaultCallback, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc }, null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
                Global.setarFocoProximoCampo(knout.id);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.Destroy = function () {
        divBusca.Destroy();
    }
}

var BuscarLocalidadesPolo = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.Localidade.PesquisaLocalidadesPolo, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.Localidade.LocalidadesPolo, type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Localidade.Cidade.getFieldDescription() });
        this.CodigoIBGE = PropertyEntity({ col: 3, getType: typesKnockout.string, maxlength: 7, text: Localization.Resources.Consultas.Localidade.CodigoIBGE.getFieldDescription() });
        this.Estado = PropertyEntity({ col: 3, val: ko.observable(EnumEstado.Todos), def: EnumEstado.Todos, options: EnumEstado.obterOpcoesPesquisaSemExterior(), text: Localization.Resources.Consultas.Localidade.UF.getFieldDescription() });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    $("#" + knoutOpcoes.CodigoIBGE.id).mask("0000000", { selectOnFocus: true, clearIfNotMatch: true });

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Localidade/PesquisaPolos", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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

var BuscarLocalidadesBrasil = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.Localidade.PesquisaLocalidades, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.Localidade.Localidades, type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Localidade.Cidade.getFieldDescription() });
        this.CodigoIBGE = PropertyEntity({ col: 3, getType: typesKnockout.string, maxlength: 7, text: Localization.Resources.Consultas.Localidade.CodigoIBGE.getFieldDescription() });
        this.Estado = PropertyEntity({ col: 3, val: ko.observable(EnumEstado.Todos), def: EnumEstado.Todos, options: EnumEstado.obterOpcoesPesquisaSemExterior(), text: Localization.Resources.Consultas.Localidade.UF.getFieldDescription() });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    $("#" + knoutOpcoes.CodigoIBGE.id).mask("0000000", { selectOnFocus: true, clearIfNotMatch: true });

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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Localidade/PesquisaBrasil", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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