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
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="ImportacaoProgramacaoColeta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridImportacaoPedido;
var _pesquisaImportacaoPedido;
var _CRUDImportacaoPedido;

var PesquisaImportacaoPedido = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Grid = PropertyEntity({ idGrid: guid() });
};

var CRUDImportacaoPedido = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparCamposClick, type: types.event, text: "Limpar Campos / Novo", visible: ko.observable(true) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "ImportacaoProgramacaoColeta/Importar",
        UrlConfiguracao: "ImportacaoProgramacaoColeta/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O049_ImportacaoProgramacaoColeta,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true,
                Codigo: _importacaoProgramacaoColeta.Codigo.val()
            };
        },
        CallbackImportacao: function () {
            _gridImportacaoPedido.CarregarGrid();
            _gridImportacaoProgramacaoColeta.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function LoadImportacaoPedido() {
    _CRUDImportacaoPedido = new CRUDImportacaoPedido();
    KoBindings(_CRUDImportacaoPedido, "knockoutCRUDImportacaoPedido");

    _pesquisaImportacaoPedido = new PesquisaImportacaoPedido();
    KoBindings(_pesquisaImportacaoPedido, "knockoutPesquisaImportacaoPedido", false);

    _gridImportacaoPedido = new GridView(_pesquisaImportacaoPedido.Grid.idGrid, "ImportacaoProgramacaoColeta/PesquisaCargas", _pesquisaImportacaoPedido, null, { column: 0, dir: orderDir.asc });
}

//*******MÉTODOS*******

function BuscarImportacaoPedido() {
    _pesquisaImportacaoPedido.Codigo.val(_importacaoProgramacaoColeta.Codigo.val());
    _gridImportacaoPedido.CarregarGrid();
}