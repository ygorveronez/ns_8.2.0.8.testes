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
/// <reference path="BloqueioEmissaoPorNaoConformidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoOperacao;
var _tipoOperacao;

var TipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CadastrarTipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTipoOperacao() {
    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirRegraTipoOperacaoFiliaisClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Tipo Operação", width: "70%" },
    ];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacao.CadastrarTipoOperacao, null, null, null, _gridTipoOperacao);

    RecarregarGridTipoOperacao();
}

function RecarregarGridTipoOperacao() {
    var data = new Array();
    if (_bloqueioEmissaoPorNaoConformidade.TiposOperacao.val() != "") {
        $.each(_bloqueioEmissaoPorNaoConformidade.TiposOperacao.val(), function (i, tipoCarga) {
            var tiposTipoOperacaoGrid = new Object();

            tiposTipoOperacaoGrid.Codigo = tipoCarga.Codigo;
            tiposTipoOperacaoGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposTipoOperacaoGrid);
        });
    }
    _gridTipoOperacao.CarregarGrid(data);
}

function ExcluirRegraTipoOperacaoFiliaisClick(data) {
    var tiposTipoOperacaoGrid = _gridTipoOperacao.BuscarRegistros();

    for (var i = 0; i < tiposTipoOperacaoGrid.length; i++) {
        if (data.Codigo == tiposTipoOperacaoGrid[i].Codigo) {
            tiposTipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    _gridTipoOperacao.CarregarGrid(tiposTipoOperacaoGrid);
}

function LimparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
    _gridTipoOperacao.CarregarGrid(new Array());
}