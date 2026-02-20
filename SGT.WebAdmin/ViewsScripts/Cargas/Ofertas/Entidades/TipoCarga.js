/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../ParametrosOfertas.js" />

var _gridTiposCarga;
var _tiposCarga;

var TiposCarga = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.TipoCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Carga", idBtnSearch: guid() });
};

function LoadTiposCarga() {
    _tiposCarga = new TiposCarga();
    KoBindings(_tiposCarga, "knockoutTiposCarga");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirTiposCargasClick(data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
    ];

    _gridTiposCarga = new BasicDataTable(_tiposCarga.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _tiposCarga.TipoCarga.basicTable = _gridTiposCarga;

    new BuscarTiposdeCarga(_tiposCarga.TipoCarga, null, null, _gridTiposCarga);

    RecarregarGridTiposCargas();
}

function RecarregarGridTiposCargas() {

    let data = new Array();

    if (_parametrosOfertas.TiposCarga.val() != "" && _parametrosOfertas.TiposCarga.val().length > 0) {
        $.each(_parametrosOfertas.TiposCarga.val(), function (i, tipoCarga) {
            let tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;
            tiposCargasGrid.CodigoRelacionamento = tipoCarga.CodigoRelacionamento;

            data.push(tiposCargasGrid);
        });
    }

    _gridTiposCarga.CarregarGrid(data);
}

function PreencherTiposCarga(listaTiposCargaRetornadas) {
    _parametrosOfertas.TiposCarga.val(listaTiposCargaRetornadas);
    RecarregarGridTiposCargas();
}

function ExcluirTiposCargasClick(data) {
    let tiposCargasGrid = _gridTiposCarga.BuscarRegistros();

    for (let i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridTiposCarga.CarregarGrid(tiposCargasGrid);
}

function LimparCamposTiposCargas() {
    LimparCampos(_tiposCarga);
    _gridTiposCarga.CarregarGrid(new Array());
}