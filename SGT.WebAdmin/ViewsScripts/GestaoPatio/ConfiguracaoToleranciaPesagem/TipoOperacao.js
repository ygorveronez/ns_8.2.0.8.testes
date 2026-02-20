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
/// <reference path="../../Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoOperacao;
var _tipoOperacao;
var _tiposOperacao = new Array();

var TipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: "Adicionar Tipo Operação", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadTipoOperacao() {

    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoOperacaoClick(_tipoOperacao.Tipo, data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarTiposOperacao(_tipoOperacao.Tipo, function (r) {
        if (r != null) {
            for (let i = 0; i < r.length; i++)
                _tiposOperacao.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

            _gridTipoOperacao.CarregarGrid(_tiposOperacao);

            RecarregarOpcoesTipoOperacao();
        }
    }, null, null, _gridTipoOperacao);
    _tipoOperacao.Tipo.basicTable = _gridTipoOperacao;

    RecarregarGridTipoOperacao();
}

function RecarregarGridTipoOperacao() {
    _gridTipoOperacao.CarregarGrid(_configuracaoToleranciaPesagem.TiposOperacao.val());
    _tiposOperacao = _configuracaoToleranciaPesagem.TiposOperacao.val();
}

function ExcluirTipoOperacaoClick(knoutTipoOperacao, data) {
    let tiposOperacao = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (let i = 0; i < tiposOperacao.length; i++) {
        if (data.Codigo == tiposOperacao[i].Codigo) {
            tiposOperacao.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tiposOperacao);

    RecarregarOpcoesTipoOperacao();
}

function LimparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
    _gridTipoOperacao.CarregarGrid(new Array());
    _tiposOperacao = new Array();
}

function RecarregarOpcoesTipoOperacao() {
    let tipoOperacaoGrid = _tipoOperacao.Tipo.basicTable.BuscarRegistros();

    _opcoesTipoOperacao = new Array();

    for (let i = 0; i < tipoOperacaoGrid.length; i++)
        _opcoesTipoOperacao.push({ value: tipoOperacaoGrid[i].Codigo, text: tipoOperacaoGrid[i].Descricao });
}