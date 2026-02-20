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
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoModeloVeicularCarga.js" />
/// <reference path="GrupoPessoas.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloReboque;
var _modeloReboque;

var ModeloReboque = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.Modelo = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarModeloReboque, idBtnSearch: guid() });
}


//*******EVENTOS*******

function loadModeloReboque() {

    _modeloReboque = new ModeloReboque();
    KoBindings(_modeloReboque, "knockoutModeloReboque");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), metodo: function (data) {
                excluirModeloReboqueClick(_modeloReboque.Modelo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: Localization.Resources.Pessoas.GrupoPessoas.Descricao, width: "80%" }];

    _gridModeloReboque = new BasicDataTable(_modeloReboque.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_modeloReboque.Modelo, null, null, null, [EnumTipoModeloVeicularCarga.Geral, EnumTipoModeloVeicularCarga.Reboque], null, _gridModeloReboque);

    recarregarGridModeloReboque();
}

function recarregarGridModeloReboque() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_grupoPessoas.ModelosReboque.val())) {
        $.each(_grupoPessoas.ModelosReboque.val(), function (i, modeloReboque) {
            var modeloReboqueGrid = new Object();

            modeloReboqueGrid.Codigo = modeloReboque.Modelo.Codigo;
            modeloReboqueGrid.Descricao = modeloReboque.Modelo.Descricao;

            data.push(modeloReboqueGrid);
        });

        
    }
    _gridModeloReboque.CarregarGrid(data);
}


function excluirModeloReboqueClick(knoutReboques, data) {
    var reboqueGrid = _gridModeloReboque.BuscarRegistros();

    for (var i = 0; i < reboqueGrid.length; i++) {
        if (data.Codigo == reboqueGrid[i].Codigo) {
            reboqueGrid.splice(i, 1);
            break;
        }
    }
    _gridModeloReboque.CarregarGrid(reboqueGrid);
}

function limparCamposModeloReboque() {
    LimparCampos(_modeloReboque);
}