/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Carga.js" />
/// <reference path="Tranportador.js" />
/// <reference path="../Configuracao/Sistema/ConfiguracaoTMS.js" />

var BuscarCargaCTe = function (knout, callbackRetorno, basicGrid, knoutEmpresa, somenteCTesAquaviarios) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    if (somenteCTesAquaviarios == null)
        somenteCTesAquaviarios = false;
    else
        somenteCTesAquaviarios = true;

    var OpcoesKnout = function () {
        this.Carga = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
        this.RotasFrete = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Rotas de Frete:", idBtnSearch: guid() });
        this.CTe = PropertyEntity({ text: "Nº CT-e: ", col: 3 });
        this.NumeroNF = PropertyEntity({ text: "Nº NF-e: ", col: 3 });
        this.Empresa = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: false });
        this.SomenteCTesAquaviarios = PropertyEntity({ col: 0, visible: false, val: ko.observable(somenteCTesAquaviarios) });
        this.Titulo = PropertyEntity({ text: "Buscar CT-es", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "CT-es", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && knoutEmpresa == null)
        knoutOpcoes.Empresa.visible = true;

    var funcaoParamentroDinamico = null;
    if (knoutEmpresa != null) {
        knoutOpcoes.Empresa.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Empresa.codEntity(knoutEmpresa.codEntity());
            knoutOpcoes.Empresa.val(knoutEmpresa.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        BuscarCargas(knoutOpcoes.Carga);
        BuscarTransportadores(knoutOpcoes.Empresa);
        BuscarRotasFrete(knoutOpcoes.RotasFrete);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaCTe/ConsultarCTesParaEmissaoMDFe", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaCTe/ConsultarCTesParaEmissaoMDFe", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

var BuscarCargaCTeMultiCTe = function (knout, callbackRetorno, basicGrid, knoutEmpresa) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Carga = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
        this.RotasFrete = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Rotas de Frete:", idBtnSearch: guid() });
        this.CTe = PropertyEntity({ text: "Nº CT-e: ", col: 3 });
        this.NumeroNF = PropertyEntity({ text: "Nº NF-e: ", col: 3 });
        this.Empresa = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: false });
        this.Titulo = PropertyEntity({ text: "Buscar CT-es", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "CT-es", type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        knoutOpcoes.Empresa.visible = true;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        BuscarCargas(knoutOpcoes.Carga);
        BuscarTransportadores(knoutOpcoes.Empresa);
        BuscarRotasFrete(knoutOpcoes.RotasFrete);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaCTe/ConsultarCTesParaEmissaoMDFeMultiCTe", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaCTe/ConsultarCTesParaEmissaoMDFeMultiCTe", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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