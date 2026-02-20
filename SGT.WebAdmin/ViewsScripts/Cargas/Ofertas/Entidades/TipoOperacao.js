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

var _gridTiposOperacao;
var _tiposOperacao;

var TipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid() });
}
function LoadTiposOperacao() {
    _tiposOperacao = new TipoOperacao();
    KoBindings(_tiposOperacao, "knockoutTipoOperacao");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirTiposOperacaoClick(data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
    ];

    _gridTiposOperacao = new BasicDataTable(_tiposOperacao.Grid.id, header, menuOpcoes);
    _tiposOperacao.TipoOperacao.basicTable = _gridTiposOperacao;

    new BuscarTiposOperacao(_tiposOperacao.TipoOperacao, null, null, null, _gridTiposOperacao);

    RecarregarGridTiposOperacao();
}

function RecarregarGridTiposOperacao() {

    let data = new Array();

    if (_parametrosOfertas.TiposOperacao.val() != "" && _parametrosOfertas.TiposOperacao.val().length > 0) {
        $.each(_parametrosOfertas.TiposOperacao.val(), function (i, tipoOp) {
            let tiposOpGrid = new Object();

            tiposOpGrid.Codigo = tipoOp.Codigo;
            tiposOpGrid.Descricao = tipoOp.Descricao;
            tiposOpGrid.CodigoRelacionamento = tipoOp.CodigoRelacionamento;

            data.push(tiposOpGrid);
        });
    }

    _gridTiposOperacao.CarregarGrid(data);
}

function PreencherTiposOperacao(listaTiposOperacaoRetornadas) {
    _parametrosOfertas.TiposOperacao.val(listaTiposOperacaoRetornadas);
    RecarregarGridTiposOperacao();
}

function ExcluirTiposOperacaoClick(data) {
    let tiposOperacaoGrid = _gridTiposOperacao.BuscarRegistros();

    for (let i = 0; i < tiposOperacaoGrid.length; i++) {
        if (data.Codigo == tiposOperacaoGrid[i].Codigo) {
            tiposOperacaoGrid.splice(i, 1);
            break;
        }
    }

    _gridTiposOperacao.CarregarGrid(tiposOperacaoGrid);
}

function LimparCamposTiposOperacao() {
    LimparCampos(_tiposOperacao);
    _gridTiposOperacao.CarregarGrid(new Array());
}