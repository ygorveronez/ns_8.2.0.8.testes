/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="TabelaFreteRota.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _HTMLTabelaRotaTipoCarga;

var TipoCarga = function () {
    this.ModeloVeicularCarga = PropertyEntity({ idGrid: guid(), type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular de Carga:", required: true, idBtnSearch: guid() });
    this.ValorPedagio = PropertyEntity({ text: "*Valor Pedágio:", required: true, type: types.map, val: ko.observable(""), def: "", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true } });
    this.ValorFrete = PropertyEntity({ text: "*Valor do Frete:", required: true, type: types.map, val: ko.observable(""), def: "", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular de Carga", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", visible: ko.observable(false) });

}

var TipoCargaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Ativo = PropertyEntity({ type: types.map, val: true, def: true, getType: typesKnockout.bool });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.listEntity, list: new Array() });
}

var ModeloVeicularCargaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.local, val: "" });
    this.ValorPedagio = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.decimal });
    this.ValorFrete = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.decimal });
}



//*******EVENTOS*******

function loadTabelaFreteRotaTipoCarga() {
    new BuscarTiposdeCarga(_tabelaFreteRota.TiposCarga);
    $.get("Content/Static/Frete/TabelaFreteRotaTipoCarga.html?dyn=" + guid(), function (data) {
        _HTMLTabelaRotaTipoCarga = data;
    })
}

