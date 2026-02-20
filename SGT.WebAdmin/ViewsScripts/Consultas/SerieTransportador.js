/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Enumeradores/EnumTipoSerie.js" />

var BuscarSeriesTransportador = function (codigoTransportador, knout, tituloOpcional, tituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.SerieTransportador.ConsultaDeSeriesDoTransportador, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.SerieTransportador.Series, type: types.local });
        this.Numero = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.SerieTransportador.Numero, getType: typesKnockout.int });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);



    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            Global.fecharModal(idDiv);
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Transportador/PesquisarSeries?CodigoTransportador=" + codigoTransportador, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 3, dir: orderDir.asc });

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}


var BuscarSeriesMDFeTransportador = function (knout, callbackRetorno, tituloOpcional, tituloGridOpcional, knouFilial, knoutTransportador) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.SerieTransportador.ConsultaDeSeriesDeMDFe, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.Series, type: types.local });
        this.TipoSerie = PropertyEntity({ visible: false, getType: typesKnockout.int, val: ko.observable(EnumTipoSerie.MDFe), def: EnumTipoSerie.MDFe });
        this.Numero = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.SerieTransportador.Numero.getFieldDescription(), getType: typesKnockout.int });
        this.Filial = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Transportador = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knouFilial != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Filial.codEntity(knouFilial.codEntity());
            knoutOpcoes.Filial.val(knouFilial.val());
        }
    } else if (knoutTransportador != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Transportador.codEntity(knoutTransportador.codEntity());
            knoutOpcoes.Transportador.val(knoutTransportador.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            Global.fecharModal(idDiv);
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Transportador/PesquisarSeriesPorTipo", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
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

var BuscarSeriesCTeTransportador = function (knout, callbackRetorno, tituloOpcional, tituloGridOpcional, knouFilial, knoutTransportador) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.SerieTransportador.ConsultaDeSeriesDeCTe, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.SerieTransportador.Series, type: types.local });
        this.TipoSerie = PropertyEntity({ visible: false, getType: typesKnockout.int, val: ko.observable(EnumTipoSerie.CTe), def: EnumTipoSerie.CTe });
        this.Numero = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.SerieTransportador.Numero.getFieldDescription(), getType: typesKnockout.int });
        this.Filial = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Transportador = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knouFilial != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Filial.codEntity(knouFilial.codEntity());
            knoutOpcoes.Filial.val(knouFilial.val());
        }
    } else if (knoutTransportador != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Transportador.codEntity(knoutTransportador.codEntity());
            knoutOpcoes.Transportador.val(knoutTransportador.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            Global.fecharModal(idDiv);
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Transportador/PesquisarSeriesPorTipo", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
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



var BuscarSeriesNFSeTransportador = function (knout, callbackRetorno, tituloOpcional, tituloGridOpcional, knouFilial, knoutTransportador) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloOpcional != null ? tituloOpcional : Localization.Resources.Consultas.SerieTransportador.ConsultaDeSeriesDeNFSe, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGridOpcional != null ? tituloGridOpcional : Localization.Resources.Consultas.SerieTransportador.Series, type: types.local });
        this.TipoSerie = PropertyEntity({ visible: false, getType: typesKnockout.int, val: ko.observable(EnumTipoSerie.NFSe), def: EnumTipoSerie.NFSe });
        this.Numero = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.SerieTransportador.Numero.getFieldDescription(), getType: typesKnockout.int });
        this.Filial = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Transportador = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knouFilial != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Filial.codEntity(knouFilial.codEntity());
            knoutOpcoes.Filial.val(knouFilial.val());
        }
    } else if (knoutTransportador != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Transportador.codEntity(knoutTransportador.codEntity());
            knoutOpcoes.Transportador.val(knoutTransportador.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            Global.fecharModal(idDiv);
            callbackRetorno(e);
        }
    }


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Transportador/PesquisarSeriesPorTipo", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
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