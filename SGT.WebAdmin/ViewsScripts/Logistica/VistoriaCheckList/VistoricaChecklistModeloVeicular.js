/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="VistoriaCheckList.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _modeloVeicularCheckList, _gridModeloVeicularCheckList;

var VistoriaCheckListModeloVeiculars = function () {
    this.ModeloVeicular = PropertyEntity({ type: types.local });
    this.AdicionarModeloVeicular = PropertyEntity({ type: types.event, text: "Adicionar", idBtnSearch: guid(), enable: ko.observable(true), val: ko.observable([]) });
}

//*******EVENTOS*******

function loadVistoriaCheckListModeloVeicular() {
    _modeloVeicularCheckList = new VistoriaCheckListModeloVeiculars();
    KoBindings(_modeloVeicularCheckList, "knockoutCadastroVistoriaCheckListModeloVeicular");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirModeloVeicularClick(_modeloVeicularCheckList.AdicionarModeloVeicular, data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "30%" },

    ];
    _gridModeloVeicularCheckList = new BasicDataTable(_modeloVeicularCheckList.ModeloVeicular.id, header, menuOpcoes);
    _modeloVeicularCheckList.AdicionarModeloVeicular.basicTable = _gridModeloVeicularCheckList;

    new BuscarModelosVeicularesCarga(_modeloVeicularCheckList.AdicionarModeloVeicular, retornBuscaModeloVeiculars, null, null, null, null, _gridModeloVeicularCheckList );

    RecarregarGridVistoriaCheckListModeloVeiculars();
}

//*******MÉTODOS*******

function RecarregarGridVistoriaCheckListModeloVeiculars() {
    var data = new Array();

    $.each(_modeloVeicularCheckList.AdicionarModeloVeicular.val(), function (i, modeloVeicular) {
        var ModeloVeicularGrid = new Object();
        
        ModeloVeicularGrid.Codigo = modeloVeicular.Codigo;
        ModeloVeicularGrid.Descricao = modeloVeicular.Descricao;

        data.push(ModeloVeicularGrid);
    });

    _gridModeloVeicularCheckList.CarregarGrid(data);
}

function RecarregarGridVistoriaChecklistModeloVeicular() {
    var data = _VistoriaCheckList.ModeloVeicular.val()
    _gridModeloVeicularCheckList.CarregarGrid(data);
    }


function ExcluirModeloVeicularClick(knoutModeloVeicular, data) {
    var ModeloVeicularGrid = knoutModeloVeicular.basicTable.BuscarRegistros();

    for (var i = 0; i < ModeloVeicularGrid.length; i++) {
        if (data.Codigo == ModeloVeicularGrid[i].Codigo) {
            ModeloVeicularGrid.splice(i, 1);
            break;
        }
    }

    knoutModeloVeicular.basicTable.CarregarGrid(ModeloVeicularGrid);
}

function preencherListasSelecaoVistoriaCheckListModeloVeiculars() {

    var ListaModeloGrid = _modeloVeicularCheckList.AdicionarModeloVeicular.basicTable.BuscarRegistros();
    var codigosModelos = ListaModeloGrid.map(modelo => modelo.Codigo);
    _VistoriaCheckList.ModeloVeicular.val(JSON.stringify(codigosModelos));
    
}

function limparCamposContratoFinanciamentoVeiculo() {
    LimparCampos(_modeloVeicularCheckList);
    RecarregarGridContratoFinanciamentoVeiculo();
}

function retornBuscaModeloVeiculars(ModeloVeicularsSelecionadas) {
    if (ModeloVeicularsSelecionadas.length == 0)
        return;

    var ModeloVeicularsAtuais = _gridModeloVeicularCheckList.BuscarRegistros();
    for (var i = 0; i < ModeloVeicularsSelecionadas.length; i++)
        ModeloVeicularsAtuais.push({
            Codigo: ModeloVeicularsSelecionadas[i].Codigo,
            Descricao: ModeloVeicularsSelecionadas[i].Descricao
        });

    _gridModeloVeicularCheckList.CarregarGrid(ModeloVeicularsAtuais);
}