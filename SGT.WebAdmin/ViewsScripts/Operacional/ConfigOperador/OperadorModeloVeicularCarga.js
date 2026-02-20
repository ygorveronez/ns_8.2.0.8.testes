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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" /
/// <reference path="../../Consultas/ModeloVeicular.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="OperadorTipoOperacao.js" />
/// <reference path="OperadorFilial.js" />
/// <reference path="ConfigOperador.js" />
/// <reference path="OperadorModeloVeicular.js" />
/// <reference path="TabelaFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var OperadorModeloVeicular = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.ModeloVeicularCarga = PropertyEntity({ idGrid: guid(), type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Operacional.ConfigOperador.ModeloVeicularCarga.getRequiredFieldDescription(), required: true, idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Adicionar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadOperadorModeloVeicular(idDivOperadorTipoCarga, operadorTipoCarga) {

    var knoutOperadorModeloVeicular = new OperadorModeloVeicular();
    var gridOperadorModeloVeicular;

    knoutOperadorModeloVeicular.Adicionar.eventClick = function (e) {
        adicionarModeloVeicularClick(operadorTipoCarga, knoutOperadorModeloVeicular, gridOperadorModeloVeicular);
    };

    KoBindings(knoutOperadorModeloVeicular, idDivOperadorTipoCarga);

    new BuscarModelosVeicularesCarga(knoutOperadorModeloVeicular.ModeloVeicularCarga, null, operadorTipoCarga.TipoCarga.codEntity);

    var excluir = {
        descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), evento: "onclick", metodo: function (data)
        { exlcuirModeloVeicularClick(knoutOperadorModeloVeicular, operadorTipoCarga, data, gridOperadorModeloVeicular) },
        tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);
    var header = [{ data: "Codigo", visible: false },
        { data: "ModeloVeicularCarga", title: Localization.Resources.Operacional.ConfigOperador.ModeloVeicularCarga, width: "85%" }
    ];
    gridOperadorModeloVeicular = new BasicDataTable(knoutOperadorModeloVeicular.ModeloVeicularCarga.idGrid, header, menuOpcoes);
    recarregarGridModelosVeiculares(gridOperadorModeloVeicular, operadorTipoCarga);

}

function adicionarModeloVeicularClick(operadorTipoCarga, knoutOperadorModeloVeicular, gridOperadorModeloVeicular) {
    var tudoCerto = ValidarCamposObrigatorios(knoutOperadorModeloVeicular);
    if (tudoCerto) {
        var existe = false;
        $.each(operadorTipoCarga.OperadorTipoCargaModelosVeicular.list, function (i, modeloVeicular) {
            if (modeloVeicular.ModeloVeicularCarga.codEntity == knoutOperadorModeloVeicular.ModeloVeicularCarga.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            operadorTipoCarga.OperadorTipoCargaModelosVeicular.list.push(SalvarListEntity(knoutOperadorModeloVeicular));
            recarregarGridModelosVeiculares(gridOperadorModeloVeicular, operadorTipoCarga);
            $("#" + knoutOperadorModeloVeicular.ModeloVeicularCarga.id).focus();
        } else {
            exibirMensagem("aviso", Localization.Resources.Operacional.ConfigOperador.ModeloVeicularJaInformado, Localization.Resources.Operacional.ConfigOperador.ModeloVeicularJaInformadoParaCarga);
        }
        LimparCamposModeloVeicularCarga(knoutOperadorModeloVeicular);
    } else {
        exibirMensagem("atencao", Localization.Resources.Operacional.ConfigOperador.CamposObrigatorios, Localization.Resources.Operacional.ConfigOperador.InformeCamposObrigatorios);
    }
}

function exlcuirModeloVeicularClick(knoutOperadorModeloVeicular, operadorTipoCarga, linha, gridOperadorModeloVeicular) {
    exibirConfirmacao(Localization.Resources.Operacional.ConfigOperador.Confirmacao, Localization.Resources.Operacional.ConfigOperador.RealmenteDesejaExcluirModeloVeicularCargaX.format(linha.ModeloVeicularCarga), function () {
        $.each(operadorTipoCarga.OperadorTipoCargaModelosVeicular.list, function (i, operadorModeloVeicular) {
            if (operadorModeloVeicular.ModeloVeicularCarga.codEntity == linha.Codigo) {
                operadorTipoCarga.OperadorTipoCargaModelosVeicular.list.splice(i, 1);
                return false;
            }
        });
        recarregarGridModelosVeiculares(gridOperadorModeloVeicular, operadorTipoCarga);
        LimparCamposModeloVeicularCarga(knoutOperadorModeloVeicular);
    });
}

//*******MÉTODOS*******

function recarregarGridModelosVeiculares(grid, operadorTipoCarga) {
    var data = new Array();
    $.each(operadorTipoCarga.OperadorTipoCargaModelosVeicular.list, function (i, modelo) {
        var modeloVeicularGrid = new Object();
        modeloVeicularGrid.Codigo = modelo.ModeloVeicularCarga.codEntity;
        modeloVeicularGrid.ModeloVeicularCarga = modelo.ModeloVeicularCarga.val;
        data.push(modeloVeicularGrid);
    });
    grid.CarregarGrid(data);
}

function LimparCamposModeloVeicularCarga(knoutOperadorModeloVeicular) {
    LimparCampos(knoutOperadorModeloVeicular);
}
