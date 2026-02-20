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
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/MarcaVeiculo.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoVeiculo.js" />
/// <reference path="../../Cargas/Carga/DadosCarga/Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _painelVeiculoVeiculosVinculado;
var _gridReboques;

var PainelVeiculoVeiculosVinculados = function () {
    this.Reboques = PropertyEntity({ type: types.map, required: false, text: "Informar Reboques", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
}

//*******EVENTOS*******
function loadPainelVeiculoVeiculosVinculados() {
    _painelVeiculoVeiculosVinculado = new PainelVeiculoVeiculosVinculados();
    KoBindings(_painelVeiculoVeiculosVinculado, "knoutPainelVeiculoVeiculosVinculados", false);

    const excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverReboqueClick(_painelVeiculoVeiculosVinculado.Reboques, data)
        }, tamanho: "15", icone: ""
    };

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    const header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Placa", width: "80%", className: "text-align-left" }
    ];

    _gridReboques = new BasicDataTable(_painelVeiculoVeiculosVinculado.Reboques.idGrid, header, menuOpcoes);
    _painelVeiculoVeiculosVinculado.Reboques.basicTable = _gridReboques;

    BuscarReboques(_painelVeiculoVeiculosVinculado.Reboques, RetornoInserirReboque, _gridReboques);
    RecarregarListaReboques();
}

function RetornoInserirReboque(data) {
    if (data !== null) {
        let dataGrid = _gridReboques.BuscarRegistros();

        for (let i = 0; i < data.length; i++) {
            let obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.Placa = data[i].Placa;

            dataGrid.push(obj);
        }
        _gridReboques.CarregarGrid(dataGrid);
    }
}

function RecarregarListaReboques() {
    let data = new Array();
    if (!string.IsNullOrWhiteSpace(_indicacaoVeiculo.ListaReboques.val())) {

        $.each(_indicacaoVeiculo.ListaReboques.val(), function (i, reboque) {
            let obj = new Object();

            obj.Codigo = reboque.Codigo;
            obj.Placa = reboque.Placa;

            data.push(obj);
        });
    }
    _gridReboques.CarregarGrid(data);
}

function preencherListaReboques() {
    _indicacaoVeiculo.ListaReboques.list = new Array();

    let reboques = new Array();

    $.each(_painelVeiculoVeiculosVinculado.Reboques.basicTable.BuscarRegistros(), function (i, reboque) {
        reboques.push({ Reboque: reboque });
    });

    _indicacaoVeiculo.ListaReboques.val(JSON.stringify(reboques));
}

function RemoverReboqueClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o reboque " + sender.Placa + "?", function () {
        let reboqueGrid = e.basicTable.BuscarRegistros();

        for (let i = 0; i < reboqueGrid.length; i++) {
            if (sender.Codigo === reboqueGrid[i].Codigo) {
                reboqueGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(reboqueGrid);
    });
}
