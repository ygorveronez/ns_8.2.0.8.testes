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
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="TipoMovimentoArquivoContabil.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoMovimento;
var _tipoMovimento;

var TipoMovimento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoMovimento = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Movimento", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTipoMovimento() {
    _tipoMovimento = new TipoMovimento();
    KoBindings(_tipoMovimento, "knockoutTipoMovimento");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirTipoMovimentoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTipoMovimento = new BasicDataTable(_tipoMovimento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoMovimento(_tipoMovimento.TipoMovimento, null, null, null, null, null, _gridTipoMovimento);
    _tipoMovimento.TipoMovimento.basicTable = _gridTipoMovimento;

    RecarregarGridTipoMovimento();
}

function RecarregarGridTipoMovimento() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_tipoMovimentoArquivoContabil.TiposMovimentos.val())) {
        $.each(_tipoMovimentoArquivoContabil.TiposMovimentos.val(), function (i, tipoMovimento) {
            var tipoMovimentoGrid = new Object();

            tipoMovimentoGrid.Codigo = tipoMovimento.Codigo;
            tipoMovimentoGrid.Descricao = tipoMovimento.Descricao;

            data.push(tipoMovimentoGrid);
        });
    }
    _gridTipoMovimento.CarregarGrid(data);
}

function ExcluirTipoMovimentoClick(data) {
    var tipoMovimentoGrid = _tipoMovimento.TipoMovimento.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoMovimentoGrid.length; i++) {
        if (data.Codigo == tipoMovimentoGrid[i].Codigo) {
            tipoMovimentoGrid.splice(i, 1);
            break;
        }
    }

    _tipoMovimento.TipoMovimento.basicTable.CarregarGrid(tipoMovimentoGrid);
}

function LimparCamposTipoMovimento() {
    LimparCampos(_tipoMovimento);
    _tipoMovimento.TipoMovimento.basicTable.CarregarGrid(new Array());
}