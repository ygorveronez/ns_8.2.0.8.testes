/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloVeicularCarga;
var _modeloVeicularCarga;
var _modelosVeicularesCarga = new Array();

var ModeloVeicularCarga = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadModeloVeicularCarga() {

    _modeloVeicularCarga = new ModeloVeicularCarga();
    KoBindings(_modeloVeicularCarga, "knockoutModeloVeicular");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirModeloVeicularCargaClick(_modeloVeicularCarga.Tipo, data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridModeloVeicularCarga = new BasicDataTable(_modeloVeicularCarga.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarModelosVeicularesCarga(_modeloVeicularCarga.Tipo, function (r) {
        if (r != null) {
            for (let i = 0; i < r.length; i++)
                _modelosVeicularesCarga.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

            _gridModeloVeicularCarga.CarregarGrid(_modelosVeicularesCarga);

            RecarregarOpcoesModeloVeicularCarga();
        }
    }, null, null, null, null, _gridModeloVeicularCarga);
    _modeloVeicularCarga.Tipo.basicTable = _gridModeloVeicularCarga;

    RecarregarGridModeloVeicularCarga();
}

function RecarregarGridModeloVeicularCarga() {
    _gridModeloVeicularCarga.CarregarGrid(_configuracaoToleranciaPesagem.ModeloVeicularCarga.val());
    _modelosVeicularesCarga = _configuracaoToleranciaPesagem.ModeloVeicularCarga.val();
}

function ExcluirModeloVeicularCargaClick(knoutModeloVeicularCarga, data) {
    let modelosVeicularesCarga = knoutModeloVeicularCarga.basicTable.BuscarRegistros();

    for (let i = 0; i < modelosVeicularesCarga.length; i++) {
        if (data.Codigo == modelosVeicularesCarga[i].Codigo) {
            modelosVeicularesCarga.splice(i, 1);
            break;
        }
    }

    knoutModeloVeicularCarga.basicTable.CarregarGrid(modelosVeicularesCarga);

    RecarregarOpcoesModeloVeicularCarga();
}

function LimparCamposModeloVeicularCarga() {
    LimparCampos(_modeloVeicularCarga);
    _gridModeloVeicularCarga.CarregarGrid(new Array());
    _modelosVeicularesCarga = new Array();
}

function RecarregarOpcoesModeloVeicularCarga() {
    let modeloVeicularCargaGrid = _modeloVeicularCarga.Tipo.basicTable.BuscarRegistros();

    _opcoesModeloVeicularCarga = new Array();

    for (let i = 0; i < modeloVeicularCargaGrid.length; i++)
        _opcoesModeloVeicularCarga.push({ value: modeloVeicularCargaGrid[i].Codigo, text: modeloVeicularCargaGrid[i].Descricao });
}