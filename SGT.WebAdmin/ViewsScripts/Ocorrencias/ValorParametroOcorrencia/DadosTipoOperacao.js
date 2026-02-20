/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="Dados.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _dadosTipoOperacao, _gridDadosTipoOperacao;

var DadosTipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo(s) de Operação", idBtnSearch: guid(), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadDadosTipoOperacao() {
    _dadosTipoOperacao = new DadosTipoOperacao();
    KoBindings(_dadosTipoOperacao, "knockoutDadosTipoOperacao");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirTipoOperacaoClick(_dadosTipoOperacao.TipoOperacao, data); } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Tipo de Operação", width: "80%" }
    ];
    _gridDadosTipoOperacao = new BasicDataTable(_dadosTipoOperacao.Grid.id, header, menuOpcoes);

    new BuscarTiposOperacao(_dadosTipoOperacao.TipoOperacao, function (r) {
        if (r != null) {
            var tiposOperacao = _gridDadosTipoOperacao.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                tiposOperacao.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao
                });

            _gridDadosTipoOperacao.CarregarGrid(tiposOperacao);
        }
    }, null, null, _gridDadosTipoOperacao);

    _dadosTipoOperacao.TipoOperacao.basicTable = _gridDadosTipoOperacao;

    RecarregarGridDadosTipoOperacao();
}

//*******MÉTODOS*******

function RecarregarGridDadosTipoOperacao() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_dados.TiposOperacao.val())) {
        $.each(_dados.TiposOperacao.val(), function (i, tipoOperacao) {
            var tipoOperacaoGrid = new Object();

            tipoOperacaoGrid.Codigo = tipoOperacao.TIPOOPERACAO.Codigo;
            tipoOperacaoGrid.Descricao = tipoOperacao.TIPOOPERACAO.Descricao;

            data.push(tipoOperacaoGrid);
        });
    }
    _gridDadosTipoOperacao.CarregarGrid(data);
}

function ExcluirTipoOperacaoClick(knoutTipoOperacao, data) {
    var tipoOperacaoGrid = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoOperacaoGrid.length; i++) {
        if (data.Codigo == tipoOperacaoGrid[i].Codigo) {
            tipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tipoOperacaoGrid);
}

function preencherListasSelecaoDadosTipoOperacao() {
    var tiposOperacao = new Array();

    $.each(_dadosTipoOperacao.TipoOperacao.basicTable.BuscarRegistros(), function (i, tipoOperacao) {
        tiposOperacao.push({ TIPOOPERACAO: tipoOperacao });
    });

    _dados.TiposOperacao.val(JSON.stringify(tiposOperacao));
}

function limparCamposDadosTipoOperacao() {
    LimparCampos(_dadosTipoOperacao);
    RecarregarGridDadosTipoOperacao();
}