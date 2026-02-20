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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoCarga;
var _tipoCarga;
var _tiposCarga = new Array();

var TipoCarga = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: "Adicionar Tipo De Carga", idBtnSearch: guid(), issue: 53 });
};

//*******EVENTOS*******

function LoadTipoCarga() {

    _tipoCarga = new TipoCarga();
    KoBindings(_tipoCarga, "knockoutTipoCarga");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoCargaClick(_tipoCarga.Tipo, data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridTipoCarga = new BasicDataTable(_tipoCarga.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarTiposdeCarga(_tipoCarga.Tipo, function (r) {
        if (r != null) {
            for (let i = 0; i < r.length; i++)
                _tiposCarga.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

            _gridTipoCarga.CarregarGrid(_tiposCarga);

            RecarregarOpcoesTipoCarga();
        }
    }, null, _gridTipoCarga);
    _tipoCarga.Tipo.basicTable = _gridTipoCarga;

    RecarregarGridTipoCarga();
}

function RecarregarGridTipoCarga() {
    _gridTipoCarga.CarregarGrid(_configuracaoToleranciaPesagem.TiposCarga.val());
    _tiposCarga = _configuracaoToleranciaPesagem.TiposCarga.val();
}

function ExcluirTipoCargaClick(knoutTipoCarga, data) {
    let tiposCarga = knoutTipoCarga.basicTable.BuscarRegistros();

    for (let i = 0; i < tiposCarga.length; i++) {
        if (data.Codigo == tiposCarga[i].Codigo) {
            tiposCarga.splice(i, 1);
            break;
        }
    }

    knoutTipoCarga.basicTable.CarregarGrid(tiposCarga);

    RecarregarOpcoesTipoCarga();
}

function LimparCamposTipoCarga() {
    LimparCampos(_tipoCarga);
    _gridTipoCarga.CarregarGrid(new Array());
    _tiposCarga = new Array();
}

function RecarregarOpcoesTipoCarga() {
    let tipoCargaGrid = _tipoCarga.Tipo.basicTable.BuscarRegistros();

    _opcoesTipoCarga = new Array();

    for (let i = 0; i < tipoCargaGrid.length; i++)
        _opcoesTipoCarga.push({ value: tipoCargaGrid[i].Codigo, text: tipoCargaGrid[i].Descricao });
}