function adicionarTipoCargaClick(e) {
    var tudoCerto = ValidarCampoObrigatorioEntity(_tabelaFreteRota.TiposCarga);
    if (tudoCerto) {
        var existe = false;
        $.each(_tabelaFreteRota.TiposCarga.list, function (i, tipoCarga) {
            if (tipoCarga.Codigo.val == _tabelaFreteRota.TiposCarga.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            var tipoCarga = new TipoCargaMap();
            tipoCarga.Codigo.val = parseInt(_tabelaFreteRota.TiposCarga.codEntity());
            tipoCarga.Descricao.val = _tabelaFreteRota.TiposCarga.val();

            _tabelaFreteRota.TiposCarga.list.push(tipoCarga);
            criarHTMLTipoCarga(tipoCarga);
            $("#" + _tabelaFreteRota.TiposCarga.id).focus();
        } else {
            exibirMensagem("aviso", "Tipo de Carga já informado", "O tipo de carga já foi informado para esta rota");
        }
        LimparCampoEntity(_tabelaFreteRota.TiposCarga);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}

function adicionarModeloVeicularClick(tipoCargaMap, knoutTipoCarga, gridModelosVeiculares) {
    var tudoCerto = ValidarCamposObrigatorios(knoutTipoCarga);
    if (tudoCerto) {
        var existe = false;
        $.each(tipoCargaMap.ModeloVeicularCarga.list, function (i, modeloVeicular) {
            if (modeloVeicular.Codigo.val == knoutTipoCarga.ModeloVeicularCarga.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            var modeloVeicular = new ModeloVeicularCargaMap();
            modeloVeicular.Codigo.val = parseInt(knoutTipoCarga.ModeloVeicularCarga.codEntity());
            modeloVeicular.Descricao.val = knoutTipoCarga.ModeloVeicularCarga.val();
            modeloVeicular.ValorFrete.val = knoutTipoCarga.ValorFrete.val();
            modeloVeicular.ValorPedagio.val = knoutTipoCarga.ValorPedagio.val();
            tipoCargaMap.ModeloVeicularCarga.list.push(modeloVeicular);
            recarregarGridModelosVeiculares(gridModelosVeiculares, tipoCargaMap);
            $("#" + knoutTipoCarga.ModeloVeicularCarga.id).focus();
        } else {
            exibirMensagem("aviso", "Modelo veicular já informado", "O modelo veicular já foi informado para esta tipo de carga");
        }
        LimparCamposModeloVeicularCarga(knoutTipoCarga);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}


function atualizarModeloVeicularClick(tipoCargaMap, knoutTipoCarga, gridModelosVeiculares) {
    var tudoCerto = ValidarCamposObrigatorios(knoutTipoCarga);
    if (tudoCerto) {
        $.each(tipoCargaMap.ModeloVeicularCarga.list, function (i, modeloVeicular) {
            if (modeloVeicular.Codigo.val == knoutTipoCarga.ModeloVeicularCarga.codEntity()) {
                modeloVeicular.ValorFrete.val = knoutTipoCarga.ValorFrete.val();
                modeloVeicular.ValorPedagio.val = knoutTipoCarga.ValorPedagio.val();
                return false;
            }
        });
        recarregarGridModelosVeiculares(gridModelosVeiculares, tipoCargaMap);
        LimparCamposModeloVeicularCarga(knoutTipoCarga);
    } else {
        exibirMensagem("atencao", "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}

function exlcuirModeloVeicularClick(tipoCargaMap, knoutTipoCarga, gridModelosVeiculares) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o modelo veicular de carga " + knoutTipoCarga.ModeloVeicularCarga.val() + "?", function () {
        var listaAtualizada = new Array();
        $.each(tipoCargaMap.ModeloVeicularCarga.list, function (i, modeloVeicular) {
            if (modeloVeicular.Codigo.val != knoutTipoCarga.ModeloVeicularCarga.codEntity()) {
                listaAtualizada.push(modeloVeicular);
            }
        });
        tipoCargaMap.ModeloVeicularCarga.list = listaAtualizada;
        recarregarGridModelosVeiculares(gridModelosVeiculares, tipoCargaMap);
        LimparCamposModeloVeicularCarga(knoutTipoCarga);
    });
}

//*******MÉTODOS*******

function recarregarTiposCarga(listaTipoCarga) {
    $.each(listaTipoCarga, function (i, tipoCargaMap) {
        criarHTMLTipoCarga(tipoCargaMap);
    });
}

function recarregarGridModelosVeiculares(grid, tipoCargaMap) {
    var data = new Array();
    $.each(tipoCargaMap.ModeloVeicularCarga.list, function (i, modelo) {
        var modeloVeicularGrid = new Object();
        modeloVeicularGrid.Codigo = modelo.Codigo.val;
        modeloVeicularGrid.ModeloVeicularCarga = modelo.Descricao.val;
        modeloVeicularGrid.ValorFrete = modelo.ValorFrete.val;
        modeloVeicularGrid.ValorPedagio = modelo.ValorPedagio.val;
        data.push(modeloVeicularGrid);
    });
    grid.CarregarGrid(data);
}

function ativarDesativarTipoCarga(idDivTabelaFreteTipoCarga, idCheckAtivoInativo, tipoCargaMap) {
    if ($('#' + idCheckAtivoInativo).prop("checked")) {
        $('#' + idCheckAtivoInativo).prop("checked", true);

        $("#" + idCheckAtivoInativo).parent().parent().parent().find(".card-body").show();
        $("#" + idCheckAtivoInativo).parent().parent().parent().find(".card-footer").show();

        //$('#' + idCheckAtivoInativo).parent().attr("class", "toggle");
        //$('#' + idCheckAtivoInativo + "_tit").removeAttr("class");
        tipoCargaMap.Ativo.val = true;
    } else {
        $('#' + idCheckAtivoInativo).prop("checked", false);
        //$('#' + idCheckAtivoInativo).parent().attr("class", "toggle state-disabled");
        //$('#' + idCheckAtivoInativo + "_tit").attr("class", "disableText");

        $("#" + idCheckAtivoInativo).parent().parent().parent().find(".card-body").hide();
        $("#" + idCheckAtivoInativo).parent().parent().parent().find(".card-footer").hide();

        tipoCargaMap.Ativo.val = false;
    }
}

function criarHTMLTipoCarga(tipoCargaMap) {
    var idDivTabelaFreteTipoCarga = guid();
    var idCheckAtivoInativo = guid();

    $("#DivTiposDeCarga").prepend(
        _HTMLTabelaRotaTipoCarga.replace(/#idDivTabelaFreteTipoCarga/g, idDivTabelaFreteTipoCarga)
            .replace(/#DescricaoTipoCarga/g, tipoCargaMap.Descricao.val)
            .replace(/#ckbAtivoInativo/g, idCheckAtivoInativo)
            .replace(/#idTitulo/g, idCheckAtivoInativo + "_tit")
    );

    const checkAtivoInativo = document.getElementById(idCheckAtivoInativo);

    checkAtivoInativo.checked = tipoCargaMap.Ativo.val;

    //if (tipoCargaMap.Ativo.val) {
    //    checkAtivoInativo.checked = true;
    //    //$('#' + idCheckAtivoInativo).prop("checked", true);
    //    //$('#' + idCheckAtivoInativo).parent().attr("class", "toggle");
    //    //$('#' + idCheckAtivoInativo + "_tit").removeAttr("class");
    //} else {
    //    checkAtivoInativo.checked = false;
    //    //$('#' + idCheckAtivoInativo).prop("checked", false);
    //    //$('#' + idCheckAtivoInativo).parent().attr("class", "toggle state-disabled");
    //    //$('#' + idCheckAtivoInativo + "_tit").attr("class", "disableText");
    //}

    checkAtivoInativo.addEventListener('change', (event) => { ativarDesativarTipoCarga(idDivTabelaFreteTipoCarga, idCheckAtivoInativo, tipoCargaMap); });

    //$('#' + idCheckAtivoInativo).on("change", function () { ativarDesativarTipoCarga(idDivTabelaFreteTipoCarga, idCheckAtivoInativo, tipoCargaMap) });

    var knoutTipoCarga = new TipoCarga();
    var gridModelosVeiculares;
    knoutTipoCarga.Adicionar.eventClick = function (e) {
        adicionarModeloVeicularClick(tipoCargaMap, knoutTipoCarga, gridModelosVeiculares);
    };

    knoutTipoCarga.Atualizar.eventClick = function (e) {
        atualizarModeloVeicularClick(tipoCargaMap, knoutTipoCarga, gridModelosVeiculares);
    };

    knoutTipoCarga.Excluir.eventClick = function (e) {
        exlcuirModeloVeicularClick(tipoCargaMap, knoutTipoCarga, gridModelosVeiculares);
    };

    knoutTipoCarga.Cancelar.eventClick = function (e) {
        LimparCamposModeloVeicularCarga(knoutTipoCarga);
    }

    KoBindings(knoutTipoCarga, idDivTabelaFreteTipoCarga);
    new BuscarModelosVeicularesCarga(knoutTipoCarga.ModeloVeicularCarga, null, tipoCargaMap.Codigo.val);
    //if (tipoCargaMap.ModeloVeicularCarga.list.length == 0) {
    //    $("#" + idDivTabelaFreteTipoCarga).attr("class", "panel-collapse collapse in");
    //}
    $("#divTiposCargaParent").show();

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { editarModeloVeicularCarga(knoutTipoCarga, data, gridModelosVeiculares) }, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [{ data: "Codigo", visible: false },
    { data: "ModeloVeicularCarga", title: "Modelo Veicular de Carga", width: "60%" },
    { data: "ValorFrete", title: "Valor do Frete", width: "15%", className: "text-align-center" },
    { data: "ValorPedagio", title: "Pedágio", width: "10%", className: "text-align-center" }
    ];
    gridModelosVeiculares = new BasicDataTable(knoutTipoCarga.ModeloVeicularCarga.idGrid, header, menuOpcoes);
    recarregarGridModelosVeiculares(gridModelosVeiculares, tipoCargaMap);
}


function editarModeloVeicularCarga(knoutTipoCarga, data) {
    LimparCampos(knoutTipoCarga);
    knoutTipoCarga.ModeloVeicularCarga.codEntity(data.Codigo);
    knoutTipoCarga.ModeloVeicularCarga.val(data.ModeloVeicularCarga);
    knoutTipoCarga.ValorFrete.val(data.ValorFrete);
    knoutTipoCarga.ValorPedagio.val(data.ValorPedagio);
    knoutTipoCarga.Adicionar.visible(false);
    knoutTipoCarga.Atualizar.visible(true);
    knoutTipoCarga.Excluir.visible(true);
    knoutTipoCarga.Cancelar.visible(true);
}

function LimparCamposModeloVeicularCarga(knoutTipoCarga) {
    LimparCampos(knoutTipoCarga);
    knoutTipoCarga.Adicionar.visible(true);
    knoutTipoCarga.Atualizar.visible(false);
    knoutTipoCarga.Excluir.visible(false);
    knoutTipoCarga.Cancelar.visible(false);
}

function LimparCamposTipoCarga() {
    $("#DivTiposDeCarga").html("");
    $("#divTiposCargaParent").hide();
}