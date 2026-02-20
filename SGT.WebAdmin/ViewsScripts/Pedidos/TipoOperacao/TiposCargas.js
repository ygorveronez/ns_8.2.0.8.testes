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
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTiposCargas;
var _tiposCargas;

var TiposCargas = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.TipoCarga = PropertyEntity({ type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarTipoDeCarga, idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTiposCargas() {
    _tiposCargas = new TiposCargas();
    KoBindings(_tiposCargas, "knockoutTiposCargas");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTiposCargasClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%" },
    ];

    _gridTiposCargas = new BasicDataTable(_tiposCargas.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_tiposCargas.TipoCarga, null, null, _gridTiposCargas);

    RecarregarGridTiposCargas();
}

function RecarregarGridTiposCargas() {

    var data = new Array();

    if (_tipoOperacao.TiposCargas.val() != "" && _tipoOperacao.TiposCargas.length > 0)
    {
        $.each(_tipoOperacao.TiposCargas.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridTiposCargas.CarregarGrid(data);
}

function ExcluirTiposCargasClick(data) {
    var tiposCargasGrid = _gridTiposCargas.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridTiposCargas.CarregarGrid(tiposCargasGrid);
}

function LimparCamposTiposCargas() {
    LimparCampos(_tiposCargas);
    _gridTiposCargas.CarregarGrid(new Array());
}