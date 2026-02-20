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
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloTracao;
var _modeloTracao;

var ModeloTracao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Modelo = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarModeloDeTracao, idBtnSearch: guid(), issue: 81 });
}


//*******EVENTOS*******

function loadModeloTracao() {

    _modeloTracao = new ModeloTracao();
    KoBindings(_modeloTracao, "knockoutModeloTracao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                excluirModeloTracaoClick(_modeloTracao.Modelo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }];

    _gridModeloTracao = new BasicDataTable(_modeloTracao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_modeloTracao.Modelo, null, null, null, [EnumTipoModeloVeicularCarga.Geral, EnumTipoModeloVeicularCarga.Tracao], null, _gridModeloTracao);
    _modeloTracao.Modelo.basicTable = _gridModeloTracao;

    recarregarGridModeloTracao();
}

function recarregarGridModeloTracao() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.ModelosTracao.val())) {

        $.each(_tabelaFrete.ModelosTracao.val(), function (i, modeloTracao) {
            var modeloTracaoGrid = new Object();

            modeloTracaoGrid.Codigo = modeloTracao.Modelo.Codigo;
            modeloTracaoGrid.Descricao = modeloTracao.Modelo.Descricao;

            data.push(modeloTracaoGrid);
        });
    }

    _gridModeloTracao.CarregarGrid(data);
}


function excluirModeloTracaoClick(knoutTracoes, data) {
    var tracaoGrid = knoutTracoes.basicTable.BuscarRegistros();

    for (var i = 0; i < tracaoGrid.length; i++) {
        if (data.Codigo == tracaoGrid[i].Codigo) {
            tracaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTracoes.basicTable.CarregarGrid(tracaoGrid);
}

function limparCamposModeloTracao() {
    LimparCampos(_modeloTracao);
}